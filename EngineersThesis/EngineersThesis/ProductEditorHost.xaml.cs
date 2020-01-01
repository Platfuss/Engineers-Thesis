using System;
using System.Data;
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
    /// Interaction logic for ProductEditorHost.xaml
    /// </summary>
    public partial class ProductEditorHost : Window
    {
        SqlHandler sqlHandler;

        public ProductEditorHost(SqlHandler _sqlHandler, Action setGrid, bool enabled, bool _isComplex)
        {
            InitializeComponent();
            sqlHandler = _sqlHandler;
            frame.Content = new ProductEditorControl(sqlHandler, setGrid, enabled, _isComplex);
        }

        public ProductEditorHost(SqlHandler _sqlHandler, Action setGrid, DataRowView data)
        {
            InitializeComponent();
            sqlHandler = _sqlHandler;
            frame.Content = new ProductEditorControl(sqlHandler, setGrid, data);
        }

        private void OnExitButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
