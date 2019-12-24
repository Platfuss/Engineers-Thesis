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
    /// Interaction logic for ProductsOnDocuments.xaml
    /// </summary>
    public partial class ProductsOnDocuments : Window
    {
        private SqlHandler sqlHandler;
        private String warehouseId;
        private Dictionary<int, string> productIndexToId = new Dictionary<int, string>();

        public ProductsOnDocuments(SqlHandler _sqlHandler, String _warehouseId)
        {
            sqlHandler = _sqlHandler;
            warehouseId = _warehouseId;
            InitializeComponent();
            SetComboBox();
            SetDataGrid();
        }

        private void SetComboBox()
        {
            var sqlExecutionResult = sqlHandler.DataSetToList(sqlHandler.ExecuteCommand(SqlSelectCommands.ShowProductsName(sqlHandler.Database)));
            for (int i = 0; i < sqlExecutionResult.Count; i++)
            {
                productIndexToId.Add(i, sqlExecutionResult[i][0]);
                sqlExecutionResult[i].RemoveAt(0);
                String product = sqlExecutionResult[i][0];
                productComboBox.Items.Add(product);
            }

            if (sqlExecutionResult.Count > 0)
            {
                productComboBox.SelectedIndex = 0;
            }
        }

        private void OnCheckBoxClicked(object sender, RoutedEventArgs e)
        {
            SetDataGrid();
        }

        private void OnProductComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetDataGrid();
        }

        private void SetDataGrid()
        {
            String productId = productIndexToId[productComboBox.SelectedIndex];
            var documentTypes = new List<String>();

            if (pzCheckBox.IsChecked == true)
            {
                documentTypes.Add("PZ");
            }
            if (wzCheckBox.IsChecked == true)
            {
                documentTypes.Add("WZ");
            }
            if (pwCheckBox.IsChecked == true)
            {
                documentTypes.Add("PW");
            }
            if (rwCheckBox.IsChecked == true)
            {
                documentTypes.Add("RW");
            }
            if (mmCheckBox.IsChecked == true)
            {
                documentTypes.Add("MM");
            }

            DataTable sqlResult;
            if (allWarehousesRadioButton.IsChecked == true)
            {
                sqlResult = sqlHandler.ExecuteCommand(SqlSelectCommands.ShowOrdersContainingProduct(productId, documentTypes)).Tables[0];
            }
            else
            {
                sqlResult = sqlHandler.ExecuteCommand(SqlSelectCommands.ShowOrdersContainingProduct(productId, documentTypes, warehouseId)).Tables[0];
            }

            dataGrid.ItemsSource = sqlResult.DefaultView;
        }

        private void OnDataGridColumnsGenerated(object sender, EventArgs e)
        {
            dataGrid.Visibility = Visibility.Visible;
            foreach (var column in dataGrid.Columns)
            {
                if (SqlConstants.translations.TryGetValue(column.Header.ToString(), out String result))
                {
                    column.Header = result;
                }
            }
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            documentInfoButton.IsEnabled = dataGrid.SelectedIndex > -1;
        }

        private void OnShowDocumentInfoClick(object sender, RoutedEventArgs e)
        {
            var row = (DataRowView)dataGrid.Items[dataGrid.SelectedIndex];
            var documentEditor = new DocumentEditor(sqlHandler, row)
            {
                Owner = this
            };
            documentEditor.ShowDialog();
        }

        private void OnExitButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
