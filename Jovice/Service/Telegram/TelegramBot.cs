using Aphysoft.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;


using System.Diagnostics;
using Microsoft.VisualBasic.Devices;
using System.Management;

namespace Center
{
    internal static class TelegramBot
    {
        #region Fields

        private static TelegramBotClient telegramBot = null;
        private static DateTime startDate;
        private static List<long> groups = new List<long>();
        private static Dictionary<int, Tuple<string, object, int>> talks = new Dictionary<int, Tuple<string, object, int>>();
        private static Dictionary<long, int> lastFromIDs = new Dictionary<long, int>();

        private readonly static PerformanceCounter cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        private readonly static PerformanceCounter ramCounter = new PerformanceCounter("Memory", "Available MBytes");
        private readonly static ComputerInfo computerInfo = new ComputerInfo();
        private readonly static ManagementObjectSearcher mos = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_Processor");

        private static List<long> noticeWhenNecrowIsBackUp = new List<long>();

        #endregion

        public static void Init()
        {
            cpuCounter.NextValue();

#if DEBUG
            telegramBot = new TelegramBotClient("353770204:AAEs0Snc9Zc9crw-8_zQS8QGHyI0A4QBE6E");
#else
            telegramBot = new TelegramBotClient("302702777:AAEvFgWiSnqM6DZb9dq-uAnnOmJMLGNiDbw");
#endif

            telegramBot.OnMessage += OnMessage;

            telegramBot.MessageOffset = -1; // get only last 1 message before this bot even start
            startDate = DateTime.UtcNow;

            string groupsSetting = Settings.Get("TelegramBotCenterGroups");
            if (groupsSetting != null)
            {
                string[] groupTokens = groupsSetting.Split(StringSplitTypes.Comma);
                foreach (string groupToken in groupTokens)
                {
                    long group;
                    if (long.TryParse(groupToken, out group)) groups.Add(group);
                }
            }

            telegramBot.StartReceiving();

            foreach (long group in groups)
            {
                SendMessage("telkom.center is now online", group);
            }
        }

        public delegate void SendMessageCallBack(Message message);

        public static void NecrowOnline()
        {
            if (noticeWhenNecrowIsBackUp.Count > 0)
            {
                foreach (long chatID in noticeWhenNecrowIsBackUp)
                {
                    SendMessage("Necrow is now back up", chatID);
                }
                noticeWhenNecrowIsBackUp.Clear();
            }
        }

        public static void NecrowProbeStatus(ServerNecrowServiceMessage m)
        {
            object[] datas = m.Data;

            StringBuilder probeloc = new StringBuilder();
            int actv = 0;
            foreach (object data in datas)
            {
                Tuple<string, string, string, DateTime, string> entry = (Tuple<string, string, string, DateTime, string>)data;
                if (entry.Item1 == "A")
                {
                    probeloc.Append("\nPROBE" + entry.Item2 + " on " + entry.Item3 + " (" + Math.Round((DateTime.UtcNow - entry.Item4).TotalSeconds) + "s) [" +
                        entry.Item5 + "]");
                    actv++;
                }
                else
                {
                    if (entry.Item3 != null)
                        probeloc.Append("\nPROBE" + entry.Item2 + " idle (was on " + entry.Item3 + " " + Math.Round((DateTime.UtcNow - entry.Item4).TotalSeconds) + "s ago)");
                    else
                        probeloc.Append("\nPROBE" + entry.Item2 + " starting up ");
                }
            }

            if (datas.Length == 0) 
                SendMessage("There are no Necrow's probes currently active", (long)m.IdentifierData1, (int)m.IdentifierData2, (int)m.IdentifierData3);
            else
            {
                SendMessage(datas.Length + " probe" + (datas.Length > 1 ? "s" : "") + " currently active:" + probeloc.ToString(), (long)m.IdentifierData1, (int)m.IdentifierData2, (int)m.IdentifierData3);
            }
        }

