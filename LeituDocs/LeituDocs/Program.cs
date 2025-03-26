using System;
using System.IO;
using System.Drawing;
using PdfiumViewer;
using ZXing;

class Program
{
    static void Main()
    {
        string pasta = @"C:\Gustavo\LeituraDocs\Docs"; // Substituir pelo caminho real
        ListarEProcessarPDFs(pasta);
    }

    static void ListarEProcessarPDFs(string pasta)
    {
        if (!Directory.Exists(pasta))
        {
            Console.WriteLine("A pasta não existe!");
            return;
        }

        string[] arquivos = Directory.GetFiles(pasta, "*.pdf");
        foreach (string arquivo in arquivos)
        {
            Console.WriteLine($"Processando: {arquivo}");
            LerQRCodeDePDF(arquivo);
        }
    }

    static void LerQRCodeDePDF(string caminhoPDF)
    {
        try
        {
            using (var pdfDoc = PdfDocument.Load(caminhoPDF))
            {
                for (int i = 0; i < pdfDoc.PageCount; i++)
                {
                    using (var imagem = pdfDoc.Render(i, 300, 300, true)) // Renderiza a página como imagem
                    {
                        Bitmap bitmap = new Bitmap(imagem);
                        string qrTexto = LerQRCode(bitmap);
                        if (!string.IsNullOrEmpty(qrTexto))
                        {
                            Console.WriteLine($"Página {i + 1}: QR Code encontrado -> {qrTexto}");
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao processar {caminhoPDF}: {ex.Message}");
        }
    }

    static string LerQRCode(Bitmap imagem)
    {
        try
        {
            BarcodeReader leitor = new BarcodeReader();
            var resultado = leitor.Decode(imagem);
            return resultado?.Text;
        }
        catch
        {
            return null;
        }
    }
}