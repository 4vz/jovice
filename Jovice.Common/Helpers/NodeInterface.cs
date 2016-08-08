using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jovice
{
    internal class NodeInterface
    {
        #region Fields

        private string type;

        /// <summary>
        /// GigabitEthernet, Ethernet, FastEthernet, Serial, etc...
        /// </summary>
        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        private string shortType;

        /// <summary>
        /// Gi, Fa, Se, etc...
        /// </summary>
        public string ShortType
        {
            get { return shortType; }
            set { shortType = value; }
        }

        private string codeType;

        /// <summary>
        /// G, F, S, E, etc...
        /// </summary>
        public string CodeType
        {
            get { return codeType; }
            set { codeType = value; }
        }

        private string port;

        /// <summary>
        /// 0/1/0, 0/1, 0/3, 1/0/0, etc....
        /// </summary>
        public string Port
        {
            get { return port; }
            set { port = value; }
        }

        private int channel = -1;

        /// <summary>
        /// For serial interface, 0, 2, 11, etc...
        /// </summary>
        public int Channel
        {
            get { return channel; }
            set { channel = value; }
        }

        private int subInterface = -1;

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

        /// <summary>
        /// For ethernet interface, 3456, 4094, etc...
        /// </summary>
        public int SubInterface
        {
            get { return subInterface; }
            set { subInterface = value; }
        }

        private int subSubInterface = -1;

        public int SubSubInterface
        {
            get { return subSubInterface; }
            set { subSubInterface = value; }
        }

        #endregion

        #region Constructors

        internal NodeInterface()
        {

        }

        #endregion

        #region Methods

        public string GetShort()
        {
            return shortType + port + "" + (IsChannel ? ":" + channel.ToString() : "") + (IsSubInterface ? "." + subInterface.ToString() : "");
        }

        public string GetBase()
        {
            return shortType + port + "" + (IsChannel ? ":" + channel.ToString() : "");
        }
        
        public string GetFull()
        {
            return type + port + "" + (IsChannel ? ":" + channel.ToString() : "") + (IsSubInterface ? "." + subInterface.ToString() : "");
        }

        public string GetFullType()
        {
            return type;
        }

        public int GetTypeRate()
        {
            int typerate = -1;

            if (ShortType == "Hu") typerate = 104857600;
            else if (ShortType == "Te") typerate = 10485760;
            else if (ShortType == "Ge") typerate = 1048576;
            else if (ShortType == "Fa") typerate = 102400;
            else if (ShortType == "Et") typerate = 10240;

            return typerate;
        }

        public static NodeInterface Parse(string input)
        {
            if (input == null) return null;

            input = input.Trim();

            if (input.Length > 0)
            {
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

                        // cek huawei weirdness to define TenGig, it used GE and ends with (10G).
                        if (rest.EndsWith("(10G)"))
                        {
                            rest = rest.Remove(rest.Length - 4);
                            interfaceType = "te";
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


                        NodeInterface ci = new NodeInterface();

                        bool interfaceIdentified = true;
                        if (interfaceType == "g" || interfaceType == "gi" || interfaceType == "gigabitethernet" || interfaceType == "ge")
                        {
                            ci.Type = "GigabitEthernet";
                            ci.ShortType = "Gi";
                            ci.CodeType = "G";
                        }
                        else if (interfaceType == "f" || interfaceType == "fa" || interfaceType == "fastethernet" || interfaceType == "fe")
                        {
                            ci.Type = "FastEthernet";
                            ci.ShortType = "Fa";
                            ci.CodeType = "F";
                        }
                        else if (interfaceType == "e" || interfaceType == "et" || interfaceType == "ethernet")
                        {
                            ci.Type = "Ethernet";
                            ci.ShortType = "Et";
                            ci.CodeType = "E";
                        }
                        else if (interfaceType == "s" || interfaceType == "se" || interfaceType == "serial")
                        {
                            ci.Type = "Serial";
                            ci.ShortType = "Se";
                            ci.CodeType = "S";
                        }
                        else if (interfaceType == "l" || interfaceType == "lo" || interfaceType == "loopback" || interfaceType == "loop")
                        {
                            ci.Type = "Loopback";
                            ci.ShortType = "Lo";
                            ci.CodeType = "L";
                        }
                        else if (interfaceType == "t" || interfaceType == "te" || interfaceType == "tengige" || interfaceType == "xe")
                        {
                            ci.Type = "TenGigE";
                            ci.ShortType = "Te";
                            ci.CodeType = "T";
                        }
                        else if (interfaceType == "h" || interfaceType == "hu" || interfaceType == "hundredgige")
                        {
                            ci.Type = "HundredGigE";
                            ci.ShortType = "Hu";
                            ci.CodeType = "H";
                        }
                        else if (interfaceType == "ag" || interfaceType == "eth-trunk")
                        {
                            ci.Type = "AggregatedInterface";
                            ci.ShortType = "Ag";
                            ci.CodeType = "A";
                        }
                        else if (interfaceType == "ex")
                        {
                            ci.Type = "UnspecifiedInterface";
                            ci.ShortType = "Ex";
                            ci.CodeType = "U";
                        }
                        else interfaceIdentified = false;

                        if (interfaceIdentified)
                        {
                            ci.Port = port;
                            ci.Channel = channel;
                            ci.SubInterface = subif;
                            ci.SubSubInterface = subsubif;

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
