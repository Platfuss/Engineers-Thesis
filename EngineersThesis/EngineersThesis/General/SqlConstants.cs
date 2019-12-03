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
            {"id", "ID"},
            {"name", "Nazwa"},
            {"street", "Ulica"},
            {"city", "Miejscowość" },
            {"postal_code", "Kod pocztowy" },
            {"date", "Data"},
            {"amount", "Ilość"},
            {"unit", "j.m."},
            {"short", "Skrót"},
            {"price_buy", "Zakup netto"},
            {"price_sell", "Sprzedaż netto"},
            {"tax", "VAT"},
            {"tax_code", "NIP" },
            {"complex", "Złożony"},
            {"number", "Numer" },
            {"ordervalue", "Wartość dokumentu" },
            {"kind", "Rodzaj dokumentu" },
            {"net_price", "Cena netto" },
            {"net_worth", "Wartość netto" },
            {"gross", "Kwota VAT" },
            {"net_and_gross", "Wartość brutto" },
        };
    }
}
