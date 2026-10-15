using System;
using System.Collections.ObjectModel;
using ProtoBuf;

namespace Launcher
{
	// Token: 0x02000006 RID: 6
	[ProtoContract]
	public class AvailableData
	{
		// Token: 0x17000003 RID: 3
		// (get) Token: 0x0600000F RID: 15 RVA: 0x000020D0 File Offset: 0x000002D0
		// (set) Token: 0x06000010 RID: 16 RVA: 0x000020D8 File Offset: 0x000002D8
		[ProtoMember(1)]
		public DynamicContentUpdateInfo NewsInfo { get; set; }

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000011 RID: 17 RVA: 0x000020E1 File Offset: 0x000002E1
		// (set) Token: 0x06000012 RID: 18 RVA: 0x000020E9 File Offset: 0x000002E9
		[ProtoMember(2)]
		public DynamicContentUpdateInfo BackgroundInfo { get; set; }

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000013 RID: 19 RVA: 0x000020F2 File Offset: 0x000002F2
		// (set) Token: 0x06000014 RID: 20 RVA: 0x000020FA File Offset: 0x000002FA
		[ProtoMember(3)]
		public ObservableCollection<NewsStory> NewsStories { get; set; }
	}
}
