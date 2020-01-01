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
using System.Text.RegularExpressions;

namespace EngineersThesis
{
    /// <summary>
    /// Interaction logic for RecipeManager.xaml
    /// </summary>
    public partial class RecipeManager : Window
    {
        SqlHandler sqlHandler;
        String productId;
        String warehouseId;

        public RecipeManager(SqlHandler _sqlHandler, String _productId, String _warehouseId)
        {
            InitializeComponent();
            sqlHandler = _sqlHandler;
            productId = _productId;
            warehouseId = _warehouseId;
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (checkBox.IsChecked == true)
            {
                if (cycleTextBox.Text != null && cycleTextBox.Text != "")
                {
                    createButton.IsEnabled = true;
                }
                else
                {
                    createButton.IsEnabled = false;
                }
            }
            else
            {
                if (cycleTextBox.Text != null && cycleTextBox.Text != "" && priceTextBox.Text != null && priceTextBox.Text != "")
                {
                    createButton.IsEnabled = true;
                }
                else
                {
                    createButton.IsEnabled = false;
                }
            }
        }

        private void OnTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            e.Handled = !Regex.IsMatch(textBox.Text + e.Text, @"^\d*$");
        }


        private void OnProductionButtonClick(object sender, RoutedEventArgs e)
        {
            int cycleCounter = Convert.ToInt32(cycleTextBox.Text);
            String date = DateTime.Now.Date.ToString("yyyy/MM/dd");

            if (checkBox.IsChecked == false)
            {
                var productsToMove = new Dictionary<String, double> ();
                bool canProductsBeMoved = CanProductsBeMovedOut(ref productsToMove);

                if (canProductsBeMoved == true)
                {
                    String productionCode = GetNextCode("KOM");
                    sqlHandler.ExecuteNonQuery(SqlInsertCommands.InsertOrder(sqlHandler.Database, productionCode, "-1",
                        warehouseId, "KOM", "NULL", date));
                    String insertedProductionId = sqlHandler.DataSetToList(sqlHandler.ExecuteCommand(SqlSelectCommands.ShowLastInsertedID(sqlHandler.Database, "orders")))[0][0];

                    String nextCode = GetNextCode("PW");

                    sqlHandler.ExecuteNonQuery(SqlInsertCommands.InsertOrder(sqlHandler.Database, nextCode, "-1",
                        warehouseId, "PW", "0", date));
                    String insertedOrderId = sqlHandler.DataSetToList(sqlHandler.ExecuteCommand(SqlSelectCommands.ShowLastInsertedID(sqlHandler.Database, "orders")))[0][0];
                    sqlHandler.ExecuteNonQuery(SqlInsertCommands.InsertOrderDetails(sqlHandler.Database, insertedOrderId.ToString(),
                            productId, cycleCounter.ToString(), priceTextBox.Text));
                    
                    sqlHandler.ExecuteCommand(SqlInsertCommands.InsertAttachement(insertedProductionId, insertedOrderId));

                    nextCode = GetNextCode("RW");

                    String strategy = sqlHandler.DataSetToList(sqlHandler.ExecuteCommand(SqlSelectCommands.GetSetting("1")))[0][0];

                    sqlHandler.ExecuteNonQuery(SqlInsertCommands.InsertOrder(sqlHandler.Database, nextCode, "-1",
                        warehouseId, "RW", "1", date));
                    insertedOrderId = sqlHandler.DataSetToList(sqlHandler.ExecuteCommand(SqlSelectCommands.ShowLastInsertedID(sqlHandler.Database, "orders")))[0][0];

                    foreach (var toMove in productsToMove)
                    {
                        var result = sqlHandler.ExecuteCommand(SqlSelectCommands.GetProductFromOrders(toMove.Key, warehouseId, strategy)).Tables[0];

                        double toDelete = toMove.Value;
                        for (int i = 0; toDelete > 0; i++)
                        {
                            double availableWare = Convert.ToDouble(result.Rows[i].ItemArray[1].ToString());
                            double difference = availableWare - toDelete;

                            var columns = result.Rows[i].ItemArray;
                            String buyOrderId = columns[0].ToString();
                            String price = columns[2].ToString();

                            if (difference >= 0)
                            {
                                sqlHandler.ExecuteNonQuery(SqlInsertCommands.InsertDetailsForOutOrders(insertedOrderId, buyOrderId, toMove.Key,
                                    toDelete.ToString(), price));

                                sqlHandler.ExecuteNonQuery(SqlUpdateCommands.UpdateLeftovers(buyOrderId, toMove.Key, difference.ToString()));
                                break;
                            }
                            else
                            {
                                sqlHandler.ExecuteNonQuery(SqlInsertCommands.InsertDetailsForOutOrders(insertedOrderId, buyOrderId, toMove.Key,
                                    availableWare.ToString(), price));

                                sqlHandler.ExecuteNonQuery(SqlUpdateCommands.UpdateLeftovers(buyOrderId, toMove.Key, "0"));

                                toDelete = -difference;
                                continue;
                            }
                        }
                    }
                    sqlHandler.ExecuteCommand(SqlInsertCommands.InsertAttachement(insertedProductionId, insertedOrderId));
                }
            }
            else
            {
                var productsToAdd = new Dictionary<String, double>();
                bool canComplexProductBeDecompleted = CanComplexProductBeDecompleted(ref productsToAdd);

                if (canComplexProductBeDecompleted)
                {
                    String productionCode = GetNextCode("DEK");
                    sqlHandler.ExecuteNonQuery(SqlInsertCommands.InsertOrder(sqlHandler.Database, productionCode, "-1",
                        warehouseId, "DEK", "NULL", date));
                    String insertedProductionId = sqlHandler.DataSetToList(sqlHandler.ExecuteCommand(SqlSelectCommands.ShowLastInsertedID(sqlHandler.Database, "orders")))[0][0];

                    String nextCode = GetNextCode("RW");

                    String strategy = sqlHandler.DataSetToList(sqlHandler.ExecuteCommand(SqlSelectCommands.GetSetting("1")))[0][0];

                    sqlHandler.ExecuteNonQuery(SqlInsertCommands.InsertOrder(sqlHandler.Database, nextCode, "-1",
                        warehouseId, "RW", "1", date));
                    String insertedOrderId = sqlHandler.DataSetToList(sqlHandler.ExecuteCommand(SqlSelectCommands.ShowLastInsertedID(sqlHandler.Database, "orders")))[0][0];

                    var result = sqlHandler.ExecuteCommand(SqlSelectCommands.GetProductFromOrders(productId, warehouseId, strategy)).Tables[0];

                    double toDelete = cycleCounter;
                    for (int i = 0; toDelete > 0; i++)
                    {
                        double availableWare = Convert.ToDouble(result.Rows[i].ItemArray[1].ToString());
                        double difference = availableWare - toDelete;

                        var columns = result.Rows[i].ItemArray;
                        String buyOrderId = columns[0].ToString();
                        String price = columns[2].ToString();

                        if (difference >= 0)
                        {
                            sqlHandler.ExecuteNonQuery(SqlInsertCommands.InsertDetailsForOutOrders(insertedOrderId, buyOrderId, productId,
                                toDelete.ToString(), price));

                            sqlHandler.ExecuteNonQuery(SqlUpdateCommands.UpdateLeftovers(buyOrderId, productId, difference.ToString()));
                            break;
                        }
                        else
                        {
                            sqlHandler.ExecuteNonQuery(SqlInsertCommands.InsertDetailsForOutOrders(insertedOrderId, buyOrderId, productId,
                                availableWare.ToString(), price));

                            sqlHandler.ExecuteNonQuery(SqlUpdateCommands.UpdateLeftovers(buyOrderId, productId, "0"));

                            toDelete = -difference;
                            continue;
                        }
                    }
                    sqlHandler.ExecuteCommand(SqlInsertCommands.InsertAttachement(insertedProductionId, insertedOrderId));


                    nextCode = GetNextCode("PW");

                    sqlHandler.ExecuteNonQuery(SqlInsertCommands.InsertOrder(sqlHandler.Database, nextCode, "-1",
                        warehouseId, "PW", "0", date));
                    insertedOrderId = sqlHandler.DataSetToList(sqlHandler.ExecuteCommand(SqlSelectCommands.ShowLastInsertedID(sqlHandler.Database, "orders")))[0][0];

                    const String maxDate = "9999-12-31";
                    var sqlResult = sqlHandler.ExecuteCommand(SqlSelectCommands.GetLastProductsBuyPrice(maxDate, warehouseId)).Tables[0];
                    foreach (var toAdd in productsToAdd)
                    {
                        int price = 0;
                        foreach (DataRow knownPrices in sqlResult.Rows)
                        {
                            if (knownPrices[0].ToString() == toAdd.Key)
                            {
                                price = Convert.ToInt32(knownPrices[1].ToString());
                                break;
                            }
                        }

                        if (price <= 0)
                        {
                            var proposedPrices = sqlHandler.ExecuteCommand(SqlSelectCommands.ShowProducts(sqlHandler.Database)).Tables[0];
                            foreach (DataRow row in proposedPrices.Rows)
                            {
                                if (row[0].ToString() == toAdd.Key)
                                {
                                    price = Convert.ToInt32(row[4].ToString());
                                    break;
                                }
                            }
                        }

                        sqlHandler.ExecuteNonQuery(SqlInsertCommands.InsertOrderDetails(sqlHandler.Database, insertedOrderId.ToString(),
                            toAdd.Key, toAdd.Value.ToString(), price.ToString()));
                    }
                    sqlHandler.ExecuteCommand(SqlInsertCommands.InsertAttachement(insertedProductionId, insertedOrderId));
                }
            }
            Close();
        }

