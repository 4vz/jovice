using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aphysoft.Common.Html;

namespace Aphysoft.Share
{
    public interface IUIControls
    {
        string ID { get; set; }
        void Process(HtmlNode node, string id);
    }
}
