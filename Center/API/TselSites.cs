using Aphysoft.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Center.APIs
{
    public static class TselSites
    {
        public static APIPacket APIRequest(APIAsyncResult result, string[] paths, string apiAccessID)
        {
            Database j = Jovice.Database;

            TselSitesSearchAPIPacket packet = null;

            int count = paths.Length;

            if (paths[1] == "search")
            {
                // search
                if (count > 3)
                {
                    string searchby = null;
                    if (paths[2].ArgumentIndexOf("siteid", "mac") > -1) searchby = paths[2];
                    else return new ErrorAPIPacket(APIErrors.BadRequestFormat);

                    string searchpattern = paths[3].Replace('*', '%');

                    packet = new TselSitesSearchAPIPacket();
                    packet.SearchType = searchby;
                    packet.SearchPattern = paths[3];

                    if (searchby == "siteid")
                    {
                        Result r = j.Query("select * from MEInterface where MI_Description like {0}", searchpattern);

                        if (r.Count > 0)
                        {
                            packet.SearchPattern = r[0]["MI_Description"].ToString();
                        }
                    }
                }
                else return new ErrorAPIPacket(APIErrors.BadRequestFormat);
            }
            else
            {
                // show
            }


            return packet;
        }
    }



    [DataContract]
    public class TselSitesSearchAPIPacket : ResultAPIPacket
    {
        #region Fields

        private string searchType;

        [DataMember(Name = "searchtype")]
        public string SearchType
        {
            get { return searchType; }
            set { searchType = value; }
        }

        private string searchPattern;

        [DataMember(Name = "searchpattern")]
        public string SearchPattern
        {
            get { return searchPattern; }
            set { searchPattern = value; }
        }
     
        #endregion

        #region Constructors

        public TselSitesSearchAPIPacket()
        {

        }

        #endregion
    }
}