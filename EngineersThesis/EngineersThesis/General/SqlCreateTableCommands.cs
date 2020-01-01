using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineersThesis.General
{
    class SqlCreateTableCommands
    {
        public static String SettingsTable()
        {
            return
                "CREATE TABLE IF NOT EXISTS `settings` (" +
                "ID int PRIMARY KEY," +
                "VALUE int" +
                ");";
        }

        public static String WarehousesTable()
        {
            return 
                "CREATE TABLE IF NOT EXISTS `warehouses` (" +
                "ID int PRIMARY KEY AUTO_INCREMENT," +
                "SHORT varchar(4) NOT NULL UNIQUE," +
                "NAME varchar(20) NOT NULL UNIQUE" +
                ");";
        }

        public static String ProductsTable()
        {
            return 
                "CREATE TABLE IF NOT EXISTS `products` (" +
                "ID int PRIMARY KEY AUTO_INCREMENT," +
                "NAME varchar(255) UNIQUE NOT NULL," +
                "UNIT varchar(5) NOT NULL," +
                "PRICE_BUY double NOT NULL CHECK(PRICE_BUY >= 0)," +
                "PRICE_SELL double NOT NULL CHECK(PRICE_SELL >= 0)," +
                "TAX int DEFAULT 0 CHECK (TAX >=0)," +
                "VISIBLE bool" +
                ");";
        }

        public static String ComplexProductComponentsTable()
        {
            return 
                "CREATE TABLE IF NOT EXISTS `components` (" +
                "ID_COMPLEX int NOT NULL," +
                "ID_COMPONENT int NOT NULL," +
                "AMOUNT double NOT NULL," +
                "PRIMARY KEY (ID_COMPLEX, ID_COMPONENT)," +
                "FOREIGN KEY (ID_COMPLEX) REFERENCES `products`(ID) ON DELETE CASCADE," +
                "FOREIGN KEY (ID_COMPONENT) REFERENCES `products`(ID) ON DELETE CASCADE" +
                ");";
        }

        public static String WarehousesProducts()
        {
            return 
                "CREATE TABLE IF NOT EXISTS `warehouses_products` (" +
                "WAREHOUSE_ID int NOT NULL," +
                "PRODUCT_ID int NOT NULL," +
                "AMOUNT double CHECK (AMOUNT >= 0)," +
                "PRIMARY KEY(WAREHOUSE_ID, PRODUCT_ID)," +
                "FOREIGN KEY(WAREHOUSE_ID) REFERENCES `warehouses`(ID) ON DELETE CASCADE," +
                "FOREIGN KEY(PRODUCT_ID) REFERENCES `products`(ID) ON DELETE CASCADE" +
                ");";
        }

        public static String ContractorsTable()
        {
            return 
                "CREATE TABLE IF NOT EXISTS `contractors` (" +
                "ID int PRIMARY KEY AUTO_INCREMENT," +
                "NAME varchar(255) NOT NULL UNIQUE," +
                "STREET varchar(255)," +
                "CITY varchar(255)," +
                "POSTAL_CODE varchar(10)," +
                "TAX_CODE varchar(15) UNIQUE" +
                ");" +
                "INSERT IGNORE contractors (id, name, street, city, postal_code, tax_code) VALUES (-1, 'Operacja wewnętrzna', ' ', ' ', ' ', ' ');";
        }

        public static String OrdersTable()
        {
            return 
                "CREATE TABLE IF NOT EXISTS `orders`(" +
                "ID int PRIMARY KEY AUTO_INCREMENT," +
                "NUMBER varchar(30) NOT NULL UNIQUE," +
                "CONTRACTOR_ID int," +
                "WAREHOUSE_ID int NOT NULL," +
                "WAREHOUSE_MOVE_ID int," +
                "KIND varchar(5) NOT NULL," +
                "PURCHASE_SELL bool," +
                "DATE date NOT NULL," +
                "FOREIGN KEY(CONTRACTOR_ID) REFERENCES `contractors`(ID) ON DELETE CASCADE," +
                "FOREIGN KEY(WAREHOUSE_ID) REFERENCES `warehouses`(ID) ON DELETE CASCADE," +
                "FOREIGN KEY(WAREHOUSE_MOVE_ID) REFERENCES `warehouses`(ID) ON DELETE CASCADE" +
                ");";
        }

        public static String OrderDetailsTable()
        {
            return 
                "CREATE TABLE IF NOT EXISTS `order_details`(" +
                "ORDER_ID int NOT NULL," +
                "BUY_ORDER_ID int," +
                "PRODUCT_ID int NOT NULL," +
                "AMOUNT double NOT NULL," +
                "LEFTOVER double," +
                "PRICE double NOT NULL," +
                "FOREIGN KEY(ORDER_ID) REFERENCES `orders`(ID)ON DELETE CASCADE," +
                "FOREIGN KEY(PRODUCT_ID) REFERENCES `products`(ID)ON DELETE CASCADE" +
                ");";
        }

        public static String Attachments()
        {
            return
                "CREATE TABLE IF NOT EXISTS `attachements`(" +
                "ORDER_ID int NOT NULL," +
                "ATTACHED_ORDER_ID int NOT NULL," +
                "PRIMARY KEY (ORDER_ID, ATTACHED_ORDER_ID)," +
                "FOREIGN KEY (ORDER_ID) REFERENCES `orders` (ID) ON DELETE CASCADE," +
                "FOREIGN KEY (ATTACHED_ORDER_ID) REFERENCES `orders` (ID) ON DELETE CASCADE" +
                ");";
        }

        public static String CreateUpdateWarehousesProductTrigger()
        {
            return
            "DROP TRIGGER IF EXISTS updateWarehouses;" +
            "CREATE TRIGGER updateWarehouses AFTER INSERT ON order_details " +
            "FOR EACH ROW " +
            "BEGIN " +
            "  DECLARE war_id int; " +
            "  DECLARE doctype bool; " +
            "  SELECT orders.purchase_sell INTO doctype FROM orders INNER JOIN order_details ON order_details.order_id = orders.id WHERE order_details.order_id = new.order_id GROUP BY orders.purchase_sell; " +
            "  SELECT o.warehouse_id INTO war_id FROM orders o INNER JOIN order_details o_d ON o.id = o_d.ORDER_ID WHERE o_d.ORDER_ID = new.order_id GROUP BY o.warehouse_id; " +
            "  IF(doctype = '0') THEN " +
            "      IF(EXISTS(SELECT product_id FROM warehouses_products w_p WHERE w_p.PRODUCT_ID = NEW.product_id AND warehouse_id = war_id)) THEN " +
            "          UPDATE warehouses_products w_p SET w_p.amount = w_p.amount + new.amount WHERE w_p.product_id = new.product_id AND w_p.warehouse_id = war_id; " +
            "      ELSE " +
            "          INSERT INTO warehouses_products(warehouse_id, product_id, amount) VALUES (war_id, new.product_id, new.amount); " +
            "      END IF ;" +
            "  ELSEIF(doctype='1') THEN " +
            "      UPDATE warehouses_products w_p SET w_p.amount = w_p.amount - new.amount WHERE w_p.product_id = new.product_id AND w_p.warehouse_id = war_id; " +
            "  END IF; " +
            "END$$";
        }
    }
}
