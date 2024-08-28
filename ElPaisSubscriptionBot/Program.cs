using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace ElPaisSubscriptionBot
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IWebDriver driver = null;
            try
            {
                // Inicializamos el driver de Chrome y navegamos a la página principal.
                driver = InitializeWebDriver();

                // Aceptamos las cookies.
                if (AcceptCookies(driver))
                {
                    // Hacemos clic en el botón de suscripción.
                    ClickSubscribeButton(driver);
                }
                else
                {
                    Console.WriteLine("No se encontró el botón de aceptar cookies.");
                }
            }
            catch (WebDriverException ex)
            {
                Console.WriteLine("Error del WebDriver: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ha ocurrido un error inesperado: " + ex.Message);
            }
            finally
            {
                // Aseguramos de que el navegador se cierre correctamente
                driver?.Quit();

                // Mantener la consola abierta para leer los mensajes de error
                Console.WriteLine("Presione cualquier tecla para salir...");
                Console.ReadKey();
            }
        }

        private static IWebDriver InitializeWebDriver()
        {
            try
            {
                var chromeOptions = new ChromeOptions();

                // Verificar si estamos en un entorno Docker usando una variable de entorno
                var isRunningInDocker = Environment.GetEnvironmentVariable("RUNNING_IN_DOCKER");

                if (!string.IsNullOrEmpty(isRunningInDocker) && isRunningInDocker == "true")
                {
                    // Configuración para Docker
                    chromeOptions.AddArgument("--headless"); // Ejecutar en modo headless
                    chromeOptions.AddArgument("--no-sandbox"); // Desactivar el sandbox
                    chromeOptions.AddArgument("--disable-dev-shm-usage"); // Evitar problemas de uso compartido de memoria
                    chromeOptions.AddArgument("--disable-gpu"); // Desactivar la GPU
                    chromeOptions.AddArgument("--ignore-certificate-errors"); // Ignorar errores de certificados SSL
                    chromeOptions.AddArgument("--disable-web-security"); // Desactivar la seguridad web
                    chromeOptions.AddArgument("--allow-running-insecure-content"); // Permitir contenido inseguro
                }
                else
                {
                    // Configuración para un entorno con interfaz gráfica (local)
                    chromeOptions.AddArgument("--start-maximized"); // Iniciar maximizado
                }

                IWebDriver driver = new ChromeDriver(chromeOptions);
                driver.Navigate().GoToUrl("https://elpais.com");
                return driver;
            }
            catch (WebDriverException ex)
            {
                Console.WriteLine("Error al inicializar el WebDriver: " + ex.Message);
                throw; // Relanza la excepción para que sea capturada en el main.
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ocurrió un error inesperado al inicializar el WebDriver: " + ex.Message);
                throw; // Relanza la excepción para que sea capturada en el main.
            }
        }

        private static bool AcceptCookies(IWebDriver driver)
        {
            try
            {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                IWebElement acceptCookiesButton = wait.Until(d => d.FindElement(By.XPath("//a[contains(@onclick, 'acceptConsentWall()')]")));
                acceptCookiesButton.Click();
                return true;
            }
            catch (NoSuchElementException)
            {
                Console.WriteLine("No se encontró el botón de aceptar cookies.");
                return false;
            }
            catch (WebDriverTimeoutException)
            {
                Console.WriteLine("Se alcanzó el tiempo de espera al intentar encontrar el botón de aceptar cookies.");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ocurrió un error inesperado al aceptar las cookies: " + ex.Message);
                return false;
            }
        }

        private static void ClickSubscribeButton(IWebDriver driver)
        {
            try
            {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                IWebElement subscribeButton = wait.Until(d => d.FindElement(By.CssSelector("#s_b_df"))); // Asegúrate de que el selector sea correcto.
                subscribeButton.Click();
            }
            catch (NoSuchElementException ex)
            {
                Console.WriteLine("No se encontró el botón de suscripción: " + ex.Message);
            }
            catch (WebDriverTimeoutException ex)
            {
                Console.WriteLine("Se alcanzó el tiempo de espera al intentar hacer clic en el botón de suscripción: " + ex.Message);
            }
            catch (InvalidSelectorException ex)
            {
                Console.WriteLine("El selector CSS proporcionado es inválido: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ocurrió un error inesperado al hacer clic en el botón de suscripción: " + ex.Message);
            }
        }
    }
}
