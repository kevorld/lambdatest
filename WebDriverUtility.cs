using System;
using System.Linq;
using System.Text;
using System.Threading;
using log4net;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;

namespace LambdaTest
{
    public class WebDriverUtility
    {
        private IWebDriver WebDriver;
        
        private readonly ILog _log = LogManager.GetLogger(typeof(WebDriverUtility));
        
        public static int NORMAL_WAIT_TIME = 5;
        public static int LONGER_WAIT_TIME = 15;
        public static int VERY_LONG_WAIT_TIME = 60;
        private static DateTime _stopwatch;
        public static long SeleniumActionTime;
        private long _previousLoadingTime; 

        public WebDriverUtility(IWebDriver webDriver)
        {
            WebDriver = webDriver;
        }

        public void TypeText(By locator, String text, bool clearText=true)
        {
            TypeText(WebDriver.FindElement(locator), text, clearText);
        }
        
        public bool WaitUntilElementIsVisible(By locator, int maxTimeOutSeconds=5) {
            WebDriverWait wait = new WebDriverWait(WebDriver, TimeSpan.FromSeconds(maxTimeOutSeconds));
            wait.PollingInterval = TimeSpan.FromMilliseconds(1);
            wait.Until(ExpectedConditions.ElementIsVisible(locator));
            return true;
        }
        
        public void TypeText(IWebElement element, String text,bool clearText=true) {
            
            if (clearText)
            {
                element.Clear();
            }
            
            element.SendKeys(text);
        }

        public void ClickAndRetryUntilTimeOut(By locator, int maxTimeOutSeconds=5) {
            ClickAndRetryUntilTimeOut(WebDriver.FindElement(locator), maxTimeOutSeconds);
        }
        
        public bool WaitUntilPageIsFullyLoaded(int maxTimeOutSeconds=5) {
            WebDriverWait wait = new WebDriverWait(WebDriver, TimeSpan.FromSeconds(NORMAL_WAIT_TIME));
            
            wait.Until(webDriver => ((IJavaScriptExecutor) webDriver).
                ExecuteScript("return document.readyState").Equals("complete"));
            return true;
        }

        public void ClickAndRetryUntilTimeOut(IWebElement element, int maxTimeOutSeconds=5)
        {
            WebDriverWait wait = new WebDriverWait(WebDriver, TimeSpan.FromSeconds(maxTimeOutSeconds));
            wait.Until(driver =>
            {
                try {
                    element.Click();
                    return true;
                } catch (WebDriverException e) {
                    _log.Debug(e.Message);
                    return false;
                }
            });
        }
        
        public void ClickByJavascript(IWebElement element) {
            ExecuteJavaScript("arguments[0].click();", element);
        }
        
        public Object ExecuteJavaScript(String script, params Object[] args) {
            IJavaScriptExecutor jse = (IJavaScriptExecutor) WebDriver;
            return jse.ExecuteScript(script, args);
        }

        public void MoveToElement(By locator)
        {
            MoveToElement(WebDriver.FindElement(locator));
        }
        
        public void MoveToElement(IWebElement element)
        {
            Actions builder = new Actions(WebDriver);
            builder.MoveToElement(element).Perform();
        }

        public string WaitUntilTextBecome(By locator, String text, int maxTimeOutSeconds=5) {
            return WaitUntilTextBecome(WebDriver.FindElement(locator), text, maxTimeOutSeconds);
        }

        public string WaitUntilTextBecome(IWebElement element, String expectedTitle, int maxTimeOutSeconds=5) {
            
            return WaitUntil(
                () => element.Text,
                text => text.Equals(expectedTitle), 
                maxTimeOutSeconds);
        }
        
        public bool WaitUntilAnyTextContains(By locator, String text)
        {
            return WebElementDynamicWait(locator, element => element.Text.Contains(text));
        }

        public string WaitUntilTextContains(IWebElement element, String expectedText, int maxTimeOutSeconds=5) {
            
            return WaitUntil(
                () => element.Text,
                text => text.Contains(expectedText), 
                maxTimeOutSeconds);
        }
        
        public void PressKeyFromKeyboard(By locator, string key)
        {
            PressKeyFromKeyboard(WebDriver.FindElement(locator), key);
        }
        
