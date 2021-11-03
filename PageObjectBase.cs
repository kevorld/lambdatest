using OpenQA.Selenium;
using PMUITestBase.Utilities;

namespace LambdaTest
{
    public class PageObjectBase
    {
        protected WebDriverUtility _webDriverUtility;
        protected IWebDriver _webDriver;

        public PageObjectBase(IWebDriver webDriver)
        {
            _webDriver = webDriver;
            _webDriverUtility = new WebDriverUtility(webDriver);

        }
    }
}