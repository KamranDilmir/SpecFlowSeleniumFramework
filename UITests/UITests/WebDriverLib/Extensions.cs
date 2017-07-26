using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Support.UI;
using FluentAssertions;

namespace UITests.WebDriverLib
{
    
    public static class Extensions
    {
    
        private static int defaultTimeoutInSeconds = 60;

        private static bool shouldHighLight = false;
                
        public static bool ShouldHighLight
        {
            get
            {
                return shouldHighLight;
            }

            set
            {
                shouldHighLight = value;
            }
        }

                
        public static int TimeOutInSeconds
        {
            get { return defaultTimeoutInSeconds; }
            set { defaultTimeoutInSeconds = value; }
        }
        
                
        public static IWebElement FindElement(this IWebDriver caller, By by, int timeoutInSeconds = -1)
        {
            try
            {
                caller.WaitForPageLoad();
                var elem = caller.Wait(ExpectedConditions.ElementExists(by), timeoutInSeconds);
                caller.Highlight(elem);
                return elem;
            }
            catch (WebDriverTimeoutException e)
            {                
                throw new WebDriverTimeoutException("Can't find web element:" + by + " on page "
                                                     + " {title=" + caller.Title + ", url=" + caller.Url + "}"
                                                      , e);
            }
        }
                
        public static IWebElement GetElement(this IWebDriver caller, By by, int timeoutInSeconds = -1)
        {
            return caller.FindElement(by, timeoutInSeconds);
        }

        
        public static IWebElement GetElement(this IWebElement caller, By by, int timeoutInSeconds = -1)
        {
            IWebDriver driver = caller.GetWebDriver();

            try
            {
                var elem = driver.Wait(d => caller.FindElement(by), timeoutInSeconds);
                driver.Highlight(elem);
                return elem;
            }
            catch (WebDriverTimeoutException e)
            {
                // generate better messsages for error debug
                throw new WebDriverTimeoutException(
                    "Can't find web element:" + by + " within " + caller
                    + " on page " + " {title=" + driver.Title + ", url=" + driver.Url + "}",
                    e);
            }
        }

                
        public static IWebElement FindElementOrNull(this IWebDriver driver, By by, int timerInSeconds = 2)
        {
            try
            {
                return driver.GetElement(by, timerInSeconds);

            }
            catch
            {
                return null;
            }
        }

        
        public static IWebElement FindElementOrNull(this IWebElement element, By by, int timerInSeconds = 2)
        {
            try
            {
                return element.GetElement(by, timerInSeconds);
            }
            catch
            {
                return null;
            }
        }

        
        public static IReadOnlyCollection<IWebElement> GetElements(this IWebDriver driver, By by, int timeoutInSeconds = -1)
        {
            try
            {
                driver.WaitForPageLoad();
                return driver.Wait(
                    d =>
                    {
                        var result = d.FindElements(by).Where(e => e.Displayed).ToList();
                        return result.Count > 0 ? result : null;
                    },
                    timeoutInSeconds);
            }
            catch
            {
                return new Collection<IWebElement>();
            }
        }

        
        public static IReadOnlyCollection<IWebElement> GetElements(this IWebElement element, By by, bool onlyDisplayed = false, int timeoutInSeconds = -1)
        {
            try
            {
                IWebDriver driver = element.GetWebDriver();
                driver.WaitForPageLoad();

                return driver.Wait(
                d =>
                {
                    var result = element.FindElements(by).Where(e => !onlyDisplayed || e.Displayed).ToList();
                    return result.Count > 0 ? result : null;
                },
                    timeoutInSeconds);
            }
            catch
            {
                return new Collection<IWebElement>();
            }

        }
        
        public static void Highlight(this IWebDriver driver, IWebElement element)
        {
            if (!ShouldHighLight) return;

            try
            {
                var jsDriver = (IJavaScriptExecutor)driver.GetWebDriver();
                string highlightJavascript =
                    @"$(arguments[0]).css({ ""border-width"" : ""2px"", ""border-style"" : ""solid"", ""border-color"" : ""red"" });";
                jsDriver.ExecuteScript(highlightJavascript, element);
            }
            catch (Exception)
            {
               
            }
        }

