using System;
using System.Drawing;
using System.IO;
using PdfiumViewer;
using ZXing;
using System.Text.RegularExpressions;

class Program
{
    static void Main()
    {
        string pastaPDFs = @"C:\Gustavo\LeituraDocs\Docs"; // Caminho da pasta com PDFs
        string[] arquivosPDF = Directory.GetFiles(pastaPDFs, "*.pdf");

        foreach (var pdfPath in arquivosPDF)
        {
            Console.WriteLine($"Processando: {Path.GetFileName(pdfPath)}");
            LerQRCodeDePDF(pdfPath);
        }
    }

    static void LerQRCodeDePDF(string pdfPath)
    {
        using (PdfDocument pdf = PdfDocument.Load(pdfPath))
        {
            for (int i = 0; i < pdf.PageCount; i++)  // Percorre as páginas
            {
                using (var imagem = pdf.Render(i, 300, 300, PdfRenderFlags.CorrectFromDpi))
                {
                    var leitor = new BarcodeReader();
                    var resultado = leitor.Decode((Bitmap)imagem);

                    if (resultado != null)
                    {
                        // Usando Split para dividir a string
                        string[] partes = resultado.Text.Split('*');  // Divide a string onde encontrar o '*'

                        // Exibindo as partes divididas no console
                        foreach (string parte in partes)
                        {
                            Console.WriteLine(parte);  // Imprime cada parte
                        }
                    }
                }
            }
        }
    }
}

