using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Aphysoft.Common;

namespace Jovice
{
    internal sealed partial class Probe
    {
        #region Methods

        private bool PeekProcess()
        {
            #region CPU / Memory

            Event("Checking CPU / Memory"); //mengecek memory dan CPU 

            int cpu = -1;
            int mtotal = -1;
            int mused = -1;

            #region Live

            if (nodeManufacture == cso)
            {
                #region cso

                if (Send("show processes cpu | in CPU")) { NodeSaveMainLoopRestart(); return true; }
                bool timeout;
                List<string> lines = NodeRead(out timeout);
                if (requestFailure) { requestFailure = false; MainLoopRestart(); return true; }
                if (timeout) { NodeReadTimeOutExit(); return true; }

                foreach (string line in lines)
                {
                    if (line.StartsWith("CPU "))
                    {
                        int oid = line.Trim().LastIndexOf(' ');
                        if (oid > -1)
                        {
                            string okx = line.Substring(oid + 1).Trim();
                            string perc = okx.Substring(0, okx.IndexOf('%'));
                            if (!int.TryParse(perc, out cpu)) cpu = -1;
                        }
                    }
                }

                if (nodeVersion == xr)
                {
                    //show memory summary | in Physical Memory
                    if (Send("show memory summary | in Physical Memory")) { NodeSaveMainLoopRestart(); return true; }
                    lines = NodeRead(out timeout);
                    if (requestFailure) { requestFailure = false; MainLoopRestart(); return true; }
                    if (timeout) { NodeReadTimeOutExit(); return true; }

                    foreach (string line in lines)
                    {
                        string lint = line.Trim();
                        if (lint.StartsWith("Physical Memory: "))
                        {
                            string[] linex = lint.Split(StringSplitTypes.Space, StringSplitOptions.RemoveEmptyEntries);

                            string ltot = linex[2];
                            string lfree = linex[4];

                            ltot = ltot.Substring(0, ltot.Length - 1);
                            lfree = lfree.Substring(1, lfree.Length - 2);

                            int ltots;
                            if (int.TryParse(ltot, out ltots))
                                mtotal = ltots * 1000;
                            else
                                mtotal = -1;

                            if (mtotal > -1)
                            {
                                int lfrees;
                                if (int.TryParse(lfree, out lfrees))
                                    mused = mtotal - (lfrees * 1000);
                                else
                                    mused = -1;
                            }                          

                        }

                    }
                }
                else
                {
                    //show process memory  | in Processor Pool
                    if (Send("show process memory | in Total:")) { NodeSaveMainLoopRestart(); return true; }
                    lines = NodeRead(out timeout);
                    if (requestFailure) { requestFailure = false; MainLoopRestart(); return true; }
                    if (timeout) { NodeReadTimeOutExit(); return true; }

                    foreach (string line in lines)
                    {
                        string lint = line.Trim();
                        if (lint.StartsWith("Processor Pool"))
                        {
                            string[] linex = lint.Split(StringSplitTypes.Space, StringSplitOptions.RemoveEmptyEntries);

                            string ltot = linex[3];
                            string luse = linex[5];

                            double ltots;
                            if (!double.TryParse(ltot, out ltots)) ltots = -1;

                            double luses;
                            if (!double.TryParse(luse, out luses)) luses = -1;

                            if (ltots >= 0) mtotal = (int)Math.Round(ltots / 1000);
                            if (luses >= 0) mused = (int)Math.Round(luses / 1000);
                            break;
                        }
                        else if (lint.StartsWith("Total:"))
                        {
                            string[] linex = lint.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                            string ltot = linex[0].Trim().Split(StringSplitTypes.Space)[1];
                            string luse = linex[1].Trim().Split(StringSplitTypes.Space)[1];

                            double ltots;
                            if (!double.TryParse(ltot, out ltots)) ltots = -1;

                            double luses;
                            if (!double.TryParse(luse, out luses)) luses = -1;

                            if (ltots >= 0) mtotal = (int)Math.Round(ltots / 1000);
                            if (luses >= 0) mused = (int)Math.Round(luses / 1000);
                            break;
                        }
                    }
                }

                #endregion
            }
            else if (nodeManufacture == alu)
            {
                #region alu

                if (Send("show system cpu | match \"Busiest Core\"")) { NodeSaveMainLoopRestart(); return true; }
                bool timeout;
                List<string> lines = NodeRead(out timeout);
                if (requestFailure) { requestFailure = false; MainLoopRestart(); return true; }
                if (timeout) { NodeReadTimeOutExit(); return true; }

                foreach (string line in lines)
                {
                    string lint = line.Trim();

                    if (lint.StartsWith("Busiest Core "))
                    {
                        int oid = lint.LastIndexOf(' ');
                        if (oid > -1)
                        {
                            string okx = lint.Substring(oid + 1).Trim();
                            string perc = okx.Substring(0, okx.IndexOf('%'));

                            float cpuf;
                            if (!float.TryParse(perc, out cpuf)) cpuf = -1;

                            if (cpuf == -1) cpu = -1;
                            else cpu = (int)Math.Round(cpuf);
                        }
                    }
                }

                //show system memory-pools | match bytes

                if (Send("show system memory-pools | match bytes")) { NodeSaveMainLoopRestart(); return true; }
                lines = NodeRead(out timeout);
                if (requestFailure) { requestFailure = false; MainLoopRestart(); return true; }
                if (timeout) { NodeReadTimeOutExit(); return true; }

                foreach (string line in lines)
                {
                    string lint = line.Trim();

                    if (lint.StartsWith("Total In Use") || lint.StartsWith("Available Memory"))
                    {
                        string[] linex = lint.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                        if (linex.Length >= 2)
                        {
                            string ibytes = linex[1].Trim();
                            ibytes = ibytes.Substring(0, ibytes.IndexOf(' '));
                            string[] ibytesx = ibytes.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            string cbytes = string.Join("", ibytesx);

                            double dbytes;
                            if (!double.TryParse(cbytes, out dbytes)) dbytes = -1;

                            if (lint.StartsWith("Total In Use") && dbytes > -1)
                            {
                                mused = (int)Math.Round(dbytes / 1000);
                            }
                            else if (mused > -1 && lint.StartsWith("Available Memory") && dbytes > -1)
                            {
                                mtotal = mused + (int)Math.Round(dbytes / 1000);
                            }
                        }
                    }
                }

                #endregion
            }
            else if (nodeManufacture == hwe)
            {
                #region hwe

                if (Send("display cpu-usage")) { NodeSaveMainLoopRestart(); return true; }
                bool timeout;
                List<string> lines = NodeRead(out timeout);
                if (requestFailure) { requestFailure = false; MainLoopRestart(); return true; }
                if (timeout) { NodeReadTimeOutExit(); return true; }


                foreach (string line in lines)
                {
                    string lint = line.Trim();

                    if (nodeVersion.StartsWith("8"))
                    {
                        //System cpu use rate is : 10%
                        //012345678901234567890123456789
                        if (lint.StartsWith("System cpu use rate is"))
                        {
                            string okx = line.Substring(25).Trim();
                            string perc = okx.Substring(0, okx.IndexOf('%'));
                            if (!int.TryParse(perc, out cpu)) cpu = -1;
                            break;
                        }
                    }
                    else
                    {
                        if (lint.StartsWith("CPU utilization for"))
                        {
                            int oid = lint.LastIndexOf(' ');
                            if (oid > -1)
                            {
                                string okx = line.Substring(oid + 1).Trim();
                                string perc = okx.Substring(0, okx.IndexOf('%'));
                                if (!int.TryParse(perc, out cpu)) cpu = -1;
                            }
                            break;
                        }
                    }
                }


                if (Send("display memory-usage")) { NodeSaveMainLoopRestart(); return true; }
                lines = NodeRead(out timeout);
                if (requestFailure) { requestFailure = false; MainLoopRestart(); return true; }
                if (timeout) { NodeReadTimeOutExit(); return true; }

                foreach (string line in lines)
                {
                    string lint = line.Trim();

                    if (lint.StartsWith("System Total") || lint.StartsWith("Total Memory"))
                    {
                        string[] linex = lint.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

                        if (linex.Length >= 2)
                        {
                            string odata = linex[1].Trim();
                            odata = odata.Substring(0, odata.IndexOf(' '));

                            double dbytes;
                            if (!double.TryParse(odata, out dbytes)) dbytes = -1;

                            if (lint.StartsWith("System Total"))
                            {
                                mtotal = (int)Math.Round(dbytes / 1000);
                            }
                            else if (lint.StartsWith("Total Memory"))
                            {
                                mused = (int)Math.Round(dbytes / 1000);
                            }
                        }
                    }
                }
                #endregion
            }
            else if (nodeManufacture == jun)
            {
                #region jun

                //show chassis routing-engine | match Idle
                if (Send("show chassis routing-engine | match Idle")) { NodeSaveMainLoopRestart(); return true; }
                bool timeout;
                List<string> lines = NodeRead(out timeout);
                if (requestFailure) { requestFailure = false; MainLoopRestart(); return true; }
                if (timeout) { NodeReadTimeOutExit(); return true; }

                foreach (string line in lines)
                {
                    string lint = line.Trim();

                    if (lint.StartsWith("Idle"))
                    {
                        string[] linex = lint.Split(StringSplitTypes.Space, StringSplitOptions.RemoveEmptyEntries);
                        if (linex.Length == 3)
                        {
                            string perc = linex[1];
                            int idlecpu;
                            if (int.TryParse(perc, out idlecpu))
                            {
                                cpu = 100 - idlecpu;
                            }
                            else
                            {
                                cpu = -1;
                            }
                        }
                        break;
                    }
                }

                //show task memory
                if (Send("show task memory")) { NodeSaveMainLoopRestart(); return true; }
                lines = NodeRead(out timeout);
                if (requestFailure) { requestFailure = false; MainLoopRestart(); return true; }
                if (timeout) { NodeReadTimeOutExit(); return true; }

                foreach (string line in lines)
                {
                    string lint = line.Trim();

                    if (lint.StartsWith("Currently In Use:") || lint.StartsWith("Available:"))
                    {
                        string[] linex = lint.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

                        if (linex.Length >= 2)
                        {
                            string rightside = linex[1].Trim();

                            linex = rightside.Split(StringSplitTypes.Space, StringSplitOptions.RemoveEmptyEntries);

                            if (linex.Length >= 1)
                            {
                                string odata = linex[0].Trim();
                                int dbytes;
                                if (!int.TryParse(odata, out dbytes)) dbytes = -1;

                                if (lint.StartsWith("Currently In Use:"))
                                {
                                    mused = dbytes;
                                }
                                else if (mused > -1 && lint.StartsWith("Available:"))
                                {
                                    mtotal = mused + dbytes;
                                }
                            }
                        }
                    }
                }

                #endregion
            }

            #endregion

            #region Check

            if (cpu > -1 && cpu < 100)
                Event("CPU = " + cpu + "%");
            else
                Event("CPU load is unavailable (" + cpu + ")");
            if (mtotal > -1)
                Event("Memory total = " + mtotal + "KB");
            else
                Event("Memory total is unavailable");
            if (mused > -1)
                Event("Memory used = " + mused + "KB");
            else
                Event("Memory used is unavailable");

            #endregion

            #region Execute

            List<string> sets = new List<string>();

            if (cpu > -1 && cpu < 100)
                sets.Add(j.Format("NO_CPU = {0}", cpu));
            else
                sets.Add("NO_CPU = NULL");

            if (mtotal > -1)
                sets.Add(j.Format("NO_MTotal = {0}", mtotal));
            else
                sets.Add("NO_MTotal = NULL");
            if (mused > -1)
                sets.Add(j.Format("NO_MUsed = {0}", mused));
            else
                sets.Add("NO_MUsed = NULL");

            sets.Add("NO_PeekTime = GETDATE()");

            j.Query("update Node set " + string.Join(",", sets) + " where NO_ID = {0}", nodeID);

            #endregion

            #endregion            

            return false;
        }

        #endregion
    }
}
