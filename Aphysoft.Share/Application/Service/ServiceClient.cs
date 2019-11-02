using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aphysoft.Share
{
    public abstract class OldServiceClient
    {
        #region Fields

        private bool running = false;
                
        private static Timer connecting;
        protected Thread instanceThread = null;
        private Thread consoleThread = null;

        public bool IsConsole { get => (consoleThread != null); }
        public bool IsRunning { get => running; }

        private IPAddress serviceServerAddress = null;
        protected IPAddress ServiceServerAddress { get => serviceServerAddress; set => serviceServerAddress = value; }

        public bool IsServiceClient { get => serviceServerAddress == null; }

        private ConsoleInput consoleInput = null;

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

        protected virtual void OnUpdating()
        {

        }

        protected virtual void OnUpdated()
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
                running = true;

                Culture.Default();

                if (serviceServerAddress != null)
                {
                    OldService.Client(serviceServerAddress, console);

                    OldService.Connected += delegate (Connection connection)
                    {
                        connecting = new Timer(new TimerCallback(delegate (object state)
                        {
                            OnServiceConnected();
                        }), null, 0, 20000);
                    };
                }
                                
                OnStart();

                if (serviceServerAddress != null)
                {
                    OldService.End();
                }
            }));
            instanceThread.Start();

            bool quitViaConsole = false;

            if (console)
            {
                consoleThread = new Thread(new ThreadStart(delegate ()
                {
                    while (true)
                    {
                        string line = Console.ReadLine();
                        if (!instanceThread.IsAlive) break;

                        consoleInput = new ConsoleInput(line);

                        if (consoleInput.IsCommand("exit"))
                        {
                            running = false;
                            quitViaConsole = true;
                            break;
                        }
                    }

                    Event("Console End");
                }));
                consoleThread.Start();
            }

            instanceThread.Join();

            OnStop();

            if (console)
            {
                if (!quitViaConsole) 
                    Console.WriteLine("Application terminated, press ENTER to end the program");
                if (consoleThread.IsAlive)
                    consoleThread.Join();
            }
        }

        public void Stop()
        {
            running = false;
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

        public int ConsoleWait(string[] options, int timeout)
        {
            int ret = -1;
            int c = 0;
            int secincounter = timeout * 100;

            while (true)
            {
                if (consoleInput != null)
                {
                    int ci = -1;
                    foreach (string option in options)
                    {
                        ci++;
                        if (consoleInput.IsCommand(option))
                        {
                            ret = ci;
                            break;
                        }
                    }

                    if (ret > -1)
                    {
                        break;
                    }

                }
                Thread.Sleep(10);
                c++;

                if (c >= secincounter) break;
            }

            return ret;
        }

        #endregion
    }
}
