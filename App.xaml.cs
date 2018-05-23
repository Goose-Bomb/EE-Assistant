using System;
using System.IO;
using System.IO.Ports;
using System.Windows;
using System.Windows.Threading;
using EEAssistant.Modules;

namespace EEAssistant
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Config.Load();
            base.OnStartup(e);
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            using (var sr = new StreamWriter("crash_report.log", true, System.Text.Encoding.UTF8))
            {
                sr.WriteLine($"Time: {DateTime.Now}");
                sr.WriteLine($"[Exception Info]\n {e.Exception.Message}");
                sr.WriteLine($"[Exception Source]\n {e.Exception.Source}");
                sr.WriteLine($"[Stack Trace]\n {e.Exception.StackTrace}");
            }

            var result = MessageBox.Show("发生了未处理异常！\n是否查看日志文件？", ">w<", MessageBoxButton.YesNo, MessageBoxImage.Error);

            if(result == MessageBoxResult.Yes)
            {
                System.Diagnostics.Process.Start("crash_report.log");
            }

            this.Shutdown(-1);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            SerialPortArgs.Instance.Close();
            Config.Save();
            base.OnExit(e);
        }
    }
}
