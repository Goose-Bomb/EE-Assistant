using System.IO.Ports;
using System.Windows;
using EEAssistant.Modules;

namespace EEAssistant
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public static SerialPort SerialPort { get; } = new SerialPort();

        protected override void OnStartup(StartupEventArgs e)
        {
            Config.Load();
            Config.Args.SerialPortArgs.TxHandler.PortWrite = SerialPort.Write;
            SerialPort.DataReceived += _SerialPort_DataReceived;
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            SerialPort.DataReceived -= _SerialPort_DataReceived;
            SerialPort.Close();

            Config.Save();
            base.OnExit(e);
        }

        private async void _SerialPort_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            int bytesToRead = SerialPort.BytesToRead;
            Config.Args.SerialPortArgs.RxHandler.BytesReceived += bytesToRead;

            byte[] buffer = new byte[bytesToRead];

            await SerialPort.BaseStream.ReadAsync(buffer, 0, bytesToRead);

            Config.Args.SerialPortArgs.RxHandler.WriteData(buffer);
        }
    }
}
