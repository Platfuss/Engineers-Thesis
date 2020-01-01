﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineersThesis.General
{
    class SqlInsertCommands
    {
        public static String InsertNewWarehouse(String database, String shortcut, String name)
        {
            return $"INSERT INTO `{database}`.`warehouses` (SHORT, NAME) VALUES ('{shortcut}', '{name}');";
        }

        public static String InsertNewProduct(String database, String name, String unit, String priceBuy,String priceSell, String tax)
        {
            return $"INSERT INTO `{database}`.`products` (NAME, UNIT, PRICE_BUY, PRICE_SELL, TAX) VALUES ('{name}', '{unit}', '{priceBuy}', '{priceSell}', '{tax}');";
        }

        public static String InsertProductToWarehouse(String warehouseID, String productId, String amount)
        {
            return $"INSERT INTO `warehouses_products` (warehouse_id, product_id, amount) VALUES ('{warehouseID}', '{productId}', '{amount}')";
        }

        public static String InsertComponents(String database, String idComplex, List<Tuple<String, String>> list)
        {
            String command = $"INSERT INTO `{database}`.`components` (ID_COMPLEX, ID_COMPONENT, AMOUNT) VALUES ";
            foreach (var tuple in list)
            {
                command += $"('{idComplex}', '{tuple.Item1}', '{tuple.Item2}'),";
            }
            return command.Remove(command.Length - 1) + ";";
        }

        public static String InsertContractor(String database, String name, String street, String city, String postalCode, String taxCode)
        {
            return $"INSERT INTO `{database}`.`contractors` (name, street, city, postal_code, tax_code) VALUES ('{name}', '{street}', '{city}', '{postalCode}', '{taxCode}');";
        }

        public static String InsertMyCompany(String database, String name, String street, String city, String postalCode, String taxCode)
        {
            return $"INSERT INTO `{database}`.`contractors` (id, name, street, city, postal_code, tax_code) VALUES (-2, '{name}', '{street}', '{city}', '{postalCode}', '{taxCode}');";
        }

        public static String InsertOrder(String database, String number, String contractor_id, String warehouse_id, String kind, String purchase_sell, String date)
        {
            String pur_sell = purchase_sell == "NULL" ? "NULL" : $"'{purchase_sell}'";
            return $"INSERT INTO `{database}`.`orders` (number, contractor_id, warehouse_id, kind, purchase_sell, date) VALUES" +
                $"('{number}', '{contractor_id}', '{warehouse_id}', '{kind}', {pur_sell}, '{date}')";
        }

        public static String InsertOrderForMM(String database, String number, String warehouse_id, String warehouse_move_id, String kind, String purcharse_sell, String date)
        {
            return $"INSERT INTO `{database}`.`orders` (number, warehouse_id, warehouse_move_id, kind, purchase_sell, date) VALUES" +
                $"('{number}', '{warehouse_id}', '{warehouse_move_id}', '{kind}', '{purcharse_sell}', '{date}')";
        }

        public static String InsertOrderDetails (String database, String order_id, String product_id, String amount, String price = "")
        {
            String result;
            if (price.Length > 0)
            {
                result = $"INSERT INTO `{database}`.`order_details` (order_id, product_id, amount, leftover, price) VALUES ('{order_id}', '{product_id}', '{amount}', '{amount}', '{price}');";
            }
            else
            {
                result = $"INSERT INTO `{database}`.`order_details` (order_id, product_id, amount) VALUES ('{order_id}', '{product_id}', '{amount}');";
            }
            return result;
        }

        public static String InsertDetailsForOutOrders(String order_id, String buyOrder, String productId, String amount, String price)
        {
            return
                $"INSERT INTO order_details (order_id, buy_order_id, product_id, amount, price) VALUES ('{order_id}', '{buyOrder}', '{productId}', '{amount}', '{price}')";
        }

        public static String InsertSettings(String id, String setting)
        {
            return $"INSERT IGNORE settings (ID, VALUE) VALUES ('{id}', {setting})";
        }

        public static String InsertAttachement(String orderId, String attachedOrderId)
        {
            return $"INSERT INTO attachments (order_id, attached_order_id) VAlUES ('{orderId}', '{attachedOrderId}')";
        }
    }
}
