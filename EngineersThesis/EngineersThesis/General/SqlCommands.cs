using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineersThesis.General
{
    class SqlCommands
    {
        public static String ShowProductsCommand(String database)
        {
            return $"SELECT name, amount, unit, price FROM `{database}`.`products`;";
        }

        public static String ShowWarehousesCommand(String database)
        {
            return $"SELECT short, name FROM `{database}`.`warehouses`;";
        }

        public static String InsertNewWarehouse(String database, String shortcut, String name)
        {
            return $"INSERT INTO `{database}`.`warehouses` (SHORT, NAME) VALUES ('{shortcut}', '{name}');";
        }

        public static String UpdateWarehouse(String database, String oldShortcut, String oldName, String newShortcut, String newName)
        {
            return $"UPDATE `{database}`.`warehouses` SET SHORT = '{newShortcut}', NAME = '{newName}' WHERE SHORT = '{oldShortcut}' AND NAME = '{oldName}';";
        }

        public static String DeleteFromWarehouses(String database, String shortcut, String name)
        {
            return $"DELETE FROM `{database}`.`warehouses` WHERE SHORT = '{shortcut}' AND NAME = '{name}';";
        }

        public static String AllowDiactricMarksCommand(String database)
        {
            return $"ALTER DATABASE `{database}` DEFAULT CHARACTER SET utf8 COLLATE utf8_unicode_ci";
        }

        public static String IfDatabaseAlreadyExistCommand(String name)
        {
            return SqlConstants.showDatabases + $" like '{name}'";
        }

        public static String NewDatabaseCommand(String name)
        {
            return $"CREATE SCHEMA `{name}`;";
        }

        public static String WarehousesTableCommand(String database)
        {
            return $"CREATE TABLE IF NOT EXISTS `{database}`.`warehouses` (" +
                $"ID int PRIMARY KEY AUTO_INCREMENT," +
                $"SHORT varchar(4) NOT NULL UNIQUE," +
                $"NAME varchar(20) NOT NULL UNIQUE);";
        }

        public static String ProductsTableCommand(String database)
        {
            return $"CREATE TABLE IF NOT EXISTS `{database}`.`products` (" +
                $"ID int PRIMARY KEY AUTO_INCREMENT," +
                $"NAME varchar(255) NOT NULL," +
                $"CLASS varchar(255)," +
                $"WAREHOUSE_ID int NOT NULL," +
                $"AMOUNT double default 0," +
                $"UNIT varchar(5) NOT NULL," +
                $"PRICE double NOT NULL," +
                $"FOREIGN KEY(WAREHOUSE_ID) REFERENCES `{database}`.`warehouses`(ID));";
        }

        public static String ConstractorsTableCommand(String database)
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
                $"ADDRESS varchar(255)," +
                $"UNIQUE(NAME, ADDRESS));";
        }

        public static String OrdersTableCommand(String database)
        {
            return $"CREATE TABLE IF NOT EXISTS `{database}`.`orders`(" +
                $"ID int PRIMARY KEY AUTO_INCREMENT," +
                $"CONTRACTOR_ID int NOT NULL," +
                $"WAREHOUSE int NOT NULL," +
                $"DATE date," +
                $"FOREIGN KEY(CONTRACTOR_ID) REFERENCES `{database}`.`contractors`(ID)ON DELETE CASCADE," +
                $"FOREIGN KEY(WAREHOUSE) REFERENCES `{database}`.`warehouses`(ID)ON DELETE CASCADE);";
        }

        public static String OrderDetailsTableCommand(String database)
        {
            return $"CREATE TABLE IF NOT EXISTS `{database}`.`order_details`(" +
                $"ID int PRIMARY KEY AUTO_INCREMENT," +
                $"ORDER_ID int NOT NULL," +
                $"PRODUCT_ID int NOT NULL," +
                $"AMOUNT double NOT NULL," +
                $"FOREIGN KEY(ORDER_ID) REFERENCES `{database}`.`orders`(ID)ON DELETE CASCADE," +
                $"FOREIGN KEY(PRODUCT_ID) REFERENCES `{database}`.`products`(ID)ON DELETE CASCADE);";
        }
    }
}