        public static IWebDriver ClickIt(this IWebDriver caller, IWebElement elem, int timeoutInSeconds = -1)
        {
            if (timeoutInSeconds < 0) timeoutInSeconds = TimeOutInSeconds;

            IWebDriver driver = caller;

            caller.Highlight(elem);

            try
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
                wait.IgnoreExceptionTypes(typeof(InvalidOperationException), typeof(ElementNotVisibleException));

                wait.Until(d =>
                {                    
                    if (elem.Enabled)
                    {
                        elem.Click();
                        return true;
                    }

                    return false;
                });
            }
            catch (WebDriverTimeoutException e)
            {
                throw new WebDriverTimeoutException("Timeout waiting for element to be clickable:" + elem.Text, e);
            }

            return caller;
        }
                
        public static IWebDriver ClickIt(this IWebDriver caller, By elem, int timeoutInSeconds = -1)
        {
            try
            {
                return caller.ClickIt(caller.GetElement(elem, timeoutInSeconds), timeoutInSeconds);
            }
            catch (StaleElementReferenceException)
            {
                caller.WaitForPageLoad();
                return caller.ClickIt(caller.GetElement(elem, timeoutInSeconds), timeoutInSeconds);
            }
        }
                
        public static IWebDriver ClickIt(this IWebElement caller, int timeoutInSeconds = -1)
        {
            return caller.GetWebDriver().ClickIt(caller, timeoutInSeconds);
        }

        
        public static IWebDriver GetWebDriver(this IWebElement element)
        {
            var e = element;
                    
            while (e is IWrapsElement && !(e is IWrapsDriver))
            {
                e = ((IWrapsElement)e).WrappedElement;
            }

            return ((IWrapsDriver)e).WrappedDriver;
        }
                
        public static IWebDriver GetWebDriver(this IWebDriver driver)
        {
            var d = driver;
            while (d is IWrapsDriver)
            {
                d = ((IWrapsDriver)d).WrappedDriver;
            }

            return d;
        }

        public static IWebElement SetText(this IWebElement element, string value, int timeoutInSeconds = -1)
        {

            switch (element.TagName.ToLower())
            {
                case "textarea":
                case "input":
                    element.Clear();
                    element.SendKeys(value);
                    break;
                case "select":
                                        
                    try
                    {
                        Wait(
                           element.GetWebDriver(),
                            e =>
                            {
                                try
                                {
                                    new SelectElement(element).SelectByText(value);
                                    return true;
                                }
                                catch (NoSuchElementException)
                                {
                                    return false;
                                }
                            },
                            timeoutInSeconds);
                    }
                    catch (WebDriverTimeoutException e)
                    {
                        throw new WebDriverTimeoutException("Can't find " + value + " to select", e);
                    }

                    break;
                default:
                    throw new ArgumentException("SetValue for " + element.TagName + " is currently not supported");
            }

            return element;
        }


        public static string GetText(this IWebElement element)
        {
            if (element != null)
            {
                switch (element.TagName.ToLower())
                {
                    case "input":
                        return element.GetAttribute("value");
                    case "select":
                        return new SelectElement(element).SelectedOption.Text;
                    default:
                        return element.Text;
                }
            }
            return null;

        }
             
