using Svg;
using Svg.Pathing;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aphysoft.Share
{
    public static class Svg
    {
        public static GraphicsPath Path(string path)
        {
            GraphicsPath gp = new GraphicsPath();

            foreach (SvgPathSegment segment in SvgPathBuilder.Parse(path))
                segment.AddToPath(gp);

            return gp;
        }
    }
}
