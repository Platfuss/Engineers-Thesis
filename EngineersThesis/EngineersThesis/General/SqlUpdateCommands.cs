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

        public static String UpdateProduct(String database, String oldName, String name, String unit, String price, String tax)
        {
            return $"UPDATE `{database}`.`products` SET NAME = '{name}', UNIT = '{unit}', PRICE = '{price}', TAX = '{tax}' WHERE NAME = '{oldName}' AND ID > '-1';";
        }
    }
}
