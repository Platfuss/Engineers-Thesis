﻿using System;
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
using System.Text.RegularExpressions;
using EngineersThesis.General;

namespace EngineersThesis
{
    /// <summary>
    /// Interaction logic for ProductEdition.xaml
    /// </summary>
    public partial class ProductEditor : Window
    {
        private SqlHandler sqlHandler;

        public ProductEditor(SqlHandler handler)
        {
            InitializeComponent();
            sqlHandler = handler;
            editProductFrame.Content = new ProductEditorControl();
            newProductFrame.Content = new ProductEditorControl();
        }

        private void SetDataGrid()
        {
            var database = sqlHandler.Database;
            if (database != null)
            {
                var dataSet = sqlHandler.ExecuteCommand(SqlSelectCommands.ShowProducts(sqlHandler.Database));
                dataGrid.ItemsSource = dataSet.Tables[0].DefaultView;
                foreach (var column in dataGrid.Columns)
                {
                    column.MinWidth = 100;
                    if (SqlConstants.translations.TryGetValue(column.Header.ToString(), out String result))
                        column.Header = result;
                }
            }
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            SetDataGrid();
            //if (editMode)
            //{
            //    upperLeftTextBox.Text = oldName;
            //    centerLeftTextBox.Text = price;
            //    for (int i = 0; i < unitComboBox.Items.Count; i++)
            //    {
            //        if (Regex.Replace(unitComboBox.Items[i].ToString(), "System.Windows.Controls.ComboBoxItem: ", "") == unit)
            //        {
            //            unitComboBox.SelectedIndex = i;
            //            break;
            //        }
            //    }

            //    for (int i = 0; i < taxComboBox.Items.Count; i++)
            //    {
            //        if (Regex.Replace(unitComboBox.Items[i].ToString(), "System.Windows.Controls.ComboBoxItem: ", "") == tax + "%")
            //        {
            //            taxComboBox.SelectedIndex = i;
            //            break;
            //        }
            //    }
            //}
        }

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OnNetValidation(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            e.Handled = !Regex.IsMatch(textBox.Text + e.Text, @"^\d*\,?\d{0,3}$");
        }

        private void OnTaxChanged(object sender, SelectionChangedEventArgs e)
        {
            SetGrossTextBox();
        }

        private void NormalProductChecked(object sender, RoutedEventArgs e)
        {
            //complexProductRadioButton.IsChecked = false;
            //tabComponent.IsEnabled = false;
        }

        private void ComplexProductChecked(object sender, RoutedEventArgs e)
        {
            //normalProductRadioButton.IsChecked = false;
            //tabComponent.IsEnabled = true;
        }

        private void OnNetTextChanged(object sender, TextChangedEventArgs e)
        {
            SetGrossTextBox();
        }

        private void SetGrossTextBox()
        {
            //if (centerLeftTextBox.Text != "" && taxComboBox.SelectedItem != null)
            //{
            //    var taxComboBoxText = Regex.Replace(taxComboBox.SelectedItem.ToString(), "[^0-9]", "");
            //    var percent = Convert.ToDouble(taxComboBoxText) / 100;
            //    lowerRightTextBox.Text = Convert.ToString(Convert.ToDouble(centerLeftTextBox.Text) * (1.0 + percent));
            //}
        }

        private void OnAcceptClick(object sender, RoutedEventArgs e)
        {
            //if (upperLeftTextBox.Text != "" && centerLeftTextBox.Text != "" && unitComboBox.SelectedIndex != -1)
            //{
            //    var taxComboBoxText = Regex.Replace(taxComboBox.SelectedItem.ToString(), "[^0-9]", "");
            //    var unitText = unitComboBox.Text;
            //    var price = Regex.Replace(centerLeftTextBox.Text, @"\.", ",");

            //    if (editMode)
            //    {
            //        sqlHandler.ExecuteCommand(SqlUpdateCommands.UpdateProduct(sqlHandler.Database, oldName, upperLeftTextBox.Text, unitText, price, taxComboBoxText));
            //    }
            //    else
            //    {
            //        sqlHandler.ExecuteCommand(SqlInsertCommands.InsertNewProduct(sqlHandler.Database, upperLeftTextBox.Text, unitText, price, taxComboBoxText));
            //    }
            //    Close();
            //}
            //else
            //{
            //    MessageBox.Show("Nie wszystkie pola zostały wypełnione!");
            //}
        }

        private void OnDataGridSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
