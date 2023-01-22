using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace KsDumperClient.Properties
{
	// Token: 0x02000006 RID: 6
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
	[DebuggerNonUserCode]
	[CompilerGenerated]
	internal class Resources
	{
		// Token: 0x0600004D RID: 77 RVA: 0x00004406 File Offset: 0x00002606
		internal Resources()
		{
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x0600004E RID: 78 RVA: 0x00004410 File Offset: 0x00002610
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				bool flag = Resources.resourceMan == null;
				if (flag)
				{
					ResourceManager temp = new ResourceManager("KsDumperClient.Properties.Resources", typeof(Resources).Assembly);
					Resources.resourceMan = temp;
				}
				return Resources.resourceMan;
			}
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x0600004F RID: 79 RVA: 0x00004458 File Offset: 0x00002658
		// (set) Token: 0x06000050 RID: 80 RVA: 0x0000446F File Offset: 0x0000266F
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			get
			{
				return Resources.resourceCulture;
			}
			set
			{
				Resources.resourceCulture = value;
			}
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000051 RID: 81 RVA: 0x00004478 File Offset: 0x00002678
		internal static Bitmap icons8_crossed_axes_100
		{
			get
			{
				object obj = Resources.ResourceManager.GetObject("icons8_crossed_axes_100", Resources.resourceCulture);
				return (Bitmap)obj;
			}
		}

		// Token: 0x04000033 RID: 51
		private static ResourceManager resourceMan;

		// Token: 0x04000034 RID: 52
		private static CultureInfo resourceCulture;
	}
}
