using System;
using System.Data;
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

        private String warehouseShortcut;
        private String warehouseName;

        private int selectedRow;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            welcomeScreen = new WelcomeScreen()
            {
                Owner = this
            };
            ChooseDatabase(new object(), new RoutedEventArgs());
            if (sqlHandler.Database != null)
            {
                OpenWarehousesManager(new object(), new RoutedEventArgs());
            }
        }

        private void ChooseDatabase(object sender, RoutedEventArgs e)
        {
            welcomeScreen.ShowDialog();
            sqlHandler = welcomeScreen.SqlHandler;
            sqlHandler.PrepareDatabase();
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
               
                var dataSet = sqlHandler.ExecuteCommand(SqlSelectCommands.ShowProductsInWarehouse(sqlHandler.Database, warehouseName));
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
            var productEditor = new ProductEditor(sqlHandler)
            {
                Owner = this
            };
            productEditor.ShowDialog();
        }

        private void OpenWarehousesManager(object sender, RoutedEventArgs e)
        {
            var warehousesManager = new WarehousesManager(sqlHandler)
            {
                Owner = this
            };
            warehousesManager.ShowDialog();
            if (warehousesManager.Accepted)
            {
                warehouseName = warehousesManager.WarehouseName;
                warehouseShortcut = warehousesManager.Shortcut;
                SetDataGrid();
            }
        }

        private void OnEditProduct(object sender, RoutedEventArgs e)
        {
            var rowData = (DataRowView)dataGrid.SelectedItems[0];
            var productEditor = new ProductEditor(sqlHandler, (String)rowData[0], (String)rowData[1], (double)rowData[3], (int)rowData[4])
            {
                Owner = this
            };
            productEditor.ShowDialog();
            SetDataGrid();
        }

        private void OnDataGridSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedRow = dataGrid.SelectedIndex;
            if (selectedRow != -1)
            {
                EditProductButton.IsEnabled = true;
            }
        }
    }
}
