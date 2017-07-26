using OpenQA.Selenium;
using UITests.WebDriverLib;

namespace UITests.Model.Pages.Login
{
    public class LoginPage : BasePage
    {
        private readonly By createAccount = By.Id("SubmitCreate");

        private readonly By forgotPasswordBtn = By.LinkText("Forgot your password?");
        
        private readonly By loginBtn = By.Id("SubmitLogin");

        private readonly By nameInput = By.Id("email");

        private readonly By passwordInput = By.Id("passwd");

        public LoginPage(IWebDriver driver1) : base(driver1)
        {
        }

        public string Id
        {
            get { return WebDriver.GetElement(nameInput).GetText(); }

            set { WebDriver.GetElement(nameInput).SetText(value); }
            }

        public void SetPassword(string password)
        {
            WebDriver.FindElement(passwordInput).SetText(password);
        }

        public void ClickSubmitButton()
        {
            WebDriver.ClickIt(loginBtn, 5);
        }

        
        public void ClickForgetPassword()
        {
            WebDriver.ClickIt(forgotPasswordBtn);
        }


        public void CreateNewAccount()
        {
            WebDriver.ClickIt(createAccount);
        }
        
    }
}
