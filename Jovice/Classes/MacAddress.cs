using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Aphysoft.Share;

namespace Jovice
{
    public class MacAddress
    {
        #region Fields

        private string address;

        /// <summary>
        /// 00-11-22-33-44-55
        /// </summary>
        public string HyphenAddress
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                int i = 0;
                foreach (char c in address)
                {
                    if (i % 2 == 0 && i > 0) sb.Append('-');
                    sb.Append(c);
                    i++;
                }

                return sb.ToString();
            }
        }

        public string ColonAddress
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                int i = 0;
                foreach (char c in address)
                {
                    if (i % 2 == 0 && i > 0) sb.Append(':');
                    sb.Append(c);
                    i++;
                }

                return sb.ToString();
            }
        }

        public string DotAddress
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                int i = 0;
                foreach (char c in address)
                {
                    if (i % 4 == 0 && i > 0) sb.Append(':');
                    sb.Append(c);
                    i++;
                }

                return sb.ToString();
            }
        }

        #endregion

        #region Constructors

        private MacAddress()
        {                
        }

        #endregion

        #region Methods

        public static MacAddress Parse(string mac)
        {
            if (string.IsNullOrEmpty(mac)) return null;

            mac = string.Join("", mac.ToLower().Split(new char[] { ':', '-', '.' }, StringSplitOptions.RemoveEmptyEntries));

            string hex = "abcdef0123456789";

            bool notexists = false;
            foreach (char c in mac)
            {
                if (hex.IndexOf(c) == -1)
                {
                    notexists = true;
                    break;
                }
            }

            if (notexists) return null;

            if (mac.Length == 12 || mac.Length == 16)
            {
                MacAddress m = new MacAddress();

                m.address = mac;

                return m;
            }
            else return null;
        }

        #endregion
    }
}