        public static void NecrowProbe(ServerNecrowServiceMessage m)
        {
            object[] data = m.Data;

            string node = (string)data[0];
            string ev = (string)data[1];

            if (ev == "STARTING")
                SendMessage("Starting probe process to " + node, (long)m.IdentifierData1, (int)m.IdentifierData2, (int)m.IdentifierData3);
            else if (ev == "UPTODATE")
            {
                DateTime lastUpdate = (DateTime)data[2];
                SendMessage(new string[] {
                    node + " is already up to date. Do you still want to continue the probe?",
                    node + " is up to date. Continue the probe?",
                    node + " is up to date. You want to continue the probe?"
                }, (long)m.IdentifierData1, (int)m.IdentifierData2, (int)m.IdentifierData3);
                SetTalk("necrow>probe>continueprobe?", node, (int)m.IdentifierData2, (int)m.IdentifierData3);
            }
            else if (ev == "ERROR")
            {
                string message = (string)data[2];
                SendMessage(new string[] {
                     "I am sorry, I caught an error during probe process to " + node,
                     "I caught an error during probe to " + node + ", I am sorry",
                     "Sorry, I caught an error during probe to " + node
                }, (long)m.IdentifierData1, (int)m.IdentifierData2, (int)m.IdentifierData3);
                if (message != null)
                {
                    SendMessage(new string[] {
                    "The error as follow: " + message,
                    "The error message I retrived: " + message
                }, (long)m.IdentifierData1, (int)m.IdentifierData2, (int)m.IdentifierData3, 500, null);
                }
                SendMessage(new string[] {
                    "Do you want to retry the probe?",
                    "Do you want to try it again?",
                    "You mind to try it again?",
                    "Retry?",
                    "You want to try it again?"
                }, (long)m.IdentifierData1, (int)m.IdentifierData2, (int)m.IdentifierData3, 1500, null);
                SetTalk("necrow>probe>retry?", node, (int)m.IdentifierData2, (int)m.IdentifierData3);
            }
            else if (ev == "FINISH")
            {
                SendMessage(new string[] {
                     "I finished the probe process to " + node,
                     "The probe process to " + node + " is completed",
                     node + "'s probe is finished",
                     node + " has successfully probed",
                     "I inform you that " + node + " is successfully probed"
                }, (long)m.IdentifierData1, (int)m.IdentifierData2, (int)m.IdentifierData3, true);
                SetTalk("smalltalk", null, 0, (int)m.IdentifierData3);
            }
        }

        private static void SendMessage(string[] messages, long chatID)
        {
            SendMessage(messages.Random(), chatID);
        }

        private static void SendMessage(string message, long chatID)
        {
            SendMessage(message, chatID, 0, 0);
        }

        private static void SendMessage(string[] messages, long chatID, int replyMessageID, int fromID)
        {
            SendMessage(messages.Random(), chatID, replyMessageID, fromID);
        }

        private static void SendMessage(string message, long chatID, int replyMessageID, int fromID)
        {
            SendMessage(message, chatID, replyMessageID, fromID, null);
        }

        private static void SendMessage(string[] messages, long chatID, int replyMessageID, int fromID, bool forceReply)
        {
            SendMessage(messages.Random(), chatID, replyMessageID, fromID, forceReply);
        }

        private static void SendMessage(string message, long chatID, int replyMessageID, int fromID, bool forceReply)
        {
            SendMessage(message, chatID, replyMessageID, fromID, forceReply, null);
        }

        private static void SendMessage(string[] messages, long chatID, int replyMessageID, int fromID, SendMessageCallBack callback)
        {
            SendMessage(messages.Random(), chatID, replyMessageID, fromID, callback);
        }

        private static void SendMessage(string message, long chatID, int replyMessageID, int fromID, SendMessageCallBack callback)
        {
            SendMessage(message, chatID, replyMessageID, fromID, false, 0, callback);
        }

        private static void SendMessage(string[] messages, long chatID, int replyMessageID, int fromID, bool forceReply, SendMessageCallBack callback)
        {
            SendMessage(messages.Random(), chatID, replyMessageID, fromID, forceReply, callback);
        }

        private static void SendMessage(string message, long chatID, int replyMessageID, int fromID, bool forceReply, SendMessageCallBack callback)
        {
            SendMessage(message, chatID, replyMessageID, fromID, forceReply, 0, callback);
        }

        private static void SendMessage(string[] messages, long chatID, int replyMessageID, int fromID, int delay, SendMessageCallBack callback)
        {
            SendMessage(messages.Random(), chatID, replyMessageID, fromID, delay, callback);
        }

        private static void SendMessage(string message, long chatID, int replyMessageID, int fromID, int delay, SendMessageCallBack callback)
        {
            SendMessage(message, chatID, replyMessageID, fromID, false, delay, callback);
        }

        private static void SendMessage(string[] messages, long chatID, int replyMessageID, int fromID, bool forceReply, int delay, SendMessageCallBack callback)
        {
            SendMessage(messages.Random(), chatID, replyMessageID, fromID, forceReply, delay, callback);
        }

