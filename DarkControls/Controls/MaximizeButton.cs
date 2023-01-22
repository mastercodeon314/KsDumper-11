using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DarkControls.Controls
{
    public class MaximizeButton : WindowsDefaultTitleBarButton
    {
        public MaximizeButton()
        {
            this.ButtonType = DarkControls.Controls.WindowsDefaultTitleBarButton.Type.Maximize;
            this.ClickColor = System.Drawing.Color.Red;
            this.ClickIconColor = System.Drawing.Color.Black;
            this.HoverColor = System.Drawing.Color.OrangeRed;
            this.HoverIconColor = System.Drawing.Color.Black;
            this.IconColor = System.Drawing.Color.Black;
            this.IconLineThickness = 2;
            this.Size = new System.Drawing.Size(40, 40);
            this.UseVisualStyleBackColor = true;
        }
    }
}
