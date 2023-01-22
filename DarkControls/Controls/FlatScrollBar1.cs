//using System;
//// Copyright (c) Smart PC Utilities, Ltd.
//// All rights reserved.

//#region References
//using System.Windows.Forms;

//using System.ComponentModel;
//using System.Linq;

//#endregion

//namespace DarkControls.Controls
//{

//    [Designer(typeof(FlatScrollBarDesigner))]
//    [ToolboxBitmap(typeof(ScrollBar))]
//    [DefaultEvent("Scroll")]
//    [DefaultProperty("Value")]
//    public partial class FlatScrollBar : Control, ITheme
//    {

//        #region Private Members

//        private const int SETREDRAW = 11; // Redraw constant

//        private bool _isUpdating; // Indicates many changes to the scrollbar are happening, so stop painting till finished.

//        private ScrollBarOrientation _orientation = ScrollBarOrientation.Vertical; // The scrollbar orientation - horizontal / vertical.
//        private ScrollOrientation _scrollOrientation = ScrollOrientation.VerticalScroll; // The scroll orientation in scroll events.

//        private Rectangle _rectClickedBar; // The clicked channel rectangle.
//        private Rectangle _rectThumb;
//        private Rectangle _rectTopArrow;
//        private Rectangle _rectBottomArrow;
//        private Rectangle _rectChannel;

//        private bool _topArrowClicked; // Indicates if top arrow was clicked.
//        private bool _bottomArrowClicked; // Indicates if down arrow was clicked.
//        private bool _topBarClicked; // Indicates if channel rectangle above the thumb was clicked.
//        private bool _bottomBarClicked; // Indicates if channel rectangle under the thumb was clicked.
//        private bool _thumbClicked; // Indicates if the thumb was clicked.

//        private ScrollBarState _thumbState = ScrollBarState.Normal; // The state of the thumb.

//        private ScrollBarArrowButtonState _topButtonState = ScrollBarArrowButtonState.UpNormal; // The state of the top arrow.
//        private ScrollBarArrowButtonState _bottomButtonState = ScrollBarArrowButtonState.DownNormal; // The state of the bottom arrow.

//        private int _minimum;
//        private int _maximum = 100;
//        private int _smallChange = 1;
//        private int _largeChange = 10;
//        private int _value;

//        private int _thumbWidth = 15;
//        private int _thumbHeight;

//        private int _arrowWidth = 15;
//        private int _arrowHeight = 17;

//        private int _thumbBottomLimitBottom; // The bottom limit for the thumb bottom.
//        private int _thumbBottomLimitTop; // The bottom limit for the thumb top.
//        private int _thumbTopLimit; // The top limit for the thumb top.
//        private int _thumbPosition; // The current position of the thumb.

//        private int _trackPosition; // The track position.

//        private readonly Timer progressTimer = new DateAndTime.Timer(); // The progress timer for moving the thumb.

//        private UITheme _theme = UITheme.VS2019LightBlue;

//        private Color _backColor = Color.FromArgb(245, 245, 245);

//        private Color _borderColor = Color.FromArgb(245, 245, 245);
//        private Color _borderColorDisabled = Color.FromArgb(245, 245, 245);

//        private readonly Color[] _thumbColors = new Color[4];
//        private readonly Color[] _arrowColors = new Color[4];

//        #endregion

//        #region Constructor

//        public FlatScrollBar()
//        {

//            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.Selectable | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);

//            SetUpScrollBar(); // Setup the ScrollBar

//            progressTimer.Interval = 20;
//            this.progressTimer.Tick += ProgressTimerTick;

//            _thumbColors[0] = Color.FromArgb(194, 195, 201); // Normal state
//            _thumbColors[1] = Color.FromArgb(104, 104, 104); // Hover state
//            _thumbColors[2] = Color.FromArgb(91, 91, 91); // Pressed state

//            _arrowColors[0] = Color.FromArgb(134, 137, 153); // Normal state
//            _arrowColors[1] = Color.FromArgb(28, 151, 234); // Hover state
//            _arrowColors[2] = Color.FromArgb(0, 122, 204); // Pressed state

//        }

//        #endregion

//        #region Events

//        /// <summary>
//        /// Raised when the ScrollBar control is scrolled.
//        /// </summary>
//        [Category("Behavior")]
//        [Description("Raised when the ScrollBar control is scrolled.")]
//        public event ScrollEventHandler Scroll;

//        #endregion

//        #region Public Properties

//        [Category("Layout")]
//        [Description("Gets or sets the ScrollBar orientation.")]
//        [DefaultValue(ScrollBarOrientation.Vertical)]
//        public ScrollBarOrientation Orientation
//        {
//            get
//            {
//                return _orientation;
//            }
//            set
//            {

//                if (value != _orientation)
//                {

//                    _orientation = value;
//                    _scrollOrientation = value == ScrollBarOrientation.Vertical ? ScrollOrientation.VerticalScroll : ScrollOrientation.HorizontalScroll;

//                    if (DesignMode) // only in DesignMode switch width and height
//                    {
//                        Size = new Size(Height, Width);
//                    }

