using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Aphysoft.Share;

namespace Center
{
    public class Center : Share
    {
        protected override void OnInit()
        {
            Client.Init();
        }

        protected override void OnResourceLoad()
        {
            BaseResources.Init();

            #region Development
#if DEBUG
            Resource.Common(Resource.Register("css_ui", ResourceTypes.CSS, "../Aphysoft.Share.Resources/Resources/Css/share.css"));
            Resource.Common(Resource.Register("script_share", ResourceTypes.JavaScript, "../Aphysoft.Share.Resources/Resources/Scripts/share.js").NoMinify().NoCache());

            BaseResources.Debug();
#endif
            #endregion

            #region Center

            Resource.Common(Resource.Register("center", ResourceTypes.JavaScript, Resources.ResourceManager, "center", "~/View/center.js"));

            Content.Register("main", Resources.ResourceManager, "main", "~/View/main.js", "/");
            Content.Register("search", Resources.ResourceManager, "search", "~/View/search.js", "/search:true");

            #endregion

            #region User

            #endregion

            #region Jovice

            Resource.Register("search_jovice_service", ResourceTypes.JavaScript, Resources.ResourceManager, "jovice_search_service", "~/View/Jovice/Search/service.js");
            Resource.Register("search_jovice_interface", ResourceTypes.JavaScript, Resources.ResourceManager, "jovice_search_interface", "~/View/Jovice/Search/interface.js");

            Content.Register("jovice_service", Resources.ResourceManager, "jovice_service", "~/View/Jovice/service.js", "/network/service:true");
            Content.Register("jovice_network", Resources.ResourceManager, "jovice_network", "~/View/Jovice/network.js", "/network:true");

            #endregion

            #region Search

            Provider.Register(15001, Providers.Network.ProviderRequest); // network map
            Provider.Register(101, Providers.Search.ProviderRequest); // Search

            #endregion

            #region Developers

            Content.Register("developers", DevelopersResources.ResourceManager, "developers", "~/View/Developers/developers.js", "/developers::Center Developers");

            #endregion
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