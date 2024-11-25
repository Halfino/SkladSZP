using Sklad.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;

namespace Sklad
{
    class PdfCreator
    {
        public static void Initialize() { }

        public static void GeneratePdf()
        {
            var data = Database.GetDataForPrint();

            // Určení cesty pro uložení souboru
            string directoryPath = "Print";
            if (!Directory.Exists(directoryPath)) 
            {
                Directory.CreateDirectory(directoryPath);
            }
            string filePath = Path.Combine(directoryPath,"Sklad.pdf");
            // Inicializace dokumentu a zápis na disk
            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            {
                Document document = new Document();
                PdfWriter.GetInstance(document, fs);
                document.Open();

                // Font pro tučný text
                Font boldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);

                // Náhradní díly
                document.Add(new Paragraph("Náhradní díly\n\n", boldFont));
                document.Add(new Paragraph(""));
                document.Add(new Paragraph(""));
                document.Add(new Paragraph(""));
                PdfPTable sparePartsTable = new PdfPTable(6);
                sparePartsTable.AddCell("Název");
                sparePartsTable.AddCell("KČM");
                sparePartsTable.AddCell("Množství");
                sparePartsTable.AddCell("Umístění");
                sparePartsTable.AddCell("MU");
                sparePartsTable.AddCell("SN");

                foreach (var item in data.SpareParts)
                {
                    sparePartsTable.AddCell(item.Name);
                    sparePartsTable.AddCell(item.CatalogNumber);
                    sparePartsTable.AddCell(item.Quantity.ToString());
                    sparePartsTable.AddCell(item.Location);
                    sparePartsTable.AddCell(item.material_unit);
                    sparePartsTable.AddCell(item.serial_number);
                }
                document.Add(sparePartsTable);

                // Spotřební materiál
                document.Add(new Paragraph("Spotrební materiál\n\n", boldFont));

                PdfPTable consumablesTable = new PdfPTable(6);
                consumablesTable.AddCell("Název");
                consumablesTable.AddCell("KČM");
                consumablesTable.AddCell("Množství");
                consumablesTable.AddCell("Umístění");
                consumablesTable.AddCell("MU");
                consumablesTable.AddCell("SN");

                foreach (var item in data.Consumables)
                {
                    consumablesTable.AddCell(item.Name);
                    consumablesTable.AddCell(item.CatalogNumber);
                    consumablesTable.AddCell(item.Quantity.ToString());
                    consumablesTable.AddCell(item.Location);
                    consumablesTable.AddCell(item.material_unit);
                    consumablesTable.AddCell(item.serial_number);
                }
                document.Add(consumablesTable);

                document.Close();
            }
            Process.Start(new ProcessStartInfo
            {
                FileName = filePath,
                UseShellExecute = true
            });
        }
    }
}
