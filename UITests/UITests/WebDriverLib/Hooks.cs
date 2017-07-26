using System;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Tracing;
using UITests.Model.Steps;

namespace UITests.WebDriverLib
{
    [Binding]
    public sealed class Hooks
    {
        private Driver Driver
        {
            get
            {
                return ScenarioContext.Current.Get<Driver>(BaseSteps.DriverKey);
            }

            set
            {
                ScenarioContext.Current[BaseSteps.DriverKey] = value;
            }
        }


        [BeforeScenario]
        public void InitialiseAUT()
        {            
            Driver = new Driver();
            Driver.Init();            
        }


        [AfterScenario]
        public void TearDown()
        {
            if (Driver == null)
            {
                Console.WriteLine("Driver is null");
                return;
            }

            if (ScenarioContext.Current.TestError != null)
            {
                var name = string.Format(
                     "{0}_{1}",
                     FeatureContext.Current.FeatureInfo.Title.ToIdentifier(),
                     ScenarioContext.Current.ScenarioInfo.Title.ToIdentifier());
                Driver.TakeScreenshot(name);
            }

            Driver.Kill();
        }
    }
}
