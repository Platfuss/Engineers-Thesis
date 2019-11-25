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
            return $"DELETE FROM `{database}`.`warehouses` WHERE SHORT = '{shortcut}' AND NAME = '{name}';";
        }
    }
}