//                    SetUpScrollBar();

//                }

//            }
//        }

//        [Category("Behavior")]
//        [Description("Gets or sets the ScrollBar minimum value.")]
//        [DefaultValue(0)]
//        public int Minimum
//        {
//            get
//            {
//                return _minimum;
//            }
//            set
//            {

//                if (_minimum == value || value < 0 || value >= _maximum)
//                {
//                    return;
//                }

//                _minimum = value;

//                // Current value less than new minimum value - adjust
//                if (_value < value)
//                {
//                    _value = value;
//                }

//                // Current large change value invalid - adjust
//                if (_largeChange > _maximum - _minimum)
//                {
//                    _largeChange = _maximum - _minimum;
//                }

//                SetUpScrollBar();

//                if (_value < value) // Current value less than new minimum value - adjust
//                {
//                    Value = value;
//                }
//                else
//                {
//                    ChangeThumbPosition(GetThumbPosition()); // Current value is valid - adjust thumb position
//                    Refresh();
//                }

//            }
//        }

//        [Category("Behavior")]
//        [Description("Gets or sets the ScrollBar maximum value.")]
//        [DefaultValue(100)]
//        public int Maximum
//        {
//            get
//            {
//                return _maximum;
//            }
//            set
//            {

//                if (value == _maximum || value < 1 || value <= _minimum)
//                {
//                    return;
//                }

//                _maximum = value;

//                if (_largeChange > _maximum - _minimum)
//                {
//                    _largeChange = _maximum - _minimum;
//                }

//                SetUpScrollBar();

//                if (_value > _maximum)
//                {
//                    Value = _maximum;
//                }
//                else
//                {
//                    ChangeThumbPosition(GetThumbPosition());
//                    Refresh();
//                }

//            }
//        }

//        [Category("Behavior")]
//        [Description("Gets or sets the ScrollBar small change value.")]
//        [DefaultValue(1)]
//        public int SmallChange
//        {
//            get
//            {
//                return _smallChange;
//            }
//            set
//            {

//                if (value == _smallChange || value < 1 || value >= _largeChange)
//                {
//                    return;
//                }

//                _smallChange = value;
//                SetUpScrollBar();

//            }
//        }

//        [Category("Behavior")]
//        [Description("Gets or sets the ScrollBar large change value.")]
//        [DefaultValue(10)]
//        public int LargeChange
//        {
//            get
//            {
//                return _largeChange;
//            }
//            set
//            {

//                if (value == _largeChange || value < _smallChange || value < 2)
//                {
//                    return;
//                }

//                if (value > _maximum - _minimum)
//                {
//                    _largeChange = _maximum - _minimum;
//                }
//                else
//                {
//                    _largeChange = value;
//                }

//                SetUpScrollBar();

//            }
//        }

//        [Category("Behavior")]
//        [Description("Gets or sets the ScrollBar current value.")]
//        [DefaultValue(0)]
//        public int Value
//        {
//            get
//            {
//                return _value;
//            }
//            set
//            {

//                if (_value == value || value < _minimum || value > _maximum)
//                {
//                    return;
//                }

//                _value = value;
//                ChangeThumbPosition(GetThumbPosition());
//                OnScroll(new ScrollEventArgs(ScrollEventType.ThumbPosition, -1, _value, _scrollOrientation));
//                Refresh();

//            }
//        }

//        [Category("Appearance")]
//        [Description("The theme to apply to the Flat ScrollBar control.")]
//        [DefaultValue(typeof(UITheme), "1")]
//        public UITheme Theme
//        {
//            get
//            {
//                return _theme;
//            }
//            set
//            {
//                _theme = value;

//                if (_theme == UITheme.VS2019DarkBlue)
//                {

//                    _backColor = Color.FromArgb(62, 62, 66);

//                    _borderColor = Color.FromArgb(62, 62, 66);
//                    _borderColorDisabled = Color.FromArgb(62, 62, 66);

//                    _thumbColors[0] = Color.FromArgb(104, 104, 104);
//                    _thumbColors[1] = Color.FromArgb(158, 158, 158);
//                    _thumbColors[2] = Color.FromArgb(239, 235, 239);

//                    _arrowColors[0] = Color.FromArgb(153, 153, 153);
//                    _arrowColors[1] = Color.FromArgb(28, 151, 234);
//                    _arrowColors[2] = Color.FromArgb(0, 122, 204);
//                }

//                else if (_theme == UITheme.VS2019LightBlue)
//                {

//                    _backColor = Color.FromArgb(245, 245, 245);

//                    _borderColor = Color.FromArgb(245, 245, 245);
//                    _borderColorDisabled = Color.FromArgb(245, 245, 245);

//                    _thumbColors[0] = Color.FromArgb(194, 195, 201); // Normal state
//                    _thumbColors[1] = Color.FromArgb(104, 104, 104); // Hover state
//                    _thumbColors[2] = Color.FromArgb(91, 91, 91); // Pressed state

//                    _arrowColors[0] = Color.FromArgb(134, 137, 153); // Normal state
//                    _arrowColors[1] = Color.FromArgb(28, 151, 234); // Hover state
//                    _arrowColors[2] = Color.FromArgb(0, 122, 204); // Pressed state
//                }

