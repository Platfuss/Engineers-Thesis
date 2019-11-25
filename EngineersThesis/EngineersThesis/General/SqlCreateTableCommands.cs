using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineersThesis.General
{
    class SqlCreateTableCommands
    {
        public static String WarehousesTable(String database)
        {
            return $"CREATE TABLE IF NOT EXISTS `{database}`.`warehouses` (" +
                $"ID int PRIMARY KEY AUTO_INCREMENT," +
                $"SHORT varchar(4) NOT NULL UNIQUE," +
                $"NAME varchar(20) NOT NULL UNIQUE" +
                $");";
        }

        public static String ProductsTable(String database)
        {
            return $"CREATE TABLE IF NOT EXISTS `{database}`.`products` (" +
                $"ID int PRIMARY KEY AUTO_INCREMENT," +
                $"NAME varchar(255) UNIQUE NOT NULL," +
                $"UNIT varchar(5) NOT NULL," +
                $"PRICE double NOT NULL," +
                $"TAX int default 0," +
                $"COMPLEX bool default false" +
                $");";
        }

        public static String WarehousesProducts(String database)
        {
            return $"CREATE TABLE IF NOT EXISTS `{database}`.`warehouses_products` (" +
                $"WAREHOUSE_ID int NOT NULL," +
                $"PRODUCT_ID int NOT NULL," +
                $"AMOUNT double default 0," +
                $"PRIMARY KEY(WAREHOUSE_ID, PRODUCT_ID)," +
                $"FOREIGN KEY(WAREHOUSE_ID) REFERENCES `{database}`.`warehouses`(ID) ON DELETE CASCADE," +
                $"FOREIGN KEY(PRODUCT_ID) REFERENCES `{database}`.`products`(ID) ON DELETE CASCADE" +
                $");";
        }

        public static String ConstractorsTable(String database)
        {
            return $"CREATE TABLE IF NOT EXISTS `{database}`.`contractors` (" +
                $"ID int PRIMARY KEY AUTO_INCREMENT," +
                $"NAME varchar(255) NOT NULL," +
                $"SUPPLIER bit," +
                $"PURCHASER bit," +
                $"PHONE_NUMBER int(15)," +
                $"EMAIL varchar(255)," +
                $"TAX_CODE int(30)," +
                $"ADDRESS varchar(255)," +
                $"UNIQUE(NAME, ADDRESS)" +
                $");";
        }

        public static String OrdersTable(String database)
        {
            return $"CREATE TABLE IF NOT EXISTS `{database}`.`orders`(" +
                $"ID int PRIMARY KEY AUTO_INCREMENT," +
                $"CONTRACTOR_ID int NOT NULL," +
                $"WAREHOUSE int NOT NULL," +
                $"DATE date," +
                $"FOREIGN KEY(CONTRACTOR_ID) REFERENCES `{database}`.`contractors`(ID) ON DELETE CASCADE," +
                $"FOREIGN KEY(WAREHOUSE) REFERENCES `{database}`.`warehouses`(ID) ON DELETE CASCADE" +
                $");";
        }

        public static String OrderDetailsTable(String database)
        {
            return $"CREATE TABLE IF NOT EXISTS `{database}`.`order_details`(" +
                $"ID int PRIMARY KEY AUTO_INCREMENT," +
                $"ORDER_ID int NOT NULL," +
                $"PRODUCT_ID int NOT NULL," +
                $"AMOUNT double NOT NULL," +
                $"FOREIGN KEY(ORDER_ID) REFERENCES `{database}`.`orders`(ID)ON DELETE CASCADE," +
                $"FOREIGN KEY(PRODUCT_ID) REFERENCES `{database}`.`products`(ID)ON DELETE CASCADE" +
                $");";
        }
    }
}
