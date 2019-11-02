using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aphysoft.Share
{
    [Serializable]
    internal class RegisterMessage : BaseClientMessage
    {
        #region Fields

        public string[] Registers { get; set; }

        public bool Force { get; set; } = false;

        #endregion

        #region Constructors

        public RegisterMessage(string clientID, string[] registers) : base(clientID, null)
        {
            this.Registers = registers;
        }

        #endregion
    }
}
