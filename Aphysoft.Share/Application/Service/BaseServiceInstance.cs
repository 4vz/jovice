using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aphysoft.Share
{
    public class BaseServiceInstance
    {
        #region Fields
                
        private bool outputConsole = false;
        private static Timer connecting;

        protected Thread instanceThread = null;
        private Thread consoleThread = null;

        public bool IsConsole { get => outputConsole; }

        #endregion

        #region Constructors

        public BaseServiceInstance()
        {

        }

        #endregion

        #region Methods

        protected virtual void OnStart()
        {

        }

        protected virtual void OnStop()
        {

        }

        protected virtual void OnEvent(string message)
        {
            Console.WriteLine(message);
        }

        protected virtual void OnServiceConnected()
        {

        }

        protected void EndConnect()
        {
            connecting.Dispose();
        }

        public void Start()
        {
            Start(false);
        }

        public void Start(bool console)
        {            
            instanceThread = new Thread(new ThreadStart(delegate ()
            {
                Culture.Default();                
                Service.Client();

                Service.Connected += delegate (Connection connection)
                {
                    connecting = new Timer(new TimerCallback(delegate (object state)
                    {
                        OnServiceConnected();
                    }), null, 0, 20000);
                };
                
                OnStart();

                Service.End();
            }));
            instanceThread.Start();

            if (console)
            {
                outputConsole = true;

                consoleThread = new Thread(new ThreadStart(delegate ()
                {
                    while (true)
                    {
                        string line = Console.ReadLine();
                        if (!instanceThread.IsAlive) break;

                        ConsoleInput cs = new ConsoleInput(line);

                        if (cs.IsCommand("exit"))
                        {
                            Stop();
                            break;
                        }
                    }
                }));
                consoleThread.Start();
            }

            instanceThread.Join();
            Event("Instance thread end");
            
            if (console)
            {
                Console.WriteLine("Application terminated, press ENTER to end the program");
                consoleThread.Join();
            }

            OnStop();
        }

        public void Stop()
        {
            //OnStop();           
        }

        public void Event(string message)
        {
            Event(message, null);
        }

        public void Event(string message, string label)
        {
            Event(message, label, null);
        }

        public void Event(string message, string label, string subLabel)
        {
            if (label == null)
                OnEvent(DateTime.UtcNow.ToString("HH:mm:ss.fff") + "||" + message);
            else
            {
                if (subLabel == null)
                    OnEvent(DateTime.UtcNow.ToString("HH:mm:ss.fff") + "|" + label + "|" + message);
                else
                    OnEvent(DateTime.UtcNow.ToString("HH:mm:ss.fff") + "|" + label + ">" + subLabel + "|" + message);
            }
        }

        #endregion
    }
}
