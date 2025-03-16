
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Tests.WebTests
{
    public class SmartContractUITest : IDisposable
    {
        private readonly IWebDriver _driver;

        public SmartContractUITest()
        {
            var projectPath = Directory.GetCurrentDirectory();

            while (projectPath != null && !Directory.Exists(Path.Combine(projectPath, "chromedriver-win64")))
            {
                projectPath = Directory.GetParent(projectPath)?.FullName;
            }

            if (projectPath == null)
            {
                throw new DirectoryNotFoundException("Не удалось найти директорию с chromedriver-win64");
            }

            var browserPath = Path.Combine(projectPath, "chrome-win64", "chrome.exe");
            var driverPath = Path.Combine(projectPath, "chromedriver-win64");

            var options = new ChromeOptions();
            options.AddArgument("--headless"); // lauch without GUI
            options.BinaryLocation = browserPath;

            Console.WriteLine($"Используется ChromeDriver из: {driverPath}");

            _driver = new ChromeDriver(driverPath, options);
        }

        [Fact]
        public void GenerateContract_ShouldDisplayContractCode()
        {
            // Arrange
            _driver.Navigate().GoToUrl("https://localhost:7255");
            var inputField = _driver.FindElement(By.CssSelector("input[placeholder='Enter contract name']"));
            var generateButton = _driver.FindElement(By.CssSelector("button.custom-btn"));

            // Act
            var contractName = "TestContract";
            inputField.SendKeys(contractName);
            generateButton.Click();

            System.Threading.Thread.Sleep(3000);

            // Assert
            var contractOutput = _driver.FindElement(By.CssSelector(".code-container pre")).Text;
            Assert.Contains($"contract {contractName} is Ownable {{", contractOutput);
        }

        public void Dispose()
        {
            _driver.Quit();
        }
    }
}
