using System;
using ProtoBuf;

namespace Launcher
{
	// Token: 0x02000005 RID: 5
	[ProtoContract]
	public class DynamicContentUpdateInfo
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x0600000A RID: 10 RVA: 0x000020AE File Offset: 0x000002AE
		// (set) Token: 0x0600000B RID: 11 RVA: 0x000020B6 File Offset: 0x000002B6
		[ProtoMember(1)]
		public int LastUpdated { get; set; }

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x0600000C RID: 12 RVA: 0x000020BF File Offset: 0x000002BF
		// (set) Token: 0x0600000D RID: 13 RVA: 0x000020C7 File Offset: 0x000002C7
		[ProtoMember(2)]
		public string DataUrl { get; set; }
	}
}
