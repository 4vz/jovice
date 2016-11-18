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
    internal abstract class SshConnection
    {
        private bool started = false;

        public bool IsStarted
        {
            get { return started; }
            set { started = value; }
        }

        private Thread listenerThread;
        private SshShell shell;

        private string currentOutput = "";

        public string CurrentOutput
        {
            get { return currentOutput; }
        }

        private Queue<string> lastOutputs = new Queue<string>();
        private string lastOutput;

        private bool haveConnected = false;

        public string LastOutput
        {
            get { return lastOutput; }
        }

        private string expect = string.Empty;

        public string CurrentExpectString
        {
            get { return expect; }
        }

        private Regex expectRegex = null;

        protected void Start(string host, string user, string pass)
        {
            if (!started)
            {
                shell = new SshShell(host, user, pass);
                shell.RemoveTerminalEmulationCharacters = true;

                string cantconnectmessage = null;

                try
                {
                    shell.Connect();
                    started = true;
                }
                catch (Exception ex)
                {
                    cantconnectmessage = ex.Message;
                }

                if (started)
                {
                    lastOutputs.Clear();
                    lastOutput = null;

                    listenerThread = new Thread(new ThreadStart(Response_WaitCallback));
                    listenerThread.IsBackground = false;
                    listenerThread.Start();
                }
                else
                {
                    shell.Close();
                    shell = null;
                    CantConnect(cantconnectmessage);
                }
            }
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

        protected void Expect()
        {
            this.expect = string.Empty;
        }

        protected virtual void CantConnect(string message)
        {
        }

        protected virtual void Connected()
        {
        }

        protected virtual void Disconnected()
        {

        }

        private void Response_WaitCallback()
        {
            //  LOOP OF ETERNITY
            while (true)
            {
                if (shell.Connected)
                {
                    if (!haveConnected)
                    {
                        haveConnected = true;
                        Connected();
                    }

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
                        if (ex.Message == "Pipe closed")
                        {
                            // being kicked
                        }
                        else if (ex.Message == "Pipe broken")
                        {
                            // connection failure
                        }

                        haveConnected = false;
                        sendOutput = false;
                        shell.Close();
                        shell = null;
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
                        currentOutput = output;

                        OnResponse(output);
                    }
                }
                else
                {
                    started = false;
                    try
                    {
                        shell.Close();
                    }
                    catch { }
                    shell = null;
                    haveConnected = false;
                    break;
                }
            }

            Disconnected();
        }

        protected void Stop()
        {
            if (listenerThread != null)
            {
                listenerThread.Abort();
            }

            try
            {
                shell.Close();
            }
            catch { }
            haveConnected = false;
            started = false;

            Disconnected();
        }

        protected bool WriteLine(string data)
        {
            try
            {
                if (shell.Connected)
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
                if (shell.Connected)
                {
                    shell.Write(data);
                    return true;
                }
                else return false;
            }
            catch { return false; }
        }

        public virtual void OnResponse(string output)
        {
        }
    }
}