using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineersThesis.General
{
    class SqlConstants
    {
        public static readonly String[] extraDatabases = { "information_schema", "mysql", "performance_schema" };
        public static readonly String showDatabases = "show databases";

        public static String CreateIfDatabaseAlreadyExistCommand(String name)
        {
            return showDatabases + $" like '{name}'";
        }

        public static String CreateNewDatabaseCommand(String name)
        {
            return $"CREATE SCHEMA `{name}`;";
        }

        public static String CreateUnitTableCommand(String database)
        {
            return $"CREATE TABLE IF NOT EXISTS `{database}`.`units`(" +
                $"NAME varchar(10) PRIMARY KEY);";
        }

        public static String CreateWarehousesTableCommand(String database)
        {
            return $"CREATE TABLE IF NOT EXISTS `{database}`.`warehouses` (" +
                $"ID int PRIMARY KEY AUTO_INCREMENT," +
                $"SHORT varchar(4) NOT NULL," +
                $"NAME varchar(255) NOT NULL);";
        }

        public static String CreateProductsTableCommand(String database)
        {
            return $"CREATE TABLE IF NOT EXISTS `{database}`.`products` (" +
                $"ID int PRIMARY KEY AUTO_INCREMENT," +
                $"NAME varchar(255) NOT NULL," +
                $"CLASS varchar(255)," +
                $"AMOUNT double default 0," +
                $"UNIT varchar(10) NOT NULL," +
                $"PRICE int NOT NULL," +
                $"FOREIGN KEY(UNIT) REFERENCES `{database}`.`units`(NAME)ON DELETE CASCADE);";
        }

        public static String CreateConstractorsTableCommand(String database)
        {
            return $"CREATE TABLE IF NOT EXISTS `{database}`.`contractors` (" +
                $"ID int PRIMARY KEY AUTO_INCREMENT," +
                $"NAME varchar(255) NOT NULL," +
                $"SUPPLIER bit," +
                $"PURCHASER bit," +
                $"PHONE_NUMBER int(15)," +
                $"PHONE_OWNER varchar(255), " +
                $"EMAIL varchar(255)," +
                $"TAX_CODE int(30)," +
                $"ADDRESS varchar(255));";
        }

        public static String CreateOrdersTableCommand(String database)
        {
            return $"CREATE TABLE IF NOT EXISTS `{database}`.`orders`(" +
                $"ID int PRIMARY KEY AUTO_INCREMENT," +
                $"CONTRACTOR_ID int NOT NULL," +
                $"WAREHOUSE int NOT NULL," +
                $"DATE date," +
                $"FOREIGN KEY(CONTRACTOR_ID) REFERENCES `{database}`.`contractors`(ID)ON DELETE CASCADE," +
                $"FOREIGN KEY(WAREHOUSE) REFERENCES `{database}`.`warehouses`(ID)ON DELETE CASCADE);";
        }

        public static String CreateOrderDetailsTableCommand(String database)
        {
            return $"CREATE TABLE IF NOT EXISTS `{database}`.`order_details`(" +
                $"ORDER_ID int NOT NULL," +
                $"CONTRACTOR_ID int NOT NULL," +
                $"PRODUCT_ID int NOT NULL," +
                $"AMOUNT double NOT NULL," +
                $"PRIMARY KEY(ORDER_ID, CONTRACTOR_ID)," +
                $"FOREIGN KEY(ORDER_ID) REFERENCES `{database}`.`orders`(ID)ON DELETE CASCADE," +
                $"FOREIGN KEY(CONTRACTOR_ID) REFERENCES `{database}`.`contractors`(ID)ON DELETE CASCADE," +
                $"FOREIGN KEY(PRODUCT_ID) REFERENCES `{database}`.`products`(ID)ON DELETE CASCADE);";
        }
    }
}
