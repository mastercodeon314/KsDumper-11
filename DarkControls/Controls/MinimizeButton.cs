using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DarkControls.Controls
{
    public class MinimizeButton : WindowsDefaultTitleBarButton
    {
        public MinimizeButton()
        {
            this.ButtonType = DarkControls.Controls.WindowsDefaultTitleBarButton.Type.Minimize;
            this.ClickColor = System.Drawing.Color.DodgerBlue;
            this.ClickIconColor = System.Drawing.Color.Black;
            this.HoverColor = System.Drawing.Color.SkyBlue;
            this.HoverIconColor = System.Drawing.Color.Black;
            this.IconColor = System.Drawing.Color.Black;
            this.IconLineThickness = 2;
            this.Size = new System.Drawing.Size(40, 40);
            this.UseVisualStyleBackColor = true;
        }
    }
}
