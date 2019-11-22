using System;
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
using System.Windows.Forms;
using EngineersThesis.General;

namespace EngineersThesis
{
    /// <summary>
    /// Interaction logic for WelcomeScreen.xaml
    /// </summary>
    public partial class WelcomeScreen : Window
    {
        public SqlHandler SqlHandler { get; private set; }
        private String server;
        private String uid;
        private String password;

        public WelcomeScreen()
        {
            InitializeComponent();
            upperTextBox.Text = Properties.Settings.Default.ipAddress;
            centerTextBox.Text = Properties.Settings.Default.userName;
            lowerTextBox.Password = Properties.Settings.Default.password;
            SqlHandler = new SqlHandler();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (Convert.ToBoolean(databaseSelection.Visibility) == Convert.ToBoolean(Visibility.Hidden))
                {
                    Button_Click_1(new object(), new RoutedEventArgs());
                }
                else
                {
                    DatabasePickButton_Click(new object(), new RoutedEventArgs());
                }
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            databaseSelection.Visibility = databaseSelectionLabel.Visibility = databasePickButton.Visibility = newDatabaseButton.Visibility = Visibility.Hidden;
            SqlHandler = new SqlHandler()
            {
                Server = server = upperTextBox.Text,
                Uid = uid = centerTextBox.Text,
                Password = password = lowerTextBox.Password
            };

            var result = SqlHandler.DataSetToList(SqlHandler.ExecuteCommand(SqlConstants.showDatabases));
            if (result.Count > 0)
            {
                SqlHandler.CleanUselessDatabases(ref result);
                if (result.Count > 0)
                {
                    databaseSelection.Items.Clear();
                    databaseSelection.Visibility = databaseSelectionLabel.Visibility = databasePickButton.Visibility = newDatabaseButton.Visibility = Visibility.Visible;
                    foreach (var dataBase in result)
                    {
                        databaseSelection.Items.Add(dataBase.First());
                    }
                    databaseSelection.SelectedIndex = databaseSelection.Items.Count - 1;
                    SaveSetting();
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Brak poprawnych baz danych", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void DatabasePickButton_Click(object sender, RoutedEventArgs e)
        {
            SqlHandler = new SqlHandler(server, databaseSelection.Text, uid, password);
            Hide();
        }

        private void AddNewBase(object sender, RoutedEventArgs e)
        {
            newBaseTextBox.Text = "";
            mainGrid.Visibility = Visibility.Hidden;
            newDatabaseGrid.Visibility = Visibility.Visible;
        }

        private void ConfirmNewBase(object sender, RoutedEventArgs e)
        {
            if (newBaseTextBox.Text != "")
            {
                var names = SqlHandler.DataSetToList(SqlHandler.ExecuteCommand(SqlConstants.CreateIfDatabaseAlreadyExistCommand(newBaseTextBox.Text)));
                if (names.Count == 0)
                {
                    SqlHandler.ExecuteCommand(SqlConstants.CreateNewDatabaseCommand(newBaseTextBox.Text));
                    newDatabaseGrid.Visibility = Visibility.Hidden;
                    mainGrid.Visibility = Visibility.Visible;
                    Button_Click_1(new object(), new RoutedEventArgs());
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Podana nazwa bazy jest już zajęta");
                }
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Nazwa nowej bazy jest pusta");
            }
        }

        private void DontAddNewBase(object sender, RoutedEventArgs e)
        {
            newDatabaseGrid.Visibility = Visibility.Hidden;
            mainGrid.Visibility = Visibility.Visible;
        }

        private void SaveSetting()
        {
            Properties.Settings.Default.ipAddress = upperTextBox.Text;
            Properties.Settings.Default.userName = centerTextBox.Text;
            Properties.Settings.Default.password = lowerTextBox.Password;
            Properties.Settings.Default.Save();
        }
    }
}
