using System;
using System.IO;
using Tesseract;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using ZXing;
using System.Drawing;

class Program
{
    static void Main(string[] args)
    {
        string folderPath = @"C:\Gustavo\LeituraDocs\Docs";

        if (!Directory.Exists(folderPath))
        {
            Console.WriteLine("A pasta especificada não existe. Verifique o caminho.");
            return;
        }

        string[] files = Directory.GetFiles(folderPath);

        if (files.Length == 0)
        {
            Console.WriteLine("Nenhum ficheiro encontrado na pasta.");
            return;
        }


        Environment.SetEnvironmentVariable("TESSDATA_PREFIX", @"C:\Gustavo\LeituraDocs\Docs");

        foreach (var filePath in files)
        {
            string fileExtension = Path.GetExtension(filePath).ToLower();

            Console.WriteLine($"Processando ficheiro: {filePath}");

            if (fileExtension == ".pdf")
            {
                ProcessPdf(filePath);
            }
            else if (fileExtension == ".jpg" || fileExtension == ".png")
            {
                ProcessImage(filePath);
            }
            else
            {
                Console.WriteLine($"Tipo de ficheiro não suportado: {filePath}");
            }
        }
    }

    static void ProcessPdf(string filePath)
    {
        try
        {
            if (IsImagePdf(filePath))
            {
                Console.WriteLine("PDF contém imagens. Iniciando OCR...");
                ExtractTextFromImagePdf(filePath);
            }
            else
            {
                Console.WriteLine("PDF estruturado. Extraindo texto...");
                ExtractTextFromStructuredPdf(filePath);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao processar o PDF: {ex.Message}");
        }
    }

    static bool IsImagePdf(string filePath)
    {
        try
        {
            using (PdfReader reader = new PdfReader(filePath))
            using (PdfDocument pdfDoc = new PdfDocument(reader))
            {
                return pdfDoc.GetFirstPage().GetContentStreamCount() == 0;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao verificar o tipo de PDF: {ex.Message}");
            return false;
        }
    }

    static void ExtractTextFromStructuredPdf(string filePath)
    {
        try
        {
            using (PdfReader reader = new PdfReader(filePath))
            using (PdfDocument pdfDoc = new PdfDocument(reader))
            {
                for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
                {
                    try
                    {

                        Console.WriteLine("\n=================================================");
                        Console.WriteLine($"              Página {i} do Documento");
                        Console.WriteLine("=================================================\n");

                        string text = PdfTextExtractor.GetTextFromPage(pdfDoc.GetPage(i));
                        Console.WriteLine(text);


                        Console.WriteLine("\n-------------------------------------------------\n");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao extrair texto da página {i}: {ex.Message}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao processar o PDF: {ex.Message}");
        }
    }

    static void ExtractTextFromImagePdf(string filePath)
    {
        try
        {
            Console.WriteLine("Iniciando OCR em PDF baseado em imagem...");
            using (var engine = new TesseractEngine(@"C:\Gustavo\LeituraDocs\Docs", "por", EngineMode.Default))
            {
                using (var image = Pix.LoadFromFile(filePath))
                {
                    using (var page = engine.Process(image))
                    {
                        Console.WriteLine(page.GetText());
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao realizar OCR no PDF com imagem: {ex.Message}");
        }
    }

    static void ProcessImage(string filePath)
    {
        try
        {
            Console.WriteLine("Ficheiro de imagem detectado. Verificando código QR...");

            if (TryDecodeQRCode(filePath))
            {
                return;
            }


            Console.WriteLine("Iniciando OCR na imagem...");
            using (var engine = new TesseractEngine(@"C:\Gustavo\LeituraDocs\Docs", "por", EngineMode.Default))
            {
                using (var image = Pix.LoadFromFile(filePath))
                {
                    using (var page = engine.Process(image))
                    {
                        Console.WriteLine(page.GetText());
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao processar imagem: {ex.Message}");
        }
    }

    static bool TryDecodeQRCode(string filePath)
    {
        try
        {
            Bitmap bitmap = new Bitmap(filePath);
            BarcodeReader reader = new BarcodeReader();
            var result = reader.Decode(bitmap);

            if (result != null)
            {
                Console.WriteLine($"Código QR detectado: {result.Text}");
                return true;
            }
            else
            {
                Console.WriteLine("Nenhum código QR detectado na imagem.");
                return false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao processar código QR: {ex.Message}");
            return false;
        }
    }
}
