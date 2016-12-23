using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aphysoft.Share
{
    public enum SortMethods
    {
        LengthAscending,
        LengthDescending,
        Ascending,
        Descending
    }

    public static class List
    {
        public static int IndexOf(string[] list, string find)
        {
            
            int res = 0;
            foreach (string item in list)
            {                
                if (item.IndexOf(find) > -1) return res;
                res++;
            }

            return -1;
        }

        public static int StartsWith(string[] list, string startsWith)
        {
            int res = 0;
            foreach (string item in list)
            {
                if (item.StartsWith(startsWith)) return res;
                res++;
            }

            return -1;
        }

        public static int EndsWith(string[] list, string endsWith)
        {
            int res = 0;
            foreach (string item in list)
            {
                if (item.EndsWith(endsWith)) return res;
                res++;
            }

            return -1;
        }

        public static int StartsWith(string text, string[] list)
        {
            int res = 0;
            foreach (string item in list)
            {
                if (text.StartsWith(item)) return res;
                res++;
            }

            return -1;
        }

        public static int EndsWith(string text, string[] list)
        {
            int res = 0;
            foreach (string item in list)
            {
                if (text.EndsWith(item)) return res;
                res++;
            }

            return -1;
        }
        
        
        public static List<T> New<T>(params T[] args)
        {
            List<T> list = new List<T>();
            foreach (T arg in args) list.Add(arg);
            return list;
        }

        public static List<string> New(params string[] args)
        {
            return List.New<string>(args);
        }

        public static List<T> Add<T>(List<T> list, params T[] args)
        {
            foreach (T arg in args)
            {
                list.Add(arg);
            }

            return list;
        }

        public static List<string> Add(List<string> list, params string[] args)
        {
            Add<string>(list, args);

            return list;
        }

        public static List<int> Add(List<int> list, params int[] args)
        {
            Add<int>(list, args);

            return list;
        }

        public static List<string> Sort(List<string> list, SortMethods type)
        {
            if (type == SortMethods.LengthAscending) list.Sort((a, b) => a.Length.CompareTo(b.Length));
            else if (type == SortMethods.LengthDescending) list.Sort((a, b) => b.Length.CompareTo(a.Length));
            else if (type == SortMethods.Ascending) list.Sort((a, b) => a.CompareTo(b));
            else if (type == SortMethods.Descending) list.Sort((a, b) => b.CompareTo(a));

            return list;
        }

        public static List<string> Tokenize(string text)
        {
            string[] rawTokens = text.Split(StringSplitTypes.Space, StringSplitOptions.RemoveEmptyEntries);
            List<string> tokens = new List<string>();
            StringBuilder innerToken = null;

            foreach (string token in rawTokens)
            {
                if (innerToken == null)
                {
                    if (token.StartsWith("\""))
                    {
                        if (token.EndsWith("\"") && token.Length > 1)
                        {
                            if (token.Length > 2) tokens.Add(token.Substring(1, token.Length - 2));
                        }
                        else
                        {
                            innerToken = new StringBuilder();
                            if (token.Length > 1) innerToken.Append(" " + token.Substring(1));
                        }
                    }
                    else if (token.EndsWith("\""))
                    {
                        if (token.Length > 1) tokens.Add(token.Substring(0, token.Length - 1));
                        innerToken = new StringBuilder();
                    }
                    else tokens.Add(token);
                }
                else
                {
                    if (token.EndsWith("\""))
                    {
                        if (token.StartsWith("\"") && token.Length > 1)
                        {
                            string prev = innerToken.ToString().Trim();
                            if (prev.Length > 0) tokens.Add(prev);
                            if (token.Length > 2) tokens.Add(token.Substring(1, token.Length - 2));
                            innerToken = new StringBuilder();
                        }
                        else
                        {
                            if (token.Length > 1) innerToken.Append(" " + token.Substring(0, token.Length - 1));
                            string prev = innerToken.ToString().Trim();
                            if (prev.Length > 0) tokens.Add(prev);
                            innerToken = null;
                        }
                    }
                    else if (token.StartsWith("\""))
                    {
                        string prev = innerToken.ToString().Trim();
                        if (prev.Length > 0) tokens.Add(prev);
                        innerToken = null;
                        if (token.Length > 1) tokens.Add(token.Substring(1));
                    }
                    else innerToken.Append(" " + token);
                }
            }
            if (innerToken != null)
            {
                string prev = innerToken.ToString().Trim();
                if (prev.Length > 0) tokens.Add(prev);
            }

            return tokens;
        }

        public static List<T> Copy<T>(List<T> source, int startIndex, int length)
        {
            if (source == null) return null;

            if (startIndex < 0) startIndex = 0;
            if (startIndex + length > source.Count) length = source.Count - startIndex;

            List<T> copy = new List<T>();

            for (int i = startIndex; i < length + startIndex; i++) copy.Add(source[i]);

            return copy;
        }

        public static List<string> Copy(List<string> source, int startIndex, int length)
        {
            return Copy<string>(source, startIndex, length);
        }
    }
}
