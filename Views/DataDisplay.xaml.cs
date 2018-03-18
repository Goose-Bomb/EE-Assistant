using EEAssistant.Modules;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace EEAssistant.Views
{
    /// <summary>
    /// DataProcess.xaml 的交互逻辑
    /// </summary>
    public partial class DataDisplay : Grid
    {
        private IPortArgs portArgs;

        public DataDisplay()
        {
            InitializeComponent();
        }

        private void DataDisplay_Panel_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.Name.StartsWith("SerialPort"))
            {
                this.portArgs = Config.Args.SerialPortArgs;
            }
            this.DataContext = portArgs;

            portArgs.RxHandler.TextReceived += (string str) =>
            Dispatcher.Invoke(() =>
            {
                ReceiveBox.AppendText(str);
                ReceiveBox.ScrollToEnd();
            });

            portArgs.RxHandler.TextCleared += () => ReceiveBox.Clear();
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            if (portArgs.IsOpen)
            {
                SendButton.IsEnabled = false;

                if (portArgs.TxHandler.IsImportFile)
                {
                    await portArgs.TxHandler.TransmitFileAsync();
                }
                else
                {
                    await portArgs.TxHandler.TransmitTextAsync();
                }

                SendButton.IsEnabled = true;
            }
            else
            {
                if (portArgs is SerialPortArgs)
                {
                    MainWindow.ShowMessage("串口未打开", null);
                }
                else
                {
                    MainWindow.ShowMessage("网络端口未打开", null);
                }
            }
        }

        private void EditRedirectFilePathButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.SaveFileDialog()
            {
                Title = "重定向接收数据至文件",
            };

            if (dialog.ShowDialog() ?? false)
            {
                portArgs.RxHandler.RedirectFilePath = dialog.FileName;
                portArgs.RxHandler.IsRedirectToFile = true;
            } 
        }
    }
}
