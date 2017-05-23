using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;

namespace Aphysoft.Share.WPF
{
    public static class View
    {
        public static void WindowDragMove(Window window, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                window.DragMove();
            }
        }

        public static void WindowMaximizeNormalToggle(Window window, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
            {
                if (window.WindowState == WindowState.Maximized)
                    window.WindowState = WindowState.Normal;
                else
                    window.WindowState = WindowState.Maximized;
            }
        }

        public static void WindowResize(Window window, ResizeSides side, DragDeltaEventArgs e)
        {
            if (side == ResizeSides.Left || side == ResizeSides.TopLeft || side == ResizeSides.BottomLeft)
            {
                double nleft = window.Left + e.HorizontalChange;
                double nwidth = window.Width - e.HorizontalChange;
                if (nwidth < window.MinWidth)
                {
                    nwidth = window.MinWidth;
                    nleft = window.Left + (window.Width - window.MinWidth);
                }

                window.Left = nleft;
                window.Width = nwidth;
            }
            if (side == ResizeSides.Right || side == ResizeSides.TopRight || side == ResizeSides.BottomRight)
            {
                double nwidth = window.Width + e.HorizontalChange;
                if (nwidth < window.MinWidth)
                {
                    nwidth = window.MinWidth;
                }

                window.Width = nwidth;
            }
            if (side == ResizeSides.TopLeft || side == ResizeSides.Top || side == ResizeSides.TopRight)
            {
                double ntop = window.Top + e.VerticalChange;
                double nheight = window.Height - e.VerticalChange;
                if (nheight < window.MinHeight)
                {
                    nheight = window.MinHeight;
                    ntop = window.Top + (window.Height - window.MinHeight);
                }

                window.Top = ntop;
                window.Height = nheight;
            }
            if (side == ResizeSides.BottomLeft || side == ResizeSides.Bottom || side == ResizeSides.BottomRight)
            {
                double nheight = window.Height + e.VerticalChange;
                if (nheight < window.MinHeight)
                {
                    nheight = window.MinHeight;
                }

                window.Height = nheight;
            }
        }
    }

    public enum ResizeSides
    {
        Left,
        Right,
        Top,
        Bottom,
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight
    }
}
