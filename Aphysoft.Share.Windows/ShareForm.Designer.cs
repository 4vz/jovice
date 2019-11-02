namespace Aphysoft.Share
{
    partial class ShareForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.minimize = new Aphysoft.Share.ShareButton();
            this.maximize = new Aphysoft.Share.ShareButton();
            this.resizeBottom = new System.Windows.Forms.Panel();
            this.resizeRight = new System.Windows.Forms.Panel();
            this.resizeLeft = new System.Windows.Forms.Panel();
            this.resizeBottomRight = new System.Windows.Forms.Panel();
            this.resizeTopRight = new System.Windows.Forms.Panel();
            this.resizeBottomLeft = new System.Windows.Forms.Panel();
            this.resizeTopLeft = new System.Windows.Forms.Panel();
            this.resizeTop = new System.Windows.Forms.Panel();
            this.close = new Aphysoft.Share.ShareButton();
            this.header = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // minimize
            // 
            this.minimize.BackColor = System.Drawing.Color.Transparent;
            this.minimize.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.minimize.Location = new System.Drawing.Point(927, 12);
            this.minimize.Name = "minimize";
            this.minimize.Size = new System.Drawing.Size(51, 45);
            this.minimize.TabIndex = 12;
            this.minimize.UseVisualStyleBackColor = false;
            // 
            // maximize
            // 
            this.maximize.BackColor = System.Drawing.Color.Transparent;
            this.maximize.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.maximize.Location = new System.Drawing.Point(987, 12);
            this.maximize.Name = "maximize";
            this.maximize.Size = new System.Drawing.Size(51, 45);
            this.maximize.TabIndex = 11;
            this.maximize.UseVisualStyleBackColor = false;
            // 
            // resizeBottom
            // 
            this.resizeBottom.BackColor = System.Drawing.Color.Crimson;
            this.resizeBottom.Cursor = System.Windows.Forms.Cursors.SizeNS;
            this.resizeBottom.Location = new System.Drawing.Point(7, 444);
            this.resizeBottom.Margin = new System.Windows.Forms.Padding(0);
            this.resizeBottom.Name = "resizeBottom";
            this.resizeBottom.Size = new System.Drawing.Size(897, 7);
            this.resizeBottom.TabIndex = 9;
            // 
            // resizeRight
            // 
            this.resizeRight.BackColor = System.Drawing.Color.Crimson;
            this.resizeRight.Cursor = System.Windows.Forms.Cursors.SizeWE;
            this.resizeRight.Location = new System.Drawing.Point(917, 7);
            this.resizeRight.Margin = new System.Windows.Forms.Padding(0);
            this.resizeRight.Name = "resizeRight";
            this.resizeRight.Size = new System.Drawing.Size(7, 424);
            this.resizeRight.TabIndex = 8;
            // 
            // resizeLeft
            // 
            this.resizeLeft.BackColor = System.Drawing.Color.Crimson;
            this.resizeLeft.Cursor = System.Windows.Forms.Cursors.SizeWE;
            this.resizeLeft.Location = new System.Drawing.Point(0, 7);
            this.resizeLeft.Margin = new System.Windows.Forms.Padding(0);
            this.resizeLeft.Name = "resizeLeft";
            this.resizeLeft.Size = new System.Drawing.Size(7, 438);
            this.resizeLeft.TabIndex = 7;
            // 
            // resizeBottomRight
            // 
            this.resizeBottomRight.BackColor = System.Drawing.Color.Purple;
            this.resizeBottomRight.Cursor = System.Windows.Forms.Cursors.SizeNWSE;
            this.resizeBottomRight.Location = new System.Drawing.Point(904, 431);
            this.resizeBottomRight.Margin = new System.Windows.Forms.Padding(0);
            this.resizeBottomRight.Name = "resizeBottomRight";
            this.resizeBottomRight.Size = new System.Drawing.Size(20, 20);
            this.resizeBottomRight.TabIndex = 6;
            // 
            // resizeTopRight
            // 
            this.resizeTopRight.BackColor = System.Drawing.Color.Purple;
            this.resizeTopRight.Cursor = System.Windows.Forms.Cursors.SizeNESW;
            this.resizeTopRight.Location = new System.Drawing.Point(917, 0);
            this.resizeTopRight.Margin = new System.Windows.Forms.Padding(0);
            this.resizeTopRight.Name = "resizeTopRight";
            this.resizeTopRight.Size = new System.Drawing.Size(7, 7);
            this.resizeTopRight.TabIndex = 5;
            // 
            // resizeBottomLeft
            // 
            this.resizeBottomLeft.BackColor = System.Drawing.Color.Purple;
            this.resizeBottomLeft.Cursor = System.Windows.Forms.Cursors.SizeNESW;
            this.resizeBottomLeft.Location = new System.Drawing.Point(0, 444);
            this.resizeBottomLeft.Margin = new System.Windows.Forms.Padding(0);
            this.resizeBottomLeft.Name = "resizeBottomLeft";
            this.resizeBottomLeft.Size = new System.Drawing.Size(7, 7);
            this.resizeBottomLeft.TabIndex = 4;
            // 
            // resizeTopLeft
            // 
            this.resizeTopLeft.BackColor = System.Drawing.Color.Purple;
            this.resizeTopLeft.Cursor = System.Windows.Forms.Cursors.SizeNWSE;
            this.resizeTopLeft.Location = new System.Drawing.Point(0, 0);
            this.resizeTopLeft.Margin = new System.Windows.Forms.Padding(0);
            this.resizeTopLeft.Name = "resizeTopLeft";
            this.resizeTopLeft.Size = new System.Drawing.Size(7, 7);
            this.resizeTopLeft.TabIndex = 3;
            // 
            // resizeTop
            // 
            this.resizeTop.BackColor = System.Drawing.Color.Crimson;
            this.resizeTop.Cursor = System.Windows.Forms.Cursors.SizeNS;
            this.resizeTop.Location = new System.Drawing.Point(7, 0);
            this.resizeTop.Margin = new System.Windows.Forms.Padding(0);
            this.resizeTop.Name = "resizeTop";
            this.resizeTop.Size = new System.Drawing.Size(904, 7);
            this.resizeTop.TabIndex = 2;
            // 
            // close
            // 
            this.close.BackColor = System.Drawing.Color.Transparent;
            this.close.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.close.Location = new System.Drawing.Point(1044, 10);
            this.close.Name = "close";
            this.close.Size = new System.Drawing.Size(51, 45);
            this.close.TabIndex = 10;
            this.close.UseVisualStyleBackColor = false;
            // 
            // header
            // 
            this.header.BackColor = System.Drawing.Color.DarkBlue;
            this.header.Location = new System.Drawing.Point(7, 7);
            this.header.Name = "header";
            this.header.Size = new System.Drawing.Size(904, 93);
            this.header.TabIndex = 1;
            this.header.MouseDown += new System.Windows.Forms.MouseEventHandler(this.HeaderMouseDown);
            this.header.MouseMove += new System.Windows.Forms.MouseEventHandler(this.HeaderMouseMove);
            this.header.MouseUp += new System.Windows.Forms.MouseEventHandler(this.HeaderMouseUp);
            // 
            // ShareForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1317, 673);
            this.Controls.Add(this.minimize);
            this.Controls.Add(this.maximize);
            this.Controls.Add(this.resizeBottom);
            this.Controls.Add(this.resizeRight);
            this.Controls.Add(this.resizeLeft);
            this.Controls.Add(this.resizeBottomRight);
            this.Controls.Add(this.resizeTopRight);
            this.Controls.Add(this.resizeBottomLeft);
            this.Controls.Add(this.resizeTopLeft);
            this.Controls.Add(this.resizeTop);
            this.Controls.Add(this.close);
            this.Controls.Add(this.header);
            this.MinimumSize = new System.Drawing.Size(100, 100);
            this.Name = "ShareForm";
            this.Text = "Aphysoft.Share.Windows";
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel header;
        private System.Windows.Forms.Panel resizeTop;
        private System.Windows.Forms.Panel resizeTopLeft;
        private System.Windows.Forms.Panel resizeBottomLeft;
        private System.Windows.Forms.Panel resizeTopRight;
        private System.Windows.Forms.Panel resizeBottomRight;
        private System.Windows.Forms.Panel resizeLeft;
        private System.Windows.Forms.Panel resizeRight;
        private System.Windows.Forms.Panel resizeBottom;
        private ShareButton close;
        private ShareButton maximize;
        private ShareButton minimize;
    }
}