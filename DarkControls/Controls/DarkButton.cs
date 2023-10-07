using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace DarkControls.Controls
{
    public class DarkButton : Button
    {
        public DarkButton()
        {
            this.BackColor = Color.FromArgb(33, 33, 33);
            this.ForeColor = Color.Silver;

            this.Size = new Size(75, 23);
            this.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.UseVisualStyleBackColor = true;

            //this.Region = Region.FromHrgn(Utils.CreateRoundRectRgn(0, 0, Width, Height, 25, 25));
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            if (this.Enabled)
            {
                this.ForeColor = Color.Silver;
                this.Update();
                this.Invalidate();
            }
            else
            {
                this.ForeColor = Color.FromArgb(Color.Silver.R - 32, Color.Silver.G - 32, Color.Silver.B - 32);
                this.Update();
                this.Invalidate();
            }

            base.OnEnabledChanged(e);
        }
    }
}
