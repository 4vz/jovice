using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Aphysoft.Share
{
    [Serializable]
    public class InstallationFile
    {
        #region Fields

        public int ID { get; }

        public string Name { get; }

        public long Size { get; }

        public string Hash { get; }

        public DateTime TimeStamp { get; }

        #endregion

        #region Constructors

        public InstallationFile(string name, long size, string hash, DateTime timeStamp)
        {
            ID = Rnd.Int();
            Name = name;
            Size = size;
            Hash = hash;
            TimeStamp = timeStamp;
        }

        #endregion
    }
}
