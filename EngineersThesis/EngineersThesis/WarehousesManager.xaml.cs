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
using System.Windows.Shapes;
using EngineersThesis.General;

namespace EngineersThesis
{
    /// <summary>
    /// Interaction logic for WarehousesManager.xaml
    /// </summary>
    public partial class WarehousesManager : Window
    {
        public String Shortcut { get; private set; }
        public String WarehouseName { get; private set; }
        public bool Accepted { get; private set; }

        private SqlHandler sqlHandler;
        private bool editMode = false;

        public WarehousesManager(SqlHandler handler)
        {
            sqlHandler = handler;
            InitializeComponent();
            SetDataGrid();
        }

        private void SetDataGrid()
        {
            var database = sqlHandler.Database;
            var dataSet = sqlHandler.ExecuteCommand(SqlSelectCommands.ShowWarehouses(sqlHandler.Database));
            dataGrid.ItemsSource = dataSet.Tables[0].DefaultView;
        }

        private void OnColumnsGenerated(object sender, EventArgs e)
        {
            foreach (var column in dataGrid.Columns)
            {
                column.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
                if (SqlConstants.translations.TryGetValue(column.Header.ToString(), out String result))
                    column.Header = result;
            }
        }

        private void AddNewWarehouse(object sender, RoutedEventArgs e)
        {
            mainGrid.Visibility = Visibility.Hidden;
            editWarehouseGrid.Visibility = Visibility.Visible;
            upperTextBox.Text = centerTextBox.Text = "";
            confirmEditingButton.IsEnabled = false;
            editMode = false;
        }

        private void EditWarehouse(object sender, RoutedEventArgs e)
        {
            mainGrid.Visibility = Visibility.Hidden;
            editWarehouseGrid.Visibility = Visibility.Visible;
            confirmEditingButton.IsEnabled = editWarehouseButton.IsEnabled = true;
            editMode = true;
        }

        private void DeleteWarehouse(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Jesteś pewny, że chcesz usunąć ten magazyn?", "Usuwanie magazynu", MessageBoxButton.YesNo, MessageBoxImage.Question) 
                == MessageBoxResult.Yes)
            {
                var rowData = (DataRowView)dataGrid.SelectedItems[0];
                sqlHandler.ExecuteCommand(SqlDeleteCommands.DeleteFromWarehouses(sqlHandler.Database, (String)rowData[0], (String)rowData[1]));
                confirmChoiceButton.IsEnabled = editWarehouseButton.IsEnabled = deleteWarehouseButton.IsEnabled = false;
                SetDataGrid();
            }
        }

        private void ConfirmChoice(object sender, RoutedEventArgs e)
        {
            if (editWarehouseGrid.Visibility == Visibility.Visible)
            {
                if (editMode)
                {
                    sqlHandler.ExecuteCommand(SqlUpdateCommands.UpdateWarehouse(sqlHandler.Database,
                        Shortcut, WarehouseName, centerTextBox.Text, upperTextBox.Text));
                }
                else
                {
                    sqlHandler.ExecuteCommand(SqlInsertCommands.InsertNewWarehouse(sqlHandler.Database, centerTextBox.Text, upperTextBox.Text));
                }
                ClickExit(new object(), new RoutedEventArgs());
            }
            else
            {
                Accepted = true;
                Close();
            }
        }

        private void ClickExit(object sender, RoutedEventArgs e)
        {
            if (editWarehouseGrid.Visibility == Visibility.Visible)
            {
                editWarehouseGrid.Visibility = Visibility.Hidden;
                mainGrid.Visibility = Visibility.Visible;
                confirmChoiceButton.IsEnabled = editWarehouseButton.IsEnabled = deleteWarehouseButton.IsEnabled = false;
                SetDataGrid();
            }
            else
            {
                Accepted = false;
                Shortcut = "";
                Name = "";
                Close();
            }
        }

        private void OnTextBoxChange(object sender, TextChangedEventArgs e)
        {
            if (upperTextBox.Text != "" && centerTextBox.Text != "")
                confirmEditingButton.IsEnabled = true;
            else
                confirmEditingButton.IsEnabled = false;
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectedRow = dataGrid.SelectedIndex;
            if (selectedRow != -1)
            {
                editWarehouseButton.IsEnabled = confirmChoiceButton.IsEnabled = deleteWarehouseButton.IsEnabled = true;
                var rowData = (DataRowView)dataGrid.SelectedItems[0];
                centerTextBox.Text = Shortcut = (String)rowData[0];
                upperTextBox.Text = WarehouseName = (String)rowData[1];
            }
        }
    }

}
