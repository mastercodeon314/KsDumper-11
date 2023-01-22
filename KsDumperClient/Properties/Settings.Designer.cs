using System;
using System.CodeDom.Compiler;
using System.Configuration;
using System.Runtime.CompilerServices;

namespace KsDumperClient.Properties
{
	// Token: 0x02000007 RID: 7
	[CompilerGenerated]
	[GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "17.4.0.0")]
	internal sealed partial class Settings : ApplicationSettingsBase
	{
		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000052 RID: 82 RVA: 0x000044A8 File Offset: 0x000026A8
		public static Settings Default
		{
			get
			{
				return Settings.defaultInstance;
			}
		}

		// Token: 0x04000035 RID: 53
		private static Settings defaultInstance = (Settings)SettingsBase.Synchronized(new Settings());
	}
}