        private static void SendMessage(string message, long chatID, int replyMessageID, int fromID, bool forceReply, int delay, SendMessageCallBack callback)
        {
            Thread thread = new Thread(new ThreadStart(delegate ()
            {
                if (delay > 0) Thread.Sleep(delay);

                int toReply = 0;

                if (replyMessageID > 0)
                {
                    if (forceReply || LastFromID(chatID) != fromID) toReply = replyMessageID;
                }

                Task<Message> task = telegramBot.SendTextMessageAsync(chatID, message, replyToMessageId: toReply);

                if (callback != null)
                {
                    task.Wait();
                    callback(task.Result);
                }
            }));
            thread.Start();
        }

        private static int LastFromID(long chatID)
        {
            int lastFromID = 0;
            if (!lastFromIDs.ContainsKey(chatID))
                lastFromIDs.Add(chatID, 0);
            else lastFromID = lastFromIDs[chatID];
            return lastFromID;
        }

        private static void SetTalk(string talk, object register, int replyRegister, int fromID)
        {
            if (talks.ContainsKey(fromID))
                talks[fromID] = new Tuple<string, object, int>(talk, register, replyRegister);
            else
                talks.Add(fromID, new Tuple<string, object, int>(talk, register, replyRegister));
        }

        private static void ClearTalk(int fromID)
        {
            SetTalk(null, null, 0, fromID);
        }

        private static Tuple<string, object, int> GetTalk(int fromID)
        {
            if (talks.ContainsKey(fromID))
                return talks[fromID];
            else
                return new Tuple<string, object, int>(null, null, 0);
        }

        #region /probe

