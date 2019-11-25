using System;
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

        public static String InsertNewProduct(String database, String name, String unit, String price, String tax)
        {
            return $"INSERT INTO `{database}`.`products` (NAME, UNIT, PRICE, TAX) VALUES ('{name}', '{unit}', '{price}', '{tax}');";
        }
    }
}
