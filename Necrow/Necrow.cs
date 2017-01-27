using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Globalization;
using System.IO;

using Aphysoft.Share;
using Telegram.Bot;

namespace Center
{
    public class ProbeProperties
    {
        #region Fields

        private TimeSpan timeStart;

        public TimeSpan TimeStart
        {
            get { return timeStart; }
            set { timeStart = value; }
        }

        private TimeSpan timeEnd;

        public TimeSpan TimeEnd
        {
            get { return timeEnd; }
            set { timeEnd = value; }
        }

        private string sshUser;

        public string SSHUser
        {
            get { return sshUser; }
            set { sshUser = value; }
        }

        private string sshPassword;

        public string SSHPassword
        {
            get { return sshPassword; }
            set { sshPassword = value; }
        }

        private string tacacUser;

        public string TacacUser
        {
            get { return tacacUser; }
            set { tacacUser = value; }
        }

        private string tacacPassword;

        public string TacacPassword
        {
            get { return tacacPassword; }
            set { tacacPassword = value; }
        }

        private string sshTerminal;

        public string SSHTerminal
        {
            get { return sshTerminal; }
            set { sshTerminal = value; }
        }

        private string sshServerAddress;

        public string SSHServerAddress
        {
            get { return sshServerAddress; }
            set { sshServerAddress = value; }
        }

        #endregion
    }

    public class TelegramInformation
    {
        private long chatID;

        public long ChatID
        {
            get { return chatID; }
        }

        private int messageID;

        public int MessageID
        {
            get { return messageID; }
        }

        public TelegramInformation(long chatID, int messageID)
        {
            this.chatID = chatID;
            this.messageID = messageID;
        }
    }

    public static class Necrow
    {
        #region Fields

        internal readonly static int Version = 16;

        private static Database j = null;

#if DEBUG
        private static bool console = true;
#else
        private static bool console = false;
#endif
        private static Queue<Tuple<int, string>> list = null;

        private static Queue<Tuple<string, TelegramInformation>> prioritize = new Queue<Tuple<string, TelegramInformation>>();

        private static List<Tuple<string, string, string>> supportedVersions = null;

        private static bool mainLoop = true;

        internal static TelegramBotClient telegram = null;

        private static Dictionary<string, Probe> instances = null;

        #endregion

        #region Helpers

        private static Dictionary<string, string[]> interfaceTestPrefixes = null;

        internal static Dictionary<string, string[]> InterfaceTestPrefixes
        {
            get { return interfaceTestPrefixes; }
        }

        #endregion

        #region Methods

        internal static void Event(string message, string subsystem)
        {
            if (console)
            {
                //yyyy/MM/dd 
                if (subsystem == null)
                    System.Console.WriteLine(DateTime.UtcNow.ToString("HH:mm:ss.fff") + "|" + message);
                else
                    System.Console.WriteLine(DateTime.UtcNow.ToString("HH:mm:ss.fff") + "|" + subsystem + "|" + message);
            }
        }

        internal static void Event(string message)
        {
            Event(message, null);
        }

#if DEBUG
        public static bool Debug()
        {
            return true;
        }

        public static void Test(string name)
        {
            prioritize.Enqueue(new Tuple<string, TelegramInformation>(name.ToUpper() + "*", null));
        }
#else
        internal static void Log(string source, string message, string stacktrace)
        {
            Insert insert = j.Insert("ProbeLog");
            insert.Value("XL_TimeStamp", DateTime.UtcNow);
            insert.Value("XL_Source", source);
            insert.Value("XL_Message", message);
            insert.Value("XL_StackTrace", stacktrace);
            insert.Execute();
        }

        internal static void Log(string source, string message)
        {
            Log(source, message, null);
        }
#endif

        public static bool InTime(ProbeProperties properties)
        {
            TimeSpan start = properties.TimeStart;
            TimeSpan end = properties.TimeEnd;
            TimeSpan time = DateTime.UtcNow.TimeOfDay;

            return (start < end && start < time && time < end) ||
                   (start > end && (time > start || time < end)) ||
                   (start == end);
        }

