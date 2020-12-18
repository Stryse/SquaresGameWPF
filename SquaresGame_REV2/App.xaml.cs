using SquaresGame.Model;
using SquaresGame.Persistence;
using SquaresGame_REV2.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace SquaresGame_REV2
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private MainWindow mainWindow;
        private MainWindowViewModel mainViewModel;

        public App()
        {
            this.Startup += Init;
        }

        public void Init(object sender, StartupEventArgs e)
        {
            mainWindow = new MainWindow();
            mainViewModel = new MainWindowViewModel();
            mainWindow.DataContext = mainViewModel;
            mainWindow.Show();
        }

    }
}
