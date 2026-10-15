using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;

namespace Launcher
{
	// Token: 0x02000033 RID: 51
	public class App : Application
	{
		// Token: 0x0600012E RID: 302 RVA: 0x000072D0 File Offset: 0x000054D0
		public App()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>
			{
				{ "english", "en-US" },
				{ "german", "de-DE" },
				{ "french", "fr-FR" },
				{ "italian", "it-IT" },
				{ "spanish", "es-ES" },
				{ "japanese", "ja-JP" }
			};
			string steamAppsCurrentGameLanguage = NativeMethods.SteamAppsCurrentGameLanguage;
			string text;
			if (dictionary.ContainsKey(steamAppsCurrentGameLanguage))
			{
				text = dictionary[steamAppsCurrentGameLanguage];
			}
			else
			{
				text = "en-US";
			}
			CultureInfo cultureInfo = new CultureInfo(text);
			Thread.CurrentThread.CurrentUICulture = cultureInfo;
			Thread.CurrentThread.CurrentCulture = cultureInfo;
		}

		// Token: 0x0600012F RID: 303 RVA: 0x00007388 File Offset: 0x00005588
		protected override void OnStartup(StartupEventArgs e)
		{
			if (e.Args != null && e.Args.Length != 0)
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (string text in e.Args)
				{
					stringBuilder.Append(text);
					stringBuilder.Append(' ');
				}
				base.Properties["StartupArguments"] = stringBuilder.ToString().Trim();
			}
			base.OnStartup(e);
		}

		// Token: 0x06000130 RID: 304 RVA: 0x000073F8 File Offset: 0x000055F8
		public static bool ApplicationIsActivated()
		{
			IntPtr foregroundWindow = App.GetForegroundWindow();
			if (foregroundWindow == IntPtr.Zero)
			{
				return false;
			}
			int id = Process.GetCurrentProcess().Id;
			int num;
			App.GetWindowThreadProcessId(foregroundWindow, out num);
			return num == id;
		}

		// Token: 0x06000131 RID: 305
		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		private static extern IntPtr GetForegroundWindow();

		// Token: 0x06000132 RID: 306
		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern int GetWindowThreadProcessId(IntPtr handle, out int processId);

		// Token: 0x06000133 RID: 307 RVA: 0x00002AA3 File Offset: 0x00000CA3
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			base.StartupUri = new Uri("MainWindow.xaml", UriKind.Relative);
		}

		// Token: 0x06000134 RID: 308 RVA: 0x00002AB6 File Offset: 0x00000CB6
		[STAThread]
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public static void Main()
		{
			App app = new App();
			app.InitializeComponent();
			app.Run();
		}
	}
}
