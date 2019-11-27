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
using System.Text.RegularExpressions;
using System.Windows.Shapes;
using EngineersThesis.General;

namespace EngineersThesis
{
    /// <summary>
    /// Interaction logic for ProductEditorControl.xaml
    /// </summary>
    public partial class ProductEditorControl : Page
    {
        private SqlHandler sqlHandler;
        private Action setOwnerGrid;
        private bool editMode;
        private bool wasComplex;
        private DataRowView givenEditedRow;

        public ProductEditorControl(SqlHandler handler, Action setGrid, bool enabled)
        {
            InitializeComponent();
            sqlHandler = handler;
            setOwnerGrid = setGrid;
            nameTextBox.IsEnabled = buyNetTextBox.IsEnabled = taxComboBox.IsEnabled = unitComboBox.IsEnabled
                = normalProductRadioButton.IsEnabled = complexProductRadioButton.IsEnabled = AcceptButton.IsEnabled = enabled;
        }

        public ProductEditorControl(SqlHandler handler, Action setGrid, DataRowView data)
        {
            InitializeComponent();
            sqlHandler = handler;
            setOwnerGrid = setGrid;
            givenEditedRow = data;
            editMode = true;
            wasComplex = false;

            var list = sqlHandler.DataSetToList(sqlHandler.ExecuteCommand(SqlSelectCommands.ShowComplexProductsID(sqlHandler.Database)));
            foreach (var node in list)
            {
                if (node[0] == givenEditedRow[0].ToString())
                    wasComplex = true;
            }

            if (wasComplex)
            {
                complexProductRadioButton.IsChecked = tabComponent.IsEnabled = true;
                normalProductRadioButton.IsChecked = false;
            }
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            if (editMode)
            {
                nameTextBox.Text = givenEditedRow[1].ToString();
                buyNetTextBox.Text = givenEditedRow[4].ToString();
                for (int i = 0; i < unitComboBox.Items.Count; i++)
                {
                    if (Regex.Replace(unitComboBox.Items[i].ToString(), "System.Windows.Controls.ComboBoxItem: ", "") == givenEditedRow[2].ToString())
                    {
                        unitComboBox.SelectedIndex = i;
                        break;
                    }
                }

                for (int i = 0; i < taxComboBox.Items.Count; i++)
                {
                    if (Regex.Replace(unitComboBox.Items[i].ToString(), "System.Windows.Controls.ComboBoxItem: ", "") == givenEditedRow[3].ToString() + "%")
                    {
                        taxComboBox.SelectedIndex = i;
                        break;
                    }
                }
            }
            SetDataGrid();
        }

        private void SetDataGrid()
        {
            var database = sqlHandler.Database;
            if (database != null)
            {
                var dataSet = sqlHandler.ExecuteCommand(SqlSelectCommands.ShowProductsWithFollowingZero(sqlHandler.Database));
                dataGrid.ItemsSource = dataSet.Tables[0].DefaultView;
                OnDataGridColumnsGenerated(new object(), new EventArgs());
            }
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
                dataGrid.Columns[0].Visibility = Visibility.Hidden;
                var lastColumn = dataGrid.Columns[dataGrid.Columns.Count - 1];
                lastColumn.CellStyle = new Style(typeof(DataGridCell));
                lastColumn.CellStyle.Setters.Add(new Setter(Control.BackgroundProperty, new SolidColorBrush(Colors.LightSkyBlue)));
                lastColumn.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
                lastColumn.IsReadOnly = false;
            }

            if (editMode)
            {
                var id = givenEditedRow[0].ToString();
                var list = sqlHandler.DataSetToList(sqlHandler.ExecuteCommand(SqlSelectCommands.ShowComplexProductRecipe(sqlHandler.Database, id)));
                if (list.Count > 0)
                {
                    foreach (var node in list)
                    {
                        foreach(DataRowView row in dataGrid.Items)
                        {
                            if (row[0].ToString() == node[0])
                            {
                                row[row.Row.Table.Columns.Count - 1] = node[1];
                            }
                        }
                    }
                }

                foreach (DataRowView row in dataGrid.Items)
                {
                    if (row[0].ToString() == givenEditedRow[0].ToString())
                    {
                        row.Delete();
                        break;
                    }
                }
            }
        }

        private void OnNetValidation(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            e.Handled = !Regex.IsMatch(textBox.Text + e.Text, @"^\d*\,?\d{0,3}$");
        }

        private void OnTaxChanged(object sender, SelectionChangedEventArgs e)
        {
            SetGrossTextBox();
        }

        private void NormalProductChecked(object sender, RoutedEventArgs e)
        {
            complexProductRadioButton.IsChecked = false;
            tabComponent.IsEnabled = false;
        }

        private void ComplexProductChecked(object sender, RoutedEventArgs e)
        {
            normalProductRadioButton.IsChecked = false;
            tabComponent.IsEnabled = true;
        }

        private void OnNetTextChanged(object sender, TextChangedEventArgs e)
        {
            SetGrossTextBox();
        }

        private void SetGrossTextBox()
        {
            if (buyNetTextBox.Text != "" && taxComboBox.SelectedItem != null)
            {
                var taxComboBoxText = Regex.Replace(taxComboBox.SelectedItem.ToString(), "[^0-9]", "");
                var percent = Convert.ToDouble(taxComboBoxText) / 100;
                buyGrossTextBox.Text = Convert.ToString(Convert.ToDouble(buyNetTextBox.Text) * (1.0 + percent));
            }
        }