        public static bool ContainsText(this IWebDriver driver, string txt)
        {
            driver.WaitForPageLoad();
            return driver.FindElement(By.TagName("body"), 1).Text.Contains(txt);
        }
        
     
        public static T Wait<T>(this T obj, int timerInSeconds = 30)
        {
            if (timerInSeconds < 0) timerInSeconds = 0;

            Thread.Sleep(timerInSeconds * 1000);

            return obj;
        }

       
        public static T Wait<T>(this IWebDriver driver, Func<IWebDriver, T> cond, int timeoutInSeconds)
        {
            return driver.Wait(cond, null, timeoutInSeconds);
        }

       
        public static T Wait<T>(this IWebDriver driver, Func<IWebDriver, T> cond, int timeoutInSeconds, int pollInSeconds)
        {
            return driver.Wait(cond, null, timeoutInSeconds, pollInSeconds);
        }

        
        public static T Wait<T>(this IWebDriver driver, Func<IWebDriver, T> cond, string message = null, int timeoutInSeconds = -1, int pollInSeconds = -1)
        {
            if (timeoutInSeconds <= -1) timeoutInSeconds = TimeOutInSeconds;

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
            if (pollInSeconds > 0) wait.PollingInterval = TimeSpan.FromSeconds(pollInSeconds);
            if (!string.IsNullOrEmpty(message)) wait.Message = message;

            return wait.Until(cond);
        }

        public const string WaitForAngularPageLoad = @"
                    try {
                      if (!window.angular) {                        
                        return true;
                      }                    
                      if (window.angular) {
                        if (!window.qa) {
                          // Used to track the render cycle finish after loading is complete
                          window.qa = {
                            doneRendering: false
                          };
                        }
                        // Get the angular injector for this app
                        var injector = window.angular.element(document.body).injector();
                        // Store providers to use for these checks
                        var $rootScope = injector.get('$rootScope');
                        var $http = injector.get('$http');
                        var $timeout = injector.get('$timeout');
                        // Check if digest
                        if ($rootScope.$$phase === '$apply' || $rootScope.$$phase === '$digest' || $http.pendingRequests.length !== 0) {
                          window.qa.doneRendering = false;
                          return false; // Angular digesting or loading data
                        }
                        if (!window.qa.doneRendering) {
                          // Set timeout to mark angular rendering as finished
                          $timeout(function() {
                            window.qa.doneRendering = true;
                          }, 0);
                          return false;
                        }
                      }
                      return true;
                    } catch (ex) {
                      console.log(ex.message);
                      return true; // done for staff page
                    }";

