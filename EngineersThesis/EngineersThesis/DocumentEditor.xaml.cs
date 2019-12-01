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
    /// Interaction logic for DocumentEditor.xaml
    /// </summary>
    public partial class DocumentEditor : Window
    {
        private SqlHandler sqlHandler;
        private String warehouseName, documentType;
        private Dictionary<int, String> contractorIndexToID = new Dictionary<int, String>();
        private Dictionary<int, String> productIndexToID;
        private DataView source = new DataView();
        private DocumentNature documentNature;
        private enum DocumentNature : byte
        {
            Buy,
            Move,
            Sell,
        }
        bool readOnly = false;

        public DocumentEditor(SqlHandler handler, DataRowView row)
        {
            InitializeComponent();
            readOnly = true;
            sqlHandler = handler;
            newEntryButton.Visibility = productComboBox.Visibility = acceptButton.Visibility = deleteButton.Visibility = Visibility.Hidden;
            contractorComboBox.IsEnabled = datePicker.IsEnabled = false;
            datePicker.Text = row[0].ToString();
            String orderNumber = row[1].ToString();
            var result = sqlHandler.ExecuteCommand(SqlSelectCommands.ShowProductsInDocument(orderNumber));
            dataGrid.ItemsSource = result.Tables[0].DefaultView;
            if (dataGrid.Items.Count > 0)
            {
                dataGrid.Visibility = Visibility.Visible;
            }
            List<List<String>> sqlExecutionResult = new List<List<string>>();
            var list = sqlHandler.DataSetToList(sqlHandler.ExecuteCommand(SqlSelectCommands.ShowDocumentHasContractor(row[1].ToString())));
            if (list[0][0] == "yes")
            {
                sqlExecutionResult = sqlHandler.DataSetToList(sqlHandler.ExecuteCommand(SqlSelectCommands.ShowContractorDataForOrderNumber(row[1].ToString())));
            }
            else
            {
                sqlExecutionResult = sqlHandler.DataSetToList(sqlHandler.ExecuteCommand(SqlSelectCommands.ShowWarehouseDataForOrderNumber(row[1].ToString())));
            }

            for (int i = 0; i < sqlExecutionResult.Count; i++)
            {
                String contractor = "";
                foreach (var field in sqlExecutionResult[i])
                {
                    contractor += field + ", ";
                }
                contractor = contractor.Remove(contractor.Length - 2);
                contractorComboBox.Items.Add(contractor);
            }
            contractorComboBox.SelectedIndex = 0;
        }

        public DocumentEditor(SqlHandler handler, String _warehouseName, String _documentType)
        {
            InitializeComponent();
            sqlHandler = handler;
            warehouseName = _warehouseName;
            documentType = _documentType;
            switch (_documentType)
            {
                case "PZ":
                    documentNature = DocumentNature.Buy;
                    break;
                case "PW":
                    documentNature = DocumentNature.Buy;
                    break;
                case "WZ":
                    documentNature = DocumentNature.Sell;
                    break;
                case "RW":
                    documentNature = DocumentNature.Sell;
                    break;
                case "MM":
                    documentNature = DocumentNature.Move;
                    break;
            }
            PrepareControls();
        }

        private void PrepareControls()
        {
            List<List<String>> sqlExecutionResult = new List<List<String>>();
            if (documentNature != DocumentNature.Move)
            {
                sqlExecutionResult = sqlHandler.DataSetToList(sqlHandler.ExecuteCommand(SqlSelectCommands.ShowContractors(sqlHandler.Database)));
            }
            else
            {
                sqlExecutionResult = sqlHandler.DataSetToList(sqlHandler.ExecuteCommand(SqlSelectCommands.ShowOtherWarehouses(sqlHandler.Database, warehouseName)));
            }

            for (int i = 0; i < sqlExecutionResult.Count; i++)
            {
                contractorIndexToID.Add(i, sqlExecutionResult[i][0]);
                sqlExecutionResult[i].RemoveAt(0);
                String contractor = "";
                foreach (var field in sqlExecutionResult[i])
                {
                    contractor += field + ", ";
                }
                contractor = contractor.Remove(contractor.Length - 2);
                contractorComboBox.Items.Add(contractor);
            }

            if (sqlExecutionResult.Count > 0)
            {
                contractorComboBox.SelectedIndex = 0;
            }

            datePicker.Text = DateTime.Now.ToString();

            SetProductComboBox();
        }

        private void SetProductComboBox()
        {
            productComboBox.Items.Clear();
            var sqlExecutionResult = sqlHandler.DataSetToList(sqlHandler.ExecuteCommand(SqlSelectCommands.ShowProductsName(sqlHandler.Database)));
            productIndexToID = new Dictionary<int, String>();
            for (int i = 0; i < sqlExecutionResult.Count; i++)
            {
                bool deleted = false;
                foreach(DataRowView row in dataGrid.Items)
                {
                    if (row[0].ToString().Equals(sqlExecutionResult[i][0]))
                    {
                        sqlExecutionResult.RemoveAt(i);
                        i--;
                        deleted = true;
                        break;
                    }
                }

                if (deleted)
                    continue;

                productIndexToID.Add(i, sqlExecutionResult[i][0]);
                sqlExecutionResult[i].RemoveAt(0);
                String product = "";
                foreach (var field in sqlExecutionResult[i])
                {
                    product += field + ", ";
                }
                product = product.Remove(product.Length - 2);
                productComboBox.Items.Add(product);
            }

            if (productComboBox.Items.Count > 0)
            {
                productComboBox.SelectedIndex = 0;
                productComboBox.IsEnabled = newEntryButton.IsEnabled = true;
            }
            else
            {
                productComboBox.SelectedIndex = -1;
                productComboBox.IsEnabled = newEntryButton.IsEnabled = false;
            }
        }

        private void OnNewEntryClick(object sender, RoutedEventArgs e)
        {
            if (source.Count == 0)
            {
                var result = sqlHandler.ExecuteCommand(SqlSelectCommands.ShowProductsWithFollowingZeroForID(sqlHandler.Database, productIndexToID[productComboBox.SelectedIndex]));
                dataGrid.ItemsSource = source = result.Tables[0].DefaultView;
            }
            else
            {
                var result = sqlHandler.ExecuteCommand(SqlSelectCommands.ShowProductsWithFollowingZeroForID(sqlHandler.Database, productIndexToID[productComboBox.SelectedIndex]));
                var newRow = source.AddNew();
                for (int i = 0; i < newRow.Row.Table.Columns.Count; i++)
                {
                    newRow[i] = result.Tables[0].DefaultView[0][i];
                }
                newRow.EndEdit();
                dataGrid.ItemsSource = source;
            }
            SetProductComboBox();
            dataGrid.Visibility = Visibility.Visible;
        }

        private void OnDataGridColumnsGenerated(object sender, EventArgs e)
        {
            foreach (var column in dataGrid.Columns)
            {
                if (SqlConstants.translations.TryGetValue(column.Header.ToString(), out String result))
                {
                    column.Header = result;
                    column.IsReadOnly = true;
                }
            }

            if (dataGrid.Columns.Count > 0)
            {
                var lastColumn = dataGrid.Columns[dataGrid.Columns.Count - 1];
                lastColumn.CellStyle = new Style(typeof(DataGridCell));
                lastColumn.CellStyle.Setters.Add(new Setter(BackgroundProperty, new SolidColorBrush(Colors.LightSkyBlue)));
                lastColumn.Width = new DataGridLength(1, DataGridLengthUnitType.Star);

                if (!readOnly)
                {
                    dataGrid.Columns[0].Visibility = Visibility.Hidden;
                    lastColumn.IsReadOnly = false;
                }
            }
        }

        private void OnAcceptClick(object sender, RoutedEventArgs e)
        {
            if (contractorComboBox.SelectedIndex != -1 && dataGrid.Items.Count > 0)
            {
                bool validated = true;
                foreach(DataRowView row in dataGrid.Items)
                {
                    double lastColumnData = Convert.ToDouble(row[row.Row.Table.Columns.Count - 1]);
                    if (lastColumnData.ToString() == "" || lastColumnData.ToString() == "0")
                    {
                        validated = false;
                        break;
                    }
                }

                var list = sqlHandler.DataSetToList(sqlHandler.ExecuteCommand(SqlSelectCommands.ShowLastDocumentNumber(sqlHandler.Database, datePicker.Text)));
                int number = 0;
                if (list[0][0] != null && list[0][0] != "")
                    number = Convert.ToInt32(list[0][0].ToString());
                String nextCode = ((++number).ToString()).PadLeft(4, '0') + @"/" + Convert.ToDateTime(datePicker.Text).Year;
                String warehouseId = sqlHandler.DataSetToList(sqlHandler.ExecuteCommand(SqlSelectCommands.ShowWarehouseNameToId(sqlHandler.Database, warehouseName)))[0][0];

                if (validated)
                {
                    if (documentNature == DocumentNature.Buy)
                    {
                        sqlHandler.ExecuteNonQuery(SqlInsertCommands.InsertOrder(sqlHandler.Database, nextCode, contractorIndexToID[contractorComboBox.SelectedIndex],
                            warehouseId, documentType, "0", datePicker.SelectedDate.ToString()));
                        String insertedOrderId = sqlHandler.DataSetToList(sqlHandler.ExecuteCommand(SqlSelectCommands.ShowLastInsertedID(sqlHandler.Database, "orders")))[0][0];
                        foreach(DataRowView row in dataGrid.Items)
                        {
                            sqlHandler.ExecuteNonQuery(SqlInsertCommands.InsertOrderDetails(sqlHandler.Database, insertedOrderId.ToString(), 
                                row[0].ToString(), row[row.Row.Table.Columns.Count - 1].ToString()));
                        }
                        Close();
                    }
                    else if (documentNature == DocumentNature.Sell)
                    {
                        int lastColumnIndex = dataGrid.Columns.Count - 1;
                        DataSet productsInWarehouse = sqlHandler.ExecuteCommand(SqlSelectCommands.ShowProductsInWarehouse(sqlHandler.Database, warehouseName));
                        var complexProducts = sqlHandler.DataSetToList(sqlHandler.ExecuteCommand(SqlSelectCommands.ShowComplexProductsID(sqlHandler.Database)));
                        Dictionary<String, double> complexProductsToComplete = new Dictionary<string, double>();
                        List<String> complexProductsId = new List<String>();
                        Dictionary<String, double> componentsInNeed = new Dictionary<String, double>();
                        Dictionary<String, double> ordinaryProductsInNeed = new Dictionary<string, double>();

                        bool canProductsBeMoved = CanProductsBeMovedOut(ref complexProductsToComplete, ref complexProductsId, ref componentsInNeed, ref ordinaryProductsInNeed);
                        
                        if (canProductsBeMoved)
                        {
                            foreach (var component in componentsInNeed)
                            {
                                sqlHandler.ExecuteNonQuery(SqlUpdateCommands.UpdateProductToWarehouse(warehouseId, component.Key, (-component.Value).ToString()));
                            }

                            sqlHandler.ExecuteNonQuery(SqlInsertCommands.InsertOrder(sqlHandler.Database, nextCode, contractorIndexToID[contractorComboBox.SelectedIndex],
                                warehouseId, documentType, "1", datePicker.SelectedDate.ToString()));
                            String insertedOrderId = sqlHandler.DataSetToList(sqlHandler.ExecuteCommand(SqlSelectCommands.ShowLastInsertedID(sqlHandler.Database, "orders")))[0][0];

                            foreach (DataRowView row in dataGrid.Items)
                            {
                                if (complexProductsId.Contains(row[0].ToString()))
                                {
                                    bool isProductInWarehouse = sqlHandler.DataSetToList(sqlHandler.ExecuteCommand(
                                        SqlSelectCommands.CheckIfProductIdIsInWarehouse(warehouseId, row[0].ToString()))).Count == 0;

                                    if (isProductInWarehouse)
                                    {
                                        sqlHandler.ExecuteNonQuery(SqlInsertCommands.InsertProductToWarehouse(warehouseId, row[0].ToString(),
                                            row[lastColumnIndex].ToString()));
                                    }
                                    else
                                    {
                                        sqlHandler.ExecuteNonQuery(SqlUpdateCommands.UpdateProductToWarehouse(warehouseId,
                                            row[0].ToString(), row[lastColumnIndex].ToString()));
                                    }
                                    sqlHandler.ExecuteNonQuery(SqlInsertCommands.InsertOrderDetails(sqlHandler.Database, insertedOrderId.ToString(),
                                        row[0].ToString(), row[lastColumnIndex].ToString()));
                                }
                                else
                                {
                                    sqlHandler.ExecuteNonQuery(SqlInsertCommands.InsertOrderDetails(sqlHandler.Database, insertedOrderId.ToString(),
                                         row[0].ToString(), row[lastColumnIndex].ToString()));
                                }

                            }
                            Close();
                        }
                    }
                    else
                    {
                        int lastColumnIndex = dataGrid.Columns.Count - 1;
                        DataSet productsInWarehouse = sqlHandler.ExecuteCommand(SqlSelectCommands.ShowProductsInWarehouse(sqlHandler.Database, warehouseName));
                        var complexProducts = sqlHandler.DataSetToList(sqlHandler.ExecuteCommand(SqlSelectCommands.ShowComplexProductsID(sqlHandler.Database)));
                        Dictionary<String, double> complexProductsToComplete = new Dictionary<string, double>();
                        List<String> complexProductsId = new List<String>();
                        Dictionary<String, double> componentsInNeed = new Dictionary<String, double>();
                        Dictionary<String, double> ordinaryProductsInNeed = new Dictionary<string, double>();

                        bool canProductsBeMoved = CanProductsBeMovedOut(ref complexProductsToComplete, ref complexProductsId, ref componentsInNeed, ref ordinaryProductsInNeed);

                        if (canProductsBeMoved)
                        {
                            foreach (var component in componentsInNeed)
                            {
                                sqlHandler.ExecuteNonQuery(SqlUpdateCommands.UpdateProductToWarehouse(warehouseId, component.Key, (-component.Value).ToString()));
                            }

                            sqlHandler.ExecuteNonQuery(SqlInsertCommands.InsertOrderForMM(sqlHandler.Database, nextCode, warehouseId, 
                                contractorIndexToID[contractorComboBox.SelectedIndex], documentType +"-", "1", datePicker.SelectedDate.ToString()));
                            String moveOutOrderId = sqlHandler.DataSetToList(sqlHandler.ExecuteCommand(SqlSelectCommands.ShowLastInsertedID(sqlHandler.Database, "orders")))[0][0];

                            nextCode = ((++number).ToString()).PadLeft(4, '0') + @"/" + Convert.ToDateTime(datePicker.Text).Year;
                            sqlHandler.ExecuteNonQuery(SqlInsertCommands.InsertOrderForMM(sqlHandler.Database, nextCode, contractorIndexToID[contractorComboBox.SelectedIndex],
                                warehouseId, documentType+"+", "0", datePicker.SelectedDate.ToString()));
                            String moveInOrderId = sqlHandler.DataSetToList(sqlHandler.ExecuteCommand(SqlSelectCommands.ShowLastInsertedID(sqlHandler.Database, "orders")))[0][0];

                            foreach (DataRowView row in dataGrid.Items)
                            {
                                if (complexProductsId.Contains(row[0].ToString()))
                                {
                                    bool isProductInWarehouse = sqlHandler.DataSetToList(sqlHandler.ExecuteCommand(
                                        SqlSelectCommands.CheckIfProductIdIsInWarehouse(warehouseId, row[0].ToString()))).Count == 0;

                                    if (isProductInWarehouse)
                                    {
                                        sqlHandler.ExecuteNonQuery(SqlInsertCommands.InsertProductToWarehouse(warehouseId, row[0].ToString(),
                                            row[lastColumnIndex].ToString()));
                                    }
                                    else
                                    {
                                        sqlHandler.ExecuteNonQuery(SqlUpdateCommands.UpdateProductToWarehouse(warehouseId,
                                            row[0].ToString(), row[lastColumnIndex].ToString()));
                                    }

                                    isProductInWarehouse = sqlHandler.DataSetToList(sqlHandler.ExecuteCommand(
                                        SqlSelectCommands.CheckIfProductIdIsInWarehouse(warehouseId, row[0].ToString()))).Count == 0;

                                    if (isProductInWarehouse)
                                    {
                                        sqlHandler.ExecuteNonQuery(SqlInsertCommands.InsertProductToWarehouse(contractorIndexToID[contractorComboBox.SelectedIndex], 
                                            row[0].ToString(), row[lastColumnIndex].ToString()));
                                    }
                                    else
                                    {
                                        sqlHandler.ExecuteNonQuery(SqlUpdateCommands.UpdateProductToWarehouse(contractorIndexToID[contractorComboBox.SelectedIndex],
                                            row[0].ToString(), row[lastColumnIndex].ToString()));
                                    }
                                    sqlHandler.ExecuteNonQuery(SqlInsertCommands.InsertOrderDetails(sqlHandler.Database, moveOutOrderId.ToString(),
                                        row[0].ToString(), row[lastColumnIndex].ToString()));
                                    sqlHandler.ExecuteNonQuery(SqlInsertCommands.InsertOrderDetails(sqlHandler.Database, moveInOrderId.ToString(),
                                        row[0].ToString(), row[lastColumnIndex].ToString()));
                                }
                                else
                                {
                                    sqlHandler.ExecuteNonQuery(SqlInsertCommands.InsertOrderDetails(sqlHandler.Database, moveOutOrderId.ToString(),
                                         row[0].ToString(), row[lastColumnIndex].ToString()));
                                    sqlHandler.ExecuteNonQuery(SqlInsertCommands.InsertOrderDetails(sqlHandler.Database, moveInOrderId.ToString(),
                                        row[0].ToString(), row[lastColumnIndex].ToString()));
                                }
                            }
                            Close();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Każdy produkt powinien mieć podaną ilość");
                }
            }
            else
            {
                MessageBox.Show("Upewnij się, że pole Kontrahent jest wypełnione a do dokumentu przypisano co najmniej produkt");
            }
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            deleteButton.IsEnabled = dataGrid.SelectedIndex > -1;
        }

        private void OnDeleteEntryClick(object sender, RoutedEventArgs e)
        {
            source.Delete(dataGrid.SelectedIndex);
            SetProductComboBox();
            if (dataGrid.Items.Count < 1)
            {
                dataGrid.Visibility = Visibility.Hidden;
                deleteButton.IsEnabled = false;
            }
        }

        private void OnCancelClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private bool CanProductsBeMovedOut(ref Dictionary<String, double> complexProductsToComplete, ref List<String> complexProductsId, 
            ref Dictionary<String, double> componentsInNeed, ref Dictionary<String, double> ordinaryProductsInNeed)
        {
            int lastColumnIndex = dataGrid.Columns.Count - 1;
            DataSet productsInWarehouse = sqlHandler.ExecuteCommand(SqlSelectCommands.ShowProductsInWarehouse(sqlHandler.Database, warehouseName));
            var complexProducts = sqlHandler.DataSetToList(sqlHandler.ExecuteCommand(SqlSelectCommands.ShowComplexProductsID(sqlHandler.Database)));

            foreach (DataRowView row in dataGrid.Items)
            {
                foreach (var node in complexProducts)
                {
                    if (node[0] == row[0].ToString())
                    {
                        complexProductsId.Add(node[0].ToString());
                        bool isProductInWarehouse = false;
                        foreach (DataRowView productInWarehouseRow in productsInWarehouse.Tables[0].DefaultView)
                        {
                            if (productInWarehouseRow[0].ToString() == node[0])
                            {
                                isProductInWarehouse = true;
                                double itemsInWarehouseAfterSell = Convert.ToDouble(productInWarehouseRow[lastColumnIndex].ToString())
                                    - Convert.ToDouble(row[lastColumnIndex].ToString());
                                if (itemsInWarehouseAfterSell < 0)
                                {
                                    complexProductsToComplete[node[0]] = -itemsInWarehouseAfterSell;
                                }
                                break;
                            }
                        }
                        if (isProductInWarehouse == false)
                        {
                            complexProductsToComplete[node[0]] = Convert.ToDouble(row[lastColumnIndex].ToString());
                        }
                    }
                }
            }

            foreach (DataRowView row in dataGrid.Items)
            {
                if (!complexProductsId.Contains(row[0].ToString()))
                {
                    bool productIsInWarehouse = false;
                    foreach (DataRowView productInWarehouseRow in productsInWarehouse.Tables[0].DefaultView)
                    {
                        if (row[0].ToString() == productInWarehouseRow[0].ToString())
                        {
                            productIsInWarehouse = true;
                            double amountInWarehouse = Convert.ToDouble(productInWarehouseRow[3].ToString());
                            double amountToDelete = Convert.ToDouble(row[lastColumnIndex].ToString());
                            if (amountInWarehouse - amountToDelete < 0)
                            {
                                MessageBox.Show("Za mało przedmiotów w magazynie by sprzedać produkt");
                                return false;
                            }
                            else
                            {
                                bool result = ordinaryProductsInNeed.TryGetValue(row[0].ToString(), out double sth);
                                if (!result)
                                {
                                    ordinaryProductsInNeed.Add(row[0].ToString(), 0);
                                }
                                ordinaryProductsInNeed[row[0].ToString()] += amountToDelete;
                            }
                            break;
                        }
                    }

                    if (productIsInWarehouse == false)
                    {
                        MessageBox.Show("Za mało przedmiotów w magazynie by sprzedać produkt");
                        return false;
                    }
                }
            }

            foreach (var pair in complexProductsToComplete)
            {
                var productRecipe = sqlHandler.ExecuteCommand(SqlSelectCommands.ShowComplexProductRecipe(sqlHandler.Database, pair.Key));
                foreach (DataRowView recipeRow in productRecipe.Tables[0].DefaultView)
                {
                    double recipeMultipler = Convert.ToDouble(recipeRow[1].ToString());
                    foreach (DataRowView row in dataGrid.Items)
                    {
                        if (row[0].ToString() == pair.Key)
                        {
                            double amountToDelete = Convert.ToDouble(row[lastColumnIndex].ToString()) * recipeMultipler;
                            if (componentsInNeed.ContainsKey(recipeRow[0].ToString()))
                            {
                                componentsInNeed[recipeRow[0].ToString()] += amountToDelete;
                            }
                            else
                            {
                                componentsInNeed.Add(recipeRow[0].ToString(), amountToDelete);
                            }

                            if (ordinaryProductsInNeed.ContainsKey(recipeRow[0].ToString()))
                            {
                                ordinaryProductsInNeed[recipeRow[0].ToString()] += amountToDelete;
                            }
                            else
                            {
                                ordinaryProductsInNeed.Add(recipeRow[0].ToString(), amountToDelete);
                            }
                            break;
                        }
                    }
                }

                foreach (DataRowView recipeRow in productRecipe.Tables[0].DefaultView)
                {
                    bool componentIsInWarehouse = false;
                    foreach (DataRowView productInWarehouseRow in productsInWarehouse.Tables[0].DefaultView)
                    {
                        if (recipeRow[0].ToString() == productInWarehouseRow[0].ToString())
                        {
                            componentIsInWarehouse = true;
                            double amountInWarehouse = Convert.ToDouble(productInWarehouseRow[3].ToString());
                            double amountToDelete = ordinaryProductsInNeed[recipeRow[0].ToString()];
                            if (amountInWarehouse - amountToDelete < 0)
                            {
                                MessageBox.Show("Za mało przedmiotów w magazynie by ukończyć transakcję");
                                return false;
                            }
                            break;
                        }
                    }
                    if (componentIsInWarehouse == false)
                    {
                        MessageBox.Show("Za mało przedmiotów w magazynie by ukończyć transakcję");
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
