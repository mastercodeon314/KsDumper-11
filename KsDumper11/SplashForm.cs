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
using System.Runtime.Remoting.Contexts;
using System.Runtime.InteropServices.ComTypes;

namespace KsDumper11
{
    public partial class SplashForm : Form
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool FlushFileBuffers(IntPtr handle);

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

        int maxProviders = 31;
        //int maxProviders = 9;

        List<int> workingProviders = new List<int>();

        string logFolder = Environment.CurrentDirectory + "\\Logs";
        string workingProvidersPath = Environment.CurrentDirectory + "\\Providers.txt";
        string scanningPath = Environment.CurrentDirectory + "\\Scanning.txt";
        Random rnd = new Random();
        void saveProviders(int providerID)
        {
            StringBuilder b = new StringBuilder();
            for (int i = 0; i < workingProviders.Count; i++)
            {
                if (i == workingProviders.Count - 1)
                {
                    b.Append(workingProviders[i]);
                }
                else
                {
                    b.Append(workingProviders[i].ToString() + "|");
                }
            }

            if (providerID != maxProviders)
            {
                writeToDisk(scanningPath, providerID.ToString());
                File.WriteAllText(scanningPath, b.ToString());
            }

            writeToDisk(workingProvidersPath, b.ToString());

            Thread.Sleep(1000);
        }

        private void writeToDisk(string path, string text)
        {
            if (!File.Exists(path))
            {
                FileStream fs = File.Create(path);
                StreamWriter sw = new StreamWriter(fs);
                sw.Write(text);
                sw.Flush();
                FlushFileBuffers(fs.Handle);
                sw.Close();
                sw.Dispose();
            }
            else
            {
                File.Delete(path);
                FileStream fs = File.Create(path);
                StreamWriter sw = new StreamWriter(fs);
                sw.Write(text);
                sw.Flush();
                FlushFileBuffers(fs.Handle);
                sw.Close();
                sw.Dispose();
            }
        }

        private void StartDriver()
        {
            if (!Directory.Exists(logFolder))
            {
                Directory.CreateDirectory(logFolder);
            }

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

            int idx = 0;
            int providerID = 0;

            if (File.Exists(scanningPath))
            {
                if (File.Exists(workingProvidersPath))
                {
                    string provsStr = File.ReadAllText(workingProvidersPath);
                    if (provsStr != String.Empty && provsStr != null)
                    {
                        string[] parts = provsStr.Split('|');
                        foreach (string provider in parts)
                        {
                            workingProviders.Add(int.Parse(provider));
                        }
                    }
                }

                providerID = int.Parse(File.ReadAllText(scanningPath));

                // Save the crash providerID to a blacklist.

                providerID++;
                if (scan(providerID))
                {
                    File.Delete(scanningPath);
                    return;
                }
            }

            if (File.Exists(workingProvidersPath))
            {
                UpdateStatus($"Saved providers found, trying each provider until one works...", 50);
                Thread.Sleep(1000);
                string provsStr = File.ReadAllText(workingProvidersPath);

                if (provsStr != String.Empty && provsStr != null)
                {
                    string[] parts = provsStr.Split('|');
                    foreach (string provider in parts)
                    {
                        workingProviders.Add(int.Parse(provider));
                    }
                }
                while (!DriverInterface.IsDriverOpen("\\\\.\\KsDumper"))
                {
                    if (idx == workingProviders.Count)
                    {
                        retryCountDown = 3;
                        while (retryCountDown != 0)
                        {
                            UpdateStatus($"Driver failed to start, no saved providers worked! Exiting in {retryCountDown}s", 50);
                            Thread.Sleep(1000);
                            retryCountDown -= 1;
                        }

                        Environment.Exit(0);
                        break;
                    }

                    providerID = workingProviders[idx];
                    tryLoad(providerID);

                    if (!DriverInterface.IsDriverOpen("\\\\.\\KsDumper"))
                    {
                        UpdateStatus($"Saved Provider: {providerID} failed!", 50);
                        Thread.Sleep(1000);
                        idx++;
                        continue;
                    }
                    else
                    {
                        UpdateStatus($"Saved Provider: {providerID} worked!", 100);
                        Thread.Sleep(1000);
                        LoadedDriver();
                        return;
                    }
                }
            }

            string logPath = Environment.CurrentDirectory + "\\driverLoading.log";

            //Thread.Sleep(750);

            //{
            //    UpdateStatus("Starting driver with default provider #1", 50);

            //    string args = " /c " + Environment.CurrentDirectory + "\\Driver\\kdu.exe -prv 1 -map .\\Driver\\KsDumperDriver.sys > " + "\"" + logPath + "\"";

            //    ProcessStartInfo inf = new ProcessStartInfo("cmd")
            //    {
            //        Arguments = args,
            //        CreateNoWindow = true,
            //        UseShellExecute = false,
            //    };
            //    Process proc = Process.Start(inf);
            //    proc.WaitForExit();
            //}

            scan(0);

            UpdateStatus("Driver Started!", 100);
            Thread.Sleep(750);

            LoadedDriver();
        }

