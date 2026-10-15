using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace Launcher
{
	// Token: 0x02000004 RID: 4
	internal class CustomScrollViewer : ScrollViewer
	{
		// Token: 0x06000008 RID: 8 RVA: 0x00002096 File Offset: 0x00000296
		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			base.OnMouseLeftButtonDown(e);
			e.Handled = false;
		}
	}
}
