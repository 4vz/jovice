using Aphysoft.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Athena
{
    public class MessengerGroup
    {
        #region Fields

        private static Dictionary<string, MessengerGroup> groups = null;

        public string ID { get; private set; }

        public long TelegramID { get; private set; }

        public string Name { get; private set; }

        public string Application { get; private set; } = null;

        #endregion

        #region Constructors

        private MessengerGroup()
        {

        }

        #endregion

        #region Methods

        private static void Load()
        {
            if (groups == null)
            {
                groups = new Dictionary<string, MessengerGroup>();

                Result r = Athena.Instance.Database.Query("select * from MessengerGroup");

                foreach (Row ro in r)
                {
                    string id = ro["MG_ID"].ToString();
                    long tid = ro["MG_TelegramID"].ToLong(0);
                    string name = ro["MG_Name"].ToString();
                    string application = ro["MG_Application"].ToString();

                    groups.Add(id, new MessengerGroup() { ID = id, TelegramID = tid, Name = name, Application = application });
                }
            }
        }

        public static MessengerGroup[] GetGroups()
        {
            Load();

            List<MessengerGroup> gs = new List<MessengerGroup>();

            foreach (KeyValuePair<string, MessengerGroup> pair in groups)
            {
                gs.Add(pair.Value);
            }

            return gs.ToArray();
        }

        public static MessengerGroup[] GetGroupsByApplication(string application)
        {
            Load();

            List<MessengerGroup> gs = new List<MessengerGroup>();

            foreach (KeyValuePair<string, MessengerGroup> pair in groups)
            {
                if (pair.Value.Application == application)
                    gs.Add(pair.Value);
            }

            return gs.ToArray();
        }

        public static MessengerGroup GetByTelegramID(long id)
        {
            Load();

            MessengerGroup found = null;

            lock (groups)
            {
                foreach (KeyValuePair<string, MessengerGroup> pair in groups)
                {
                    if (pair.Value.TelegramID == id)
                    {
                        found = pair.Value;
                        break;
                    }
                }
            }

            return found;
        }

        public static MessengerGroup CreateByTelegramID(string name, long telegramID)
        {
            MessengerGroup group = new MessengerGroup() { ID = Database.ID(), TelegramID = telegramID, Name = name };

            Insert i = Athena.Instance.Database.Insert("MessengerGroup");
            i.Value("MG_ID", group.ID);
            i.Value("MG_Name", group.Name);
            i.Value("MG_TelegramID", group.TelegramID);
            i.Execute();

            lock (groups)
            {
                groups.Add(group.ID, group);
            }

            return group;
        }

        public void UpdateGroupName(string name)
        {
            Name = name;
            Update u = Athena.Instance.Database.Update("MessengerGroup");
            u.Set("MG_Name", name);
            u.Where("MG_ID", ID);
            u.Execute();
        }

        public void SetApplication(string application)
        {
            Application = application;
            Update u = Athena.Instance.Database.Update("MessengerGroup");
            u.Set("MG_Application", application);
            u.Where("MG_ID", ID);
            u.Execute();
        }

        public void Remove()
        {
            Athena.Instance.Database.Execute("delete from MessengerGroup where MG_ID = {0}", ID);

            lock (groups)
            {
                groups.Remove(ID);
            }
        }

        #endregion
    }

    public class MessengerUser
    {
        #region Fields

        private static Dictionary<string, MessengerUser> users = null;

        public string ID { get; private set; }

        public int TelegramID { get; private set; }

        public string Name { get; private set; }

        #endregion

        #region Constructors

        private MessengerUser()
        {

        }

        #endregion

        #region Methods

        private static void Load()
        {
            if (users == null)
            {
                users = new Dictionary<string, MessengerUser>();

                Result r = Athena.Instance.Database.Query("select * from MessengerUser");

                foreach (Row ro in r)
                {
                    string id = ro["MU_ID"].ToString();
                    string name = ro["MU_Name"].ToString();
                    int tid = ro["MU_TelegramID"].ToInt();
                    
                    users.Add(id, new MessengerUser() { ID = id, Name = name, TelegramID = tid });
                }
            }
        }

        public static MessengerUser GetByTelegramID(int id)
        {
            Load();

            MessengerUser found = null;

            lock (users)
            {
                foreach (KeyValuePair<string, MessengerUser> pair in users)
                {
                    if (pair.Value.TelegramID == id)
                    {
                        found = pair.Value;
                        break;
                    }
                }
            }

            return found;
        }

        public static MessengerUser CreateByTelegramID(string name, int telegramID)
        {
            MessengerUser user = new MessengerUser() { ID = Database.ID(), TelegramID = telegramID, Name = name };

            Insert i = Athena.Instance.Database.Insert("MessengerUser");
            i.Value("MU_ID", user.ID);
            i.Value("MU_Name", user.Name);
            i.Value("MU_TelegramID", user.TelegramID);            
            i.Execute();

            lock (users)
            {
                users.Add(user.ID, user);
            }

            return user;
        }

        public void UpdateName(string name)
        {
            Name = name;
            Update u = Athena.Instance.Database.Update("MessengerUser");
            u.Set("MU_Name", name);
            u.Where("MU_ID", ID);
            u.Execute();
        }

        #endregion
    }

    public static class Messenger
    {
        public static void Event(string message)
        {
            Athena.Instance.Event(message);
        }

        public static void Event(string message, string label)
        {
            Athena.Instance.Event(message, label);
        }

        private static ManualResetEvent telegramInitRoutineResetEvent = new ManualResetEvent(false);
        private static bool telegramInitRoutineAction = false;

        private static Telegram.Bot.TelegramBotClient telegram;
        
        public static void Init()
        {
            telegram = new Telegram.Bot.TelegramBotClient("353770204:AAEs0Snc9Zc9crw-8_zQS8QGHyI0A4QBE6E");
            telegram.OnMessage += Telegram_OnMessage;
            telegram.MessageOffset = 0;
            telegram.StartReceiving();
            
            Thread telegramInitRoutineThread = new Thread(new ThreadStart(async delegate ()
            {
                telegramInitRoutineAction = true;
                telegramInitRoutineResetEvent.WaitOne(5000);

                if (telegramInitRoutineAction)
                {
                    Event("Telegram Init Routine executed");
                    foreach (MessengerGroup g in MessengerGroup.GetGroups())
                    {
                        bool testOK = await TelegramTest(g.TelegramID);

                        if (!testOK)
                        {
                            g.Remove();
                        }
                    }
                }

            }));
            telegramInitRoutineThread.Start();
        }
        
        public static async Task<bool> TelegramTest(long chatID)
        {
            bool success = false;

            try
            {
                int cm = await telegram.GetChatMembersCountAsync(chatID);
                success = true;
            }
            catch
            {
            }

            return success;
        }

        private static string Telegram_Name(Telegram.Bot.Types.User user)
        {
            return user.FirstName + "|" + user.LastName + "@" + user.Username;
        }

        private static async void Telegram_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            telegramInitRoutineAction = false;
            telegramInitRoutineResetEvent.Set();

            long chatID = e.Message.Chat.Id;
            int fromID = e.Message.From.Id;
            bool isPrivateMessage = (e.Message.Chat.Id == e.Message.From.Id);

            MessengerGroup group = null;
            MessengerUser user = MessengerUser.GetByTelegramID(fromID);

            if (user == null) user = MessengerUser.CreateByTelegramID(Telegram_Name(e.Message.From), fromID);
            if (user.Name != Telegram_Name(e.Message.From)) user.UpdateName(Telegram_Name(e.Message.From));

            if (!isPrivateMessage)
            {
                group = MessengerGroup.GetByTelegramID(chatID);

                if (group == null) group = MessengerGroup.CreateByTelegramID(e.Message.Chat.Title, chatID);
                if (group.Name != e.Message.Chat.Title) group.UpdateGroupName(e.Message.Chat.Title);

                if (e.Message.LeftChatMember != null)
                {
                    if (e.Message.LeftChatMember.Id == telegram.BotId)
                    {
                        group.Remove();
                    }
                    else
                    {
                        int cm = await telegram.GetChatMembersCountAsync(chatID); 
                        
                        if (cm == 1)
                        {
                            group.Remove();
                        }
                    }
                }
                if (e.Message.NewChatMembers != null)
                {
                    foreach (Telegram.Bot.Types.User newUser in e.Message.NewChatMembers)
                    {
                        if (newUser.Id == telegram.BotId)
                        {
                            
                        }
                    }
                }

                if (group.Application != null)
                {
                    if (e.Message.Text.StartsWith("/") && e.Message.Text.Length > 1)
                    {
                        // command
                        string command = e.Message.Text.Substring(1);

                        Event("command: " + command);
                    }
                }

            }

        }
    }
}
