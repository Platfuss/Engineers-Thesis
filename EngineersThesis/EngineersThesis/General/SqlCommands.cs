using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineersThesis.General
{
    class SqlCommands
    { 
        public static String AllowDiactricMarksCommand(String database)
        {
            return $"ALTER DATABASE `{database}` DEFAULT CHARACTER SET utf8 COLLATE utf8_unicode_ci";
        }

        public static String IfDatabaseAlreadyExistCommand(String name)
        {
            return SqlConstants.showDatabases + $" like '{name}'";
        }

        public static String NewDatabaseCommand(String name)
        {
            return $"CREATE SCHEMA `{name}`;";
        }
    }
}
