using System.Windows;

namespace LGSTrayUI
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow(UserSettingsWrapper userSettings)
        {
            InitializeComponent();
            DataContext = userSettings;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
