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

        public static String InsertNewProduct(String database, String name, String unit, String priceBuy,String priceSell, String tax)
        {
            return $"INSERT INTO `{database}`.`products` (NAME, UNIT, PRICE_BUY, PRICE_SELL, TAX) VALUES ('{name}', '{unit}', '{priceBuy}', '{priceSell}', '{tax}');";
        }

        public static String InsertComponents(String database, String idComplex, List<Tuple<String, String>> list)
        {
            String command = $"INSERT INTO `{database}`.`components` (ID_COMPLEX, ID_COMPONENT, AMOUNT) VALUES ";
            foreach (var tuple in list)
            {
                command += $"({idComplex}, {tuple.Item1}, {tuple.Item2}),";
            }
            return command.Remove(command.Length - 1) + ";";
        }
        
    }
}
