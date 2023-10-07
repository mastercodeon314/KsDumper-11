using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace KsDumper11
{
    public class CrashMon
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool FlushFileBuffers(IntPtr handle);

        private int _checkingProvider = -1;
        public int CheckingProvider
        {
            get
            {
                return _checkingProvider;
            }
            set
            {
                _checkingProvider = value;
                Save();
            }
        }

        string savePath = KduSelfExtract.AssemblyDirectory + @"\\Setings.json";

        public CrashMon()
        {
            if (File.Exists(savePath))
            {
                _checkingProvider = JsonConvert.DeserializeObject<int>(File.ReadAllText(savePath));
            }
            else
            {
                _checkingProvider = -1;
            }
        }

        private void Save()
        {
            string json = JsonConvert.SerializeObject(_checkingProvider);

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
    }
}
