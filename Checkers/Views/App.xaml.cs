using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using Checkers.ViewModels;

namespace Checkers
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            MainViewModel mvm = new MainViewModel();
            MainWindow mainWindow = new MainWindow();
            mainWindow.DataContext = mvm;
            mainWindow.Show();
        }
    }
}
