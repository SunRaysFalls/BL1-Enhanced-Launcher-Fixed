using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using ProtoBuf;

namespace Launcher
{
	// Token: 0x02000008 RID: 8
	public class DynamicContent : INotifyPropertyChanged
	{
		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000022 RID: 34 RVA: 0x00002173 File Offset: 0x00000373
		public string LauncherBaseUrl
		{
			get
			{
				if (string.IsNullOrEmpty(this.m_sLauncherBaseUrl))
				{
					this.m_sLauncherBaseUrl = string.Format("http://gbx.parnic.com/launcher/LauncherContent.{0}.wpb", MainWindow.ThreeLetterLanguageCode.ToLower());
				}
				return this.m_sLauncherBaseUrl;
			}
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000023 RID: 35 RVA: 0x000021A2 File Offset: 0x000003A2
		private string UserCachedDataPath
		{
			get
			{
				if (string.IsNullOrEmpty(this.m_sUserCachedDataPath))
				{
					this.m_sUserCachedDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "My Games", MainWindow.GameName, "LauncherData");
				}
				return this.m_sUserCachedDataPath;
			}
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000024 RID: 36 RVA: 0x000021D7 File Offset: 0x000003D7
		// (set) Token: 0x06000025 RID: 37 RVA: 0x000021DF File Offset: 0x000003DF
		public ObservableCollection<NewsStory> NewsStories
		{
			get
			{
				return this._newsStories;
			}
			set
			{
				this._newsStories = value;
				this.NotifyPropertyChanged("NewsStories");
			}
		}

		// Token: 0x14000002 RID: 2
		// (add) Token: 0x06000026 RID: 38 RVA: 0x0000359C File Offset: 0x0000179C
		// (remove) Token: 0x06000027 RID: 39 RVA: 0x000035D4 File Offset: 0x000017D4
		public event PropertyChangedEventHandler PropertyChanged;

		// Token: 0x06000028 RID: 40 RVA: 0x0000360C File Offset: 0x0000180C
		private void NotifyPropertyChanged(string propertyName)
		{
			PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
			if (propertyChanged != null)
			{
				propertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		// Token: 0x14000003 RID: 3
		// (add) Token: 0x06000029 RID: 41 RVA: 0x00003630 File Offset: 0x00001830
		// (remove) Token: 0x0600002A RID: 42 RVA: 0x00003668 File Offset: 0x00001868
		public event DynamicContent.OnNewBackgroundReceived NewBackgroundReceivedEvents;

		// Token: 0x0600002B RID: 43 RVA: 0x000021F3 File Offset: 0x000003F3
		public DynamicContent()
		{
		}

		// Token: 0x0600002C RID: 44 RVA: 0x0000221F File Offset: 0x0000041F
		public DynamicContent(Dispatcher InMainWindowDispatcher)
			: this()
		{
			this.MainWindowDispatcher = InMainWindowDispatcher;
		}

		// Token: 0x0600002D RID: 45 RVA: 0x0000222E File Offset: 0x0000042E
		public void InitDynamicContent()
		{
			ThreadPool.QueueUserWorkItem(new WaitCallback(this.BlockingDownloadAvailableData));
		}

		// Token: 0x0600002E RID: 46 RVA: 0x000036A0 File Offset: 0x000018A0
		private void BlockingDownloadAvailableData(object StateInfo)
		{
			this.TryLoadCachedNewsInfo();
			this.TryLoadCachedBackgroundInfo();
			MainWindow.LogMessage(string.Format("Attempting to download dynamic content metadata from {0}...", this.LauncherBaseUrl), false);
			HttpWebRequest httpWebRequest = WebRequest.Create(this.LauncherBaseUrl) as HttpWebRequest;
			try
			{
				AvailableData availableData = null;
				using (WebResponse response = httpWebRequest.GetResponse())
				{
					MainWindow.LogMessage(string.Format("Dynamic content metadata response received. Parsing...", new object[0]), false);
					using (GZipStream gzipStream = new GZipStream(response.GetResponseStream(), CompressionMode.Decompress))
					{
						availableData = Serializer.Deserialize<AvailableData>(gzipStream);
					}
				}
				if (availableData != null)
				{
					bool flag = false;
					MainWindow.LogMessage(string.Format("Successfully parsed dynamic content metadata. Looking for new info...", new object[0]), false);
					flag = this.ConditionalDownloadNewBackground(availableData.BackgroundInfo) || flag;
					this.MainWindowDispatcher.Invoke(new Action<ObservableCollection<NewsStory>>(delegate(ObservableCollection<NewsStory> stories)
					{
						try
						{
							this.NewsStories = stories;
							MainWindow.LogMessage("News stories loaded. Caching...", false);
						}
						catch (Exception ex2)
						{
							MainWindow.LogMessage(string.Format("Failed to load news stories. Error: {0}{1}{2}", ex2.Message, Environment.NewLine, ex2.StackTrace), false);
						}
					}), new object[] { availableData.NewsStories });
					this.CachedNewsInfo = availableData.NewsInfo;
					if (this.CachedNewsInfo == null)
					{
						this.CachedNewsInfo = new DynamicContentUpdateInfo
						{
							LastUpdated = UnixTimestampToTextDate.DateTimeToUnixTime(DateTime.Now)
						};
					}
					if (this.TryWriteNewsCache(this.NewsStories))
					{
						MainWindow.LogMessage("All news downloads and caching completed successfully.", false);
					}
					if (!flag)
					{
						MainWindow.LogMessage("All cached dynamic data is up to date.", false);
					}
				}
				else
				{
					MainWindow.LogMessage("Failed to parse dynamic content metadata.", false);
				}
			}
			catch (Exception ex)
			{
				MainWindow.LogMessage(string.Format("Error retrieving dynamic content metadata: {0}{1}{2}", ex.Message, Environment.NewLine, ex.StackTrace), false);
			}
		}

		// Token: 0x0600002F RID: 47 RVA: 0x0000385C File Offset: 0x00001A5C
		private bool ConditionalDownloadNewBackground(DynamicContentUpdateInfo dynamicContentUpdateInfo)
		{
			if (dynamicContentUpdateInfo != null && dynamicContentUpdateInfo.LastUpdated <= UnixTimestampToTextDate.DateTimeToUnixTime(DateTime.Now) && (this.CachedBackgroundInfo == null || dynamicContentUpdateInfo.LastUpdated > this.CachedBackgroundInfo.LastUpdated))
			{
				MainWindow.LogMessage(string.Format("New background info found. Old date: {0}, New date: {1}.", (this.CachedBackgroundInfo != null) ? UnixTimestampToTextDate.UnixTimeToDateTime(this.CachedBackgroundInfo.LastUpdated) : DateTime.MinValue, UnixTimestampToTextDate.UnixTimeToDateTime(dynamicContentUpdateInfo.LastUpdated)), false);
				ThreadPool.QueueUserWorkItem(new WaitCallback(this.BlockingDownloadReplacementBackground), dynamicContentUpdateInfo);
				this.CachedBackgroundInfo = dynamicContentUpdateInfo;
				return true;
			}
			return false;
		}

		// Token: 0x06000030 RID: 48 RVA: 0x00003900 File Offset: 0x00001B00
		private bool ConditionalDownloadNewNews(DynamicContentUpdateInfo dynamicContentUpdateInfo)
		{
			if (dynamicContentUpdateInfo != null && (this.CachedNewsInfo == null || dynamicContentUpdateInfo.LastUpdated > this.CachedNewsInfo.LastUpdated))
			{
				MainWindow.LogMessage(string.Format("New news info found. Old date: {0}, New date: {1}.", (this.CachedNewsInfo != null) ? UnixTimestampToTextDate.UnixTimeToDateTime(this.CachedNewsInfo.LastUpdated) : DateTime.MinValue, UnixTimestampToTextDate.UnixTimeToDateTime(dynamicContentUpdateInfo.LastUpdated)), false);
				ThreadPool.QueueUserWorkItem(new WaitCallback(this.BlockingDownloadNewsStories), dynamicContentUpdateInfo);
				this.CachedNewsInfo = dynamicContentUpdateInfo;
				return true;
			}
			return false;
		}

		// Token: 0x06000031 RID: 49 RVA: 0x0000398C File Offset: 0x00001B8C
		private void BlockingDownloadReplacementBackground(object StateInfo)
		{
			DynamicContentUpdateInfo dynamicContentUpdateInfo = StateInfo as DynamicContentUpdateInfo;
			MainWindow.LogMessage(string.Format("Retrieving replacement background from {0}...", dynamicContentUpdateInfo.DataUrl), false);
			HttpWebRequest httpWebRequest = WebRequest.Create(dynamicContentUpdateInfo.DataUrl) as HttpWebRequest;
			try
			{
				using (WebResponse response = httpWebRequest.GetResponse())
				{
					MainWindow.LogMessage("Replacement background received. Loading...", false);
					try
					{
						using (MemoryStream memoryStream = new MemoryStream())
						{
							response.GetResponseStream().CopyTo(memoryStream);
							if (this.LoadBackgroundImageFromStream(memoryStream))
							{
								MainWindow.LogMessage("Replacement background loaded. Caching...", false);
								if (this.TryWriteBackgroundCache(memoryStream))
								{
									MainWindow.LogMessage("All background replacement downloads and caching completed successfully.", false);
								}
							}
							else
							{
								MainWindow.LogMessage("Replacement background failed to load from stream.", false);
							}
						}
					}
					catch (Exception ex)
					{
						MainWindow.LogMessage(string.Format("Failed to load replacement background. Error: {0}{1}{2}", ex.Message, Environment.NewLine, ex.StackTrace), false);
					}
				}
			}
			catch (Exception ex2)
			{
				MainWindow.LogMessage(string.Format("Failed to retrieve replacement background. Error: {0}{1}{2}", ex2.Message, Environment.NewLine, ex2.StackTrace), false);
			}
		}

		// Token: 0x06000032 RID: 50 RVA: 0x00003AC0 File Offset: 0x00001CC0
		private void BlockingDownloadNewsStories(object StateInfo)
		{
			DynamicContentUpdateInfo dynamicContentUpdateInfo = StateInfo as DynamicContentUpdateInfo;
			MainWindow.LogMessage(string.Format("Retrieving news stories from {0}...", dynamicContentUpdateInfo.DataUrl), false);
			HttpWebRequest httpWebRequest = WebRequest.Create(string.Format(this.LauncherNewsUrl, dynamicContentUpdateInfo.DataUrl)) as HttpWebRequest;
			try
			{
				using (WebResponse response = httpWebRequest.GetResponse())
				{
					MainWindow.LogMessage("News stories received. Loading...", false);
					this.MainWindowDispatcher.Invoke(new Action<Stream>(delegate(Stream stream)
					{
						try
						{
							this.NewsStories = Serializer.Deserialize<ObservableCollection<NewsStory>>(stream);
							MainWindow.LogMessage("News stories loaded. Caching...", false);
						}
						catch (Exception ex2)
						{
							MainWindow.LogMessage(string.Format("Failed to load news stories. Error: {0}{1}{2}", ex2.Message, Environment.NewLine, ex2.StackTrace), false);
						}
					}), new object[] { response.GetResponseStream() });
					if (this.TryWriteNewsCache(this.NewsStories))
					{
						MainWindow.LogMessage("All news downloads and caching completed successfully.", false);
					}
				}
			}
			catch (Exception ex)
			{
				MainWindow.LogMessage(string.Format("Failed to retrieve replacement background. Error: {0}{1}{2}", ex.Message, Environment.NewLine, ex.StackTrace), false);
			}
		}

		// Token: 0x06000033 RID: 51 RVA: 0x00003BA8 File Offset: 0x00001DA8
		private void TryLoadCachedNewsInfo()
		{
			MainWindow.LogMessage(string.Format("Loading cached news data from '{0}'...", "NewsMetadata.wpb"), false);
			this.TryLoadCachedInfo("NewsMetadata.wpb", out this.CachedNewsInfo);
			if (this.CachedNewsInfo != null)
			{
				MainWindow.LogMessage("Cached news info loaded.", false);
			}
			else
			{
				MainWindow.LogMessage("Failed to load cached news info.", false);
			}
			try
			{
				string text = Path.Combine(this.UserCachedDataPath, "News.wpb");
				if (this.MainWindowDispatcher != null && File.Exists(text))
				{
					using (FileStream fileStream = new FileStream(text, FileMode.Open))
					{
						ObservableCollection<NewsStory> observableCollection = Serializer.Deserialize<ObservableCollection<NewsStory>>(fileStream);
						this.MainWindowDispatcher.BeginInvoke(new Action<ObservableCollection<NewsStory>>(delegate(ObservableCollection<NewsStory> news)
						{
							this.NewsStories = news;
						}), new object[] { observableCollection });
					}
				}
			}
			catch (Exception ex)
			{
				MainWindow.LogMessage(string.Format("Failed to load cached news. Error: {0}{1}{2}", ex.Message, Environment.NewLine, ex.StackTrace), false);
			}
		}

		// Token: 0x06000034 RID: 52 RVA: 0x00003C9C File Offset: 0x00001E9C
		private void TryLoadCachedBackgroundInfo()
		{
			MainWindow.LogMessage(string.Format("Loading cached background data from '{0}'...", "BackgroundMetadata.wpb"), false);
			this.TryLoadCachedInfo("BackgroundMetadata.wpb", out this.CachedBackgroundInfo);
			if (this.CachedNewsInfo != null)
			{
				MainWindow.LogMessage("Cached background data loaded.", false);
			}
			else
			{
				MainWindow.LogMessage("Failed to load cached background data.", false);
			}
			string text = Path.Combine(this.UserCachedDataPath, "Background.wid");
			if (File.Exists(text))
			{
				using (FileStream fileStream = new FileStream(text, FileMode.Open))
				{
					using (MemoryStream memoryStream = new MemoryStream())
					{
						fileStream.CopyTo(memoryStream);
						this.LoadBackgroundImageFromStream(memoryStream);
					}
				}
			}
		}

		// Token: 0x06000035 RID: 53 RVA: 0x00003D58 File Offset: 0x00001F58
		private void TryLoadCachedInfo(string Filename, out DynamicContentUpdateInfo info)
		{
			info = null;
			try
			{
				string text = Path.Combine(this.UserCachedDataPath, Filename);
				if (File.Exists(text))
				{
					using (FileStream fileStream = new FileStream(text, FileMode.Open))
					{
						info = Serializer.Deserialize<DynamicContentUpdateInfo>(fileStream);
					}
				}
			}
			catch (Exception ex)
			{
				MainWindow.LogMessage(string.Format("Unable to retrieve dynamic data for file '{0}'. Error: {1}", Filename, ex.Message), false);
			}
		}

		// Token: 0x06000036 RID: 54 RVA: 0x00003DD4 File Offset: 0x00001FD4
		private bool LoadBackgroundImageFromStream(MemoryStream ms)
		{
			bool flag;
			try
			{
				BitmapImage bitmapImage = new BitmapImage();
				bitmapImage.BeginInit();
				bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
				bitmapImage.StreamSource = ms;
				bitmapImage.EndInit();
				bitmapImage.Freeze();
				if (this.MainWindowDispatcher != null)
				{
					this.MainWindowDispatcher.BeginInvoke(new Action<BitmapImage>(delegate(BitmapImage bmpImg)
					{
						this.NewBackgroundReceivedEvents(bmpImg, EventArgs.Empty);
					}), new object[] { bitmapImage });
				}
				flag = true;
			}
			catch (Exception)
			{
				MainWindow.LogMessage("Can't load background image from MemoryStream: ms.Length=" + ms.Length, false);
				flag = false;
			}
			return flag;
		}

		// Token: 0x06000037 RID: 55 RVA: 0x00003E68 File Offset: 0x00002068
		private bool TryWriteMetadataCache(string Filename, DynamicContentUpdateInfo info)
		{
			bool flag;
			try
			{
				using (FileStream fileStream = new FileStream(Path.Combine(this.UserCachedDataPath, Filename), FileMode.Create))
				{
					info.DataUrl = null;
					Serializer.Serialize<DynamicContentUpdateInfo>(fileStream, info);
				}
				flag = true;
			}
			catch (Exception ex)
			{
				MainWindow.LogMessage(string.Format("Unable to write dynamic data to file '{0}'. Error: {1}{2}{3}", new object[]
				{
					Filename,
					ex.Message,
					Environment.NewLine,
					ex.StackTrace
				}), false);
				flag = false;
			}
			return flag;
		}

		// Token: 0x06000038 RID: 56 RVA: 0x00003EFC File Offset: 0x000020FC
		private bool TryWriteBackgroundCache(MemoryStream ms)
		{
			try
			{
				if (!Directory.Exists(this.UserCachedDataPath))
				{
					Directory.CreateDirectory(this.UserCachedDataPath);
				}
				using (FileStream fileStream = new FileStream(Path.Combine(this.UserCachedDataPath, "Background.wid"), FileMode.Create))
				{
					ms.Seek(0L, SeekOrigin.Begin);
					fileStream.Write(ms.ToArray(), 0, checked((int)ms.Length));
					MainWindow.LogMessage("Replacement background data cached. Caching metadata...", false);
				}
				if (this.TryWriteMetadataCache("BackgroundMetadata.wpb", this.CachedBackgroundInfo))
				{
					return true;
				}
			}
			catch (Exception ex)
			{
				MainWindow.LogMessage(string.Format("Failed to cache background replacement. Error: {0}{1}{2}", ex.Message, Environment.NewLine, ex.StackTrace), false);
			}
			return false;
		}

		// Token: 0x06000039 RID: 57 RVA: 0x00003FCC File Offset: 0x000021CC
		private bool TryWriteNewsCache(ObservableCollection<NewsStory> news)
		{
			try
			{
				if (!Directory.Exists(this.UserCachedDataPath))
				{
					Directory.CreateDirectory(this.UserCachedDataPath);
				}
				using (FileStream fileStream = new FileStream(Path.Combine(this.UserCachedDataPath, "News.wpb"), FileMode.Create))
				{
					Serializer.Serialize<ObservableCollection<NewsStory>>(fileStream, news);
					MainWindow.LogMessage("Cached actual news data. Caching metadata...", false);
				}
				if (this.TryWriteMetadataCache("NewsMetadata.wpb", this.CachedNewsInfo))
				{
					return true;
				}
			}
			catch (Exception ex)
			{
				MainWindow.LogMessage(string.Format("Failed to cache news stories. Error: {0}{1}{2}", ex.Message, Environment.NewLine, ex.StackTrace), false);
			}
			return false;
		}

		// Token: 0x0400000B RID: 11
		private string m_sLauncherBaseUrl;

		// Token: 0x0400000C RID: 12
		private string m_sUserCachedDataPath;

		// Token: 0x0400000D RID: 13
		private const string CachedNewsInfoFile = "NewsMetadata.wpb";

		// Token: 0x0400000E RID: 14
		private const string CachedNewsFile = "News.wpb";

		// Token: 0x0400000F RID: 15
		private const string CachedBackgroundInfoFile = "BackgroundMetadata.wpb";

		// Token: 0x04000010 RID: 16
		private const string CachedBackgroundFile = "Background.wid";

		// Token: 0x04000011 RID: 17
		private const string LauncherDataLangKey = "lang";

		// Token: 0x04000012 RID: 18
		private readonly string LauncherNewsUrl = "{0}" + string.Format("?{0}={1}", "lang", MainWindow.ThreeLetterLanguageCode);

		// Token: 0x04000013 RID: 19
		private ObservableCollection<NewsStory> _newsStories;

		// Token: 0x04000015 RID: 21
		private DynamicContentUpdateInfo CachedBackgroundInfo;

		// Token: 0x04000016 RID: 22
		private DynamicContentUpdateInfo CachedNewsInfo;

		// Token: 0x04000017 RID: 23
		private Dispatcher MainWindowDispatcher;

		// Token: 0x02000009 RID: 9
		// (Invoke) Token: 0x0600003F RID: 63
		public delegate void OnNewBackgroundReceived(object sender, EventArgs e);
	}
}
