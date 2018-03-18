using EEAssistant.Modules;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Threading.Tasks;
using System.Linq;

namespace EEAssistant.Views
{
    public partial class SerialPortCfg : Grid
    {
        public SerialPortCfg()
        {
            InitializeComponent();
        }

        /*
        private void Refresh_SerialPort()
        {
            //availablePorts = SerialPort.GetPortNames();
            availablePorts.Clear();

            using (var searcher = new ManagementObjectSearcher(
                "root\\CIMV2",
                "SELECT * FROM Win32_PnPEntity WHERE ClassGuid=\"{4D36E978-E325-11CE-BFC1-08002BE10318}\""))
            {
                foreach (var device in searcher.Get())
                {
                    availablePorts.Add(device.GetPropertyValue("Name").ToString());
                }
            }
        }*/

        private async void SwitchButton_Checked(object sender, RoutedEventArgs e)
        {
            if (!SerialPortArgs.AvailablePorts.Any())
            {
                MainWindow.ShowMessage("打开失败", "当前无可用串口");
                Config.Args.SerialPortArgs.IsOpen = false;
                return;
            }

            App.SerialPort.PortName = SerialPortArgs.AvailablePorts[portsBox.SelectedIndex];

            try
            {
                SwitchButton.IsEnabled = false;
                await Task.Run(() => App.SerialPort.Open());

                Config.Args.SerialPortArgs.TxHandler.BaseStream = App.SerialPort.BaseStream;
                Config.Args.SerialPortArgs.RxHandler.BaseStream = App.SerialPort.BaseStream;
            }
            catch (UnauthorizedAccessException)
            {
                MainWindow.ShowMessage($"{App.SerialPort.PortName} 打开失败", "该串口正在被占用，拒绝访问");
                Config.Args.SerialPortArgs.IsOpen = false;
            }
            finally
            {
                SwitchButton.IsEnabled = true;
            }
        }

        private async void SwitchButton_Unchecked(object sender, RoutedEventArgs e)
        {
            SwitchButton.IsEnabled = false;
            await Task.Run(() => App.SerialPort.Close());
            SwitchButton.IsEnabled = true;
        }
    }
}
