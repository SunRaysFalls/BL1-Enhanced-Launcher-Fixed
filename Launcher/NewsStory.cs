using System;
using System.ComponentModel;
using ProtoBuf;

namespace Launcher
{
	// Token: 0x02000007 RID: 7
	[ProtoContract]
	public class NewsStory : INotifyPropertyChanged
	{
		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000016 RID: 22 RVA: 0x00002103 File Offset: 0x00000303
		// (set) Token: 0x06000017 RID: 23 RVA: 0x0000210B File Offset: 0x0000030B
		[ProtoMember(1)]
		public string Headline
		{
			get
			{
				return this._headline;
			}
			set
			{
				this._headline = value;
				this.NotifyPropertyChanged("Headline");
			}
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000018 RID: 24 RVA: 0x0000211F File Offset: 0x0000031F
		// (set) Token: 0x06000019 RID: 25 RVA: 0x00002127 File Offset: 0x00000327
		[ProtoMember(2)]
		public int Date
		{
			get
			{
				return this._date;
			}
			set
			{
				this._date = value;
				this.NotifyPropertyChanged("Date");
			}
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x0600001A RID: 26 RVA: 0x0000213B File Offset: 0x0000033B
		// (set) Token: 0x0600001B RID: 27 RVA: 0x00002143 File Offset: 0x00000343
		[ProtoMember(3)]
		public string Url
		{
			get
			{
				return this._url;
			}
			set
			{
				this._url = value;
				this.NotifyPropertyChanged("Url");
			}
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x0600001C RID: 28 RVA: 0x00002157 File Offset: 0x00000357
		// (set) Token: 0x0600001D RID: 29 RVA: 0x0000215F File Offset: 0x0000035F
		[ProtoMember(4)]
		public string FullStory
		{
			get
			{
				return this._fullStory;
			}
			set
			{
				this._fullStory = value;
				this.NotifyPropertyChanged("FullStory");
			}
		}

		// Token: 0x14000001 RID: 1
		// (add) Token: 0x0600001E RID: 30 RVA: 0x00003508 File Offset: 0x00001708
		// (remove) Token: 0x0600001F RID: 31 RVA: 0x00003540 File Offset: 0x00001740
		public event PropertyChangedEventHandler PropertyChanged;

		// Token: 0x06000020 RID: 32 RVA: 0x00003578 File Offset: 0x00001778
		private void NotifyPropertyChanged(string propertyName)
		{
			PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
			if (propertyChanged != null)
			{
				propertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		// Token: 0x04000006 RID: 6
		private string _headline;

		// Token: 0x04000007 RID: 7
		private int _date;

		// Token: 0x04000008 RID: 8
		private string _url;

		// Token: 0x04000009 RID: 9
		private string _fullStory;
	}
}
