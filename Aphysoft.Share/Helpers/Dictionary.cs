using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aphysoft.Share
{
    public static class Dictionary
    {
        public static string Key<T>(Dictionary<string, List<T>> dictionary, T value)
        {
            foreach (KeyValuePair<string, List<T>> pair in dictionary)
            {
                string key = pair.Key;
                List<T> values = pair.Value;

                if (values.Contains(value)) return key;
            }

            return null;
        }

        public static string Key(Dictionary<string, List<string>> dictionary, string value)
        {
            return Key<string>(dictionary, value);
        }

        public static string Key(Dictionary<string, string[]> dictionary, string value)
        {
            foreach (KeyValuePair<string, string[]> pair in dictionary)
            {
                string key = pair.Key;
                string[] values = pair.Value;

                foreach (string cvalue in values)
                {
                    if (cvalue == value) return key;
                }
            }

            return null;
        }
    }

}
