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

        private void StartDriver()
        {
            string logPath = Environment.CurrentDirectory + "\\driverLoading.log";
            FileStream outputStream;
            if (!File.Exists(logPath))
            {
                outputStream = File.Create(logPath);
                UpdateStatus("Created log file...", 25);
            }
            else
            {
                outputStream = File.OpenWrite(logPath);
                UpdateStatus("Opened log file...", 25);
            }
            StreamWriter wr = new StreamWriter(outputStream);

            Thread.Sleep(750);

            UpdateStatus("Starting driver...", 50);

            ProcessStartInfo inf = new ProcessStartInfo(Environment.CurrentDirectory + "\\Driver\\kdu.exe", " -prv 1 -map .\\Driver\\KsDumperDriver.sys")
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                //RedirectStandardOutput = true,
                //RedirectStandardError = true
            };
            Process proc = Process.Start(inf);
            proc.OutputDataReceived += delegate (object sender, DataReceivedEventArgs e)
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    wr.WriteLine(e.Data);
                }
            };
            proc.ErrorDataReceived += delegate (object sender, DataReceivedEventArgs e)
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    wr.WriteLine(e.Data);
                }
            };
            proc.WaitForExit();
            wr.Flush();
            wr.Close();
            outputStream.Close();
            outputStream.Dispose();
            if (!DriverInterface.IsDriverOpen("\\\\.\\KsDumper"))
            {
                UpdateStatus("Driver failed to start! Exiting in 3s", 0);

                Thread.Sleep(3000);

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
                this.Invoke(new LoadedDriverDel(LoadedDriver), new object[] {  });
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
