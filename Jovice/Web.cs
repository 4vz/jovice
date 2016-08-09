using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Aphysoft.Share;
using Aphysoft.Common;

namespace Jovice
{
    public class Web : Jovice
    {
        protected override void OnInit()
        {
            Version = 3;

            base.OnInit(); // access to Jovice.Client
        }

        protected override void OnResourceLoad()
        {
            #region Jovice

            Resource.Common(Resource.Register("jovice", ResourceType.JavaScript, "~/View/jovice.js"));

            #endregion

            #region Views

            Content.Register("main",
                new ContentPage[] {
                    new ContentPage("/")
                },
                new ContentPackage(
                    Resource.Register("main", ResourceType.JavaScript, "~/View/main.js"),
                    null));

            Content.Register("search",
                new ContentPage[] {
                    new ContentPage("/search", true)
                },
                new ContentPackage(
                    Resource.Register("search", ResourceType.JavaScript, "~/View/search.js"),
                    null));

            #region Search

            Resource.Register("search_service", ResourceType.JavaScript, "~/View/Search/service.js");
            Resource.Register("search_node", ResourceType.JavaScript, "~/View/Search/node.js");

            #endregion

            #endregion

            #region Providers

            Provider.Register(101, Providers.Search.ProviderRequest); // Search
            Provider.Register(new int[] {
                50001, // Is Necrow Available
                50005  // Ping
            }, Providers.NecrowClient.ProviderRequest); // Necrow
            Provider.Register(new int[] {
                5001, // Main Statistics
            }, Providers.Statistics.ProviderRequest);

            //Provider.Register("probe");


            #endregion
        }

        protected override void OnScriptDataBinding(HttpContext context, ScriptData data)
        {
        }
     }
}