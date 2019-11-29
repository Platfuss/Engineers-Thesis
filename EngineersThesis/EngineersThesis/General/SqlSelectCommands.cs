using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineersThesis.General
{
    class SqlSelectCommands
    {
        public static String ShowWarehouses(String database)
        {
            return $"SELECT short, name FROM `{database}`.`warehouses` ORDER BY short;";
        }

        public static String ShowProducts(String database)
        {
            return $"SELECT id, name, unit, tax, price_buy, price_sell FROM `{database}`.`products` ORDER BY name;";
        }

        public static String ShowProductsWithFollowingEmpty(String database, String columnName)
        {
            return $"SELECT id, name, unit, tax, price_buy, price_sell, '' AS {columnName} FROM `{database}`.`products` ORDER BY name;";
        }

        public static String ShowProductsWithFollowingZero(String database)
        {
            return $"SELECT id, name, unit, tax, price_buy, price_sell, '0' AS amount FROM `{database}`.`products` ORDER BY name;";
        }

        public static String ShowProductReversedComponents(String database, String complexProduct, String componentProduct)
        {
            return $"SELECT * FROM `{database}`.`components` WHERE ID_COMPLEX = '{componentProduct}' AND ID_COMPONENT = '{complexProduct}'";
        }

        public static String ShowComplexProductRecipe(String database, String id)
        {
            return $"SELECT id_component, amount FROM `{database}`.`components` WHERE id_complex = '{id}';";
        }

        public static String ShowComplexProductsID(String database)
        {
            return $"SELECT id_complex FROM `{database}`.`components` GROUP BY id_complex;";
        }

        public static String ShowLastInsertedID(String database, String table)
        {
            return $"SELECT MAX(id) FROM `{database}`.`{table}`";
        }

        public static String ShowProductsInWarehouse(String database, String warehouseName)
        {
            return $"SELECT p.id, p.name, unit, amount, price_buy, price_sell, tax " +
                $"FROM `{database}`.`warehouses` w " +
                $"INNER JOIN `{database}`.`warehouses_products` w_p ON w.id = w_p.warehouse_id " +
                $"INNER JOIN `{database}`.`products` p              ON p.ID = w_p.product_id " +
                $"AND w.name = '{warehouseName}';";
        }

        public static String ShowContractors(String database)
        {
            return $"SELECT * FROM `{database}`.`contractors` WHERE id <> 0;";
        }

        public static String ShowOrders(String database)
        {
            return "SELECT o.date, o.number, c.name, IF(o.purchase_sell, sum(p.PRICE_BUY) * amount, sum(p.PRICE_SELL) * amount) as orderValue " +
                "FROM orders o " +
                "INNER JOIN order_details o_d   ON o.id = o_d.order_id " +
                "INNER JOIN contractors c       ON o.contractor_id = c.id " +
                "INNER JOIN products p          ON o_d.product_id = p.id " +
                "HAVING orderValue IS NOT NULL " +
                "ORDER BY date;";
        }

        public static String ShowMyCompanyData(String database)
        {
            return $"SELECT * FROM `{database}`.`contractors` WHERE id = 0";
        }
    }
}