        private static void MessageProbeExecute(string probe, long chatID, int messageID, int fromID)
        {
            Database jovice = Jovice.Database;
            // query name apakah ada
            Result result = jovice.Query("select * from Node where lower(NO_Name) = {0}", probe);

            if (result.Count == 1)
            {
                Row row = result[0];

                string nodeName = row["NO_Name"].ToString();
                bool active = row["NO_Active"].ToBool();

                if (active == false)
                {
                    SendMessage(new string[] {
                        nodeName + " is appearing inactive. Would you still try to probe this node?",
                        nodeName + " is inactive. Do you still want to probe this node?",
                        nodeName + " is not active. Do you still wan to probe this node?"
                    }, chatID, messageID, fromID);
                    SetTalk("necrow>probe>forceinactive?", nodeName, messageID, fromID);
                }
                else
                {
                    ServerNecrowServiceMessage m = new ServerNecrowServiceMessage(NecrowServiceMessageType.Probe);
                    m.Data = new object[] { nodeName };
                    m.IdentifierData1 = chatID;
                    m.IdentifierData2 = messageID;
                    m.IdentifierData3 = fromID;

                    Server.NecrowSend(m);

                    SendMessage(new string[] {
                        "Understood, I'll probe " + nodeName + " shortly",
                        "You got it, I will probe " + nodeName + " shortly",
                        "Yes sir, I will probe " + nodeName + " right away",
                        "Affrimative, " + nodeName + " will be probed in no time",
                        nodeName + ", Right away!",
                        nodeName + ", got it",
                        nodeName + ", you got it",
                        "All right, I'll probe " + nodeName + " soon",
                        "Got it, I will probe " + nodeName + " very soon",
                        "Right away sir, " + nodeName + " shall be done"
                    }, chatID, messageID, fromID, true);

                    ClearTalk(fromID);
                }
            }
            else
            {
                // ok g ketemu
                string[] probeTokens = probe.Trim().Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries);
                if (probeTokens.Length == 1)
                {
                    SendMessage(new string[] { "I guess what you've typed is incorrect", "It's not correct", "Please specify the node correctly", "What?", "Are you sure?" }, chatID, messageID, fromID, true);
                }
                else if (probeTokens.Length == 2)
                {
                    string depan = probeTokens[0].ToLower();
                    string tengah = probeTokens[1].ToLower();
                    if (depan.StartsWith("me", "pe", "ces", "hrb") && tengah.StartsWith("d"))
                    {
                        SendMessage(new string[] { probe + " what?", "The location part is missing", string.Join("-", probeTokens).ToUpper() + "-???, what is the third part?"
                        }, chatID, messageID, fromID, true);
                    }
                    else if (depan.StartsWith("me", "pe", "ces", "hrb") && !tengah.StartsWith("d"))
                    {
                        SendMessage(new string[] { probe + " what?", "The location part is missing, and also it should be D in the middle part", "It should be " + probeTokens[0].ToUpper() + "-D<1 to 7>-<location/code>"
                        }, chatID, messageID, fromID, true);
                    }
                    else
                    {
                        SendMessage(new string[] { "I don't understand", "I guess what you've typed is incorrect", "It's not correct", "Please specify the node correctly", "It's not correct, it should have at least three parts" }, chatID, messageID, fromID, true);
                    }
                }
                else
                {
                    string depan = probeTokens[0].ToLower();
                    string tengah = probeTokens[1].ToLower();
                    if (depan.StartsWith("me", "pe", "ces", "hrb") && tengah.StartsWith("d"))
                    {
                        string firstPart = depan + "-" + tengah;
                        string lastPart = "";
                        for (int i = 2; i < probeTokens.Length; i++) lastPart += probeTokens[i];

                        result = jovice.Query(@"
select NO_Name from Node where 
dbo.DoubleMetaPhone(LOWER(REPLACE(SUBSTRING(NO_Name, CHARINDEX('-', NO_Name, CHARINDEX('-', NO_Name) + 1) + 1, 32), '-', ''))) = 
dbo.DoubleMetaPhone({0}) and LOWER(SUBSTRING(NO_Name, 0, CHARINDEX('-', NO_Name, CHARINDEX('-', NO_Name) + 1))) = {1}", lastPart, firstPart);

                        if (result.Count > 0)
                        {
                            List<string> possibilities = new List<string>();
                            foreach (Row row in result)
                            {
                                possibilities.Add(row["NO_Name"].ToString());
                            }

                            if (possibilities.Count > 1)
                            {
                                if (possibilities.Count == 2)
                                {
                                    SendMessage(new string[] {
                                        "Did you mean " + possibilities[0] + " or " + possibilities[1] + "?",
                                        "Probably what you want to probe are whether " + possibilities[0] + " or " + possibilities[1] + ", which one?",
                                    }, chatID, messageID, fromID, true);

                                    SetTalk("necrow>probe>choice?", string.Join(",", possibilities.ToArray()), messageID, fromID);
                                }
                                else if (possibilities.Count > 2 && possibilities.Count <= 4)
                                {
                                    string which = possibilities[0] + ", " + possibilities[1];
                                    if (possibilities.Count == 3)
                                        which += " or " + possibilities[2];
                                    else if (possibilities.Count == 4)
                                        which += ", " + possibilities[2] + " or " + possibilities[3];
                                    
                                    SendMessage(new string[] {
                                        "Based on my database, I guess what you mean are whether " + which + ", which one?",
                                    }, chatID, messageID, fromID, true);

                                    SetTalk("necrow>probe>choice?", string.Join(",", possibilities.ToArray()), messageID, fromID);
                                }
                                else
                                {
                                    SendMessage(new string[] {
                                        "I can't find the specified probe in my database",
                                        "I can't find that",
                                        "Sorry, I can't find that",
                                        "There is no such probe name in my database",
                                        "There is no probe called " + probe + " in my database"
                                    }, chatID, messageID, fromID, true);

                                    SetTalk("smalltalk", null, 0, fromID);
                                }
                            }
                            else
                            {
                                SendMessage(new string[] {
                                    "Did you mean " + possibilities[0] + "?",
                                    "Probably what you want to probe is " + possibilities[0] + ", is that correct?",
                                    "Is that " + possibilities[0] + "?",
                                    "Did you mean the name you want to probe is " + possibilities[0] + "?"
                                }, chatID, messageID, fromID, true);

                                SetTalk("necrow>probe>choice?", possibilities[0], messageID, fromID);
                            }
                        }
                        else
                        {
                            SendMessage(new string[] {
                                "I cannot find the specified node in my database",
                                "I am sorry, I cannot find " + probe + " in my database"
                            }, chatID, messageID, fromID, true);

                            SetTalk("smalltalk", null, 0, fromID);
                        }
                    }
                    else if (depan.StartsWith("me", "pe", "ces", "hrb") && !tengah.StartsWith("d"))
                    {
                        SendMessage(new string[] { "The second part of the node you specified is not correct",
                            "It should be D<something> for the second part of the node you specified", 
                            "The D-part of specified node is not correct"
                        }, chatID, messageID, fromID, true);

                        SetTalk("smalltalk", null, 0, fromID);
                    }
                    else
                    {
                        SendMessage(new string[] {
                            "I don't understand",
                            "I guess what you've typed is incorrect",
                            "It's not correct", "Please specify the node correctly",
                            "It is not correct bro",
                            "Please specify the correct node",
                        }, chatID, messageID, fromID, true);

                        SetTalk("smalltalk", null, 0, fromID);
                    }
                }
            }
        }

