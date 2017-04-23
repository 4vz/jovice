
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

using Aphysoft.Share;

namespace Center.Providers
{
    public class InterfaceSearchMatch : SearchMatch
    {
        #region Constructors

        public InterfaceSearchMatch()
        {
            Root("interfaces");
            Root("interface");

            Language("descriptions", "DESCRIPTION");
            Language("description", "DESCRIPTION");
            Language("desc", "DESCRIPTION");
            Language("des", "DESCRIPTION");

            Language("pe", "PE");
            Language("me", "ME");
            Language("metro", "ME");

            Language(delegate ()
            {
                return jovice.QueryList("select NO_Name from Node where NO_Active = 1", "NO_Name").ToArray();
            }, "NODENAME");
        }

        #endregion

        #region Methods

        public override void Process(SearchMatchResult matchResult, SearchMatchQuery matchQuery)
        {
            matchResult.Type = "jovice_interface";

            Where whereInterface = SearchDescriptor.Build(matchQuery.Descriptors, delegate (SearchDescriptor descriptor)
            {
                SearchConstraints c = descriptor.Constraint;

                return descriptor.Build(delegate (int index, string value)
                {
                    string v = jovice.Escape(value);

                    if (descriptor.Descriptor == "NODENAME")
                    {
                        if (c == SearchConstraints.StartsWith) return "NO_Name like '" + v + "%'";
                        else if (c == SearchConstraints.EndsWith) return "NO_Name like '%" + v + "'";
                        else if (c == SearchConstraints.Like) return "NO_Name like '%" + v + "%'";
                        else if (c == SearchConstraints.Equal) return "NO_Name like '" + v + "'";
                    }
                    else if (descriptor.Descriptor == "PE") return "NO_Type = 'P'";
                    else if (descriptor.Descriptor == "ME") return "NO_Type = 'M'";
                    else if (descriptor.Descriptor == "DESCRIPTION")
                    {
                        if (c == SearchConstraints.StartsWith) return "I_Desc like '" + v + "%'";
                        else if (c == SearchConstraints.EndsWith) return "I_Desc like '%" + v + "'";
                        else if (c == SearchConstraints.Like || c == SearchConstraints.Equal) return "I_Desc like '%" + v + "%'";
                    }

                    return null;
                });
            });

            matchResult.QueryCount = @"
select * from (
select PI_NO as I_Node, PI_Description as I_Desc from PEInterface
union all
select MI_NO as I_Node, MI_Description as I_Desc from MEInterface
) a, Node
where a.I_Node = NO_ID and NO_Active = 1" + whereInterface.Format(" and ");

            matchResult.Query = @"
select I_ID, NO_Name, I_Name, I_Type, I_Desc, NO_Manufacture, NO_Type from (
select PI_NO as I_Node, PI_ID as I_ID, PI_Description as I_Desc, PI_Name as I_Name, PI_Type as I_Type from PEInterface
union all
select MI_NO as I_Node, MI_ID as I_ID, MI_Description as I_Desc, MI_Name as I_Name, MI_Type as I_Type from MEInterface
) a, Node
where a.I_Node = NO_ID and NO_Active = 1" + whereInterface.Format(" and ");

            matchResult.RowID = "I_ID";

            matchResult.Hide("I_ID");

            matchResult.Sort("NO_Name", "Node");
            matchResult.Sort("I_Name", "Interface");
            matchResult.Sort("I_Desc", "Description");
        }

        public override void RowProcess(SearchMatchResult matchResult, List<object> objects)
        {
            string noType = (string)objects[8];
            string id = (string)objects[2];
        }

        #endregion
    }
}