        bool scan(int providerID)
        {
            int retryCountDown = 3;

            if (!DriverInterface.IsDriverOpen("\\\\.\\KsDumper"))
            {
                retryCountDown = 3;

                UpdateStatus("Scanning for working providers...", 50);
                while (!DriverInterface.IsDriverOpen("\\\\.\\KsDumper"))
                {
                    if (providerID == maxProviders)
                    {
                        if (workingProviders.Count > 0)
                        {
                            providerID = workingProviders[rnd.Next(0, workingProviders.Count - 1)];
                            UpdateStatus("Saving working providers!", 50);
                            Thread.Sleep(500);
                            saveProviders(providerID);

                            tryLoad(providerID);

                            if (DriverInterface.IsDriverOpen("\\\\.\\KsDumper"))
                            {
                                LoadedDriver();
                                return true;
                            }
                            else
                            {
                                retryCountDown = 3;
                                while (retryCountDown != 0)
                                {
                                    UpdateStatus($"No working providers found! Exiting in {retryCountDown}s", 50);
                                    Thread.Sleep(1000);
                                    retryCountDown -= 1;
                                }

                                Environment.Exit(0);
                            }
                        }
                        else
                        {
                            retryCountDown = 3;
                            while (retryCountDown != 0)
                            {
                                UpdateStatus($"No working providers found! Exiting in {retryCountDown}s", 50);
                                Thread.Sleep(1000);
                                retryCountDown -= 1;
                            }

                            Environment.Exit(0);
                        }
                    }
                    if (providerID == 1)// || providerID == 7 || providerID == 29 || providerID == 28)
                    {
                        providerID++;
                        continue;
                    }
                    saveProviders(providerID);

                    tryLoad(providerID);

                    if (!DriverInterface.IsDriverOpen("\\\\.\\KsDumper"))
                    {
                        UpdateStatus($"Provider: {providerID} failed!", 50);
                        Thread.Sleep(1000);
                        providerID++;
                        continue;
                    }
                    else
                    {
                        UpdateStatus($"Provider: {providerID} works", 50);
                        workingProviders.Add(providerID);
                        DriverInterface.OpenKsDumperDriver().UnloadDriver();
                        Thread.Sleep(1000);
                        providerID++;
                        continue;
                    }
                }

                if (!DriverInterface.IsDriverOpen("\\\\.\\KsDumper"))
                {
                    while (retryCountDown != 0)
                    {
                        UpdateStatus($"Driver failed to start! Exiting in {retryCountDown}s", 0);
                        Thread.Sleep(1000);
                        retryCountDown -= 1;
                    }

                    Environment.Exit(0);
                }
            }

            return false;
        }

        void tryLoad(int providerID)
        {
            UpdateStatus($"Starting driver with provider: {providerID}", 50);
            int timeout = 5;
            int retryCountDown = 5;

            string logPath = logFolder + $"\\driverLoading_ProviderID_{providerID}.log";

            string args = " /c " + Environment.CurrentDirectory + $"\\Driver\\kdu.exe -prv {providerID} -map .\\Driver\\KsDumperDriver.sys > " + "\"" + logPath + "\"";

            ProcessStartInfo inf = new ProcessStartInfo("cmd")
            {
                Arguments = args,
                CreateNoWindow = true,
                UseShellExecute = false,
            };
            Process proc = Process.Start(inf);
            if (!proc.WaitForExit(10000))
            {
                proc.Kill();
            }

            if (proc.ExitCode == 1)
            {
                Thread.Sleep(750);
            }
            //if (!DriverInterface.IsDriverOpen("\\\\.\\KsDumper"))
            //{
            //    retryCountDown = 3;

            //    while (retryCountDown != 0)
            //    {
            //        UpdateStatus($"Driver failed to start! Exiting in {retryCountDown}s", 0);
            //        Thread.Sleep(1000);
            //        retryCountDown -= 1;
            //    }

            //    Environment.Exit(0);
            //}

            //UpdateStatus("Driver Started!...", 100);
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
                try
                {
                    StartDriver();
                }
                catch (Exception ex)
                {
                    return;
                }
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