//                else if (_theme == UITheme.Custom)
//                {
//                    ParentTheme = false;

//                }

//                Invalidate();
//            }
//        }

//        [Category("Appearance")]
//        [Description("True to allow the control to inherit the parent control style.")]
//        [DefaultValue(true)]
//        public bool ParentTheme { get; set; } = true;

//        #endregion

//        #region Public Methods

//        public void BeginUpdate()
//        {
//            NativeMethods.SendMessage(Handle, SETREDRAW, false, 0);
//            _isUpdating = true;
//        }

//        public void EndUpdate()
//        {
//            NativeMethods.SendMessage(Handle, SETREDRAW, true, 0);
//            _isUpdating = false;
//            SetUpScrollBar();
//            Refresh();
//        }

//        #endregion

//        #region Overridden Methods

//        protected virtual void OnScroll(ScrollEventArgs e)
//        {
//            ;
//#error Cannot convert RaiseEventStatementSyntax - see comment for details
//            /* Cannot convert ArgumentListSyntax, System.NullReferenceException: Object reference not set to an instance of an object.
//                           at ICSharpCode.CodeConverter.Util.SymbolExtensions.GetParameters(ISymbol symbol) in D:\GitWorkspace\CodeConverter\CodeConverter\Util\SymbolExtensions.cs:line 18
//                           at ICSharpCode.CodeConverter.CSharp.ExpressionNodeVisitor.<>c__DisplayClass127_0.<<ConvertArgumentsAsync>g__ConvertArg|0>d.MoveNext() in D:\GitWorkspace\CodeConverter\CodeConverter\CSharp\ExpressionNodeVisitor.cs:line 1585
//                        --- End of stack trace from previous location ---
//                           at ICSharpCode.CodeConverter.Common.AsyncEnumerableTaskExtensions.SelectAsync[TArg,TResult](IEnumerable`1 source, Func`2 selector) in D:\GitWorkspace\CodeConverter\CodeConverter\Common\AsyncEnumerableTaskExtensions.cs:line 0
//                           at ICSharpCode.CodeConverter.Common.AsyncEnumerableTaskExtensions.SelectAsync[TArg,TResult](IEnumerable`1 nodes, Func`3 selector) in D:\GitWorkspace\CodeConverter\CodeConverter\Common\AsyncEnumerableTaskExtensions.cs:line 0
//                           at ICSharpCode.CodeConverter.CSharp.ExpressionNodeVisitor.ConvertArgumentsAsync(ArgumentListSyntax node) in D:\GitWorkspace\CodeConverter\CodeConverter\CSharp\ExpressionNodeVisitor.cs:line 1579
//                           at ICSharpCode.CodeConverter.CSharp.ExpressionNodeVisitor.VisitArgumentList(ArgumentListSyntax node) in D:\GitWorkspace\CodeConverter\CodeConverter\CSharp\ExpressionNodeVisitor.cs:line 489
//                           at ICSharpCode.CodeConverter.CSharp.CommentConvertingVisitorWrapper.ConvertHandledAsync[T](VisualBasicSyntaxNode vbNode, SourceTriviaMapKind sourceTriviaMap) in D:\GitWorkspace\CodeConverter\CodeConverter\CSharp\CommentConvertingVisitorWrapper.cs:line 36

//                        Input: (Me, e)

//                        Context:
//                                    RaiseEvent Scroll(Me, e)

//                         */
//        }

//        protected override void OnPaint(PaintEventArgs e)
//        {

//            var g = e.Graphics;
//            g.SmoothingMode = SmoothingMode.None;

//            DrawBackground(g, ClientRectangle);
//            DrawThumb(g, _rectThumb, _thumbState);
//            DrawArrowButton(g, _rectTopArrow, _topButtonState, true, _orientation);
//            DrawArrowButton(g, _rectBottomArrow, _bottomButtonState, false, _orientation);

//            if (_topBarClicked)
//            {

//                if (_orientation == ScrollBarOrientation.Vertical)
//                {
//                    _rectClickedBar.Y = _thumbTopLimit;
//                    _rectClickedBar.Height = _rectThumb.Y - _thumbTopLimit;
//                }
//                else
//                {
//                    _rectClickedBar.X = _thumbTopLimit;
//                    _rectClickedBar.Width = _rectThumb.X - _thumbTopLimit;
//                }
//            }

//            else if (_bottomBarClicked)
//            {

//                if (_orientation == ScrollBarOrientation.Vertical)
//                {
//                    _rectClickedBar.Y = _rectThumb.Bottom + 1;
//                    _rectClickedBar.Height = _thumbBottomLimitBottom - _rectClickedBar.Y + 1;
//                }
//                else
//                {
//                    _rectClickedBar.X = _rectThumb.Right + 1;
//                    _rectClickedBar.Width = _thumbBottomLimitBottom - _rectClickedBar.X + 1;
//                }

//            }

