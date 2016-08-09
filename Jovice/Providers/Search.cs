using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using Aphysoft.Common;
using Aphysoft.Share;

namespace Jovice.Providers
{
    public static class Search
    {
        private static List<SearchMatch> matches = null;
        private static Dictionary<string, List<string>> roots = null;
        private static List<string> languages = null;
        private static Dictionary<string, string> references = null;
        private static List<string> conjuctions = null;

        public static void Language(string language, string reference)
        {
            if (!languages.Contains(language))
                languages.Add(language);
            if (!references.ContainsKey(language) ||
                !references[language].Contains(reference))
                references.Add(language, reference);
        }

        public static void Root(string root, string phrase)
        {
            if (!roots.ContainsKey(root)) roots.Add(root, new List<string>());
            if (!roots[root].Contains(phrase)) roots[root].Add(phrase);
        }

        public static string JoinValues(string[] values)
        {
            if (values == null || values.Length == 0) return string.Empty;
            else if (values.Length == 1) return values[0];
            else if (values.Length == 2) return values[0] + " and " + values[1];
            else return string.Join(", ", values, 0, values.Length - 1) + " and " + values[values.Length - 1];
        }
        
        public static ProviderPacket ProviderRequest(ResourceResult result, int id)
        {
            #region Lazy Load for searches
            if (matches == null)
            {
                matches = new List<SearchMatch>();
                languages = new List<string>();
                references = new Dictionary<string, string>();
                roots = new Dictionary<string, List<string>>();

                conjuctions = List.New(
                    "is", "are", "that", "thats", "that's", "which", "of", "have", "has", "had",
                    "the", "to", "than", "then", "where", "with", "bound"
                    );
                
                #region Matches

                matches.Add(new ServiceSearchMatch());
                matches.Add(new InterfaceSearchMatch());

                #endregion

                List.Sort(languages, SortMethods.LengthDescending);
                List.Sort(conjuctions, SortMethods.LengthDescending);      
            }
            #endregion
            
            if (id == 101) // 101 Search
            {
                #region Search

                string search = Params.GetValue("s").Trim(); // search query
                string main = Params.GetValue("m");
                string sid = Params.GetValue("sid"); // previous search id;

                bool isMain = false;
                if (main == "1") isMain = true;

                List<string> tokenList = List.Tokenize(search);

                #region Determine root

                string root = null;
                List<string> prevTokens = new List<string>();
                string[] preTokens = null, postTokens = null, tokens = null;

                int tokenIndex = 0;
                foreach (string token in tokenList)
                {
                    prevTokens.Add(token.ToLower());

                    for (int i = prevTokens.Count - 1; i >= 0; i--)
                    {
                        string compoundToken = string.Join(" ", prevTokens.ToArray(), i, prevTokens.Count - i);
                        root = Dictionary.Key(roots, compoundToken.ToLower());

                        if (root != null)
                        {
                            tokens = tokenList.ToArray();
                            preTokens = List.Copy(tokenList, 0, tokenIndex).ToArray();
                            postTokens = List.Copy(tokenList, tokenIndex + 1, tokenList.Count - (tokenIndex + 1)).ToArray();
                            break;
                        }
                    }

                    if (root != null) break;

                    tokenIndex++;
                }

                #endregion

                ProviderSearchResult searchResult = null;

                Result r;

                Database jovice = Jovice.Database;
                Database center = Center.Database;

                if (root != null)
                {
                    SearchMatch match = null;
                    foreach (SearchMatch m in matches) { if (m.Name == root) match = m; }

                    #region Process
                    string sort = Params.GetValue("o"); // sort
                    if (sort == "") sort = null;

                    string sortMethod = Params.GetValue("ot"); // sort method                
                    if (sortMethod != "asc" && sortMethod != "desc") sortMethod = null;

                    int page;
                    string pageString = Params.GetValue("p"); // page
                    if (pageString == null || !int.TryParse(pageString, out page)) page = 0;

                    int pageSize;
                    string pageSizeString = Params.GetValue("n"); // n page;
                    if (pageSizeString == null || !int.TryParse(pageSizeString, out pageSize)) pageSize = 20;

                    int pageLength;
                    string pageLengthString = Params.GetValue("u"); // page length;
                    if (pageLengthString == null || !int.TryParse(pageLengthString, out pageLength)) pageLength = 1;

                    SearchMatchResult matchResult = new SearchMatchResult();
                    match.Process(matchResult, tokens, preTokens, postTokens, sort, sortMethod, page, pageSize, pageLength);

                    if (matchResult.ResultCount == -1) // we are using auto query
                    {
                        if (matchResult.QueryCount == null) matchResult.QueryCount = matchResult.Query;

                        if (isMain)
                        {
                            r = jovice.Query("select count(*) from ( " + matchResult.QueryCount + " ) source");
                            matchResult.ResultCount = r[0][0].ToInt();
                        }

                        string sortString = matchResult.RowID;

                        if (sort != null)
                        {
                            if (sortMethod != null)  sortString = sort + " " + sortMethod;
                            else sortString = sort;
                        }

                        string psql =
                            "select * from (select ROW_NUMBER() over (order by " + sortString + ") as ROWNUM, " + matchResult.RowID + " as ROWID, * from (" +
                            matchResult.Query + ") source) source where ROWNUM > " + (page * pageSize) + " AND ROWNUM <= " + ((page + pageLength) * pageSize);

                        r = jovice.Query(psql);

                        matchResult.Columns = r.ColumnNames;

                        List<object[]> results = new List<object[]>();

                        foreach (Row row in r)
                        {
                            List<object> objects = new List<object>();

                            foreach (KeyValuePair<string, Column> column in row)
                            {
                                objects.Add(column.Value.ToObject());
                            }

                            match.RowProcess(matchResult, objects);

                            string[] hides = matchResult.Hides;

                            if (hides != null)
                            {
                                string[] columns = matchResult.Columns;
                                foreach (string columnToHide in hides)
                                {
                                    int ci = -1;
                                    foreach (string column in columns)
                                    {
                                        ci++;
                                        if (column == columnToHide)
                                        {
                                            objects[ci] = null;
                                            break;
                                        }
                                    }
                                }
                                objects[1] = null; // ROWID
                            }

                            results.Add(objects.ToArray());
                        }

                        matchResult.Results = results.ToArray();
                    }

                    searchResult = new ProviderSearchResult(matchResult.Type);

                    searchResult.Results = matchResult.Results;

                    if (isMain)
                    {
                        searchResult.ResultCount = matchResult.ResultCount;
                        searchResult.Columns = matchResult.Columns;
                        searchResult.Filters = matchResult.Sorts;

                        int pageCount = (int)Math.Floor((decimal)(matchResult.ResultCount - 1) / pageSize) + 1;
                        if ((page + 1) > pageCount) page = pageCount - 1;

                        searchResult.PageCount = pageCount;
                        searchResult.PageSize = pageSize;
                        searchResult.Page = page;

                        Tuple<string, string>[] relates = matchResult.Relates;

                        if (relates != null)
                        {
                            List<string> exQueries = new List<string>();
                            List<string> exExplainations = new List<string>();

                            foreach (Tuple<string, string> relate in relates)
                            {
                                exQueries.Add(relate.Item1);
                                exExplainations.Add(relate.Item2);
                            }

                            searchResult.ExQueries = exQueries.ToArray();
                            searchResult.ExExplainations = exExplainations.ToArray();
                        }

                        Tuple<string, string>[] otherRelates = matchResult.OtherRelates;
                        
                        if (otherRelates != null)
                        {
                            List<string> exQueries = new List<string>();
                            List<string> exExplainations = new List<string>();

                            foreach (Tuple<string, string> relate in otherRelates)
                            {
                                exQueries.Add(relate.Item1);
                                exExplainations.Add(relate.Item2);
                            }

                            searchResult.OtQueries = exQueries.ToArray();
                            searchResult.OtExplainations = exExplainations.ToArray();
                        }                        
                    }
                    #endregion
                }
                else
                {
                    searchResult = new ProviderSearchResult("");
                    
                    List<string> exQueries = new List<string>();
                    List<string> exExplainations = new List<string>();

                    #region SID
                    
                    if (tokenList.Count > 0 && tokenList[0].Length > 3)
                    {
                        // 4700278-0006182119

                        string ssid = tokenList[0];

                        Column c;
                        c = jovice.Scalar("select count(*) from Service where SE_SID = {0}", ssid);

                        if (c.ToInt() > 0)
                        {
                            exQueries.Add("service with SID " + ssid);
                            exExplainations.Add("Searches service with specified SID.");                           
                        }
                        if (ssid.Length == 7 && (ssid.StartsWith("47") || ssid.StartsWith("37")))
                        {
                            exQueries.Add("service that SID starts with " + ssid);
                            exExplainations.Add("Searches service with specified SID.");
                        }
                        if (ssid.Length >= 5 && ssid.Length <= 18 && StringHelper.IsAllIsIn(ssid, commonSIDCharacters))
                        {
                            exQueries.Add("service that SID contain " + ssid);
                            exExplainations.Add("Searches all services that the SID includes the specified text.");
                        }
                    }

                    #endregion

                    if (exQueries.Count > 0)
                    {
                        searchResult.DidYouMean = true;
                    }
                    else
                    {
                        searchResult.DidNotUnderstand = true;
                    }

                    searchResult.ExQueries = exQueries.ToArray();
                    searchResult.ExExplainations = exExplainations.ToArray();
                }



                if (sid == null)
                {
                    r = center.ExecuteIdentity(@"
insert into SearchResult(SR_Created, SR_Address, SR_Query, SR_Match, SR_Count)
values(GETUTCDATE(), {0}, {1}, {2}, {3})
", result.Context.Request.UserHostAddress, search, searchResult.Type, searchResult.ResultCount);
                    searchResult.SearchID = r.Identity.ToString();
                }



                return searchResult;               
                
                #endregion
            }

            return ProviderPacket.Null();
        }

        static readonly char[] commonSIDCharacters = { '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '-' };

        public static List<SearchDescriptor> ParsePostTokens(string[] postTokens)
        {
            List<SearchDescriptor> descriptors = new List<SearchDescriptor>();

            StringBuilder phrases = new StringBuilder();

            int tokenIndex = 0;

            string lastDescriptor = null;
            string lastSuperDescriptor = null;
            string lastConstraint = null;
            string lastSeparator = "AND";

            int lastDescriptorIndex = -1;
            int referToDescriptor = -1;

            foreach (string token in postTokens)
            {
                string tokenForSearch = token.ToLower();
                bool endOfPhrase = false;
                string thisSeparator = lastSeparator;

                if (tokenForSearch == "and" || tokenForSearch == "or" || tokenForSearch == "but")
                {
                    if (tokenForSearch == "and" || tokenForSearch == "but")
                        lastSeparator = "AND";
                    else
                        lastSeparator = "OR";

                    endOfPhrase = true;
                }
                else
                {
                    if (phrases.Length > 0) phrases.Append(" ");

                    if (tokenForSearch.EndsWith(","))
                    {
                        phrases.Append(token.Substring(0, token.Length - 1));
                        lastSeparator = "AND";
                        endOfPhrase = true;
                    }
                    else
                    {
                        phrases.Append(token);
                        if (tokenIndex == postTokens.Length - 1) endOfPhrase = true;
                    }
                }

                if (endOfPhrase)
                {
                    if (phrases.Length > 0)
                    {
                        string phrase = phrases.ToString();
                        string phraseForSearch = " " + phrase.ToLower() + " ";
                        string phraseExisting = " " + phrase + " ";

                        int descriptorIndex = -1;
                        int descriptorLength = 0;
                        string descriptor = null;
                        string superDescriptor = null;
                        string constraint = null;
                        string value = null;

                        foreach (string language in languages)
                        {
                            int languageIndex = -1;
                            int languageLength = 0;

                            if (descriptorIndex == -1)
                            {
                                languageIndex = phraseForSearch.IndexOf(" " + language + " ");
                                languageLength = language.Length;

                                if (languageIndex == -1)
                                {
                                    languageIndex = phraseForSearch.IndexOf(" " + language + "'s ");
                                    languageLength = language.Length + 2;
                                }
                                if (languageIndex == -1)
                                {
                                    languageIndex = phraseForSearch.IndexOf(" " + language + "' ");
                                    languageLength = language.Length + 1;
                                }

                                if (languageIndex > -1)
                                {
                                    descriptor = language;
                                    descriptorIndex = languageIndex + 1;
                                    descriptorLength = languageLength;

                                    phraseExisting = phraseExisting.Remove(descriptorIndex - 1, descriptorLength + 1);
                                }
                            }
                            else
                            {
                                string frontPhraseForSearch = phraseForSearch.Substring(0, descriptorIndex);

                                // if front
                                languageIndex = frontPhraseForSearch.IndexOf(" " + language + " ");
                                languageLength = language.Length;

                                if (languageIndex == -1)
                                {
                                    // depan's
                                    languageIndex = frontPhraseForSearch.IndexOf(" " + language + "'s ");
                                    languageLength = language.Length + 2;
                                }
                                if (languageIndex == -1)
                                {
                                    // depan'
                                    languageIndex = frontPhraseForSearch.IndexOf(" " + language + "' ");
                                    languageLength = language.Length + 1;
                                }

                                if (languageIndex > -1)
                                {
                                    languageIndex += 1;

                                    phraseExisting = phraseExisting.Remove(languageIndex - 1, languageLength + 1);

                                    // front exists
                                    // ada backward conjuction?

                                    bool inspace = false;    
                                    int spacelength = descriptorIndex - (languageIndex + languageLength);

                                    if (spacelength > 0) 
                                    {
                                        string space = phraseForSearch.Substring(languageIndex + languageLength, spacelength);
                                        if (space.IndexOf(" of ") > -1 || space.IndexOf(" in ") > -1 || space.IndexOf(" on ") > -1 || space.IndexOf(" at ") > -1) inspace = true;
                                    }

                                    if (!inspace)
                                    {
                                        superDescriptor = language;
                                    }
                                    else
                                    {
                                        superDescriptor = descriptor;
                                        descriptor = language;
                                    }
                                }

                                if (languageIndex == -1)
                                {
                                    // if back
                                    string backPhraseForSearch = phraseForSearch.Substring(descriptorIndex + descriptorLength);

                                    languageIndex = backPhraseForSearch.IndexOf(" " + language + " ");
                                    languageLength = language.Length;

                                    if (languageIndex == -1)
                                    {
                                        // back's
                                        languageIndex = backPhraseForSearch.IndexOf(" " + language + "'s ");
                                        languageLength = language.Length + 2;
                                    }
                                    if (languageIndex == -1)
                                    {
                                        // back'
                                        languageIndex = backPhraseForSearch.IndexOf(" " + language + "' ");
                                        languageLength = language.Length + 1;
                                    }

                                    if (languageIndex > -1)
                                    {
                                        languageIndex += 1;

                                        phraseExisting = phraseExisting.Remove(languageIndex + descriptorIndex - 1, languageLength + 1);

                                        // back exists
                                        // ada backward conjuction?

                                        bool inspace = false;
                                        int spacelength = languageIndex;

                                        if (spacelength > 0)
                                        {
                                            string space = backPhraseForSearch.Substring(0, spacelength);
                                            if (space.IndexOf(" of ") > -1 || space.IndexOf(" in ") > -1 || space.IndexOf(" on ") > -1 || space.IndexOf(" at ") > -1) inspace = true;
                                        }

                                        if (!inspace)
                                        {
                                            superDescriptor = descriptor;
                                            descriptor = language;
                                        }
                                        else
                                        {
                                            superDescriptor = language;
                                        }
                                    }
                                }
                            }

                            if (superDescriptor != null) break;
                        }
                        
                        if (descriptor == null)
                        {
                            

                        }

                        string descriptorCode = null;
                        string superDescriptorCode = null;

                        if (descriptor != null) descriptorCode = references[descriptor];
                        if (superDescriptor != null) superDescriptorCode = references[superDescriptor];

                        #region Constraints
                        string phraseExistingLower = phraseExisting.ToLower();

                        int index, phraseIndex, wordLength;
                        if (StringHelper.Find(phraseExistingLower, out index, out phraseIndex, out wordLength, 
                            " not contain ", " not contained ", " not like ",
                            "n't contain ", "n't contained ", "n't like "
                            ))
                            constraint = "NOTLIKE";
                        else if (StringHelper.Find(phraseExistingLower, out index, out phraseIndex, out wordLength, 
                            " contain ", " contained ", " like "
                            ))
                            constraint = "LIKE";
                        else if (StringHelper.Find(phraseExistingLower, out index, out phraseIndex, out wordLength, 
                            " not starts with ", " not starts ", " not begins ", " not begins with ", " not begin with ", " not start with ", " not starting with ", " not starting ",
                            "n't starts with ", "n't starts ", "n't begins ", "n't begins with ", "n't begin with ", "n't start with ", "n't starting with ", "n't starting "
                            ))
                            constraint = "NOTSTARTSWITH";
                        else if (StringHelper.Find(phraseExistingLower, out index, out phraseIndex, out wordLength, 
                            " starts with ", " starts ", " begins ", " begin ", " begins with ", " begin with ", " start with ", " starting with ", " starting ", " start "
                            ))
                            constraint = "STARTSWITH";
                        else if (StringHelper.Find(phraseExistingLower, out index, out phraseIndex, out wordLength, 
                            " not ends with ", " not ending with ", " not end with ", " not end ",
                            "n't ends with ", "n't ending with ", "n't end with ", "n't end "
                            ))
                            constraint = "NOTENDSWITH";
                        else if (StringHelper.Find(phraseExistingLower, out index, out phraseIndex, out wordLength, 
                            " ends with ", " ending with ", " end with ", " end "
                            ))
                            constraint = "ENDSWITH";
                        else if (StringHelper.Find(phraseExistingLower, out index, out phraseIndex, out wordLength,
                            " not larger ", "n't larger "
                            ))
                            constraint = "NOTLARGER";
                        else if (StringHelper.Find(phraseExistingLower, out index, out phraseIndex, out wordLength,
                            " larger "
                            ))
                            constraint = "LARGER";
                        else if (StringHelper.Find(phraseExistingLower, out index, out phraseIndex, out wordLength,
                            " not smaller ", "n't smaller "
                            ))
                            constraint = "NOTSMALLER";
                        else if (StringHelper.Find(phraseExistingLower, out index, out phraseIndex, out wordLength,
                            " smaller "
                            ))
                            constraint = "SMALLER";
                        else if (StringHelper.Find(phraseExistingLower, out index, out phraseIndex, out wordLength,
                            " have not ", " has not ", " had not ", " is not ", " not equal ", " are not ",
                            " haven't ", " hasn't ", " hadn't ", " isn't ", " aren't ", " not "
                            ))
                            constraint = "NOTEQUAL";
                        else if (StringHelper.Find(phraseExistingLower, out index, out phraseIndex, out wordLength,
                            " have ", " has ", " had ", " is ", " equal ", " are ", " = "
                            ))
                            constraint = "EQUAL";
                        else
                        {
                            constraint = "EQUAL";
                            index = -1;
                        }
                        #endregion

                        if (index > -1)
                        {
                            phraseExisting = phraseExisting.Remove(phraseIndex, wordLength - 1);
                        }

                        foreach (string conjunction in conjuctions)
                        {
                            phraseIndex = phraseExisting.ToLower().IndexOf(" " + conjunction + " ");
                            wordLength = conjunction.Length;

                            if (phraseIndex > -1)
                            {
                                phraseExisting = phraseExisting.Remove(phraseIndex, wordLength + 1);
                            }
                        }

                        value = phraseExisting.Trim();

                        bool changed = false;

                        if (descriptorCode != null)
                        {
                            if (descriptorCode != lastDescriptor) changed = true;
                            lastDescriptor = descriptorCode;
                        }
                        else
                        {
                            descriptorCode = lastDescriptor;
                        }
                        
                        if (superDescriptorCode != null)
                        {
                            if (superDescriptorCode != lastSuperDescriptor) changed = true;
                            lastSuperDescriptor = superDescriptorCode;
                        }
                        else
                        {
                            superDescriptorCode = lastSuperDescriptor;
                        }

                        if (constraint != null)
                        {
                            if (constraint != lastConstraint) changed = true;
                            lastConstraint = constraint;
                        }
                        else
                        {
                            constraint = lastConstraint;
                        }

                        //Service.Event("value: " + value + " changed: " + (changed == true) + " referToDescriptor: " + referToDescriptor);

                        if (descriptorCode != null)
                        {
                            if (changed)
                            {
                                SearchDescriptor sd = new SearchDescriptor();

                                sd.Descriptor = descriptorCode;
                                sd.SuperDescriptor = superDescriptorCode;
                                sd.Constraint = constraint;
                                sd.Separator = thisSeparator;
                                sd.Add(value);

                                descriptors.Add(sd);

                                referToDescriptor = descriptors.Count - 1;
                            }
                            else
                            {
                                descriptors[referToDescriptor].Add(value);
                            }

                            lastDescriptorIndex++;
                        }

                        
                    }
                    phrases.Clear();
                }

                tokenIndex++;
            }

            return descriptors;
        }        
    }

    public class SearchDescriptor
    {
        #region Fields

        private string descriptor;

        public string Descriptor
        {
            get { return descriptor; }
            set { descriptor = value; }
        }

        private string superDescriptor;

        public string SuperDescriptor
        {
            get { return superDescriptor; }
            set { superDescriptor = value; }
        }

        private string constraint;

        public string Constraint
        {
            get { return constraint; }
            set { constraint = value; }
        }

        private string separator;

        public string Separator
        {
            get { return separator; }
            set { separator = value; }
        }

        private List<string> values = new List<string>();

        public string[] Values
        {
            get 
            { 
                return values.ToArray();
            }
        }

        #endregion

        #region Methods

        public void Add(string value)
        {
            values.Add(value);
        }

        public delegate string ValueCallback(int index, string value);
        public delegate string DescriptorCallback(SearchDescriptor descriptor);

        public string Build(ValueCallback callback)
        {
            StringBuilder inner = new StringBuilder();

            int index = 0;

            foreach (string value in values)
            {
                string phrase = callback(index, value);

                if (phrase != null && phrase.Length > 0)
                {
                    if (inner.Length > 0) inner.Append(" OR ");
                    inner.Append(phrase);
                }

                index++;
            }

            return inner.Length > 0 ? "(" + inner.ToString() + ")" : null;
        }

        public static Where Build(List<SearchDescriptor> descriptors, DescriptorCallback callback)
        {
            StringBuilder outer = new StringBuilder();

            foreach (SearchDescriptor searchDescriptor in descriptors)
            {
                string phrase = callback(searchDescriptor);

                if (phrase != null && phrase.Length > 0)
                {
                    if (outer.Length > 0)
                    {
                        if (searchDescriptor.Separator == "AND") outer.Append(" AND ");
                        else if (searchDescriptor.Separator == "OR") outer.Append(" OR ");
                    }
                    outer.Append(phrase);
                }
            }
            
            string where = outer.Length > 0 ? "(" + outer.ToString() + ")" : null;

            return new Where(where);
        }

        #endregion
    }

    public class SearchMatch
    {
        #region Fields

        private string name = null;

        public string Name
        {
            get { return name; }
        }

        protected Database jovice = Jovice.Database;
        protected Database center = Center.Database;

        #endregion

        #region Methods

        protected void Root(string phrase)
        {
            if (name == null) name = Database.ID();
            Search.Root(name, phrase);
        }

        protected void Language(string language, string reference)
        {
            Search.Language(language, reference);
        }

        public virtual void Process(SearchMatchResult matchResult, string[] tokens, string[] preTokens, string[] postTokens, string sort, string sortMethod, int page, int pageSize, int pageLength)
        {
        }

        public virtual void RowProcess(SearchMatchResult matchResult, List<object> objects)
        {
        }

        #endregion
    }

    public class SearchMatchResult
    {
        #region Fields

        private string type = null;

        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        private string subType = null;

        public string SubType
        {
            get { return subType; }
            set { subType = value; }
        }

        private int resultCount = -1;

        public int ResultCount
        {
            get { return resultCount; }
            set 
            { 
                if (value > -1)
                    resultCount = value;
            }
        }

        private string query;

        public string Query
        {
            get { return query; }
            set { query = value; }
        }

        private string queryCount;

        public string QueryCount
        {
            get { return queryCount; }
            set { queryCount = value; }
        }

        private string rowID;

        public string RowID
        {
            get { return rowID; }
            set { rowID = value; }
        }

        private string[] columns = null;

        private List<string> extColumns = null;

        public string[] Columns
        {
            get 
            {
                if (columns == null && extColumns == null) return null;
                else if (columns == null && extColumns != null) return extColumns.ToArray();
                else if (columns != null && extColumns == null) return columns;
                else
                {
                    string[] obj = new string[columns.Length + extColumns.Count];
                    columns.CopyTo(obj, 0);
                    extColumns.CopyTo(obj, columns.Length);
                    return obj;
                }
            }
            set { columns = value; }
        }

        private List<string> sorts = null;

        public string[] Sorts
        {
            get 
            {
                if (sorts == null) return null;
                else return sorts.ToArray(); 
            }
        }

        private object[][] results = null;

        public object[][] Results
        {
            get { return results; }
            set { results = value; }
        }

        private List<Tuple<string, string>> relates = null;

        public Tuple<string, string>[] Relates
        {
            get 
            {
                if (relates == null) return null;
                else return relates.ToArray(); 
            }
        }

        private List<Tuple<string, string>> otherRelates = null;

        public Tuple<string, string>[] OtherRelates
        {
            get
            {
                if (otherRelates == null) return null;
                else return otherRelates.ToArray();
            }
        }

        private List<string> hides = null;

        public string[] Hides
        {
            get 
            {
                if (hides == null) return null;
                return hides.ToArray();
            }
        }

        #endregion

        #region Methods

        public void Sort(string name, string description)
        {
            if (sorts == null) sorts = new List<string>();
            sorts.Add(description + ":" + name);
        }

        public void Column(string name)
        {
            if (extColumns == null) extColumns = new List<string>();
            extColumns.Add(name);
        }

        public void RelatedSearch(string query, string explaination)
        {
            if (relates == null) relates = new List<Tuple<string, string>>();
            if (relates.Count < 10)
                relates.Add(new Tuple<string, string>(query, explaination));
        }

        public void OtherRelatesSearch(string query, string explaination)
        {
            if (otherRelates == null) otherRelates = new List<Tuple<string, string>>();
            if (otherRelates.Count < 10)
                otherRelates.Add(new Tuple<string, string>(query, explaination));
        }

        public void Hide(string name)
        {
            if (hides == null) hides = new List<string>();
            hides.Add(name);
        }

        #endregion
    }

    [DataContractAttribute]
    public class ProviderSearchResult : ProviderPacket
    {
        #region Fields

        private string sid;

        [DataMemberAttribute(Name = "sid")]
        public string SearchID
        {
            get { return sid; }
            set { sid = value; }
        }
        
        private string type = null;

        [DataMemberAttribute(Name = "t")]
        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        private string refinedSearch = null;

        [DataMemberAttribute(Name = "rs")]
        public string RefinedSearch
        {
            get { return refinedSearch; }
            set { refinedSearch = value; }
        }

        private string subType = null;

        [DataMemberAttribute(Name = "st")]
        public string SubType
        {
            get { return subType; }
            set { subType = value; }
        }

        private int resultCount = 0;

        [DataMemberAttribute(Name = "n")]
        public int ResultCount
        {
            get { return resultCount; }
            set { resultCount = value; }
        }

        private int page = 0;

        [DataMemberAttribute(Name = "pa")]
        public int Page
        {
            get { return page; }
            set { page = value; }
        }

        private int pageSize = 20;

        [DataMemberAttribute(Name = "pl")]
        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = value; }
        }

