using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using Aphysoft.Share;
using ImageProcessor;
using Svg;
using Svg.Pathing;

namespace Aphysoft.Share
{
    public partial class ShareForm : Form
    {
        #region Fields

        protected Node node = null;

        private bool dwmEffect = false;

        [StructLayout(LayoutKind.Sequential)]
        struct MARGINS { public int Left, Right, Top, Bottom; }

        [DllImport("dwmapi.dll", PreserveSig = false)]
        static extern void DwmExtendFrameIntoClientArea(IntPtr hwnd, ref MARGINS margins);

        [DllImport("dwmapi.dll", PreserveSig = false)]
        static extern bool DwmIsCompositionEnabled();

        const int CS_DROPSHADOW = 0x00020000;
        const int WS_EX_COMPOSITED = 0x02000000;
        const int WS_MINIMIZEBOX = 0x20000;
        const int WS_SYSMENU = 0x80000;
        const int WS_SIZEBOX = 0x40000;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams handleParam = base.CreateParams;
                if (!DesignMode)
                {
                    if (dwmEffect)
                    {
                        //handleParam.Style |= WS_MINIMIZEBOX;
                        //handleParam.ClassStyle |= CS_DBLCLKS;
                        //handleParam.Style = WS_SIZEBOX;
                    }
                    else
                        handleParam.ClassStyle |= CS_DROPSHADOW;

                    handleParam.ExStyle |= WS_EX_COMPOSITED;  
                }
                return handleParam;
            }
        }

        private FormWindowState lastWindowState = FormWindowState.Normal;
        private Size lastWindowSize = Size.Empty;
        
               
        private FormBorderStyle formBorderStyle = FormBorderStyle.Sizable;

        public new FormBorderStyle FormBorderStyle
        {
            get => formBorderStyle;
            set
            {
                if (value != formBorderStyle)
                {
                    Apps.Console("FormBorderStyle has changed to: " + value);
                    formBorderStyle = value;
                    RefreshControls();
                }
            }
        }

        private bool resizable = false;
        private bool activated = true;

        private Dictionary<string, Bitmap> backgrounds = new Dictionary<string, Bitmap>();
        private Dictionary<string, Brush> backgroundBrushes = new Dictionary<string, Brush>();
        private Dictionary<string, Brush> backgroundHeadBrushes = new Dictionary<string, Brush>();

        private Bitmap currentBackgroundBitmap = null;
        private Brush currentBackgroundBrush = null;
        private Brush currentBackgroundHeadBrush = null;

        private Pen outerBlack = new Pen(Color.Black);
        private Pen linelit = new Pen(Color.FromArgb(40, Color.White));
        private Pen line = new Pen(Color.FromArgb(20, Color.White));
        private Pen linedim = new Pen(Color.FromArgb(10, Color.White));
        private Brush controlBrushNormal = new SolidBrush(Color.FromArgb(30, Color.White));
        private Brush controlBrushOver = new SolidBrush(Color.FromArgb(98, Color.White));
        private Brush topFormAccentNormal = new SolidBrush(Color.FromArgb(10, Color.White));
        private Brush topFormAccentInactive = new SolidBrush(Color.FromArgb(30, Color.Black));

        GraphicsPath gpClose = null, gpMaximize = null, gpMinimize = null, gpRestore = null;

        private bool drag = false;
        private bool resize = false;
        private bool resizing = false;
        private Point mouseStart = Point.Empty;

        const int controlButtonSize = 30;

        #endregion

        #region Constructors

        public ShareForm()
        {
            InitializeComponent();
        }

        public ShareForm(Node node)
        {
            InitializeComponent();

            dwmEffect = true;// IsCompositionEnabled();

            if (!DesignMode)
            {
                this.node = node;

                SetStyle(
                    ControlStyles.AllPaintingInWmPaint
                     | ControlStyles.OptimizedDoubleBuffer
                     | ControlStyles.UserPaint
                     | ControlStyles.ResizeRedraw
                     , true);
            }
        }

        #endregion
        
        #region Node Methods

        protected void Event(string message)
        {
            node.Event(message, "FORM", this.Name);
        }

        #endregion

        #region Methods

        //this is for checking the OS's functionality.
        //Windows XP does not have dwmapi.dll
        //also, This corrupts the designer... 
        //so i used the Release/Debug configuration
        private bool IsCompositionEnabled()
        {
            if (!DesignMode)
                return File.Exists(Environment.SystemDirectory + "\\dwmapi.dll")
                    && DwmIsCompositionEnabled();
            else
                return false;
        }

        public void PreloadBackground(string key, Bitmap bitmap)
        {
            if (!backgrounds.ContainsKey(key))
            {
                backgrounds.Add(key, null);

                Thread t = new Thread(new ThreadStart(delegate ()
                {
                    BitmapData srcData = bitmap.LockBits(
                        new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                        ImageLockMode.ReadOnly,
                        PixelFormat.Format32bppArgb);

                    int stride = srcData.Stride;

                    IntPtr Scan0 = srcData.Scan0;

                    long[] totals = new long[] { 0, 0, 0 };

                    int width = bitmap.Width;
                    int height = bitmap.Height;

                    unsafe
                    {
                        byte* p = (byte*)(void*)Scan0;

                        for (int y = 0; y < height; y++)
                        {
                            for (int x = 0; x < width; x++)
                            {
                                for (int color = 0; color < 3; color++)
                                {
                                    int idx = (y * stride) + x * 4 + color;

                                    totals[color] += p[idx];
                                }
                            }
                        }
                    }

                    int avgB = (int)(totals[0] / (width * height)) - 20;
                    int avgG = (int)(totals[1] / (width * height)) - 20;
                    int avgR = (int)(totals[2] / (width * height)) - 20;

                    if (avgB < 0) avgB = 0;
                    if (avgG < 0) avgG = 0;
                    if (avgR < 0) avgR = 0;

                    Color headColor = Color.FromArgb(avgR, avgG, avgB);

                    backgroundHeadBrushes.Add(key, new SolidBrush(headColor));

                    avgB -= 10;
                    avgG -= 10;
                    avgR -= 10;

                    if (avgB < 0) avgB = 0;
                    if (avgG < 0) avgG = 0;
                    if (avgR < 0) avgR = 0;

                    Color backgroundColor = Color.FromArgb(avgR, avgG, avgB);

                    backgroundBrushes.Add(key, new SolidBrush(backgroundColor));

                    bitmap.UnlockBits(srcData);

                    // Draw
                    using (Graphics gb = Graphics.FromImage(bitmap))
                    {
                        Rectangle ghleft = new Rectangle(0, 0, 200, 100);
                        Rectangle ghright = new Rectangle(bitmap.Width - 200, 0, 200, 100);
                        gb.FillRectangle(new LinearGradientBrush(ghleft, headColor, Color.Transparent, LinearGradientMode.Horizontal), ghleft);
                        gb.FillRectangle(new LinearGradientBrush(ghright, Color.Transparent, headColor, LinearGradientMode.Horizontal), ghright);

                        Rectangle gbleft = new Rectangle(0, 100, 200, height - 100);
                        Rectangle gbright = new Rectangle(bitmap.Width - 200, 100, 200, height - 100);
                        gb.FillRectangle(new LinearGradientBrush(gbleft, backgroundColor, Color.Transparent, LinearGradientMode.Horizontal), gbleft);
                        gb.FillRectangle(new LinearGradientBrush(gbright, Color.Transparent, backgroundColor, LinearGradientMode.Horizontal), gbright);

                        Rectangle gbbottom = new Rectangle(0, bitmap.Height - 200, width, 200);
                        gb.FillRectangle(new LinearGradientBrush(gbbottom, Color.Transparent, backgroundColor, LinearGradientMode.Vertical), gbbottom);

                    }


                    backgrounds[key] = bitmap;


                }));

                t.Start();
            }
        }        

        public void SetBackground(string key)
        {
            if (backgrounds.ContainsKey(key))
            {
                Thread t = new Thread(new ThreadStart(delegate ()
                {
                    while (backgrounds[key] == null)
                    {
                        // loading
                        Thread.Sleep(100);
                    }
                    currentBackgroundBitmap = backgrounds[key];
                    currentBackgroundBrush = backgroundBrushes[key];
                    currentBackgroundHeadBrush = backgroundHeadBrushes[key];
                    Invalidate();
                }));
                t.Start();
            }
        }

        private void BeginResize(object sender, MouseEventArgs e)
        {
            resize = true;
            mouseStart = e.Location;
            resizing = false;
        }

        private void EndResize(object sender, MouseEventArgs e)
        {
            resize = false;

            if (resizing)            
                OnResizeEnd(EventArgs.Empty);

            resizing = false;
        }

        private void TopResize(object sender, MouseEventArgs e)
        {
            if (resize)
            {
                int hc = (mouseStart.Y - e.Y);
                Top = Top - hc;
                Height = Height + hc;
            }
        }

        private void BottomResize(object sender, MouseEventArgs e)
        {
            if (resize)
            {
                int hc = (e.Y - mouseStart.Y);
                Height = Height + hc;
            }
        }

        private void LeftResize(object sender, MouseEventArgs e)
        {
            if (resize)
            {
                int hc = (mouseStart.X - e.X);

                int xa = Width + hc;
                Width = xa;

               // Apps.Console(Width + " " + hc);

                //if (xa == Width)
               // {
                //    int nhc = 
                    Left = Left - hc;
                //    Apps.Console("Moving " + nhc);
               // }




            }
        }

        private void RightResize(object sender, MouseEventArgs e)
        {
            if (resize)
            {
                int hc = (e.X - mouseStart.X);
                Width = Width + hc;
            }
        }

        private void HeaderMouseDown(object sender, MouseEventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
            {
                drag = true;

                mouseStart = e.Location;
            }
        }

        private void HeaderMouseMove(object sender, MouseEventArgs e)
        {
            if (drag)
            {
                Location = new Point(Location.X + e.X - mouseStart.X, Location.Y + e.Y - mouseStart.Y);
            }
        }

        private void HeaderMouseUp(object sender, MouseEventArgs e)
        {
            drag = false;
        }

        private Graphics BeginPaint(PaintEventArgs e)
        {
            return null;
        }

        #endregion

        #region Handler
    
        const int WM_NCCALCSIZE = 0x0083;

        // WndProc for special handle
        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            if (dwmEffect)
            {                
                switch (m.Msg)
                {
                    // no border
                    case WM_NCCALCSIZE:
                        if (dwmEffect)
                        {
                            if (m.WParam.ToInt32() == 1)
                            {
                                m.Result = new IntPtr(0xF0);
                                return;
                            }
                            //return;
                        }
                        break;
                }
            }

            base.WndProc(ref m);
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            activated = true;
            Invalidate();
        }

        protected override void OnDeactivate(EventArgs e)
        {
            base.OnDeactivate(e);
            activated = false;
            Invalidate();
        }

        protected override void OnLoad(EventArgs e)
        {
            if (!DesignMode)
            {
                Apps.Console("Onload: " + formBorderStyle);

                lastWindowState = WindowState;
                lastWindowSize = Size;

                if (dwmEffect)
                {
                    base.FormBorderStyle = FormBorderStyle.FixedSingle;
                    //Size = new Size(Width, Height);
                    MARGINS _glassMargins = new MARGINS()
                    {
                        Top = 1,
                        Left = 1,
                        Bottom = 1,
                        Right = 1
                    };
                    DwmExtendFrameIntoClientArea(this.Handle, ref _glassMargins);
                }
                else
                    base.FormBorderStyle = FormBorderStyle.None;

                Rectangle rts = RectangleToScreen(ClientRectangle);
                int wdif = (Width - rts.Width);
                int hdif = (Height - rts.Height);

                #region Paths

                gpClose = Svg.Path(@"M336.559,68.611L231.016,174.165l105.543,105.549c15.699,15.705,15.699,41.145,0,56.85
		    c-7.844,7.844-18.128,11.769-28.407,11.769c-10.296,0-20.581-3.919-28.419-11.769L174.167,231.003L68.609,336.563
		    c-7.843,7.844-18.128,11.769-28.416,11.769c-10.285,0-20.563-3.919-28.413-11.769c-15.699-15.698-15.699-41.139,0-56.85
		    l105.54-105.549L11.774,68.611c-15.699-15.699-15.699-41.145,0-56.844c15.696-15.687,41.127-15.687,56.829,0l105.563,105.554
		    L279.721,11.767c15.705-15.687,41.139-15.687,56.832,0C352.258,27.466,352.258,52.912,336.559,68.611z");

                gpMaximize = Svg.Path(@"M408,51H51C22.95,51,0,73.95,0,102v255c0,28.05,22.95,51,51,51h357c28.05,0,51-22.95,51-51V102
			C459,73.95,436.05,51,408,51z M408,357H51V102h357V357z");

                gpRestore = Svg.Path(@"M70,0H29.9C24.4,0,20,4.4,20,9.9V50c0,5.5,4.5,10,10,10h40c5.5,0,10-4.5,10-10V10
			C80,4.5,75.5,0,70,0z M70,50H30V10h40V50z M10,40H0v30c0,5.5,4.5,10,10,10h30V70H10V40z");

                gpMinimize = Svg.Path(@"M37.059,16H26H16H4.941C2.224,16,0,18.282,0,21s2.224,5,4.941,5H16h10h11.059C39.776,26,42,23.718,42,21
	S39.776,16,37.059,16z");

                using (Matrix r = new Matrix())
                {
                    r.Translate(9, 9);
                    r.Scale(0.034f, 0.034f);
                    gpClose.Transform(r);
                }
                using (Matrix r = new Matrix())
                {
                    r.Translate(7, 7);
                    r.Scale(0.034f, 0.034f);
                    gpMaximize.Transform(r);
                }
                using (Matrix r = new Matrix())
                {
                    //
                    //
                    r.Translate(7f, 7f);
                    r.Scale(0.2f, 0.2f);
                    r.Rotate(180f);
                    r.Translate(-80f, -75f);
                    
                    

                    gpRestore.Transform(r);
                }
                using (Matrix r = new Matrix())
                {
                    r.Translate(6.8f, 12.5f);
                    r.Scale(0.35f, 0.3f);
                    gpMinimize.Transform(r);
                }

                #endregion

                #region Header

                header.Width = Width - 20 - wdif;
                header.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                header.BackColor = Color.Transparent;

                header.DoubleClick += delegate (object sender, EventArgs edc)
                {
                    Console.WriteLine("Double click: " + FormBorderStyle);

                    if (formBorderStyle == FormBorderStyle.Sizable)
                    {
                        if (WindowState == FormWindowState.Normal)
                        {
                            WindowState = FormWindowState.Maximized;
                            RefreshControls();
                        }
                        else if (WindowState == FormWindowState.Maximized)
                        {
                            WindowState = FormWindowState.Normal;
                            RefreshControls();
                        }

                        //
                    }
                };
                   
                #endregion

                #region Resize

                resizeTop.Width = Width - 14 - wdif;
                resizeTop.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                resizeTop.BackColor = Color.Transparent;
                resizeTop.MouseDown += BeginResize;
                resizeTop.MouseMove += TopResize;
                resizeTop.MouseUp += EndResize;

                resizeLeft.Height = Height - 14 - hdif;
                resizeLeft.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
                resizeLeft.BackColor = Color.Transparent;
                resizeLeft.MouseDown += BeginResize;
                resizeLeft.MouseMove += LeftResize;
                resizeLeft.MouseUp += EndResize;

                resizeRight.Left = Width - 7 - wdif;
                resizeRight.Height = Height - 27 - hdif;
                resizeRight.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
                resizeRight.BackColor = Color.Transparent;
                resizeRight.MouseDown += BeginResize;
                resizeRight.MouseMove += RightResize;
                resizeRight.MouseUp += EndResize;

                resizeBottom.Width = Width - 27 - wdif;
                resizeBottom.Top = Height - 7 - hdif;
                resizeBottom.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
                resizeBottom.BackColor = Color.Transparent;
                resizeBottom.MouseDown += BeginResize;
                resizeBottom.MouseMove += BottomResize;
                resizeBottom.MouseUp += EndResize;

                resizeTopLeft.BackColor = Color.Transparent;
                resizeTopLeft.MouseDown += BeginResize;
                resizeTopLeft.MouseMove += TopResize;
                resizeTopLeft.MouseMove += LeftResize;
                resizeTopLeft.MouseUp += EndResize;

                resizeBottomLeft.Top = Height - 7 - hdif;
                resizeBottomLeft.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
                resizeBottomLeft.BackColor = Color.Transparent;
                resizeBottomLeft.MouseDown += BeginResize;
                resizeBottomLeft.MouseMove += BottomResize;
                resizeBottomLeft.MouseMove += LeftResize;


                resizeBottomLeft.MouseUp += EndResize;

                resizeTopRight.Left = Width - 7 - wdif;
                resizeTopRight.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                resizeTopRight.BackColor = Color.Transparent;
                resizeTopRight.MouseDown += BeginResize;
                resizeTopRight.MouseMove += TopResize;
                resizeTopRight.MouseMove += RightResize;
                resizeTopRight.MouseUp += EndResize;

                resizeBottomRight.Left = Width - 20 - wdif;
                resizeBottomRight.Top = Height - 20 - hdif;
                resizeBottomRight.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
                resizeBottomRight.BackColor = Color.Transparent;
                resizeBottomRight.MouseDown += BeginResize;
                resizeBottomRight.MouseMove += BottomResize;
                resizeBottomRight.MouseMove += RightResize;
                resizeBottomRight.MouseUp += EndResize;

                #endregion

                #region Control Buttons



                close.Width = controlButtonSize;
                close.Height = controlButtonSize;
                close.Anchor = AnchorStyles.Top | AnchorStyles.Right;

                close.NormalPaint += delegate (GDI gdi)
                {
                    Graphics g = gdi.Graphics;
                    g.FillPath(controlBrushNormal, gpClose);
                };
                close.MouseOverPaint += delegate (GDI gdi)
                {
                    Graphics g = gdi.Graphics;
                    g.FillPath(controlBrushOver, gpClose);
                };
                close.MouseDownPaint += delegate (GDI gdi)
                {
                    Graphics g = gdi.Graphics;
                    using (GraphicsPath gps = (GraphicsPath)gpClose.Clone())
                    {
                        using (Matrix r = new Matrix())
                        {
                            r.Translate(1.5f, 1.5f);
                            gps.Transform(r);
                        }
                        g.FillPath(controlBrushNormal, gps);
                    }                    
                };
                close.Click += delegate (object sender, EventArgs ea)
                {
                    Close();
                };

                maximize.Width = controlButtonSize;
                maximize.Height = controlButtonSize;
                maximize.Anchor = AnchorStyles.Top | AnchorStyles.Right;

                maximize.NormalPaint += delegate (GDI gdi)
                {
                    Graphics g = gdi.Graphics;

                    if (WindowState == FormWindowState.Maximized)
                        g.FillPath(controlBrushNormal, gpRestore);
                    else
                        g.FillPath(controlBrushNormal, gpMaximize);
                };
                maximize.MouseOverPaint += delegate (GDI gdi)
                {
                    Graphics g = gdi.Graphics;

                    if (WindowState == FormWindowState.Maximized)
                        g.FillPath(controlBrushOver, gpRestore);
                    else
                        g.FillPath(controlBrushOver, gpMaximize);
                };
                maximize.MouseDownPaint += delegate (GDI gdi)
                {
                    Graphics g = gdi.Graphics;

                    GraphicsPath tp = null;
                    if (WindowState == FormWindowState.Maximized)
                        tp = gpRestore;
                    else
                        tp = gpMaximize;

                    using (GraphicsPath gps = (GraphicsPath)tp.Clone())
                    {
                        using (Matrix r = new Matrix())
                        {
                            r.Translate(1.5f, 1.5f);
                            gps.Transform(r);
                        }
                        g.FillPath(controlBrushNormal, gps);
                    }
                };
                maximize.Click += delegate (object sender, EventArgs ea)
                {
                    if (WindowState == FormWindowState.Maximized)
                        WindowState = FormWindowState.Normal;
                    else
                        WindowState = FormWindowState.Maximized;

                    RefreshControls();

                    
                };
                //maximize.Click

                minimize.Width = controlButtonSize;
                minimize.Height = controlButtonSize;
                minimize.Anchor = AnchorStyles.Top | AnchorStyles.Right;

                minimize.NormalPaint += delegate (GDI gdi)
                {
                    Graphics g = gdi.Graphics;
                    g.FillPath(controlBrushNormal, gpMinimize);
                };
                minimize.MouseOverPaint += delegate (GDI gdi)
                {
                    Graphics g = gdi.Graphics;
                    g.FillPath(controlBrushOver, gpMinimize);
                };
                minimize.MouseDownPaint += delegate (GDI gdi)
                {
                    Graphics g = gdi.Graphics;
                    using (GraphicsPath gps = (GraphicsPath)gpMinimize.Clone())
                    {
                        using (Matrix r = new Matrix())
                        {
                            r.Translate(1.5f, 1.5f);
                            gps.Transform(r);
                        }
                        g.FillPath(controlBrushNormal, gps);
                    }
                };
                minimize.Click += delegate (object sender, EventArgs ea)
                {
                    WindowState = FormWindowState.Minimized;
                };

                #endregion

                RefreshControls();
            }


            base.OnLoad(e);
        }

        protected override void OnResizeEnd(EventArgs e)
        {
            Apps.Console("Resized");

            base.OnResizeEnd(e);

            Invalidate();
        }

        private void RefreshControls()
        {
            if (!DesignMode)
            {
                Rectangle rts = RectangleToScreen(ClientRectangle);
                int wdif = (Width - rts.Width);
                int hdif = (Height - rts.Height);

                if (formBorderStyle == FormBorderStyle.Sizable || formBorderStyle == FormBorderStyle.SizableToolWindow)
                {
                    if (WindowState == FormWindowState.Normal)
                    {
                        resizable = true;
                    }
                    else
                    {
                        resizable = false;
                    }
                }
                else
                    resizable = false;

                Apps.Console(formBorderStyle + " Resizable: " + resizable);

                if (resizable)
                {
                    resizeTop.Show();
                    resizeLeft.Show();
                    resizeRight.Show();
                    resizeBottom.Show();
                    resizeTopLeft.Show();
                    resizeTopRight.Show();
                    resizeBottomLeft.Show();
                    resizeBottomRight.Show();
                }
                else
                {
                    resizeTop.Hide();
                    resizeLeft.Hide();
                    resizeRight.Hide();
                    resizeBottom.Hide();
                    resizeTopLeft.Hide();
                    resizeTopRight.Hide();
                    resizeBottomLeft.Hide();
                    resizeBottomRight.Hide();
                }

                int controlMarginTop = 0, controlMarginRight = 0;

                if (WindowState == FormWindowState.Maximized)
                {
                    controlMarginTop = 8;
                    controlMarginRight = 5;
                }

                close.Top = resizeTop.Height - 3 + controlMarginTop;
                close.Left = Width - controlButtonSize - wdif - resizeRight.Width - controlMarginRight;
                maximize.Top = resizeTop.Height - 3 + controlMarginTop;
                maximize.Left = Width - (controlButtonSize * 2) - wdif - resizeRight.Width - controlMarginRight;
                minimize.Top = resizeTop.Height - 3 + controlMarginTop;
                minimize.Left = Width - (controlButtonSize * 3) - wdif - resizeRight.Width - controlMarginRight;

                Invalidate();
            }
        }

        public void SetScale(Graphics g, out float scale)
        {
            scale = (float)DeviceDpi / 120;
            g.ScaleTransform(scale, scale);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (!DesignMode)
            {
                if (restoring)
                {
                    restoring = false;
                    Size = lastWindowSize;
                }

                GDI gdi = GDI.Begin(this, e, out Graphics g);

                int w = gdi.ClientWidth;
                int h = gdi.ClientHeight;

                //Apps.Console(Width + " " + Height + " - " + w + " " + h);

                if (currentBackgroundBitmap != null)
                {
                    g.FillRectangle(currentBackgroundHeadBrush, new RectangleF(0, 0, w - 1, 100));
                    g.FillRectangle(currentBackgroundBrush, new RectangleF(0, 100, w - 1, h - 101));
                    g.DrawImage(currentBackgroundBitmap, (gdi.ClientWidthF - currentBackgroundBitmap.Width) / 2, 0, currentBackgroundBitmap.Width, currentBackgroundBitmap.Height);
                }

                // TOP FORM ACCENT
                PointF[] tfap = { new PointF(130, 2), new PointF(140, 12), new PointF(w - 140, 12), new PointF(w - 130, 2) };
                PointF[] tfapshade = { new PointF(2, 2),
                    new PointF(129, 2), new PointF(139, 13), new PointF(w - 139, 13), new PointF(w - 129, 2),
                    new PointF(w - 2, 2), new PointF(w - 2, h - 2), new PointF(2, h - 2)
                };

                gdi.DrawOutsideRectangle(outerBlack);
                gdi.DrawOutsideRectangle(line, 1);

                if (w > 500 && WindowState != FormWindowState.Maximized)
                {
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.DrawLines(linedim, tfapshade);
                    g.SmoothingMode = SmoothingMode.None;

                    GraphicsPath gpFormTopAccent = new GraphicsPath();
                    gpFormTopAccent.AddLines(tfap);
                    gpFormTopAccent.CloseFigure();


                    if (activated)
                    {
                        g.FillPath(topFormAccentNormal, gpFormTopAccent);
                    }
                    else
                    {
                        g.FillPath(topFormAccentInactive, gpFormTopAccent);
                    }

                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.DrawLines(line, tfap);
                    g.SmoothingMode = SmoothingMode.None;
                }

                g.DrawLine(linelit, new Point(2, 100), new Point(w - 3, 100));

                if (resizable)
                {
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.DrawLine(line, w - 9, h - 6, w - 6, h - 9);
                    g.DrawLine(line, w - 14, h - 6, w - 6, h - 14);
                    g.DrawLine(line, w - 19, h - 6, w - 6, h - 19);
                    g.SmoothingMode = SmoothingMode.None;
                }
            }

            base.OnPaint(e);
        }

        protected override void OnDpiChanged(DpiChangedEventArgs e)
        {            
            base.OnDpiChanged(e);
            Invalidate();
        }

        private bool restoring = false;

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            resizing = true;

            if (lastWindowState != WindowState)
            {
                if (WindowState == FormWindowState.Normal)
                {
                    restoring = true;
                    lastWindowSize = Size;
                }
                lastWindowState = WindowState;

                Thread s = new Thread(new ThreadStart(delegate () {
                    Thread.Sleep(10);
                    Invalidate();

                    
                }));
                s.Start();
            }
        }

        #endregion
    }
}