        public void PressKeyFromKeyboard(IWebElement element, string key)
        {
            element.SendKeys(key);
        }

        public String GetText(By locator)
        {
            return GetText(WebDriver.FindElement(locator));
        }
        
        public String GetText(IWebElement element)
        {
            return element.Text;
        }
        
        public String WaitUntilAllTextsReturn(By locator, int pollingMilliSecs=10, int maxTimeOutSeconds=5)
        {
            string finalText = "";
            WebDriverWait wait = new WebDriverWait(WebDriver, TimeSpan.FromSeconds(maxTimeOutSeconds));
            wait.PollingInterval = TimeSpan.FromMilliseconds(pollingMilliSecs);
            
            wait.Until(driver =>
            {
                try
                {
                    string firstResult = GetTexts(locator);
                    WaitForMilliSeconds(100); 
                    finalText = GetTexts(locator);
                    
                    return firstResult.Equals(finalText);
                } catch (WebDriverException e) {
                    _log.Debug(e.Message);
                    return false;
                }
            });

            return finalText;
        }
        
        public String GetTexts(By locator)
        {
            var sb = new StringBuilder();
            
            foreach (var element in WebDriver.FindElements(locator))
            {
                sb.Append(element.Text);
            }
            
            return sb.ToString();
        }

        public void SwitchToIframe(By frame, int maxTimeOutSeconds=5)
        {
            WebDriverWait wait = new WebDriverWait(WebDriver, TimeSpan.FromSeconds(maxTimeOutSeconds));
            wait.PollingInterval = TimeSpan.FromMilliseconds(1);
            wait.Until(ExpectedConditions.FrameToBeAvailableAndSwitchToIt(frame));
        }

        public void SwitchBackToMainFrame()
        {
            WebDriver.SwitchTo().DefaultContent();
        }

        public static void WaitForSeconds(int maxWaitSeconds) {
            Thread.Sleep(TimeSpan.FromSeconds(maxWaitSeconds));
        }
        
        public static void WaitForMilliSeconds(int maxWaitMilliSeconds) {
            Thread.Sleep(TimeSpan.FromMilliseconds(maxWaitMilliSeconds));
        }
        
        
        public string GetElementAttribute(By locator, string targetAttribute)
        {
            return WaitUntil(
                () => WebDriver.FindElement(locator).GetAttribute(targetAttribute),
                result => result != string.Empty);
        }



        /// <summary>
        /// Helper method to wait until the expected result is available on the UI
        /// </summary>
        /// <typeparam name="T">The type of result to retrieve</typeparam>
        /// <param name="getResult">The function to poll the result from the UI</param>
        /// <param name="isResultAccepted">The function to decide if the polled result is accepted</param>
        /// <returns>An accepted result returned from the UI. If the UI does not return an accepted result within the timeout an exception is thrown.</returns>
        private T WaitUntil<T>(Func<T> getResult, Func<T, bool> isResultAccepted) where T: class
        {
            return WaitUntil(getResult, isResultAccepted, NORMAL_WAIT_TIME);
        }
        
        private T WaitUntil<T>(Func<T> getResult, Func<T, bool> isResultAccepted, int maxTimeOutSeconds=5) where T: class
        {
            var wait = new WebDriverWait(WebDriver, TimeSpan.FromSeconds(maxTimeOutSeconds));
            return wait.Until(driver =>
            {
                var result = getResult();
                if (!isResultAccepted(result))
                    return default;

                return result;
            });
        }
        
        /**
         * If any elements could meet condition, return true. Otherwise, timeout.
         */
        public bool WebElementDynamicWait(By locator, Func<IWebElement, bool> isAnyResultAccepted, int pollingMilliSecs=10, int maxTimeOutSeconds=5)
        {
            WebDriverWait wait = new WebDriverWait(WebDriver, TimeSpan.FromSeconds(maxTimeOutSeconds));
            wait.PollingInterval = TimeSpan.FromMilliseconds(pollingMilliSecs);
            wait.Until(driver =>
            {
                try
                {
                    return driver.FindElements(locator).Any(isAnyResultAccepted);
                } catch (WebDriverException e) {
                    _log.Debug(e.Message);
                    return false;
                }
            });

            return true;
        }
    }
}