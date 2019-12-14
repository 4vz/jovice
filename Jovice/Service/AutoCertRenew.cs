using Aphysoft.Share;
using Microsoft.Web.Administration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Jovice
{
    public static class AutoCertRenew
    {
        #region Methods

        public static void Init()
        {
            Timer timer = new Timer(new TimerCallback(delegate (object state)
            {
                Check();
            }), null, 0, 1000 * 3600); // hourly
        }
        
        private static void Check()
        {
            Database2 s = Web.Database;
            Result2 r = s.Query("select * from DomainController");

            string probeHost = null;
            string probeUser = null;
            string probePass = null;
            string probeSu = null;
            string probeGoDaddySSOKey = null;
            string probeCertPass = null;

            string cert = ConfigurationManager.AppSettings["cert"];

            if (cert != null)
            {
                string[] ctoks = cert.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string ctok in ctoks)
                {
                    string[] ports = ctok.Trim().Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);

                    if (ports.Length == 2)
                    {
                        string key = ports[0].ToLower();
                        string value = ports[1];

                        if (key == "host") probeHost = value;
                        else if (key == "user id") probeUser = value;
                        else if (key == "password") probePass = value;
                        else if (key == "su") probeSu = value;
                        else if (key == "godaddyssokey") probeGoDaddySSOKey = value;
                        else if (key == "certpass") probeCertPass = value;
                    }
                }
            }

            if (r == null || probeHost == null || probeUser == null || probePass == null || probeSu == null || probeGoDaddySSOKey == null || probeCertPass == null)
                return;

            Console.WriteLine("Checking...");

            List<Tuple<string, string, string>> domainCheck = new List<Tuple<string, string, string>>();

            X509Store storeCheck = null;
            ServerManager managerCheck = null;
            SiteCollection sitesCheck = null;

            foreach (Row2 row in r)
            {
                string id = row["DC_ID"].ToString();
                string domain = row["DC_Domain"].ToString();
                string baseDomain = row["DC_BaseDomain"].ToString();
                string hash = row["DC_CertHash"].ToString();
                string machine = row["DC_Machine"].ToString();
                DateTime? validUntil = row["DC_CertValidUntil"].ToDateTime();

                bool check = false;

                if (hash == null)
                {
                    check = true;
                    domainCheck.Add(new Tuple<string, string, string>(domain, baseDomain, id));
                }
                else
                {
                    TimeSpan span = validUntil.Value - DateTime.Now;

                    if (span.TotalDays < 29) // expired
                    {
                        check = true;
                        domainCheck.Add(new Tuple<string, string, string>(domain, baseDomain, id));
                    }
                    else if (machine == "local")
                    {
                        // check klo di store, ga ada certificate ini
                        if (storeCheck == null)
                        {
                            storeCheck = new X509Store(StoreName.My, StoreLocation.LocalMachine);
                            storeCheck.Open(OpenFlags.ReadWrite);
                        }

                        foreach (X509Certificate2 find in storeCheck.Certificates)
                        {
                            if (find.GetCertHashString() == hash)
                            {
                                // la ini apa?
                                if (find.FriendlyName != domain)
                                {
                                    // kenapa ga bener friendly namenya? benerin
                                    check = true;
                                    domainCheck.Add(new Tuple<string, string, string>(domain, baseDomain, id));
                                    break;
                                }
                            }
                        }
                    }
                }

                if (!check)
                {
                    if (machine == "local")
                    {
                        if (managerCheck == null)
                        {
                            managerCheck = new ServerManager();
                            sitesCheck = managerCheck.Sites;
                        }

                        Site site = sitesCheck[baseDomain];

                        if (site != null)
                        {
                            Binding remove = null;

                            // find binding, attach ssl
                            bool bindingFound = false;

                            foreach (Binding binding in site.Bindings)
                            {
                                if (binding.Protocol == "https" && binding.Host == domain)
                                {
                                    bindingFound = true;

                                    // check certificate
                                    string bindingCertHash = binding.CertificateHash.ToHex().ToUpper();

                                    if (bindingCertHash != hash)
                                    {
                                        // ga bener ini
                                        // remove aja
                                        remove = binding;
                                    }

                                    break;
                                }
                            }

                            bool add = false;

                            if (bindingFound)
                            {
                                if (remove != null)
                                {
                                    site.Bindings.Remove(remove);
                                    add = true;
                                }
                            }
                            else add = true;

                            if (add)
                            {
                                // add binding
                                // find certificate in store
                                X509Certificate2 certificate = null;
                                foreach (X509Certificate2 find in storeCheck.Certificates)
                                {
                                    if (find.GetCertHashString() == hash)
                                    {
                                        certificate = find;
                                        break;
                                    }
                                }

                                if (certificate != null)
                                { 
                                    site.Bindings.Add("*:443:" + domain, certificate.GetCertHash(), storeCheck.Name, SslFlags.Sni);
                                }
                                else
                                {
                                    // tapi certificate ga ada. Ok check aja
                                    check = true;
                                    domainCheck.Add(new Tuple<string, string, string>(domain, baseDomain, id));
                                }
                            }
                        }

                    }
                }
            }

            if (storeCheck != null)
                storeCheck.Close();
            if (managerCheck != null)
                managerCheck.CommitChanges();

            if (domainCheck.Count > 0)
            {
                CertRenewProbe probe = new CertRenewProbe(delegate (CertRenewProbe p)
                {
                    Console.WriteLine("Probe Done");

                    X509Store store = null;
                    ServerManager manager = null;                    
                    SiteCollection sites = null;

                    bool earthRestart = false;

                    foreach (Tuple<string, string, string, string> tuple in p.GetCertificates())
                    {
                        string baseDomain = tuple.Item1;
                        string domain = tuple.Item2;
                        string path = tuple.Item3;
                        string info = tuple.Item4;

                        // certificate
                        X509Certificate2 certificate = new X509Certificate2(path, probeCertPass, X509KeyStorageFlags.MachineKeySet);
                        certificate.FriendlyName = domain;
                        string certHash = certificate.GetCertHash().ToHex().ToUpper();

                        // database
                        string dcid = null;
                        foreach (Tuple<string, string, string> tuple2 in domainCheck)
                        {
                            string domain2 = tuple2.Item1;
                            string baseDomain2 = tuple2.Item2;

                            if (domain2 == domain && baseDomain2 == baseDomain)
                            {
                                dcid = tuple2.Item3;
                                break;
                            }
                        }
                        if (dcid == null) continue;

                        string machine = s.Scalar("select DC_Machine from DomainController where DC_ID = {0}", dcid).ToString();

                        // update CertHash / CertValidUntil
                        if (DateTime.TryParse(certificate.GetExpirationDateString(), out DateTime expire))
                        {
                            s.Execute("update DomainController set DC_CertHash = {0}, DC_CertValidUntil = {1} where DC_ID = {2}",
                                certHash, expire, dcid);
                        }

                        // check store, replace if different for specified domain
                        if (machine == "local")
                        {
                            if (store == null)
                            {
                                store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
                                store.Open(OpenFlags.ReadWrite);
                            }

                            bool alreadyInStore = false;

                            foreach (X509Certificate2 find in store.Certificates)
                            {
                                if (find.GetCertHashString() == certHash)
                                {
                                    // already in store
                                    alreadyInStore = true;
                                    break;
                                }
                            }

                            if (!alreadyInStore)
                            {
                                List<X509Certificate2> remove = new List<X509Certificate2>();

                                foreach (X509Certificate2 find in store.Certificates)
                                {
                                    if (find.FriendlyName == certificate.FriendlyName)
                                    {
                                        // probably expired, remove this
                                        remove.Add(find);
                                    }
                                    else
                                    {
                                        string[] tokens = find.Subject.Split(new char[] { '=' });
                                        if (tokens.Length == 2)
                                        {
                                            string subjectName = tokens[1].Trim();
                                            if (subjectName == domain)
                                            {
                                                // found the name domain, without friendly name, probably expired, remove this
                                                remove.Add(find);
                                            }
                                        }
                                    }
                                }

                                foreach (X509Certificate2 removeThis in remove)
                                {
                                    store.Remove(removeThis);
                                }
                            }

                            // add
                            store.Add(certificate);

                            // FIX BINDING TO THIS CERTIFICATE
                            if (manager == null)
                            {
                                manager = new ServerManager();
                                sites = manager.Sites;
                            }

                            Site site = sites[baseDomain];

                            if (site != null)
                            {
                                Binding remove = null;
                                Tuple<string, byte[], string> add = null;

                                // find binding, attach ssl
                                bool bindingFound = false;

                                foreach (Binding binding in site.Bindings)
                                {
                                    if (binding.Protocol == "https" && binding.Host == domain)
                                    {
                                        bindingFound = true;

                                        byte[] existingCertificateHash = binding.CertificateHash;

                                        if (!certificate.GetCertHash().SequenceEqual(existingCertificateHash))
                                        {
                                            remove = binding;
                                            add = new Tuple<string, byte[], string>(domain, certificate.GetCertHash(), store.Name);
                                        }

                                        break;
                                    }
                                }

                                if (bindingFound)
                                {
                                    if (remove != null)
                                    {
                                        site.Bindings.Remove(remove);
                                        site.Bindings.Add("*:443:" + add.Item1, add.Item2, add.Item3, SslFlags.Sni);
                                    }
                                }
                                else
                                {
                                    site.Bindings.Add("*:443:" + domain, certificate.GetCertHash(), store.Name, SslFlags.Sni);
                                }
                            }

                        }
                        else if (machine == "earth")
                        {
                            // earth
                            if (info == "renewed")
                            {
                                earthRestart = true;
                            }
                        }
                    }

                    if (store != null)
                        store.Close();
                    if (manager != null)
                        manager.CommitChanges();

                    if (earthRestart)
                    {
                        CertRenewProbe restartProbe = new CertRenewProbe(delegate (CertRenewProbe p2)
                        {
                        });

                        restartProbe.RestartApache();
                        restartProbe.Start(probeHost, probeUser, probePass, probeSu, probeGoDaddySSOKey, probeCertPass);
                    }
                });

                foreach (Tuple<string, string, string> tuple in domainCheck)
                {
                    string domain = tuple.Item1;
                    string baseDomain = tuple.Item2;

                    probe.Renew(domain, baseDomain);
                }

                probe.Start(probeHost, probeUser, probePass, probeSu, probeGoDaddySSOKey, probeCertPass);

            }
        }
        
        #endregion
    }

    internal delegate void CertEventHandler(CertRenewProbe probe);

    internal class CertRenewProbe : SshConnection
    {
        #region Fields

        private string task = null;

        private Queue<Tuple<string, string>> domains = new Queue<Tuple<string, string>>();

        private string host;
        private string user;
        private string pass;
        private string su;
        private string goDaddySSOKey;
        private string certPass;

        private CertEventHandler whenDone;

        private List<Tuple<string, string, string, string>> certificates = new List<Tuple<string, string, string, string>>();

        #endregion

        public CertRenewProbe(CertEventHandler handler)
        {
            whenDone = handler;
        }

        public void Start(string host, string user, string pass, string su, string goDaddySSOKey, string certPass)
        {
            this.host = host;
            this.user = user;
            this.pass = pass;
            this.su = su;
            this.goDaddySSOKey = goDaddySSOKey;
            this.certPass = certPass;

            Start(host, user, pass);
        }

        public void RestartApache()
        {
            task = "restartApache";
        }

        public void Renew(string domain, string baseDomain)
        {
            lock (domains)
            {
                domains.Enqueue(new Tuple<string, string>(domain, baseDomain));
            }

            task = "renew";
        }

        public Tuple<string, string, string, string>[] GetCertificates()
        {
            if (certificates != null)
            {
                return certificates.ToArray();
            }
            else return null;
        }

        protected override void OnConnected()
        {
        }

        protected override void OnDisconnected()
        {
            whenDone(this);
        }

        protected override void OnProcess()
        {
            if (task == "renew")
            {
                while (domains.Count > 0)
                {
                    string domain = null;
                    string baseDomain = null;

                    lock (domains)
                    {
                        Tuple<string, string> tuple = domains.Dequeue();

                        domain = tuple.Item1;
                        baseDomain = tuple.Item2;
                    }

                    bool notChanged = false;
                    bool suspended = false;

                    string domainChallenge = null;
                    string txtValue = null;

                    string[] lines;

                    if (Request("acme/acme.sh --issue --dns -d " + domain, out lines)) SessionFailure();

                    Console.WriteLine("----------" + domain + "---------");
                    foreach (string line in lines)
                    {
                        if (line.IndexOf("Domains not changed.") > -1)
                        {
                            notChanged = true;
                            break;
                        }
                        else if (line.IndexOf("many failed authorizations recently") > -1)
                        {
                            suspended = true;
                            break;
                        }
                        else if (line.IndexOf("Domain: ") > -1)
                        {
                            domainChallenge = line.Substring(line.IndexOf("Domain: ") + 8).Trim(new char[] { '\'' });
                        }
                        else if (line.IndexOf("TXT value: ") > -1)
                        {
                            txtValue = line.Substring(line.IndexOf("TXT value: ") + 11).Trim(new char[] { '\'' });
                        }
                    }

                    if (suspended)
                    {
                        Console.WriteLine("SUSPENDED");
                    }
                    else
                    {
                        string info = null;

                        if (!notChanged)
                        {
                            if (domainChallenge != null && txtValue != null)
                            {
                                // front end
                                string domainFrontEnd = domainChallenge.Substring(0, domainChallenge.Length - baseDomain.Length).Trim(new char[] { '.' });

                                Console.WriteLine(domainFrontEnd + " " + txtValue);

                                HttpClient client = new HttpClient();

                                string content = "[{\"data\": \"" + txtValue + "\", \"ttl\": " + Aphysoft.Share.Rnd.Int(600, 650) + "}]";

                                var request = new HttpRequestMessage()
                                {
                                    RequestUri = new Uri("https://api.godaddy.com/v1/domains/" + baseDomain + "/records/TXT/" + domainFrontEnd),
                                    Method = HttpMethod.Put,
                                    Content = new StringContent(content, Encoding.UTF8, "application/json")
                                };

                                request.Headers.Add("Authorization", "sso-key " + goDaddySSOKey);

                                HttpResponseMessage response = client.SendAsync(request).Result;

                                Console.WriteLine("RESPONSE: " + response.StatusCode);

                                Console.WriteLine("Waiting for 60 seconds");
                                Thread.Sleep(60000);

                                if (Request("acme/acme.sh --renew --dns -d " + domain, out lines)) SessionFailure();

                                bool success = false;
                                bool notCorrect = false;
                                bool dnsProblem = false;
                                bool challengeError = false;

                                foreach (string line in lines)
                                {
                                    if (line.IndexOf("Success") > -1)
                                    {
                                        success = true;
                                        info = "renewed";
                                        break;
                                    }
                                    else if (line.IndexOf("DNS problem: NXDOMAIN") > -1)
                                    {
                                        dnsProblem = true;
                                        break;
                                    }
                                    else if (line.IndexOf("Verify error:Correct value not found") > -1)
                                    {
                                        notCorrect = true;
                                        break;
                                    }
                                    else if (line.IndexOf(":Challenge error:") > -1)
                                    {
                                        challengeError = true;
                                        break;
                                    }
                                }

                                if (!success)
                                {
                                    Console.WriteLine("NOT SUCCESS");

                                    if (notCorrect)
                                    {
                                        Console.WriteLine("Not Correct DNS Value");
                                    }
                                    else if (dnsProblem)
                                    {
                                        Console.WriteLine("DNS Problem");
                                    }
                                    else if (challengeError)
                                    {
                                        Console.WriteLine("Challenge Error");
                                    }
                                    else
                                    {
                                        foreach (string line in lines)
                                        {
                                            Console.WriteLine(line);
                                        }
                                    }

                                    // avoid retrive cert
                                    continue;
                                }
                                else
                                {
                                    Console.WriteLine("SUCCESS");

                                    if (Request("openssl pkcs12 -export -out acme/" + domain + "/" + domain + ".pfx -inkey acme/" + domain + "/" + domain + ".key -in acme/" + domain + "/" + domain + ".cer -password pass:" + certPass, out lines)) SessionFailure();
                                }
                            }
                        }

                        // attempt to retrieve cert
                        string pfx = domain + ".pfx";
                        string localFile = System.IO.Path.GetTempPath() + pfx;

                        if (SFTP.Get(host, user, pass, "acme/" + domain + "/" + pfx, localFile))
                        {
                            certificates.Add(new Tuple<string, string, string, string>(baseDomain, domain, localFile, info));
                        }
                    }
                }
            }
            else if (task == "restartApache")
            {
                SendLine("su");
                WaitUntilEndsWith("Password:");
                SendLine(su);
                WaitUntilEndsWith("# ");
                SendLine("/etc/init.d/apache2 restart");
                WaitUntilEndsWith("# ");
                SendLine("exit");
                WaitUntilTerminalReady();

                Console.WriteLine("Done restart");
            }

            Stop();
        }
    }
}
