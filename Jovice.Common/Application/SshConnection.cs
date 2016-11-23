using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using Tamir.SharpSsh;
using System.Text.RegularExpressions;
using System.Net.Sockets;
using System.Text;
using System.IO;

namespace Jovice
{
    internal delegate void SshConnectionEventHandler(object sender);

    internal delegate void SshConnectionFailedEventHandler(object sender, Exception exception);

    internal delegate void SshConnectionReceivedEventHandler(object sender, string message);

    internal abstract class SshConnection
    {
        #region Fields

        private Thread listenerThread;

        private SshShell shell;

        private Queue<string> lastOutputs = new Queue<string>();

        private string lastOutput;

        public string LastOutput
        {
            get { return lastOutput; }
        }

        private bool connected = false;

        public bool IsConnected
        {
            get { return connected; }
        }

        private string expect = string.Empty;

        private Regex expectRegex = null;

        #endregion

        #region Events

        public event SshConnectionEventHandler Connected;

        public event SshConnectionEventHandler Disconnected;

        public event SshConnectionFailedEventHandler ConnectionFailed;

        public event SshConnectionReceivedEventHandler Received;

        #endregion

        #region Methods

        protected void Start(string host, string user, string pass)
        {
            if (!connected)
            {
                shell = new SshShell(host, user, pass);
                shell.RemoveTerminalEmulationCharacters = true;

                try
                {
                    shell.Connect();
                    connected = true;

                    Connected?.Invoke(this);

                    lastOutputs.Clear();
                    lastOutput = null;

                    listenerThread = new Thread(new ThreadStart(delegate() {
                        while (true)
                        {
                            if (shell.Connected)
                            {
                                string output = null;
                                bool sendOutput = false;

                                try
                                {
                                    if (string.IsNullOrEmpty(expect) && expectRegex == null)
                                    {
                                        output = shell.Expect();
                                        sendOutput = true;
                                    }
                                    else if (expectRegex != null)
                                    {
                                        output = shell.Expect(expectRegex);
                                        sendOutput = true;
                                    }
                                    else
                                    {
                                        output = shell.Expect(expect);
                                        sendOutput = true;
                                    }
                                }
                                catch (IOException ex)
                                {
                                    if (ex.Message == "Pipe closed") ;
                                    else if (ex.Message == "Pipe broken") ;
                                    sendOutput = false;
                                    break;
                                }

                                if (sendOutput && output != null)
                                {
                                    lastOutputs.Enqueue(output);
                                    if (lastOutputs.Count > 100) lastOutputs.Dequeue();
                                    StringBuilder lastOutputSB = new StringBuilder();
                                    foreach (string s in lastOutputs)
                                        lastOutputSB.Append(s);

                                    lastOutput = lastOutputSB.ToString();

                                    Received?.Invoke(this, output);
                                }
                            }
                            else break;
                        }

                        OnDisconnected();
                    }));
                    listenerThread.IsBackground = false;
                    listenerThread.Start();
                }
                catch (Exception ex)
                {
                    shell.Close();
                    shell = null;

                    ConnectionFailed?.Invoke(this, ex);
                }
            }
        }

        protected void Stop()
        {
            if (connected)
            {
                listenerThread.Abort();
                OnDisconnected();
            }
        }

        private void OnDisconnected()
        {
            if (shell != null) shell.Close();
            shell = null;

            connected = false;

            Disconnected?.Invoke(this);
        }

        protected void Expect(string expect)
        {
            this.expect = expect;
            this.expectRegex = null;
        }

        protected void Expect(Regex expect)
        {
            this.expectRegex = expect;
            this.expect = string.Empty;
        }

        protected bool WriteLine(string data)
        {
            try
            {
                if (shell != null && shell.Connected)
                {
                    shell.WriteLine(data);
                    return true;
                }
                else return false;
            }
            catch { return false; }
        }

        protected bool Write(string data)
        {
            try
            {
                if (shell != null && shell.Connected)
                {
                    shell.Write(data);
                    return true;
                }
                else return false;
            }
            catch { return false; }
        }
        
        #endregion
    }
}