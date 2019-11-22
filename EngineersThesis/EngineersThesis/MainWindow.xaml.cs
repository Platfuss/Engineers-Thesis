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
using EngineersThesis.General;

namespace EngineersThesis
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private WelcomeScreen welcomeScreen;
        private SqlHandler sqlHandler;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            welcomeScreen.ShowDialog();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            welcomeScreen = new WelcomeScreen()
            {
                Owner = this
            };
            welcomeScreen.ShowDialog();
            sqlHandler = welcomeScreen.SqlHandler;
            sqlHandler.PrepareDatabase();
            var sth = sqlHandler.ExecuteCommand("Select * from `new_schema`.`units`;");
            dataGrid.ItemsSource = sth.Tables[0].DefaultView;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            welcomeScreen.Close();
        }
    }
}