        #endregion

        private static void OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            //Service.Debug(DateTime.UtcNow + ", incoming message: " + e.Message.Date);
            if (e.Message.Date < startDate) return; // avoid old message, and just discard em

            bool isPrivateMessage = (e.Message.Chat.Id == e.Message.From.Id);

            long chatID = e.Message.Chat.Id;
            int fromID = e.Message.From.Id;
            string message = e.Message.Text;
            string messageLower = message.ToLower();
            int messageID = e.Message.MessageId;

            string messageFamily = null;

            if (messageLower.InOf("/yes", "yes", "ya", "yaa", "yaaa", "yess", "yesss", "hell yes", "yeah", "iya", "lakukan", "baiklah", "yup", "mainkan", "yuup", "yuuup", "yep", "yeep") > -1) messageFamily = "yes";
            else if (messageLower.InOf("/no", "no", "noo", "hell no", "nooo", "tidak", "gak", "enggak", "gak usah", "nanti aja", "ga", "ga usah", "pass", "later", "nope") > -1) messageFamily = "no";
            else if (messageLower.IndexOf("terima kasih", "makasih", "makacih", "makasi", "makaci", "thank you", "thanks", "terbaik", "merci", "gracias", "gratzie") > -1) messageFamily = "thanks";
            else if (messageLower.InOf(
                "mas robot", "mas telkom.center", "mas bot",
                "hi mas bot", "hi mas robot", "hi mas telkom.center", "hi mas center",
                "telkom.center", "bot telkom.center",
                "hi robot", "hi telkom.center", "hi center", "hi bot", "hi mas bot",
                "halo telkom.center", "halo center", "halo robot", "halo bot",
                "hello center", "hello telkom.center", "hello robot", "hello bot",
                "hey center", "hey telkom.center", "hey robot", "hey bot",
                "hoy center", "hoy telkom.center", "hoy robot", "hoy bot",
                ) > -1) messageFamily = "called";
            else if (messageLower.InOf(
                "ass robot", "ass telkom.center", "ass center", "ass bot",
                "assalamualaikum robot", "assalamualaikum telkom.center", "assalamualaikum center", "assalamualaikum bot",
                "assalamualaikum wr wb robot", "assalamualaikum wr wb telkom.center", "assalamualaikum wr wb center", "assalamualaikum wr wb bot",
                "assalamualaikum wrwb robot", "assalamualaikum wrwb telkom.center", "assalamualaikum wrwb center", "assalamualaikum wrwb bot"
                ) > -1) messageFamily = "called>salam";