//            using (var p = new Pen(Enabled ? _borderColor : _borderColorDisabled))
//            {
//                e.Graphics.DrawRectangle(p, 0, 0, Width - 1, Height - 1);
//            }

//        }

//        protected override void OnMouseDown(MouseEventArgs e)
//        {
//            base.OnMouseDown(e);
//            Focus();

//            if (e.Button == MouseButtons.Left)
//            {

//                Point mouseLocation = e.Location;

//                if (_rectThumb.Contains(mouseLocation))
//                {

//                    _thumbClicked = true;
//                    _thumbPosition = Conversions.ToInteger(_orientation == ScrollBarOrientation.Vertical ? mouseLocation.Y - _rectThumb.Y : mouseLocation.X - _rectThumb.X);
//                    _thumbState = ScrollBarState.Pressed;
//                    Invalidate(_rectThumb);
//                }

//                else if (_rectTopArrow.Contains(mouseLocation))
//                {

//                    _topArrowClicked = true;
//                    _topButtonState = ScrollBarArrowButtonState.UpPressed;
//                    Invalidate(_rectTopArrow);
//                    ProgressThumb(true);
//                }

//                else if (_rectBottomArrow.Contains(mouseLocation))
//                {

//                    _bottomArrowClicked = true;
//                    _bottomButtonState = ScrollBarArrowButtonState.DownPressed;
//                    Invalidate(_rectBottomArrow);
//                    ProgressThumb(true);
//                }

//                else
//                {
//                    _trackPosition = Conversions.ToInteger(_orientation == ScrollBarOrientation.Vertical ? mouseLocation.Y : mouseLocation.X);

//                    if (Conversions.ToBoolean(Operators.ConditionalCompareObjectLess(_trackPosition, _orientation == ScrollBarOrientation.Vertical ? _rectThumb.Y : _rectThumb.X, false)))
//                    {
//                        _topBarClicked = true;
//                    }
//                    else
//                    {
//                        _bottomBarClicked = true;
//                    }

//                    ProgressThumb(true);

//                }
//            }

//            else if (e.Button == MouseButtons.Right)
//            {
//                _trackPosition = Conversions.ToInteger(_orientation == ScrollBarOrientation.Vertical ? e.Y : e.X);
//            }

//        }

//        protected override void OnMouseUp(MouseEventArgs e)
//        {
//            base.OnMouseUp(e);

//            if (e.Button == MouseButtons.Left)
//            {

//                if (_thumbClicked)
//                {
//                    _thumbClicked = false;
//                    _thumbState = ScrollBarState.Normal;
//                    OnScroll(new ScrollEventArgs(ScrollEventType.EndScroll, -1, _value, _scrollOrientation));
//                }

//                else if (_topArrowClicked)
//                {
//                    _topArrowClicked = false;
//                    _topButtonState = ScrollBarArrowButtonState.UpNormal;
//                    StopTimer();
//                }

//                else if (_bottomArrowClicked)
//                {
//                    _bottomArrowClicked = false;
//                    _bottomButtonState = ScrollBarArrowButtonState.DownNormal;
//                    StopTimer();
//                }

//                else if (_topBarClicked)
//                {
//                    _topBarClicked = false;
//                    StopTimer();
//                }

//                else if (_bottomBarClicked)
//                {
//                    _bottomBarClicked = false;
//                    StopTimer();

//                }

//                Invalidate();

//            }

//        }

//        protected override void OnMouseLeave(EventArgs e)
//        {
//            base.OnMouseLeave(e);
//            ResetScrollStatus();
//        }

//        protected override void OnMouseMove(MouseEventArgs e)
//        {
//            base.OnMouseMove(e);

//            if (e.Button == MouseButtons.Left) // Moving and holding the left mouse button
//            {

//                if (_thumbClicked)
//                {

//                    int oldScrollValue = _value;

//                    int pos = Conversions.ToInteger(_orientation == ScrollBarOrientation.Vertical ? e.Location.Y : e.Location.X);

//                    if (pos <= _thumbTopLimit + _thumbPosition) // The thumb is all the way to the top
//                    {
//                        ChangeThumbPosition(_thumbTopLimit);
//                        _value = _minimum;
//                    }

//                    else if (pos >= _thumbBottomLimitTop + _thumbPosition) // The thumb is all the way to the bottom
//                    {
//                        ChangeThumbPosition(_thumbBottomLimitTop);
//                        _value = _maximum;
//                    }

//                    else // The thumb is between the ends of the track.
//                    {

//                        ChangeThumbPosition(pos - _thumbPosition);

//                        int pixelRange, thumbPos, arrowSize;

//                        // Calculate the value - first some helper variables dependent on the current orientation

//                        if (_orientation == ScrollBarOrientation.Vertical)
//                        {
//                            pixelRange = Height - 2 * _arrowHeight - _thumbHeight;
//                            thumbPos = _rectThumb.Y;
//                            arrowSize = _arrowHeight;
//                        }
//                        else
//                        {
//                            pixelRange = Width - 2 * _arrowWidth - _thumbWidth;
//                            thumbPos = _rectThumb.X;
//                            arrowSize = _arrowWidth;
//                        }

//                        float perc = 0f;

