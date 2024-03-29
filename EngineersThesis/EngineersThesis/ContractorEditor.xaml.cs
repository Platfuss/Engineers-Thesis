﻿using System;
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
using System.Text.RegularExpressions;

namespace EngineersThesis
{
    /// <summary>
    /// Interaction logic for ContractorEditor.xaml
    /// </summary>
    public partial class ContractorEditor : Window
    {
        private SqlHandler sqlHandler;
        private DataRowView givenRow;
        private bool editMode = false;
        private bool forCompany;

        public ContractorEditor(SqlHandler handler, bool _forCompany = false)
        {
            InitializeComponent();
            sqlHandler = handler;
            forCompany = _forCompany;
        }

        public ContractorEditor(SqlHandler handler, DataRowView row)
        {
            InitializeComponent();
            sqlHandler = handler;
            givenRow = row;
            editMode = true;
            nameTextBox.Text = row[1].ToString();
            streetTextBox.Text = row[2].ToString();
            cityTextBox.Text = row[3].ToString();
            postalCodeTextBox.Text = row[4].ToString();
            taxCodeTextBox.Text = row[5].ToString();
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (cityTextBox.Text != "" && nameTextBox.Text != "" && postalCodeTextBox.Text != "" && streetTextBox.Text != "" && taxCodeTextBox.Text != "")
            {
                if (editMode)
                {
                    if (nameTextBox.Text != givenRow[1].ToString() || streetTextBox.Text != givenRow[2].ToString() || cityTextBox.Text != givenRow[3].ToString() ||
                        postalCodeTextBox.Text != givenRow[4].ToString() || taxCodeTextBox.Text != givenRow[5].ToString())
                    {
                        acceptButton.IsEnabled = true;
                    }
                    else
                    {
                        acceptButton.IsEnabled = false;
                    }
                }
                else
                {
                    acceptButton.IsEnabled = true;
                }
            }
            else
            {
                acceptButton.IsEnabled = false;
            }
        }

        private void OnAcceptClick(object sender, RoutedEventArgs e)
        {
            bool result = false;

            if (!Regex.IsMatch(postalCodeTextBox.Text, @"^\d{2}-\d{3}$"))
            {
                MessageBox.Show("Niepoprawny kod pocztowy", "Błąd", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            if (CheckTaxCodeValidation(Regex.Replace(taxCodeTextBox.Text, @"[^0-9]*", "")))
            {
                if (editMode)
                {
                    result = sqlHandler.ExecuteNonQuery(SqlUpdateCommands.UpdateContractor(sqlHandler.Database, givenRow[0].ToString(), nameTextBox.Text, streetTextBox.Text, cityTextBox.Text,
                        postalCodeTextBox.Text, taxCodeTextBox.Text));
                }
                else
                {
                    if (forCompany == false)
                    {
                        result = sqlHandler.ExecuteNonQuery(SqlInsertCommands.InsertContractor(sqlHandler.Database, nameTextBox.Text, streetTextBox.Text, cityTextBox.Text,
                            postalCodeTextBox.Text, taxCodeTextBox.Text));
                    }
                    else
                    {
                        result = sqlHandler.ExecuteNonQuery(SqlInsertCommands.InsertMyCompany(sqlHandler.Database, nameTextBox.Text, streetTextBox.Text, cityTextBox.Text,
                            postalCodeTextBox.Text, taxCodeTextBox.Text));
                    }
                }
            }
            else
            {
                MessageBox.Show("Niepoprawny NIP", "Błąd", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }

            if (result == true)
            {
                Close();
            }
        }

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private bool CheckTaxCodeValidation(String taxCode)
        {
            bool result = false;
            if (taxCode.Length == 10)
            {
                List<int> weight = new List<int> { 6, 5, 7, 2, 3, 4, 5, 6, 7 };
                int sum = 0;
                for (int i = 0; i < weight.Count; i++)
                {
                    sum += weight[i] * (int)Char.GetNumericValue(taxCode[i]);
                }

                if (sum % 11 == (int)Char.GetNumericValue(taxCode[9]))
                {
                    result = true;
                }
            }
            return result;
        }
    }
}
