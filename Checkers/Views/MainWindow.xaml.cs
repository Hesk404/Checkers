using System.Windows;

namespace Checkers
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

		private void MenuItemExit_OnClick(object sender, RoutedEventArgs e)
		{
			Close();
		}
    }
}
