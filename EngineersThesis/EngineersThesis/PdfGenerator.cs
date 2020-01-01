using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using EngineersThesis.General;
using Microsoft.Win32;
using System.Text.RegularExpressions;

namespace EngineersThesis
{
    class PdfGenerator
    {
        SqlHandler sqlHandler;
        DataRowView givenRow;
        String warehouseName;

        public PdfGenerator(SqlHandler handler, DataRowView row, String _warehouseName)
        {
            sqlHandler = handler;
            givenRow = row;
            warehouseName = _warehouseName;
        }

        private Paragraph GetNewLine()
        {
            return new Paragraph(" ");
        }

        private PdfPTable FillPdfPTableRow(String firstCell, String secondCell, Font firstCellFont, Font secondCellFont)
        {
            var table = new PdfPTable(2)
            {
                WidthPercentage = 100
            };

            PdfPCell cellContent = new PdfPCell(new Phrase($"{firstCell}", firstCellFont))
            {
                Border = Rectangle.NO_BORDER
            };
            table.AddCell(cellContent);

            cellContent = new PdfPCell(new Phrase($"{secondCell}", secondCellFont))
            {
                Border = Rectangle.NO_BORDER
            };
            table.AddCell(cellContent);

            return table;
        }

        public void ExportToPdf()
        {
            var titleFont = FontFactory.GetFont(BaseFont.TIMES_ROMAN, BaseFont.CP1250, true, 18, Convert.ToInt32(Font.BOLD));
            var boldFont = FontFactory.GetFont(BaseFont.TIMES_ROMAN, BaseFont.CP1250, true, 12, Convert.ToInt32(Font.BOLD));
            var normalFont = FontFactory.GetFont(BaseFont.TIMES_ROMAN, BaseFont.CP1250, true, 12, Convert.ToInt32(Font.NORMAL));
            var italicFont = FontFactory.GetFont(BaseFont.TIMES_ROMAN, BaseFont.CP1250, true, 12, Convert.ToInt32(Font.ITALIC));

            var normalTableFont = FontFactory.GetFont(BaseFont.TIMES_ROMAN, BaseFont.CP1250, true, 10, Convert.ToInt32(Font.NORMAL));
            var boldTableFont = FontFactory.GetFont(BaseFont.TIMES_ROMAN, BaseFont.CP1250, true, 10, Convert.ToInt32(Font.BOLD));

            String date = givenRow[0].ToString();
            String number = givenRow[1].ToString();
            String kind = givenRow[2].ToString();

            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Filter = "PDF file (*.pdf)|*.pdf",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                FileName = $"{Regex.Replace(number, @"\\|/", "-")}-{kind}.pdf"
            };

            if (saveFileDialog.ShowDialog() == false)
                return;

