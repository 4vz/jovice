using Aphysoft.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UAParser;
using Humanizer;
using System.Diagnostics;
using System.Net.Http;
using Telegram.Bot;
using Telegram.Bot.Types;
using System.Configuration;

namespace Center
{
    internal class Conversation
    {
        #region Fields

        private static Database share = Share.Database;

        private static Dictionary<string, Conversation> conversations = new Dictionary<string, Conversation>();

        private string id;

        public string ID { get => id; }

        #endregion

        #region Constructors

        public Conversation()
        {

        }

        #endregion

        #region Methods

        public static Conversation Get(string id)
        {
            if (conversations.ContainsKey(id)) return conversations[id];
            return null;
        }

        public static Conversation Create(string id)
        {
            if (!conversations.ContainsKey(id))
                conversations.Add(id, new Conversation());

            return null;
        }

        #endregion
    }

    

    internal class BotGroup
    {
        #region Fields

        private static Database share = Share.Database;

        private static Dictionary<long, BotGroup> groups = new Dictionary<long, BotGroup>();

        public static List<BotGroup> Groups
        {
            get
            {
                if (groups.Count == 0)
                {
                    Result result = share.Query("select * from BotGroup");

                    if (result.Count > 0)
                    {
                        lock (groups)
                        {
                            foreach (Row row in result)
                            {
                                long id = row["BG_TelegramID"].ToLong();
                                BotGroup group = new BotGroup(row["BG_ID"].ToString(), id, row["BG_Name"].ToString());
                                groups.Add(id, group);
                            }
                        }                        
                    }
                }

                List<BotGroup> lg = new List<BotGroup>();
                foreach (KeyValuePair<long, BotGroup> pair in groups)
                {
                    lg.Add(pair.Value);
                }
                return lg;
            }
        }

        private string id;

        public string ID { get => id; }

        private long telegramID;

        public long TelegramID
        {
            get { return telegramID; }
        }

