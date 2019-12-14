using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aphysoft.Share
{
    [Serializable]
    public sealed class Result2 : IList<Row2>
    {
        #region Fields

        public bool IsExceptionThrown { get; set; } = false;

        private List<Row2> rows;

        public TimeSpan ExecutionTime { get; set; }

        public int AffectedRows { get; set; } = 0;

        public Int64 Identity { get; set; }

        public string[] ColumnNames { get; set; }

        public string Sql { get; set; }

        public static Result2 Null
        {
            get { return new Result2(""); }
        }

        #endregion

        #region Constructor

        public Result2(string sql)
        {
            rows = new List<Row2>();
            this.Sql = sql;
        }

        #endregion

        #region Methods

        public int IndexOf(Row2 item)
        {
            return rows.IndexOf(item);
        }

        public void Insert(int index, Row2 item)
        {
            rows.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            rows.RemoveAt(index);
        }

        public Row2 this[int index]
        {
            get
            {
                if (IsExceptionThrown)
                {
                    throw new Exception("Warning: This result contain exceptions");
                }
                return rows[index];
            }
            set { }
        }

        public Row2 Last()
        {
            return rows == null ? null : rows.Count == 0 ? null : rows[rows.Count - 1];
        }

        public void Add(Row2 item)
        {
            rows.Add(item);
        }

        public void Clear()
        {
            rows.Clear();
        }

        public bool Contains(Row2 item)
        {
            return rows.Contains(item);
        }

        public void CopyTo(Row2[] array, int arrayIndex)
        {
            rows.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return rows.Count; }
        }

        public static implicit operator bool(Result2 d)
        {
            return !d.IsExceptionThrown;
        }

        public static implicit operator int(Result2 d)
        {
            return d.Count;
        }

        public override string ToString()
        {
            return Count.ToString();
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(Row2 item)
        {
            return rows.Remove(item);
        }

        public IEnumerator<Row2> GetEnumerator()
        {
            return rows.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)rows.GetEnumerator();
        }

        #endregion
    }
}