//                        if (pixelRange != 0)
//                        {
//                            perc = (thumbPos - arrowSize) / (float)pixelRange;
//                        }

//                        _value = Convert.ToInt32(perc * (_maximum - _minimum) + _minimum);

//                    }

//                    if (oldScrollValue != _value)
//                    {
//                        OnScroll(new ScrollEventArgs(ScrollEventType.ThumbTrack, oldScrollValue, _value, _scrollOrientation));
//                        Refresh();
//                    }

//                }
//            }

//            else if (!ClientRectangle.Contains(e.Location))
//            {
//                ResetScrollStatus();
//            }

//            else if (e.Button == MouseButtons.None) // Only moving the mouse
//            {

//                if (_rectTopArrow.Contains(e.Location))
//                {
//                    _topButtonState = ScrollBarArrowButtonState.UpHot;
//                    Invalidate(_rectTopArrow);
//                }

//                else if (_rectBottomArrow.Contains(e.Location))
//                {
//                    _bottomButtonState = ScrollBarArrowButtonState.DownHot;
//                    Invalidate(_rectBottomArrow);
//                }

//                else if (_rectThumb.Contains(e.Location))
//                {
//                    _thumbState = ScrollBarState.Hot;
//                    Invalidate(_rectThumb);

//                }

//            }

//        }

//        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
//        {

//            if (DesignMode)
//            {

//                if (_orientation == ScrollBarOrientation.Vertical)
//                {

//                    if (height < 2 * _arrowHeight + 10)
//                        height = 2 * _arrowHeight + 10;
//                    width = SystemInformation.VerticalScrollBarWidth;
//                }

//                else
//                {

//                    if (width < 2 * _arrowWidth + 10)
//                        width = 2 * _arrowWidth + 10;
//                    height = SystemInformation.VerticalScrollBarWidth;

//                }

//            }

//            base.SetBoundsCore(x, y, width, height, specified);

//            if (DesignMode)
//                SetUpScrollBar();

//        }

//        protected override void OnSizeChanged(EventArgs e)
//        {
//            base.OnSizeChanged(e);
//            SetUpScrollBar();
//        }

//        protected override bool ProcessDialogKey(Keys keyData)
//        {

//            Keys keyUp = Keys.Up;
//            Keys keyDown = Keys.Down;

//            if (_orientation == ScrollBarOrientation.Horizontal)
//            {
//                keyUp = Keys.Left;
//                keyDown = Keys.Right;
//            }

//            if (keyData == keyUp)
//            {
//                Value -= _smallChange;
//                return true;
//            }

//            if (keyData == keyDown)
//            {
//                Value += _smallChange;
//                return true;
//            }

//            if (keyData == Keys.PageUp)
//            {
//                Value = GetValue(false, true);
//                return true;
//            }

//            if (keyData == Keys.PageDown)
//            {

//                if (Value + _largeChange > _maximum)
//                {
//                    Value = _maximum;
//                }
//                else
//                {
//                    Value += _largeChange;
//                }

//                return true;

//            }

//            if (keyData == Keys.Home)
//            {
//                Value = _minimum;
//                return true;
//            }

//            if (keyData == Keys.End)
//            {
//                Value = _maximum;
//                return true;
//            }

//            return base.ProcessDialogKey(keyData);

//        }

//        protected override void OnEnabledChanged(EventArgs e)
//        {
//            base.OnEnabledChanged(e);

//            if (Enabled)
//            {
//                _thumbState = ScrollBarState.Normal;
//                _topButtonState = ScrollBarArrowButtonState.UpNormal;
//                _bottomButtonState = ScrollBarArrowButtonState.DownNormal;
//            }
//            else
//            {
//                _thumbState = ScrollBarState.Disabled;
//                _topButtonState = ScrollBarArrowButtonState.UpDisabled;
//                _bottomButtonState = ScrollBarArrowButtonState.DownDisabled;
//            }

//            Refresh();

//        }

//        #endregion

//        #region Overriden Properties

//        protected override Size DefaultSize
//        {
//            get
//            {
//                return new Size(SystemInformation.VerticalScrollBarWidth, 200);
//            }
//        }

//        #endregion

//        #region Private Methods

//        private void SetUpScrollBar()
//        {

//            if (_isUpdating)
//                return;

