using FluentAssertions;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using TechTalk.SpecFlow;
using UITests.Model.Pages.Login;
using UITests.WebDriverLib;

namespace UITests.Model.Steps
{
    [Binding]
    public class MyFirstTestsSteps : BaseSteps
    {
                
        [Given(@"I Navigate to website ""(.*)""")]
        public void GivenINavigateToWebsite(string p0)
        {
            WebDriver.Title.Should().Be("My Store");
        }

        [When(@"I login to website for user ""(.*)"" with default password")]
        public void WhenILoginToWebsiteForUserWithDefaultPassword(string username)
        {            
            WebDriver.Wait(ExpectedConditions.ElementIsVisible(By.CssSelector("a.login")));
            WebDriver.ClickIt(By.CssSelector("a.login"));            
            WebDriver.Wait(ExpectedConditions.TitleContains("Login - My Store"));            
            WebDriver.Wait(ExpectedConditions.ElementIsVisible(By.Id("email")));
            LoginPage login = new LoginPage(WebDriver);
            login.Id = username;
            login.SetPassword("01Testing");
            login.ClickSubmitButton();
        }

        [Then(@"I should be logged in successfully")]
        public void ThenIShouldBeLoggedInSuccessfully()
        {
            WebDriver.Wait(ExpectedConditions.TitleContains("My account - My Store"));
            WebDriver.GetElement(By.CssSelector("a[href*='controller=my-account'].account")).Text.Should().Be("atest testauto");
        }

        [Then(@"My account page is displayed")]
        public void ThenMyAccountPageIsDisplayed()
        {
            WebDriver.GetElement(By.CssSelector("#center_column h1")).Text.Should().Be("MY ACCOUNT");
        }

    }
}