        public static void WaitForPageLoad(this IWebDriver driver, int maxWaitTimeInSeconds = -1)
        {
            if (maxWaitTimeInSeconds < 0)
            {
                maxWaitTimeInSeconds = TimeOutInSeconds;
            }

            string state = string.Empty;
            try
            {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(maxWaitTimeInSeconds));

                wait.Until<bool>(d => { return (bool)((IJavaScriptExecutor)d.GetWebDriver()).ExecuteScript(WaitForAngularPageLoad); });
                   
                wait.Until(
                    d =>
                    {
                        try
                        {
                            state =
                                ((IJavaScriptExecutor)driver.GetWebDriver()).ExecuteScript(@"return document.readyState")
                                    .ToString();
                        }
                        catch (InvalidOperationException)
                        {
                            // Ignore
                        }
                        catch (NoSuchWindowException)
                        {
                            // when popup is closed, switch to last windows
                            // driver.SwitchTo().Window(driver.WindowHandles.Last());
                        }

                        // In IE7 there are chances we may get state as loaded instead of complete
                        return state.Equals("complete", StringComparison.InvariantCultureIgnoreCase)
                               || state.Equals("loaded", StringComparison.InvariantCultureIgnoreCase);
                    });
            }
            catch (TimeoutException)
            {
                // sometimes Page remains in Interactive mode and never becomes Complete, then we can still try to access the controls
                if (!state.Equals("interactive", StringComparison.InvariantCultureIgnoreCase))
                {
                    throw;
                }
            }
            catch (NullReferenceException)
            {
                // sometimes Page remains in Interactive mode and never becomes Complete, then we can still try to access the controls
                if (!state.Equals("interactive", StringComparison.InvariantCultureIgnoreCase))
                {
                    throw;
                }
            }
            catch (WebDriverException)
            {              
                if (
                    !(state.Equals("complete", StringComparison.InvariantCultureIgnoreCase)
                      || state.Equals("loaded", StringComparison.InvariantCultureIgnoreCase)))
                {
                    throw;
                }
            }
        }

        #region additonal
               
        public static void Select(this IWebElement el)
        {
            if (el.Selected) return;
            el.Click();
        }
              

        public static void UnSelect(this IWebElement el)
        {
            if (!el.Selected) return;
            el.Click();
        }
        
        
        public static void Refresh(this IWebDriver driver)
        {
            driver.Navigate().Refresh();
        }

        #endregion
                
        public static void AcceptAlertIfPopup(this IWebDriver driver)
        {
            try
            {                
                driver.Wait(ExpectedConditions.AlertIsPresent(), 5);
                driver.SwitchTo().Alert().Accept();
                driver.SwitchTo().Alert().Dismiss();
            }
            catch
            {
                // ignore 
            }
        }
             
        public static void Hover(this IWebDriver driver, IWebElement el, int seconds = 3)
        {
            var action = new Actions(driver);

            action.MoveToElement(el).Build().Perform();
        }
                
        public static void IfNotNull<T>(this T obj, Action<T> act)
        {
            if (obj == null) return;

            act.Invoke(obj);
        }

        
        public static void SelectOption(this IWebDriver driver, string Id)
        {
            driver.FindElement(By.Id(Id)).ClickIt();
        }

      
        public static string SwitchToNewWindow(this IWebDriver driver, string titleSwitchTo = null)
        {
            // Store the current window handle
            var winHandleBefore = driver.CurrentWindowHandle;

            // Perform the click operation that opens new window

            // Switch to new window opened
            var newWinHandle = driver.Wait(d =>
            {
                // switch window is current window
                if (titleSwitchTo != null && driver.Title.Equals(titleSwitchTo))
                    return winHandleBefore;


                return driver.WindowHandles.FirstOrDefault(s =>
                {
                    driver.SwitchTo().Window(s);
                    return (!s.Equals(winHandleBefore)) &&
                           (titleSwitchTo == null || titleSwitchTo.Equals(driver.Title));
                });
            },
                "Can't switch to new window :" + titleSwitchTo ?? string.Empty);

            driver.SwitchTo().Window(newWinHandle);
            return newWinHandle;
        }


        public static string GetTestIdFromScenarioTitle(string scenarioTitle)
        {
            return Regex.Match(scenarioTitle.TrimStart(), @"\[([0-9]+)\].*").Groups[1].Value;
        }

        
        private static void SetTextWithPrompt(this IWebDriver driver, IWebElement input, By dropdownListItem, int waitForDroppingDownInSeconds = 20)
        {
            var i = 0;
            while (i++ < 5)
            {
                var t = driver.FindElementOrNull(dropdownListItem, waitForDroppingDownInSeconds);

                if (t != null)
                {
                    t.ClickIt(waitForDroppingDownInSeconds);
                    break;
                }

                input.SendKeys(" ");
                input.SendKeys(Keys.Backspace);
            }
        }

   
        public static string SetTextWithPrompt(this IWebDriver driver, By inputBy, string text, By dropdownListItem, int waitForDroppingDownInSeconds = 20)
        {
            var input = driver.GetElement(inputBy);
            input.SetText(text);

            SetTextWithPrompt(driver, input, dropdownListItem, waitForDroppingDownInSeconds);

            return input.GetText();
        }

       
        public static void SetTextWithPrompt(this IWebDriver driver, By inputBy, string text, string expected, By dropdownListItem, int waitForDroppingDownInSeconds = 20)
        {
            var input = driver.GetElement(inputBy);
            input.SetText(text);

            SetTextWithPrompt(driver, input, dropdownListItem, waitForDroppingDownInSeconds);

            input.GetText().Should().BeEquivalentTo(expected);
        }

        public static void SetTextWithPrompt(this IWebDriver driver, IWebElement input, string text, By dropdownListItem, int waitForDroppingDownInSeconds = 20)
        {
            input.SetText(text);
            
            SetTextWithPrompt(driver, input, dropdownListItem, waitForDroppingDownInSeconds);

        }
        
    }
       
}