//            if (_orientation == ScrollBarOrientation.Vertical)
//            {
//                _arrowHeight = 17;
//                _arrowWidth = 15;
//                _thumbWidth = 13;
//                _thumbHeight = GetThumbSize();
//                _rectClickedBar = ClientRectangle;
//                _rectClickedBar.Inflate(-1, -1);
//                _rectClickedBar.Y += _arrowHeight;
//                _rectClickedBar.Height -= _arrowHeight * 2;
//                _rectChannel = _rectClickedBar;
//                _rectThumb = new Rectangle(ClientRectangle.Right / 2 - (int)Math.Round(_thumbWidth / 2d), ClientRectangle.Y + _arrowHeight + 1, _thumbWidth, _thumbHeight);
//                _rectTopArrow = new Rectangle(ClientRectangle.Right / 2 - (int)Math.Round(_arrowWidth / 2d) + 1, ClientRectangle.Y + 1, _arrowWidth, _arrowHeight);
//                _rectBottomArrow = new Rectangle(ClientRectangle.Right / 2 - (int)Math.Round(_arrowWidth / 2d), ClientRectangle.Bottom - _arrowHeight - 1, _arrowWidth, _arrowHeight);
//                _thumbPosition = _rectThumb.Height / 2;
//                _thumbBottomLimitBottom = ClientRectangle.Bottom - _arrowHeight - 2;
//                _thumbBottomLimitTop = _thumbBottomLimitBottom - _rectThumb.Height;
//                _thumbTopLimit = ClientRectangle.Y + _arrowHeight + 2;
//            }
//            else
//            {
//                _arrowHeight = 15;
//                _arrowWidth = 17;
//                _thumbHeight = 13;
//                _thumbWidth = GetThumbSize();
//                _rectClickedBar = ClientRectangle;
//                _rectClickedBar.Inflate(-1, -1);
//                _rectClickedBar.X += _arrowWidth;
//                _rectClickedBar.Width -= _arrowWidth * 2;
//                _rectChannel = _rectClickedBar;
//                _rectThumb = new Rectangle(ClientRectangle.X + _arrowWidth + 1, ClientRectangle.Bottom / 2 - (int)Math.Round(_thumbHeight / 2d), _thumbWidth, _thumbHeight);
//                _rectTopArrow = new Rectangle(ClientRectangle.X + 2, ClientRectangle.Bottom / 2 - (int)Math.Round(_arrowHeight / 2d) + 1, _arrowWidth, _arrowHeight);
//                _rectBottomArrow = new Rectangle(ClientRectangle.Right - _arrowWidth - 2, ClientRectangle.Bottom / 2 - (int)Math.Round(_arrowHeight / 2d), _arrowWidth, _arrowHeight);
//                _thumbPosition = _rectThumb.Width / 2;
//                _thumbBottomLimitBottom = ClientRectangle.Right - _arrowWidth - 3;
//                _thumbBottomLimitTop = _thumbBottomLimitBottom - _rectThumb.Width;
//                _thumbTopLimit = ClientRectangle.X + _arrowWidth + 3;
//            }

//            ChangeThumbPosition(GetThumbPosition());
//            Refresh();

//        }

//        private void ResetScrollStatus()
//        {

//            Point pos = PointToClient(Cursor.Position);

//            if (ClientRectangle.Contains(pos))
//            {

//                if (_rectThumb.Contains(pos))
//                {

//                    _thumbState = ScrollBarState.Hot;
//                    _topButtonState = ScrollBarArrowButtonState.UpNormal;
//                    _bottomButtonState = ScrollBarArrowButtonState.DownNormal;
//                }

//                else if (_rectTopArrow.Contains(pos))
//                {

//                    _thumbState = ScrollBarState.Normal;
//                    _topButtonState = ScrollBarArrowButtonState.UpActive;
//                    _bottomButtonState = ScrollBarArrowButtonState.DownNormal;
//                }

//                else if (_rectBottomArrow.Contains(pos))
//                {

//                    _thumbState = ScrollBarState.Normal;
//                    _topButtonState = ScrollBarArrowButtonState.UpNormal;
//                    _bottomButtonState = ScrollBarArrowButtonState.DownActive;
//                }

//                else
//                {
//                    _thumbState = ScrollBarState.Normal;
//                    _topButtonState = ScrollBarArrowButtonState.UpNormal;
//                    _bottomButtonState = ScrollBarArrowButtonState.DownNormal;
//                }
//            }

//            else
//            {
//                _thumbState = ScrollBarState.Normal;
//                _topButtonState = ScrollBarArrowButtonState.UpNormal;
//                _bottomButtonState = ScrollBarArrowButtonState.DownNormal;
//            }

//            _topArrowClicked = false;
//            _bottomArrowClicked = false;
//            _topBarClicked = false;
//            _bottomBarClicked = false;

//            StopTimer();
//            Refresh();

//        }

//        private int GetValue(bool smallIncrement, bool up)
//        {

//            int newValue;

//            if (up)
//            {

//                newValue = _value - (smallIncrement ? _smallChange : _largeChange);
//                if (newValue < _minimum)
//                    newValue = _minimum;
//            }

//            else
//            {

//                newValue = _value + (smallIncrement ? _smallChange : _largeChange);
//                if (newValue > _maximum)
//                    newValue = _maximum;

//            }

//            return newValue;

//        }

//        private int GetThumbPosition()
//        {

//            int pixelRange, arrowSize;

//            if (_orientation == ScrollBarOrientation.Vertical)
//            {
//                pixelRange = Height;
//                arrowSize = _arrowHeight;
//            }
//            else
//            {
//                pixelRange = Width;
//                arrowSize = _arrowWidth;
//            }

//            int realRange = _maximum - _minimum;
//            float perc = 0f;

//            if (realRange != 0)
//            {
//                perc = (_value - (float)_minimum) / realRange;
//            }

