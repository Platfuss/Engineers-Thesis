using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using EngineersThesis.General;

namespace EngineersThesis
{
    /// <summary>
    /// Interaction logic for ProductEdition.xaml
    /// </summary>
    public partial class ProductEdition : Window
    {
        private SqlHandler sqlHandler;

        public ProductEdition(SqlHandler handler)
        {
            sqlHandler = handler;
            InitializeComponent();
        }
    }
}
