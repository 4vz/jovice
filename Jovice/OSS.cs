using Aphysoft.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Aveezo;

namespace Jovice
{
    public class OSS
    {
        public static Database Database => Database.Get("NOSSF");

        public static bool RefreshOrder(string sid, string seid, bool newService = false)
        {
            if (sid == null || seid == null) return false;

            Database jovice = Center.Jovice.Database;
            Database oss = Database;

            bool ok = false;

            Result nsores = oss.Query(@"
select ORDER_ID, AM, CUSTACCNTNAME, CUSTACCNTNUM, SERVACCNTNAME, ACTION_CD, ORDER_STATUS, ORDER_CREATED_DATE, LI_PRODUCT_NAME 
from DWH_SALES.eas_ncrm_agree_order_line where li_sid = {0}
order by ORDER_CREATED_DATE asc
", sid);

            if (nsores)
            {
                if (nsores.Count > 0)
                {
                    string scid = null;

                    Row lastRow = nsores.Last();

                    string customerName = null;
                    string customerAlternateName = null;
                    string customerAccountNumber = null;
                    string serviceDetail = null;
                    string serviceProduct = null;

                    // ServiceCustomer
                    (customerName, customerAlternateName) = DetermineCustomerName(lastRow["CUSTACCNTNAME"]);

                    serviceDetail = lastRow["SERVACCNTNAME"];
                    customerAccountNumber = lastRow["CUSTACCNTNUM"];
                    serviceProduct = lastRow["LI_PRODUCT_NAME"];

                    if (jovice.Query(out Row curow, "select * from ServiceCustomer where SC_AccountNumber = {0}", customerAccountNumber))
                    {
                        string storedName = curow["SC_Name"];
                        string storedAlternateName = curow["SC_AlternateName"];

                        if (customerName != storedName || customerAlternateName != storedAlternateName)
                        {
                            Update cu = jovice.Update("ServiceCustomer");
                            cu.Set("SC_Name", customerName, customerName != null);
                            cu.Set("SC_AlternateName", customerAlternateName);
                            cu.Where("SC_ID", curow["SC_ID"]);
                            cu.Execute();
                        }

                        scid = curow["SC_ID"];
                    }
                    else
                    {
                        if (customerName == null)
                        {
                            customerName = "-";
                        }

                        Insert ci = jovice.Insert("ServiceCustomer");
                        scid = ci.Key("SC_ID");
                        ci.Value("SC_Name", customerName);
                        if (customerAlternateName != null)
                        {
                            ci.Value("SC_AlternateName", customerAlternateName);
                        }
                        ci.Value("SC_AccountNumber", customerAccountNumber);
                        ci.Execute();
                    }

                    string spId = null;

                    if (serviceProduct != null)
                    {
                        if (jovice.Query(out Row sprow, "select * from ServiceProduct where SP_Product = {0}", serviceProduct))
                        {
                            spId = sprow["SP_ID"];
                        }
                        else
                        {
                            spId = Database.ID();

                            Insert spi = jovice.Insert("ServiceProduct");
                            spi.Value("SP_ID", spId);
                            spi.Value("SP_Product", serviceProduct);
                            spi.Execute();
                        }
                    }

                    // Service
                    if (newService)
                    {
                        Insert sei = jovice.Insert("Service");
                        sei.Value("SE_ID", seid);
                        sei.Value("SE_SC", scid);
                        sei.Value("SE_SID", sid);
                        sei.Value("SE_SP", spId);
                        sei.Value("SE_Detail", serviceDetail);
                        sei.Value("SE_LastCheck", DateTime.UtcNow);
                        sei.Execute();
                    }
                    else if (jovice.Query(out Row serow, "select * from Service where SE_ID = {0}", seid))
                    {
                        Update seu = jovice.Update("Service");
                        seu.Set("SE_Detail", serviceDetail, serviceDetail != serow["SE_Detail"]);
                        seu.Set("SE_SP", spId, spId != serow["SE_SP"]);
                        seu.Set("SE_LastCheck", DateTime.UtcNow);
                        seu.Where("SE_ID", seid);
                        seu.Execute();
                    }

                    // ServiceOrder
                    Dictionary<string, Row> sod = jovice.QueryDictionary("select * from ServiceOrder where SO_SE = {0}", "SO_OID", seid);

                    Dictionary<string, Row> sox = new Dictionary<string, Row>();

                    foreach (Row nsorow in nsores)
                    {
                        string oid = nsorow["ORDER_ID"];

                        if (sox.ContainsKey(oid))
                        {
                            // replace the old with this
                            sox[oid] = nsorow;
                        }
                        else
                        {
                            sox.Add(oid, nsorow);
                        }
                    }

                    foreach (KeyValuePair<string, Row> soxPair in sox)
                    {
                        string oid = soxPair.Key;
                        Row soxw = soxPair.Value;

                        if (!sod.ContainsKey(oid))
                        {
                            DateTime created = soxw["ORDER_CREATED_DATE"].ToDateTime();

                            string status = "N";
                            switch ((string)soxw["ORDER_STATUS"])
                            {
                                case "Failed":
                                    status = "F";
                                    break;
                                case "Abandoned":
                                    status = "A";
                                    break;
                                case "In Progress":
                                    status = "I";
                                    break;
                                case "Cancelled":
                                    status = "L";
                                    break;
                                case "Pending Cancel":
                                    status = "G";
                                    break;
                                case "Submitted":
                                    status = "S";
                                    break;
                                case "Pending":
                                    status = "P";
                                    break;
                                case "Complete":
                                    status = "C";
                                    break;
                                default:
                                    status = "N";
                                    break;
                            }

                            string action = "N";
                            switch ((string)soxw["ACTION_CD"])
                            {
                                case "Add":
                                    action = "A";
                                    break;
                                case "Delete":
                                    action = "D";
                                    break;
                                case "-":
                                    action = "N";
                                    break;
                                case "Update":
                                    action = "U";
                                    break;
                                case "Suspend":
                                    action = "S";
                                    break;
                                case "Resume":
                                    action = "R";
                                    break;
                                default:
                                    action = "N";
                                    break;
                            }

                            string am = null;
                            string dam = soxw["AM"];

                            if (dam != null)
                            {
                                string[] fs = dam.Split(StringSplitTypes.Pipe);
                                am = fs[0].Length > 0 && fs[0].IsOnlyContainsCharacters(Characters.Numeric) ? fs[0] : null;
                            }

                            Insert soi = jovice.Insert("ServiceOrder");
                            soi.Key("SO_ID");
                            soi.Value("SO_SE", seid);
                            soi.Value("SO_OID", oid);
                            soi.Value("SO_Created", created);
                            soi.Value("SO_Action", action);
                            soi.Value("SO_Status", status);
                            soi.Value("SO_AM", am);
                            soi.Execute();
                        }
                    }

                }

                ok = true;
            }

            return ok;
        }

        private static Regex alternateNameRegex = new Regex(@"\(([a-zA-Z\s\d]+)\)");

        private static (string name, string altname) DetermineCustomerName(string name)
        {
            string un = null;
            string an = null;

            if (name != null)
            {
                StringBuilder nameSB = new StringBuilder(name.ToUpper().NormalizeWhiteSpace());

                nameSB.Remove("(PERSERO)");

                StringBuilder altnameSB = new StringBuilder();

                int id = 0;
                foreach (Match m in alternateNameRegex.Matches(nameSB.ToString()))
                {
                    if (m.Success)
                    {
                        Group g = m.Groups[1];
                        if (id == 0)
                        {
                            altnameSB.Append(g.Value.Trim());
                        }
                        id++;
                        nameSB.ReplaceWithWhiteSpace(g.Index - 1, g.Length + 2);
                    }
                }

                nameSB.RemoveCharactersExcept(Characters.Alphanumeric, Characters.WhiteSpace);
                altnameSB.RemoveCharactersExcept(Characters.Alphanumeric, Characters.WhiteSpace);

                nameSB.Remove("PT ", "CV ", "TBK", " PT", " CV");
                altnameSB.Remove("PT ", "CV ", "TBK", " PT", " CV");

                nameSB.NormalizeWhiteSpace();
                altnameSB.NormalizeWhiteSpace();

                un = nameSB.ToString().Trim();
                string anx = altnameSB.ToString().Trim();

                if (anx.Length > 0) an = anx;

                if (an != null)
                {
                    if (un.Length < an.Length)
                    {
                        string t = un;
                        un = an;
                        an = t;
                    }
                }
            }

            return (un, an);
        }
    }
}
