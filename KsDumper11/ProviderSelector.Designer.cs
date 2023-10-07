namespace KsDumper11
{
    partial class ProviderSelector
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProviderSelector));
            this.transparentLabel1 = new DarkControls.Controls.TransparentLabel();
            this.closeBtn = new DarkControls.Controls.WindowsDefaultTitleBarButton();
            this.appIcon1 = new DarkControls.Controls.AppIcon();
            this.providerList = new DarkControls.Controls.DarkListView();
            this.provIdCol = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.provNameCol = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.providerExtraInfoBox = new DarkControls.Controls.DarkTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.driverNameBox = new DarkControls.Controls.DarkTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.deviceNameBox = new DarkControls.Controls.DarkTextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.signerNameBox = new DarkControls.Controls.DarkTextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.minWinBuildBox = new DarkControls.Controls.DarkTextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.maxWinBuildBox = new DarkControls.Controls.DarkTextBox();
            this.driverWhqlSignedBox = new DarkControls.Controls.DarkCheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.shellcodeMaskBox = new DarkControls.Controls.DarkTextBox();
            this.testProviderBtn = new DarkControls.Controls.DarkButton();
            this.driverLoadedLbl = new System.Windows.Forms.Label();
            this.driverLoadedLblTimer = new System.Windows.Forms.Timer(this.components);
            this.setDefaultProviderBtn = new DarkControls.Controls.DarkButton();
            this.label8 = new System.Windows.Forms.Label();
            this.defaultProviderIDBox = new DarkControls.Controls.DarkTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.appIcon1)).BeginInit();
            this.SuspendLayout();
            // 
            // transparentLabel1
            // 
            this.transparentLabel1.AutoSize = true;
            this.transparentLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.transparentLabel1.Location = new System.Drawing.Point(32, 4);
            this.transparentLabel1.Name = "transparentLabel1";
            this.transparentLabel1.Size = new System.Drawing.Size(237, 20);
            this.transparentLabel1.TabIndex = 8;
            this.transparentLabel1.Text = "KsDumper 11 Provider Selection";
            // 
            // closeBtn
            // 
            this.closeBtn.ButtonType = DarkControls.Controls.WindowsDefaultTitleBarButton.Type.Close;
            this.closeBtn.ClickColor = System.Drawing.Color.Red;
            this.closeBtn.ClickIconColor = System.Drawing.Color.Black;
            this.closeBtn.HoverColor = System.Drawing.Color.OrangeRed;
            this.closeBtn.HoverIconColor = System.Drawing.Color.Black;
            this.closeBtn.IconColor = System.Drawing.Color.Black;
            this.closeBtn.IconLineThickness = 2;
            this.closeBtn.Location = new System.Drawing.Point(773, 0);
            this.closeBtn.Name = "closeBtn";
            this.closeBtn.Size = new System.Drawing.Size(40, 40);
            this.closeBtn.TabIndex = 7;
            this.closeBtn.Text = "windowsDefaultTitleBarButton1";
            this.closeBtn.UseVisualStyleBackColor = true;
            // 
            // appIcon1
            // 
            this.appIcon1.AppIconImage = ((System.Drawing.Image)(resources.GetObject("appIcon1.AppIconImage")));
            this.appIcon1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            this.appIcon1.DragForm = null;
            this.appIcon1.Image = ((System.Drawing.Image)(resources.GetObject("appIcon1.Image")));
            this.appIcon1.Location = new System.Drawing.Point(0, 1);
            this.appIcon1.Name = "appIcon1";
            this.appIcon1.Scale = 3.5F;
            this.appIcon1.Size = new System.Drawing.Size(28, 28);
            this.appIcon1.TabIndex = 9;
            this.appIcon1.TabStop = false;
            // 
            // providerList
            // 
            this.providerList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            this.providerList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.provIdCol,
            this.provNameCol});
            this.providerList.ForeColor = System.Drawing.Color.Silver;
            this.providerList.FullRowSelect = true;
            this.providerList.HideSelection = false;
            this.providerList.Location = new System.Drawing.Point(0, 35);
            this.providerList.Name = "providerList";
            this.providerList.OwnerDraw = true;
            this.providerList.Size = new System.Drawing.Size(400, 399);
            this.providerList.TabIndex = 10;
            this.providerList.UseCompatibleStateImageBehavior = false;
            this.providerList.View = System.Windows.Forms.View.Details;
            this.providerList.SelectedIndexChanged += new System.EventHandler(this.providerList_SelectedIndexChanged);
            // 
            // provIdCol
            // 
            this.provIdCol.Text = "ID";
            // 
            // provNameCol
            // 
            this.provNameCol.Text = "Provider Name";
            this.provNameCol.Width = 396;
            // 
            // providerExtraInfoBox
            // 
            this.providerExtraInfoBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            this.providerExtraInfoBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.providerExtraInfoBox.ForeColor = System.Drawing.Color.Silver;
            this.providerExtraInfoBox.Location = new System.Drawing.Point(406, 354);
            this.providerExtraInfoBox.Multiline = true;
            this.providerExtraInfoBox.Name = "providerExtraInfoBox";
            this.providerExtraInfoBox.Size = new System.Drawing.Size(397, 80);
            this.providerExtraInfoBox.TabIndex = 11;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(406, 338);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(94, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "Provider Extra Info";
            // 
            // driverNameBox
            // 
            this.driverNameBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            this.driverNameBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.driverNameBox.ForeColor = System.Drawing.Color.Silver;
            this.driverNameBox.Location = new System.Drawing.Point(409, 49);
            this.driverNameBox.Name = "driverNameBox";
            this.driverNameBox.Size = new System.Drawing.Size(394, 20);
            this.driverNameBox.TabIndex = 13;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(406, 33);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 13);
            this.label2.TabIndex = 14;
            this.label2.Text = "Driver Name";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(406, 72);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(72, 13);
            this.label3.TabIndex = 16;
            this.label3.Text = "Device Name";
            // 
            // deviceNameBox
            // 
            this.deviceNameBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            this.deviceNameBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.deviceNameBox.ForeColor = System.Drawing.Color.Silver;
            this.deviceNameBox.Location = new System.Drawing.Point(409, 88);
            this.deviceNameBox.Name = "deviceNameBox";
            this.deviceNameBox.Size = new System.Drawing.Size(394, 20);
            this.deviceNameBox.TabIndex = 15;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(406, 113);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(68, 13);
            this.label4.TabIndex = 18;
            this.label4.Text = "Signer Name";
            // 
            // signerNameBox
            // 
            this.signerNameBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            this.signerNameBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.signerNameBox.ForeColor = System.Drawing.Color.Silver;
            this.signerNameBox.Location = new System.Drawing.Point(409, 129);
            this.signerNameBox.Name = "signerNameBox";
            this.signerNameBox.Size = new System.Drawing.Size(394, 20);
            this.signerNameBox.TabIndex = 17;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(406, 196);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(120, 13);
            this.label5.TabIndex = 22;
            this.label5.Text = "Minimum Windows build";
            // 
            // minWinBuildBox
            // 
            this.minWinBuildBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            this.minWinBuildBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.minWinBuildBox.ForeColor = System.Drawing.Color.Silver;
            this.minWinBuildBox.Location = new System.Drawing.Point(409, 212);
            this.minWinBuildBox.Name = "minWinBuildBox";
            this.minWinBuildBox.Size = new System.Drawing.Size(394, 20);
            this.minWinBuildBox.TabIndex = 21;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(406, 155);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(123, 13);
            this.label6.TabIndex = 20;
            this.label6.Text = "Maximum Windows build";
            // 
            // maxWinBuildBox
            // 
            this.maxWinBuildBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            this.maxWinBuildBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.maxWinBuildBox.ForeColor = System.Drawing.Color.Silver;
            this.maxWinBuildBox.Location = new System.Drawing.Point(409, 171);
            this.maxWinBuildBox.Name = "maxWinBuildBox";
            this.maxWinBuildBox.Size = new System.Drawing.Size(394, 20);
            this.maxWinBuildBox.TabIndex = 19;
            // 
            // driverWhqlSignedBox
            // 
            this.driverWhqlSignedBox.Appearance = System.Windows.Forms.Appearance.Button;
            this.driverWhqlSignedBox.BoxBorderColor = System.Drawing.Color.DarkSlateBlue;
            this.driverWhqlSignedBox.BoxFillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            this.driverWhqlSignedBox.CheckColor = System.Drawing.Color.CornflowerBlue;
            this.driverWhqlSignedBox.FlatAppearance.BorderSize = 0;
            this.driverWhqlSignedBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.driverWhqlSignedBox.Location = new System.Drawing.Point(653, 324);
            this.driverWhqlSignedBox.Name = "driverWhqlSignedBox";
            this.driverWhqlSignedBox.Size = new System.Drawing.Size(150, 27);
            this.driverWhqlSignedBox.TabIndex = 23;
            this.driverWhqlSignedBox.Text = "Driver is WHQL Signed";
            this.driverWhqlSignedBox.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.driverWhqlSignedBox.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(406, 241);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(120, 13);
            this.label7.TabIndex = 25;
            this.label7.Text = "Shellcode support mask";
            // 
            // shellcodeMaskBox
            // 
            this.shellcodeMaskBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            this.shellcodeMaskBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.shellcodeMaskBox.ForeColor = System.Drawing.Color.Silver;
            this.shellcodeMaskBox.Location = new System.Drawing.Point(409, 257);
            this.shellcodeMaskBox.Name = "shellcodeMaskBox";
            this.shellcodeMaskBox.Size = new System.Drawing.Size(394, 20);
            this.shellcodeMaskBox.TabIndex = 24;
            // 
            // testProviderBtn
            // 
            this.testProviderBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            this.testProviderBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.testProviderBtn.ForeColor = System.Drawing.Color.Silver;
            this.testProviderBtn.Location = new System.Drawing.Point(0, 440);
            this.testProviderBtn.Name = "testProviderBtn";
            this.testProviderBtn.Size = new System.Drawing.Size(75, 23);
            this.testProviderBtn.TabIndex = 26;
            this.testProviderBtn.Text = "Test Driver";
            this.testProviderBtn.UseVisualStyleBackColor = true;
            this.testProviderBtn.Click += new System.EventHandler(this.testProviderBtn_Click);
            // 
            // driverLoadedLbl
            // 
            this.driverLoadedLbl.AutoSize = true;
            this.driverLoadedLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F);
            this.driverLoadedLbl.Location = new System.Drawing.Point(81, 440);
            this.driverLoadedLbl.Name = "driverLoadedLbl";
            this.driverLoadedLbl.Size = new System.Drawing.Size(133, 24);
            this.driverLoadedLbl.TabIndex = 27;
            this.driverLoadedLbl.Text = "Driver Loaded!";
            this.driverLoadedLbl.Visible = false;
            // 
            // driverLoadedLblTimer
            // 
            this.driverLoadedLblTimer.Interval = 2500;
            this.driverLoadedLblTimer.Tick += new System.EventHandler(this.driverLoadedLblTimer_Tick);
            // 
            // setDefaultProviderBtn
            // 
            this.setDefaultProviderBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            this.setDefaultProviderBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.setDefaultProviderBtn.ForeColor = System.Drawing.Color.Silver;
            this.setDefaultProviderBtn.Location = new System.Drawing.Point(686, 443);
            this.setDefaultProviderBtn.Name = "setDefaultProviderBtn";
            this.setDefaultProviderBtn.Size = new System.Drawing.Size(117, 23);
            this.setDefaultProviderBtn.TabIndex = 28;
            this.setDefaultProviderBtn.Text = "Set Default Provider";
            this.setDefaultProviderBtn.UseVisualStyleBackColor = true;
            this.setDefaultProviderBtn.Click += new System.EventHandler(this.setDefaultProviderBtn_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(406, 284);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(97, 13);
            this.label8.TabIndex = 29;
            this.label8.Text = "Default Provider ID";
            // 
            // defaultProviderIDBox
            // 
            this.defaultProviderIDBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            this.defaultProviderIDBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.defaultProviderIDBox.ForeColor = System.Drawing.Color.Silver;
            this.defaultProviderIDBox.Location = new System.Drawing.Point(409, 300);
            this.defaultProviderIDBox.Name = "defaultProviderIDBox";
            this.defaultProviderIDBox.Size = new System.Drawing.Size(394, 20);
            this.defaultProviderIDBox.TabIndex = 30;
            // 
            // Form1
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            this.ClientSize = new System.Drawing.Size(813, 471);
            this.Controls.Add(this.defaultProviderIDBox);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.setDefaultProviderBtn);
            this.Controls.Add(this.driverLoadedLbl);
            this.Controls.Add(this.testProviderBtn);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.shellcodeMaskBox);
            this.Controls.Add(this.driverWhqlSignedBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.minWinBuildBox);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.maxWinBuildBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.signerNameBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.deviceNameBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.driverNameBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.providerExtraInfoBox);
            this.Controls.Add(this.providerList);
            this.Controls.Add(this.appIcon1);
            this.Controls.Add(this.transparentLabel1);
            this.Controls.Add(this.closeBtn);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.Silver;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "KsDumper 11 Provider Selection";
            ((System.ComponentModel.ISupportInitialize)(this.appIcon1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private DarkControls.Controls.WindowsDefaultTitleBarButton closeBtn;
        private DarkControls.Controls.TransparentLabel transparentLabel1;
        private DarkControls.Controls.AppIcon appIcon1;
        private DarkControls.Controls.DarkListView providerList;
        private System.Windows.Forms.ColumnHeader provNameCol;
        private DarkControls.Controls.DarkTextBox providerExtraInfoBox;
        private System.Windows.Forms.Label label1;
        private DarkControls.Controls.DarkTextBox driverNameBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private DarkControls.Controls.DarkTextBox deviceNameBox;
        private System.Windows.Forms.Label label4;
        private DarkControls.Controls.DarkTextBox signerNameBox;
        private System.Windows.Forms.Label label5;
        private DarkControls.Controls.DarkTextBox minWinBuildBox;
        private System.Windows.Forms.Label label6;
        private DarkControls.Controls.DarkTextBox maxWinBuildBox;
        private DarkControls.Controls.DarkCheckBox driverWhqlSignedBox;
        private System.Windows.Forms.ColumnHeader provIdCol;
        private System.Windows.Forms.Label label7;
        private DarkControls.Controls.DarkTextBox shellcodeMaskBox;
        private DarkControls.Controls.DarkButton testProviderBtn;
        private System.Windows.Forms.Label driverLoadedLbl;
        private System.Windows.Forms.Timer driverLoadedLblTimer;
        private DarkControls.Controls.DarkButton setDefaultProviderBtn;
        private System.Windows.Forms.Label label8;
        private DarkControls.Controls.DarkTextBox defaultProviderIDBox;
    }
}

