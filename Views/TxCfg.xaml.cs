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
    public partial class TxCfg : GroupBox
    {
        private IPortArgs portArgs;

        public TxCfg()
        {
            InitializeComponent();
        }

        private void TxCfg_Panel_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.Name.StartsWith("SerialPort"))
            {
                this.portArgs = Config.Args.SerialPortArgs;
            }
            /*else
            {
                this.portArgs = Config.Args.NetPortArgs;
            }*/
            this.DataContext = portArgs.TxHandler;
        }

        private void ImportFile_Button_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog()
            {
                Title = "请选择要加载的文件",
            };

            if (dialog.ShowDialog() ?? false)
            {
                portArgs.TxHandler.ImportFilePath = dialog.FileName;
                portArgs.TxHandler.IsImportFile = true;
            }
        }
    }
}
