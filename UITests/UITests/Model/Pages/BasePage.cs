using System;
using OpenQA.Selenium;
using UITests.WebDriverLib;

namespace UITests.Model.Pages
{
    public abstract class BasePage
    {        
        protected BasePage(IWebDriver driver, By selector = null, int pageLoadTimerInSeconds = -1)
        {
            WebDriver = driver;

            if (selector == null) return;

            try
            {
                WebDriver.FindElement(selector, pageLoadTimerInSeconds);
            }
            catch (Exception e)
            {
                throw new Exception( GetType() + " Page is not currectly loaded : locator is not found -" + selector, e);
            }
        }
                
        public IWebDriver WebDriver { get; private set; }
    }
}
