using System;
using System.Globalization;
using System.Windows.Data;

namespace Launcher
{
	// Token: 0x0200002D RID: 45
	public class NegateNumber : IValueConverter
	{
		// Token: 0x06000116 RID: 278 RVA: 0x00006F98 File Offset: 0x00005198
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is int)
			{
				return checked(0 - (int)value);
			}
			if (value is float)
			{
				return -(float)value;
			}
			if (value is double)
			{
				return -(double)value;
			}
			MainWindow.LogException(new Exception(MainWindow.Instance.GetResourceString("ConfigFileLoadError_Text")), string.Format("Supplied value was of unexpected type {0}.", value.GetType()));
			return null;
		}

		// Token: 0x06000117 RID: 279 RVA: 0x00002A61 File Offset: 0x00000C61
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
