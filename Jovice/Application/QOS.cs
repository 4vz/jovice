using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Center
{
    public class QOS
    {
        #region Fields

        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private int bandwidth = -1;

        /// <summary>
        /// Bandwidth in Kbps
        /// </summary>
        public int Bandwidth
        {
            get { return bandwidth; }
            set { bandwidth = value; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns in KBPS
        /// </summary>
        public static double ParseBandwidth(string input, int defaultMultiplier)
        {
            input = input.Trim().Replace(" ", "").ToLower();

            double main = 0;
            double multiplier = 1;

            int io;

            bool parseThat = true;

            if ((io = input.IndexOf("kbps")) > -1 && input.EndsWith("kbps"))
            {
                multiplier = 1000;
            }
            else if ((io = input.IndexOf("kilobps")) > -1 && input.EndsWith("kilobps"))
            {
                multiplier = 1000;
            }
            else if ((io = input.IndexOf("kilo")) > -1 && input.EndsWith("kilo"))
            {
                multiplier = 1000;
            }
            else if ((io = input.IndexOf("kb")) > -1 && input.EndsWith("kb"))
            {
                multiplier = 1000;
            }
            else if ((io = input.IndexOf("k")) > -1 && input.EndsWith("k"))
            {
                multiplier = 1000;
            }
            else if ((io = input.IndexOf("mbps")) > -1 && input.EndsWith("mbps"))
            {
                multiplier = 1024000;
            }
            else if ((io = input.IndexOf("megabps")) > -1 && input.EndsWith("megabps"))
            {
                multiplier = 1024000;
            }
            else if ((io = input.IndexOf("mega")) > -1 && input.EndsWith("mega"))
            {
                multiplier = 1024000;
            }            
            else if ((io = input.IndexOf("mb")) > -1 && input.EndsWith("mb"))
            {
                multiplier = 1024000;
            }
            else if ((io = input.IndexOf("m")) > -1 && input.EndsWith("m"))
            {
                multiplier = 1024000;
            }
            else if ((io = input.IndexOf("gbps")) > -1 && input.EndsWith("gbps"))
            {
                multiplier = 1048576000;
            }
            else if ((io = input.IndexOf("gigabps")) > -1 && input.EndsWith("gigabps"))
            {
                multiplier = 1048576000;
            }
            else if ((io = input.IndexOf("giga")) > -1 && input.EndsWith("giga"))
            {
                multiplier = 1048576000;
            }
            else if ((io = input.IndexOf("gb")) > -1 && input.EndsWith("gb"))
            {
                multiplier = 1048576000;
            }
            else if ((io = input.IndexOf("g")) > -1 && input.EndsWith("g"))
            {
                multiplier = 1048576000;
            }
            else if ((io = input.IndexOf("bps")) > -1 && input.EndsWith("bps"))
            {
                multiplier = 1;
            }
            else if ((io = input.IndexOf("b")) > -1 && input.EndsWith("b"))
            {
                multiplier = 1;
            }
            else
            {
                parseThat = false;
                multiplier = defaultMultiplier;
            }

            string rawt;

            if (parseThat)
                rawt = input.Substring(0, io);
            else
                rawt = input;

            int koma = rawt.IndexOf(',');
            int titik = rawt.IndexOf('.');

            if (koma > -1 && titik > -1)
            {
                if (koma < titik) // 10,000.540 -> 10000.540
                {
                    string pre = rawt.Substring(0, titik).Replace(",", "");
                    string post = rawt.Substring(titik + 1);

                    int pred;
                    if (pre.Length == 0) pred = 0;
                    else if (int.TryParse(pre, out pred)) { }
                    else pred = 0;

                    string soo = "0." + post;
                    double postd;
                    if (post.Length == 0) postd = 0;
                    else if (double.TryParse(soo, out postd)) { }
                    else postd = 0;

                    main = (double)pred + postd;
                }
                else if (titik < koma)
                {
                    string pre = rawt.Substring(0, koma).Replace(".", "");
                    string post = rawt.Substring(koma + 1);

                    int pred;
                    if (pre.Length == 0) pred = 0;
                    else if (int.TryParse(pre, out pred)) { }
                    else pred = 0;

                    string soo = "0." + post;
                    double postd;
                    if (post.Length == 0) postd = 0;
                    else if (double.TryParse(soo, out postd)) { }
                    else postd = 0;

                    main = (double)pred + postd;
                }
            }
            else if (titik > -1)
            {
                string pre = rawt.Substring(0, titik);
                string post = rawt.Substring(titik + 1);

                int pred;
                if (pre.Length == 0) pred = 0;
                else if (int.TryParse(pre, out pred)) { }
                else pred = 0;

                string soo = "0." + post;
                double postd;
                if (post.Length == 0) postd = 0;
                else if (double.TryParse(soo, out postd)) { }
                else postd = 0;

                main = (double)pred + postd;
            }
            else if (koma > -1)
            {
                string pre = rawt.Substring(0, koma);
                string post = rawt.Substring(koma + 1);

                int pred;
                if (pre.Length == 0) pred = 0;
                else if (int.TryParse(pre, out pred)) { }
                else pred = 0;

                string soo = "0." + post;
                double postd;
                if (post.Length == 0) postd = 0;
                else if (double.TryParse(soo, out postd)) { }
                else postd = 0;

                main = (double)pred + postd;
            }
            else
            {
                if (!double.TryParse(rawt, out main))
                {
                    main = 0;
                }
            }

            return (main * multiplier) / 1000;
        }
        
        public static double ParseBandwidth(string input)
        {
            return ParseBandwidth(input, 1000000);
        }

        #endregion
    }

    public class CiscoQOS : QOS
    {
        #region Fields

        private string package;

        public string Package
        {
            get { return package; }
            set { package = value; }
        }

        #endregion

        #region Constructors

        public CiscoQOS()
        {

        }

        #endregion

        #region Methods

        public static CiscoQOS Parse(string input)
        {
            string pmpack = null;
            int pmspeed = -1;

            int idc;
            string pmnamelower = input.ToLower();
            if ((idc = pmnamelower.IndexOf("package1")) > -1) pmpack = "1";
            else if ((idc = pmnamelower.IndexOf("package2")) > -1) pmpack = "2";
            else if ((idc = pmnamelower.IndexOf("package3")) > -1) pmpack = "3";
            else if ((idc = pmnamelower.IndexOf("package4")) > -1) pmpack = "4";
            else if ((idc = pmnamelower.IndexOf("package5")) > -1) pmpack = "5";
            else if ((idc = pmnamelower.IndexOf("package7")) > -1) pmpack = "7";
            else if ((idc = pmnamelower.IndexOf("paket_2")) > -1) pmpack = "2";
            else if ((idc = pmnamelower.IndexOf("paket_3")) > -1) pmpack = "3";
            else if ((idc = pmnamelower.IndexOf("paket_4")) > -1) pmpack = "4";
            else if ((idc = pmnamelower.IndexOf("paket_5")) > -1) pmpack = "5";
            else if ((idc = pmnamelower.IndexOf("paket_7")) > -1) pmpack = "7";

            if (pmpack == null) idc = 0;
            else idc += 8;

            string[] pnmes = input.Split(new char[] { '-', '_' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string pnme in pnmes)
            {
                if (char.IsDigit(pnme[0]))
                {
                    double speed = ParseBandwidth(pnme, 1000);
                    pmspeed = (int)Math.Round(speed);
                    //break;
                }
            }

            if (pmspeed < 8) pmspeed = -1; // below 8kbps is not applicable

            CiscoQOS qos = new CiscoQOS();
            qos.Name = input;
            qos.Package = pmpack;
            qos.Bandwidth = pmspeed;

            return qos;
        }
        
        #endregion
    }

    public class AlcatelLucentQOS : QOS
    {
        #region Methods

        public static AlcatelLucentQOS Parse(string input)
        {
            AlcatelLucentQOS q = new AlcatelLucentQOS();
            q.Name = input;

            if (input.Length == 5)
            {               
                double multiplier = 0;
                if (input.StartsWith("11")) multiplier = 1;
                else if (input.StartsWith("12")) multiplier = 1024;
                else if (input.StartsWith("13")) multiplier = 1048576;

                string rend = input.Substring(2);

                int kib = 0;

                if (int.TryParse(rend, out kib))
                {
                    if (multiplier == 0)
                        q.Bandwidth = -1;
                    else
                        q.Bandwidth = (int)Math.Round((double)kib * multiplier);
                }

            }

            return q;
        }

        #endregion
    }

    public class HuaweiQOS : QOS
    {
        #region Methods

        public static HuaweiQOS Parse(string input)
        {
            int pmspeed = -1;

            HuaweiQOS q = new HuaweiQOS();
            q.Name = input;

            string[] pnmes = input.Split(new char[] { '-', '_' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string pnme in pnmes)
            {
                if (char.IsDigit(pnme[0]))
                {
                    double speed = ParseBandwidth(pnme, 1000);
                    pmspeed = (int)Math.Round(speed);
                    //break;
                }
            }

            if (pmspeed != -1)
                q.Bandwidth = pmspeed;

            return q;
        }

        #endregion
    }
}
