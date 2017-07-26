using TechTalk.SpecFlow;
using OpenQA.Selenium;
using UITests.WebDriverLib;

namespace UITests.Model.Steps
{

    public class BaseSteps : TechTalk.SpecFlow.Steps
    {
        internal const string UserKey = "UserKey";
        internal const string DriverKey = "DriverKey";


        public void AddContext(string key, object value)
        {
            ScenarioContext.Current[key] = value;
        }

        
        public T GetContext<T>(string key)
        {
            return ScenarioContext.Current.Get<T>(key);
        }


        public bool IsContextExist(string key)
        {
            return ScenarioContext.Current.ContainsKey(key);
        }
        
       
        protected IWebDriver WebDriver
        {
            get { return ScenarioContext.Current.Get<Driver>(DriverKey).WebDriver; }
            set { ScenarioContext.Current.Get<Driver>(DriverKey).WebDriver = value; }
        }
        
        protected Driver Driver
        {
            get { return GetContext<Driver>(DriverKey); }
            set { AddContext(DriverKey, value); }
        }
    }
}
