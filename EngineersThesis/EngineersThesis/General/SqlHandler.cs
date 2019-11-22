using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Data;

namespace EngineersThesis.General
{
    public class SqlHandler
    {
        public String Server { get; set; }
        public String Database { get; set; }
        public String Uid { get; set; }
        public String Password { get; set; }
        private MySqlConnection connection;
        private const uint timeout = 3;

        public SqlHandler() { }
        public SqlHandler (String Server, String Database, String Uid, String Password)
        {
            this.Server = Server;
            this.Database = Database;
            this.Uid = Uid;
            this.Password = Password;
        }

        public DataSet ExecuteCommand(String command)
        {
            var dataSet = new DataSet();
            if (Connect())
            {
                var dataAdapter = new MySqlDataAdapter(command, connection);
                dataSet = new DataSet();
                dataAdapter.Fill(dataSet);
                Disconnect();
            }
            return dataSet;
        }

        public List<List<String>> DataSetToList(DataSet dataSet)
        {
            var result = new List<List<String>>();
            foreach(DataTable table in dataSet.Tables)
            {
                foreach(DataRow row in table.Rows)
                {
                    var list = new List<String>();
                    foreach(DataColumn col in table.Columns)
                    {
                        list.Add(row[col].ToString());
                    }
                    result.Add(list);
                }
            }
            return result;
        }

        public List<List<String>> CleanUselessDatabases(ref List<List<String>> entry)
        {
            foreach (String knownDatabase in SqlConstants.extraDatabases)
            {
                for (int i = 0; i < entry.Count; i++)
                {
                    entry[i].RemoveAll(x => x == knownDatabase);
                }
            }
            entry.RemoveAll(x => x.Count == 0);
            return entry;
        }

        public void PrepareDatabase()
        {
            if (Database != null && Database != "")
            {
                ExecuteCommand(SqlConstants.CreateUnitTableCommand(Database));
                ExecuteCommand(SqlConstants.CreateWarehousesTableCommand(Database));
                ExecuteCommand(SqlConstants.CreateConstractorsTableCommand(Database));
                ExecuteCommand(SqlConstants.CreateProductsTableCommand(Database));
                ExecuteCommand(SqlConstants.CreateOrdersTableCommand(Database));
                ExecuteCommand(SqlConstants.CreateOrderDetailsTableCommand(Database));
            }
        }

        private void BuildConnection()
        {
            var builder = new MySqlConnectionStringBuilder()
            {
                Server = Server,
                UserID = Uid,
                Password = Password,
                Database = Database,
                ConnectionTimeout = timeout
            };

            connection = new MySqlConnection(builder.ToString());
        }

        private bool Connect()
        {
            BuildConnection();
            try
            {
                connection.Open();
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show("Polaczenie sie nie udalo (" + e.Message + ")", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void Disconnect()
        {
            connection.Close();
        }
    }
}
