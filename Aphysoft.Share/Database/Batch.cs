
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aphysoft.Share
{
    public class Batch
    {
        #region Fields

        private List<string> lines = new List<string>();

        private Database2 database = null;

        public int Count
        {
            get { return lines.Count; }
        }

        #endregion

        #region Constructors

        internal Batch(Database2 database)
        {
            this.database = database;
        }

        #endregion

        #region Methods

        public void Begin()
        {
            lines.Clear();
        }

        public void Add(string sql, params object[] args)
        {
            if (!string.IsNullOrEmpty(sql))
            {
                string line = database.Format(sql, args);
                lines.Add(line);
            }
        }

        public void Add(Insert insert)
        {
            if (!insert.IsEmpty)
                lines.Add(insert.ToString());
        }

        public void Add(Update update)
        {
            if (!update.IsEmpty)
                lines.Add(update.ToString());
        }

        public Result2 Commit()
        {
            int count = lines.Count;
            Result2 result = new Result2(null);

            Stopwatch elapsed = new Stopwatch();
            elapsed.Restart();

            if (count > 0)
            {
                int index = 0;
                StringBuilder batch = new StringBuilder();

                bool ok = true;
                while (index < count)
                {
                    string line = lines[index];
                    batch.Append(line + ";\r\n");
                    index++;

                    if (index % 25 == 0)
                    {
                        Result2 currentResult = database.Execute(batch.ToString());
                        if (!currentResult)
                        {
                            result.IsExceptionThrown = true;
                            ok = false;
                            break;
                        }
                        else
                        {
                            batch.Clear();
                            result.AffectedRows = result.AffectedRows + currentResult.AffectedRows;
                        }
                    }
                }

                if (ok && batch.Length > 0)
                {
                    Result2 currentResult = database.Execute(batch.ToString());
                    if (!currentResult)
                        result.IsExceptionThrown = true;
                    else
                        result.AffectedRows = result.AffectedRows + currentResult.AffectedRows;
                }
            }

            result.ExecutionTime = elapsed.Elapsed;

            return result;
        }

        #endregion
    }
}
