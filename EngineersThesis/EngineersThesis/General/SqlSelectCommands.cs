﻿using System;
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

        public static String ShowOtherWarehouses(String database, String thisWarehouseName)
        {
            return $"SELECT id, short, name FROM warehouses WHERE name <> '{thisWarehouseName}' ORDER BY short;";
        }

        public static String ShowWarehouseNameToId(String database, String name)
        {
            return $"SELECT id FROM `{database}`.`warehouses` WHERE name = '{name}';";
        }

        public static String ShowProducts(String database)
        {
            return $"SELECT id, name, unit, tax, price_buy, price_sell FROM `{database}`.`products` ORDER BY name;";
        }

        public static String CheckIfProductIdIsInWarehouse(String warehouseId, String productId)
        {
            return $"SELECT product_id FROM warehouses_products WHERE warehouse_id = '{warehouseId}' AND product_id = '{productId}' GROUP BY product_id;";
        }

        public static String ShowProductsName(String database)
        {
            return $"SELECT id, name FROM `{database}`.`products` ORDER BY name;";
        }

        public static String ShowProductsWithFollowingEmpty(String database, String columnName)
        {
            return $"SELECT id, name, unit, tax, price_buy, price_sell, '' AS {columnName} FROM `{database}`.`products` ORDER BY name;";
        }

        public static String ShowProductsWithFollowingZero(String database)
        {
            return $"SELECT id, name, unit, tax, price_buy, price_sell, '0' AS amount FROM `{database}`.`products` ORDER BY name;";
        }

        public static String ShowOrdinaryProductsWithFollowingZero(String database)
        {
            return $"SELECT id, name, unit, tax, price_buy, price_sell, '0' AS amount FROM `{database}`.`products` WHERE NOT EXISTS(SELECT id_complex FROM components where id = id_complex) ORDER BY name;";
        }

        public static String ShowProductsWithFollowingZeroForID(String database, String id)
        {
            return $"SELECT id, name, unit, tax, price_buy, price_sell, '0' AS amount FROM `{database}`.`products` WHERE id = '{id}' ORDER BY name;";
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
                $"AND w.name = '{warehouseName}'" +
                $"WHERE amount > 0;";
        }

        public static String ShowContractors(String database)
        {
            return $"SELECT * FROM `{database}`.`contractors` WHERE id <> 0;";
        }

        public static String ShowOrders(String database, String warehouseId)
        {
            return
                $"SELECT DATE_FORMAT(o.date, '%Y-%m-%d') as date, o.number, o.kind, IF(c.name IS NOT NULL, c.name, w.short) as name, IF(o.purchase_sell, sum(p.PRICE_SELL) * amount, sum(p.PRICE_BUY) * amount) as orderValue " +
                $"FROM `orders` o " +
                $"INNER JOIN `order_details` o_d ON o.id = o_d.order_id " +
                $"LEFT JOIN `contractors` c ON o.contractor_id = c.id " +
                $"LEFT JOIN `warehouses` w ON w.id = o.WAREHOUSE_MOVE_ID " +
                $"INNER JOIN `products` p ON o_d.product_id = p.id " +
                $"WHERE o.warehouse_id = '{warehouseId}' " +
                $"GROUP BY o.number " +
                $"HAVING orderValue IS NOT NULL " +
                $"ORDER BY date DESC, o.number DESC;";
        }

        public static String ShowMyCompanyData(String database)
        {
            return $"SELECT * FROM `{database}`.`contractors` WHERE id = 0";
        }

        public static String ShowLastDocumentNumber(String database, String year)
        {
            return $"SELECT MAX(SUBSTRING_INDEX(number, '/', 1)) FROM `{database}`.`orders` " +
                $"WHERE YEAR(date) = YEAR('{year}');";
        }

        public static String ShowProductsInDocument(String orderNumber)
        {
            return $"SELECT p.name, p.unit, p.tax, p.price_buy, p.price_sell, details.amount FROM products p " +
                $"INNER JOIN order_details details ON product_id = p.id " +
                $"INNER JOIN orders ord ON details.order_id = ord.id " +
                $"WHERE ord.number = '{orderNumber}';";
        }

        public static String ShowDocumentHasContractor(String orderNumber)
        {
            return $"SELECT IF (contractor_id is not null, 'yes', 'no') FROM orders WHERE number = '{orderNumber}';";
        }

        public static String ShowContractorDataForOrderNumber(String orderNumber)
        {
            return $"SELECT name, street, city, postal_code, tax_code FROM contractors " +
                $"INNER JOIN orders ON contractors.id = orders.contractor_id WHERE orders.number = '{orderNumber}'";
        }

        public static String ShowWarehouseDataForOrderNumber(String orderNumber)
        {
            return $"SELECT short, name FROM warehouses " +
                $"INNER JOIN orders ON warehouses.id = orders.warehouse_move_id WHERE orders.number = '{orderNumber}'";
        }
    }
}
