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
using System.Text.RegularExpressions;
using EngineersThesis.General;

namespace EngineersThesis
{
    /// <summary>
    /// Interaction logic for ProductEdition.xaml
    /// </summary>
    public partial class ProductEditor : Window
    {
        private SqlHandler sqlHandler;

        public ProductEditor(SqlHandler handler)
        {
            InitializeComponent();
            sqlHandler = handler;
            Action setDataGrid = SetDataGrid;
            editProductFrame.Content = new ProductEditorControl(sqlHandler, setDataGrid, false);
            newProductFrame.Content = new ProductEditorControl(sqlHandler, setDataGrid, true);
        }

        private void SetDataGrid()
        {
            var database = sqlHandler.Database;
            if (database != null)
            {
                var dataSet = sqlHandler.ExecuteCommand(SqlSelectCommands.ShowProductsWithFollowingEmpty(sqlHandler.Database, "complex"));
                dataGrid.ItemsSource = dataSet.Tables[0].DefaultView;
                foreach (var column in dataGrid.Columns)
                {
                    if (SqlConstants.translations.TryGetValue(column.Header.ToString(), out String result))
                        column.Header = result;
                }

                if (dataGrid.Columns.Count > 0)
                {
                    dataGrid.Columns[0].Visibility = Visibility.Hidden;
                    dataGrid.Columns[dataGrid.Columns.Count - 1].Width = new DataGridLength(1, DataGridLengthUnitType.Star);
                    var list = sqlHandler.DataSetToList(sqlHandler.ExecuteCommand(SqlSelectCommands.ShowComplexProductsID(sqlHandler.Database)));
                    for (int i = 0; i < list.Count; i++)
                    {
                        foreach (DataRowView row in dataGrid.Items)
                        {
                            var lastColumnIndex = row.Row.Table.Columns.Count - 1;
                            if (row[0].ToString() == list[i][0])
                            {
                                row[lastColumnIndex] = "Tak";
                            }
                        }
                    }

                    foreach (DataRowView row in dataGrid.Items)
                    {
                        var lastColumnIndex = row.Row.Table.Columns.Count - 1;
                        if (row[lastColumnIndex].ToString() != "Tak")
                        {
                            row[lastColumnIndex] = "Nie";
                        }
                    }
                }
            }
        }

        private void OnDataGridSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataGrid.SelectedIndex != -1)
            {
                deleteProductButton.IsEnabled = true;
                Action setDataGrid = SetDataGrid;
                editProductFrame.Content = new ProductEditorControl(sqlHandler, setDataGrid, (DataRowView)dataGrid.SelectedItem);
            }
            else
            {
                deleteProductButton.IsEnabled = false;
            }
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            SetDataGrid();
        }

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OnDeleteProductButton(object sender, RoutedEventArgs e)
        {
            sqlHandler.ExecuteNonQuery(SqlDeleteCommands.DeleteProduct(sqlHandler.Database, ((DataRowView)dataGrid.SelectedItem)[0].ToString()));
            SetDataGrid();
        }
    }
}
