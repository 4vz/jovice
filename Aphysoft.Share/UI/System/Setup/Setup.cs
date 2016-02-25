using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Aphysoft.Share.UI.System.Setup
{
    public static class Setup
    {
        public static void PageLoad(PageData page)
        {
            page.Data("serverVersion", Share.ShareVersion);
            page.Data("defaultPage", Settings.DefaultPage);
        }
    }
}