        private int pageCount = 0;

        [DataMemberAttribute(Name = "pc")]
        public int PageCount
        {
            get { return pageCount; }
            set { pageCount = value; }
        }

        private string[] columns = null;

        [DataMemberAttribute(Name = "c")]
        public string[] Columns
        {
            get { return columns; }
            set { columns = value; }
        }

        private string[] filters = null;

        [DataMemberAttribute(Name = "f")]
        public string[] Filters
        {
            get { return filters; }
            set { filters = value; }
        }

        private object[][] results = null;

        [DataMemberAttribute(Name = "r")]
        public object[][] Results
        {
            get { return results; }
            set { results = value; }
        }

        private bool didYouMean = false;

        [DataMemberAttribute(Name = "dy")]
        public bool DidYouMean
        {
            get { return didYouMean; }
            set { didYouMean = value; }
        }

        private bool didNotUnderstand = false;

        [DataMemberAttribute(Name = "du")]
        public bool DidNotUnderstand
        {
            get { return didNotUnderstand; }
            set { didNotUnderstand = value; }
        }

        private string[] exQueries = null;

        [DataMemberAttribute(Name = "xq")]
        public string[] ExQueries
        {
            get { return exQueries; }
            set { exQueries = value; }
        }

        private string[] exExplainations = null;

        [DataMemberAttribute(Name = "xe")]
        public string[] ExExplainations
        {
            get { return exExplainations; }
            set { exExplainations = value; }
        }

        private string[] otQueries = null;

        [DataMemberAttribute(Name = "oq")]
        public string[] OtQueries
        {
            get { return otQueries; }
            set { otQueries = value; }
        }

        private string[] otExplainations = null;

        [DataMemberAttribute(Name = "oe")]
        public string[] OtExplainations
        {
            get { return otExplainations; }
            set { otExplainations = value; }
        }

        #endregion

        #region Constructors

        public ProviderSearchResult(string type)
        {
            this.type = type;
        }

        #endregion
    }
}
