using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Windows.Forms.VisualStyles;
using System.Diagnostics;

namespace DarkControls.Controls
{
    public class DarkTextBox : System.Windows.Forms.TextBox
    {
        public DarkTextBox()
        {
            // Initialize the renderer

            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            this.ForeColor = Color.Silver;

            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //Debugger.Break();
            if (ScrollBarRenderer.IsSupported)
            {
                Debugger.Break();
                // Draw the custom scrollbar
                ScrollBarRenderer.DrawUpperVerticalTrack(e.Graphics, new Rectangle(this.Right - 18, this.Top, 18, this.Height), ScrollBarState.Normal);
                ScrollBarRenderer.DrawLowerVerticalTrack(e.Graphics, new Rectangle(this.Right - 18, this.Top, 18, this.Height), ScrollBarState.Normal);
                ScrollBarRenderer.DrawVerticalThumb(e.Graphics, new Rectangle(this.Right - 18, this.Top, 18, this.Height), ScrollBarState.Normal);
                ScrollBarRenderer.DrawVerticalThumbGrip(e.Graphics, new Rectangle(this.Right - 18, this.Top, 18, this.Height), ScrollBarState.Normal);
            }

            base.OnPaint(e);
        }

        //protected override void WndProc(ref Message m)
        //{
        //    base.WndProc(ref m);
        //    if (m.Msg == 0x00F7)
        //    {
        //        m.Result = (IntPtr)1;
        //    }
        //}
    }
}
