using EEAssistant.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EEAssistant.Views
{
    /// <summary>
    /// RxCfg.xaml 的交互逻辑
    /// </summary>
    public partial class RxCfg : GroupBox
    {
        private IPortArgs portArgs;

        public RxCfg()
        {
            InitializeComponent();
        }

        private void RxCfg_Panel_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.Name.StartsWith("SerialPort"))
            {
                this.portArgs = Config.Args.SerialPortArgs;
            }
            /*else
            {
                this.portArgs = Config.Args.NetPortArgs;
            }*/
            this.DataContext = portArgs.RxHandler;
        }

        private async void SaveToFileButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.SaveFileDialog()
            {
                Title = "保存接收数据至文件",
            };

            if (dialog.ShowDialog() ?? false)
            {
                await portArgs.RxHandler.SaveRxDataToFile(dialog.FileName);
            }
        }

        private void ClearDataButton_Click(object sender, RoutedEventArgs e) => portArgs.RxHandler.ClearRxData();
    }
}
