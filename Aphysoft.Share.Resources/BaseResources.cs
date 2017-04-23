using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

                //
                // css
                //
                if (Settings.EnableUI)
                {
                    Resource.Register("css_ui", ResourceType.CSS, Resources.Resources.Css.ResourceManager, "ui");
                    Resource.Group(Resource.CommonResourceCSS, "css_ui", 1);
                }

                //
                // scripts
                //
                Resource.Register("script_jquery", "jquery", ResourceType.JavaScript, Resources.Resources.Scripts.ResourceManager, "jquery").NoMinify();
                Resource.Group(Resource.CommonResourceScript, "script_jquery", 1);

                Resource.Register("script_modernizr", ResourceType.JavaScript, Resources.Resources.Scripts.ResourceManager, "modernizr").NoMinify();
                Resource.Group(Resource.CommonResourceScript, "script_modernizr", 2);

                Resource.Register("script_libs", ResourceType.JavaScript, Resources.Resources.Scripts.ResourceManager, "libs");
                Resource.Group(Resource.CommonResourceScript, "script_libs", 3);

                if (Settings.THREE)
                {
                    Resource.Register("script_three", ResourceType.JavaScript, Resources.Resources.Scripts.ResourceManager, "three").NoMinify();
                    Resource.Group(Resource.CommonResourceScript, "script_three", 4);
                }

                if (Settings.Raphael)
                {
                    Resource.Register("script_raphael", ResourceType.JavaScript, Resources.Resources.Scripts.ResourceManager, "raphael").NoMinify();
                    Resource.Group(Resource.CommonResourceScript, "script_raphael", 5);
                }

                if (Settings.Fabric)
                {
                    Resource.Register("script_fabric", ResourceType.JavaScript, Resources.Resources.Scripts.ResourceManager, "fabric").NoMinify();
                    Resource.Group(Resource.CommonResourceScript, "script_fabric", 6);
                }

#if !DEBUG
                Resource.Register("script_share", ResourceType.JavaScript, Resources.Resources.Scripts.ResourceManager, "share");
                Resource.Group(Resource.CommonResourceScript, "script_share", 10);
#endif
                
                if (Settings.EnableUI)
                {
#if !DEBUG
                    Resource.Register("script_ui", ResourceType.JavaScript, Resources.Resources.Scripts.ResourceManager, "ui");
                    Resource.Group(Resource.CommonResourceScript, "script_ui", 11);
#endif
                }

                // Images
                Resource.Register("image_shortcuticon", ResourceType.PNG, Resources.Resources.Images.ResourceManager, "shortcuticon");

                // Service
                //Resource.Register("service", ResourceType.JSON, Provider.ServiceBegin)

                if (Settings.EnableUI)
                {
                    if (Settings.FontHeadings == Settings.FontHeadingsDefault)
                    {
                        WebFont.Register("avenir85", "Avenir", null, WebFontWeight.Normal,
                            Resource.Register("font_avenir85_ttf", ResourceType.TTF, Resources.Resources.Fonts.ResourceManager, "avenir85_ttf"),
                            Resource.Register("font_avenir85_woff", ResourceType.WOFF, Resources.Resources.Fonts.ResourceManager, "avenir85_woff"),
                            null);
                    }

                    if (Settings.FontBody == Settings.FontBodyDefault)
                    {
                        WebFont.Register("segoeuil", "Segoe UI", "Segoe UI Light", WebFontWeight.Weight200,
                            Resource.Register("font_segoeuil_ttf", ResourceType.TTF, Resources.Resources.Fonts.ResourceManager, "segoeuil_ttf"),
                            Resource.Register("font_segoeuil_woff", ResourceType.WOFF, Resources.Resources.Fonts.ResourceManager, "segoeuil_woff"),
                            null);
                        WebFont.Register("segoeuisl", "Segoe UI", "Segoe UI SemiLight", WebFontWeight.Weight300,
                            Resource.Register("font_segoeuis_ttf", ResourceType.TTF, Resources.Resources.Fonts.ResourceManager, "segoeuisl_ttf"),
                            Resource.Register("font_segoeuis_woff", ResourceType.WOFF, Resources.Resources.Fonts.ResourceManager, "segoeuisl_woff"),
                            null);
                        WebFont.Register("segoeui", "Segoe UI", null, WebFontWeight.Normal,
                            Resource.Register("font_segoeui_ttf", ResourceType.TTF, Resources.Resources.Fonts.ResourceManager, "segoeui_ttf"),
                            Resource.Register("font_segoeui_woff", ResourceType.WOFF, Resources.Resources.Fonts.ResourceManager, "segoeui_woff"),
                            null);
                        WebFont.Register("seguisb", "Segoe UI", "Segoe UI SemiBold", WebFontWeight.Weight600,
                            Resource.Register("font_seguisb_ttf", ResourceType.TTF, Resources.Resources.Fonts.ResourceManager, "seguisb_ttf"),
                            Resource.Register("font_seguisb_woff", ResourceType.WOFF, Resources.Resources.Fonts.ResourceManager, "seguisb_woff"),
                            null);
                        WebFont.Register("segoeuib", "Segoe UI", null, WebFontWeight.Bold,
                            Resource.Register("font_segoeuib_ttf", ResourceType.TTF, Resources.Resources.Fonts.ResourceManager, "segoeuib_ttf"),
                            Resource.Register("font_segoeuib_woff", ResourceType.WOFF, Resources.Resources.Fonts.ResourceManager, "segoeuib_woff"),
                            null);
                    }

                    WebFont.Register("keepcalmm", "Keep Calm", null, WebFontWeight.Normal,
                        Resource.Register("font_keepcalmm_ttf", ResourceType.TTF, Resources.Resources.Fonts.ResourceManager, "keepcalmm_ttf"),
                        Resource.Register("font_keepcalmm_woff", ResourceType.WOFF, Resources.Resources.Fonts.ResourceManager, "keepcalmm_woff"),
                        null);
                }
            }
        }

        public static void InitDebug()
        {
#if DEBUG
            Resource.Common(Resource.Register("script_ui_debug", ResourceType.JavaScript, Resources.Resources.Scripts.ResourceManager, "ui_debug"));
#endif
        }
    }
}
