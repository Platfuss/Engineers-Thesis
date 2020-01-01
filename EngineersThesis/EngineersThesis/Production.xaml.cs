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

namespace EngineersThesis.General
{
    /// <summary>
    /// Interaction logic for Production.xaml
    /// </summary>
    public partial class Production : Window
    {
        String warehouseId;
        SqlHandler sqlHandler;
        Dictionary<int, String> productIndexToID = new Dictionary<int, String>();

        public Production(SqlHandler _sqlHandler, String _warehouseId)
        {
            InitializeComponent();
            sqlHandler = _sqlHandler;
            warehouseId = _warehouseId;
        }

        private void OnExitButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SetComboBox()
        {
            comboBox.Items.Clear();
            productIndexToID.Clear();
            List<List<String>> sqlExecutionResult = new List<List<string>>();
            sqlExecutionResult = sqlHandler.DataSetToList(sqlHandler.ExecuteCommand(SqlSelectCommands.ShowComplexProductsName()));
            for (int i = 0; i < sqlExecutionResult.Count; i++)
            {
                productIndexToID.Add(i, sqlExecutionResult[i][0]);
                sqlExecutionResult[i].RemoveAt(0);
                String product = "";
                foreach (var field in sqlExecutionResult[i])
                {
                    product += field + ", ";
                }
                product = product.Remove(product.Length - 2);
                comboBox.Items.Add(product);
                comboBox.IsEnabled = true;
                comboBox.SelectedIndex = 0;
            }
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            SetComboBox();
            SetHistoryDataGrid();
        }

        private void SetRecipeDataGrid()
        {
            recipeDataGrid.Visibility = Visibility.Visible;
            String productId = productIndexToID[comboBox.SelectedIndex];
            var sqlResult = sqlHandler.ExecuteCommand(SqlSelectCommands.ShowComplexProductRecipeWithName(productId)).Tables[0];
            recipeDataGrid.ItemsSource = sqlResult.DefaultView;
        }

        private void OnRecipeDataGridColumnsGenerated(object sender, EventArgs e)
        {
            recipeDataGrid.Columns[0].Visibility = Visibility.Hidden;
            foreach (var column in recipeDataGrid.Columns)
            {
                column.IsReadOnly = true;
                if (SqlConstants.translations.TryGetValue(column.Header.ToString(), out String result))
                {
                    column.Header = result;
                }
            }
        }

        private void SetHistoryDataGrid()
        {
            historyDataGrid.Visibility = Visibility.Visible;
            var sqlResult = sqlHandler.ExecuteCommand(SqlSelectCommands.ShowComplexDocuments(warehouseId)).Tables[0];
            historyDataGrid.ItemsSource = sqlResult.DefaultView;
        }

        private void OnHistoryDataGridColumnsGenerated(object sender, EventArgs e)
        {
            foreach (var column in historyDataGrid.Columns)
            {
                column.IsReadOnly = true;
                if (SqlConstants.translations.TryGetValue(column.Header.ToString(), out String result))
                {
                    column.Header = result;
                }
            }
        }

        private void OnNewRecipeButtonClick(object sender, RoutedEventArgs e)
        {
            Action action = () => { };
            var productEditor = new ProductEditorHost(sqlHandler, action, true, true)
            {
                Owner = this
            };
            productEditor.ShowDialog();
            SetHistoryDataGrid();
            SetComboBox();
        }

        private void OnComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBox.SelectedIndex > -1)
            {
                SetRecipeDataGrid();
                String productId = productIndexToID[comboBox.SelectedIndex];
                AmountTextBox.Text = sqlHandler.ExecuteCommand(SqlSelectCommands.ShowProductAmountInWarehouse(warehouseId, productId)).Tables[0].Rows[0][0].ToString();
                createButton.IsEnabled = true;
            }
            else
            {
                createButton.IsEnabled = false;
            }
        }

        private void OnProductionButtonClick(object sender, RoutedEventArgs e)
        {
            String productId = productIndexToID[comboBox.SelectedIndex];
            var recipeManager = new RecipeManager(sqlHandler, productId, warehouseId)
            {
                Owner = this
            };
            recipeManager.ShowDialog();
            SetHistoryDataGrid();
            SetComboBox();
        }
    }
}
