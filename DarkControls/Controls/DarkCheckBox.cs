using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using SystemMath = System.Math;

namespace DarkControls.Controls
{
    public class DarkCheckBox : CheckBox
    {
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
(
int nLeftRect,     // x-coordinate of upper-left corner
int nTopRect,      // y-coordinate of upper-left corner
int nRightRect,    // x-coordinate of lower-right corner
int nBottomRect,   // y-coordinate of lower-right corner
int nWidthEllipse, // width of ellipse
int nHeightEllipse // height of ellipse
);

        public Color CheckColor { get; set; } = Color.CornflowerBlue;
        public Color BoxFillColor { get; set; } = Color.FromArgb(33, 33, 33);
        public Color BoxBorderColor { get; set; } = Color.DarkSlateBlue;
        public DarkCheckBox()
        {
            Appearance = System.Windows.Forms.Appearance.Button;
            FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            TextAlign = ContentAlignment.MiddleRight;
            FlatAppearance.BorderSize = 0;
            AutoSize = false;
            Height = 16;
        }

        public static GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            Size size = new Size(diameter, diameter);
            Rectangle arc = new Rectangle(bounds.Location, size);
            GraphicsPath path = new GraphicsPath();

            if (radius == 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            // top left arc
            path.AddArc(arc, 180, 90);

            // top right arc
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);

            // bottom right arc
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            // bottom left arc
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }

        bool mouseEntered = false;
        bool mouseDown = false;

        protected override void OnMouseEnter(EventArgs eventargs)
        {
            mouseEntered = true;
            base.OnMouseEnter(eventargs);
        }

