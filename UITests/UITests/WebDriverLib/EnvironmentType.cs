using System;

namespace UITests.WebDriverLib
{
    public enum EnvironmentType
    {
        Dev,
        Test,
        PreProd,
        Prod
    }
    
    public class Environment
    {    
        public static string GetUrl(EnvironmentType environment)
        {
            switch (environment)
            {                
                case EnvironmentType.Dev:
                    return "http://automationpractice.com";
                case EnvironmentType.Test:
                    return "http://automationpractice.com";
                case EnvironmentType.PreProd:                    
                    return "http://automationpractice.com";
                case EnvironmentType.Prod:
                    return "http://automationpractice.com";
                default:
                    Console.WriteLine("No Environment URL configured");
                    return "No Environment URL configured";
            }
        }
        
    }
}
