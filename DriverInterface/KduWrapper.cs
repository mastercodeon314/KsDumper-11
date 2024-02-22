using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using KsDumper11.Driver;
using Newtonsoft.Json;

namespace KsDumper11
{
    public class KduWrapper
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool FlushFileBuffers(IntPtr handle);

        string logFolder = Environment.CurrentDirectory + "\\Logs";

        public string KduPath { get; set; }

        private CancellationToken checkerTaskCancelToken;
        private CancelableTask<int> checkerTask;

        public event EventHandler<object[]> DriverLoaded;
        public event EventHandler ProvidersLoaded;

        private KduProviderSettings kduSettingsJson;

        public List<KduProvider> providers = new List<KduProvider>();

        private bool _IsDirty = false;

        //public event EventHandler IsDirtyChanged;

        public bool IsDirty
        {
            get
            {
                return _IsDirty;
            }
            set
            {
                _IsDirty = value;
                //if (IsDirtyChanged != null)
                //{
                //    IsDirtyChanged(_IsDirty, EventArgs.Empty);
                //}
            }
        }

        //CrashMon crashMon;

        public int DefaultProvider
        {
            get
            {
                return kduSettingsJson.DefaultProvider;
            }
        }

        public KduWrapper(string kduPath)
        {
            KduPath = kduPath;
            //crashMon = new CrashMon();

            kduSettingsJson = new KduProviderSettings();
        }

        public void SetDefaultProvider(int providerID)
        {
            kduSettingsJson.DefaultProvider = providerID;

            SaveProviders();
        }

        public void ResetProviders()
        {

            for (int i = 0; i < providers.Count; i++)
            {
                string non_W = "[NOT WORKING] ";
                string W_ = "[WORKING] ";
                providers[i].ProviderName = providers[i].ProviderName.Replace(non_W, "").Replace(W_, "");
            }

            kduSettingsJson.DefaultProvider = -1;

            IsDirty = false;

            SaveProviders();
        }

        public void LoadProviders()
        {
            if (!File.Exists(KduSelfExtract.AssemblyDirectory + @"\\Providers.json"))
            {
                populateProviders();

                IsDirty = false;
            }
            else
            {
                kduSettingsJson = JsonConvert.DeserializeObject<KduProviderSettings>(File.ReadAllText(KduSelfExtract.AssemblyDirectory + @"\\Providers.json"));
                providers = kduSettingsJson.Providers;

                bool foundADirty = false;
                foreach (KduProvider provider in providers)
                {
                    if (provider.IsNonWorking == true || provider.IsWorking == true)
                    {
                        foundADirty = true;
                        break;
                    }
                }

                IsDirty = foundADirty;

                //if (crashMon.CheckingProvider != -1)
                //{
                //    //if (KsDumper11.BSOD.JustHappened())
                //    {
                //        providers[crashMon.CheckingProvider].ProviderName = "[NOT WORKING] " + providers[crashMon.CheckingProvider].ProviderName;
                //        SaveProviders();

                //        crashMon.CheckingProvider = -1;
                //    }
                //}

                FireProvidersLoaded();
            }
        }

        private void populateProviders()
        {

            ProcessStartInfo inf = new ProcessStartInfo();
            inf.FileName = KduPath;
            inf.Arguments = "-list";
            inf.CreateNoWindow = true;
            inf.WindowStyle = ProcessWindowStyle.Hidden;
            inf.RedirectStandardOutput = true;
            inf.UseShellExecute = false;

            Process proc = Process.Start(inf);
            string str = proc.StandardOutput.ReadToEnd();

            List<string> parts = new List<string>(str.Split(new string[] { "Provider #" }, StringSplitOptions.RemoveEmptyEntries));
            parts.RemoveAt(0);

            for (int i = 0; i < parts.Count; i++)
            {
                parts[i] = parts[i].Trim().Replace('\r'.ToString(), "").Replace('\t'.ToString(), "");
            }

            foreach (string prov in parts)
            {
                KduProvider p = new KduProvider(prov);
                providers.Add(p);
            }

            SaveProviders();

            FireProvidersLoaded();
        }

        private void FireProvidersLoaded()
        {
            if (ProvidersLoaded != null)
            {
                ProvidersLoaded(this, EventArgs.Empty);
            }
        }

        private void SaveProviders()
        {
            kduSettingsJson.Providers = providers;

            string json = JsonConvert.SerializeObject(kduSettingsJson);
            string savePath = KduSelfExtract.AssemblyDirectory + @"\\Providers.json";
            if (!File.Exists(savePath))
            {
                FileStream fs = File.Create(savePath);
                StreamWriter sw = new StreamWriter(fs);
                sw.Write(json);
                sw.Flush();
                FlushFileBuffers(fs.Handle);
                sw.Close();
                sw.Dispose();
            }
            else
            {
                File.Delete(savePath);
                FileStream fs = File.Create(savePath);
                StreamWriter sw = new StreamWriter(fs);
                sw.Write(json);
                sw.Flush();
                FlushFileBuffers(fs.Handle);
                sw.Close();
                sw.Dispose();
            }
        }

