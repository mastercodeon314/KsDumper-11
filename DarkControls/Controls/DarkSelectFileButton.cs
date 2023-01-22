using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace DarkControls.Controls
{
    public class DarkSelectFileButton : Button
    {
        public DarkSelectFileButton()
        {
            this.BackColor = Color.FromArgb(33, 33, 33);
            this.ForeColor = Color.Silver;
            this.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.FlatAppearance.BorderSize = 0;
            this.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Silver;
            this.Image = Properties.Resources.selectFileBtn_Image;
            this.Size = new Size(75, 23);
            this.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.UseVisualStyleBackColor = true;
        }
    }
}
