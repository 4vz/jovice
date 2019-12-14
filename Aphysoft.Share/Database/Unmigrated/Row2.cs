using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace Aphysoft.Share
{
    [Serializable]
    public sealed class Row2 : IDictionary<string, Column2>
    {
        #region Fields

        private Dictionary<string, Column2> columns;

        #endregion

        #region Constructor

        public Row2()
        {
            columns = new Dictionary<string, Column2>();
        }

        #endregion

        #region Methods

        public void Add(string key, Column2 value)
        {
            columns.Add(key, value);
        }

        public bool ContainsKey(string key)
        {
            return columns.ContainsKey(key);
        }

        public ICollection<string> Keys
        {
            get { return columns.Keys; }
        }

        public bool Remove(string key)
        {
            return columns.Remove(key);
        }

        public bool TryGetValue(string key, out Column2 value)
        {
            return columns.TryGetValue(key, out value);
        }

        public ICollection<Column2> Values
        {
            get { return columns.Values; }
        }

        public Column2 this[int index]
        {
            get
            {
                if (index >= 0 && index < columns.Count)
                {
                    int i = 0;
                    foreach (KeyValuePair<string, Column2> kvpc in columns)
                    {
                        if (i == index) return kvpc.Value;
                        i++;
                    }
                    return null;
                }
                else return null;
            }
            set { }
        }

        public Column2 this[string key]
        {
            get
            {
                return columns[key];
            }
            set { }
        }

        public void Add(KeyValuePair<string, Column2> item)
        {
            columns.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            columns.Clear();
        }

        public bool Contains(KeyValuePair<string, Column2> item)
        {
            return columns.ContainsKey(item.Key);
        }

        public void CopyTo(KeyValuePair<string, Column2>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { return columns.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(KeyValuePair<string, Column2> item)
        {
            return columns.Remove(item.Key);
        }

        public IEnumerator<KeyValuePair<string, Column2>> GetEnumerator()
        {
            return columns.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)columns.GetEnumerator();
        }

        #endregion
    }
}
