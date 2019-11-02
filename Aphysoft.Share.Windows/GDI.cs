using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Aphysoft.Share
{
    public sealed class GDI
    {
        public static GDI Begin(Control c, PaintEventArgs e, out Graphics graphics)
        {
            GDI gdi = new GDI(c);

            graphics = e.Graphics;

            float scale = (float)c.DeviceDpi / 120;
            graphics.ScaleTransform(scale, scale);

            Rectangle rts = c.RectangleToScreen(c.ClientRectangle);

            gdi.Graphics = graphics;
            gdi.ClientWidthF = rts.Width / scale;
            gdi.ClientHeightF = rts.Height / scale;
            gdi.ClientWidth = gdi.ClientWidthF.Round();
            gdi.ClientHeight = gdi.ClientHeightF.Round();

            return gdi;
        }

        #region Fields

        private Control control;

        public Graphics Graphics { get; private set; }

        public float ClientWidthF { get; private set; }

        public float ClientHeightF { get; private set; }

        public int ClientWidth { get; private set; }
        
        public int ClientHeight { get; private set; }

        #endregion

        #region Constructors

        private GDI(Control control)
        {
            this.control = control;
        }

        #endregion

        #region Methods

        public void DrawOutsideRectangle(Pen pen, int offset = 0)
        {
            int offsetCounterpart = offset * 2;
            Rectangle or = new Rectangle(offset, offset, ClientWidth - (1 + offsetCounterpart), ClientHeight - (1 + offsetCounterpart));

            Graphics.DrawRectangle(pen, or);
        }

        #endregion
    }
}
