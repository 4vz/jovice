using Aphysoft.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Center
{
    internal class IntentEntity
    {
        private string intent;

        public string Intent
        {
            get { return intent; }
        }

        private bool asking;

        public bool Asking
        {
            get { return asking; }
        }

        private bool mentionMyName;

        public bool MentionMyName
        {
            get { return mentionMyName; }
            set { mentionMyName = value; }
        }

        public IntentEntity(string intent, bool asking, bool mentionMyName)
        {
            this.intent = intent;
            this.asking = asking;
            this.mentionMyName = mentionMyName;
        }
    }
    
    internal class Intent
    {
        private List<IntentEntity> entities = new List<IntentEntity>();

        public IntentEntity[] Entities
        {
            get { return entities.ToArray(); }
        }

        public Intent()
        {

        }
        
        private void Add(IntentEntity entity)
        {
            entities.Add(entity);
        }

        private static Regex urlRegex = new Regex(@"(((http|ftp|https):\/\/)*[\w\-_]+(\.(com|net|org|edu|ac|co|go|id|us|tk|sg|au|center|tv|info|xyz|gov))+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?)");

        private static char[] messageSeparators = { ' ', '.', ',', '?', '!' };
        private static char[] tokenTrims = { '~', '`', '@', '#', '$', '^', '*' };

        private static string[] constraintPriority = { "SW" };

        private static Dictionary<string, List<Tuple<string, string>>> intentReferences = new Dictionary<string, List<Tuple<string, string>>>();

        public static void Init()
        {
            Result result;
            Database center = Share.Database;

            intentReferences.Clear();

            // update nlw yg blum ada double metaphonenya
            result = center.Query("select * from NaturalLanguageWord where NLW_Word is not null and NLW_DM is null");

            if (result.Count > 0)
            {
                Batch batch = center.Batch();
                batch.Begin();
                foreach (Row row in result)
                {
                    string word = row["NLW_Word"].ToString();
                    DoubleMetaphone dm = new DoubleMetaphone(word);
                    string dmw = string.Format("{0}-{1}", dm.PrimaryKey, dm.AlternateKey);

                    Update update = center.Update("NaturalLanguageWord");
                    update.Set("NLW_DM", dmw);
                    update.Where("NLW_ID", row["NLW_ID"].ToLong());
                    batch.Execute(update);
                }
                batch.Commit();
            }

            List<string> intents = center.QueryList("select distinct NLW_Intent from NaturalLanguageWord", "NLW_Intent");

            foreach (string intent in intents)
            {
                Column cmax = center.Scalar("select max(NLW_Position) from NaturalLanguageWord where NLW_Intent = {0}", intent);
                int max = cmax.ToInt() + 1;

                List<Tuple<string, string>>[] lists = new List<Tuple<string, string>>[max];
                for (int i = 0; i < max; i++) lists[i] = new List<Tuple<string, string>>();

                result = center.Query("select NLW_Position, NLW_Constraint, NLW_DM from NaturalLanguageWord where NLW_Intent = {0}", intent);

                foreach (Row row in result)
                {
                    int pos = row["NLW_Position"].ToInt();
                    lists[pos].Add(new Tuple<string, string>(row["NLW_DM"].ToString(), row["NLW_Constraint"].ToString()));
                }

                IntentWordCombination(lists, intent, null, new StringBuilder(), 0);
            }

            foreach (KeyValuePair<string, List<Tuple<string, string>>> pair in intentReferences)
            {
                pair.Value.Sort(delegate (Tuple<string, string> a, Tuple<string, string> b)
                {
                    string ae = a.Item2;
                    string be = b.Item2;

                    if (ae == be) return 0;
                    else if (ae != null && be == null) return -1;
                    else if (ae == null && be != null) return 1;
                    else
                    {
                        int ac = Array.IndexOf(constraintPriority, ae);
                        int bc = Array.IndexOf(constraintPriority, be);

                        if (ac < bc) return 1;
                        else if (ac > bc) return -1;
                        else return 0;
                    }

                });
            }

        }

        private static void IntentWordCombination(List<Tuple<string, string>>[] lists, string intent, string constraint, StringBuilder sb, int index)
        {
            if (index < lists.Length)
            {
                List<Tuple<string, string>> list = lists[index];

                if (index > 0) sb.Append(" ");

                foreach (Tuple<string, string> entry in list)
                {
                    StringBuilder sbc = new StringBuilder(sb.ToString());

                    if (index == 0) constraint = entry.Item2;

                    IntentWordCombination(lists, intent, constraint, sbc.Append(entry.Item1), index + 1);
                }
            }
            else
            {
                string dmkey = sb.ToString().Trim();
                if (!intentReferences.ContainsKey(dmkey))
                {
                    List<Tuple<string, string>> entryList = new List<Tuple<string, string>>();
                    intentReferences.Add(dmkey, entryList);
                }
                intentReferences[dmkey].Add(new Tuple<string, string>(intent, constraint));
            }
        }
        
        public static Intent Parse(string message)
        {
            if (message.Length > 50) return null;

            string messageLower = message.ToLower().Trim();

            string messageOneSpace = string.Join(" ", messageLower.Split(StringSplitTypes.Space, StringSplitOptions.RemoveEmptyEntries));

            List<string> tokens = new List<string>();
            List<string> dmTokens = new List<string>();

            int startIndex = 0;
            foreach (string token in messageLower.Split(messageSeparators, StringSplitOptions.RemoveEmptyEntries))
            {                
                if (token == "center")
                {
                    tokens.Add("{MENTIONMYNAME}");
                    continue;
                }

                int tokenIndex = messageOneSpace.IndexOf(token, startIndex);
                startIndex = tokenIndex + token.Length;

                if (tokenIndex >= 2)
                {
                    if (messageOneSpace[tokenIndex - 2] == '?') tokens.Add("{QUESTIONMARK}");
                }
                                
                string ntoken = token;
                if (ntoken.Length > 2 && !char.IsDigit(ntoken[0]) && ntoken.EndsWith("2"))
                    ntoken = string.Format("{0}-{0}", ntoken);
                ntoken = ntoken.Trim(tokenTrims);

                tokens.Add(token);
            }

            if (messageOneSpace.IndexOf("?", startIndex) > -1) tokens.Add("{QUESTIONMARK}");
            
            if (tokens.Count > 0)
            {
                foreach (string token in tokens)
                {
                    if (token.StartsWith("{") && token.EndsWith("}")) dmTokens.Add(token);
                    else
                    {
                        DoubleMetaphone dm = new DoubleMetaphone(token);
                        string dmToken = string.Format("{0}-{1}", dm.PrimaryKey, dm.AlternateKey);
                        dmTokens.Add(dmToken);
                    }
                }                

                int checkPair = 4;
                if (dmTokens.Count < checkPair) checkPair = dmTokens.Count;

                // 0 1 2 3 4 5 6 7
                // 3 4 5
                // i = 3, checkPair = 3
                // 0 1 2 6 7

                Intent intent = new Intent();
                
                while (checkPair > 0)
                {
                    int until = dmTokens.Count - checkPair;

                    for (int i = 0; i <= until; i++)
                    {
                        string s = string.Join(" ", dmTokens.ToArray(), i, checkPair).Trim();

                        if (s != string.Empty)
                        {
                            if (intentReferences.TryGetValue(s, out List<Tuple<string, string>> references))
                            {
                                foreach (Tuple<string, string> reference in references)
                                {
                                    string name = reference.Item1;
                                    string constraint = reference.Item2;
                                    bool ask = false;
                                    bool mentionmyname = false;

                                    if (constraint == "SW")
                                    {
                                        if (i > 0) continue;
                                    }
                                    for (int y = 0; y < checkPair; y++)
                                    {
                                        dmTokens[y + i] = "";
                                    }

                                    int te = i + checkPair;
                                    if (dmTokens.Count > te)
                                    {
                                        if (dmTokens[te] == "{QUESTIONMARK}") ask = true;
                                        else if (dmTokens[te] == "{MENTIONMYNAME}") mentionmyname = true;
                                    }


                                    bool exists = false;
                                    foreach (IntentEntity ent in intent.entities)
                                    {
                                        if (ent.Intent == name)
                                        {
                                            exists = true;
                                            break;
                                        }
                                    }
                                    if (!exists)
                                    {
                                        intent.entities.Add(new IntentEntity(name, ask, mentionmyname));
                                    }
                                }
                            }
                        }
                    }

                    checkPair--;                    
                }

                return intent;
            }
            else
                return null;
        }
    }
}
