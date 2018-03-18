using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Windows.Threading;

namespace EEAssistant.Modules
{
    [DataContract]
    class RxHandler : INotifyPropertyChanged
    {
        #region 公共属性

        public bool IsDisplayPaused
        {
            get => _IsDisplayPaused;
            set
            {
                _IsDisplayPaused = value;
            }
        }
        
        public int BytesReceived
        {
            get => _BytesReceived;
            set
            {
                _BytesReceived = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BytesReceived)));
            }
        }
        
        [DataMember(Order = 0)]
        public string RedirectFilePath
        {
            get => _RedirectFilePath;
            set
            {
                _RedirectFilePath = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RedirectFilePath)));
            }
        }
        
        [DataMember(Order = 1)]
        public bool IsRedirectToFile
        {
            get => _IsRedirectToFile;
            set
            {
                if (value)
                {
                    if (string.IsNullOrEmpty(_RedirectFilePath))
                    {
                        var dialog = new Microsoft.Win32.SaveFileDialog()
                        {
                            Title = "重定向接收数据至文件",
                        };

                        if (dialog.ShowDialog() ?? false)
                        {
                            RedirectFilePath = dialog.FileName;
                        }
                        else return;
                    }

                    _RedirectedFileStream = new FileStream(RedirectFilePath, FileMode.Append, FileAccess.Write);
                }
                else
                {
                    if(_RedirectedFileStream != null)
                    {
                        _RedirectedFileStream.Close();
                    }

                    if(_ReceiveBuffer == null)
                    {
                        _ReceiveBuffer = new List<byte>();
                    }
                    else
                    {
                        _ReceiveBuffer.Clear();
                    }
                }

                _IsRedirectToFile = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsRedirectToFile)));
            }
        }

        [DataMember(Order = 2)]
        public bool IsHexDisplay
        {
            get => _IsHexDisplay;
            set
            {
                _IsHexDisplay = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsHexDisplay)));
            }
        }

        public Stream BaseStream
        {
            get => _BaseStream;
            set
            {
                _BaseStream = value;
                //_BaseStreamReader = new StreamReader(value, Encoding.GetEncoding(Config.Args.CodePage));
            }
        }
        #endregion

        #region 公共方法

        public void WriteData(byte[] data)
        {
            if (_IsRedirectToFile)
            {
                _RedirectedFileStream.Write(data, 0, data.Length);
            }
            else
            {
                _ReceiveBuffer.AddRange(data);

                if (_IsHexDisplay)
                {
                    var sb = new StringBuilder(data.Length * 3);
                    foreach (var b in data)
                    {
                        sb.Append(b.ToString("X2")).Append(' ');
                    }

                    TextReceived?.Invoke(sb.ToString());
                }
                else
                {
                    TextReceived?.Invoke(Config.Args.Encoding.GetString(data));
                } 
            }
        }

        public void ClearRxData()
        {
            TextCleared?.Invoke();
            BytesReceived = 0;
        }

        public async Task SaveRxDataToFile(string path)
        {
            try
            {
                using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    await fs.WriteAsync(_ReceiveBuffer.ToArray(), 0, _ReceiveBuffer.Count);
                }
            }
            catch (UnauthorizedAccessException)
            {
                MainWindow.ShowMessage("写入失败", "该文件正在被占用！");
            }
        }

        #endregion

        #region 私有对象

        private Stream _BaseStream;
        private FileStream _RedirectedFileStream;
        private List<byte> _ReceiveBuffer;

        private string _RedirectFilePath;
        private int _BytesReceived;
        private bool _IsDisplayPaused;
        private bool _IsRedirectToFile;
        private bool _IsHexDisplay;

        #endregion

        #region 私有方法

        #endregion

        #region 公共事件

        public event PropertyChangedEventHandler PropertyChanged;
        public event Action<string> TextReceived;
        public event Action TextCleared;

        #endregion
    }
}
