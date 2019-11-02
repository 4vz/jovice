using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Aphysoft.Share
{
    public delegate void ButtonPaintEventHandler(GDI gdi);

    public class ShareButton : Button
    {
        #region Fields

        public new string Text { get; set; } = "";

        private bool mouseOver = false;
        private bool mouseDown = false;

        #endregion

        #region Constructors

        public ShareButton()
        {
            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer,
                true);

            FlatStyle = FlatStyle.Flat;
            FlatAppearance.MouseOverBackColor = Color.Transparent;
            FlatAppearance.MouseDownBackColor = Color.Transparent;
            FlatAppearance.BorderColor = Color.FromArgb(0, 255, 255, 255);
            FlatAppearance.BorderSize = 0;

            base.Text = "";
        }

        #endregion

        #region Events
        
        public event ButtonPaintEventHandler NormalPaint;

        public event ButtonPaintEventHandler MouseOverPaint;

        public event ButtonPaintEventHandler MouseDownPaint;

        #endregion

        #region Methods

        #endregion

        #region Handlers

        protected override void OnMouseEnter(EventArgs e)
        {
            mouseOver = true;
            base.OnMouseEnter(e);

            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            mouseOver = false;
            base.OnMouseLeave(e);

            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            mouseDown = true;
            base.OnMouseDown(mevent);

            Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            mouseDown = false;
            base.OnMouseUp(mevent);

            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            GDI gdi = GDI.Begin(this, e, out Graphics g);

            g.SmoothingMode = SmoothingMode.HighQuality;

            if (mouseDown)
                MouseDownPaint?.Invoke(gdi);
            else if (mouseOver)
                MouseOverPaint?.Invoke(gdi);
            else
                NormalPaint?.Invoke(gdi);

            g.SmoothingMode = SmoothingMode.Default;
        }

        #endregion
    }
}