//            return Math.Max(_thumbTopLimit, Math.Min(_thumbBottomLimitTop, Convert.ToInt32(perc * pixelRange + arrowSize)));

//        }

//        private int GetThumbSize()
//        {

//            int trackSize = Conversions.ToInteger(_orientation == ScrollBarOrientation.Vertical ? Height - 2 * _arrowHeight : Width - 2 * _arrowWidth);

//            if (_maximum == 0 || _largeChange == 0)
//            {
//                return trackSize;
//            }

//            float newThumbSize = _largeChange * (float)trackSize / _maximum;

//            return Convert.ToInt32(Math.Min(trackSize, Math.Max(newThumbSize, 10.0f)));

//        }

//        private void ChangeThumbPosition(int position)
//        {

//            if (_orientation == ScrollBarOrientation.Vertical)
//            {
//                _rectThumb.Y = position;
//            }
//            else
//            {
//                _rectThumb.X = position;
//            }

//        }

//        private void ProgressThumb(bool enableTimer)
//        {

//            int scrollOldValue = _value;
//            ScrollEventType type = ScrollEventType.First;
//            int thumbSize, thumbPos;

//            if (_orientation == ScrollBarOrientation.Vertical)
//            {
//                thumbPos = _rectThumb.Y;
//                thumbSize = _rectThumb.Height;
//            }
//            else
//            {
//                thumbPos = _rectThumb.X;
//                thumbSize = _rectThumb.Width;
//            }

//            if (_bottomArrowClicked || _bottomBarClicked && thumbPos + thumbSize < _trackPosition)
//            {

//                type = _bottomArrowClicked ? ScrollEventType.SmallIncrement : ScrollEventType.LargeIncrement;
//                _value = GetValue(_bottomArrowClicked, false);

//                if (_value == _maximum)
//                {
//                    ChangeThumbPosition(_thumbBottomLimitTop);
//                    type = ScrollEventType.Last;
//                }
//                else
//                {
//                    ChangeThumbPosition(Math.Min(_thumbBottomLimitTop, GetThumbPosition()));
//                }
//            }

//            else if (_topArrowClicked || _topBarClicked && thumbPos > _trackPosition)
//            {

//                type = _topArrowClicked ? ScrollEventType.SmallDecrement : ScrollEventType.LargeDecrement;
//                _value = GetValue(_topArrowClicked, true);

//                if (_value == _minimum)
//                {
//                    ChangeThumbPosition(_thumbTopLimit);
//                    type = ScrollEventType.First;
//                }
//                else
//                {
//                    ChangeThumbPosition(Math.Max(_thumbTopLimit, GetThumbPosition()));
//                }
//            }

//            else if (!(_topArrowClicked && thumbPos == _thumbTopLimit || _bottomArrowClicked && thumbPos == _thumbBottomLimitTop))
//            {

//                ResetScrollStatus();
//                return;

//            }

//            if (scrollOldValue != _value)
//            {

//                OnScroll(new ScrollEventArgs(type, scrollOldValue, _value, _scrollOrientation));
//                Invalidate(_rectChannel);

//                if (enableTimer)
//                    StartTimer();
//            }

//            else
//            {

//                if (_topArrowClicked)
//                {
//                    type = ScrollEventType.SmallDecrement;
//                }
//                else if (_bottomArrowClicked)
//                {
//                    type = ScrollEventType.SmallIncrement;
//                }

//                OnScroll(new ScrollEventArgs(type, _value));

//            }

//        }

//        #endregion

//        #region Timer Methods

//        private void ProgressTimerTick(object sender, EventArgs e)
//        {
//            ProgressThumb(true);
//        }

//        private void StartTimer()
//        {

//            if (!progressTimer.Enabled)
//            {
//                progressTimer.Interval = 600;
//                progressTimer.Start();
//            }
//            else
//            {
//                progressTimer.Interval = 10;
//            }

//        }

//        private void StopTimer()
//        {
//            progressTimer.Stop();
//        }

//        #endregion

//        #region Drawing Methods

//        private void DrawBackground(Graphics g, Rectangle rect)
//        {

//            if (g is null || rect.IsEmpty || g.IsVisibleClipEmpty || !g.VisibleClipBounds.IntersectsWith(rect))
//                return;

//            using (var sb = new SolidBrush(_backColor))
//            {
//                g.FillRectangle(sb, rect);
//            }

//        }

//        private void DrawThumb(Graphics g, Rectangle rect, ScrollBarState state)
//        {

//            if (g is null || rect.IsEmpty || g.IsVisibleClipEmpty || !g.VisibleClipBounds.IntersectsWith(rect) || state == ScrollBarState.Disabled)
//            {
//                return;
//            }

//            int index = 0;

//            switch (state)
//            {

//                case ScrollBarState.Hot:
//                {
//                    index = 1;
//                    break;
//                }

//                case ScrollBarState.Pressed:
//                {
//                    index = 2;
//                    break;
//                }

//            }

//            using (var sb = new SolidBrush(_thumbColors[index]))
//            {
//                g.FillRectangle(sb, rect);
//            }

//        }

