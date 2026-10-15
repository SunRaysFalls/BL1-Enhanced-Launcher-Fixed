using System;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using ProtoBuf;

namespace Launcher
{
	// Token: 0x0200000E RID: 14
	[ProtoContract]
	public class LauncherSettings : INotifyPropertyChanged
	{
		// Token: 0x1700000D RID: 13
		// (get) Token: 0x0600005F RID: 95 RVA: 0x00002355 File Offset: 0x00000555
		// (set) Token: 0x06000060 RID: 96 RVA: 0x0000235D File Offset: 0x0000055D
		[ProtoMember(1)]
		public bool bMuteAudio
		{
			get
			{
				return this._bMuteAudio;
			}
			set
			{
				if (this._bMuteAudio != value)
				{
					this._bMuteAudio = value;
					this.NotifyPropertyChanged("bMuteAudio");
				}
			}
		}

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000061 RID: 97 RVA: 0x0000237A File Offset: 0x0000057A
		private static string UserCachedDataPath
		{
			get
			{
				if (string.IsNullOrEmpty(LauncherSettings.m_sUserCachedDataPath))
				{
					LauncherSettings.m_sUserCachedDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "My Games", MainWindow.GameName, "LauncherData");
				}
				return LauncherSettings.m_sUserCachedDataPath;
			}
		}

		// Token: 0x14000005 RID: 5
		// (add) Token: 0x06000062 RID: 98 RVA: 0x000049F8 File Offset: 0x00002BF8
		// (remove) Token: 0x06000063 RID: 99 RVA: 0x00004A30 File Offset: 0x00002C30
		public event PropertyChangedEventHandler PropertyChanged;

		// Token: 0x06000064 RID: 100 RVA: 0x000023AC File Offset: 0x000005AC
		public void NotifyPropertyChanged(string PropertyName)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
			}
		}

		// Token: 0x06000065 RID: 101 RVA: 0x00004A68 File Offset: 0x00002C68
		public static LauncherSettings Load(string FromFile = "LauncherSettings.w2l")
		{
			LauncherSettings launcherSettings = null;
			if (string.IsNullOrEmpty(FromFile))
			{
				return launcherSettings;
			}
			string text = Path.Combine(LauncherSettings.UserCachedDataPath, FromFile);
			if (!File.Exists(text))
			{
				return launcherSettings;
			}
			try
			{
				using (FileStream fileStream = new FileStream(text, FileMode.Open))
				{
					using (GZipStream gzipStream = new GZipStream(fileStream, CompressionMode.Decompress))
					{
						launcherSettings = Serializer.Deserialize<LauncherSettings>(gzipStream);
						launcherSettings.LoadedFromPath = text;
					}
				}
			}
			catch (Exception ex)
			{
				MainWindow.LogMessage("LauncherSettings.Load(): FromFile=" + FromFile + "; msg=" + ex.Message, false);
			}
			return launcherSettings;
		}

		// Token: 0x06000066 RID: 102 RVA: 0x0000208E File Offset: 0x0000028E
		private LauncherSettings()
		{
		}

		// Token: 0x06000067 RID: 103 RVA: 0x000023C8 File Offset: 0x000005C8
		public LauncherSettings(string FromFile = "LauncherSettings.w2l")
			: this()
		{
			this.LoadedFromPath = Path.Combine(LauncherSettings.UserCachedDataPath, FromFile);
		}

		// Token: 0x06000068 RID: 104 RVA: 0x00004B1C File Offset: 0x00002D1C
		public void Save()
		{
			try
			{
				if (!Directory.Exists(Path.GetDirectoryName(this.LoadedFromPath)))
				{
					Directory.CreateDirectory(Path.GetDirectoryName(this.LoadedFromPath));
				}
				using (FileStream fileStream = new FileStream(this.LoadedFromPath, FileMode.Create))
				{
					using (GZipStream gzipStream = new GZipStream(fileStream, CompressionMode.Compress))
					{
						Serializer.Serialize<LauncherSettings>(gzipStream, this);
					}
				}
			}
			catch (Exception ex)
			{
				MainWindow.LogMessage("LauncherSettings.Save(): LoadedFromPath=" + this.LoadedFromPath + "; msg=" + ex.Message, false);
			}
		}

		// Token: 0x0400002A RID: 42
		private bool _bMuteAudio;

		// Token: 0x0400002B RID: 43
		private static string m_sUserCachedDataPath;

		// Token: 0x0400002C RID: 44
		private string LoadedFromPath;
	}
}
