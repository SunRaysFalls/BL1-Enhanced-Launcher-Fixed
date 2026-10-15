using System;
using System.Globalization;
using System.Threading;
using System.Windows.Data;

namespace Launcher
{
	// Token: 0x02000031 RID: 49
	public class UnixTimestampToTextDate : IValueConverter
	{
		// Token: 0x06000124 RID: 292 RVA: 0x000071BC File Offset: 0x000053BC
		public static int DateTimeToUnixTime(DateTime dt)
		{
			return checked((int)(dt - UnixTimestampToTextDate.dtEpoch).TotalSeconds);
		}

		// Token: 0x06000125 RID: 293 RVA: 0x000071E0 File Offset: 0x000053E0
		public static DateTime UnixTimeToDateTime(int UnixTime)
		{
			return UnixTimestampToTextDate.dtEpoch.AddSeconds((double)UnixTime);
		}

		// Token: 0x06000126 RID: 294 RVA: 0x000071FC File Offset: 0x000053FC
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			CultureInfo currentUICulture = Thread.CurrentThread.CurrentUICulture;
			return UnixTimestampToTextDate.UnixTimeToDateTime((int)value).ToString(currentUICulture.DateTimeFormat.MonthDayPattern.Replace("MMMM", "MMM"));
		}

		// Token: 0x06000127 RID: 295 RVA: 0x00002A61 File Offset: 0x00000C61
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		// Token: 0x040000E2 RID: 226
		private static readonly DateTime dtEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);
	}
}
