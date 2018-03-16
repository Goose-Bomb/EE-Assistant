using System;
using System.Globalization;
using System.IO.Ports;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace EEAssistant.Helpers
{
    public class TimeSpanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var time = (TimeSpan)value;

            if(time != TimeSpan.Zero)
            {
                return $"{time.TotalSeconds}s";
            }
            else return "0.1s";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var str = value as string;

            if (str.EndsWith("s"))
            {
                str = str.TrimEnd('s');
            }

            if (double.TryParse(str, out double seconds))
            {
                if (seconds < 0.05)
                {
                    seconds = 0.05;
                }

                return new TimeSpan((long)(seconds * 10000000));
            }

            return new TimeSpan(5000000);
        }
    }

    public class NegativeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !((bool)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class EncodingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var encoding = value as Encoding;
            if (encoding == Encoding.Default) return "GBK";
            if (encoding == Encoding.UTF8) return "UTF-8";
            if (encoding == Encoding.Unicode) return "Unicode";
            return "GBK";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch(value as string)
            {
                case "GBK": return Encoding.Default;
                case "UTF-8": return Encoding.UTF8;
                case "Unicode": return Encoding.Unicode;
                default: return Encoding.Default;
            }
        }
    }

    public class ParityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Enum.GetName(typeof(Parity), value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Enum.Parse(typeof(Parity), (string)value);
        }
    }

    public class StopBitsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Enum.GetName(typeof(StopBits), value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Enum.Parse(typeof(StopBits), (string)value);
        }
    }
}