//        private void DrawArrowButton(Graphics g, Rectangle rect, ScrollBarArrowButtonState state, bool arrowUp, ScrollBarOrientation orientation)
//        {

//            if (g is null || rect.IsEmpty || g.IsVisibleClipEmpty || !g.VisibleClipBounds.IntersectsWith(rect))
//            {
//                return;
//            }

//            if (orientation == ScrollBarOrientation.Vertical)
//            {
//                DrawArrowButtonVertical(g, rect, state, arrowUp);
//            }
//            else
//            {
//                DrawArrowButtonHorizontal(g, rect, state, arrowUp);
//            }

//        }

//        private void DrawArrowButtonVertical(Graphics g, Rectangle rect, ScrollBarArrowButtonState state, bool arrowUp)
//        {

//            using (var img = GetArrowDownButtonImage(state))
//            {

//                if (arrowUp)
//                    img.RotateFlip(RotateFlipType.Rotate180FlipNone);
//                g.DrawImage(img, rect);

//            }

//        }

//        private void DrawArrowButtonHorizontal(Graphics g, Rectangle rect, ScrollBarArrowButtonState state, bool arrowUp)
//        {

//            using (var img = GetArrowDownButtonImage(state))
//            {

//                if (arrowUp)
//                {
//                    img.RotateFlip(RotateFlipType.Rotate90FlipNone);
//                }
//                else
//                {
//                    img.RotateFlip(RotateFlipType.Rotate270FlipNone);
//                }

//                g.DrawImage(img, rect);

//            }

//        }

//        private Image GetArrowDownButtonImage(ScrollBarArrowButtonState state)
//        {

//            var rect = new Rectangle(0, 0, _arrowWidth, _arrowHeight);
//            var bitmap = new Bitmap(_arrowWidth, _arrowHeight, PixelFormat.Format32bppArgb);

//            using (Graphics g = Graphics.FromImage(bitmap))
//            {

//                g.SmoothingMode = SmoothingMode.None;
//                g.InterpolationMode = InterpolationMode.HighQualityBicubic;

//                int index = 0;

//                switch (state)
//                {

//                    case ScrollBarArrowButtonState.UpHot:
//                    case ScrollBarArrowButtonState.DownHot:
//                    {
//                        index = 1;
//                        break;
//                    }

//                    case ScrollBarArrowButtonState.UpActive:
//                    case ScrollBarArrowButtonState.DownActive:
//                    {
//                        index = 1;
//                        break;
//                    }

//                    case ScrollBarArrowButtonState.UpPressed:
//                    case ScrollBarArrowButtonState.DownPressed:
//                    {
//                        index = 2;
//                        break;
//                    }

//                }

//                using (var sb = new SolidBrush(_arrowColors[index]))
//                {
//                    g.FillPolygon(sb, GetDownArrow(rect));
//                }

//            }

//            return bitmap;

//        }

//        private static Point[] GetDownArrow(Rectangle r)
//        {

//            var middle = new Point(r.Left + r.Width / 2 - 1, r.Top + r.Height / 2 + 1);
//            return new[] { new Point(middle.X - 3, middle.Y - 2), new Point(middle.X + 4, middle.Y - 2), new Point(middle.X, middle.Y + 2) };

//        }

//        #endregion

//        #region Enumerations

//        private enum ScrollBarArrowButtonState
//        {

//            /// <summary>
//            /// Indicates the up arrow is in normal state.
//            /// </summary>
//            UpNormal,

//            /// <summary>
//            /// Indicates the up arrow is in hot state.
//            /// </summary>
//            UpHot,

//            /// <summary>
//            /// Indicates the up arrow is in active state.
//            /// </summary>
//            UpActive,

//            /// <summary>
//            /// Indicates the up arrow is in pressed state.
//            /// </summary>
//            UpPressed,

//            /// <summary>
//            /// Indicates the up arrow is in disabled state.
//            /// </summary>
//            UpDisabled,

//            /// <summary>
//            /// Indicates the down arrow is in normal state.
//            /// </summary>
//            DownNormal,

//            /// <summary>
//            /// Indicates the down arrow is in hot state.
//            /// </summary>
//            DownHot,

//            /// <summary>
//            /// Indicates the down arrow is in active state.
//            /// </summary>
//            DownActive,

//            /// <summary>
//            /// Indicates the down arrow is in pressed state.
//            /// </summary>
//            DownPressed,

//            /// <summary>
//            /// Indicates the down arrow is in disabled state.
//            /// </summary>
//            DownDisabled

//        }

//        private enum ScrollBarState
//        {

//            /// <summary>
//            /// Indicates a normal scrollbar state.
//            /// </summary>
//            Normal,

//            /// <summary>
//            /// Indicates a hot scrollbar state.
//            /// </summary>
//            Hot,

//            /// <summary>
//            /// Indicates an active scrollbar state.
//            /// </summary>
//            Active,

//            /// <summary>
//            /// Indicates a pressed scrollbar state.
//            /// </summary>
//            Pressed,

//            /// <summary>
//            /// Indicates a disabled scrollbar state.
//            /// </summary>
//            Disabled

//        }

//        #endregion

//    }

//}