        protected override void OnMouseLeave(EventArgs eventargs)
        {
            mouseEntered = false;
            base.OnMouseLeave(eventargs);
        }

        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            if (mevent.Button == MouseButtons.Left)
            {
                mouseDown = true;
                this.Invalidate();
            }
            base.OnMouseDown(mevent);
        }

        //protected override void OnMouseMove(MouseEventArgs mevent)
        //{
        //    if (mevent.Button == MouseButtons.Left)
        //    {
        //        mouseDown = true;
        //        this.Invalidate();
        //    }
        //    base.OnMouseMove(mevent);
        //}

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            if (mevent.Button == MouseButtons.Left)
            {
                mouseDown = false;
                this.Invalidate();
            }
            base.OnMouseUp(mevent);
        }

        public static void DrawCheckBox(Graphics g, Point p, CheckState Checked)
        {
            Color BoxFillColor = Color.FromArgb(33, 33, 33);
            //using (SolidBrush brush = new SolidBrush(BoxFillColor))
            //    g.DrawString(Text, Font, brush, 26, 6);

            Point pt = new Point(4, 4);
            Rectangle rect = new Rectangle(p, new Size(12, 12));

            //if (mouseEntered)
            //{
            //    if (mouseDown)
            //    {
            //        Color col = Color.FromArgb(BoxFillColor.R - 4, BoxFillColor.G - 4, BoxFillColor.B - 4);
            //        using (GraphicsPath path = RoundedRect(rect, 4))
            //        {
            //            g.FillPath(new SolidBrush(col), path);
            //        }
            //    }
            //    else
            //    {
            //        Color col = Color.FromArgb(BoxFillColor.R + 22, BoxFillColor.G + 22, BoxFillColor.B + 22);
            //        using (GraphicsPath path = RoundedRect(rect, 4))
            //        {
            //            g.FillPath(new SolidBrush(col), path);
            //        }
            //    }
            //}
            //else
            {
                using (GraphicsPath path = RoundedRect(rect, 4))
                {
                    g.FillPath(new SolidBrush(BoxFillColor), path);
                }
            }

            if (Checked == CheckState.Checked)
            {
                using (SolidBrush brush = new SolidBrush(Color.CornflowerBlue))
                using (Font wing = new Font("Wingdings", 12f))
                    g.DrawString("ü", wing, brush, p.X - 2f, p.Y - 2f);
            }
            else if (Checked == CheckState.Indeterminate)
            {
                using (GraphicsPath path = RoundedRect(rect, 4))
                {
                    g.FillPath(new SolidBrush(Color.CornflowerBlue), path);
                }
            }

            using (GraphicsPath path = RoundedRect(rect, 4))
            {
                g.DrawPath(new Pen(Color.Silver), path);
            }
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            //base.OnPaint(pevent);

            pevent.Graphics.Clear(BackColor);

            if (this.Enabled)
            {
                using (SolidBrush brush = new SolidBrush(ForeColor))
                    pevent.Graphics.DrawString(Text, Font, brush, 26, 6);

                Point pt = new Point(4, 4);
                Rectangle rect = new Rectangle(pt, new Size(16, 16));

                if (mouseEntered)
                {
                    if (mouseDown)
                    {
                        Color col = Color.FromArgb(BoxFillColor.R - 4, BoxFillColor.G - 4, BoxFillColor.B - 4);
                        using (GraphicsPath path = RoundedRect(rect, 4))
                        {
                            pevent.Graphics.FillPath(new SolidBrush(col), path);
                        }
                    }
                    else
                    {
                        Color col = Color.FromArgb(BoxFillColor.R + 22, BoxFillColor.G + 22, BoxFillColor.B + 22);
                        using (GraphicsPath path = RoundedRect(rect, 4))
                        {
                            pevent.Graphics.FillPath(new SolidBrush(col), path);
                        }
                    }
                }
                else
                {
                    using (GraphicsPath path = RoundedRect(rect, 4))
                    {
                        pevent.Graphics.FillPath(new SolidBrush(BoxFillColor), path);
                    }
                }

                if (Checked)
                {
                    using (SolidBrush brush = new SolidBrush(CheckColor))
                    using (Font wing = new Font("Wingdings", 12f))
                        pevent.Graphics.DrawString("ü", wing, brush, 3, 5);
                }

                using (GraphicsPath path = RoundedRect(rect, 4))
                {
                    pevent.Graphics.DrawPath(new Pen(BoxBorderColor), path);
                }
            }
            else
            {
                double brightnessDim = 0.4;
                double[] hsbForeColor = SimpleColorTransforms.RgBtoHsb(ForeColor);
                double[] hsBoxFillColor = SimpleColorTransforms.RgBtoHsb(BoxFillColor);
                double[] hsBoxBorderColor = SimpleColorTransforms.RgBtoHsb(BoxBorderColor);
                double[] hsCheckColor = SimpleColorTransforms.RgBtoHsb(CheckColor);

                hsbForeColor[1] = 0.0;
                hsbForeColor[2] -= brightnessDim;

                hsBoxFillColor[1] = 0.0;
                //hsBoxFillColor[2] -= brightnessDim;

                hsBoxBorderColor[1] = 0.0;
                hsBoxBorderColor[2] -= brightnessDim;

                hsCheckColor[1] = 0.0;
                hsCheckColor[2] -= brightnessDim;

                Color foreColor = SimpleColorTransforms.HsBtoRgb(hsbForeColor[0], hsbForeColor[1], hsbForeColor[2]);
                Color boxFillColor = SimpleColorTransforms.HsBtoRgb(hsBoxFillColor[0], hsBoxFillColor[1], hsBoxFillColor[2]);
                Color boxBorderColor = SimpleColorTransforms.HsBtoRgb(hsBoxBorderColor[0], hsBoxBorderColor[1], hsBoxBorderColor[2]);
                Color checkColor = SimpleColorTransforms.HsBtoRgb(hsCheckColor[0], hsCheckColor[1], hsCheckColor[2]);


                using (SolidBrush brush = new SolidBrush(foreColor))
                    pevent.Graphics.DrawString(Text, Font, brush, 26, 6);

                Point pt = new Point(4, 4);
                Rectangle rect = new Rectangle(pt, new Size(16, 16));

                pevent.Graphics.FillRectangle(new SolidBrush(boxFillColor), rect);

                if (Checked)
                {
                    using (SolidBrush brush = new SolidBrush(checkColor))
                    using (Font wing = new Font("Wingdings", 12f))
                        pevent.Graphics.DrawString("ü", wing, brush, 3, 5);
                }

                using (GraphicsPath path = RoundedRect(rect, 4))
                {
                    pevent.Graphics.DrawPath(new Pen(boxBorderColor), path);
                }
            }
        }
    }
}