        private bool CanProductsBeMovedOut(ref Dictionary<String, double> productsToMove)
        {
            int cycleCounter = Convert.ToInt32(cycleTextBox.Text);
            String warehouseName = sqlHandler.ExecuteCommand(SqlSelectCommands.ShowWarehousesForGivenField("id", warehouseId)).Tables[0].Rows[0][2].ToString();
            DataTable productsInWarehouse = sqlHandler.ExecuteCommand(SqlSelectCommands.ShowProductsInWarehouse(sqlHandler.Database, warehouseName)).Tables[0];
            DataTable fullRecipe = sqlHandler.ExecuteCommand(SqlSelectCommands.ShowComplexProductRecipe(sqlHandler.Database, productId)).Tables[0];

            foreach (DataRow recipeRow in fullRecipe.Rows)
            {
                bool productWasInWarehouse = false;
                foreach (DataRow product in productsInWarehouse.Rows)
                {
                    if (recipeRow[0].ToString() == product[0].ToString())
                    {
                        productWasInWarehouse = true;
                        int toDelete = Convert.ToInt32(recipeRow[1].ToString()) * cycleCounter;
                        if (Convert.ToInt32(product[3].ToString()) - toDelete >= 0)
                        {
                            productsToMove.Add(recipeRow[0].ToString(), toDelete);
                        }
                        else
                        {
                            MessageBox.Show("Błąd! Niewystarczająca ilość produktów składowych w magazynie");
                            return false;
                        }
                    }
                }
                
                if (productWasInWarehouse == false)
                {
                    MessageBox.Show("Błąd! Niewystarczająca ilość produktów składowych w magazynie");
                    return false;
                }
            }

            return true;
        }

