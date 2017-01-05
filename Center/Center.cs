using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Aphysoft.Share;


namespace Center
{
    public class Center : Share
    {
        #region Database

        private static Database center = null;

        public static new Database Database
        {
            get
            {
                if (center == null)
                {
                    string database = ConfigurationHelper.Settings("database");
                    string connectionString = string.Format("Data Source={0};Initial Catalog=center;User ID=telkom.center;Password=t3lk0mdotc3nt3r;async=true", database);
                    center = new Database(connectionString, DatabaseType.SqlServer);
                }
                return center;
            }
        }

        #endregion

        protected override void OnInit()
        {
            Client.Init();
        }

        protected override void OnResourceLoad()
        {
            BaseResources.Init();

            #region Center

            Resource.Common(Resource.Register("center", ResourceType.JavaScript, Resources.ResourceManager, "center", "~/View/center.js"));

            Content.Register("main",
                new ContentPage[] {
                    new ContentPage("/")
                },
                new ContentPackage(
                    Resource.Register("main", ResourceType.JavaScript, Resources.ResourceManager, "main", "~/View/main.js"),
                    null));

            Content.Register("search",
                new ContentPage[] {
                    new ContentPage("/search", true)
                },
                new ContentPackage(
                    Resource.Register("search", ResourceType.JavaScript, Resources.ResourceManager, "search", "~/View/search.js"),
                    null));

            #endregion

            #region Jovice

            Resource.Register("search_jovice_service", ResourceType.JavaScript, Resources.ResourceManager, "jovice_search_service", "~/View/Jovice/Search/service.js");
            Content.Register("jovice_service",
                new ContentPage[] {
                    new ContentPage("/network/service", true)
                },
                new ContentPackage(
                    Resource.Register("jovice_service", ResourceType.JavaScript, Resources.ResourceManager, "jovice_service", "~/View/Jovice/service.js"),
                    null));

            #endregion

            Provider.Register(101, Providers.Search.ProviderRequest); // Search
        }

        protected override void OnScriptDataBinding(HttpContext context, ScriptData data)
        {
        }
    }

    //public class Web
    //{
    //    protected override void OnInit()
    //    {
    //        Version = 3;

    //        base.OnInit(); // access to Jovice.Client
    //    }

    //    protected override void OnResourceLoad()
    //    {
    //        #region Jovice

    //        //

    //        #endregion

    //        #region Views

    //        /*Content.Register("main",
    //            new ContentPage[] {
    //                new ContentPage("/")
    //            },
    //            new ContentPackage(
    //                Resource.Register("main", ResourceType.JavaScript, "~/View/main.js"),
    //                null));

    //        Content.Register("search",
    //            new ContentPage[] {
    //                new ContentPage("/search", true)
    //            },
    //            new ContentPackage(
    //                Resource.Register("search", ResourceType.JavaScript, "~/View/search.js"),
    //                null));

    //        Content.Register("service",
    //            new ContentPage[] {
    //                new ContentPage("/service", true)
    //            },
    //            new ContentPackage(
    //                Resource.Register("service", ResourceType.JavaScript, "~/View/service.js"),
    //                null));*/

    //        #region Search

    //        //Resource.Register("search_service", ResourceType.JavaScript, "~/View/Search/service.js");
    //        //Resource.Register("search_node", ResourceType.JavaScript, "~/View/Search/node.js");


    //        //Resource.Register("res", Resources.ResourceManager, )

    //        #endregion

    //        #endregion

    //        #region Providers


    //        //Provider.Register(new int[] {
    //            50001, // Is Necrow Available
    //            50005  // Ping
    //        //}, Providers.NecrowClient.ProviderRequest); // Necrow
    //        //Provider.Register(new int[] {
    //        //    5001, // Main Statistics
    //        //}, Providers.Statistics.ProviderRequest);

    //        //Provider.Register("probe");


    //        #endregion
    //    }

    // }
}