
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aphysoft.Share
{
    public abstract class DMLBase
    {
        #region Fields

        protected Database2 database;

        protected string table;

        protected string where = null;

        protected Dictionary<string, object> objects = null;

        public bool IsEmpty
        {
            get { return objects == null || objects.Count == 0; }
        }

        #endregion

        #region Constructors

        public DMLBase(string table, Database2 database)
        {
            this.table = table;
            this.database = database;
            objects = new Dictionary<string, object>();
        }

        #endregion

        #region Methods

        protected void Object(string key, object value)
        {

            if (objects.ContainsKey(key))
                objects[key] = value;
            else
                objects.Add(key, value);
        }

        #endregion
    }

    public class Insert : DMLBase
    {
        #region Constructors

        internal Insert(string table, Database2 database) : base(table, database)
        {
        }

        #endregion

        #region Methods

        public void Value(string key, object value)
        {
            Object(key, value);
        }

        public string Key(string key)
        {
            string id = Database2.ID();
            Value(key, id);

            return id;
        }

        public override string ToString()
        {
            if (objects.Count == 0) return "";

            StringBuilder sb = new StringBuilder();
            sb.Append("insert into ");
            sb.Append(table);
            sb.Append("(");
            int counter = 0;
            foreach (KeyValuePair<string, object> pair in objects)
            {
                sb.Append(pair.Key);
                if (counter < objects.Count - 1) sb.Append(", ");
                counter++;
            }
            sb.Append(") values(");
            counter = 0;
            foreach (KeyValuePair<string, object> pair in objects)
            {
                sb.Append(database.Format("{0}", pair.Value));
                if (counter < objects.Count - 1) sb.Append(", ");
                counter++;
            }
            sb.Append(")");

            return sb.ToString();
        }

        public Result2 Execute()
        {
            return database.Execute(this);
        }

        #endregion
    }

    public class Update : DMLBase
    {
        #region Constructors

        internal Update(string table, Database2 database) : base(table, database)
        {
        }

        #endregion

        #region Methods

        public void Set(string key, object value)
        {
            Object(key, value);
        }

        public void Set(string key, object value, bool condition)
        {
            if (condition) Set(key, value);
        }

        public void Where(string key, object value)
        {
            where = database.Format(key + " = {0}", value);
        }

        public override string ToString()
        {
            if (objects.Count == 0) return "";

            StringBuilder sb = new StringBuilder();
            sb.Append("update ");
            sb.Append(table);
            sb.Append(" set ");
            int counter = 0;
            foreach (KeyValuePair<string, object> pair in objects)
            {
                sb.Append(pair.Key);
                sb.Append(" = ");
                sb.Append(database.Format("{0}", pair.Value));
                if (counter < objects.Count - 1) sb.Append(", ");
                counter++;
            }

            if (where != null)
            {
                sb.Append(" where ");
                sb.Append(where);
            }

            return sb.ToString();
        }

        public Result2 Execute()
        {
            return database.Execute(this);
        }


        #endregion
    }

    public class Where
    {
        #region Fields

        private string value = null;

        public string Value
        {
            get { return value; }
            set { this.value = value; }
        }

        #endregion

        #region Constructors

        public Where(string value)
        {
            this.value = value;
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            return value;
        }

        public string Format(string before, string after)
        {
            if (value != null && value.Length > 0)
            {
                return before + value + after;
            }
            else return "";
        }

        public string Format(string before)
        {
            return Format(before, "");
        }

        public string Format()
        {
            return Format("", "");
        }

        #endregion
    }
}
