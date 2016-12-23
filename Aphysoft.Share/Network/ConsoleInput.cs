using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aphysoft.Share
{
    public class ConsoleInput
    {
        #region Fields

        private List<string> clauses;

        public List<string> Clauses
        {
            get { return clauses; }
        }

        #endregion

        #region Constructors

        public ConsoleInput(string line)
        {
            clauses = new List<string>();

            if (line.Length > 0)
                clauses.AddRange(line.Trim().Split(new char[] { ' ' }));
        }

        public bool IsCommand(string command)
        {
            if (clauses.Count > 0) return command.ToLower() == clauses[0].ToLower();
            else return false;
        }

        public string ClausesFrom(int index)
        {
            return string.Join(" ", clauses.GetRange(index, clauses.Count - index));
        }

        #endregion
    }

    public delegate void ConsoleInputEventHandler(ConsoleInput input);
}
