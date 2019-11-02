using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Aphysoft.Share
{
    public class StreamClientInstance
    {
        #region Fields

        public string ClientID { get; } = null;

        private List<string> registers = new List<string>();

        public object registersWaitSync = new object();

        public ManualResetEvent resetEvent = new ManualResetEvent(false);

        public object dataQueueWaitSync = new object();

        public StreamSessionInstance SessionInstance { get; set; } = null;

        public Queue<StreamInstanceData> DataQueue { get; } = new Queue<StreamInstanceData>();

        public int SessionIndex { get; set; }

        #endregion

        #region Constructors

        public StreamClientInstance(string clientID)
        {
            ClientID = clientID;
        }

        #endregion

        #region Methods

        public void SetAction(string type, object data)
        {
            lock (dataQueueWaitSync)
            {
                DataQueue.Enqueue(new StreamInstanceData(type, data));
                resetEvent.Set();
            }
        }

        public void Register(string register)
        {
            lock (registersWaitSync)
            {
                if (!registers.Contains(register))
                    registers.Add(register);
            }
        }

        public void RemoveRegister(string register)
        {
            lock (registersWaitSync)
            {
                if (registers.Contains(register))
                    registers.Remove(register);
            }
        }

        public void RemoveAllRegisters()
        {
            lock (registersWaitSync)
            {
                registers.Clear();
            }
        }

        public bool IsRegistered(string register)
        {
            if (registers.Contains(register)) return true;
            else return false;
        }

        public bool IsRegisteredMatch(string registerPattern, out string[] matchTo)
        {
            bool re = false;
            List<string> matchTos = new List<string>();

            foreach (string s in registers)
            {
                if (Regex.IsMatch(s, "^" + registerPattern + "$"))
                {
                    re = true;
                    matchTos.Add(s);
                }
            }

            matchTo = matchTos.ToArray();

            return re;
        }

        #endregion
    }
}
