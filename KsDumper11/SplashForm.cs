using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using DarkControls;
using KsDumper11.Driver;

namespace KsDumper11
{
    public partial class SplashForm : Form
    {
        protected override CreateParams CreateParams
        {
            get
            {
                // Activate double buffering at the form level.  All child controls will be double buffered as well.
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }

        public bool IsAfterburnerRunning
        {
            get
            {
                Process[] procs = Process.GetProcessesByName("MSIAfterburner");

                if (procs != null)
                {
                    if (procs.Length > 0)
                    {
                        if (procs[0].ProcessName == "MSIAfterburner")
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
        }


        private void StartDriver()
        {
            int timeout = 5;
            int retryCountDown = 5;
            if (IsAfterburnerRunning)
            {
                while (true)
                {
                    if (retryCountDown == 0)
                    {
                        retryCountDown = timeout;
                        if (!IsAfterburnerRunning)
                        {
                            break;
                        }
                    }
                    UpdateStatus($"Waiting MSI Afterburner to be closed... Retry in {retryCountDown}s", 0);
                    Thread.Sleep(1000);
                    retryCountDown -= 1;
                }
                retryCountDown = 3;

                while (retryCountDown != 0)
                {

                    UpdateStatus($"Sleeping {retryCountDown}s to ensure MSI Afterburner driver is unloaded", 0);
                    Thread.Sleep(1000);
                    retryCountDown -= 1;
                }
            }

            string logPath = Environment.CurrentDirectory + "\\driverLoading.log";

            Thread.Sleep(750);

            UpdateStatus("Starting driver...", 50);

            string args = " /c " + Environment.CurrentDirectory + "\\Driver\\kdu.exe -prv 1 -map .\\Driver\\KsDumperDriver.sys > " + "\"" + logPath + "\"";

            ProcessStartInfo inf = new ProcessStartInfo("cmd")
            {
                Arguments = args,
                CreateNoWindow = true,
                UseShellExecute = false,
            };
            Process proc = Process.Start(inf);
            proc.WaitForExit();
            if (!DriverInterface.IsDriverOpen("\\\\.\\KsDumper"))
            {
                retryCountDown = 3;

                while (retryCountDown != 0)
                {
                    UpdateStatus($"Driver failed to start! Exiting in {retryCountDown}s", 0);
                    Thread.Sleep(1000);
                    retryCountDown -= 1;
                }

                Environment.Exit(0);
            }

            UpdateStatus("Driver Started!...", 100);
            Thread.Sleep(750);

            LoadedDriver();
        }

        public SplashForm()
        {

            InitializeComponent();

            this.FormBorderStyle = FormBorderStyle.None;
            this.Region = Region.FromHrgn(Utils.CreateRoundRectRgn(0, 0, Width, Height, 10, 10));
        }

        private void SplashForm_Load(object sender, EventArgs e)
        {
            //StartProgressBar();
            Task.Run(() =>
            {
                StartDriver();
            });
        }

        private void StartProgressBar()
        {
            progressBar.Style = ProgressBarStyle.Marquee;
            progressBar.Show();
        }

        private void StopProgressBar()
        {
            progressBar.Style = ProgressBarStyle.Blocks;
        }

        public delegate void UpdateStatusDel(string txt, int progress);
        public void UpdateStatus(string txt, int progress)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new UpdateStatusDel(UpdateStatus), new object[] { txt, progress });
            }
            else
            {
                this.statusLbl.Text = txt;
                this.progressBar.Value = progress;
            }
        }

        public delegate void LoadedDriverDel();
        public void LoadedDriver()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new LoadedDriverDel(LoadedDriver), new object[] { });
            }
            else
            {
                StopProgressBar();
                this.Close();
            }
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == Utils.WM_NCHITTEST)
                m.Result = (IntPtr)(Utils.HT_CAPTION);
        }


    }
}
