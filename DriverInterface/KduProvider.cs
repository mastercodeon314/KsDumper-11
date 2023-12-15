using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KsDumper11
{
    public class KduProvider
    {
        public int ProviderIndex { get; set; }

        public string ProviderName { get; set; }
        public string DriverName { get; set; }
        public string DeviceName { get; set; }
        public string SignerName { get; set; }
        public bool IsWHQL_Signed { get; set; }
        public string ShellcodeSupportMask { get; set; }

        public string MaxWindowsBuild { get; set; }
        public string MinWindowsBuild { get; set; }

        public string[] ExtraInfo { get; set; }

        public bool IsNonWorking
        {
            get
            {
                return this.ProviderName.Contains("NOT WORKING");
            }
        }

        public bool IsWorking
        {
            get
            {
                return this.ProviderName.Contains("WORKING");
            }
        }


        public KduProvider()
        {
        }

        public KduProvider(string provider)
        {
            processProvider(provider);
        }

        private void processProvider(string prov)
        {
            string[] lines = prov.Split('\n');

            string id = lines[0].Split(',')[0];

            ProviderIndex = int.Parse(id);

            string[] provInfo = lines[1].Split(',');

            ProviderName = provInfo[0];

            string drvName = provInfo[1].Trim().Replace("DriverName ", "").Replace('"'.ToString(), "");
            string devName = provInfo[2].Trim().Replace("DeviceName ", "").Replace('"'.ToString(), "");
            DriverName = drvName;
            DeviceName = devName;

            string signer = lines[2].Trim().Replace("Signed by: ", "").Replace('"'.ToString(), "");
            SignerName = signer;

            string shellCodeMask = lines[3].Trim().Replace("Shellcode support mask: ", "").Replace('"'.ToString(), "");
            ShellcodeSupportMask = shellCodeMask;

            foreach (string ln in lines)
            {
                if (ln.Contains("Driver is WHQL signed"))
                {
                    IsWHQL_Signed = true;
                }

                if (ln.StartsWith("Maximum Windows build undefined"))
                {
                    MaxWindowsBuild = "No Restrictions";
                }

                if (ln.StartsWith("Maximum supported Windows build: "))
                {
                    MaxWindowsBuild = ln.Replace("Maximum supported Windows build: ", "");
                }

                if (ln.StartsWith("Minimum supported Windows build: "))
                {
                    MinWindowsBuild = ln.Replace("Minimum supported Windows build: ", "");
                }
            }

            List<string> extraInfoLines = new List<string>();

            for (int i = 4; i < lines.Length; i++)
            {
                if (lines[i].StartsWith("Minimum"))
                {
                    break;
                }
                else if (!lines[i].Contains("Driver is WHQL signed"))
                {
                    extraInfoLines.Add(lines[i]);
                }
            }

            ExtraInfo = extraInfoLines.ToArray();
        }
    }
}
