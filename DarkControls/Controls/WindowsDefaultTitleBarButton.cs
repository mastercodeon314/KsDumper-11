using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace DarkControls.Controls
{
    /// <summary>
    /// Button which represents the default close, minimize or maximize buttons of the windows 10 aero theme.
    /// </summary>
    [ToolboxItem(true)]
    public class WindowsDefaultTitleBarButton : NoFocusCueBotton
    {
        /// <summary>
        /// Represents the 3 possible types of the windows border buttons.
        /// </summary>
        public enum Type
        {
            Close,
            Maximize,
            Minimize
        }

        private Pen activeIconColorPen;
        private Brush activeIconColorBrush;
        private Brush activeColorBrush;

        /// <summary>
        /// The type which defines the buttons behaviour.
        /// </summary>

        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Browsable(true)]
        [Category("Appearance")]
        [Description("The type which defines the buttons behaviour.")]
        public Type ButtonType { get; set; }

        /// <summary>
        /// The background color of the button when the mouse is inside the buttons bounds.
        /// </summary>

        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Browsable(true)]
        [Category("Appearance")]
        [Description("The background color of the button when the mouse is inside the buttons bounds.")]
        public Color HoverColor { get; set; }

        /// <summary>
        /// The background color of the button when the button is clicked.
        /// </summary>

        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Browsable(true)]
        [Category("Appearance")]
        [Description("The background color of the button when the button is clicked.")]
        public Color ClickColor { get; set; }

        /// <summary>
        /// The default color of the icon.
        /// </summary>

        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Browsable(true)]
        [Category("Appearance")]
        [Description("The default color of the icon.")]
        public Color IconColor { get; set; }

        /// <summary>
        /// The color of the icon when the mouse is inside the buttons bounds.
        /// </summary>

        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Browsable(true)]
        [Category("Appearance")]
        [Description("The color of the icon when the mouse is inside the buttons bounds.")]
        public Color HoverIconColor { get; set; }

        /// <summary>
        /// The color of the icon when the mouse is inside the buttons bounds.
        /// </summary>

        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue("2")]
        [Description("The thickness of the lines making up the icon")]
        public int IconLineThickness { get; set; }

        /// <summary>
        /// The color of the icon when the button is clicked.
        /// </summary>

        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Browsable(true)]
        [Category("Appearance")]
        [Description("The color of the icon when the button is clicked.")]
        public Color ClickIconColor { get; set; }

        /// <summary>
        /// Property which returns the active background color of the button depending on if the button is clicked or hovered.
        /// </summary>
        ///

        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Browsable(true)]
        public virtual Color ActiveColor
        {
            get
            {
                if (this.DesignMode == false)
                {
                    if (this.Clicked)
                        return this.ClickColor;

                    if (this.Hovered)
                        return this.HoverColor;

                    return BackColor;
                }
                else
                {
                    return this.HoverColor;
/*                    switch (this.ButtonType)
                    {
                        case Type.Close:
                        {
                            return Color.Red;
                        }
                        case Type.Maximize:
                        {
                            return Color.SkyBlue;
                        }
                        case Type.Minimize:
                        {
                            return Color.SkyBlue;
                        }
                    }*/
                }
                return Color.Empty;
            }
        }

        /// <summary>
        /// Property which returns the active color of the buttons icon depending on if the button is clicked or hovered.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public virtual Color ActiveIconColor
        {
            get
            {
                if (this.DesignMode == false)
                {
                    if (this.Clicked)
                        return this.ClickIconColor;

                    if (this.Hovered)
                        return this.HoverIconColor;

                    return IconColor;
                }
                else
                {
                    return Color.Black;
                }
            }
        }

        /// <summary>
        /// Property which indicates if the mouse is currently inside the bounds of the button.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        [DefaultValue(false)]
        public bool Hovered { get; set; }

        /// <summary>
        /// Property which indicates if the left mouse button was pressed down inside the buttons bounds. Can be true before the click event is triggered.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        [DefaultValue(false)]
        public bool Clicked { get; set; }

        public WindowsDefaultTitleBarButton() { }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            Hovered = true;
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            Hovered = false;
        }

        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            base.OnMouseDown(mevent);
            Clicked = true;
        }

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            base.OnMouseUp(mevent);
            Clicked = false;
        }

        protected override void OnClick(EventArgs e)
        {
            if (ButtonType == Type.Close)
            {
                Form frm = this.FindForm();
                if (frm != null)
                {
                    if (frm.AcceptButton != null)
                    {
                        frm.DialogResult = DialogResult.OK;
                        frm.Dispose();
                    }
                    else
                    {
                        frm.Dispose();
                    }
                }
            }

            else if (ButtonType == Type.Maximize)
                this.FindForm().WindowState = this.FindForm().WindowState == FormWindowState.Maximized ? FormWindowState.Normal : FormWindowState.Maximized;
            else
                this.FindForm().WindowState = FormWindowState.Minimized;

            base.OnClick(e);
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            System.Diagnostics.Trace.WriteLine(pevent.ClipRectangle.ToString());

            activeColorBrush?.Dispose();
            activeColorBrush = new SolidBrush(ActiveColor);

            pevent.Graphics.FillRectangle(new SolidBrush(ActiveColor), pevent.ClipRectangle);
            pevent.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            activeIconColorBrush?.Dispose();
            activeIconColorPen?.Dispose();

            activeIconColorBrush = new SolidBrush(ActiveIconColor);
            activeIconColorPen = new Pen(activeIconColorBrush, IconLineThickness);

            if (ButtonType == Type.Close)
            {
                drawCloseIcon(pevent, new Rectangle(0, 0, this.Width, this.Height));
            }
            else if (ButtonType == Type.Maximize)
            {
                drawMaximizeIcon(pevent, new Rectangle(0, 0, this.Width, this.Height));
            }
            else
            {
                drawMinimizeIcon(pevent, new Rectangle(0, 0, this.Width, this.Height));
            }
        }

        protected virtual void drawCloseIcon(PaintEventArgs e, Rectangle drawRect)
        {
            int size = 12;
            e.Graphics.DrawLine(
                activeIconColorPen,
                drawRect.X + drawRect.Width / 2 - (size / 2),
                drawRect.Y + drawRect.Height / 2 - (size / 2),
                drawRect.X + drawRect.Width / 2 + (size / 2),
                drawRect.Y + drawRect.Height / 2 + (size / 2));

            e.Graphics.DrawLine(
                activeIconColorPen,
                drawRect.X + drawRect.Width / 2 - (size / 2),
                drawRect.Y + drawRect.Height / 2 + (size / 2),
                drawRect.X + drawRect.Width / 2 + (size / 2),
                drawRect.Y + drawRect.Height / 2 - (size / 2));
        }

        protected virtual void drawMaximizeIcon(PaintEventArgs e, Rectangle drawRect)
        {
            if (this.FindForm().WindowState == FormWindowState.Normal)
            {
                int size = 10;
                Rectangle rect = new Rectangle(
                        drawRect.X + drawRect.Width / 2 - (size / 2),
                        drawRect.Y + drawRect.Height / 2 - (size / 2),
                        size, size);
                Rectangle r2 = new Rectangle(rect.X, rect.Y, rect.Width, 2);
                e.Graphics.DrawRectangle(activeIconColorPen, rect);

                e.Graphics.FillRectangle(new SolidBrush(activeIconColorPen.Color), r2);
            }
            else if (this.FindForm().WindowState == FormWindowState.Maximized)
            {
                e.Graphics.DrawRectangle(
                    activeIconColorPen,
                    new Rectangle(
                        drawRect.X + drawRect.Width / 2 - 3,
                        drawRect.Y + drawRect.Height / 2 - 5,
                        8, 8));

                Rectangle rect = new Rectangle(
                    drawRect.X + drawRect.Width / 2 - 5,
                    drawRect.Y + drawRect.Height / 2 - 3,
                    8, 8);

                e.Graphics.FillRectangle(activeIconColorBrush, rect);
                e.Graphics.DrawRectangle(activeIconColorPen, rect);
            }
        }

        protected virtual void drawMinimizeIcon(PaintEventArgs e, Rectangle drawRect)
        {
            int lower = 3;
            e.Graphics.DrawLine(
                activeIconColorPen,
                drawRect.X + drawRect.Width / 2 - 5,
                drawRect.Y + drawRect.Height / 2 + lower,
                drawRect.X + drawRect.Width / 2 + 5,
                drawRect.Y + drawRect.Height / 2 + lower);
        }
    }
}