            using (var document = new Document())
            {
                using (var writer = PdfWriter.GetInstance(document, new FileStream(saveFileDialog.FileName, FileMode.Create)))
                {
                    document.Open();
                    document.Add(GetNewLine());
                    document.Add(new Paragraph($"Dokument {kind} nr {number}", titleFont));
                    document.Add(GetNewLine());
                    document.Add(new Paragraph($"Magazyn: {warehouseName}", italicFont));
                    document.Add(new Paragraph($"Data wystawienia dokumentu: {date}", italicFont));
                    document.Add(GetNewLine());

                    var myCompanyData = sqlHandler.ExecuteCommand(SqlSelectCommands.ShowMyCompanyData(sqlHandler.Database)).Tables[0].DefaultView[0];
                    var companyName = myCompanyData["name"].ToString();
                    var companyStreet = myCompanyData["street"].ToString();
                    var companyCity = myCompanyData["city"].ToString();
                    var companyPostalCode = myCompanyData["postal_code"].ToString();
                    var companyTaxCode = myCompanyData["tax_code"].ToString();

                    DataRowView sqlExecutionResult;
                    bool isItMoveDocument = false;
                    var list = sqlHandler.DataSetToList(sqlHandler.ExecuteCommand(SqlSelectCommands.ShowDocumentHasContractor(givenRow[1].ToString())));
                    if (list[0][0] == "yes")
                    {
                        sqlExecutionResult = sqlHandler.ExecuteCommand(SqlSelectCommands.ShowContractorDataForOrderNumber(givenRow[1].ToString())).Tables[0].DefaultView[0];
                    }
                    else
                    {
                        isItMoveDocument = true;
                        sqlExecutionResult = sqlHandler.ExecuteCommand(SqlSelectCommands.ShowWarehouseDataForOrderNumber(givenRow[1].ToString())).Tables[0].DefaultView[0];
                    }

                    if (isItMoveDocument)
                    {
                        var warehouseShort = sqlExecutionResult["short"].ToString();
                        var warehouseName = sqlExecutionResult["name"].ToString();

                        var direction = kind[kind.Length - 1].ToString() == "+" ? "Z" : "Do";

                        document.Add(FillPdfPTableRow($" ", $"{direction} magazynu: ", boldFont, italicFont));
                        document.Add(FillPdfPTableRow($"{companyName}", $"{warehouseName}", boldFont, boldFont));
                        document.Add(FillPdfPTableRow($"{companyStreet}", $"{warehouseShort}", normalFont, normalFont));
                        document.Add(FillPdfPTableRow($"{companyPostalCode + " " + $"{companyCity}"}", $" ", normalFont, normalFont));
                        document.Add(FillPdfPTableRow($"NIP: {companyTaxCode}", $" ", normalFont, normalFont));
                    }
                    else
                    {
                        var contractorName = sqlExecutionResult["name"].ToString();
                        var contractorStreet = sqlExecutionResult["street"].ToString();
                        var contractorCity = sqlExecutionResult["city"].ToString();
                        var contractorPostalCode = sqlExecutionResult["postal_code"].ToString();
                        var contractorTaxCode = sqlExecutionResult["tax_code"].ToString();

                        if (contractorName == "Operacja wewnętrzna")
                        {
                            document.Add(FillPdfPTableRow($" ", $" ", boldFont, italicFont));
                            document.Add(FillPdfPTableRow($"{companyName}", $" ", boldFont, boldFont));
                        }
                        else
                        {
                            document.Add(FillPdfPTableRow($" ", $"Kontrahent: ", boldFont, italicFont));
                            document.Add(FillPdfPTableRow($"{companyName}", $"{contractorName}", boldFont, boldFont));
                        }
                        
                        document.Add(FillPdfPTableRow($"{companyStreet}", $"{contractorStreet}", normalFont, normalFont));
                        document.Add(FillPdfPTableRow($"{companyPostalCode + " " + $"{companyCity}"}", $"{contractorPostalCode}" + " " + $"{contractorCity}", normalFont, normalFont));
                        if (contractorName == "Operacja wewnętrzna")
                        {
                            document.Add(FillPdfPTableRow($"NIP: {companyTaxCode}", $"{contractorTaxCode}", normalFont, normalFont));
                        }
                        else
                        {
                            document.Add(FillPdfPTableRow($"NIP: {companyTaxCode}", $"NIP: {contractorTaxCode}", normalFont, normalFont));
                        }
                    }
                    document.Add(GetNewLine());
                    document.Add(GetNewLine());

                    DataTable sqlTable = sqlHandler.ExecuteCommand(SqlSelectCommands.ShowOrderFullInfo(number)).Tables[0];

                    int columnNumber = sqlTable.Columns.Count + 1;
                    var pdfTable = new PdfPTable(columnNumber)
                    {
                        WidthPercentage = 100
                    };
                    float[] widths = new float[] { 2, 6, 2, 3, 4, 4, 3, 4, 4 };
                    pdfTable.SetWidths(widths);

                    for (int i = 0; i < columnNumber; i++)
                    {
                        if (i == 0)
                        {
                            pdfTable.AddCell(new Phrase("Lp.", boldFont));
                            continue;
                        }
                        DataColumn column = sqlTable.Columns[i - 1];
                        if (SqlConstants.translations.TryGetValue(column.ColumnName, out String result))
                        {
                            pdfTable.AddCell(new Phrase(result, boldFont));
                        }
                        else
                        {
                            pdfTable.AddCell(new Phrase(column.ColumnName, boldFont));
                        }
                    }

                    for (int i = 0; i < sqlTable.Rows.Count; i++)
                    {
                        DataRow row = sqlTable.Rows[i];
                        for (int j = 0; j < columnNumber; j++)
                        {
                            if (j == 0)
                            {
                                pdfTable.AddCell(new Phrase((i + 1).ToString() + ".", normalTableFont));
                                continue;
                            }
                            pdfTable.AddCell(new Phrase(row[j - 1].ToString(), normalTableFont));
                        }
                    }
                    document.Add(pdfTable);

                    sqlTable = sqlHandler.ExecuteCommand(SqlSelectCommands.ShowOrderSumUp(number)).Tables[0];
                    pdfTable.FlushContent();

                    for (int i = 0; i < columnNumber; i++)
                    {
                        if (i < 5)
                        {
                            PdfPCell cellContent = new PdfPCell(new Phrase($" ", boldTableFont))
                            {
                                Border = Rectangle.NO_BORDER
                            };
                            pdfTable.AddCell(cellContent);
                            continue;
                        }
                        String header;
                        switch (i)
                        {
                            case 5:
                                header = "Netto";
                                break;
                            case 6:
                                header = "%";
                                break;
                            case 7:
                                header = "VAT";
                                break;
                            case 8:
                                header = "Brutto";
                                break;
                            default:
                                header = "?";
                                break;
                        }
                        pdfTable.AddCell(new Phrase(header, boldFont));
                    }

                    double netValue = 0;
                    double grossValue = 0;
                    double netAndGrossValue = 0;
                    for (int i = 0; i < sqlTable.Rows.Count; i++)
                    {
                        DataRow row = sqlTable.Rows[i];
                        for (int j = 0; j < columnNumber; j++)
                        {
                            if (j < 5)
                            {
                                PdfPCell cellContent = new PdfPCell(new Phrase($" ", normalTableFont))
                                {
                                    Border = Rectangle.NO_BORDER
                                };
                                pdfTable.AddCell(cellContent);
                                continue;
                            }
                            String toWrite = row[j - 5].ToString();

                            if (j == 5)
                            {
                                netValue += Convert.ToDouble(toWrite);
                            }
                            else if (j == 7)
                            {
                                grossValue += Convert.ToDouble(toWrite);
                            }
                            else if (j == 8)
                            {
                                double toAdd = Convert.ToDouble(row[0].ToString()) + Convert.ToDouble(row[2].ToString());
                                netAndGrossValue += toAdd;
                                toWrite = toAdd.ToString();
                            }
                            pdfTable.AddCell(new Phrase(toWrite, normalTableFont));
                        }
                    }

                    for (int i = 0; i < 5; i++)
                    {
                        PdfPCell cellContent = new PdfPCell(new Phrase($" ", normalTableFont))
                        {
                            Border = Rectangle.NO_BORDER
                        };
                        pdfTable.AddCell(cellContent);
                    }
                    pdfTable.AddCell(new Phrase($"{netValue}", boldTableFont));
                    pdfTable.AddCell(new Phrase($" ", boldTableFont));
                    pdfTable.AddCell(new Phrase($"{grossValue}", boldTableFont));
                    pdfTable.AddCell(new Phrase($"{netAndGrossValue}", boldTableFont));

                    document.Add(pdfTable);

                    document.Add(GetNewLine());
                    document.Add(new Phrase($"Wartość dokumentu: {netAndGrossValue.ToString()} zł", boldFont));
                    document.Add(GetNewLine());
                    document.Add(GetNewLine());

                    PdfPTable signingTable = new PdfPTable(2)
                    {
                        WidthPercentage = 100
                    };
                    String dots = "..............................................................................";
                    PdfPCell content = new PdfPCell(new Phrase($"{dots}", normalTableFont))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        Border = Rectangle.NO_BORDER
                    };
                    signingTable.AddCell(content);
                    signingTable.AddCell(content);

                    content = new PdfPCell(new Phrase($"Podpis osoby uprawnionej do wystawienia dokumentu", normalTableFont))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        Border = Rectangle.NO_BORDER
                    };
                    signingTable.AddCell(content);
                    content = new PdfPCell(new Phrase($"Podpis osoby uprawnionej do odbioru dokumentu", normalTableFont))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        Border = Rectangle.NO_BORDER
                    };
                    signingTable.AddCell(content);

                    document.Add(signingTable);
                    document.Close();
                    System.Diagnostics.Process.Start(saveFileDialog.FileName);
                }
            }
        }
        

    }
}
