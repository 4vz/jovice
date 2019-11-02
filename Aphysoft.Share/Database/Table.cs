using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Aveezo;

namespace Aphysoft.Share
{
    public class OldColumnAttribute : Attribute
    {
        public string Name { get; set; }

        public OldColumnAttribute(string name)
        {
            Name = name;
        }
    }

    public class OldTableAttribute : Attribute
    {
        public string Name { get; set; }

        public string ShortName { get; set; }

        public string IdFormat { get; set; }

        public OldTableAttribute(string name, string shortName, string idFormat)
        {
            Name = name;
            ShortName = shortName;
            IdFormat = idFormat;
        }
    }



    public class OldTable<T> : IDictionary<string, T> where T : Data
    {
        #region Fields
               

        #endregion

        #region Contructors

        private OldTable()
        {

        }

        #endregion

        #region Dictionary

        public T this[string key] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public T this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public ICollection<string> Keys => throw new NotImplementedException();

        public ICollection<T> Values => throw new NotImplementedException();

        public int Count => throw new NotImplementedException();

        public bool IsReadOnly => throw new NotImplementedException();

        public void Add(string key, T value)
        {
            throw new NotImplementedException();
        }

        public void Add(KeyValuePair<string, T> item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(KeyValuePair<string, T> item)
        {
            throw new NotImplementedException();
        }

        public bool ContainsKey(string key)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(KeyValuePair<string, T>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<string, T>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public bool Remove(string key)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<string, T> item)
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(string key, out T value)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Methods


        #endregion

    }
}
