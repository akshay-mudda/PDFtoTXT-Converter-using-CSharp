using System;
using System.IO;
using System.Configuration;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

namespace PDFtoTXT_Converter_using_CSharp
{
    class Program
    {
        static void Main(string[] args)
        {
            // Read source and destination paths from app.config
            string sourcePath = ConfigurationManager.AppSettings["SourcePath"];
            string destinationPath = ConfigurationManager.AppSettings["DestinationPath"];

            // Check if destination folder exists, create if not
            if (!Directory.Exists(destinationPath))
            {
                Directory.CreateDirectory(destinationPath);
            }

            // Get all PDF files in the source folder
            string[] pdfFiles = Directory.GetFiles(sourcePath, "*.pdf");

            if (pdfFiles.Length == 0)
            {
                Console.WriteLine("No PDF files found in the source folder.");
                return;
            }

            foreach (var pdfFilePath in pdfFiles)
            {
                try
                {
                    // Get file name without extension
                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(pdfFilePath);
                    string txtFilePath = Path.Combine(destinationPath, fileNameWithoutExtension + ".txt");

                    // Check if the TXT already exists
                    if (File.Exists(txtFilePath))
                    {
                        Console.WriteLine($"TXT already exists for {fileNameWithoutExtension}. Skipping file.");
                        continue;
                    }

                    // Convert PDF file to TXT
                    ConvertPdfToTxt(pdfFilePath, txtFilePath);

                    // After conversion, delete the source PDF file
                    File.Delete(pdfFilePath);

                    Console.WriteLine($"Successfully converted {fileNameWithoutExtension}.pdf to TXT.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error converting {pdfFilePath}: {ex.Message}");
                }
            }
        }

        // Convert PDF file to TXT using UglyToad.PdfPig
        static void ConvertPdfToTxt(string pdfFilePath, string txtFilePath)
        {
            // Use PdfPig to read the PDF document
            using (var document = PdfDocument.Open(pdfFilePath))
            {
                using (StreamWriter writer = new StreamWriter(txtFilePath))
                {
                    foreach (Page page in document.GetPages())
                    {
                        // Write the text content of the PDF page to the TXT file
                        writer.WriteLine(page.Text.TrimEnd()); // Trim any trailing spaces
                    }
                }
            }
        }
    }
}