using KsDumper11.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace KsDumper11
{
    public class StartDriver
    {
        public static void Start()
        {
            bool driverOpen = KsDumperDriverInterface.IsDriverOpen("\\\\.\\KsDumper");

            if (!driverOpen)
            {
                if (File.Exists(KduSelfExtract.AssemblyDirectory + @"\\Providers.json"))
                {
                    KduWrapper wr = new KduWrapper(KduSelfExtract.AssemblyDirectory + @"\Driver\kdu.exe");
                    wr.LoadProviders();
                    wr.Start();
                }
            }
        }
    }
}
