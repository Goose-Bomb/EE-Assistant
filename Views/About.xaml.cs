using System.Windows.Controls;
using System.Windows.Input;

namespace EEAssistant.Views
{
    /// <summary>
    /// About.xaml 的交互逻辑
    /// </summary>
    public partial class About : Grid
    {
        public About()
        {
            InitializeComponent();
        }

        private void GithubLink_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            System.Diagnostics.Process.Start((sender as TextBlock).Text);
        }
    }
}
