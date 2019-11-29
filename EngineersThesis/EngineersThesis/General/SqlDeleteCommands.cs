using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineersThesis.General
{
    class SqlDeleteCommands
    {
        public static String DeleteFromWarehouses(String database, String shortcut, String name)
        {
            return $"DELETE FROM `{database}`.`warehouses` WHERE short = '{shortcut}' AND name = '{name}';";
        }

        public static String DeleteComplexity(String database, String complexId)
        {
            return $"DELETE FROM `{database}`.`components` WHERE id_complex = '{complexId}';";
        }

        public static String DeleteProduct(String database, String id)
        {
            return $"DELETE FROM `{database}`.`products` WHERE id = '{id}';";
        }

        public static String DeleteContractor(String database, String id)
        {
            return $"DELETE FROM `{database}`.`contractors` WHERE ID = '{id}';";
        }
    }
}