        private void OnAcceptClick(object sender, RoutedEventArgs e)
        {
            if (buyNetTextBox.Text != "" && buyGrossTextBox.Text != "" && unitComboBox.SelectedIndex != -1)
            {
                var taxComboBoxText = Regex.Replace(taxComboBox.SelectedItem.ToString(), "[^0-9]", "");
                var unitText = unitComboBox.Text;
                var price = Regex.Replace(buyNetTextBox.Text, @"\,", ".");

                if (editMode)
                {
                    if (wasComplex == false && !(bool)complexProductRadioButton.IsChecked)
                    {
                        sqlHandler.ExecuteCommand(SqlUpdateCommands.UpdateProductInfo(sqlHandler.Database, givenEditedRow[0].ToString(), nameTextBox.Text, unitText, price, taxComboBoxText));
                    }
                    else if (wasComplex == true && (bool)complexProductRadioButton.IsChecked)
                    {
                        sqlHandler.ExecuteNonQuery(SqlDeleteCommands.DeleteComplexity(sqlHandler.Database, givenEditedRow[0].ToString()));
                        sqlHandler.ExecuteCommand(SqlUpdateCommands.UpdateProductInfo(sqlHandler.Database, givenEditedRow[0].ToString(), nameTextBox.Text, unitText, price, taxComboBoxText));
                    }
                    else
                    {
                        var list = new List<Tuple<String, String>>();
                        bool dataValidate = DataGridDataToList(ref list);
                        if (dataValidate && list.Count > 0)
                        {
                            sqlHandler.ExecuteNonQuery(SqlDeleteCommands.DeleteComplexity(sqlHandler.Database, givenEditedRow[0].ToString()));
                            bool complexityIsntReversed = true;
                            foreach (var tuple in list)
                            {
                                var result = sqlHandler.DataSetToList(sqlHandler.ExecuteCommand(SqlSelectCommands.ShowProductReversedComponents(sqlHandler.Database, tuple.Item1, tuple.Item2)));
                                if(result.Count > 0)
                                {
                                    complexityIsntReversed = false;
                                    break;
                                }
                            }

                            if (complexityIsntReversed)
                            {
                                sqlHandler.ExecuteNonQuery(SqlDeleteCommands.DeleteProduct(sqlHandler.Database, givenEditedRow[0].ToString()));
                                bool added = sqlHandler.ExecuteNonQuery(SqlInsertCommands.InsertNewProduct(sqlHandler.Database, nameTextBox.Text, unitText, price, taxComboBoxText));
                                if (added) //ToDo check
                                {
                                    var result = sqlHandler.DataSetToList(sqlHandler.ExecuteCommand(SqlSelectCommands.ShowLastInsertedID(sqlHandler.Database, "products")));
                                    var lastId = result[0][0];
                                    if (lastId != "" && lastId != null)
                                        sqlHandler.ExecuteCommand(SqlInsertCommands.InsertComponents(sqlHandler.Database, lastId, list));
                                }
                            }
                            else
                            {
                                MessageBox.Show("Składniki produktu nie mogą składać się z niego samego", "Błąd", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                            }
                        }
                    }
                }
                else
                {
                    if ( !(bool)complexProductRadioButton.IsChecked)
                    {
                        sqlHandler.ExecuteCommand(SqlInsertCommands.InsertNewProduct(sqlHandler.Database, nameTextBox.Text, unitText, price, taxComboBoxText));
                    }
                    else
                    {
                        var list = new List<Tuple<String, String>>();
                        bool dataValidate = DataGridDataToList(ref list);
                        if (dataValidate && list.Count > 0)
                        {
                            bool added = sqlHandler.ExecuteNonQuery(SqlInsertCommands.InsertNewProduct(sqlHandler.Database, nameTextBox.Text, unitText, price, taxComboBoxText));
                            if (added) //ToDo check
                            {
                                var result = sqlHandler.DataSetToList(sqlHandler.ExecuteCommand(SqlSelectCommands.ShowLastInsertedID(sqlHandler.Database, "products")));
                                var lastId = result[0][0];
                                if (lastId != "" && lastId != null)
                                    sqlHandler.ExecuteCommand(SqlInsertCommands.InsertComponents(sqlHandler.Database, lastId, list));
                            }
                        }
                    }
                }
                setOwnerGrid();
            }
            else
            {
                MessageBox.Show("Nie wszystkie pola zostały wypełnione!");
            }
        }

        private bool DataGridDataToList(ref List<Tuple<String, String>> list)
        {
            bool dataValidate = true;
            foreach (DataRowView row in dataGrid.ItemsSource)
            {
                try
                {
                    var cellContent = row[row.Row.Table.Columns.Count - 1].ToString();
                    if (cellContent == "")
                        row[row.Row.Table.Columns.Count - 1] = cellContent = "0";
                    var amount = Convert.ToDouble(cellContent);
                    if (amount > 0)
                    {
                        var amountString = Regex.Replace(amount.ToString(), @"\.", ",");
                        list.Add(new Tuple<string, string>(row[0].ToString(), amountString));
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Niepoprawna ilość składników produktu", "Błąd", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    dataValidate = false;
                    break;
                }
            }
            return dataValidate;
        }
    }
}
