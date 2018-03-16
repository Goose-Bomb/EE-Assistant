using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.Ports;
using System.Runtime.Serialization;

namespace EEAssistant.Modules
{
    [DataContract]
    class SerialPortArgs : IPortArgs, INotifyPropertyChanged
    {
        public static int[] BaudRateValues { get; } = { 4800, 9600, 14400, 19200, 38400, 57600, 115200, 192000, 256000, 460800, 921600, 1500000 };

        public static int[] DataBitsValues { get; } = { 5, 6, 7, 8, 9 };

        public static string[] ParityValues { get; } = { "None", "Odd", "Even", "Mark", "Space" };

        public static string[] StopBitsValues { get; } = { "One", "OnePointFive", "Two" };

        public static ObservableCollection<string> AvailablePorts { get; set; } = new ObservableCollection<string>();

        private bool _IsOpen;

        public bool IsOpen
        {
            get => _IsOpen;
            set
            {
                if (value)
                {
                    MainWindow.SetWindowTitle("串口已打开");
                }
                else
                {
                    MainWindow.SetWindowTitle("待命中");
                }
                _IsOpen = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsOpen)));
            }
        }

        private int _SelectedPortIndex;

        public int SelectedPortIndex
        {
            get => _SelectedPortIndex;
            set
            {
                _SelectedPortIndex = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedPortIndex)));
            }
        }

        [DataMember]
        public int BaudRate
        {
            get => App.SerialPort.BaudRate;
            set { App.SerialPort.BaudRate = value; }
        }

        [DataMember]
        public int DataBits
        {
            get => App.SerialPort.DataBits;
            set { App.SerialPort.DataBits = value; }
        }

        [DataMember]
        public Parity Parity
        {
            get => App.SerialPort.Parity;
            set { App.SerialPort.Parity = value; }
        }

        [DataMember]
        public StopBits StopBits
        {
            get => App.SerialPort.StopBits;
            set { App.SerialPort.StopBits = value; }
        }

        [DataMember]
        public RxHandler RxHandler { get; set; }

        [DataMember]
        public TxHandler TxHandler { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