        private bool CanComplexProductBeDecompleted(ref Dictionary <String, double> productsToAdd)
        {
            int cycleCounter = Convert.ToInt32(cycleTextBox.Text);
            String warehouseName = sqlHandler.ExecuteCommand(SqlSelectCommands.ShowWarehousesForGivenField("id", warehouseId)).Tables[0].Rows[0][2].ToString();
            DataTable productsInWarehouse = sqlHandler.ExecuteCommand(SqlSelectCommands.ShowProductsInWarehouse(sqlHandler.Database, warehouseName)).Tables[0];
        
            bool productWasInWarehouse = false;
            foreach (DataRow product in productsInWarehouse.Rows)
            {
                if (product[0].ToString() == productId)
                {
                    productWasInWarehouse = true;
                    int toDelete =  cycleCounter;
                    if (Convert.ToInt32(product[3].ToString()) - toDelete >= 0)
                    {
                        break;
                    }
                    else
                    {
                        MessageBox.Show("Błąd! Niewystarczająca ilość produktów składowych w magazynie");
                        return false;
                    }
                }
            }

            if (productWasInWarehouse == false)
            {
                MessageBox.Show("Błąd! Niewystarczająca ilość produktów składowych w magazynie");
                return false;
            }
            else
            {
                DataTable fullRecipe = sqlHandler.ExecuteCommand(SqlSelectCommands.ShowComplexProductRecipe(sqlHandler.Database, productId)).Tables[0];
                foreach (DataRow recipe in fullRecipe.Rows)
                {
                    productsToAdd.Add(recipe[0].ToString(), Convert.ToDouble(recipe[1]) * cycleCounter);
                }
            }
            
            return true;
        }

        private String GetNextCode(String documentType)
        {
            var warehouseData = sqlHandler.DataSetToList(sqlHandler.ExecuteCommand(SqlSelectCommands.ShowWarehousesForGivenField("id", warehouseId)))[0];
            String warehouseShort = warehouseData[1];
            String nextCode = "";
            var dateFormatSetting = sqlHandler.DataSetToList(sqlHandler.ExecuteCommand(SqlSelectCommands.GetSetting("2")))[0][0];
            String date = DateTime.Now.Date.ToString("yyyy/MM/dd");
            var list = sqlHandler.DataSetToList(sqlHandler.ExecuteCommand(SqlSelectCommands.ShowLastDocumentNumber(date, warehouseId, documentType, dateFormatSetting)));
            int number = 0;
            if (list[0][0] != null && list[0][0] != "")
                number = Convert.ToInt32(list[0][0].ToString());

            String endingString = $@"/{DateTime.Now.Date.Year}/{documentType}/{warehouseShort}";

            if (dateFormatSetting == "0")
            {
                nextCode = (++number).ToString() + @"/" + DateTime.Now.Date.Month;
            }
            else
            {
                nextCode = (++number).ToString();
            }
            nextCode += endingString;

            return nextCode;
        }

        private void OnTextBoxClick(object sender, RoutedEventArgs e)
        {
            if (checkBox.IsChecked == true)
            {
                priceTextBox.Text = "";
                priceTextBox.IsEnabled = false;
            }
            else
            {
                priceTextBox.IsEnabled = true;
            }

            if (checkBox.IsChecked == true)
            {
                if (cycleTextBox.Text != null && cycleTextBox.Text != "")
                {
                    createButton.IsEnabled = true;
                }
                else
                {
                    createButton.IsEnabled = false;
                }
            }
            else
            {
                if (cycleTextBox.Text != null && cycleTextBox.Text != "" && priceTextBox.Text != null && priceTextBox.Text != "")
                {
                    createButton.IsEnabled = true;
                }
                else
                {
                    createButton.IsEnabled = false;
                }
            }
        }
    }
}
