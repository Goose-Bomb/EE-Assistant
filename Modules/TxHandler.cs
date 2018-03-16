using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace EEAssistant.Modules
{
    [DataContract]
    class TxHandler : INotifyPropertyChanged
    {
        #region 公共属性

        public static string[] LineBreakSelections { get; } = { "\\n (LF)", "\\r (CR)", "\\r\\n (CRLF)" };

        public string SendText
        {
            get => _SendText;
            set
            {
                _SendText = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SendText)));
            }
        }

        public bool IsImportFile
        {
            get => _IsImportFile;
            set
            {
                if (value)
                {
                    SendFileProgress = 0.0;

                    if (File.Exists(_ImportFilePath))
                    {
                        _ImportFileInfo = new FileInfo(_ImportFilePath);

                        if (_ImportFs != null)
                        {
                            _ImportFs.Close();
                        }
                        _ImportFs = new FileStream(_ImportFilePath, FileMode.Open, FileAccess.Read, FileShare.Read, BytesPerPacket);

                        SendText = $"发送文件：\n{_ImportFileInfo.FullName}\n文件大小：{_ImportFileInfo.Length} Bytes";
                    }
                    else
                    {
                        SendText = "请选择发送文件";
                    }
                }
                else
                {
                    SendText = string.Empty;
                }

                _IsImportFile = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsImportFile)));
            }
        }

        public double SendFileProgress
        {
            get => _SendFileProgress;
            set
            {
                _SendFileProgress = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SendFileProgress)));
            }
        }

        public string ImportFilePath
        {
            get => _ImportFilePath;
            set { _ImportFilePath = value; }
        }

        public int BytesTransmitted
        {
            get => _BytesTransmitted;
            set
            {
                _BytesTransmitted = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BytesTransmitted)));
            }
        }

        [DataMember]
        public bool IsAutoTiming
        {
            get => _IsAutoTiming;
            set { _IsAutoTiming = value; }
        }

        [DataMember]
        public bool IsRepeat
        {
            get => _IsRepeat;
            set { _IsRepeat = value; }
        }

        [DataMember]
        public int BytesPerPacket
        {
            get => _BytesPerPacket;
            set { _BytesPerPacket = value; }
        }

        [DataMember]
        public bool HasAddtionalLineBreak
        {
            get => _HasAddtionalLineBreak;
            set { _HasAddtionalLineBreak = value; }
        }

        [DataMember]
        public string AddtionalLineBreak
        {
            get => _AddtionalLineBreak;
            set { _AddtionalLineBreak = value; }
        }

        [DataMember]
        public TimeSpan SendInterval
        {
            get => _TxTimer.Interval;
            set { _TxTimer.Interval = value; }
        }

        public Action<byte[], int, int> PortWrite
        {
            get => _PortWrite;
            set
            {
                _PortWrite = value;
            }
        }

        #endregion

        #region 公共方法

        public void StartTransmit()
        {
            
            
            if (IsImportFile)
            {

            }
            else
            {
                byte[] buffer = Config.Args.Encoding.GetBytes(SendText);
            }

            if(IsAutoTiming)
            {
                _TxTimer.Tick += TxTimer_Tick;
                _TxTimer.Start();
            }
        }

        private void TxTimer_Tick(object sender, EventArgs e)
        {
            byte[] buffer = new byte[BytesPerPacket];
            int bytesCount;

            if (IsImportFile)
            {
                SendFileProgress = (double)BytesTransmitted / _ImportFileInfo.Length;
                bytesCount = _ImportFs.Read(buffer, 0, BytesPerPacket);  
            }
            else
            {
                bytesCount = _TransmitBuffer.Read(buffer, 0, BytesPerPacket);
            }

            _PortWrite(buffer, 0, bytesCount);
            BytesTransmitted += bytesCount;
        }

        #endregion

        #region 私有对象

        private string _SendText;
        private string _ImportFilePath;
        private bool _IsImportFile;
        private bool _IsRepeat;
        private bool _IsAutoTiming;
        private bool _HasAddtionalLineBreak;
        private string _AddtionalLineBreak;
        private int _BytesTransmitted;
        private int _BytesPerPacket;
        private double _SendFileProgress;

        private Action<byte[], int, int> _PortWrite;
        private static DispatcherTimer _TxTimer = new DispatcherTimer(DispatcherPriority.Background);
        private static MemoryStream _TransmitBuffer;

        private FileInfo _ImportFileInfo;
        private FileStream _ImportFs;

        #endregion

        #region 私有方法
        #endregion

        #region 公共事件

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

    }
}
