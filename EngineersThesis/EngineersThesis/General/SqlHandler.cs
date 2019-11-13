using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace EngineersThesis.General
{
    class SqlHandler
    {
        public String Server { get; set; }
        public String Database { get; set; }
        public String Uid { get; set; }
        public String Password { get; set; }
        private MySqlConnection connection;

        public SqlHandler() { }
        public SqlHandler (String Server, String Database, String Uid, String Password)
        {
            this.Server = Server;
            this.Database = Database;
            this.Uid = Uid;
            this.Password = Password;
        }

        public String ExecuteCommand(String command)
        {
            String sth = "";
            if (Connect())
            {
                var serverCommand = new MySqlCommand(command, connection);
                var serverReader = serverCommand.ExecuteReader();
                while (serverReader.Read())
                {
                    for (int i = 0; i < serverReader.FieldCount; i++)
                    {
                        sth += serverReader.GetValue(i) + " ";
                    }
                }
                Disconnect();
            }
            return sth;
        }

        private void BuildConnection()
        {
            var builder = new MySqlConnectionStringBuilder()
            {
                Server = Server,
                UserID = Uid,
                Password = Password,
                Database = Database
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
            catch (Exception)
            {
                MessageBox.Show("Polaczenie sie nie udalo");
                return false;
            }
        }

        private void Disconnect()
        {
            connection.Close();
        }
    }
}