            if (!isPrivateMessage)
            {
                #region Groups
                if (messageLower == "/start")
                {
                    #region /start
                    if (!groups.Contains(chatID))
                    {
                        groups.Add(chatID);
                        Settings.Set("TelegramBotCenterGroups", string.Join(",", groups.ToArray()));

                        SendMessage("telkom.center is now online", chatID);
                    }
                    #endregion
                }
                else if (groups.Contains(chatID))
                {
                    int lastFromID = LastFromID(chatID);

                    if (messageLower == "/client" || messageLower == "/clients" || messageLower == "/session" || messageLower == "/sessions")
                    {
                        #region /client

                        int sc = Provider.GetSessionCount();
                        if (sc == 0)
                        {
                            SendMessage("There are no active session currently on telkom.center", chatID, messageID, fromID);
                        }
                        else SendMessage("There " + (sc == 1 ? "is" : "are") + " " + sc + " active session" + (sc > 1 ? "s" : "") +
                            " on telkom.center at the moment", chatID, messageID, fromID);

                        #endregion
                    }
                    else if (messageLower == "/cpu")
                    {
                        #region /cpu
                        float availableRam = ramCounter.NextValue();
                        float totalRam = (float)Math.Round((double)(computerInfo.TotalPhysicalMemory / 1024000));
                        float percentageRam = 100 - (float)Math.Round(availableRam / totalRam * 100, 2);

                        string rsp = "";
                        string oname = null;
                        int ocpu = 0;
                        foreach (ManagementObject mo in mos.Get())
                        {
                            string cname = mo["Name"].ToString();
                            ocpu += int.Parse(mo["NumberOfLogicalProcessors"].ToString());
                            if (oname != null && cname != oname)
                            {
                                rsp += cname + " " + ocpu + " Logical Processors\n";
                                oname = cname;
                                ocpu = 0;
                            }
                            else if (oname == null) oname = cname;
                        }
                        if (oname != null)
                        {
                            rsp += oname + " " + ocpu + " Logical Processors\n";
                        }

                        rsp += "CPU Usage: " + Math.Round(cpuCounter.NextValue(), 2) + "%\n" +
                            "RAM Usage: " + percentageRam + "%\n" +
                            "RAM Available/Total: " + availableRam + "MB/" + totalRam + "MB";

                        cpuCounter.NextValue();

                        SendMessage(rsp, chatID, messageID, fromID);

                        #endregion
                    }
                    else if (messageLower.StartsWith("/probe") || messageLower == "/probestatus")
                    {
                        #region Necrow Commands

                        if (Server.IsNecrowConnected())
                        {
                            ClearTalk(fromID);

                            if (messageLower == "/probe")
                            {
                                #region /probe
                                SendMessage(new string[] { "Which node you want to probe?", "Which node?", "Please specify the node you want to probe?" }, chatID, messageID, fromID);
                                SetTalk("necrow>probe>what?", null, messageID, fromID);
                                #endregion
                            }
                            else if (messageLower.StartsWith("/probe "))
                            {
                                #region /probe <node name>
                                string[] tokens = messageLower.Split(StringSplitTypes.Space, StringSplitOptions.RemoveEmptyEntries);
                                MessageProbeExecute(tokens[1], chatID, messageID, fromID);
                                #endregion
                            }
                            else if (messageLower == "/probestatus")
                            {
                                #region /probestatus

                                ServerNecrowServiceMessage m = new ServerNecrowServiceMessage(NecrowServiceMessageType.ProbeStatus);
                                m.IdentifierData1 = chatID;
                                m.IdentifierData2 = messageID;
                                m.IdentifierData3 = fromID;

                                Server.NecrowSend(m);

                                #endregion
                            }
                        }
                        else
                        {
                            #region not available
                            if (noticeWhenNecrowIsBackUp.Contains(chatID))
                                SendMessage(new string[] {
                                    "I am sorry, Necrow is not available right now. I will notice you whenever the Necrow is back up",
                                    "Necrow is not available right now, I'll let you know if the Necrow is back up",
                                    "My apologize, Necrow is not available. I will note you whenever the Necrow is back up",
                                    "Necrow is not available. I will notice you"
                                }, chatID, messageID, fromID);
                            else
                            {
                                SendMessage(new string[] {
                                    "I am sorry, Necrow is not available right now",
                                    "Necrow is not available",
                                    "My apologize, Necrow is not available",
                                    "I am sorry, Necrow's not available right now"
                                }, chatID, messageID, fromID);
                                SendMessage(new string[] {
                                    "Would you want to be noticed when the Necrow is back up?",
                                    "Do you want to be noticed whenever the Necrow is back up?",
                                    "You want to know whether the Necrow is back up?"
                                }, chatID, messageID, fromID, 1500, delegate (Message m) { SetTalk("necrowup>noticed?", null, 0, fromID); });
                            }
                            #endregion
                        }

                        #endregion
                    }
                    else if (messageFamily == "called")
                    {
                        #region called
                        if (GetTalk(fromID).Item1 == null)
                        {
                            SendMessage(new string[] { "Yes Master", "Hey", "You called?", "Yeah what's up", "What's up",
                                "Hi", "Hello", "Hello... It's me", "Ahay", "Heiii", "Hiii", "Hey", "What's up", "Yo", "Yap", "What's up", "Hello hello",
                                "Master", "Yes My Lord", "Yes Milord", "Yes My Master"
                            }, chatID, messageID, fromID);
                            SetTalk("introduced", null, 0, fromID);
                        }
                        #endregion
                    }
                    else if (messageFamily == "called>salam")
                    {
                        #region called>salam
                        if (GetTalk(fromID).Item1 == null)
                        {
                            SendMessage(new string[] {
                                "Wassalamualaikum Warahmatullahi Wabarakatuh",
                                "Wassalamualaikum",
                                "Wassalamualaikum Wr Wb",
                            }, chatID, messageID, fromID);
                            SetTalk("introduced", null, 0, fromID);
                        }
                        #endregion
                    }
                    else if (messageLower.EndsWith("ysh") && messageLower.IndexOf("robot", "telkom.center", "center") > -1)
                    {
                        #region ysh

                        SendMessage(new string[] {
                            "I am not honorable to bear that title. It should be Mas Desy",
                            "I am sorry I am not honorable bear that title",
                            "You mean Mas Desy ysh?"
                        }, chatID, messageID, fromID);

                        #endregion
                    }
                    else if (GetTalk(fromID).Item1 == "necrowup>noticed?")
                    {
                        #region necrowup>noticed?
                        if (messageFamily == "yes")
                        {
                            if (Server.IsNecrowConnected())
                                SendMessage(new string[] {
                                    "Necrow is already back up, you can try it again",
                                    "Necrow is already up now, you can try it again",
                                    "Necrow's up, just try it again",
                                }, chatID, messageID, fromID);
                            else
                            {
                                SendMessage(new string[] {
                                    "All right, I'll notice you whenever the Necrow is back up",
                                    "Okay, I will notice you",
                                    "You got it, I will let you know"
                                }, chatID, messageID, fromID);
                                noticeWhenNecrowIsBackUp.Add(chatID);
                            }
                            ClearTalk(fromID);
                        }
                        else if (messageFamily == "no")
                        {
                            if (Server.IsNecrowConnected()) SendMessage("Necrow is already back up", chatID, messageID, fromID);
                            else SendMessage("Ok. Please try again later", chatID, messageID, fromID);
                            SetTalk("smalltalk", null, 0, fromID);
                        }
                        #endregion
                    }
                    else if (GetTalk(fromID).Item1 == "necrow>probe>what?")
                    {
                        #region necrow>probe>what?

                        if (Server.IsNecrowConnected())
                        {
                            string[] tokens = messageLower.Split(StringSplitTypes.Space, StringSplitOptions.RemoveEmptyEntries);
                            if (tokens.Length > 1)
                            {
                                SendMessage(new string[] {
                                    "Do you mean you want to probe " + tokens[0] + "?",
                                    "You want to probe " + tokens[0] + "?",
                                    "You mean " + tokens[0] + "?",
                                    "Please confirm you want to probe " + tokens[0] + "?"
                                }, chatID, messageID, fromID);
                                SetTalk("necrow>probe>confirm?", tokens[0], messageID, fromID);
                            }
                            else
                            {
                                MessageProbeExecute(tokens[0], chatID, messageID, fromID);
                            }
                        }
                        else
                        {
                            SendMessage("I am sorry, Necrow is getting unavailable right now. Would you want to be noticed when the Necrow is back up?", chatID, messageID, fromID);
                            SetTalk("necrowup>noticed?", null, 0, fromID);
                        }

                        #endregion
                    }
                    else if (GetTalk(fromID).Item1 == "necrow>probe>confirm?")
                    {
                        #region necrow>probe>confirm?

                        if (messageFamily == "yes")
                        {
                            MessageProbeExecute((string)GetTalk(fromID).Item2, chatID, GetTalk(fromID).Item3, fromID);
                        }
                        else if (messageFamily == "no")
                        {
                            SendMessage(new string[] {
                                "Well, I am getting a little bit confused",
                                "Well",
                                "Hmm",
                                "All right, tell me if you need something",
                                "Okay, tell me if you need something",
                                "All right"
                            }, chatID, messageID, fromID);
                            SetTalk("smalltalk>null", null, 0, fromID);
                        }

                        #endregion
                    }
                    else if (GetTalk(fromID).Item1 == "necrow>probe>retry?")
                    {
                        #region necrow>probe>confirm?

                        if (messageFamily == "yes")
                        {
                            MessageProbeExecute((string)GetTalk(fromID).Item2, chatID, GetTalk(fromID).Item3, fromID);
                        }
                        else if (messageFamily == "no")
                        {
                            SendMessage(new string[] {
                                "Okay", "Got it", "I understand", "Understood", "You got it", "No problem", "That's all right"
                            }, chatID, messageID, fromID);
                            SetTalk("smalltalk", null, 0, fromID);
                        }

                        #endregion
                    }
                    else if (GetTalk(fromID).Item1 == "necrow>probe>forceinactive?")
                    {
                        #region necrow>probe>forceinactive?

                        if (messageFamily == "yes")
                        {
                            SendMessage(new string[] {
                                "Got it, I'll probe " + (string)GetTalk(fromID).Item2 + " shortly",
                                "Understood, I'll probe " + (string)GetTalk(fromID).Item2 + " shortly",
                                "You got it",
                                "I understand"
                            }, chatID, messageID, fromID);

                            ServerNecrowServiceMessage m = new ServerNecrowServiceMessage(NecrowServiceMessageType.Probe);
                            m.Data = new object[] { GetTalk(fromID).Item2 };
                            m.IdentifierData1 = chatID;
                            m.IdentifierData2 = GetTalk(fromID).Item3;
                            m.IdentifierData3 = fromID;

                            Server.NecrowSend(m);
                            SetTalk("smalltalk>update", null, 0, fromID);
                        }
                        else if (messageFamily == "no")
                        {
                            SendMessage(new string[] { "Understood", "I understand", "You bet", "Love it", "No problem" }, chatID, messageID, fromID);
                            SetTalk("smalltalk", null, 0, fromID);
                        }

                        #endregion
                    }
                    else if (GetTalk(fromID).Item1 == "necrow>probe>continueprobe?")
                    {
                        #region necrow>probe>continueprobe?

                        if (messageFamily == "yes")
                        {
                            SendMessage(new string[] { "All right, continuing the probe", "Got it, we'll continue the probe soon", "Affrimative", "Understood", "You got it" }, chatID, messageID, fromID);

                            ServerNecrowServiceMessage m = new ServerNecrowServiceMessage(NecrowServiceMessageType.Probe);
                            m.Data = new object[] { GetTalk(fromID).Item2, "FORCE" };
                            m.IdentifierData1 = chatID;
                            m.IdentifierData2 = GetTalk(fromID).Item3;
                            m.IdentifierData3 = fromID;

                            Server.NecrowSend(m);
                            SetTalk("smalltalk>update", null, 0, fromID);
                        }
                        else if (messageFamily == "no")
                        {
                            SendMessage(new string[] { "All right I'll stop the probe", "All right I'll stop", "Got it I'll stop", "I will stop the probe", "No problem, I'll stop the probe", "Understood, I'll stop the probe" }, chatID, messageID, fromID);
                            SetTalk("smalltalk", null, 0, fromID);
                        }

                        #endregion
                    }
                    else if (GetTalk(fromID).Item1 == "necrow>probe>choice?")
                    {
                        #region necrow>probe>choice?

                        string choiceString = (string)GetTalk(fromID).Item2;
                        string[] choices = choiceString.Split(StringSplitTypes.Comma, StringSplitOptions.RemoveEmptyEntries);

                        if (choices.Length == 1)
                        {
                            if (messageFamily == "yes")
                            {
                                MessageProbeExecute(choices[0], chatID, messageID, fromID);
                            }
                            else if (messageFamily == "no")
                            {
                                SendMessage(new string[] { "That's okay", "I understand", "No problem", "Okay" }, chatID, messageID, fromID);
                                SetTalk("smalltalk", null, 0, fromID);
                            }
                        }
                        else
                        {
                            if (messageLower.ToUpper().InOf(choices) > -1)
                            {
                                MessageProbeExecute(messageLower, chatID, messageID, fromID);
                            }
                        }

                        #endregion
                    }
                    else if (GetTalk(fromID).Item1 == "smalltalk")
                    {
                        #region smalltalk

                        if (messageFamily == "thanks")
                        {
                            SendMessage(new string[] { "You're welcome", "Welcome", "No problem", "Hope that's help", "Any time" }, chatID, messageID, fromID);
                            ClearTalk(fromID);
                        }

                        #endregion
                    }
                    else if (GetTalk(fromID).Item1 == "smalltalk>null")
                    {
                        #region smalltalk>null

                        if (messageFamily == "thanks")
                        {
                            SendMessage(new string[] { "I don't think you should thank me", "Well", "Ok" }, chatID, messageID, fromID);
                            ClearTalk(fromID);
                        }

                        #endregion
                    }
                    else if (GetTalk(fromID).Item1 == "smalltalk>update")
                    {
                        #region smalltalk>update

                        if (messageFamily == "thanks")
                        {
                            SendMessage(new string[] { "No problem, I'll let you know", "Ok", "Got it", "I'll let you know", "Yep" }, chatID, messageID, fromID);
                            ClearTalk(fromID);
                        }

                        #endregion
                    }
                    else if (GetTalk(fromID).Item1 != null)
                    {
                        #region fallback
                        ClearTalk(fromID);
                        #endregion
                    }

                    #region Update Last From
                    if (!lastFromIDs.ContainsKey(chatID)) lastFromIDs.Add(chatID, fromID);
                    else lastFromIDs[chatID] = fromID;
                    #endregion
                }
                #endregion
            }
            else
            {
                #region Chats

                #endregion
            }

            
        }
    }
}
