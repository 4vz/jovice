using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aphysoft.Share
{
    public static class SizeExtensions
    {
        public static Size ResizeByWidth(this Size value, int width)
        {
            int ow = value.Width;
            int oh = value.Height;
            int height = oh * width / ow;
            return new Size(width, height);
        }

        public static Size ResizeByHeight(this Size value, int height)
        {
            int ow = value.Width;
            int oh = value.Height;
            int width = ow * height / oh;
            return new Size(width, height);
        }
    }
}
