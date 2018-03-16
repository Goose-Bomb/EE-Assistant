using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace EEAssistant.Views
{
    /// <summary>
    /// ImageProcess.xaml 的交互逻辑
    /// </summary>
    public partial class ImageProcess : Grid
    {
        static BitmapImage bitmap;

        public ImageProcess()
        {
            InitializeComponent();
            RGB565_Button.IsEnabled = false;
        }

        private async void LoadImage_Button_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog()
            {
                Title = "请选择图片文件",
                Filter = "图像文件| *.png; *.jpg; *.bmp; *.gif",
            };

            if (dialog.ShowDialog() ?? false)
            {
                bitmap = await Task.Run(() =>
                {
                    try
                    {
                        var img = new BitmapImage();
                        img.BeginInit();
                        img.UriSource = new Uri(dialog.FileName, UriKind.Absolute);
                        img.CacheOption = BitmapCacheOption.OnLoad;
                        img.EndInit();
                        img.Freeze();
                        return img;
                    }
                    catch
                    {
                        return null;
                    }
                });

                imageView.Source = bitmap;
                RGB565_Button.IsEnabled = true;
            }
        }

        private async void RGB565_Convert(object sender, RoutedEventArgs e)
        {
            var fileName = Path.GetFileNameWithoutExtension(bitmap.UriSource.LocalPath);

            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Title = "请选择保存位置",
                FileName = $"{fileName}.rgb16",
                Filter = "RGB565二进制文件| *.rgb16;",
            };

            if (dialog.ShowDialog() ?? false)
            {
                var rgb16_bitmap = await Task.Run(() =>
                {
                    var _bitmap = new FormatConvertedBitmap();
                    _bitmap.BeginInit();
                    _bitmap.Source = bitmap;
                    _bitmap.DestinationFormat = PixelFormats.Bgr565;
                    _bitmap.EndInit();
                    _bitmap.Freeze();
                    return _bitmap;
                });

                int stride = rgb16_bitmap.PixelWidth * PixelFormats.Bgr565.BitsPerPixel >> 3;
                var rgb16_buffer = new byte[stride * rgb16_bitmap.PixelHeight];
                rgb16_bitmap.CopyPixels(rgb16_buffer, stride, 0);

                using (var fs = new FileStream(dialog.FileName, FileMode.Create))
                {
                    await fs.WriteAsync(rgb16_buffer, 0, rgb16_buffer.Length);
                }
            }

            //imageView.Source = rgb565_bitmap;
            //MessageBox.Show($"{bitmap.Format.BitsPerPixel}");

        }
    }
}
