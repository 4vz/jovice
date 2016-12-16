using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jovice
{
    internal class NetworkInterface
    {
        #region Fields

        private string type;

        /// <summary>
        /// GigabitEthernet, Ethernet, FastEthernet, Serial, etc...
        /// </summary>
        public string Type
        {
            get { return type; }
        }

        private string shortType;

        /// <summary>
        /// Gi, Fa, Se, etc...
        /// </summary>
        public string ShortType
        {
            get { return shortType; }
        }

        private string codeType;

        /// <summary>
        /// G, F, S, E, etc...
        /// </summary>
        public string CodeType
        {
            get { return codeType; }
        }

        private string port;

        /// <summary>
        /// 0/1/0, 0/1, 0/3, 1/0/0, etc....
        /// </summary>
        public string Port
        {
            get { return port; }
        }

        private int channel = -1;

        /// <summary>
        /// For serial interface, 0, 2, 11, etc...
        /// </summary>
        public int Channel
        {
            get { return channel; }
        }

        public bool IsChannel
        {
            get { return channel != -1 ? true : false; }
        }
        public bool IsSubInterface
        {
            get { return subInterface != -1 ? true : false; }
        }
        public bool IsDirect
        {
            get { return subInterface == 0 ? true : false; }
        }
        
        private int subInterface = -1;

        /// <summary>
        /// For ethernet interface, 3456, 4094, etc...
        /// </summary>
        public int SubInterface
        {
            get { return subInterface; }
        }

        private int subSubInterface = -1;

        public int SubSubInterface
        {
            get { return subSubInterface; }
        }

        /// <summary>
        /// Gi0/1, Se4/3:2, Te0/0/0/1.502
        /// </summary>
        public string ShortName
        {
            get { return shortType + port + "" + (IsChannel ? ":" + channel.ToString() : "") + (IsSubInterface ? "." + subInterface.ToString() : ""); }
        }

        /// <summary>
        /// Gi0/1.456 -> Gi0/1, Se4/3:2.456 -> Se4/3:2, Te0/0/0/1.502 -> Te0/0/0/1
        /// </summary>
        public string ShortBaseName
        {
            get { return shortType +port + "" + (IsChannel ? ":" + channel.ToString() : ""); }
        }

        /// <summary>
        /// GigabitEthernet0/1, Serial4/3:2, TenGigabitEthernet0/0/0/1.502
        /// </summary>
        public string FullName
        {
            get { return type + port + "" + (IsChannel ? ":" + channel.ToString() : "") + (IsSubInterface ? "." + subInterface.ToString() : ""); }
        }

        public int TypeRate
        {
            get
            {
                int typerate = -1;

                if (ShortType == "Hu") typerate = 104857600;
                else if (ShortType == "Te") typerate = 10485760;
                else if (ShortType == "Ge") typerate = 1048576;
                else if (ShortType == "Fa") typerate = 102400;
                else if (ShortType == "Et") typerate = 10240;

                return typerate;
            }
        }

        #endregion

        #region Constructors

        private NetworkInterface()
        {

        }

        #endregion

        #region Methods

        public static NetworkInterface Parse(string input)
        {
            if (input == null) return null;

            input = input.Trim();

            if (input.Length > 0)
            {
                if (input.Length > 5 && input.StartsWith("100GE")) input = "Hu" + input.Substring(5);

                if (char.IsLetter(input[0]))
                {
                    // search digit
                    int portIndexOf = -1;
                    for (int i = 0; i < input.Length; i++)
                    {
                        char c = input[i];

                        if (char.IsDigit(c)) // enter port
                        {
                            portIndexOf = i;
                            break;
                        }
                    }

                    // harus ada portnya
                    if (portIndexOf > -1)
                    {
                        string interfaceType = input.Substring(0, portIndexOf).ToLower();
                        string port = null;
                        int channel = -1;
                        int subif = -1;
                        int subsubif = -1;
                        string rest = input.Substring(portIndexOf);

                        // cek huawei weirdness to define TenGigE, it used GE and ends with (10G).
                        if (rest.EndsWith("(10G)"))
                        {
                            rest = rest.Remove(rest.Length - 4);
                            interfaceType = "te";
                        }
                        else if (rest.EndsWith("(100G)"))
                        {
                            rest = rest.Remove(rest.Length - 5);
                            interfaceType = "hu";
                        }

                        // cek subinterface
                        string[] restbydot = rest.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

                        if (restbydot.Length >= 2)
                        {
                            rest = restbydot[0];
                            if (restbydot[1] == "DIRECT") subif = 0;
                            else if (restbydot.Length > 2)
                            {
                                if (!int.TryParse(restbydot[1], out subif)) subif = -1;
                                if (!int.TryParse(restbydot[2], out subsubif)) subsubif = -1;
                            }
                            else if (!int.TryParse(restbydot[1], out subif)) subif = -1;
                        }

                        // cek channel
                        string[] restbycha = rest.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                        port = restbycha[0];

                        if (restbycha.Length == 2)
                        {
                            if (!int.TryParse(restbycha[1], out channel)) channel = -1;
                        }

                        // fix port
                        StringBuilder portfix = new StringBuilder();
                        bool slash = false;
                        for (int i = 0; i < port.Length; i++)
                        {
                            char c = port[i];

                            if (char.IsDigit(c))
                            {
                                slash = false;
                                portfix.Append(c);
                            }
                            else if (c == '/' && slash == false)
                            {
                                portfix.Append("/");
                                slash = true;
                            }
                        }
                        port = portfix.ToString();


                        NetworkInterface ci = new NetworkInterface();

                        bool interfaceIdentified = true;
                        if (interfaceType == "g" || interfaceType == "gi" || interfaceType == "gigabitethernet" || interfaceType == "ge")
                        {
                            ci.type = "GigabitEthernet";
                            ci.shortType = "Gi";
                            ci.codeType = "G";
                        }
                        else if (interfaceType == "f" || interfaceType == "fa" || interfaceType == "fastethernet" || interfaceType == "fe")
                        {
                            ci.type = "FastEthernet";
                            ci.shortType = "Fa";
                            ci.codeType = "F";
                        }
                        else if (interfaceType == "e" || interfaceType == "et" || interfaceType == "ethernet")
                        {
                            ci.type = "Ethernet";
                            ci.shortType = "Et";
                            ci.codeType = "E";
                        }
                        else if (interfaceType == "s" || interfaceType == "se" || interfaceType == "serial")
                        {
                            ci.type = "Serial";
                            ci.shortType = "Se";
                            ci.codeType = "S";
                        }
                        else if (interfaceType == "t" || interfaceType == "te" || interfaceType == "tengige" || interfaceType == "xe")
                        {
                            ci.type = "TenGigE";
                            ci.shortType = "Te";
                            ci.codeType = "T";
                        }
                        else if (interfaceType == "h" || interfaceType == "hu" || interfaceType == "hundredgige")
                        {
                            ci.type = "HundredGigE";
                            ci.shortType = "Hu";
                            ci.codeType = "H";
                        }
                        else if (interfaceType == "ag" || interfaceType == "eth-trunk")
                        {
                            ci.type = "AggregatedInterface";
                            ci.shortType = "Ag";
                            ci.codeType = "A";
                        }
                        else if (interfaceType == "ex")
                        {
                            ci.type = "UnspecifiedInterface";
                            ci.shortType = "Ex";
                            ci.codeType = "U";
                        }
                        else interfaceIdentified = false;

                        if (interfaceIdentified)
                        {
                            ci.port = port;
                            ci.channel = channel;
                            ci.subInterface = subif;
                            ci.subSubInterface = subsubif;

                            return ci;
                        }
                        else return null;
                    }
                    else return null;
                }
                else return null;
            }
            else return null;
        }

        #endregion
    }
}