        private static int telegram_lastMessageId = 0;
        private static string telegram_waitingYesForThisNode = null;
        private static int telegram_waitingYesForThisNode_msgID = 0;

        private static void Telegram_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            string msg = e.Message.Text;
            string rsp = null;

            if (msg != null)
            {
                telegram_lastMessageId = e.Message.MessageId;
                int currentMessageId = e.Message.MessageId;

                if (msg.StartsWith("/probestatus"))
                {
                    #region /probestatus

                    int intime = 0;
                    foreach (KeyValuePair<string, Probe> ins in instances)
                    {
                        if (InTime(ins.Value.Properties)) intime++;
                    }

                    StringBuilder probeloc = new StringBuilder();
                    int actv = 0;
                    foreach (KeyValuePair<string, Probe> ins in instances)
                    {
                        if (ins.Value.NodeConnected)
                        {
                            //if (probeloc.Length > 0) probeloc.Append(", ");

                            probeloc.Append("\nPROBE" + ins.Key.Trim() + " " + ins.Value.NodeName + " is running for " + Math.Round((DateTime.UtcNow - ins.Value.NodeProbeStartTime).TotalSeconds) + "s using " + 
                                ins.Value.Properties.TacacUser);

                            actv++;
                        }
                    }

                    if (intime > 0)
                    {
                        if (actv > 0)
                        {
                            rsp = "There " + (actv > 1 ? "are" : "is") + " " + actv + " active probe" + (actv > 1 ? "s" : "") + " (" + intime + " on duty): " + probeloc.ToString();
                        }
                        else rsp = "There is no currently active probe (" + intime + " on duty)";
                    }
                    else rsp = "There is no probe currently on duty";

                    #endregion
                }
                else if (msg.StartsWith("/probe"))
                {
                    #region /probe
                    string[] tokens = msg.Split(StringSplitTypes.Space, StringSplitOptions.RemoveEmptyEntries);

                    if (tokens.Length != 2)
                    {
                        rsp = "The usage of that command is /probe <node name>";
                    }
                    else
                    {
                        string node = tokens[1].ToUpper();

                        Result rnode = Jovice.Database.Query("select * from Node where NO_Name = {0}", node);

                        if (rnode.Count == 1)
                        {
                            bool already = false;
                            foreach (Tuple<string, TelegramInformation> tup in prioritize)
                            {
                                if (tup.Item1.Contains(node) || tup.Item1.Contains(node + "*"))
                                {
                                    already = true;
                                    break;
                                }
                            }

                            if (already) rsp = "The node is currently on my list and will starting as soon as a probe on duty is available";
                            else
                            {
                                int intime = 0;
                                foreach (KeyValuePair<string, Probe> ins in instances)
                                {
                                    if (InTime(ins.Value.Properties)) intime++;
                                }

                                if (intime == 0)
                                {
                                    rsp = "There is no probe currently on duty, do you want to probe the node as soon as a probe is available?";
                                    telegram_waitingYesForThisNode = node;
                                    telegram_waitingYesForThisNode_msgID = e.Message.MessageId;
                                }
                                else
                                {
                                    prioritize.Enqueue(new Tuple<string, TelegramInformation>(node + "*", new TelegramInformation(e.Message.Chat.Id, e.Message.MessageId)));
                                    rsp = "Got It, I'll keep you updated";
                                }
                            }
                        }
                        else rsp = "I am sorry, I couldn't find " + node + " in my database";
                    }
                    #endregion
                }
                else if (telegram_waitingYesForThisNode != null && msg.ToLower().IndexOf("yes", "ya", "yaa", "yaaa", "yess", "yesss", "hell yes", "yeah", "iya", "lakukan", "baiklah") > -1)
                {
                    #region /probe + yes
                    prioritize.Enqueue(new Tuple<string, TelegramInformation>(telegram_waitingYesForThisNode + "*", new TelegramInformation(e.Message.Chat.Id, telegram_waitingYesForThisNode_msgID)));
                    rsp = "Got It, I'll keep you updated";
                    currentMessageId = telegram_waitingYesForThisNode_msgID;
                    telegram_waitingYesForThisNode = null;
                    telegram_waitingYesForThisNode_msgID = 0;
                    #endregion
                }
                else if (telegram_waitingYesForThisNode != null && msg.ToLower().IndexOf("no", "noo", "hell no", "nooo", "tidak", "gak", "enggak", "gak usah", "nanti aja", "ga", "ga usah", "pass", "later") > -1)
                {
                    #region /probe + no
                    rsp = "Okay";
                    telegram_waitingYesForThisNode = null;
                    telegram_waitingYesForThisNode_msgID = 0;
                    #endregion
                }

                if (rsp != null)
                {
                    if (telegram_lastMessageId != currentMessageId)
                        telegram.SendTextMessageAsync(e.Message.Chat.Id, rsp, false, false, currentMessageId, null);
                    else
                        telegram.SendTextMessageAsync(e.Message.Chat.Id, rsp, false, false, 0, null);
                }
            }
        }
        
