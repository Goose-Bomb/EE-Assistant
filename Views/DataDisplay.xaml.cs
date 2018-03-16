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
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            if (portArgs.IsOpen)
            {
                portArgs.TxHandler.StartTransmit();
            }
            else
            {
                if(portArgs  is SerialPortArgs)
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
            }
        }
    }
}
