using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

namespace DarkControls.Controls
{
    public class ProgressBarEx : ProgressBar
    {
        private Timer _marqueeTimer;

        public ProgressBarEx()
        {
            this.SetStyle(ControlStyles.UserPaint, true);
            _marqueeTimer = new Timer();
            _marqueeTimer.Interval = MarqueeAnimationSpeed;
            _marqueeTimer.Tick += new EventHandler(marqueeTimer_Tick);
            _marqueeTimer.Start();
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            // None... Helps control the flicker.
        }

        private void marqueeTimer_Tick(object sender, EventArgs e)
        {
            if (this.Style == ProgressBarStyle.Marquee) this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (this.Style == ProgressBarStyle.Marquee)
            {
                int blockWidth = 5;
                int blockSpacing = 2;
                int blockCount = (this.Width - 2) / (blockWidth + blockSpacing);
                int offset = DateTime.Now.Millisecond % (blockCount + blockSpacing);

                using (Image offscreenImage = new Bitmap(this.Width, this.Height))
                {
                    using (Graphics offscreen = Graphics.FromImage(offscreenImage))
                    {
                        offscreen.Clear(this.BackColor);
                        for (int i = 0; i < blockCount; i++)
                        {
                            int x = 2 + (i * (blockWidth + blockSpacing)) - offset;
                            int y = 2;
                            int width = blockWidth;
                            int height = this.Height - 4;
                            if (x + width > this.Width)
                                width = this.Width - x;
                            if (x < 2)
                            {
                                width -= 2 - x;
                                x = 2;
                            }
                            offscreen.FillRectangle(new SolidBrush(this.ForeColor), x, y, width, height);
                        }
                        e.Graphics.DrawImage(offscreenImage, 0, 0);
                    }
                }
            }
            else
            {
                const int inset = 2; // A single inset value to control the sizing of the inner rect.

                using (Image offscreenImage = new Bitmap(this.Width, this.Height))
                {
                    using (Graphics offscreen = Graphics.FromImage(offscreenImage))
                    {
                        offscreen.Clear(this.BackColor);
                        Rectangle rect = new Rectangle(0, 0, this.Width, this.Height);
                        offscreen.DrawRectangle(new Pen(Color.Silver, 2), rect);

                        //if (ProgressBarRenderer.IsSupported)
                        //    ProgressBarRenderer.DrawHorizontalBar(offscreen, rect);

                        rect.Inflate(new Size(-inset, -inset)); // Deflate inner rect.
                        rect.Width = (int)(rect.Width * ((double)this.Value / this.Maximum));
                        if (rect.Width == 0) rect.Width = 1; // Can't draw rec with width of 0.

                        //LinearGradientBrush brush = new LinearGradientBrush(rect, this.BackColor, this.ForeColor, LinearGradientMode.Horizontal);
                        SolidBrush brush = new SolidBrush(this.ForeColor);

                        offscreen.FillRectangle(brush, inset, inset, rect.Width, rect.Height);

                        e.Graphics.DrawImage(offscreenImage, 0, 0);
                    }
                }
            }
        }
    }
}
