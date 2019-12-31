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

        private String warehouseId;
        private String warehouseShortcut;
        private String warehouseName;

        private int selectedDocumentRow, selectedContractorRow;

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
        }

        private void ChooseDatabase(object sender, RoutedEventArgs e)
        {
            welcomeScreen.ShowDialog();
            sqlHandler = welcomeScreen.SqlHandler;
            sqlHandler.PrepareDatabase();
            if (welcomeScreen.LifoOrFifo != null)
            {
                sqlHandler.ExecuteNonQuery(SqlInsertCommands.InsertSettings("1", welcomeScreen.LifoOrFifo));
                sqlHandler.ExecuteNonQuery(SqlInsertCommands.InsertSettings("2", welcomeScreen.DateFromat));
            }
            SetContractorGrid();
            if(sqlHandler.Database != "" && sqlHandler.Database != null)
            {
                OpenWarehousesManagerButton.IsEnabled = companyManageButton.IsEnabled = true;
                OpenWarehousesManager(new object(), new RoutedEventArgs());
                if (warehouseName != null && warehouseName != "")
                {
                    SetDocumentGrid();
                    warehouseId = sqlHandler.DataSetToList(sqlHandler.ExecuteCommand(SqlSelectCommands.ShowWarehouseNameToId(sqlHandler.Database, warehouseName)))[0][0];
                }
            }
            else
            {
                SetButtonsEnability(false);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            welcomeScreen.Close();
        }

        private void SetProductGrid()
        {
            var database = sqlHandler.Database;
            if (database != null)
            {
                SetButtonsEnability(true);

                var dataSet = sqlHandler.ExecuteCommand(SqlSelectCommands.ShowProductsInWarehouse(sqlHandler.Database, warehouseName));
                dataGridProducts.ItemsSource = dataSet.Tables[0].DefaultView;
                foreach (var column in dataGridProducts.Columns)
                {
                    if (SqlConstants.translations.TryGetValue(column.Header.ToString(), out String result))
                        column.Header = result;
                }
                dataGridProducts.Visibility = Visibility.Visible;
                dataGridProducts.Columns[0].Visibility = Visibility.Hidden;
            }
            else
            {
                SetButtonsEnability(false);
            }
        }

        private void SetDocumentGrid()
        {
            var database = sqlHandler.Database;
            if (database != null)
            {
                String warehouseId = sqlHandler.DataSetToList(sqlHandler.ExecuteCommand(SqlSelectCommands.ShowWarehouseNameToId(sqlHandler.Database, warehouseName)))[0][0];
                var dataSet = sqlHandler.ExecuteCommand(SqlSelectCommands.ShowOrders(sqlHandler.Database, warehouseId));
                dataGridDocuments.ItemsSource = dataSet.Tables[0].DefaultView;
                foreach (var column in dataGridDocuments.Columns)
                {
                    if (SqlConstants.translations.TryGetValue(column.Header.ToString().ToLower(), out String result))
                        column.Header = result;
                }
                dataGridDocuments.Visibility = Visibility.Visible;
            }
        }

        private void SetContractorGrid()
        {
            var database = sqlHandler.Database;
            if (database != null)
            {
                var dataSet = sqlHandler.ExecuteCommand(SqlSelectCommands.ShowContractors(sqlHandler.Database));
                dataGridContractors.ItemsSource = dataSet.Tables[0].DefaultView;
                foreach (var column in dataGridContractors.Columns)
                {
                    if (SqlConstants.translations.TryGetValue(column.Header.ToString().ToLower(), out String result))
                        column.Header = result;
                }
                dataGridContractors.Visibility = Visibility.Visible;
                dataGridContractors.Columns[0].Visibility = Visibility.Hidden;
            }
        }

        private void OnManageProductButtonClick(object sender, RoutedEventArgs e)
        {
            var productEditor = new ProductEditor(sqlHandler)
            {
                Owner = this
            };
            productEditor.ShowDialog();
            SetProductGrid();
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
                SetProductGrid();
                SetDocumentGrid();
            }
        }

        private void OnAddContractor(object sender, RoutedEventArgs e)
        {
            var contractorEditor = new ContractorEditor(sqlHandler)
            {
                Owner = this
            };
            contractorEditor.ShowDialog();
            SetContractorGrid();
        }

        private void OnEditContractor(object sender, RoutedEventArgs e)
        {
            var contractorEditor = new ContractorEditor(sqlHandler, (DataRowView)dataGridContractors.Items[selectedContractorRow])
            {
                Owner = this
            };
            contractorEditor.ShowDialog();
            SetContractorGrid();
        }

        private void OnDeleteContractor(object sender, RoutedEventArgs e)
        {
            if (selectedContractorRow != -1)
            {
                string contractorId = ((DataRowView)dataGridContractors.SelectedItem)[0].ToString();
                List<List<String>> contractorUsedIn = sqlHandler.DataSetToList(sqlHandler.ExecuteCommand(SqlSelectCommands.ShowDocumentWithContractor(contractorId)));
                if (contractorUsedIn.Count == 0)
                {
                    sqlHandler.ExecuteNonQuery(SqlDeleteCommands.DeleteContractor(sqlHandler.Database, contractorId));
                }
                else
                {
                    string message = "Błąd! Kontrahent został już użyty w następujących dokumentach\n";
                    foreach (var node in contractorUsedIn)
                    {
                        message += node[0] + ", ";
                    }
                    message = message.Remove(message.Length - 2);
                    MessageBox.Show(message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            SetContractorGrid();
        }

        private void OnContractorGridSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedContractorRow = dataGridContractors.SelectedIndex;
            editContractorButton.IsEnabled = deleteContractorButton.IsEnabled = selectedContractorRow != -1;
        }

        private void OnManageCompanyInfo(object sender, RoutedEventArgs e)
        {
            bool isCompanySet = sqlHandler.DataSetToList(sqlHandler.ExecuteCommand(SqlSelectCommands.ShowMyCompanyData(sqlHandler.Database))).Count == 1;
            if (isCompanySet)
            {
                var result = sqlHandler.ExecuteCommand(SqlSelectCommands.ShowMyCompanyData(sqlHandler.Database));
                DataGrid dataGrid = new DataGrid()
                {
                    ItemsSource = result.Tables[0].DefaultView
                };
                var contractorEditor = new ContractorEditor(sqlHandler, (DataRowView)dataGrid.Items[0])
                {
                    Owner = this
                };
                contractorEditor.ShowDialog();
            }
            else
            {
                var contractorEditor = new ContractorEditor(sqlHandler, true)
                {
                    Owner = this
                };
                contractorEditor.ShowDialog();
            }
        }

        private void OnDocumentGridSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedDocumentRow = dataGridDocuments.SelectedIndex;
            showDocumentDetailsButton.IsEnabled = generatePdfButton.IsEnabled = selectedDocumentRow > -1;
        }

        private void OnAddNewDocumentClick(object sender, RoutedEventArgs e)
        {
            bool areAnyProducts = sqlHandler.DataSetToList(sqlHandler.ExecuteCommand(SqlSelectCommands.ShowProducts(sqlHandler.Database))).Count > 0;
            bool areAnyContractors = sqlHandler.DataSetToList(sqlHandler.ExecuteCommand(SqlSelectCommands.ShowContractors(sqlHandler.Database))).Count > 0;
            if (areAnyProducts && areAnyContractors)
            {
                String documentType = documentCategoryComboBox.Text;
                var documentEditor = new DocumentEditor(sqlHandler, warehouseName, documentType)
                {
                    Owner = this
                };
                documentEditor.ShowDialog();
                SetProductGrid();
                SetDocumentGrid();
            }
            else
            {
                MessageBox.Show("W przypadku braku znanych towarów lub kontrahentów, tworzenie dokumentów nie jest możliwe!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void OnGeneratePdfClick(object sender, RoutedEventArgs e)
        {
            var info = sqlHandler.DataSetToList(sqlHandler.ExecuteCommand(SqlSelectCommands.ShowMyCompanyData(sqlHandler.Database)));
            bool noCompany = false;
            if (info.Count > 0)
            {
                if(info[0].Count > 0)
                {
                    if (info[0][0] != "" && info[0][0] != null)
                    {
                        var pdfGenerator = new PdfGenerator(sqlHandler, (DataRowView)dataGridDocuments.Items[dataGridDocuments.SelectedIndex], warehouseName);
                        pdfGenerator.ExportToPdf();
                    }
                    else
                        noCompany = true;
                }
                else
                    noCompany = true;
            }
            else
                noCompany = true;

            if (noCompany)
                MessageBox.Show("Wprowadź informacje o firmie");
        }

        private void OnShowDocumentDetails(object sender, RoutedEventArgs e)
        {
            var row = (DataRowView)dataGridDocuments.Items[selectedDocumentRow];
            var documentEditor = new DocumentEditor(sqlHandler, row)
            {
                Owner = this
            };
            documentEditor.ShowDialog();
            SetProductGrid();
            SetDocumentGrid();
        }


        private void SetButtonsEnability(bool isEnabled)
        {
            ManageProductsButton.IsEnabled = addContractorButton.IsEnabled = OpenWarehousesManagerButton.IsEnabled =
                companyManageButton.IsEnabled = newDocumentButton.IsEnabled = documentCategoryComboBox.IsEnabled = ProductsOnDocumentsButton.IsEnabled = isEnabled;
            documentCategoryComboBox.SelectedIndex = isEnabled ? 0 : -1;
        }

        private void OnShowStatisticsButton(object sender, RoutedEventArgs e)
        {
            var statistics = new Statistics(sqlHandler)
            {
                Owner = this
            };
            statistics.ShowDialog();
        }

        private void OnShowStockTakingButtonClick(object sender, RoutedEventArgs e)
        {
            var stockTaking = new StockTaking()
            {
                Owner = this
            };
            stockTaking.ShowDialog();
        }

        private void OnProductsOnDocumentsClick(object sender, RoutedEventArgs e)
        {
            var productsOnDocuments = new ProductsOnDocuments(sqlHandler, warehouseId)
            {
                Owner = this
            };
            productsOnDocuments.ShowDialog();
        }
    }
}
