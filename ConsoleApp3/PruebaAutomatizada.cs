using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using System;
using System.IO;
using System.Text;

class PruebaAutomatizada
{
    static StringBuilder htmlReport;
    static string reportPath;

    static void Main(string[] args)
    {
        
        InicializarReporte();

        
        var options = new EdgeOptions();
        IWebDriver driver = new EdgeDriver(options);

        try
        {
            
            AgregarPrueba("Verificar la carga de la pagina...");
            string url = "https://example.com";
            driver.Navigate().GoToUrl(url);
            driver.Manage().Window.Maximize();

            string pageTitle = driver.Title;
            if (pageTitle.Contains("Example Domain"))
            {
                AgregarResultado("PASSED", "La pagina se ha cargado correctamente!!");
            }
            else
            {
                AgregarResultado("FAILED", "La pagina no cargo como se esperaba...");
            }
            CapturarPantalla(driver, "Prueba_CargaPagina");

            
          
            AgregarPrueba("Verificar enlace");
            try
            {
                IWebElement enlace = driver.FindElement(By.XPath("//a[contains(text(), 'More information')]"));
                enlace.Click();
                System.Threading.Thread.Sleep(2000);

                string tituloActual = driver.Title; 
                Console.WriteLine($"El titulo actual de la pagina: {tituloActual}"); 

                if (tituloActual.Contains("Example Domains"))
                {
                    AgregarResultado("PASSED", "El enlace fue verificado correctamente!!");
                }
                else
                {
                    AgregarResultado("FAILED", $"Error al verificar el enlace...: Titulo incorrecto. Totulo actual: '{tituloActual}'");
                }
            }
            catch (NoSuchElementException ex)
            {
                AgregarResultado("FAILED", "Error al verificar el enlace...: " + ex.Message);
            }

            CapturarPantalla(driver, "Prueba_Enlace");

            
            Console.WriteLine("Las pruebas fueron completabas correctamente!");
        }
        catch (Exception ex)
        {
            AgregarResultado("FAILED", "Error durante la prueba: " + ex.Message);
        }
        finally
        {
            
            driver.Quit();

            FinalizarReporte();
        }
    }

    static void InicializarReporte()
    {
        reportPath = Path.Combine(Directory.GetCurrentDirectory(), "Reportes");
        Directory.CreateDirectory(reportPath);
        string reportFile = Path.Combine(reportPath, "ReporteDePrueba.html");

        htmlReport = new StringBuilder();
        htmlReport.Append("<!DOCTYPE html>");
        htmlReport.Append("<html lang='en'><head><meta charset='UTF-8'><title>Reporte de Pruebas Automatizadas</title>");
        htmlReport.Append("<style>body { font-family: Arial, sans-serif; margin: 20px; } table { width: 100%; border-collapse: collapse; } th, td { border: 1px solid #ddd; padding: 8px; text-align: left; } th { background-color: #f2f2f2; } .PASSED { color: green; } .FAILED { color: red; }</style>");
        htmlReport.Append("</head><body>");
        htmlReport.Append("<h1>Reporte de Pruebas Automatizadas</h1>");
        htmlReport.Append("<table><thead><tr><th>Prueba</th><th>Resultado</th><th>Detalles</th></tr></thead><tbody>");
    }

    static void AgregarPrueba(string nombrePrueba)
    {
        htmlReport.Append($"<tr><td colspan='3'><strong>{nombrePrueba}</strong></td></tr>");
    }

    static void AgregarResultado(string estado, string detalles)
    {
        htmlReport.Append($"<tr><td></td><td class='{estado}'>{estado}</td><td>{detalles}</td></tr>");
    }

    static void CapturarPantalla(IWebDriver driver, string nombreArchivo)
    {
        try
        {
            ITakesScreenshot screenshotDriver = driver as ITakesScreenshot;
            Screenshot screenshot = screenshotDriver.GetScreenshot();

            string capturaPath = Path.Combine(Directory.GetCurrentDirectory(), "Capturas");
            Directory.CreateDirectory(capturaPath);

            string filePath = Path.Combine(capturaPath, $"{nombreArchivo}.png");
            screenshot.SaveAsFile(filePath);

            
            string absolutePath = new Uri(filePath).AbsoluteUri;
            htmlReport.Append($"<tr><td colspan='3'><img src='{absolutePath}' alt='Captura de pantalla' style='max-width: 100%; height: auto;'></td></tr>");
            Console.WriteLine($"Captura de pantalla guardada en: {filePath}");
        }
        catch (Exception ex)
        {
            htmlReport.Append($"<tr><td colspan='3'>Error al capturar pantalla...: {ex.Message}</td></tr>");
        }
    }

    static void FinalizarReporte()
    {
        htmlReport.Append("</tbody></table>");
        htmlReport.Append("</body></html>");

        string reportFile = Path.Combine(reportPath, "ReporteDePrueba.html");
        File.WriteAllText(reportFile, htmlReport.ToString());

        Console.WriteLine($"Reporte generado aqui: {reportFile}");
    }
}
