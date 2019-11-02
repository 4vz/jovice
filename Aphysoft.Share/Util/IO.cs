using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aphysoft.Share
{ 
    public static class IO
    {
        public delegate void FileStreamEventHandler(FileStream stream);

        public delegate void FileStreamLinesEventHandler(List<string> lines);

        public static bool HasWriteAccessToDirectory(string directoryPath)
        {
            try
            {
                // TODO

                System.Security.AccessControl.DirectorySecurity ds = Directory.GetAccessControl(directoryPath);
                return true;

            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }

            
        }

        public static void AllowEveryoneAllAccess(string path)
        {
            if (IsDirectory(path))
            {
                DirectoryInfo dInfo = new DirectoryInfo(path);

                DirectorySecurity dSecurity = dInfo.GetAccessControl();
                dSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
                dInfo.SetAccessControl(dSecurity);

                foreach (DirectoryInfo di in dInfo.GetDirectories())
                {
                    AllowEveryoneAllAccess(di.FullName);
                }
            }
            else
            {
                FileInfo fInfo = new FileInfo(path);

                FileSecurity fSecurity = fInfo.GetAccessControl();
                fSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.None, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
                fInfo.SetAccessControl(fSecurity);
            }
        }

        public static bool IsDirectory(string path)
        {
            FileAttributes attr = File.GetAttributes(path);

            if (attr.HasFlag(FileAttributes.Directory))
                return true;
            else
                return false;
        }

        public static bool IsFile(string path)
        {
            return !IsDirectory(path);
        }

        public static void ExclusiveFileOpen(string path, FileStreamEventHandler handler)
        {
            FileStream stream = null;

            while (stream == null)
            {
                try
                {
                    stream = File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
                }
                catch (IOException ex)
                {
                    Thread.Sleep(100);
                }
            }

            if (stream != null)
            {
                handler(stream);

                stream.Close();
                stream.Dispose();
            }
        }

        public static void ExclusiveFileOpen(string path, FileStreamLinesEventHandler handler)
        {
            ExclusiveFileOpen(path, delegate (FileStream fs)
            {
                byte[] array = new byte[fs.Length];
                fs.Read(array, 0, array.Length);

                int position = 0;

                List<string> lines = new List<string>();
                StringBuilder sb = new StringBuilder();

                int lineEnding = -1;

                // 0: CRLN
                // 1: CR
                // 2: LN

                while (position < array.Length)
                {
                    byte cb = array[position];
                    if (cb == '\r')
                    {
                        if ((position + 1) < array.Length)
                        {
                            byte nb = array[position + 1];
                            if (nb == '\n')
                            {
                                lines.Add(sb.ToString());
                                sb.Clear();

                                position++;

                                lineEnding = 0;
                            }
                            else
                            {
                                lineEnding = 1;
                            }
                        }
                    }
                    else if (cb == '\n')
                    {
                        lines.Add(sb.ToString());
                        sb.Clear();
                        lineEnding = 2;
                    }
                    else
                    {
                        sb.Append(Encoding.UTF8.GetString(cb.ToArray()));
                    }

                    position++;
                }

                if (sb.Length > 0)
                {
                    lines.Add(sb.ToString());
                }

                //List<string> original = new List<string>(lines);

                handler(lines);

                fs.SetLength(0);
                fs.Seek(0, SeekOrigin.Begin);

                string lineEndingString = null;

                if (lineEnding == 0) lineEndingString = "\r\n";
                else if (lineEnding == 1) lineEndingString = "\r";
                else lineEndingString = "\n";

                int lineIndex = 0;
                foreach (string line in lines)
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(line + (lineIndex < lines.Count - 1 ? lineEndingString : ""));

                    fs.Write(bytes, 0, bytes.Length);
                }
            });
        }

        public static FileInfo[] EnumerateFiles(DirectoryInfo parent)
        {
            List<FileInfo> list = new List<FileInfo>();

            foreach (DirectoryInfo di in parent.GetDirectories())
            {
                list.AddRange(EnumerateFiles(di));
            }

            foreach (FileInfo fi in parent.GetFiles())
            {
                list.Add(fi);
            }

            return list.ToArray();
        }

        public static FileInfo[] EnumerateFiles(string path)
        {
            return EnumerateFiles(new DirectoryInfo(path));
        }

        private static MD5 md5 = null;

        public static string Hash(string path)
        {
            if (md5 == null) md5 = MD5.Create();

            string hashstring = null;
            using (FileStream stream = File.OpenRead(path))
            {
                hashstring = md5.ComputeHash(stream).ToHex();
            }

            return hashstring;
        }

        public static string Hash(FileStream stream)
        {
            if (md5 == null) md5 = MD5.Create();
            if (stream == null) return null;

            return md5.ComputeHash(stream).ToHex();
        }
    }
}
