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
    /// Interaction logic for StockTaking.xaml
    /// </summary>
    public partial class StockTaking : Window
    {
        SqlHandler sqlHandler;
        String warehouseId;
        bool editMode = false;

        public StockTaking(SqlHandler _sqlHandler, String _warehouseId)
        {
            InitializeComponent();
            sqlHandler = _sqlHandler;
            warehouseId = _warehouseId;
            SetDataGrid();
        }

        public void SetDataGrid()
        {
            var sqlResult = sqlHandler.ExecuteCommand(SqlSelectCommands.ShowStockTakings(warehouseId)).Tables[0];
            dataGrid.ItemsSource = sqlResult.DefaultView;
            dataGrid.Visibility = Visibility.Visible;
        }

        private void OnDataGridColumnsGenerated(object sender, EventArgs e)
        {
            dataGrid.Columns[0].Visibility = Visibility.Hidden;
            foreach (var column in dataGrid.Columns)
            {
                if (SqlConstants.translations.TryGetValue(column.Header.ToString(), out String result))
                {
                    column.Header = result;
                    column.IsReadOnly = true;
                }
            }
        }

        private void OnAddNewStockTaking(object sender, RoutedEventArgs e)
        {
            editMode = false;
            bool knownProducts = sqlHandler.ExecuteCommand(SqlSelectCommands.ShowProducts(sqlHandler.Database)).Tables[0].DefaultView.Count > 0;
            if (knownProducts == true)
            {
                newStockTakingDatePicker.IsEnabled = true;
                newStockTakingDatePicker.SelectedDate = null;
                manageStockTakingsGrid.Visibility = Visibility.Hidden;
                newStockTakingGrid.Visibility = Visibility.Visible;
            }
            else
            {
                MessageBox.Show("Błąd! Brak znanych towarów");
            }
        }

        private void OnBackButtonClick(object sender, RoutedEventArgs e)
        {
            manageStockTakingsGrid.Visibility = Visibility.Visible;
            newStockTakingGrid.Visibility = Visibility.Hidden;
        }

        private void OnNewStackTakingSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void OnNewStockTakingSelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            stockTakingDataGrid.Visibility = Visibility.Hidden;
            if (newStockTakingDatePicker.SelectedDate != null)
            {
                if (editMode == false)
                {
                    bool stockTakingExists = Convert.ToInt32(sqlHandler.ExecuteCommand(SqlSelectCommands.CheckIfStockTakingWasMade(warehouseId, 
                        newStockTakingDatePicker.Text)).Tables[0].Rows[0][0].ToString()) > 0;

                    if (stockTakingExists == false)
                    {
                        SetNewStockTakingDataGrid();
                    }
                    else
                    {
                        MessageBox.Show("Błąd! Już stworzono remanent dla tego magazynu");
                    }
                }
                else
                {
                    SetNewStockTakingDataGrid();
                }
            }
        }

        private void SetNewStockTakingDataGrid()
        {
            String datePickedText = newStockTakingDatePicker.Text;

            var result = sqlHandler.ExecuteCommand(SqlSelectCommands.PrepareStockTaking(datePickedText, warehouseId)).Tables[0];
            var extraResult = sqlHandler.ExecuteCommand(SqlSelectCommands.GetProductsNotUsedInWarehouse(warehouseId)).Tables[0];
            var productsPrices = sqlHandler.ExecuteCommand(SqlSelectCommands.GetLastProductsBuyPrice(datePickedText, warehouseId)).Tables[0];

            foreach (DataRow price in productsPrices.Rows)
            {
                foreach (DataRow product in result.Rows)
                {
                    if (price[0].ToString() == product[0].ToString())
                    {
                        product.BeginEdit();
                        product[5] = price[1];
                        product.EndEdit();
                        break;
                    }
                }
            }

            foreach (DataRow extra in extraResult.Rows)
            {
                result.ImportRow(extra);
            }

            stockTakingDataGrid.ItemsSource = result.DefaultView;
            stockTakingDataGrid.Visibility = Visibility.Visible;
            OnNewStackTakingDataGridColumnsGenerated(new object(), new EventArgs());
        }

        private void OnNewStackTakingDataGridColumnsGenerated(object sender, EventArgs e)
        {
            foreach (var column in stockTakingDataGrid.Columns)
            {
                if (SqlConstants.translations.TryGetValue(column.Header.ToString(), out String result))
                {
                    column.Header = result;
                    column.IsReadOnly = true;
                }
            }

            stockTakingDataGrid.Columns[0].Visibility = Visibility.Hidden;

            if (editMode == false)
            {
                if (stockTakingDataGrid.Columns.Count > 0)
                {
                    for (int i = 2; i > 0; i--)
                    {
                        var column = stockTakingDataGrid.Columns[stockTakingDataGrid.Columns.Count - i];
                        column.CellStyle = new Style(typeof(DataGridCell));
                        column.CellStyle.Setters.Add(new Setter(BackgroundProperty, new SolidColorBrush(Colors.LightSkyBlue)));
                        column.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
                        column.IsReadOnly = false;
                    }
                }
            }
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShowStockTakingDetailsButton.IsEnabled = dataGrid.SelectedIndex > -1;
        }

        private void OnExitButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OnSaveStockTakingClick(object sender, RoutedEventArgs e)
        {
            var objectsToChange = new List<Tuple<String, int, int, String>>();
            bool canStockTakingBeMade = true;
            String selectedDate = newStockTakingDatePicker.Text;
            if (stockTakingDataGrid.Items.Count > 0)
            {
                foreach (DataRowView item in stockTakingDataGrid.Items)
                {
                    if (Convert.ToInt32(item[3].ToString()) - Convert.ToInt32(item[4].ToString()) != 0)
                    {
                        objectsToChange.Add(new Tuple<String, int, int, String> (item[0].ToString(), Convert.ToInt32(item[3].ToString()), 
                            Convert.ToInt32(item[4]) - Convert.ToInt32(item[3]), item[5].ToString()));
                    }
                }

                foreach (var tuple in objectsToChange)
                {
                    if (tuple.Item3 < 0)
                    {
                        var sqlResult = sqlHandler.ExecuteCommand(SqlSelectCommands.ShowProductFuture(selectedDate, warehouseId, tuple.Item1)).Tables[0];
                        int startingAmount = tuple.Item2;
                        
                        foreach (DataRow row in sqlResult.Rows)
                        {
                            if (row[1].ToString() == "0")
                            {
                                startingAmount += Convert.ToInt32(row[0].ToString());
                            }
                            else
                            {
                                startingAmount -= Convert.ToInt32(row[0].ToString());

                                if (startingAmount < 0)
                                {
                                    canStockTakingBeMade = false;
                                    MessageBox.Show("Błąd! Wprowadzenie tych zmian doprowadziłoby do ujemnych stanów magazynu");
                                    break;
                                }
                            }
                        }

                        if (canStockTakingBeMade == false)
                        {
                            break;
                        }
                    }
                }
            }
            else
            {
                OnBackButtonClick(new object(), new RoutedEventArgs());
                return;
            }

            if (canStockTakingBeMade == true)
            {
                bool decreaseProducts = false, increaseProducts = false;
                
                foreach (var tuple in objectsToChange)
                {
                    if (tuple.Item3 > 0)
                    {
                        increaseProducts = true;
                    }
                    else if (tuple.Item3 < 0)
                    {
                        decreaseProducts = true;
                    }
                    if (increaseProducts && decreaseProducts)
                    {
                        break;
                    }
                }

                String stocktakingCode = GetNextCode("INW");
                sqlHandler.ExecuteNonQuery(SqlInsertCommands.InsertOrder(sqlHandler.Database, stocktakingCode, "-1",
                    warehouseId, "INW", "NULL", selectedDate));
                String insertedStockTakingId = sqlHandler.DataSetToList(sqlHandler.ExecuteCommand(SqlSelectCommands.ShowLastInsertedID(sqlHandler.Database, "orders")))[0][0];

                if (increaseProducts == true)
                {
                    String nextCode = GetNextCode("PW");

                    sqlHandler.ExecuteNonQuery(SqlInsertCommands.InsertOrder(sqlHandler.Database, nextCode, "-1",
                        warehouseId, "PW", "0", selectedDate));
                    String insertedOrderId = sqlHandler.DataSetToList(sqlHandler.ExecuteCommand(SqlSelectCommands.ShowLastInsertedID(sqlHandler.Database, "orders")))[0][0];
                    foreach (var tuple in objectsToChange)
                    {
                        sqlHandler.ExecuteNonQuery(SqlInsertCommands.InsertOrderDetails(sqlHandler.Database, insertedOrderId.ToString(),
                            tuple.Item1, tuple.Item3.ToString(), tuple.Item4));
                    }
                    sqlHandler.ExecuteCommand(SqlInsertCommands.InsertAttachement(insertedStockTakingId, insertedOrderId));
                }

                if (decreaseProducts == true)
                {
                    String nextCode = GetNextCode("RW");

                    sqlHandler.ExecuteNonQuery(SqlInsertCommands.InsertOrder(sqlHandler.Database, nextCode, "-1",
                        warehouseId, "RW", "1", selectedDate));
                    String insertedOrderId = sqlHandler.DataSetToList(sqlHandler.ExecuteCommand(SqlSelectCommands.ShowLastInsertedID(sqlHandler.Database, "orders")))[0][0];
                    foreach (var tuple in objectsToChange)
                    {
                        sqlHandler.ExecuteNonQuery(SqlInsertCommands.InsertOrderDetails(sqlHandler.Database, insertedOrderId.ToString(),
                            tuple.Item1, (-1 * tuple.Item3).ToString(), tuple.Item4));
                    }
                    sqlHandler.ExecuteCommand(SqlInsertCommands.InsertAttachement(insertedStockTakingId, insertedOrderId));
                }
            }
            SetDataGrid();
            OnBackButtonClick(new object(), new RoutedEventArgs());
        }

        private String GetNextCode(String documentType)
        {
            var warehouseData = sqlHandler.DataSetToList(sqlHandler.ExecuteCommand(SqlSelectCommands.ShowWarehousesForGivenField("id", warehouseId)))[0];
            String warehouseShort = warehouseData[1];
            String nextCode = "";
            var dateFormatSetting = sqlHandler.DataSetToList(sqlHandler.ExecuteCommand(SqlSelectCommands.GetSetting("2")))[0][0];
            String date = $@"{Convert.ToDateTime(newStockTakingDatePicker.Text).Year}.{Convert.ToDateTime(newStockTakingDatePicker.Text).Month}.{Convert.ToDateTime(newStockTakingDatePicker.Text).Day}";
            var list = sqlHandler.DataSetToList(sqlHandler.ExecuteCommand(SqlSelectCommands.ShowLastDocumentNumber(date, warehouseId, documentType, dateFormatSetting)));
            int number = 0;
            if (list[0][0] != null && list[0][0] != "")
                number = Convert.ToInt32(list[0][0].ToString());

            String endingString = $@"/{Convert.ToDateTime(newStockTakingDatePicker.Text).Year}/{documentType}/{warehouseShort}";

            if (dateFormatSetting == "0")
            {
                nextCode = (++number).ToString() + @"/" + Convert.ToDateTime(newStockTakingDatePicker.Text).Month;
            }
            else
            {
                nextCode = (++number).ToString();
            }
            nextCode += endingString;

            return nextCode;
        }

        private void OnShowStockTakingDetailsButtonClick(object sender, RoutedEventArgs e)
        {
            editMode = true;
            newStockTakingDatePicker.IsEnabled = false;
            manageStockTakingsGrid.Visibility = Visibility.Hidden;
            newStockTakingGrid.Visibility = Visibility.Visible;

            DataRowView stockTakingRow = (DataRowView)dataGrid.SelectedItem;
            var stockTakingDocumentInfo = sqlHandler.ExecuteCommand(SqlSelectCommands.ShowStockTakingMainDocumentDetails(stockTakingRow[1].ToString())).Tables[0].Rows[0];

            newStockTakingDatePicker.SelectedDate = Convert.ToDateTime(stockTakingDocumentInfo[1].ToString());
            String datePickedText = newStockTakingDatePicker.Text;

            var result = sqlHandler.ExecuteCommand(SqlSelectCommands.PrepareStockTaking(datePickedText, warehouseId)).Tables[0];
            var productsPricesAndShouldBe = sqlHandler.ExecuteCommand(SqlSelectCommands.GetProductPriceAndShouldBeFromStockTaking(stockTakingDocumentInfo[0].ToString())).Tables[0];

            foreach (DataRow priceAndShould in productsPricesAndShouldBe.Rows)
            {
                foreach (DataRow product in result.Rows)
                {
                    if (priceAndShould[0].ToString() == product[0].ToString())
                    {
                        product.BeginEdit();
                        int beforeChange = Convert.ToInt32(product[3].ToString());
                        if (priceAndShould[1].ToString() == "0")
                        {
                            beforeChange -= Convert.ToInt32(priceAndShould[2].ToString());
                        }
                        else
                        {
                            beforeChange += Convert.ToInt32(priceAndShould[2].ToString());
                        }

                        product[4] = product[3].ToString();
                        product[3] = beforeChange.ToString();
                        product[5] = priceAndShould[3];

                        product.EndEdit();
                        break;
                    }
                }
            }

            stockTakingDataGrid.ItemsSource = result.DefaultView;
            stockTakingDataGrid.Visibility = Visibility.Visible;
            OnNewStackTakingDataGridColumnsGenerated(new object(), new EventArgs());
        }
    }
}
