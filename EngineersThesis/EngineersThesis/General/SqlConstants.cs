using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineersThesis.General
{
    class SqlConstants
    {
        public static readonly String[] extraDatabases = { "information_schema", "mysql", "performance_schema" };
        public static readonly String showDatabases = "show databases";

        public static readonly Dictionary<String, String> translations = new Dictionary<String, String>()
        {
            {"id", "Identyfikator"},
            {"name", "Nazwa"},
            {"supplier", "Dostawca"},
            {"purchaser", "Klient"},
            {"phone_number", "Numer telefonu"},
            {"phone_owner", "Właściciel telefonu"},
            {"tax_code", "NIP"},
            {"address", "Adres"},
            {"date", "Data"},
            {"amount", "Ilość"},
            {"unit", "Jednostka"},
            {"short", "Skrót"},
            {"class", "Grupa"},
            {"price_buy", "Zakup netto"},
            {"price_sell", "Sprzedaż netto"},
            {"tax", "VAT"},
            {"complex", "Złożony"},
        };
    }
}