        private void runChecker(int providerID)
        {
            checkerTaskCancelToken = new CancellationToken();

            checkerTask = new CancelableTask<int>(checkerTaskCancelToken);

            // Create a cancelable task
            var task = checkerTask.CreateTask(token =>
            {
                while (!KsDumperDriverInterface.IsDriverOpen("\\\\.\\KsDumper"))
                {
                    try
                    {
                        // Checks to see if we need to cancel the checker
                        token.ThrowIfCancellationRequested();
                    }
                    catch (Exception)
                    {
                        break;
                    }
                }

                if (KsDumperDriverInterface.IsDriverOpen("\\\\.\\KsDumper"))
                {
                    if (DriverLoaded != null)
                    {
                        updateProvider(true, providerID);
                        DriverLoaded(this, new object[] { true, providerID });
                    }
                    return 1;
                }
                else
                {
                    if (DriverLoaded != null)
                    {
                        updateProvider(false, providerID);
                        DriverLoaded(this, new object[] { false, providerID });
                    }
                    return 0;
                }
            });
        }

        private void updateProvider(bool res, int idx)
        {
            //crashMon.CheckingProvider = -1;

            KduProvider p = providers[idx];

            if (res)
            {
                KsDumperDriverInterface ksDriver = new KsDumperDriverInterface("\\\\.\\KsDumper");
                ksDriver.UnloadDriver();

                ksDriver.Dispose();

                providers[idx].ProviderName = "[WORKING] " + providers[idx].ProviderName;

                IsDirty = true;
            }
            else
            {
                providers[idx].ProviderName = "[NOT WORKING] " + providers[idx].ProviderName;

                IsDirty = true;
            }

            SaveProviders();
        }

        static string AppendDateTimeToFileName(string originalFileName)
        {
            // Get the current date and time
            DateTime currentTime = DateTime.Now;

            // Format the date and time as a string
            string formattedDateTime = currentTime.ToString("yyyyMMddHHmmss");

            // Get the file extension from the original filename (if any)
            string fileExtension = System.IO.Path.GetExtension(originalFileName);

            // Remove the file extension from the original filename
            string fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(originalFileName);

            // Append the formatted date and time to the filename
            string newFileName = $"{formattedDateTime}_{fileNameWithoutExtension}{fileExtension}";

            return newFileName;
        }

        public void Start()
        {
            int providerID = kduSettingsJson.DefaultProvider;

            if (providerID != -1)
            {
                if (providers[providerID].ProviderName.Contains("[NON WORKING]"))
                {
                    return;
                }

                string fileName = $"KsDumper11Driver_ProviderID_{providerID}.log";

                fileName = AppendDateTimeToFileName(fileName);

                string logPath = logFolder + "\\" + fileName;

                if (!Directory.Exists(logFolder))
                {
                    Directory.CreateDirectory(logFolder);
                }

                ProcessStartInfo inf = new ProcessStartInfo(KduPath)
                {
                    Arguments = $"-prv {providerID} -map .\\Driver\\KsDumperDriver.sys",
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    WorkingDirectory = Environment.CurrentDirectory,
                    RedirectStandardOutput = true
                };
                Process proc = Process.Start(inf);
                proc.Exited += Proc_Exited;
                proc.EnableRaisingEvents = true;
                proc.WaitForExit(12500);

                string output = proc.StandardOutput.ReadToEnd();

                File.WriteAllText(logPath, output);
            }
            else
            {
                // alert the user to the fact they probably need to clear the settings jsons
            }
        }

        public void tryLoad(int providerID)
        {
            if (providers[providerID].ProviderName.Contains("[WORKING]") || providers[providerID].ProviderName.Contains("[NON WORKING]"))
            {
                return;
            }

            //crashMon.CheckingProvider = providerID;

            Task.Run(() =>
            {
                runChecker(providerID);

                string fileName = $"KsDumper11Driver_ProviderID_{providerID}.log";

                fileName = AppendDateTimeToFileName(fileName);

                string logPath = logFolder + "\\" + fileName;

                if (!Directory.Exists(logFolder))
                {
                    Directory.CreateDirectory(logFolder);
                }

                ProcessStartInfo inf = new ProcessStartInfo(KduPath)
                {
                    Arguments = $"-prv {providerID} -map .\\Driver\\KsDumperDriver.sys",
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    WorkingDirectory = Environment.CurrentDirectory,
                    RedirectStandardOutput = true
                };
                Process proc = Process.Start(inf);
                proc.Exited += Proc_Exited;
                proc.EnableRaisingEvents = true;
                proc.WaitForExit(12500);


                string output = proc.StandardOutput.ReadToEnd();

                File.WriteAllText(logPath, output);

            });
        }

        private void Proc_Exited(object sender, EventArgs e)
        {
            if (checkerTask != null)
            {
                try
                {
                    checkerTask.Cancel();
                }
                catch { }
            }


            ProcessStartInfo inf = new ProcessStartInfo("cmd.exe")
            {
                Arguments = "/c taskkill /IM \"kdu.exe\"",
                CreateNoWindow = true,
                UseShellExecute = false,
            };

            Process proc = Process.Start(inf);
            proc.WaitForExit(12500);

            inf = new ProcessStartInfo("cmd")
            {
                Arguments = " /c \"taskkill /im kdu.exe\"",
                CreateNoWindow = true,
                UseShellExecute = false,
            };
            proc = Process.Start(inf);
            if (!proc.WaitForExit(12500))
            {
                proc.Kill();
            }
        }
    }
}
