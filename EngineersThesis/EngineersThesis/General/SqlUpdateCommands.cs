using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineersThesis.General
{
    class SqlUpdateCommands
    {
        public static String UpdateWarehouse(String database, String oldShortcut, String oldName, String newShortcut, String newName)
        {
            return $"UPDATE `{database}`.`warehouses` SET SHORT = '{newShortcut}', NAME = '{newName}' WHERE SHORT = '{oldShortcut}' AND NAME = '{oldName}';";
        }

        public static String UpdateProductInfo(String database, String id, String name, String unit, String priceBuy, String priceSell, String tax)
        {
            return $"UPDATE `{database}`.`products` SET NAME = '{name}', UNIT = '{unit}', PRICE_BUY = '{priceBuy}', PRICE_SELL = '{priceSell}', TAX = '{tax}' WHERE ID = '{id}';";
        }

        public static String UpdateContractor(String database, String id,  String name, String street, String city, String postalCode, String taxCode)
        {
            return $"UPDATE `{database}`.`contractors` SET name = '{name}', street = '{street}', city = '{city}', postal_code = '{postalCode}', " +
                $"tax_code = '{taxCode}' WHERE id = '{id}';";
        }

        public static String UpdateProductToWarehouse(String warehouseID, String productId, String amount)
        {
            return $"UPDATE `warehouses_products` SET amount = amount + '{amount}' WHERE warehouse_id = '{warehouseID}' AND product_id = '{productId}';";
        }
    }
}
