using Aphysoft.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Jovice.Providers
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
            Language("in", "LOCATION");
            Language("on", "LOCATION");
            Language("at", "LOCATION");
            Language("located", "LOCATION");
            Language("inside", "LOCATION");
            Language("installed", "LOCATION");
        }

        #endregion

        #region Methods

        public override void Process(SearchMatchResult matchResult, string[] tokens, string[] preTokens, string[] postTokens, string sort, string sortMethod, int page, int pageSize, int pageLength)
        {
            matchResult.Type = "interface";

            #region descriptors

            List<SearchDescriptor> descriptors = Search.ParsePostTokens(postTokens);

            Where piwhere = SearchDescriptor.Build(descriptors, delegate(SearchDescriptor descriptor)
            {
                /*string c = descriptor.Constraint;

                return descriptor.Build(delegate(int index, string value)
                {
                    if (descriptor.Descriptor == "DESCRIPTION")
                    {
                        if (c == "STARTSWITH") return "PI_Description like '" + jovice.Escape(value) + "%'";
                        else if (c == "ENDSWITH") return "PI_Description like '%" + jovice.Escape(value) + "'";
                        else if (c == "LIKE") return "PI_Description like '%" + jovice.Escape(value) + "%'";
                        else if (c == "EQUAL") return "PI_Description like '" + jovice.Escape(value) + "'";
                    }
                    else if (descriptor.Descriptor == "LOCATION")
                    {
                        if (c == "EQUAL")
                        {
                            return "NO_Name = '" + jovice.Escape(value) + "'";
                        }
                    }

                    return null;
                });*/
                return null;
            });
            Where miwhere = SearchDescriptor.Build(descriptors, delegate(SearchDescriptor descriptor)
            {
                /*string c = descriptor.Constraint;

                return descriptor.Build(delegate(int index, string value)
                {
                    if (descriptor.Descriptor == "DESCRIPTION")
                    {
                        if (c == "STARTSWITH") return "MI_Description like '" + jovice.Escape(value) + "%'";
                        else if (c == "ENDSWITH") return "MI_Description like '%" + jovice.Escape(value) + "'";
                        else if (c == "LIKE") return "MI_Description like '%" + jovice.Escape(value) + "%'";
                        else if (c == "EQUAL") return "MI_Description like '" + jovice.Escape(value) + "'";
                    }
                    else if (descriptor.Descriptor == "LOCATION")
                    {
                        if (c == "EQUAL")
                        {
                            return "NO_Name = '" + jovice.Escape(value) + "'";
                        }
                    }

                    return null;
                });*/
                return null;
            });

            #endregion

            matchResult.QueryCount = @"
select PI_ID from PEInterface 
left join Node on PI_NO = NO_ID
" + piwhere.Format(" where ") + @" 
union all 
select MI_ID 
from MEInterface
left join Node on MI_NO = NO_ID
" + miwhere.Format(" where ");

            matchResult.Query = @"
select 
PI_ID as I_ID, PI_Name as I_Name, PI_Description as I_Description, PI_Status as I_Status, PI_Protocol as I_Protocol, NO_Name, NO_Manufacture, NO_Model, NO_Version,
SE_SID
from Node, PEInterface
left join Service on PI_SE = SE_ID and PI_SE_Check = 1
where " + piwhere.Format("", " and ") + @"PI_NO = NO_ID
union all
select 
MI_ID as I_ID, MI_Name as I_Name, MI_Description as I_Description, MI_Status as I_Status, MI_Protocol as I_Protocol, NO_Name, NO_Manufacture, NO_Model, NO_Version,
SE_SID
from Node, MEInterface
left join Service on MI_SE = SE_ID and MI_SE_Check = 1
where " + miwhere.Format("", " and ") + @"MI_NO = NO_ID
";
            matchResult.RowID = "I_ID";

            matchResult.Hide("I_ID");

            matchResult.Sort("NO_Name", "Network Element");
            matchResult.Sort("I_Name", "Interface");
            matchResult.Sort("I_Status", "Status");
            matchResult.Sort("I_Protocol", "Protocol");
        }

        #endregion
    }
}