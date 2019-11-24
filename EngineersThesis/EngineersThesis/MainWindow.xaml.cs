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

        private void ChooseDatabase(object sender, RoutedEventArgs e)
        {
            welcomeScreen.ShowDialog();
            sqlHandler = welcomeScreen.SqlHandler;
            sqlHandler.PrepareDatabase();
            SetDataGrid();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            welcomeScreen = new WelcomeScreen()
            {
                Owner = this
            };
            ChooseDatabase(new object(), new RoutedEventArgs());
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            welcomeScreen.Close();
        }

        private void SetDataGrid()
        {
            var database = sqlHandler.Database;
            if (database != null)
            {
                AddNewProductButton.IsEnabled = OpenWarehousesManagerButton.IsEnabled = true;
               
                var dataSet = sqlHandler.ExecuteCommand(SqlCommands.ShowProductsCommand(sqlHandler.Database));
                dataGrid.ItemsSource = dataSet.Tables[0].DefaultView;
                foreach (var column in dataGrid.Columns)
                {
                    column.MinWidth = 100;
                    if (SqlConstants.translations.TryGetValue(column.Header.ToString(), out String result))
                        column.Header = result;
                }
            }
            else
            {
                AddNewProductButton.IsEnabled = OpenWarehousesManagerButton.IsEnabled = false;
            }
        }

        private void AddNewProduct(object sender, RoutedEventArgs e)
        {
            var productEdition = new ProductEdition(sqlHandler)
            {
                Owner = this
            };
            productEdition.ShowDialog();
        }

        private void OpenWarehousesManager(object sender, RoutedEventArgs e)
        {
            var warehousesManager = new WarehousesManager(sqlHandler)
            {
                Owner = this
            };
            warehousesManager.ShowDialog();
        }
    }
}
