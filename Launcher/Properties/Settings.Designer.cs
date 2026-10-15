using System;
using System.CodeDom.Compiler;
using System.Configuration;
using System.Runtime.CompilerServices;

namespace Launcher.Properties
{
	// Token: 0x02000047 RID: 71
	[CompilerGenerated]
	[GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "10.0.0.0")]
	internal sealed partial class Settings : ApplicationSettingsBase
	{
		// Token: 0x1700005D RID: 93
		// (get) Token: 0x0600021C RID: 540 RVA: 0x000034A9 File Offset: 0x000016A9
		public static Settings Default
		{
			get
			{
				return Settings.defaultInstance;
			}
		}

		// Token: 0x040001A9 RID: 425
		private static Settings defaultInstance = (Settings)SettingsBase.Synchronized(new Settings());
	}
}