        private string name;

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                share.Execute("update BotGroup set BG_Name = {0} where BG_ID = {1}", name, id);
            }
        }

        #endregion

        #region Constructors

        public BotGroup(string id, long telegramID, string name)
        {
            this.id = id;
            this.telegramID = telegramID;
            this.name = name;
        }

        #endregion

        #region Methods

        public static BotGroup Get(long telegramID)
        {
            if (groups.ContainsKey(telegramID)) return groups[telegramID];
            else
            {
                Result result = share.Query("select * from BotGroup where BG_TelegramID = {0}", telegramID);

                if (result.Count == 1)
                {
                    Row row = result[0];
                    BotGroup group = new BotGroup(row["BG_ID"].ToString(), telegramID, row["BG_Name"].ToString());
                    lock (groups)
                    {
                        groups.Add(telegramID, group);
                    }

                    return group;
                }
                else
                    return null;
            }
        }

        public static BotGroup Create(long telegramID, string name)
        {
            if (!groups.ContainsKey(telegramID))
            {
                Insert insert = share.Insert("BotGroup");
                string id = Database.ID();

                insert.Value("BG_ID", id);
                insert.Value("BG_Name", name);
                insert.Value("BG_TelegramID", telegramID);
                insert.Execute();

                BotGroup group = new BotGroup(id, telegramID, name);
                lock (groups)
                {
                    groups.Add(telegramID, group);
                }

                return group;
            }
            else return groups[telegramID];
        }

        public void Remove()
        {
            share.Execute("delete from BotGroup where BG_ID = {0}", id);
            lock (groups)
            {
                groups.Remove(telegramID);
            }
        }

        #endregion
    }

    internal class BotUser
    {
        #region Fields

        private static Database share = Share.Database;

        private static Dictionary<int, BotUser> users = new Dictionary<int, BotUser>();

        private string id;

        public string ID
        {
            get { return id; }
        }

        private int telegramID;

        public int TelegramID
        {
            get { return telegramID; }
        }

        private string name;

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                Share.Database.Execute("update BotUser set BU_TelegramName = {0} where BU_ID = {1}", name, id);
            }
        }

        #endregion

        #region Constructors

        public BotUser(string id, int telegramID, string name)
        {
            this.id = id;
            this.telegramID = telegramID;
            this.name = name;
        }

        #endregion

        #region Methods

        public static BotUser Get(int telegramID)
        {
            if (users.ContainsKey(telegramID)) return users[telegramID];
            else
            {
                Result result = share.Query("select * from BotUser where BU_TelegramID = {0}", telegramID);

                if (result.Count == 1)
                {
                    Row row = result[0];
                    BotUser user = new BotUser(row["BU_ID"].ToString(), telegramID, row["BU_TelegramName"].ToString());
                    lock (users)
                    {
                        users.Add(telegramID, user);
                    }

                    return user;
                }
                else
                    return null;
            }
        }

        public static BotUser Create(int telegramID, string name)
        {
            if (!users.ContainsKey(telegramID))
            {
                Insert insert = share.Insert("BotUser");
                string id = Database.ID();

                insert.Value("BU_ID", id);
                insert.Value("BU_TelegramName", name);
                insert.Value("BU_TelegramID", telegramID);
                insert.Execute();

                BotUser user = new BotUser(id, telegramID, name);
                lock (users)
                {
                    users.Add(telegramID, user);
                }

                return user;
            }
            else return users[telegramID];
        }

        #endregion

    }
    
    internal static class TelegramBot
    {
        #region Fields
        
        private static TelegramBotClient telegramBot = null;
        private static string userName = null;
        private static DateTime startDate;

        private static Dictionary<int, BotUser> botUsers = new Dictionary<int, BotUser>();
        
        #endregion
        
        private static List<string> WordCombination(string pattern)
        {
            List<string> list = new List<string>();
            List<Tuple<char, int>> parts = new List<Tuple<char, int>>();

            char lastChar = '-';
            foreach (char c in pattern)
            {
                if (char.IsDigit(c))
                {
                    if (lastChar != '-')
                    {
                        parts.Add(new Tuple<char, int>(lastChar, int.Parse(c + "")));
                        lastChar = '-';
                    }
                }
                else if (lastChar != c)
                {
                    if (lastChar != '-')
                        parts.Add(new Tuple<char, int>(lastChar, 1));
                    lastChar = c;
                }
            }
            if (lastChar != '-') parts.Add(new Tuple<char, int>(lastChar, 1));
            WordCombinationCreate(list, new StringBuilder(), parts, 0);
            return list;
        }

        private static void WordCombinationCreate(List<string> result, StringBuilder sb, List<Tuple<char, int>> parts, int index)
        {
            if (index < parts.Count)
            {
                Tuple<char, int> part = parts[index];
                for (int i = 0; i < part.Item2; i++)
                {
                    StringBuilder sbc = new StringBuilder();
                    for (int j = 0; j < (i + 1); j++) sbc.Append(part.Item1);
                    StringBuilder sbClone = new StringBuilder(sb.ToString());
                    WordCombinationCreate(result, sbClone.Append(sbc.ToString()), parts, index + 1);
                }
            }
            else result.Add(sb.ToString());
        }

        internal static void Init()
        {
#if DEBUG
            telegramBot = new TelegramBotClient(ConfigurationManager.AppSettings["telegram_debug"]);
            userName = "TelkomCenterDevBot";
#else
            telegramBot = new TelegramBotClient(ConfigurationManager.AppSettings["telegram"]);
            userName = "TelkomCenterBot";
#endif
            //Database center = Share.Database;
            //Result result;

            telegramBot.OnMessage += OnMessage;
            telegramBot.MessageOffset = -1; // get only last 1 message before this bot even start
            startDate = DateTime.UtcNow;

            //result = center.Query("select * from BotUser");
            //foreach (Row row in result)
            //{
            //    int tid = row["BU_TelegramID"].ToInt();
            //    string id = row["BU_ID"].ToString();
            //    string name = row["BU_Name"].ToString();
            //}

            Intent.Init("center");

            telegramBot.StartReceiving();
        }

        public delegate void SendMessageCallBack(Message message);

        #region Events

        internal static void NecrowOnline()
        {
        }

        internal static void NecrowOffline()
        {
        }

        internal static void NecrowProbeStatus(ServerNecrowServiceMessage m)
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

        internal static void NecrowProbe(ServerNecrowServiceMessage m)
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
                //SetTalk("necrow>probe>continueprobe?", node, (int)m.IdentifierData2, (int)m.IdentifierData3);
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
                //SetTalk("necrow>probe>retry?", node, (int)m.IdentifierData2, (int)m.IdentifierData3);
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
                //SetTalk("smalltalk", null, 0, (int)m.IdentifierData3);
            }
        }

        #endregion

        #region Send Message

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
            if (telegramBot.IsReceiving)
            {
                Thread thread = new Thread(new ThreadStart(delegate ()
                {
                    if (delay > 0) Thread.Sleep(delay);

                    int toReply = 0;

                    if (replyMessageID > 0)
                    {
                        if (chatID != fromID)
                        {
                            //if (forceReply || LastFromID(chatID) != fromID) toReply = replyMessageID;
                        }
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
            else
            {
                //Service.Debug("Message not sent: " + message);
            }
        }

        #endregion

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
                        nodeName + " is not active. Do you still want to probe this node?"
                    }, chatID, messageID, fromID);
                    //SetTalk("necrow>probe>forceinactive?", nodeName, messageID, fromID);
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

                    //ClearTalk(fromID);
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

                                    //SetTalk("necrow>probe>choice?", string.Join(",", possibilities.ToArray()), messageID, fromID);
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

                                    //SetTalk("necrow>probe>choice?", string.Join(",", possibilities.ToArray()), messageID, fromID);
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

                                    //SetTalk("smalltalk", null, 0, fromID);
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

                                //SetTalk("necrow>probe>choice?", possibilities[0], messageID, fromID);
                            }
                        }
                        else
                        {
                            SendMessage(new string[] {
                                "I cannot find the specified node in my database",
                                "I am sorry, I cannot find " + probe + " in my database"
                            }, chatID, messageID, fromID, true);

                            //SetTalk("smalltalk", null, 0, fromID);
                        }
                    }
                    else if (depan.StartsWith("me", "pe", "ces", "hrb") && !tengah.StartsWith("d"))
                    {
                        SendMessage(new string[] { "The second part of the node you specified is not correct",
                            "It should be D<something> for the second part of the node you specified", 
                            "The D-part of specified node is not correct"
                        }, chatID, messageID, fromID, true);

                        //SetTalk("smalltalk", null, 0, fromID);
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

                        //SetTalk("smalltalk", null, 0, fromID);
                    }
                }
            }
        }

        #endregion

        private static void OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            long chatID = e.Message.Chat.Id;
            int fromID = e.Message.From.Id;            

            bool isPrivateMessage = (e.Message.Chat.Id == e.Message.From.Id);

            BotGroup group = null;
            BotUser user = null;

            if (!isPrivateMessage)
            {
                group = BotGroup.Get(chatID);
                if (group == null)
                    group = BotGroup.Create(chatID, e.Message.Chat.Title);
            }

            user = BotUser.Get(fromID);

            string userNameData = e.Message.From.Username + ":" + e.Message.From.FirstName + ":" + e.Message.From.LastName;

            if (user == null)
                user = BotUser.Create(fromID, userNameData);
            else if (user.Name != userNameData)
                user.Name = userNameData;

           
            if (e.Message.NewChatTitle != null)
            {
                // group new name
                group.Name = e.Message.NewChatTitle;
            }
            else if (e.Message.NewChatMember != null)
            {
                if (e.Message.NewChatMember.Username == userName)
                {
                }
            }
            else if (e.Message.LeftChatMember != null)
            {
                // someone has been left
                if (e.Message.LeftChatMember.Username == userName)
                {
                    // bot has been booted from group
                    group.Remove();
                }
            }
            else if (e.Message.Text != null)
            {
                string message = e.Message.Text;

                Stopwatch intentProcess = new Stopwatch();
                intentProcess.Start();

                if (e.Message.Date < startDate) return;

                Intent intent = Intent.Parse(message);

                if (intent != null)
                {
                    Conversation conversation = Conversation.Get(fromID + "_" + chatID);



                    if (isPrivateMessage)
                    {
                        Service.Debug("PM from: " + user.Name);


                        // pm
                        if (conversation == null)
                        {
                            //conversation = Conversation.Create(conversationID);
                        }
                    }
                    else
                    {
                        Service.Debug("group: " + group.Name + ", from: " + user.Name);

                        // group
                        if (conversation != null)
                        {

                        }
                        else
                        {

                        }
                    }

                    foreach (Assumption ent in intent.Assumptions)
                    {
                        Service.Debug("intent: " + ent.Intent + "; asking: " + ent.Asking + "; mentionMyName: " + ent.MentionMyName);

                    }
                }

                Service.Debug("elapsed: " + intentProcess.ElapsedMilliseconds + "ms");
                intentProcess.Stop();
            }
        }

        private static void GroupChat()
        {

        }

        // obsoleted

        #region
            /*
        private static void OnOldMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            Database share = Share.Database;

            //Service.Debug(DateTime.UtcNow + ", incoming message: " + e.Message.Date);
            if (e.Message.Date < startDate) return; // avoid old message, and just discard em
            string message = e.Message.Text;

            if (message == null) return;

            message = message.Trim();

            bool isPrivateMessage = (e.Message.Chat.Id == e.Message.From.Id);

            long chatID = e.Message.Chat.Id;
            int fromID = e.Message.From.Id;

            string name = null;
            string nick = null;

            if (e.Message.From.FirstName != null) name = (e.Message.From.FirstName + " " + e.Message.From.LastName).Trim();
            if (identities.ContainsKey(fromID)) nick = identities[fromID].Nick;

            string messageTalk = GetTalk(fromID).Item1;

            string messageLower = message.ToLower();
            int messageID = e.Message.MessageId;

            string messageIntent = null;
            string messageIntent2 = null;
            string messageMention = null;

            if (messageLower.IndexOf("robot", "center", "telkom.center") > -1) messageMention = "bot";

            #region Intent

            if (messageLower.InOf("/yes", "yes", "ya", "yaa", "yaaa", "yess", "yesss", "hell yes", "yeah", "iya", "yup", "mainkan", "yuup", "yuuup", "yep", "yeep") > -1) messageIntent = "yes";
            else if (messageLower.InOf("/no", "no", "noo", "hell no", "nooo", "tidak", "gak", "enggak", "ga", "nope") > -1) messageIntent = "no";
            else if (messageLower.IndexOf("/pass", "later", "next time", "cancel", "tomorrow") > -1) messageIntent = "pass";
            else if (messageLower.IndexOf("terima kasih", "makasih", "makacih", "makasi", "makaci", "thank you", "thanks", "terbaik", "merci", "gracias", "gratzie") > -1) messageIntent = "thanks";
            else if (messageLower.IndexOf("sorry", "maaf", "apologize") > -1) messageIntent = "sorry";
            else if (messageLower.IndexOf("hey", "hi", "hello", "helo", "halo", "hoi", "hallo") > -1 && messageMention == "bot") messageIntent = "called";
            else if (messageLower.IndexOf("assalamualaikum", "assalamualaykum", "asslmualaikum", "assalamu'alaikum") > -1 && messageMention == "bot") messageIntent = "called>salam";
            else if (messageLower.IndexOf("semangat pagi") > -1) messageIntent = "called>pagi3";
            else if (messageLower.IndexOf("pagi", "morning", "bonjour") > -1 && messageMention == "bot" && DateTime.Now.TimeOfDay.Hours.Between(3, 11)) messageIntent = "called>pagi";
            else if (messageLower.IndexOf("siang", "afternoon") > -1 && messageMention == "bot" && DateTime.Now.TimeOfDay.Hours.Between(12, 21)) messageIntent = "called>siang";
            else if (messageLower.IndexOf("sore") > -1 && messageMention == "bot" && DateTime.Now.TimeOfDay.Hours.Between(15, 18)) messageIntent = "called>sore";
            else if (messageLower.IndexOf("evening") > -1 && messageMention == "bot" && DateTime.Now.TimeOfDay.Hours.Between(22, 23)) messageIntent = "called>malam";
            else if (messageLower.IndexOf("night") > -1 && messageMention == "bot" && (DateTime.Now.TimeOfDay.Hours.Between(22, 23) || DateTime.Now.TimeOfDay.Hours.Between(0, 2))) messageIntent = "called>night";
            else if (messageLower.IndexOf("malam", "malem") > -1 && messageMention == "bot" && (DateTime.Now.TimeOfDay.Hours.Between(18, 23) || DateTime.Now.TimeOfDay.Hours.Between(0, 2))) messageIntent = "called>malam";
            else if (messageLower.IndexOf("pagi", "morning", "siang", "afternoon", "sore", "evening", "malam", "malem") > -1 && messageMention == "bot") messageIntent = "called>correcttime";

            if (messageLower.IndexOf("system", "server") > -1 && messageLower.IndexOf("health", "info", "status") > -1) messageIntent2 = "serverstatus";

            if (messageIntent2 != null)
            {
                // swap between mi and mi2
                string mi = messageIntent;
                messageIntent = messageIntent2;
                messageIntent2 = mi;
            }

            #endregion

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

                        int sessionsCount = Provider.SessionInstances.Count;

                        Result result = share.Query("select * from Session where SS_ClientsCount > 0 order by SS_ClientsCount desc");

                        Parser parser = Parser.GetDefault();
                        

                        Dictionary<string, Tuple<string, string, DateTime, int>> sessions = new Dictionary<string, Tuple<string, string, DateTime, int>>();
                        foreach (Row row in result)
                        {
                            string sid = row["SS_SID"].ToString();
                            string ip = row["SS_IPAddress"].ToString();
                            string uas = row["SS_UserAgent"].ToString();
                            DateTime created = row["SS_Created"].ToDateTime();
                            int clients = row["SS_ClientsCount"].ToInt();

                            sessions.Add(sid, new Tuple<string, string, DateTime, int>(ip, uas, created, clients));
                        }

                        Dictionary<string, int> addressSessionCount = new Dictionary<string, int>();
                        Dictionary<string, int> subnetCount = new Dictionary<string, int>();
                        Dictionary<string, int> osCount = new Dictionary<string, int>();
                        List<string> userAgentSession = new List<string>();
                        Dictionary<string, int> userAgentCount = new Dictionary<string, int>();
                        Dictionary<string, DateTime> ipSessionCreated = new Dictionary<string, DateTime>();

                        string clientCountMostIP = null;
                        int clientCountMost = 1;

                        foreach (KeyValuePair<string, StreamSessionInstance> pair in Provider.SessionInstances)
                        {
                            string sid = pair.Key;
                            if (sessions.ContainsKey(sid))
                            {
                                Tuple<string, string, DateTime, int> data = sessions[sid];
                                string ip = data.Item1;
                                string uas = data.Item2;
                                DateTime created = data.Item3;
                                int clientCount = data.Item4;

                                if (clientCount > clientCountMost)
                                {
                                    clientCountMostIP = ip;
                                    clientCountMost = clientCount;
                                }                                

                                ipSessionCreated.Add(ip + "_" + sid, created);

                                if (addressSessionCount.ContainsKey(ip))
                                    addressSessionCount[ip]++;
                                else
                                {
                                    addressSessionCount.Add(ip, 1);

                                    IPNetwork ipn = IPNetwork.Parse(ip + "/24");
                                    string subnet = ipn.Network.ToString();

                                    if (subnetCount.ContainsKey(subnet))
                                        subnetCount[subnet]++;
                                    else
                                        subnetCount.Add(subnet, 1);

                                    OS os = parser.ParseOS(uas);

                                    string osKey = (os.Family + " " + os.Major).Trim();

                                    if (osCount.ContainsKey(osKey)) osCount[osKey]++;
                                    else osCount.Add(osKey, 1);
                                }

                                UserAgent ua = parser.ParseUserAgent(uas);
                                string uaKey = ua.Family.Trim();

                                string uaSKey = uaKey + "_" + ip;

                                if (userAgentSession.Contains(uaSKey)) { }
                                else
                                {
                                    userAgentSession.Add(uaSKey);

                                    if (userAgentCount.ContainsKey(uaKey)) userAgentCount[uaKey]++;
                                    else userAgentCount.Add(uaKey, 1);
                                }
                            }
                        }

                        List<KeyValuePair<string, int>> subnetCountList = subnetCount.ToList();
                        subnetCountList.Sort((pair2, pair1) => pair1.Value.CompareTo(pair2.Value));

                        string subnetInfo = "";

                        if (subnetCountList.Count > 2)
                            subnetInfo = "Out of " + subnetCountList.Count + " subnets, most of them coming from subnet " + subnetCountList[0].Key + " (" + subnetCountList[0].Value + ") and " + subnetCountList[1].Key + " (" + subnetCountList[1].Value + ").";
                        else if (subnetCountList.Count == 2)
                            subnetInfo = "All connections are from subnet " + subnetCountList[0].Key + " (" + subnetCountList[0].Value + ") and " + subnetCountList[1].Key + " (" + subnetCountList[1].Value + ").";
                        else if (subnetCountList.Count == 1)
                            subnetInfo = "All connections are from subnet " + subnetCountList[0].Key + ".";

                        if (subnetInfo != "") subnetInfo += "\n";

                        List<KeyValuePair<string, int>> osCountList = osCount.ToList();
                        osCountList.Sort((pair2, pair1) => pair1.Value.CompareTo(pair2.Value));
                        
                        string osInfo = "";

                        if (osCountList.Count >= 3)
                            osInfo = "Top there OS used by clients are " + osCountList[0].Key + " (" + osCountList[0].Value + "), " + osCountList[1].Key + " (" + osCountList[1].Value + ") and " + osCountList[2].Key + " (" + osCountList[2].Value + ").";
                        else if (osCountList.Count == 2)
                            osInfo = "All clients are using either " + osCountList[0].Key + " (" + osCountList[0].Value + ") or " + osCountList[1].Key + " (" + osCountList[1].Value + ").";
                        else if (osCountList.Count == 1)
                            osInfo = "All clients are using " + osCountList[0].Key + ".";

                        if (osInfo != "") osInfo += "\n";

                        List<KeyValuePair<string, int>> userAgentCountList = userAgentCount.ToList();
                        userAgentCountList.Sort((pair2, pair1) => pair1.Value.CompareTo(pair2.Value));

                        string userAgentInfo = "";

                        if (userAgentCountList.Count >= 3)
                            userAgentInfo = "Top three browsers used by clients are " + userAgentCountList[0].Key + " (" + userAgentCountList[0].Value + "), " + userAgentCountList[1].Key + " (" + userAgentCountList[1].Value + ") and " + userAgentCountList[2].Key + " (" + userAgentCountList[2].Value + ").";
                        else if (userAgentCountList.Count == 2)
                            userAgentInfo = "All clients are using either " + userAgentCountList[0].Key + " (" + userAgentCountList[0].Value + ") or " + userAgentCountList[1].Key + " (" + userAgentCountList[1].Value + ").";
                        else if (userAgentCountList.Count == 1)
                            userAgentInfo = "All clients are using " + userAgentCountList[0].Key + ".";

                        if (userAgentInfo != "") userAgentInfo += "\n";

                        List<KeyValuePair<string, DateTime>> ipSessionCreatedList = ipSessionCreated.ToList();
                        ipSessionCreatedList.Sort((pair2, pair1) => pair2.Value.CompareTo(pair1.Value));

                        string theLongestInfo = "";

                        if (ipSessionCreatedList.Count > 0)
                            theLongestInfo = "The longest current connected client is IP " + ipSessionCreatedList[0].Key.Split('_')[0] + " for " + (DateTime.UtcNow - ipSessionCreatedList[0].Value).Humanize() + ".";

                        if (theLongestInfo != "") theLongestInfo += "\n";

                        string mostClients = "";

                        if (clientCountMostIP != null)
                            mostClients = "Client on IP " + clientCountMostIP + " has the most tab opened in his/her browser: " + clientCountMost + " tabs";

                        if (mostClients != "") mostClients += "\n";


                        if (sessionsCount > 0)
                        {
                            SendMessage(string.Format(@"
There are currently {0} active connections to telkom.center.
{1}{2}{3}{4}{5}
                            ", addressSessionCount.Count, 
                            subnetInfo, 
                            osInfo, 
                            userAgentInfo,
                            theLongestInfo,
                            mostClients).Trim(), chatID, messageID, fromID);
                        }
                        else SendMessage("There are no active session currently on telkom.center", chatID, messageID, fromID);

                        #endregion
                    }
                    else if (messageLower == "/cpu" || messageIntent == "serverstatus")
                    {
                        #region /cpu
                        
                        SendMessage(
                            string.Format("{0}\nCPU Usage: {1}%\nRAM Usage: {2}%\nRAM Available/Total: {3}MB/{4}MB", 
                            Server.CPUClass, Server.CPUUsage, Server.PercentageRam, Server.AvailableRam, Server.TotalRam),
                            chatID, messageID, fromID);

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
                                if (nick == null)
                                    SendMessage(new string[] {
                                        "Which node you want to probe?",
                                        "Which node?",
                                        "Please specify the node you want to probe?" }, chatID, messageID, fromID);
                                else
                                    SendMessage(new string[] {
                                        "Which node you want to probe, " + nick + "?",
                                        "Which node?",
                                        "Please specify the node you want to probe?" }, chatID, messageID, fromID);

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
                            {
                                if (nick == null)
                                    SendMessage(new string[] {
                                        "I am sorry, Necrow is not available right now. I will notice you whenever the Necrow is back up",
                                        "Necrow is not available right now, I'll let you know if the Necrow is back up",
                                        "My apologize, Necrow is not available. I will note you whenever the Necrow is back up",
                                        "Necrow is not available. I will notice you"
                                    }, chatID, messageID, fromID);
                                else
                                    SendMessage(new string[] {
                                        nick + ", I am sorry, Necrow is not available right now. I will notice you whenever the Necrow is back up",
                                        "Necrow is not available right now, I'll let you know if the Necrow is back up",
                                        "My apologize " + nick + ", Necrow is not available. I will note you whenever the Necrow is back up",
                                        "Necrow is not available. I will notice you"
                                    }, chatID, messageID, fromID);
                            }
                            else
                            {
                                if (nick == null)
                                    SendMessage(new string[] {
                                        "I am sorry, Necrow is not available right now",
                                        "Necrow is currently not available",
                                        "My apologize, Necrow is not available",
                                        "I am sorry, Necrow's not available right now"
                                    }, chatID, messageID, fromID);
                                else
                                    SendMessage(new string[] {
                                        "I am sorry " + nick + ", Necrow is not available right now",
                                        "Necrow is currently not available",
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
                    else if (messageIntent == "called")
                    {
                        #region called
                        if (nick == null)
                            SendMessage(new string[] { "Hey", "Hi, what's up?", "Hello", "Hello there", "Greetings", "Hello hello", "Yes?", "You called?", "Yeah, what's up?"
                            }, chatID, messageID, fromID);
                        else
                            SendMessage(new string[] { "Greetings " + nick, "Hi " + nick + ", what's up?", "Hello " + nick, "Hi " + nick, "Hi there " + nick
                            }, chatID, messageID, fromID);

                        if (messageTalk == null) SetTalk("introduced", null, 0, fromID);
                        #endregion
                    }
                    else if (messageIntent == "called>salam")
                    {
                        #region called>salam
                        SendMessage(new string[] {
                            "Wassalamualaikum Warahmatullahi Wabarakatuh",
                            "Wassalamualaikum",
                            "Wassalamualaikum Wr Wb",
                        }, chatID, messageID, fromID);
                        if (messageTalk == null) SetTalk("introduced", null, 0, fromID);
                        #endregion
                    }
                    else if (messageIntent == "called>pagi3")
                    {
                        #region called>pagi3
                        int watdo = RandomHelper.Next(2);

                        if (watdo == 0)
                            SendMessage("PAGI PAGI PAGI!", chatID, messageID, fromID);
                        else if (watdo == 1)
                        {
                            SendMessage("PAGI", chatID, messageID, fromID);
                            SendMessage("PAGI", chatID, messageID, fromID, 500, null);
                            SendMessage("PAGI!", chatID, messageID, fromID, 1000, null);
                        }
                        #endregion
                    }
                    else if (messageIntent == "called>pagi")
                    {
                        #region called>pagi
                        if (nick == null)
                            SendMessage(new string[] {
                                "Good morning", "Morning"
                            }, chatID, messageID, fromID);
                        else
                            SendMessage(new string[] {
                                "Good morning " + nick, "Morning " + nick
                            }, chatID, messageID, fromID);
                        if (messageTalk == null) SetTalk("introduced", null, 0, fromID);
                        #endregion
                    }
                    else if (messageIntent == "called>siang")
                    {
                        #region called>siang
                        if (nick == null)
                            SendMessage(new string[] {
                                "Good afternoon",
                                "Good afternoon, it is a good day, isn't it?",
                                "Good afternoon"
                            }, chatID, messageID, fromID);
                        else
                            SendMessage(new string[] {
                                "Good afternoon " + nick,
                                "Good day " + nick
                            }, chatID, messageID, fromID);
                        if (messageTalk == null) SetTalk("introduced", null, 0, fromID);
                        #endregion
                    }
                    else if (messageIntent == "called>sore")
                    {
                        #region called>sore
                        if (nick == null)
                            SendMessage("Selamat sore", chatID, messageID, fromID);
                        else
                            SendMessage("Selamat sore " + nick, chatID, messageID, fromID);
                        if (messageTalk == null) SetTalk("introduced", null, 0, fromID);
                        #endregion
                    }
                    else if (messageIntent == "called>malam")
                    {
                        #region called>malam
                        if (nick == null)
                            SendMessage("Good evening", chatID, messageID, fromID);
                        else
                            SendMessage(new string[] {
                                "Good evening " + nick,
                                "Good evening " + nick + ", you're not sleeping yet?",
                                "Good evening " + nick + ", how's your day?",
                            }, chatID, messageID, fromID);
                        if (messageTalk == null) SetTalk("introduced", null, 0, fromID);
                        #endregion
                    }
                    else if (messageIntent == "called>night")
                    {
                        #region called>malam
                        if (nick == null)
                            SendMessage("Good night", chatID, messageID, fromID);
                        else
                            SendMessage(new string[] {
                                "Good night for you too " + nick + ", have a nice sleep",
                                "Good night " + nick + ", sweet dreams",
                                "Good night, I will be here if you need me"
                            }, chatID, messageID, fromID);
                        if (messageTalk == null) SetTalk("smalltalk", null, 0, fromID);
                        #endregion
                    }
                    else if (messageIntent == "called>correcttime")
                    {
                        #region called>correct
                        if (DateTime.Now.TimeOfDay.Hours.Between(5, 11)) SendMessage(new string[] {
                            "Well, I thought It's morning right now",
                            "Well here It's already morning, but I don't know yours",
                            "It's already morning here",
                            "Well for me, it should be good morning then",
                            "Good morning from my server"
                        }, chatID, messageID, fromID);
                        else if (DateTime.Now.TimeOfDay.Hours.Between(12, 13)) SendMessage(new string[] {
                            "It's high noon right here",
                            "Good afternoon from my server!",
                            "Good afternoon, it looks like we got different time zone",
                        }, chatID, messageID, fromID);
                        else if (DateTime.Now.TimeOfDay.Hours == 14) SendMessage(new string[] {
                            "I thought It is already afternoon right now, at least here",
                            "Oh well, it is already afternoon here",
                            "Good afternoon, it looks like we got different time zone",
                        }, chatID, messageID, fromID);
                        else if (DateTime.Now.TimeOfDay.Hours.Between(15, 18)) SendMessage(new string[] {
                            "From my server, it's good afternoon! atau selamat sore!",
                            "Good afternoon from my server!",
                            "Oh well, it is already afternoon here",
                            "Good afternoon, it looks like we got different time zone",
                        }, chatID, messageID, fromID);
                        else if (DateTime.Now.TimeOfDay.Hours.Between(19, 21)) SendMessage(new string[] {
                            "Good afternoon, it looks like we got different time zone",
                            "Good afternoon from my server, or maybe you can call it selamat malam"
                        }, chatID, messageID, fromID);
                        else if (DateTime.Now.TimeOfDay.Hours.Between(22, 23)) SendMessage(new string[] {
                            "Good evening from my server",
                            "It is evening here in my server",
                            "It is already night here, so for me it is good evening",
                        }, chatID, messageID, fromID);
                        else if (DateTime.Now.TimeOfDay.Hours.Between(0, 2)) SendMessage(new string[] {
                            "It's night here, at least in my server",
                            "It looks like we got different timezone, here it's already midnight",
                            "Good midnight from my server!"
                        }, chatID, messageID, fromID);
                        else if (DateTime.Now.TimeOfDay.Hours.Between(3, 4)) SendMessage(new string[] {
                            "Well it's early in the morning here in my server",
                            "Good morning from my server",
                            "Well good morning too from my server"
                        }, chatID, messageID, fromID);

                        if (messageTalk == null) SetTalk("compliments", null, 0, fromID);
                        #endregion
                    }
                    else if (messageLower.EndsWith("ysh") && messageMention == "bot")
                    {
                        #region ysh
                        SendMessage(new string[] {
                            "I am not honorable to bear that title. It should be Mas Desy",
                            "I am sorry I am not honorable bear that title",
                            "You mean Mas Desy ysh?"
                        }, chatID, messageID, fromID);

                        #endregion
                    }
                    else if (messageTalk == "necrowup>noticed?")
                    {
                        #region necrowup>noticed?
                        if (messageIntent == "yes")
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
                        else if (messageIntent == "no")
                        {
                            if (Server.IsNecrowConnected()) SendMessage("Necrow is already back up", chatID, messageID, fromID);
                            else SendMessage("Ok. Please try again later", chatID, messageID, fromID);
                            SetTalk("smalltalk", null, 0, fromID);
                        }
                        #endregion
                    }
                    else if (messageTalk == "necrow>probe>what?")
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
                    else if (messageTalk == "necrow>probe>confirm?")
                    {
                        #region necrow>probe>confirm?

                        if (messageIntent == "yes")
                        {
                            MessageProbeExecute((string)GetTalk(fromID).Item2, chatID, GetTalk(fromID).Item3, fromID);
                        }
                        else if (messageIntent == "no")
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
                    else if (messageTalk == "necrow>probe>retry?")
                    {
                        #region necrow>probe>confirm?

                        if (messageIntent == "yes")
                        {
                            MessageProbeExecute((string)GetTalk(fromID).Item2, chatID, GetTalk(fromID).Item3, fromID);
                        }
                        else if (messageIntent == "no")
                        {
                            SendMessage(new string[] {
                                "Okay", "Got it", "I understand", "Understood", "You got it", "No problem", "That's all right"
                            }, chatID, messageID, fromID);
                            SetTalk("smalltalk", null, 0, fromID);
                        }

                        #endregion
                    }
                    else if (messageTalk == "necrow>probe>forceinactive?")
                    {
                        #region necrow>probe>forceinactive?

                        if (messageIntent == "yes")
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
                        else if (messageIntent == "no")
                        {
                            SendMessage(new string[] { "Understood", "I understand", "You bet", "Love it", "No problem" }, chatID, messageID, fromID);
                            SetTalk("smalltalk", null, 0, fromID);
                        }

                        #endregion
                    }
                    else if (messageTalk == "necrow>probe>continueprobe?")
                    {
                        #region necrow>probe>continueprobe?

                        if (messageIntent == "yes")
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
                        else if (messageIntent == "no")
                        {
                            SendMessage(new string[] { "All right I'll stop the probe", "All right I'll stop", "Got it I'll stop", "I will stop the probe", "No problem, I'll stop the probe", "Understood, I'll stop the probe" }, chatID, messageID, fromID);
                            SetTalk("smalltalk", null, 0, fromID);
                        }

                        #endregion
                    }
                    else if (messageTalk == "necrow>probe>choice?")
                    {
                        #region necrow>probe>choice?

                        string choiceString = (string)GetTalk(fromID).Item2;
                        string[] choices = choiceString.Split(StringSplitTypes.Comma, StringSplitOptions.RemoveEmptyEntries);

                        if (choices.Length == 1)
                        {
                            if (messageIntent == "yes")
                            {
                                MessageProbeExecute(choices[0], chatID, messageID, fromID);
                            }
                            else if (messageIntent == "no")
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
                    else if (messageTalk == "smalltalk")
                    {
                        #region smalltalk

                        if (messageIntent == "thanks")
                        {
                            SendMessage(new string[] { "You're welcome", "Welcome", "No problem", "Hope that's help", "Any time" }, chatID, messageID, fromID);
                            ClearTalk(fromID);
                        }

                        #endregion
                    }
                    else if (messageTalk == "smalltalk>null")
                    {
                        #region smalltalk>null

                        if (messageIntent == "thanks")
                        {
                            SendMessage(new string[] { "I don't think you should thank me", "Well", "Ok" }, chatID, messageID, fromID);
                            ClearTalk(fromID);
                        }

                        #endregion
                    }
                    else if (messageTalk == "smalltalk>update")
                    {
                        #region smalltalk>update

                        if (messageIntent == "thanks")
                        {
                            SendMessage(new string[] { "No problem, I'll let you know", "Ok", "Got it", "I'll let you know", "Yep" }, chatID, messageID, fromID);
                            ClearTalk(fromID);
                        }

                        #endregion
                    }
                    else if (messageTalk == "compliments")
                    {
                        #region compliments

                        if (messageIntent == "sorry")
                        {
                            SendMessage(new string[] { "No, It's okay", "No worries", "Uh oh,that's okay",
                                "You shouldn't apologize, It should be me", "It is not your problem, it's mine", "It is my fault, you shouldn't apologize" }, chatID, messageID, fromID);
                            SetTalk("smalltalk>null", null, 0, fromID);
                        }
                        else if (messageIntent == "thanks")
                        {
                            SendMessage(new string[] { "It's good", "No problem", "You are welcome" }, chatID, messageID, fromID);
                            ClearTalk(fromID);
                        }

                        #endregion
                    }
                    else if (messageTalk != null)
                    {
                        ClearTalk(fromID);
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

                if (!identities.ContainsKey(fromID))
                {
                    #region /start or first time message
                    identities.Add(fromID, new TelegramUserIdentity(name));

                    Insert insert = share.Insert("TelegramUser");
                    insert.Value("TU_ID", fromID);
                    insert.Value("TU_Name", name);
                    insert.Execute();

                    string greetingTime = DateTime.Now.TimeOfDay.Hours.Between(5, 11) ? "morning" : DateTime.Now.TimeOfDay.Hours.Between(12, 18) ? "afternoon" : DateTime.Now.TimeOfDay.Hours.Between(19, 23) ? "evening" : "night";

                    SendMessage(new string[] {
                            "Hi " + name + ", thanks for message me",
                            "Heya " + name + " thank you for your PM",
                            "Hello " + name + " thank you for message me",
                            "Good " + greetingTime + " " + name + ", thank you for your PM",
                            greetingTime + " " + name + ", thank you for your PM",
                            "Greetings " + name + ", thanks for contacting me",
                            "Greetings " + name + ", thank you for message me"
                        }, fromID);

                    SendMessage(new string[] {
                            "Before I can help you, how should I call you?",
                            "Before we start, how should I call you?",
                            "Before we continue, how should I call you by name?",
                            "Before we start, what's the name that I can call you?"
                        }, chatID, messageID, fromID, 500, delegate (Message msg) { SetTalk("chat>setnickname", null, 0, fromID); });
                    #endregion
                }
                else
                {
                    if (identities[fromID].Name != name)
                    {
                        Aphysoft.Share.Update update = share.Update("TelegramUser");
                        update.Set("TU_Name", name);
                        update.Where("TU_ID", fromID);
                        update.Execute();

                        if (GetTalk(fromID).Item1 == null)
                        {
                            SetTalk("chat>changename", identities[fromID].Name, 0, fromID);
                            messageTalk = "chat>changename";
                        }
                                
                        identities[fromID].Name = name;
                    }

                    
                    if (messageTalk == "chat>setnickname")
                    {
                        #region chat>setnickname

                        if (messageLower == "/start")
                        {
                            SendMessage(new string[] {
                                "Continue from our previous conversation, how should I call you?",
                                "Continue from our previous conversation, how should I call you by name?",
                                "Continue from our previous conversation, what's the name that I can call you?"
                            }, chatID, messageID, fromID);
                        }
                        else
                        {
                            if (messageIntent == "pass")
                            {
                                #region pass
                                SendMessage(new string[] {
                                    "That's okay, you can tell me what I can call you later on",
                                    "Okay, You can tell me what I can you later on",
                                }, chatID, messageID, fromID);

                                SetTalk("smalltalk", null, 0, fromID);

                                #endregion
                            }
                            if (!messageLower.OnlyContainsCharacters("abcdefghijklmnopqrstuvwxyz"))
                            {
                                if (messageLower.Contains(" "))
                                {
                                    SendMessage(new string[] {
                                    "Are you sure? Good nick contains no space so It makes easier for someone to call you. Give me shorter one or just tell me the first word of it",
                                    "Are you sure? It contains space. It should contains no space so I can call you much easier"
                                }, chatID, messageID, fromID);
                                }
                                else
                                {
                                    SendMessage(new string[] {
                                    "Nick contains only words. Could you please tell me again what's your nick?",
                                    "You sure? There are characters beside word character in your nick name. Could you please tell me again how I should call you?",
                                    "Are you sure? It looked like an internet handle. Please give your nick name so I or someone elase can call you easier",
                                }, chatID, messageID, fromID);
                                }
                            }
                            else if (messageLower.Length > 10)
                            {
                                SendMessage(new string[] {
                                "Hmm, that is quite long for a nick name. Could you give me shorter one?",
                                "What a long nick name. Could you give me shorter one? So I can easily call you",
                                "That's a long nick name. Give me shorter one please?",
                                "Could you please give me shorter nick name?"
                            }, chatID, messageID, fromID);
                            }
                            else
                            {
                                string confirmNick = messageLower[0].ToString().ToUpper() + messageLower.Substring(1);

                                SendMessage(new string[] {
                                "So I can call you " + confirmNick + "?",
                                "That's good name, " + confirmNick + ", is that correct?",
                                confirmNick + ", so I can call you like that?",
                                "That's lovely, so I can call you " + confirmNick + "?"
                            }, chatID, messageID, fromID);

                                SetTalk("chat>confirmnickname", confirmNick, 0, fromID);
                            }
                        }
                        #endregion
                    }
                    else if (messageTalk == "chat>confirmnickname")
                    {
                        #region chat>confirmnickname

                        if (messageIntent == "yes")
                        {
                            #region yes

                            identities[fromID].Nick = (string)GetTalk(fromID).Item2;
                            nick = identities[fromID].Nick;

                            Aphysoft.Share.Update update = share.Update("TelegramUser");
                            update.Set("TU_Nick", nick);
                            update.Where("TU_ID", fromID);
                            update.Execute();

                            SendMessage(new string[] {
                                "All right, " + nick + ", from now on I can call you by name during our conversation",
                                "Good to know you more " + nick + ", from now I call you by that name",
                                "It's nice to know you more " + nick + ", from now I call you by that name",
                            }, chatID, messageID, fromID);

                            SetTalk("smalltalk", null, 0, fromID);

                            #endregion
                        }
                        else if (messageIntent == "no")
                        {
                            SendMessage(new string[] {
                                "Okay, so how should I call you then?",
                                "That's okay, could you please tell again what name can I call you?",
                                }, chatID, messageID, fromID);
                            SetTalk("chat>setnickname", null, 0, fromID);
                        }

                        #endregion
                    }
                    else if (messageTalk == "chat>changename")
                    {
                        #region chat>changename

                        SendMessage(new string[] { "Oh so you changed your name", "Changed your name?", "Wow you changed your name" }, chatID, messageID, fromID);
                        SetTalk("smalltalk", null, 0, fromID);

                        #endregion
                    }
                    else if (messageTalk == "smalltalk")
                    {
                        #region smalltalk

                        if (nick != null)
                        {
                            if (messageIntent == "thanks")
                            {
                                SendMessage(new string[] { "You're welcome", "Welcome", "No problem", "Hope that's help", "Any time" }, chatID, messageID, fromID);
                            }
                        }
                        else
                        {
                            if (messageIntent == "thanks")
                            {
                                SendMessage(new string[] { "Welcome, " + nick, "No problem", "Any time" }, chatID, messageID, fromID);
                            }
                            else if (messageIntent == "yes")
                            {
                                SendMessage(new string[] { "I see", "I understand", "Oh I see", "Well that's good", "That's good", "Oh okay" }, chatID, messageID, fromID);
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        if (messageLower == "/start" || messageIntent == "called")
                        {
                            #region /start

                            SendMessage(new string[] {
                                "Hi " + nick + ", anything that I can help?",
                                "Greetings " + nick + ", how can I help?",
                                "What's up, " + nick + "?",
                                "Greetings!"
                            }, chatID, messageID, fromID);

                            #endregion
                        }
                        else if (messageIntent == "called>salam")
                        {
                            SendMessage("Wassalamualaikum Warahmatullohi Wabarakatuh, " + nick, chatID, messageID, fromID);
                        }
                    }
                }

                #endregion
                
            }
        }

        */
        #endregion
    }
}
