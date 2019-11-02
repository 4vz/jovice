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
using System.Diagnostics;

namespace Aphysoft.Share
{
    public delegate void SshConnectionFailedEventHandler(object sender, Exception exception);

    public delegate void SshConnectionReceivedEventHandler(object sender, string message);

    public delegate void RequestOutputEventHandler(string line);

    public abstract class SshConnection
    {
        #region Fields

        private Thread listenerThread;

        private SshShell shell = null;

        public string ServerVersion { get => shell == null ? null : shell.ServerVersion; }

        public string ClientVersion { get => shell == null ? null : shell.ClientVersion; }

        public bool IsOpened { get => shell == null ? false : shell.ShellOpened; }

        private Queue<string> outputs = new Queue<string>();

        protected int OutputCount { get => outputs.Count; }

        private Queue<string> lastOutputs = new Queue<string>();

        public string LastOutput { get; private set; }

        public bool IsConnected { get; private set; } = false;

        private string expect = string.Empty;

        private Regex expectRegex = null;

        public bool IsStarted { get; private set; } = false;

        private Thread mainLoop = null;
        
        public bool IsRunning { get => mainLoop != null; }

        public string LastSendLine { get; private set; } = null;

        public string LastSend { get; private set; } = null;

        protected string terminalPrefix = null;

        #endregion

        #region Events

        public event SshConnectionFailedEventHandler ConnectionFailed;

        public event SshConnectionReceivedEventHandler Received;

        #endregion

        #region Virtualization

        protected virtual void OnStarting()
        {

        }

        protected virtual void OnConnected()
        {

        }

        protected virtual void OnProcess()
        {
        }

        protected virtual void OnBeforeTerminate()
        {
        }

        protected virtual void OnTerminated()
        {

        }

        protected virtual void OnDisconnected()
        {

        }

        protected virtual void OnSessionFailure()
        {

        }

        protected virtual void OnBeforeStop()
        {

        }

        private void ProcessTerminate()
        {
            if (mainLoop != null)
            {
                if (mainLoop.IsAlive)
                    mainLoop.Abort();
                mainLoop = null;
            }
        }

        protected void SessionFailure()
        {
            OnSessionFailure();

            Stop();

            IsStarted = false;
        }

        #endregion

        #region Methods

