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

                        if (_ImportFileStream != null)
                        {
                            _ImportFileStream.Close();
                        }
                        _ImportFileStream = new FileStream(_ImportFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);

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
            set
            {
                if (value)
                {
                    if(_IsImportFile)
                    {
                        _TxTimer.Tick += TxTimer_File_Tick;
                    }
                    else
                    {
                        _TxTimer.Tick += TxTimer_Buffer_Tick;
                    }
                }
                else
                {
                    if (_IsImportFile)
                    {
                        _TxTimer.Tick -= TxTimer_File_Tick;
                    }
                    else
                    {
                        _TxTimer.Tick -= TxTimer_Buffer_Tick;
                    }

                    _TxTimer.Stop();
                }

                _IsAutoTiming = value;
            }
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
            set
            {
                value = (value < 8) ? 8 : value;
                value = (value > 4096) ? 4096 : value;
                _BytesPerPacket = value;
            }
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

        public Stream BaseStream
        {
            get => _BaseStream;
            set
            {
                _BaseStream = value;
            }
        }

        #endregion

        #region 公共方法

        public async Task TransmitTextAsync()
        {
            byte[] buffer = Config.Args.Encoding.GetBytes(_SendText);

            if (_IsAutoTiming)
            {
                await _TransmitBuffer.WriteAsync(buffer, 0, buffer.Length);
                _TxTimer.Start();
            }
            else
            {
                await _BaseStream.WriteAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
                BytesTransmitted += buffer.Length;
            }
        }

        public async Task TransmitFileAsync()
        {
            SendFileProgress = 0.0;

            if (_IsAutoTiming)
            {
                _TxTimer.Start();
            }
            else
            {
                byte[] buffer = new byte[1024];
                int bytesCount;

                MainWindow.SetWindowTitle("正在发送文件");
                do
                {
                    bytesCount = await _ImportFileStream.ReadAsync(buffer, 0, 1024);
                    await _BaseStream.WriteAsync(buffer, 0, bytesCount);

                    BytesTransmitted += bytesCount;
                    SendFileProgress += (double)bytesCount / _ImportFileInfo.Length;

                } while (bytesCount != 0);

                _ImportFileStream.Seek(0, SeekOrigin.Begin);
                MainWindow.SetWindowTitle("文件发送完成");
            }
        }

        #endregion

        #region 私有对象

        private static DispatcherTimer _TxTimer = new DispatcherTimer(DispatcherPriority.Background);

        private Stream _BaseStream;
        private static MemoryStream _TransmitBuffer = new MemoryStream();
        private FileInfo _ImportFileInfo;
        private FileStream _ImportFileStream;

        private string _SendText;
        private string _ImportFilePath;
        private bool _IsImportFile;
        private bool _IsAutoTiming;
        private bool _IsRepeat;
        private bool _HasAddtionalLineBreak;
        private string _AddtionalLineBreak;
        private int _BytesPerPacket;
        private int _BytesTransmitted;
        private double _SendFileProgress;

        #endregion

        #region 私有方法

        private void TxTimer_File_Tick(object sender, EventArgs e)
        {
            byte[] buffer = new byte[BytesPerPacket];
            int bytesCount = _ImportFileStream.Read(buffer, 0, BytesPerPacket);
            
            if(bytesCount == 0)
            {
                _ImportFileStream.Seek(0, SeekOrigin.Begin);

                if (_IsRepeat)
                {
                    SendFileProgress = 0.0;
                }
                else
                {
                    _TxTimer.Stop();
                }
            }

            _BaseStream.Write(buffer, 0, bytesCount);
            BytesTransmitted += bytesCount;
            SendFileProgress += (double)bytesCount / _ImportFileInfo.Length;
        }

        private void TxTimer_Buffer_Tick(object sender, EventArgs e)
        {
            byte[] buffer = new byte[BytesPerPacket];
            int bytesCount = _TransmitBuffer.Read(buffer, 0, BytesPerPacket);

            if (bytesCount == 0)
            {
                _TransmitBuffer.Seek(0, SeekOrigin.Begin);

                if (!_IsRepeat)
                {
                    _TxTimer.Stop();
                }
            }

            _BaseStream.Write(buffer, 0, bytesCount);
            BytesTransmitted += bytesCount;
        }

        #endregion

        #region 公共事件

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

    }
}