        private static void Telegram_OnReceiveError(object sender, Telegram.Bot.Args.ReceiveErrorEventArgs e)
        {
            Event("Telegram Bot Error: " + e.ApiRequestException.Message);
        }

        private static void Telegram_OnReceiveGeneralError(object sender, Telegram.Bot.Args.ReceiveGeneralErrorEventArgs e)
        {
            Event("Telegram Bot General Error: " + e.Exception.Message);
        }

        internal static void Telegram_SendMessage(TelegramInformation info, string msg)
        {
            if (telegram_lastMessageId != info.MessageID)
                telegram_lastMessageId = telegram.SendTextMessageAsync(info.ChatID, msg, false, false, info.MessageID, null).Id;
            else
                telegram_lastMessageId = telegram.SendTextMessageAsync(info.ChatID, msg, false, false, 0, null).Id;
        }

        public static void Start()
        {
            Thread start = new Thread(new ThreadStart(delegate ()
            {
                Batch batch;
                Result result;

                Culture.Default();
                Event("Necrow Starting...");

                //Service.Client();
                //Service.Connected += delegate (Connection connection)
                //{
                //    Event("Service Connected");
                //    Service.Send(new ServerNecrowServiceMessage(NecrowServiceMessageType.Hello));
                //};
                //Service.Register(typeof(ServerNecrowServiceMessage), NecrowServiceMessageHandler);

                Event("Checking Jovice Database connection... ");

                bool joviceDatabaseConnected = false;
                j = Jovice.Database;

                DatabaseExceptionEventHandler checkingDatabaseException = delegate (object sender, DatabaseExceptionEventArgs eventArgs)
                {
                    Event("Connection Failed: " + eventArgs.Message);
                };

                j.Exception += checkingDatabaseException;

                if (j.Test())
                {
                    joviceDatabaseConnected = true;
                    Event("Jovice Database OK");
                }

                j.Exception -= checkingDatabaseException;

                j.Exception += delegate (object sender, DatabaseExceptionEventArgs eventArgs)
                {
                    Event("Database exception has been caught: " + eventArgs.Message.Trim(new char[] { '\r', '\n', ' ' }));
                    //throw new Exception(eventArgs.Message.Trim(new char[] { '\r', '\n', ' ' }) + "\n" + eventArgs.Sql);
                };
                j.Retry += delegate (object sender, DatabaseExceptionEventArgs eventArgs)
                {
                    if (eventArgs.Exception == DatabaseException.Timeout)
                    {
                        Event("Database query has timed out, retry in 10 seconds");
                        Thread.Sleep(10000);
                    }
                    else
                    {
                        eventArgs.NoRetry = true;
                    }
                };
                j.QueryAttempts = 5;

                if (joviceDatabaseConnected)
                {
                    batch = j.Batch();

                    #region Graph

                    //JoviceGraph.Update();

                    #endregion

                    #region Database Check

                    Event("Checking database...");

                    DatabaseCheck();

                    Event("Database checks completed");

                    #endregion

                    #region Virtualizations

                    Event("Starting database virtualizations...");

                    NecrowVirtualization.Load();

                    Event("Database virtualizations completed");

                    #endregion

                    #region Bot
                    
                    Event("Starting Telegram Bot handling...");

                    //329048230:AAFDHCcNNyDpAyfMe5vn-oLzvKjmIrIG4Hg dev
                    //298092052:AAFrX-tcSnPR_8y9xwukMwhaZC2A5wpFHYI prod
#if DEBUG
                    telegram = new TelegramBotClient("329048230:AAFDHCcNNyDpAyfMe5vn-oLzvKjmIrIG4Hg");
#else
                    telegram = new TelegramBotClient("298092052:AAFrX-tcSnPR_8y9xwukMwhaZC2A5wpFHYI");
#endif

                    telegram.OnMessage += Telegram_OnMessage;
                    telegram.OnReceiveError += Telegram_OnReceiveError;
                    telegram.OnReceiveGeneralError += Telegram_OnReceiveGeneralError;

                    telegram.StartReceiving();

                    Event("Telegram Bot started");

                    #endregion
                    
                    #region Etc

                    interfaceTestPrefixes = new Dictionary<string, string[]>();
                    interfaceTestPrefixes.Add("Hu", new string[] { "H", "HU", "GI", "GE" });
                    interfaceTestPrefixes.Add("Te", new string[] { "T", "TE", "TENGIGE", "GI", "GE", "XE" }); // kadang Te-gig direfer sebagai Gi dammit people
                    interfaceTestPrefixes.Add("Gi", new string[] { "G", "GI", "GE", "GIGAE", "GIGABITETHERNET", "TE" }); // kadang Te-gig direfer sebagai Gi dammit people
                    interfaceTestPrefixes.Add("Fa", new string[] { "F", "FA", "FE", "FASTE" });
                    interfaceTestPrefixes.Add("Et", new string[] { "E", "ET", "ETH" });
                    interfaceTestPrefixes.Add("Ag", new string[] { "LAG", "ETH-TRUNK", "BE" });

                    #endregion

                    #region Probe initialization

                    Event("Loading probe list...");

                    list = new Queue<Tuple<int, string>>();

                    foreach (Row xp in j.Query("select XP_ID, XP_NO from ProbeProgress order by XP_ID asc"))
                    {
                        list.Enqueue(new Tuple<int, string>(xp["XP_ID"].ToInt(), xp["XP_NO"].ToString()));
                    }

                    if (list.Count == 0)
                    {
                        CreateNodeQueue();
                    }
                    else
                    {
                        // set all starttime and status to null
                        j.Execute("update ProbeProgress set XP_StartTime = NULL, XP_Status = NULL");
                        Event("Using existing list, " + list.Count + " node" + (list.Count > 1 ? "s" : "") + " remaining");
                    }

                    if (supportedVersions == null)
                    {
                        Result sver = j.Query("select * from NodeSupport");

                        supportedVersions = new List<Tuple<string, string, string>>();

                        foreach (Row sve in sver)
                        {
                            supportedVersions.Add(new Tuple<string, string, string>(sve["NT_Manufacture"].ToString(), sve["NT_Version"].ToString(), sve["NT_SubVersion"].ToString()));
                        }
                    }

                    #endregion

                    instances = new Dictionary<string, Probe>();

                    long loops = 0;

                    while (mainLoop)
                    {
                        if (loops % 10 == 0)
                        {
                            result = j.Query(@"
select XA_ID, XA_TimeStart, XA_TimeEnd, XU_ServerUser, XU_ServerPassword, XU_TacacUser, XU_TacacPassword, XS_Address, XS_ConsolePrefixFormat
from ProbeAccess, ProbeUser, ProbeServer where XA_XU = XU_ID and XU_XS = XS_ID");

                            foreach (Row row in result)
                            {
                                string id = row["XA_ID"].ToString();

                                if (!instances.ContainsKey(id))
                                {
                                    // NEW
                                    ProbeProperties prop = new ProbeProperties();
                                    prop.SSHUser = row["XU_ServerUser"].ToString();
                                    prop.SSHPassword = row["XU_ServerPassword"].ToString();
                                    prop.TacacUser = row["XU_TacacUser"].ToString();
                                    prop.TacacPassword = row["XU_TacacPassword"].ToString();
                                    prop.SSHServerAddress = row["XS_Address"].ToString();
                                    prop.SSHTerminal = string.Format(row["XS_ConsolePrefixFormat"].ToString(), prop.SSHUser);
                                    prop.TimeStart = row["XA_TimeStart"].ToTimeSpan(TimeSpan.MinValue);
                                    prop.TimeEnd = row["XA_TimeEnd"].ToTimeSpan(TimeSpan.MaxValue);

                                    instances.Add(id, Probe.Create(prop, "PROBE" + id.Trim()));

                                    Event("ADD PROBE" + id.Trim() + ": " + prop.SSHUser + "@" + prop.SSHServerAddress + " [" + prop.TacacUser + "] " + ((prop.TimeStart == TimeSpan.Zero || prop.TimeEnd == TimeSpan.Zero) ? "" : (prop.TimeStart + "-" + prop.TimeEnd)));
                                }
                                else
                                {
                                    // UPDATE
                                    ProbeProperties prop = instances[id].Properties;

                                    List<string> updateinfo = new List<string>();

                                    // change timestart
                                    TimeSpan newstart = row["XA_TimeStart"].ToTimeSpan(TimeSpan.MinValue);
                                    TimeSpan newend = row["XA_TimeEnd"].ToTimeSpan(TimeSpan.MaxValue);
                                    bool updatetime = false;

                                    if (newstart != prop.TimeStart)
                                    {
                                        updatetime = true;
                                        prop.TimeStart = newstart;
                                    }
                                    if (newend != prop.TimeEnd)
                                    {
                                        updatetime = true;
                                        prop.TimeEnd = newend;
                                    }
                                    if (updatetime) updateinfo.Add("time changed to " + ((prop.TimeStart == TimeSpan.Zero || prop.TimeEnd == TimeSpan.Zero) ? "" : (prop.TimeStart + "-" + prop.TimeEnd)));

                                    // change tacac
                                    string newtacacuser = row["XU_TacacUser"].ToString();
                                    string newtacacpassword = row["XU_TacacPassword"].ToString();

                                    if (newtacacuser != prop.TacacUser)
                                    {
                                        updateinfo.Add("tacacuser " + prop.TacacUser + " -> " + newtacacuser);
                                        prop.TacacUser = newtacacuser;
                                    }
                                    if (newtacacpassword != prop.TacacPassword)
                                    {
                                        updateinfo.Add("tacacpass changed");
                                        prop.TacacPassword = newtacacpassword;
                                    }

                                    // change ssh
                                    string newsshuser = row["XU_ServerUser"].ToString();
                                    string newsshpassword = row["XU_ServerPassword"].ToString();
                                    string newsshserveraddress = row["XS_Address"].ToString();

                                    bool updatessh = false;

                                    if (newsshuser != prop.SSHUser)
                                    {
                                        updatessh = true;
                                        updateinfo.Add("sshuser " + prop.SSHUser + " -> " + newsshuser);
                                        prop.SSHUser = newsshuser;
                                    }
                                    if (newsshpassword != prop.SSHPassword)
                                    {
                                        updatessh = true;
                                        updateinfo.Add("sshpass changed");
                                        prop.SSHPassword = newsshpassword;
                                    }
                                    if (newsshserveraddress != prop.SSHServerAddress)
                                    {
                                        updatessh = true;
                                        updateinfo.Add("sshaddress " + prop.SSHServerAddress + " -> " + newsshserveraddress);
                                        prop.SSHServerAddress = newsshserveraddress;
                                    }

                                    if (updateinfo.Count > 0)
                                    {
                                        Event("UPDATE PROBE" + id.Trim() + ": " + string.Join(", ", updateinfo.ToArray()));
                                    }

                                    if (updatessh)
                                    {
                                        prop.SSHTerminal = string.Format(row["XS_ConsolePrefixFormat"].ToString(), prop.SSHUser);
                                        instances[id].BeginRestart();
                                    }
                                }
                            }

                            List<string> remove = new List<string>();
                            foreach (KeyValuePair<string, Probe> pair in instances)
                            {
                                bool found = false;
                                foreach (Row row in result)
                                {
                                    string id = row["XA_ID"].ToString();
                                    if (pair.Key == id)
                                    {
                                        found = true;
                                        break;
                                    }
                                }

                                if (!found)
                                {
                                    Event("DELETE PROBE" + pair.Key.Trim());
                                    pair.Value.QueueStop();
                                    remove.Add(pair.Key);
                                }
                            }
                            foreach (string key in remove)
                                instances.Remove(key);
                        }


                        foreach (KeyValuePair<string, Probe> pair in instances)
                        {
                            Probe probe = pair.Value;
                            if (InTime(probe.Properties)) pair.Value.Start();
                        }

                        Thread.Sleep(1000);
                        loops++;
                    }
                }
            }));
            start.Start();
        }

        public static void Stop()
        {

        }

        private static void CreateNodeQueue()
        {
            lock (list)
            {
                if (list.Count == 0)
                {
                    Event("Preparing node list...");

                    Result nres = j.Query(@"
select NO_ID from Node where NO_Active = 1 and NO_Type in ('P', 'M') and NO_TimeStamp is null and NO_LastConfiguration is null                        
");
                    Result mres = j.Query(@"
select a.NO_ID, a.NO_Name, a.NO_Remark, a.NO_TimeStamp, CASE WHEN a.span < 0 then 0 else a.span end as span from (
select NO_ID, NO_Name, NO_Remark, NO_LastConfiguration, NO_TimeStamp, DateDiff(hour, NO_LastConfiguration, NO_TimeStamp) as span 
from Node where NO_Active = 1 and NO_Type in ('P', 'M') and NO_TimeStamp is not null and NO_LastConfiguration is not null
) a
order by span asc, a.NO_LastConfiguration asc
");
                    Result sres = j.Query(@"
select NO_ID from Node where NO_Active = 1 and NO_Type in ('P', 'M') and NO_TimeStamp is not null and NO_LastConfiguration is null                        
");

                    List<string> nids = new List<string>();

                    int excluded = 0;

                    foreach (Row row in nres) nids.Add(row["NO_ID"].ToString());
                    foreach (Row row in mres)
                    {
                        string remark = row["NO_Remark"].ToString();
                        if (remark != null)
                        {
                            DateTime timestamp = row["NO_TimeStamp"].ToDateTime();
                            TimeSpan span = DateTime.Now - timestamp;

                            if (
                                (remark == "CONNECTFAIL" && span.TotalHours <= 3) ||
                                (remark == "UNRESOLVED" && span.TotalDays <= 1)
                            )
                            {
                                excluded++;
                                Event("Excluded: " + row["NO_Name"].ToString() + " Remark: " + remark);
                                continue;
                            }
                        }

                        nids.Add(row["NO_ID"].ToString());
                    }
                    foreach (Row row in sres) nids.Add(row["NO_ID"].ToString());
                    int total = nids.Count + excluded;
                    Event("Total " + total + " nodes available, " + nids.Count + " nodes eligible, " + excluded + " excluded in this list");

                    // check incompleted probeprogress
                    List<int> incid = new List<int>();
                    Result result = j.Query("select XP_ID from ProbeProgress");
                    foreach (Row row in result) incid.Add(row["XP_ID"].ToInt());
                    
                    Batch batch = j.Batch();

                    batch.Begin();
                    int id = 1;
                    foreach (string nid in nids)
                    {
                        Insert insert = j.Insert("ProbeProgress");

                        while (incid.Contains(id)) id++; // if id contained in incompleted id, then increase

                        insert.Value("XP_ID", id++);
                        insert.Value("XP_NO", nid);
                        batch.Execute(insert);
                    }
                    result = batch.Commit();
                    if (result.Count > 0) Event("List created");

                    foreach (Row xp in j.Query("select XP_ID, XP_NO from ProbeProgress order by XP_ID asc"))
                    {
                        list.Enqueue(new Tuple<int, string>(xp["XP_ID"].ToInt(), xp["XP_NO"].ToString()));
                    }
                }
            }
        }

        private static void DatabaseCheck()
        {
            Database jovice = Necrow.j;
            Result result;
            Batch batch = jovice.Batch();

            #region Upper case node name

            result = jovice.Query("select * from Node");

            string[] nodeTypes = new string[] { "P", "M", "S", "H", "D" };
            string[] nodeManufactures = new string[] { "CISCO", "HUAWEI", "ALCATEL-LUCENT", "JUNIPER", "TELLABS" };

            batch.Begin();
            foreach (Row row in result)
            {
                string id = row["NO_ID"].ToString();
                

                Update update = jovice.Update("Node");
                update.Where("NO_ID", id);

                string name = row["NO_Name"].ToString();
                if (name.ToUpper() != name) update.Set("NO_Name", name.ToUpper());

                string type = row["NO_Type"].ToString();
                if (type == "p") update.Set("NO_Type", "P");
                else if (type == "m") update.Set("NO_Type", "M");
                else if (type == "s") update.Set("NO_Type", "S");
                else if (type == "h") update.Set("NO_Type", "H");
                else if (type == "d") update.Set("NO_Type", "D");
                else if (type.InOf(nodeTypes) == -1) update.Set("NO_Active", false);

                string man = row["NO_Manufacture"].ToString();
                if (man == "alu" || man == "ALU") update.Set("NO_Manufacture", "ALCATEL-LUCENT");
                else if (man == "hwe" || man == "HWE") update.Set("NO_Manufacture", "HUAWEI");
                else if (man == "cso" || man == "CSO" || man == "csc" || man == "CSC") update.Set("NO_Manufacture", "CISCO");
                else if (man == "jun" || man == "JUN") update.Set("NO_Manufacture", "JUNIPER");
                else if (man.ToUpper().InOf(nodeManufactures) == -1) update.Set("NO_Active", false);
                else if (man.ToUpper() != man) update.Set("NO_Manufacture", man.ToUpper());

                if (!update.IsEmpty) batch.Execute(update);
            }

            if (batch.Count > 0) Event("Checking Node...");
            result = batch.Commit();

            if (result.AffectedRows > 0)
            {
                Event("Affected " + result.AffectedRows + " rows");
            }

            #endregion
            
            bool neighborAffected = false;

            #region Neighbor already exists in node

            result = jovice.Query("select NO_ID, NN_ID from Node left join NodeNeighbor on NN_Name = NO_Name where NN_ID is not null and NO_Type in ('M', 'P') and NO_Active = 1");

            if (result.Count > 0)
            {
                Event("Removing " + result.Count + " duplicated neighbor nodes...");

                batch.Begin();
                foreach (Row row in result)
                {
                    string nnid = row["NN_ID"].ToString();
                    batch.Execute("update MEInterface set MI_TO_NI = NULL where MI_TO_NI in (select NI_ID from NeighborInterface where NI_NN = {0})", nnid);
                    batch.Execute("update PEInterface set PI_TO_NI = NULL where PI_TO_NI in (select NI_ID from NeighborInterface where NI_NN = {0})", nnid);
                    batch.Execute("delete from NeighborInterface where NI_NN = {0}", nnid);
                    batch.Execute("delete from NodeNeighbor where NN_ID = {0}", nnid);
                }
                result = batch.Commit();
                Event("Affected " + result.AffectedRows + " rows");

                neighborAffected = true;
            }

            #endregion

            #region Removing unused interfaces on Node Neighbors

            result = jovice.Query(@"
select NI_ID from NeighborInterface 
left join MEInterface on MI_TO_NI = NI_ID 
left join PEInterface on PI_TO_NI = NI_ID
left join NodeNeighbor on NN_ID = NI_NN
where NI_Name <> 'UNSPECIFIED' and MI_ID is null and PI_ID is null
");
            if (result.Count > 0)
            {
                Event("Removing " + result.Count + " unused interfaces on Node Neighbors...");

                batch.Begin();
                foreach (Row row in result)
                {
                    string ni = row["NI_ID"].ToString();

                    batch.Execute("delete from NeighborInterface where NI_ID = {0}", ni);
                }
                result = batch.Commit();

                Event("Removed " + result.AffectedRows + " interfaces");

                neighborAffected = true;
            }

            #endregion

            if (NecrowVirtualization.IsReady && neighborAffected)
            {
                Event("Reloading neighbor virtualizations...");

                NecrowVirtualization.NeighborLoad();

                Event("Virtualization reloaded");
            }
        }

        internal static Tuple<int, string> NextNode()
        {
            Tuple<int, string> noded = null;

            lock (list)
            {
                if (list.Count == 0)
                {
                    // here were do things every loop
                    DatabaseCheck();

                    // create new list
                    CreateNodeQueue();
                }

                noded = list.Dequeue();

            }

            return noded;
        }

        internal static Tuple<string, TelegramInformation> NextPrioritize()
        {
            Tuple<string, TelegramInformation> node = null;

            lock (prioritize)
            {
                if (prioritize.Count > 0)
                {
                    node = prioritize.Dequeue();
                }
            }

            return node;
        }

        internal static void AcknowledgeNodeVersion(string manufacture, string version, string subVersion)
        {
            bool exists = false;

            foreach (Tuple<string, string, string> sve in supportedVersions)
            {
                if (sve.Item1 == manufacture && sve.Item2 == version && sve.Item3 == subVersion)
                {
                    exists = true;
                    break;
                }
            }

            if (!exists)
            {
                lock (supportedVersions)
                {
                    supportedVersions.Add(new Tuple<string, string, string>(manufacture, version, subVersion));

                    Insert insert = j.Insert("NodeSupport");
                    insert.Value("NT_ID", Database.ID());
                    insert.Value("NT_Manufacture", manufacture);
                    insert.Value("NT_Version", version);
                    insert.Value("NT_SubVersion", subVersion);
                    insert.Execute();
                }
            }
        }

        public static void Console()
        {
            console = true;

            bool consoleLoop = true;

            while (consoleLoop)
            {
                string line = System.Console.ReadLine();
                ConsoleInput cs = new ConsoleInput(line);

                if (cs.IsCommand("exit"))
                {
                    Stop();
                    consoleLoop = false;
                }
                else if (cs.IsCommand("probe"))
                {
                    if (cs.Clauses.Count == 2)
                    {
                        string nodename = cs.Clauses[1];
                        prioritize.Enqueue(new Tuple<string, TelegramInformation>(nodename.ToUpper(), null));
                    }
                }
            }
        }

        private static void NecrowServiceMessageHandler(MessageEventArgs e)
        {
            ServerNecrowServiceMessage m = (ServerNecrowServiceMessage)e.Message;

            if (m.Type == NecrowServiceMessageType.Request)
            {
                Event("We got request from server! = " + m.RequestID);
            }
        }

        #endregion
    }

}