        public void Start(string host, string user, string pass)
        {
            if (!IsStarted)
            {
                IsStarted = true;

                new Thread(new ThreadStart(delegate ()
                {
                    OnStarting();

                    if (!IsConnected)
                    {
                        shell = new SshShell(host, user, pass);
                        shell.RemoveTerminalEmulationCharacters = true;                        

                        try
                        {
                            shell.Connect();
                            IsConnected = true;

                            if (mainLoop != null)
                            {
                                if (mainLoop.IsAlive)
                                    mainLoop.Abort();
                                mainLoop = null;
                            }

                            OnConnected();

                            lastOutputs.Clear();
                            LastOutput = null;

                            listenerThread = new Thread(new ThreadStart(delegate ()
                            {
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

                                            LastOutput = lastOutputSB.ToString();

                                            if (output != null && output != "")
                                            {
                                                lock (outputs)
                                                {
                                                    outputs.Enqueue(output);
                                                }
                                            }

                                            Received?.Invoke(this, output);
                                        }
                                    }
                                    else break;
                                }

                                Disconnected();
                            }));
                            listenerThread.IsBackground = false;
                            listenerThread.Start();

                            string t1 = CheckLastLine();

                            Thread.Sleep(500);
                            SendCharacter((char)10);
                            Thread.Sleep(500);

                            string t2 = CheckLastLine();

                            if (t1 == t2)
                            {
                                terminalPrefix = t2;

                                mainLoop = new Thread(new ThreadStart(delegate()
                                {
                                    Culture.Default();

                                    SendLine("bash");
                                    Thread.Sleep(500);
                                    terminalPrefix = CheckLastLine();

                                    OnProcess();
                                }));
                                mainLoop.Start();
                            }
                        }
                        catch (Exception ex)
                        {
                            shell.Close();
                            shell = null;

                            ConnectionFailed?.Invoke(this, ex);

                            IsStarted = false;
                        }
                    }

                })).Start();
            }
        }

        private string CheckLastLine()
        {
            string checkLastLine = null;
            string lastLine = null;
            while (true)
            {
                while (OutputCount > 0) GetOutput();

                if (LastOutput != null)
                {
                    string[] lines = LastOutput.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                    if (lines.Length > 1)
                    {
                        string thisLastLine = lines[lines.Length - 1];
                        if (lastLine == thisLastLine)
                        {
                            checkLastLine = lastLine;
                            break;
                        }
                        else
                        {
                            lastLine = thisLastLine;
                            Thread.Sleep(1000);
                        }
                    }
                }

                Thread.Sleep(100);
            }
            return checkLastLine;
        }

        public void Stop()
        {
            OnBeforeStop();

            if (IsConnected)
            {
                listenerThread.Abort();
                Disconnected();
            }
        }

        private void Disconnected()
        {
            if (shell != null) shell.Close();
            shell = null;

            terminalPrefix = null;

            IsConnected = false;

            OnBeforeTerminate();

            new Thread(new ThreadStart(delegate ()
            {
                do
                {
                    if (IsRunning)
                    {
                        if (!mainLoop.IsAlive)
                        {
                            mainLoop = null;
                            break;
                        }
                    }
                    else break;

                    Thread.Sleep(100); // waiting to be a zombie
                }
                while (true);
                
                IsStarted = false;

                OnTerminated();
            })).Start();

            OnDisconnected();

            if (mainLoop != null)
            {                
                mainLoop.Abort();
            }
        }

        protected string GetOutput()
        {
            string output = null;

            if (OutputCount > 0)
            {
                lock (outputs)
                {
                    if (OutputCount > 0)
                    {
                        output = outputs.Dequeue();
                    }
                }
            }

            return output;
        }

        protected void ClearOutput()
        {
            outputs.Clear();
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

        protected void Send(string data)
        {
            Send(data, false);
        }

        protected void SendLine(string data)
        {
            Send(data, true);
        }

        protected void Send(string data, bool newLine)
        {
            if (IsRunning)
            {
                Thread.Sleep(50);

                bool sent = false;
                try
                {
                    if (shell != null && shell.Connected)
                    {
                        if (newLine)
                        {
                            LastSendLine = data;
                            shell.WriteLine(data);
                        }
                        else
                        {
                            LastSend = data;
                            shell.Write(data);
                        }
                        sent = true;
                    }
                }
                catch (Exception ex)
                {
                }

                if (!sent)
                    SessionFailure();
            }
        }

        protected void SendCharacter(char character)
        {
            Send(character.ToString());
        }

        protected void SendSpace()
        {
            SendCharacter((char)32);
        }

        protected void SendControlRightBracket()
        {
            SendCharacter((char)29);
        }

        protected void SendControlC()
        {
            SendCharacter((char)3);
        }

        protected void SendControlZ()
        {
            SendCharacter((char)26);
        }

        protected void WaitUntilTerminalReady()
        {
            int loop = 0;

            while (true)
            {
                // check output, break when terminal is ready
                int wait = 0;
                bool continueWait = true;
                while (wait < 100)
                {
                    while (OutputCount > 0) GetOutput();

                    if (terminalPrefix == null)
                    {
                        break;
                    }

                    if (LastOutput != null)
                    {
                        if (LastOutput.EndsWith(terminalPrefix))
                        {
                            continueWait = false;
                            break;
                        }
                    }
                    Thread.Sleep(100);
                    wait++;
                }

                if (terminalPrefix == null)
                {
                    SessionFailure();
                    break;
                }

                if (continueWait == false) break;

                // else continue wait...
                loop++;
                if (loop == 3)
                {
                    SessionFailure(); // loop 3 times, its a failure
                }

                // try sending exit characters
                SendCharacter((char)13);
                SendControlRightBracket();
                SendControlC();

                Thread.Sleep(1000);
            }

        }
        
        protected bool WaitUntilEndsWith(string endsWith)
        {
            return WaitUntilEndsWith(new string[] { endsWith });
        }
        
        protected bool WaitUntilEndsWith(string[] endsWith)
        {
            return Request(null, out string[] lines, endsWith);
        }

        protected bool Request(string command, RequestOutputEventHandler lineCallback, VoidEventHandler timeOut)
        {
            return Request(command, out string[] lines, new string[] { terminalPrefix }, lineCallback, timeOut);
        }

        protected bool Request(string command, RequestOutputEventHandler lineCallback)
        {
            return Request(command, lineCallback, delegate () { });
        }

        protected bool Request(string command, out string[] lines)
        {
            return Request(command, out lines, new string[] { terminalPrefix });
        }

        protected bool Request(string command, out string[] lines, string[] endsWith)
        {
            return Request(command, out lines, endsWith, null, null);
        }
        
        protected bool Request(string command, out string[] lines, string[] endsWith, RequestOutputEventHandler lineCallback, VoidEventHandler timeOut)
        {
            bool requestFailed = false;
            lines = null;

            if (command != null)
            {
                ClearOutput();
                SendLine(command);
            }
            else if (endsWith == null)
            {
                return false;
            }

            Stopwatch stopwatch = new Stopwatch();
            StringBuilder lineBuilder = new StringBuilder();
            List<string> listLines = new List<string>();

            bool ending = false;
            int wait = 0;

            stopwatch.Start();

            while (true)
            {
                if (OutputCount > 0)
                {
                    ending = false;

                    wait = 0;
                    string output = GetOutput();

                    for (int i = 0; i < output.Length; i++)
                    {
                        byte b = (byte)output[i];

                        if (b == 8)
                        {
                            if (lineBuilder.Length > 0)
                                lineBuilder.Remove(lineBuilder.Length - 1, 1);
                        }
                        else if (b == 13)
                        {
                            if (lineBuilder.Length > 0 && lineBuilder[lineBuilder.Length - 1] == ' ')
                            {
                                lineBuilder.Remove(lineBuilder.Length - 1, 1);
                            }
                        }
                        else if (b >= 32) lineBuilder.Append((char)b);

                        string line = lineBuilder.ToString();
                        string lineTrim = line.Trim();
                        string endsWithEncountered = null;

                        foreach (string endsWithx in endsWith)
                        {
                            string endsWithxTrim = endsWithx.Trim();

                            if (lineTrim.EndsWith(endsWithxTrim))
                            {
                                endsWithEncountered = endsWithx;
                                break;
                            }
                        }

                        if (endsWithEncountered != null)
                        {
                            int co = line.Length - endsWithEncountered.Length;
                            if (co < 0) co = 0;

                            line = line.Substring(0, co);

                            if (line.Trim().Length > 0)
                            {
                                lineCallback?.Invoke(line);

                                lineBuilder.Clear();
                                listLines.Add(line);
                            }

                            ending = true;
                            break;
                        }
                        else if (b == 10)
                        {
                            lineBuilder.Clear();
                            if (line != command && line.Trim().Length > 0)
                            {
                                listLines.Add(line);
                                lineCallback?.Invoke(line);
                            }
                        }
                        else if (b == 9) lineBuilder.Append(' ');
                    }
                }
                else
                {
                    if (ending)
                        break;

                    wait++;
                    if (wait % 200 == 0 && wait < 1600)
                    {
                        SendLine("");
                    }
                    Thread.Sleep(100);
                    if (wait == 1600)
                    {
                        Console.WriteLine("Reading timeout, cancel the reading...");

                        timeOut?.Invoke();

                        requestFailed = true;
                    }
                    else if (wait >= 1600 && wait % 50 == 0)
                    {
                        SendControlC();
                    }
                    else if (wait == 2000)
                    {
                    }
                }
            }

            stopwatch.Stop();

            if (!requestFailed)
            {
                lines = listLines.ToArray();
                return false;
            }
            else
                return true;

        }
                
        #endregion
    }

    public static class SFTP
    {
        public static bool Put(string host, string user, string pass, string[] localFiles, string remoteDirectory)
        {
            bool r = false;
            try
            {

                Sftp sftp = new Sftp(host, user, pass);
                sftp.Connect();

                sftp.Put(localFiles, remoteDirectory);

                sftp.Close();

                r = true;
            }
            catch (Exception ex)
            {

            }

            return r;
        }

        public static bool Get(string host, string user, string pass, string remoteFile, string localFile)
        {
            bool r = false;
            try
            {
                Sftp sftp = new Sftp(host, user, pass);
                sftp.Connect();

                sftp.Get(remoteFile, localFile);

                sftp.Close();

                r = true;
            }
            catch (Exception ex)
            {

            }

            return r;
        }

    }
}