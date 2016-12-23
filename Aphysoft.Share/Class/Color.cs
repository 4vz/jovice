using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aphysoft.Share
{ 
    public struct Color
    {
        #region Fields

        private int red;

        public int Red
        {
            get { return red; }
            set { red = value > 255 ? 255 : value < 0 ? 0 : value; }
        }

        private int green;

        public int Green
        {
            get { return green; }
            set { green = value > 255 ? 255 : value < 0 ? 0 : value; }
        }

        private int blue;

        public int Blue
        {
            get { return blue; }
            set { blue = value > 255 ? 255 : value < 0 ? 0 : value; }
        }

        #endregion

        #region Constructors

        public Color(int red, int green, int blue)
        {
            this.red = red > 255 ? 255 : red < 0 ? 0 : red;
            this.green = green > 255 ? 255 : green < 0 ? 0 : green;
            this.blue = blue > 255 ? 255 : blue < 0 ? 0 : blue;
        }

        public Color(string hex)
        {
            if (hex.Length == 6)
            {
                string r = hex.Substring(0, 2);
                this.red = Convert.ToInt32(r, 16);
                string g = hex.Substring(2, 2);
                this.green = Convert.ToInt32(g, 16);
                string b = hex.Substring(4, 2);
                this.blue = Convert.ToInt32(b, 16);
            }
            else if (hex.Length == 3)
            {
                string r = hex.Substring(0, 1);
                this.red = Convert.ToInt32(r, 16);
                string g = hex.Substring(1, 1);
                this.green = Convert.ToInt32(g, 16);
                string b = hex.Substring(2, 1);
                this.blue = Convert.ToInt32(b, 16);
            }
            else
            {
                this.red = 0;
                this.green = 0;
                this.blue = 0;
            }
        }

        #endregion

        #region Methods

        public string Hex()
        {
            return string.Format("{0:x2}{1:x2}{2:x2}", red, green, blue);
        }

        #endregion
    }
}
