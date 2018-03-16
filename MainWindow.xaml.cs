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
            this.Loaded += (s,e) => USBPortWatcher.Init(this);
            this.DataContext = Config.Args;
        }

        public static void SetWindowTitle(string titile)
        {
            (Application.Current.MainWindow as MetroWindow).Title = $"{titile} - EE助手";
        }

        public static void ShowMessage(string title, string message)
        {
            (Application.Current.MainWindow as MetroWindow).ShowMessageAsync(title, message, MessageDialogStyle.Affirmative);
        }

    }
}
