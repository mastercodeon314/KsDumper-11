using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace KsDumper11
{
    public class KduSelfExtract
    {
        public static void DisableDriverBlockList()
        {
            RegistryKey configKey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\CI\Config", true);

            if (configKey == null)
            {
                configKey = Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Control\CI\Config");
            }

            if (configKey != null)
            {
                if (configKey.GetValue("VulnerableDriverBlocklistEnable") == null)
                {
                    configKey.SetValue("VulnerableDriverBlocklistEnable", 0);
                }
            }
        }

        static string asmDir = "";
        static string driverDir = "";
        static KduSelfExtract()
        {
            DisableDriverBlockList();

            asmDir = AssemblyDirectory;
            driverDir = asmDir + @"\Driver";
        }
        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        public static string KduPath
        {
            get
            {
                return driverDir + @"\kdu.exe";
            }
        }

        private static bool Extracted()
        {
            bool result = false;

            string driverPath = driverDir + @"\KsDumperDriver.sys";
            string kduPath = driverDir + @"\kdu.exe";
            string drv64Path = driverDir + @"\drv64.dll";
            string taigei64Path = driverDir + @"\Taigei64.dll";

            if (!Directory.Exists(driverDir))
            {
                return false;
            }
            else
            {
                if (!File.Exists(driverPath))
                {
                    return false;
                }
                else
                {
                    result = true;
                }

                if (!File.Exists(kduPath))
                {
                    return false;
                }
                else
                {
                    result = true;
                }

                if (!File.Exists(drv64Path))
                {
                    return false;
                }
                else
                {
                    result = true;
                }

                if (!File.Exists(taigei64Path))
                {
                    return false;
                }
                else
                {
                    result = true;
                }
            }
            return result;
        }

        public static void Extract()
        {
            if (!Extracted())
            {
                string asmDir = AssemblyDirectory;
                string driverDir = asmDir + @"\Driver";
                if (!Directory.Exists(driverDir))
                {
                    Directory.CreateDirectory(driverDir);
                }

                string driverPath = driverDir + @"\KsDumperDriver.sys";
                string kduPath = driverDir + @"\kdu.exe";
                string drv64Path = driverDir + @"\drv64.dll";
                string taigei64Path = driverDir + @"\Taigei64.dll";

                if (!File.Exists(driverPath))
                {
                    File.WriteAllBytes(driverPath, DriverInterface.Properties.Resources.KsDumperDriver);
                }

                if (!File.Exists(kduPath))
                {
                    File.WriteAllBytes(kduPath, DriverInterface.Properties.Resources.kdu);
                }

                if (!File.Exists(drv64Path))
                {
                    File.WriteAllBytes(drv64Path, DriverInterface.Properties.Resources.drv64);
                }

                if (!File.Exists(taigei64Path))
                {
                    File.WriteAllBytes(taigei64Path, DriverInterface.Properties.Resources.Taigei64);
                }
            }
        }
    }
}
