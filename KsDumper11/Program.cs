using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using KsDumper11.Driver;

namespace KsDumper11
{
	public class Program
	{
		[STAThread]
		private static void Main()
		{ 
			KduSelfExtract.DisableDriverBlockList();

			KduSelfExtract.Extract();


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            bool driverOpen = KsDumperDriverInterface.IsDriverOpen("\\\\.\\KsDumper");
			//Debugger.Break();
            if (!driverOpen)
			{
                if (!File.Exists(KduSelfExtract.AssemblyDirectory + @"\\Providers.json"))
				{
					// Run the selector here to populate the providers and set a default provider. 
					Application.Run(new ProviderSelector());
                    Application.Run(new Dumper());
                }
				else
				{
					KduWrapper wr = new KduWrapper(KduSelfExtract.AssemblyDirectory + @"\Driver\kdu.exe");
					wr.LoadProviders();
					wr.Start();

					if (KsDumperDriverInterface.IsDriverOpen("\\\\.\\KsDumper"))
					{
						Application.Run(new Dumper());
					}
					else
					{
						Environment.Exit(0);
					}
                }
            }
			else
			{
                Application.Run(new Dumper());
            }
		}
	}
}
