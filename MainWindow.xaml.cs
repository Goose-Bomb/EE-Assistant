using EEAssistant.Helpers;
using EEAssistant.Modules;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Windows;

namespace EEAssistant
{
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public static void SetWindowTitle(string titile)
        {
            (Application.Current.MainWindow as MetroWindow).Title = $"{titile} - EE助手";
        }

        public static void ShowMessage(string title, string message)
        {
            (Application.Current.MainWindow as MetroWindow).ShowMessageAsync(title, message, MessageDialogStyle.Affirmative);
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            USBPortWatcher.Init(this);
            this.DataContext = Config.Args;
        }
    }
}
