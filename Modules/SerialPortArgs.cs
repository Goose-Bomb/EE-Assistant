using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.Ports;
using System.Runtime.Serialization;

namespace EEAssistant.Modules
{
    [DataContract]
    class SerialPortArgs : IPortArgs, INotifyPropertyChanged
    {
        #region 公共属性

        public static SerialPort Instance { get => _SerialPort; }

        public static ObservableCollection<string> AvailablePorts { get; set; } = new ObservableCollection<string>();

        public static int[] BaudRateValues { get; } = { 4800, 9600, 14400, 19200, 38400, 57600, 115200, 192000, 256000, 460800, 921600, 1500000 };

        public static int[] DataBitsValues { get; } = { 5, 6, 7, 8, 9 };

        public static string[] ParityValues { get; } = { "None", "Odd", "Even", "Mark", "Space" };

        public static string[] StopBitsValues { get; } = { "One", "OnePointFive", "Two" };

        public bool IsOpen
        {
            get => _IsOpen;
            set
            {
                if (value)
                {
                    _SerialPort.Encoding = Config.Args.Encoding;
                    _SerialPort.DataReceived += SerialPort_DataReceived;
                    MainWindow.SetWindowTitle("串口已打开");
                }
                else
                {
                    _SerialPort.DataReceived -= SerialPort_DataReceived;
                    MainWindow.SetWindowTitle("待命中");
                }

                _IsOpen = value;
                TxHandler.IsOpen = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsOpen)));
            }
        }

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
            get => _SerialPort.BaudRate;
            set { _SerialPort.BaudRate = value; }
        }

        [DataMember]
        public int DataBits
        {
            get => _SerialPort.DataBits;
            set { _SerialPort.DataBits = value; }
        }

        [DataMember]
        public Parity Parity
        {
            get => _SerialPort.Parity;
            set { _SerialPort.Parity = value; }
        }

        [DataMember]
        public StopBits StopBits
        {
            get => _SerialPort.StopBits;
            set { _SerialPort.StopBits = value; }
        }

        [DataMember]
        public RxHandler RxHandler
        {
            get => _RxHandler;
            set { _RxHandler = value; }
        }

        [DataMember]
        public TxHandler TxHandler
        {
            get => _TxHandler;
            set { _TxHandler = value; }
        }

        #endregion

        #region 私有对象

        private static SerialPort _SerialPort = new SerialPort();
        private RxHandler _RxHandler;
        private TxHandler _TxHandler;

        private bool _IsOpen;
        private int _SelectedPortIndex;

        #endregion

        #region 公共方法

        #endregion

        #region 私有方法

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if(RxHandler.IsRedirectToFile || RxHandler.IsHexDisplay)
            {
                int bytesToRead = _SerialPort.BytesToRead;
                byte[] buffer = new byte[bytesToRead];
                _SerialPort.BaseStream.Read(buffer, 0, bytesToRead);
                RxHandler.WriteBytes(buffer);
            }
            else
            {
                RxHandler.WriteString(_SerialPort.ReadExisting());
            }
        }

        #endregion

        #region 公共事件

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
