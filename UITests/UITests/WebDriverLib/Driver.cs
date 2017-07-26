using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using System;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using UITests.Properties;

namespace UITests.WebDriverLib
{
    public enum BrowserType
    {        
        Chrome,
        Firefox,        
        InternetExplorer
    }
    
    public class Driver
    {
        private readonly string _appName;

        public Driver()
        {
            IWebDriver driver;
            switch (Settings.Default.Browser)
            {
                case BrowserType.Firefox:
                    driver = new FirefoxDriver();
                    _appName = "firefox.exe";
                    break;
                case BrowserType.InternetExplorer:
                    var options = new InternetExplorerOptions();
                    driver = new InternetExplorerDriver(options);
                    _appName = "iexplorer.exe";
                    break;
                case BrowserType.Chrome:
                    var chromeoptions = new ChromeOptions();
                    chromeoptions.AddArgument("--auth-server-whitelist=*");
                    driver = new ChromeDriver(chromeoptions);
                    _appName = "chrome.exe";
                    break;
                default:
                    throw new Exception("unknown browser type");
            }

            WebDriver = driver;
        }

        public Driver(IWebDriver driver)
        {
            WebDriver = driver;
        }


        public IWebDriver WebDriver { get; set; }

        public void Init()
        {
            WebDriver.Url = Environment.GetUrl(Settings.Default.Environment);
            WebDriver.Manage().Timeouts().SetScriptTimeout(TimeSpan.FromSeconds(60));
            WebDriver.Manage().Window.Maximize();
        }

        public void Kill()
        {
            if (WebDriver == null)
            {
                return;
            }

            WebDriver.Quit();
            WebDriver = null;

            foreach (var process in Process.GetProcessesByName(_appName))
            {
                process.Kill();
            }
        }

        public void TakeScreenshot(string name)
        {
            if (WebDriver == null)
            {
                Console.WriteLine("TakeScreeenshot WebDriver is null");
                return;
            }

            try
            {
                var pageData = WebDriver.PageSource;
                var dateString = string.Format("{0:yyyyMMddHHmmss}", DateTime.Now);
                var fileName = string.Format("page_data_{0}.txt", dateString);
                File.WriteAllText(fileName, pageData);
                Console.WriteLine("saved page data: {0}", Path.Combine(Directory.GetCurrentDirectory(), fileName));
                var d = WebDriver as ITakesScreenshot;
                var ss = d.GetScreenshot();
                fileName = string.Format("screen_shot_{0}_{1}.jpg", dateString, name);
                ss.SaveAsFile(fileName, ImageFormat.Jpeg);
                Console.WriteLine("saved screenshot: {0}", Path.Combine(Directory.GetCurrentDirectory(), fileName));
            }
            catch (Exception e)
            {
                Console.WriteLine("TakeScreeenshot exception: {0}", e);
                throw;
            }
        }
    }
}
