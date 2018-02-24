
namespace Aphysoft.Share
{
    public static class BaseResources
    {
        private static bool inited = false;

        public static void Init()
        {
            if (!inited)
            {
                inited = true;

#if !DEBUG
                Resource.Register("css_share", ResourceTypes.CSS, Resources.Resources.Css.ResourceManager, "share");
                Resource.Group(Resource.CommonResourceCSS, "css_share", 1);
#endif

                //
                // scripts
                //
                Resource.Register("script_jquery", "jquery", ResourceTypes.JavaScript, Resources.Resources.Scripts.ResourceManager, "jquery").NoMinify();
                Resource.Group(Resource.CommonResourceScript, "script_jquery", 1);

                Resource.Register("script_modernizr", ResourceTypes.JavaScript, Resources.Resources.Scripts.ResourceManager, "modernizr").NoMinify();
                Resource.Group(Resource.CommonResourceScript, "script_modernizr", 2);

                Resource.Register("script_libs", ResourceTypes.JavaScript, Resources.Resources.Scripts.ResourceManager, "libs");
                Resource.Group(Resource.CommonResourceScript, "script_libs", 3);

                if (Settings.THREE)
                {
                    Resource.Register("script_three", ResourceTypes.JavaScript, Resources.Resources.Scripts.ResourceManager, "three").NoMinify();
                    Resource.Group(Resource.CommonResourceScript, "script_three", 4);
                }

                if (Settings.Raphael)
                {
                    Resource.Register("script_raphael", ResourceTypes.JavaScript, Resources.Resources.Scripts.ResourceManager, "raphael").NoMinify();
                    Resource.Group(Resource.CommonResourceScript, "script_raphael", 5);
                }

                if (Settings.Fabric)
                {
                    Resource.Register("script_fabric", ResourceTypes.JavaScript, Resources.Resources.Scripts.ResourceManager, "fabric").NoMinify();
                    Resource.Group(Resource.CommonResourceScript, "script_fabric", 6);
                }

                Resource.Register("script_webfont", ResourceTypes.JavaScript, Resources.Resources.Scripts.ResourceManager, "webfont").NoMinify();
                Resource.Group(Resource.CommonResourceScript, "script_webfont", 7);

#if !DEBUG
                Resource.Register("script_share", ResourceTypes.JavaScript, Resources.Resources.Scripts.ResourceManager, "share");
                Resource.Group(Resource.CommonResourceScript, "script_share", 10);
#endif


                // Images
                Resource.Register("image_shortcuticon", ResourceTypes.PNG, Resources.Resources.Images.ResourceManager, "shortcuticon");

                //WebFont.Register("roboto", "https://fonts.googleapis.com/css?family=Roboto:100,100i,300,300i,400,400i,500,500i,700,700i,900,900i");
            }
        }

#if DEBUG
        public static void Debug()
        {
            Resource.Common(Resource.Register("script_debug", ResourceTypes.JavaScript, Resources.Resources.Scripts.ResourceManager, "debug"));
        }
#endif
    }
}
