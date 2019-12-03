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
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Markup;

namespace EngineersThesis
{
    /// <summary>
    /// Interaction logic for Statistics.xaml
    /// </summary>
    public partial class Statistics : Window
    {
        SqlHandler sqlHandler;

        String yearStart, monthStart, yearStop, monthStop;

        public Statistics(SqlHandler handler)
        {
            InitializeComponent();
            sqlHandler = handler;
        }

        private void OnCalendarModeChanged(object sender, CalendarModeChangedEventArgs e)
        {
            var calendar = sender as Calendar;
            if (calendar.DisplayMode == CalendarMode.Month)
            {
                calendar.DisplayMode = CalendarMode.Year;
            }
        }

        private void OnCalendarDisplayDateChanged(object sender, CalendarDateChangedEventArgs e)
        {
            var calendar = sender as Calendar;
            var date = (DateTime)e.AddedDate;
            if (calendar.Name == "calendarSince")
            {
                yearStart = date.Year.ToString();
                monthStart = date.Month.ToString();
            }
            else
            {
                yearStop = date.Year.ToString();
                monthStop = date.Month.ToString();
            }
            Mouse.Capture(null);
        }

        private void OnGenerateStatisticsClick(object sender, RoutedEventArgs e)
        {
            if (comboBox.SelectedIndex == 0)
            {
                columnGrid.Visibility = Visibility.Visible;
                pieGrid.Visibility = Visibility.Hidden;

                columnChart.Series.Clear();
                var sqlTable = sqlHandler.ExecuteCommand(SqlSelectCommands.ShowCompanyMoneyChange(yearStart, monthStart, yearStop, monthStop, "WZ")).Tables[0];
                var valueList = new List<KeyValuePair<string, int>>();

                foreach (DataRow row in sqlTable.Rows)
                {
                    valueList.Add(new KeyValuePair<string, int>($"{row[1]}" + " " + $"{row[0]}", Convert.ToInt32(row[2])));
                }

                ColumnSeries columnSeries = new ColumnSeries()
                {
                    DependentValuePath = "Value",
                    IndependentValuePath = "Key",
                    ItemsSource = valueList,
                    Title = "Przychody",
                };
                columnChart.Series.Add(columnSeries);

                sqlTable = sqlHandler.ExecuteCommand(SqlSelectCommands.ShowCompanyMoneyChange(yearStart, monthStart, yearStop, monthStop, "PZ")).Tables[0];
                valueList = new List<KeyValuePair<string, int>>();

                foreach (DataRow row in sqlTable.Rows)
                {
                    valueList.Add(new KeyValuePair<string, int>($"{row[1]}" + " " + $"{row[0]}", Convert.ToInt32(row[2])));
                }

                columnSeries = new ColumnSeries()
                {
                    DependentValuePath = "Value",
                    IndependentValuePath = "Key",
                    ItemsSource = valueList,
                    Title = "Rozchody",
                };
                columnChart.Series.Add(columnSeries);
            }
            else
            {
                columnGrid.Visibility = Visibility.Hidden;
                pieGrid.Visibility = Visibility.Visible;
                pieChart.Series.Clear();

                var sqlTable = sqlHandler.ExecuteCommand(SqlSelectCommands.ShowPopularProducts(yearStart, monthStart, yearStop, monthStop)).Tables[0];
                var valueList = new List<KeyValuePair<string, int>>();

                foreach (DataRow row in sqlTable.Rows)
                {
                    valueList.Add(new KeyValuePair<string, int>($"{row[0]}", Convert.ToInt32(row[1])));
                }

                PieSeries pieSeries = new PieSeries()
                {
                    DependentValuePath = "Value",
                    IndependentValuePath = "Key",
                    ItemsSource = valueList,
                    Title = "Popularność",
                };
                pieChart.Series.Add(pieSeries);
            }
        }
    }
}
