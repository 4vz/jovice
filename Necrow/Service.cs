using Aphysoft.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Necrow
{
    internal class Service
    {
        #region Constants

        private static readonly string[] monthsEnglish = new string[] { "JANUARY", "FEBRUARY", "MARCH", "APRIL", "MAY", "JUNE", "JULY", "AUGUST", "SEPTEMBER", "OCTOBER", "NOVEMBER", "DECEMBER" };
        private static readonly string[] monthsBahasa = new string[] { "JANUARI", "FEBRUARI", "MARET", "APRIL", "MEI", "JUNI", "JULI", "AGUSTUS", "SEPTEMBER", "OKTOBER", "NOVEMBER", "DESEMBER" };

        private static readonly Regex findSiteID = new Regex(@"([A-Z][A-Z][A-Z][0-9][0-9][0-9])(?:MW\d)?");
        private static readonly Regex findSiteID2 = new Regex(@"^([A-Z][A-Z][A-Z][0-9][0-9][0-9])(?:MW\d)?");
        private static readonly Regex captureSIDFormat1 = new Regex(@"((?:\d+(?:-\d{7,})+)|(?:\d{7,}(?:-\d{5,})+)|(?:(?:47|30|37)\d{15})|(?:^\d{9,255}\s))");

        #endregion

        #region Fields

        public string VID { get; private set; }

        public string ServiceType { get; private set; }

        #endregion

        #region Methods

        public static Service Parse(string description)
        {
            Service de = new Service();

            string o = string.Join(" ", description.Split(new char[] { ' ', ';', '#', '.', ',', '`', '\'', '"', '^', '$', '@', '?', '/', '\\' }, StringSplitOptions.RemoveEmptyEntries)).ToUpper();
            string d = string.Join(" ", o.Split(new char[] { '-', '_' }));

            #region Find SID

            int rmv = -1;
            int rle = -1;

            //                         12345678901234567890
            if ((rmv = d.IndexOf("MM IPVPN INSTAN ")) > -1) { de.ServiceType = "VPNINSTAN"; rle = 15; }
            else if ((rmv = d.IndexOf("MM VPNIP INSTAN ")) > -1) { de.ServiceType = "VPNINSTAN"; rle = 15; }
            else if ((rmv = d.IndexOf("VPNIP INSTANT ")) > -1) { de.ServiceType = "VPNINSTAN"; rle = 13; }
            else if ((rmv = d.IndexOf("IPVPN INSTANT ")) > -1) { de.ServiceType = "VPNINSTAN"; rle = 13; }
            else if ((rmv = d.IndexOf("VPNIP INSTAN ")) > -1) { de.ServiceType = "VPNINSTAN"; rle = 12; }
            else if ((rmv = d.IndexOf("IPVPN INSTAN ")) > -1) { de.ServiceType = "VPNINSTAN"; rle = 12; }
            else if ((rmv = d.IndexOf("VPNIP VPN IP ")) > -1) { de.ServiceType = "VPNIP"; rle = 12; }
            else if ((rmv = d.IndexOf("VPNIP VPNIP ")) > -1) { de.ServiceType = "VPNIP"; rle = 11; }
            else if ((rmv = d.IndexOf("VPN INSTAN ")) > -1) { de.ServiceType = "VPNINSTAN"; rle = 10; }
            else if ((rmv = d.IndexOf("MM IPVPN")) > -1) { de.ServiceType = "VPNIP"; rle = 8; }
            else if ((rmv = d.IndexOf("MM VPNIP")) > -1) { de.ServiceType = "VPNIP"; rle = 8; }
            else if ((rmv = d.IndexOf("MM VPNIP")) > -1) { de.ServiceType = "VPNIP"; rle = 8; }
            else if ((rmv = d.IndexOf("MM VPNIP")) > -1) { de.ServiceType = "VPNIP"; rle = 8; }
            else if ((rmv = d.IndexOf("VPN IP ")) > -1) { de.ServiceType = "VPNIP"; rle = 6; }
            else if ((rmv = d.IndexOf("IP VPN ")) > -1) { de.ServiceType = "VPNIP"; rle = 6; }
            else if ((rmv = d.IndexOf("VPNIP")) > -1) { de.ServiceType = "VPNIP"; rle = 5; }
            else if ((rmv = d.IndexOf("IPVPN")) > -1) { de.ServiceType = "VPNIP"; rle = 5; }
            //                         12345678901234567890
            else if ((rmv = d.IndexOf("MM ASTINET BEDA BW ")) > -1) { de.ServiceType = "ASTINETBB"; rle = 18; }
            else if ((rmv = d.IndexOf("MM ASTINET BEDABW ")) > -1) { de.ServiceType = "ASTINETBB"; rle = 17; }
            else if ((rmv = d.IndexOf("ASTINET BEDA BW ")) > -1) { de.ServiceType = "ASTINETBB"; rle = 15; }
            else if ((rmv = d.IndexOf("ASTINET BEDABW ")) > -1) { de.ServiceType = "ASTINETBB"; rle = 14; }
            else if ((rmv = d.IndexOf("MM ASTINET BB ")) > -1) { de.ServiceType = "ASTINETBB"; rle = 13; }
            else if ((rmv = d.IndexOf("ASTINET BB ")) > -1) { de.ServiceType = "ASTINETBB"; rle = 10; }
            else if ((rmv = d.IndexOf("ASTINETBB ")) > -1) { de.ServiceType = "ASTINETBB"; rle = 9; }
            else if ((rmv = d.IndexOf("AST BEDA BW ")) > -1) { de.ServiceType = "ASTINETBB"; rle = 11; }
            else if ((rmv = d.IndexOf("AST BEDABW ")) > -1) { de.ServiceType = "ASTINETBB"; rle = 10; }
            else if ((rmv = d.IndexOf("AST BB ")) > -1) { de.ServiceType = "ASTINETBB"; rle = 7; }
            //                         12345678901234567890
            else if ((rmv = d.IndexOf("MM ASTINET")) > -1) { de.ServiceType = "ASTINET"; rle = 10; }
            else if ((rmv = d.IndexOf("ASTINET")) > -1) { de.ServiceType = "ASTINET"; rle = 7; }
            //                         12345678901234567890
            else if ((rmv = d.IndexOf("TRANSIT")) > -1) { de.ServiceType = "IPTRANSIT"; rle = 7; }
            //                         12345678901234567890123
            else if ((rmv = d.IndexOf("MM METRO ETHERNET MP2MP")) > -1) { de.ServiceType = "METRO"; rle = 23; }
            else if ((rmv = d.IndexOf("MM METRO ETHERNET P2MP")) > -1) { de.ServiceType = "METRO"; rle = 22; }
            else if ((rmv = d.IndexOf("MM METRO ETHERNET P2P")) > -1) { de.ServiceType = "METRO"; rle = 21; }
            else if ((rmv = d.IndexOf("MM METRO ETHERNETMP2MP")) > -1) { de.ServiceType = "METRO"; rle = 22; }
            else if ((rmv = d.IndexOf("MM METRO ETHERNETP2MP")) > -1) { de.ServiceType = "METRO"; rle = 21; }
            else if ((rmv = d.IndexOf("MM METRO ETHERNETP2P")) > -1) { de.ServiceType = "METRO"; rle = 20; }
            else if ((rmv = d.IndexOf("MM METRO ETHERNET")) > -1) { de.ServiceType = "METRO"; rle = 17; }
            else if ((rmv = d.IndexOf("MM METROETHERNET")) > -1) { de.ServiceType = "METRO"; rle = 16; }
            else if ((rmv = d.IndexOf("MM METRO MP2MP")) > -1) { de.ServiceType = "METRO"; rle = 14; }
            else if ((rmv = d.IndexOf("MM METRO P2MP")) > -1) { de.ServiceType = "METRO"; rle = 13; }
            else if ((rmv = d.IndexOf("MM METRO P2P")) > -1) { de.ServiceType = "METRO"; rle = 12; }
            else if ((rmv = d.IndexOf("MM METRO")) > -1) { de.ServiceType = "METRO"; rle = 8; }
            //                         12345678901234567890123
            else if ((rmv = d.IndexOf("METRO ETHERNET MP2MP")) > -1) { de.ServiceType = "METRO"; rle = 20; }
            else if ((rmv = d.IndexOf("METRO ETHERNET P2MP")) > -1) { de.ServiceType = "METRO"; rle = 19; }
            else if ((rmv = d.IndexOf("METRO ETHERNET P2P")) > -1) { de.ServiceType = "METRO"; rle = 18; }
            else if ((rmv = d.IndexOf("METRO ETHERNETMP2MP")) > -1) { de.ServiceType = "METRO"; rle = 19; }
            else if ((rmv = d.IndexOf("METRO ETHERNETP2MP")) > -1) { de.ServiceType = "METRO"; rle = 18; }
            else if ((rmv = d.IndexOf("METRO ETHERNETP2P")) > -1) { de.ServiceType = "METRO"; rle = 17; }
            else if ((rmv = d.IndexOf("METRO ETHERNET")) > -1) { de.ServiceType = "METRO"; rle = 14; }
            else if ((rmv = d.IndexOf("METROETHERNET")) > -1) { de.ServiceType = "METRO"; rle = 13; }
            else if ((rmv = d.IndexOf("METRO MP2MP")) > -1) { de.ServiceType = "METRO"; rle = 11; }
            else if ((rmv = d.IndexOf("METRO P2MP")) > -1) { de.ServiceType = "METRO"; rle = 10; }
            else if ((rmv = d.IndexOf("METRO P2P")) > -1) { de.ServiceType = "METRO"; rle = 9; }
            else if ((rmv = d.IndexOf("METRO ")) > -1) { de.ServiceType = "METRO"; rle = 5; }
            else rmv = -1;

            if (rmv > -1)
            {
                d = d.Remove(rmv, rle);
                o = o.Remove(rmv, rle);

                d = d.Insert(rmv, " ");
                o = o.Insert(rmv, " ");
            }
            rmv = -1;
            rle = -1;

            if (de.ServiceType == null || de.ServiceType == "VPNIP")
            {
                //                         12345678901234567890
                if ((rmv = d.IndexOf("VPNIP TRANS ACCESS")) > -1) { rle = 18; }
                else if ((rmv = d.IndexOf("TRANSACTIONAL ACCESS")) > -1) { rle = 20; }
                else if ((rmv = d.IndexOf("MM TRANS ACCESS")) > -1) { rle = 15; }
                else if ((rmv = d.IndexOf("TRANSS ACCESS")) > -1) { rle = 13; }
                else if ((rmv = d.IndexOf("TRANS ACCESS")) > -1) { rle = 12; }
                else if ((rmv = d.IndexOf("TRANSACCESS")) > -1) { rle = 11; }
                else if ((rmv = d.IndexOf("TRANS ACCES")) > -1) { rle = 11; }
                else if ((rmv = d.IndexOf("TRANSACC")) > -1) { rle = 8; }
                else if ((rmv = d.IndexOf("TRAN ACC")) > -1) { rle = 8; }
                else if ((rmv = d.IndexOf("TRANS AC")) > -1) { rle = 8; }
                else if ((rmv = d.IndexOf("TRANSAC")) > -1) { rle = 7; }
                else if ((rmv = d.IndexOf("TRANSC ")) > -1) { rle = 6; }
                else if ((rmv = d.IndexOf("TRANSS ")) > -1) { rle = 6; }
                else rmv = -1;

                if (rle > -1)
                {
                    de.ServiceType = "TRANS";
                }
            }

            if (rmv > -1)
            {
                d = d.Remove(rmv, rle);
                o = o.Remove(rmv, rle);

                d = d.Insert(rmv, " ");
                o = o.Insert(rmv, " ");
            }

            rmv = -1;
            rle = -1;

            //d = d.Trim();
            //o = o.Trim();

            //                         12345678901234567890
            if ((rmv = d.IndexOf(" BENTROK DENGAN ")) > -1) { rle = 16; }
            else if ((rmv = d.IndexOf("(EX SID FEAS")) > -1) { rle = 12; }
            else if ((rmv = d.IndexOf("[EX SID FEAS")) > -1) { rle = 12; }
            else if ((rmv = d.IndexOf("EX SID FEAS")) > -1) { rle = 11; }
            else if ((rmv = d.IndexOf("EX SID FEAS")) > -1) { rle = 11; }
            else if ((rmv = d.IndexOf("SID (FEAS)")) > -1) { rle = 10; }
            else if ((rmv = d.IndexOf("SID [FEAS]")) > -1) { rle = 10; }
            else if ((rmv = d.IndexOf("SID <FEAS>")) > -1) { rle = 10; }
            else if ((rmv = d.IndexOf("(SID FEAS")) > -1) { rle = 9; }
            else if ((rmv = d.IndexOf("[SID FEAS")) > -1) { rle = 9; }
            else if ((rmv = d.IndexOf("SID FEAS")) > -1) { rle = 8; }
            else if ((rmv = d.IndexOf("(EX FEAS")) > -1) { rle = 8; }
            else if ((rmv = d.IndexOf("[EX FEAS")) > -1) { rle = 8; }
            else if ((rmv = d.IndexOf("(EX SID")) > -1) { rle = 7; }
            else if ((rmv = d.IndexOf("[EX SID")) > -1) { rle = 7; }
            else if ((rmv = d.IndexOf("EX FEAS")) > -1) { rle = 7; }
            else if ((rmv = d.IndexOf("EX SID")) > -1) { rle = 6; }
            else if ((rmv = d.IndexOf("X SID")) > -1) { rle = 5; }
            else if ((rmv = d.IndexOf("EXSID")) > -1) { rle = 5; }
            else if ((rmv = d.IndexOf("XSID3")) > -1) { rle = 4; }
            else if ((rmv = d.IndexOf("XSID4")) > -1) { rle = 4; }
            else if ((rmv = d.IndexOf("XSID ")) > -1) { rle = 4; }
            else if ((rmv = d.IndexOf("FEAS ")) > -1) { rle = 4; }
            else if ((rmv = d.IndexOf("SSID ")) > -1) { rle = 4; }
            else if ((rmv = d.IndexOf("VLAN ")) > -1) { rle = 4; }
            else if ((rmv = d.IndexOf(" EX3")) > -1) { rle = 3; }
            else if ((rmv = d.IndexOf(" EX4")) > -1) { rle = 3; }
            else if ((rmv = d.IndexOf("(EX3")) > -1) { rle = 3; }
            else if ((rmv = d.IndexOf("(EX4")) > -1) { rle = 3; }
            else if ((rmv = d.IndexOf("[EX3")) > -1) { rle = 3; }
            else if ((rmv = d.IndexOf("[EX4")) > -1) { rle = 3; }
            else if ((rmv = d.IndexOf("<EX3")) > -1) { rle = 3; }
            else if ((rmv = d.IndexOf("<EX4")) > -1) { rle = 3; }

            if (rmv > -1)
            {
                int rmvn = rmv + rle;
                if (rmvn < d.Length)
                {
                    if (d[rmvn] == ' ') rmvn += 1;
                    else if (d[rmvn] == ':' || d[rmvn] == '-' || d[rmvn] == '=')
                    {
                        rmvn += 1;
                        if (rmvn < d.Length && d[rmvn] == ' ') rmvn += 1;
                    }
                }
                if (rmvn < d.Length)
                {
                    int end = d.IndexOfAny(new char[] { ' ', ')', '(', ']', '[', '.', '<', '>' }, rmvn);
                    if (end > -1)
                    {
                        d = d.Remove(rmv, end - rmv);
                        o = o.Remove(rmv, end - rmv);
                    }
                    else
                    {
                        d = d.Remove(rmv);
                        o = o.Remove(rmv);
                    }

                    d = d.Insert(rmv, " ");
                    o = o.Insert(rmv, " ");
                }
            }
            rmv = -1;
            rle = -1;

            // DENGAN IDENTIFIKASI "SID"
            if (de.VID == null)
            {
                //                         12345678901234567890
                if ((rmv = d.IndexOf("SID TENOSS ")) > -1) { rle = 11; }
                else if ((rmv = d.IndexOf("SID TENOSS:")) > -1) { rle = 11; }
                else if ((rmv = d.IndexOf("SID TENOSS=")) > -1) { rle = 11; }
                else if ((rmv = d.IndexOf("TENOSS SID ")) > -1) { rle = 11; }
                else if ((rmv = d.IndexOf("TENOSS SID:")) > -1) { rle = 11; }
                else if ((rmv = d.IndexOf("TENOSS SID=")) > -1) { rle = 11; }
                else if ((rmv = d.IndexOf("SID SID ")) > -1) { rle = 7; }
                else if ((rmv = d.IndexOf("TENOSS:")) > -1) { rle = 7; }
                else if ((rmv = d.IndexOf(" SOID ")) > -1) { rle = 6; }
                else if ((rmv = d.IndexOf("MSID ")) > -1) { rle = 5; }
                else if ((rmv = d.IndexOf("(SID ")) > -1) { rle = 5; }
                else if ((rmv = d.IndexOf("(SID:")) > -1) { rle = 5; }
                else if ((rmv = d.IndexOf("(SID=")) > -1) { rle = 5; }
                else if ((rmv = d.IndexOf("(SID%")) > -1) { rle = 5; }
                else if ((rmv = d.IndexOf("<SID:")) > -1) { rle = 5; }
                else if ((rmv = d.IndexOf("<SID=")) > -1) { rle = 5; }
                else if ((rmv = d.IndexOf("<SID%")) > -1) { rle = 5; }
                else if ((rmv = d.IndexOf("<SID ")) > -1) { rle = 5; }
                else if ((rmv = d.IndexOf("[SID:")) > -1) { rle = 5; }
                else if ((rmv = d.IndexOf("[SID=")) > -1) { rle = 5; }
                else if ((rmv = d.IndexOf("[SID%")) > -1) { rle = 5; }
                else if ((rmv = d.IndexOf("[SID ")) > -1) { rle = 5; }
                else if ((rmv = d.IndexOf(" SID:")) > -1) { rle = 5; }
                else if ((rmv = d.IndexOf(" SID=")) > -1) { rle = 5; }
                else if ((rmv = d.IndexOf(" SID%")) > -1) { rle = 5; }
                else if ((rmv = d.IndexOf(" SID ")) > -1) { rle = 5; }
                else if ((rmv = d.IndexOf(" SIDT")) > -1) { rle = 4; }
                else if ((rmv = d.IndexOf(" SID0")) > -1) { rle = 4; }
                else if ((rmv = d.IndexOf(" SID1")) > -1) { rle = 4; }
                else if ((rmv = d.IndexOf(" SID2")) > -1) { rle = 4; }
                else if ((rmv = d.IndexOf(" SID3")) > -1) { rle = 4; }
                else if ((rmv = d.IndexOf(" SID4")) > -1) { rle = 4; }
                else if ((rmv = d.IndexOf(" SID5")) > -1) { rle = 4; }
                else if ((rmv = d.IndexOf(" SID6")) > -1) { rle = 4; }
                else if ((rmv = d.IndexOf(" SID7")) > -1) { rle = 4; }
                else if ((rmv = d.IndexOf(" SID8")) > -1) { rle = 4; }
                else if ((rmv = d.IndexOf(" SID9")) > -1) { rle = 4; }
                else if ((rmv = d.IndexOf("SID.")) > -1) { rle = 4; }
                else if ((rmv = d.IndexOf("SID:")) > -1) { rle = 4; }
                else if ((rmv = d.IndexOf("SID=")) > -1) { rle = 4; }
                else if ((rmv = d.IndexOf("SID%")) > -1) { rle = 4; }
                else if ((rmv = d.IndexOf("SID ")) > -1) { rle = 4; }
                // ITS A HELL DOWN HERE
                else if ((rmv = d.IndexOf(" TELKOM FL")) > -1) { rle = 1; }
                else if ((rmv = d.IndexOf(" INTG")) > -1) { rle = 1; }
                else rmv = -1;

                if (rmv > -1)
                {
                    int rmvn = rmv + rle;
                    if (rmvn < d.Length)
                    {
                        if (d[rmvn] == ' ') rmvn += 1;
                        else if (d[rmvn] == ':' || d[rmvn] == '-' || d[rmvn] == '=' || d[rmvn] == '(' || d[rmvn] == '[')
                        {
                            rmvn += 1;
                            if (rmvn < d.Length && d[rmvn] == ' ') rmvn += 1;
                        }
                    }
                    if (rmvn < d.Length)
                    {
                        int end = -1;
                        int nextend = rmvn;

                        while (true)
                        {
                            end = o.IndexOfAny(new char[] { ' ', ')', '(', ']', '[', '.', '<', '>', '_' }, nextend);
                            if (end > -1 && end < d.Length && d[end] == ' ' && end - rmvn <= 8) nextend = end + 1;
                            else if (end > -1 && end < d.Length && d[end] == '_' && end - rmvn <= 8) nextend = end + 1;
                            else break;
                        }

                        if (end > -1)
                        {
                            int len = end - rmv - rle;

                            if (len + rmvn > d.Length) de.VID = o.Substring(rmvn).Trim();
                            else de.VID = o.Substring(rmvn, len).Trim();

                            d = d.Remove(rmv, end - rmv);
                            o = o.Remove(rmv, end - rmv);
                        }
                        else
                        {
                            string imx = o.Substring(rmvn).Trim();
                            //imx = imx.Replace(' ', '_');

                            if (imx.Length > 13)
                            {
                                StringBuilder nimx = new StringBuilder();
                                nimx.Append(imx.Substring(0, 13));
                                for (int imxi = 13; imxi < imx.Length; imxi++)
                                {
                                    if (char.IsDigit(imx[imxi])) nimx.Append(imx[imxi]);
                                    else break;
                                }

                                imx = nimx.ToString();
                            }

                            de.VID = imx;

                            d = d.Remove(rmv);
                            o = o.Remove(rmv);
                        }
                    }
                }
            }

            // DENGAN SID FORMAT
            if (de.VID == null)
            {
                Match m = captureSIDFormat1.Match(o);

                if (m.Success)
                {
                    string sid = m.Groups[0].Value;

                    int idx = o.IndexOf(sid);

                    if (idx > -1)
                    {
                        o = o.Remove(idx, sid.Length);
                        d = d.Remove(idx, sid.Length);

                        o = o.Insert(idx, " ");
                        d = d.Insert(idx, " ");

                        de.VID = sid;
                    }
                }
            }

            // HEURISTIC SEARCH
            if (de.VID == null)
            {
                List<string> sidc = new List<string>();

                List<string> sss = new List<string>();

                foreach (string si in o.Split(new char[] { ' ', ':', '=' }))
                {
                    string[] sis = si.Split(new char[] { '_' }, 8);

                    sss.AddRange(sis);
                }

                foreach (string si in sss)
                {
                    int dig = 0;

                    string fsi = si.Trim(new char[] { '-', ')', '(', '[', ']', '>', '<' });

                    // count digit in si
                    foreach (char ci in fsi)
                        if (char.IsDigit(ci))
                            dig++;

                    double oc = (double)dig / (double)fsi.Length;

                    if (oc > 0.3 && fsi.Length > 8 &&
                        !fsi.StartsWith("FAA-") &&
                        !fsi.StartsWith("FAI-") &&
                        !fsi.StartsWith("FAD-") &&
                        !fsi.StartsWith("GI") &&
                        !fsi.StartsWith("TE") &&
                        !fsi.StartsWith("FA") &&
                        fsi.IndexOf("GBPS") == -1 &&
                        fsi.IndexOf("KBPS") == -1 &&
                        fsi.IndexOf("MBPS") == -1 &&
                        fsi.IndexOf("BPS") == -1 &&
                        fsi.IndexOfAny(new char[] { '/', '.', ';', '\'', '\"', '>', '<', '/' }) == -1)
                    {
                        int imin = fsi.LastIndexOf('-');

                        if (imin > -1)
                        {
                            string lastport = fsi.Substring(imin + 1);

                            if (lastport.Length < 5) fsi = null;
                            else
                            {
                                bool adadigit = false;
                                foreach (char lastportc in lastport)
                                {
                                    if (char.IsDigit(lastportc))
                                    {
                                        adadigit = true;
                                        break;
                                    }
                                }

                                if (adadigit == false)
                                    fsi = null;
                            }
                        }

                        if (fsi != null)
                        {
                            if (fsi.Length >= 6 && fsi.Length <= 16)
                            {
                                bool probablySID = true;

                                string[] fsip = fsi.Split(new char[] { '-' });
                                if (fsip.Length == 3)
                                {
                                    string first = fsip[0];
                                    if (char.IsDigit(first[0]))
                                    {
                                        if (first.Length == 1 || first.Length == 2 && char.IsDigit(first[1])) { }
                                        else probablySID = false;
                                    }
                                    if (probablySID && !char.IsDigit(first[0]))
                                    {
                                        if (first.Length >= 3 && (
                                            ListHelper.StartsWith(monthsEnglish, first) > -1 ||
                                            ListHelper.StartsWith(monthsBahasa, first) > -1
                                            ))
                                        { }
                                        else probablySID = false;
                                    }
                                    string second = fsip[1];
                                    if (probablySID && char.IsDigit(second[0]))
                                    {
                                        if (second.Length == 1 || second.Length == 2 && char.IsDigit(second[1])) { }
                                        else probablySID = false;
                                    }
                                    if (probablySID && !char.IsDigit(second[0]))
                                    {
                                        if (second.Length >= 3 && (
                                            ListHelper.StartsWith(monthsEnglish, second) > -1 ||
                                            ListHelper.StartsWith(monthsBahasa, second) > -1
                                            ))
                                        { }
                                        else probablySID = false;
                                    }
                                    string third = fsip[2];
                                    if (probablySID && char.IsDigit(second[0]))
                                    {
                                        if (third.Length == 2 && char.IsDigit(third[1])) { }
                                        else if (third.Length == 4 && char.IsDigit(third[1]) && char.IsDigit(third[2]) && char.IsDigit(third[3])) { }
                                        else probablySID = false;
                                    }
                                    else probablySID = false;
                                }
                                else if (fsip.Length == 1)
                                {
                                    // 04APR2014
                                    // APR42014
                                    // 4APR2014
                                    // 04042014
                                    if (char.IsDigit(fsi[0])) { }
                                    else
                                    {
                                        int tlen = 1;
                                        for (int fi = 1; fi < fsi.Length; fi++)
                                        {
                                            if (char.IsDigit(fsi[fi])) break;
                                            else tlen++;
                                        }

                                        string t = fsi.Substring(0, tlen);

                                        if (ListHelper.StartsWith(monthsEnglish, t) > -1 ||
                                            ListHelper.StartsWith(monthsBahasa, t) > -1)
                                        { }
                                        else probablySID = false;

                                        if (probablySID && fsi.Length > tlen)
                                        {
                                            int remaining = fsi.Length - tlen;
                                            if (remaining >= 3 && remaining <= 6)
                                            {
                                                for (int ooi = 0; ooi < remaining; ooi++)
                                                {
                                                    char cc = fsi[tlen + ooi];
                                                    if (!char.IsDigit(cc))
                                                    {
                                                        probablySID = false;
                                                        break;
                                                    }
                                                }
                                            }
                                            else probablySID = false;
                                        }
                                    }

                                }
                                else probablySID = false;

                                if (probablySID) fsi = null;

                            }
                        }

                        if (fsi != null)
                        {
                            sidc.Add(fsi);
                        }
                    }
                }

                sss.Clear();


                if (sidc.Count > 0)
                {
                    sidc.Sort((a, b) => b.Length.CompareTo(a.Length));

                    de.VID = sidc[0];
                    int idx = o.IndexOf(de.VID);

                    d = d.Remove(idx, de.VID.Length);
                    o = o.Remove(idx, de.VID.Length);
                }
            }

            if (de.VID != null)
            {
                if (de.VID.Length <= 8)
                    de.VID = null;
                else
                {
                    string fixsid = de.VID.Trim(new char[] { '-', ')', '(', '[', ']', '>', '<', '\'', '\"', '_' });

                    fixsid = string.Join("-", fixsid.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries));
                    fixsid = string.Join("_", fixsid.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries));
                    fixsid = string.Join(" ", fixsid.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));

                    string[] sids = fixsid.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (sids.Length > 0)
                        fixsid = sids[0];

                    de.VID = fixsid;

                    if (StringHelper.Count(de.VID, '-') > 3)
                    {
                        de.VID = null;
                    }
                }
            }

            #endregion

            #region Find TELKOMSEL SITEID

            if (de.VID == null)
            {
                if (o.IndexOf("TELKOMSEL") > -1 || o.IndexOf("TSEL") > -1)
                {
                    Match m = findSiteID.Match(o);

                    if (m.Success)
                    {
                        string siteID = m.Groups[0].Value;

                        de.VID = siteID;
                        de.ServiceType = "TELKOMSELSITES";
                    }
                }
            }
            else
            {
                Match m = findSiteID2.Match(de.VID);

                if (m.Success)
                {
                    string siteID = m.Groups[1].Value;

                    de.VID = siteID;
                    de.ServiceType = "TELKOMSELSITES";
                }
            }

            #endregion

            return de;
        }

        #endregion
    }
}
