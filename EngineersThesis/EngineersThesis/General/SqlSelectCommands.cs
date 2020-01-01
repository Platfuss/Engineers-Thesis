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

        public static String ShowWarehousesForGivenField(String field, String fieldValue)
        {
            return $"SELECT id, short, name FROM warehouses WHERE {field} = '{fieldValue}';";
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

        public static String ShowProductInUse(String productId)
        {
            return 
                $"SELECT orders.number " +
                $"FROM orders " +
                $"INNER JOIN order_details ON order_details.order_id = orders.id " +
                $"INNER JOIN products ON order_details.product_id = products.id " +
                $"WHERE product_id = {productId};";
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
            return $"SELECT id, name, '0' AS amount FROM `{database}`.`products` WHERE NOT EXISTS(SELECT id_complex FROM components where id = id_complex) ORDER BY name;";
        }

        public static String ShowProductsWithFollowingZeroForID(String database, String id, bool mode)
        {
            String result;
            if (mode == false)
            {
                result = $"SELECT id, name, unit, price_buy, '0' AS amount FROM `{database}`.`products` WHERE id = '{id}' ORDER BY name;";
            }
            else
            {
                result = $"SELECT id, name, unit, '0' AS amount FROM `{database}`.`products` WHERE id = '{id}' ORDER BY name;";
            }
            return result;
        }

        public static String ShowProductReversedComponents(String database, String complexProduct, String componentProduct)
        {
            return $"SELECT * FROM `{database}`.`components` WHERE ID_COMPLEX = '{componentProduct}' AND ID_COMPONENT = '{complexProduct}'";
        }

        public static String ShowComplexProductRecipe(String database, String id)
        {
            return $"SELECT id_component, amount FROM `{database}`.`components` WHERE id_complex = '{id}';";
        }

        public static String ShowComplexProductRecipeWithName(String productId)
        {
            return
                $"SELECT id_component, products.name, amount FROM components INNER JOIN products ON products.id = components.id_component WHERE id_complex = '{productId}';";
        }

        public static String ShowComplexProductsID(String database)
        {
            return $"SELECT id_complex FROM `{database}`.`components` GROUP BY id_complex;";
        }

        public static String ShowComplexProductsName()
        {
            return $"SELECT id_complex, products.name FROM components INNER JOIN products ON products.id = components.id_complex GROUP BY id_complex;";
        }

        public static String ShowLastInsertedID(String database, String table)
        {
            return $"SELECT MAX(id) FROM `{database}`.`{table}`";
        }

        public static String ShowProductsInWarehouse(String database, String warehouseName)
        {
            return $"SELECT p.id, p.name, unit, amount, tax " +
                $"FROM `{database}`.`warehouses` w " +
                $"INNER JOIN `{database}`.`warehouses_products` w_p ON w.id = w_p.warehouse_id " +
                $"INNER JOIN `{database}`.`products` p              ON p.ID = w_p.product_id " +
                $"AND w.name = '{warehouseName}' " +
                $"WHERE amount > 0;";
        }

        public static String ShowProductAmountInWarehouse(String warehouseID, String productId)
        {
            return
                $"INSERT IGNORE warehouses_products (warehouse_id, product_id, amount) VALUES ('{warehouseID}', '{productId}', 0);" +
                $"SELECT warehouses_products.amount FROM warehouses_products WHERE warehouse_id = '{warehouseID}' AND product_id = '{productId}';";
        }

        public static String ShowContractors(String database)
        {
            return $"SELECT * FROM `{database}`.`contractors` WHERE id > 0;";
        }

        public static String ShowDocumentWithContractor(String contractorId)
        {
            return $"SELECT orders.number FROM orders INNER JOIN contractors ON orders.contractor_id = contractors.id WHERE orders.contractor_id = {contractorId};";
        }

        public static String ShowOrders(String database, String warehouseId)
        {
            return
                $"SELECT DATE_FORMAT(o.date, '%Y-%m-%d') as date, o.number, o.kind, IF(c.name IS NOT NULL, c.name, w.short) as name " +
                $"FROM `orders` o " +
                $"INNER JOIN `order_details` o_d ON o.id = o_d.order_id " +
                $"LEFT JOIN `contractors` c ON o.contractor_id = c.id " +
                $"LEFT JOIN `warehouses` w ON w.id = o.WAREHOUSE_MOVE_ID " +
                $"INNER JOIN `products` p ON o_d.product_id = p.id " +
                $"WHERE o.warehouse_id = '{warehouseId}' " +
                $"GROUP BY o.number " +
                $"ORDER BY date DESC, o.number DESC; ";
        }

        public static String ShowMyCompanyData(String database)
        {
            return $"SELECT * FROM `{database}`.`contractors` WHERE id = -2";
        }

        public static String ShowLastDocumentNumber(String date, String warehouseId, String documentType, String mode)
        {
            String result = $"SELECT MAX(CAST(SUBSTRING_INDEX(number, '/', 1) AS SIGNED)) FROM orders ";
            if (mode == "0")
            {
                result += $"WHERE MONTH('{date}') = MONTH(date) AND YEAR('{date}') = YEAR(date) ";
            }
            else
            {
                result += $"WHERE YEAR('{date}') = YEAR(date) ";
            }
            result += $"AND warehouse_id = '{warehouseId}' AND kind = '{documentType}';";

            return result;
        }

        public static String ShowProductsInDocument(String orderNumber)
        {
            return 
                $"SELECT p.name, p.unit, p.tax, details.price, details.amount FROM products p " +
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

        public static String ShowOrderFullInfo(String orderNumber)
        {
            return $"SELECT p.name, p.unit, or_det.amount, @net_price:= or_det.price AS net_price, @net_sum:=(@net_price * or_det.amount) AS net_worth, " +
                $"p.tax, @gross:=(@net_sum * p.tax / 100) AS gross,  (@gross + @net_sum) AS net_and_gross " +
                $"FROM products p " +
                $"INNER JOIN order_details or_det ON or_det.PRODUCT_ID = p.id " +
                $"INNER JOIN orders ON or_det.ORDER_ID = orders.id " +
                $"WHERE orders.number = '{orderNumber}' " +
                $"ORDER BY p.name;";
        }

        public static String ShowOrderSumUp(String orderNumber)
        {
            return
                $"SELECT @first:=SUM(x.net_price) AS net, x.tax AS percent_sign, @second:=SUM((x.net_price * x.tax / 100)) AS tax, (@first + @second) as result " +
                $"FROM  " +
                $"( " +
                $"  SELECT or_det.price * or_det.amount AS net_price, p.tax AS tax" +
                $"  FROM orders INNER JOIN order_details or_det ON or_det.order_id = orders.id " +
                $"  INNER JOIN products p ON or_det.product_id = p.id " +
                $"  WHERE orders.number = '{orderNumber}'" +
                $") x " +
                $"GROUP BY x.tax; ";
        }

        public static String ShowCompanyMoneyChange(String yearStart, String monthStart, String yearStop, String monthStop, String documentKind)
        {
            String priceToUse = documentKind == "WZ" ? "price_sell" : "price_buy";
            return
                $"SELECT YEAR(orders.date) AS 'year', MONTH(orders.date) AS 'month', SUM(IF(orders.kind = '{documentKind}', products.{priceToUse} * ord_det.amount, 0)) AS outcome " +
                $"FROM orders " +
                $"INNER JOIN order_details ord_det ON orders.id = ord_det.order_id " +
                $"INNER JOIN products ON ord_det.product_id = products.id " +
                $"WHERE DATE(orders.date) >= '{yearStart}-{monthStart}-01' AND DATE(orders.date) <= LAST_DAY('{yearStop}-{monthStop}-01') " +
                $"GROUP BY YEAR(orders.date), MONTH(orders.date);";
        }

        public static String ShowPopularProducts(String yearStart, String monthStart, String yearStop, String monthStop)
        {
            return 
                $"SELECT products.name, COUNT(products.name) FROM products " +
                $"INNER JOIN order_details ON products.id = order_details.product_id " +
                $"INNER JOIN orders ON order_details.order_id = orders.id " +
                $"WHERE DATE(orders.date) >= '{yearStart}-{monthStart}-01' AND DATE(orders.date) <= LAST_DAY('{yearStop}-{monthStop}-01')  " +
                $"GROUP BY products.name;";
        }

        public static String GetSetting(String id)
        {
            return $"SELECT value FROM settings WHERE id = '{id}'";
        }

        public static String GetProductFromOrders(String productId, String warehouseId, String fifoOrLifo)
        {
            String order = fifoOrLifo == "0" ? "asc" : "desc";
            return
                $"SELECT order_id, leftover, price " +
                $"FROM order_details " +
                $"INNER JOIN orders ON order_id = orders.id " +
                $"INNER JOIN warehouses ON warehouse_id = warehouses.id " +
                $"INNER JOIN products ON products.id = product_id " +
                $"WHERE product_id = '{productId}' AND warehouse_id = '{warehouseId}' AND leftover > 0 " +
                $"ORDER BY orders.date {order}, orders.id {order};";
        }

        public static String ShowOrdersContainingProduct(String productId, List<String> documentTypes)
        {
            String types = "";
            foreach (var type in documentTypes)
            {
                types += $"'{type}', ";
            }
            types = types.Remove(types.Length - 2);

            return
                $"SELECT  DATE_FORMAT(date, '%Y-%m-%d') as date, number, contractors.name " +
                $"FROM orders " +
                $"INNER JOIN order_details ON orders.id = order_id " +
                $"INNER JOIN contractors ON contractor_id = contractors.id " +
                $"INNER JOIN products ON product_id = products.id " +
                $"INNER JOIN warehouses ON warehouse_id = warehouses.id " +
                $"WHERE product_id = '{productId}' AND kind IN ({types}) ";
        }

        public static String ShowOrdersContainingProduct(String productId, List<String> documentTypes, String warehouseId)
        {
            String start = ShowOrdersContainingProduct(productId, documentTypes);
            return start += $"AND warehouse_id = '{warehouseId}' ";
        }

        public static String PrepareStockTaking(String date, String warehouseId)
        {
            return
                $"SELECT products.id, products.name, products.unit, SUM(IF(purchase_sell = '0', order_details.amount, 0)) - SUM(IF(purchase_sell = '1', order_details.amount, 0)) AS before_change, " +
                $"  '0' AS after_change, '0' AS price " +
                $"FROM products   LEFT JOIN order_details ON products.id = product_id " +
                $"LEFT JOIN orders ON orders.id = order_id " +
                $"LEFT JOIN warehouses ON warehouse_id = warehouses.id " +
                $"WHERE orders.date <= '{date}' AND warehouse_id = '{warehouseId}'" +
                $"HAVING products.id IS NOT NULL;";

        }

        public static String GetProductsNotUsedInWarehouse(String warehouseId)
        {
            return
                $"SELECT p.id, p.name, p.unit, '0' AS before_change, '0' AS after_change, '0' AS price " +
                $"FROM products p " +
                $"LEFT JOIN warehouses_products ON warehouses_products.product_id = p.id " +
                $"WHERE amount <= 0 AND warehouse_id = '{warehouseId}' OR NOT EXISTS(SELECT NULL FROM warehouses_products WHERE warehouses_products.product_id = p.id);";
        }

        public static String GetLastProductsBuyPrice(String date, String warehouseId)
        {
            return
                $"SELECT products.id, order_details.price " +
                $"FROM order_details " +
                $"INNER JOIN products ON products.id = product_id " +
                $"INNER JOIN orders ON order_id = orders.id " +
                $"WHERE purchase_sell = '0' AND date <= '{date}' AND warehouse_id = '{warehouseId}' " +
                $"GROUP BY products.id " + 
                $"ORDER BY date DESC, orders.id DESC ";
        }

        public static String ShowProductFuture(String date, String warehouseId, String productId)
        {
            return
                $"SELECT amount, purchase_sell " +
                $"FROM order_details " +
                $"INNER JOIN orders ON order_id = orders.id " +
                $"INNER JOIN warehouses ON warehouse_id = warehouses.id " +
                $"WHERE orders.date > '{date}' AND product_id = '{productId}' AND warehouse_id = '{warehouseId}' AND purchase_sell IN ('0', '1') " +
                $"ORDER BY date ASC, orders.id ASC";
        }

        public static String ShowStockTakings(String warehouseId)
        {
            return
                $"SELECT id, number, DATE_FORMAT(date, '%Y-%m-%d') as date FROM orders WHERE kind = 'INW' AND warehouse_id = '{warehouseId}';";
        }

        public static String ShowStockTakingMainDocumentDetails(String orderNumber)
        {
            return
                $"SELECT number, DATE_FORMAT(date, '%Y-%m-%d') as date FROM orders WHERE number = '{orderNumber}';";
        }

        public static String CheckIfStockTakingWasMade(String warehouseId, String date)
        {
            return
                $"SELECT COUNT(orders.id) FROM orders WHERE kind = 'INW' AND date >= '{date}' AND warehouse_id = '{warehouseId}';";
        }

        public static String GetProductPriceAndShouldBeFromStockTaking(String stockTakingNumber)
        {
            return
                $"SELECT products.id, IF(orders2.purchase_sell = '0', '0', '1'), order_details.amount, order_details.price " +
                $"FROM orders " +
                $"INNER JOIN attachements ON orders.id = attachements.order_id " +
                $"INNER JOIN order_details ON attachements.attached_order_id = order_details.order_id " +
                $"INNER JOIN orders orders2 ON orders2.id = attachements.attached_order_id " +
                $"INNER JOIN products ON product_id = products.id " +
                $"WHERE orders.number = '{stockTakingNumber}';";
        }

        public static String AreThereOlderDocuments(String date)
        {
            return
                $"SELECT COUNT(orders.id) FROM orders WHERE date > '{date}';";
        }

        public static String ShowComplexDocuments(String warehouseId)
        {
            return
                $"SELECT DATE_FORMAT(orders.date, '%Y-%m-%d') AS date, products.name, IF (orders.kind = 'kom', 'Skompletowano', 'Rozłożono') AS action, " +
                $"  IF (orders.kind = 'KOM', IF (ord2.kind = 'PW', order_details.amount, NULL), IF(ord2.kind = 'RW', order_details.amount, NULL)) AS amount " +
                $"FROM orders " +
                $"INNER JOIN attachements ON orders.id = attachements.order_id " +
                $"INNER JOIN orders ord2 ON ord2.id = attachements.attached_order_id " +
                $"INNER JOIN order_details ON order_details.order_id = ord2.id " +
                $"INNER JOIN products ON products.id = order_details.product_id " +
                $"WHERE orders.warehouse_id = '{warehouseId}' AND orders.kind IN('kom', 'dek') " +
                $"HAVING amount IS NOT NULL; ";
        }
    }
}
