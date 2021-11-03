using System;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using Xunit;

namespace LambdaTest
{
    public class LambdaTest : IDisposable
    {
        private RemoteWebDriver driver = null;
        private int _implicitWait = 3;
        private int _pageLoad = 15;
        private String username = Environment.GetEnvironmentVariable("userName");
        private String accessKey = Environment.GetEnvironmentVariable("accessKey");

        public LambdaTest()
        {
            ChromeOptions options = new ChromeOptions();
            options.AddAdditionalCapability("user",$"{username}", true);
            options.AddAdditionalCapability("accessKey",$"{accessKey}", true);
            options.AddAdditionalCapability("build", "LocalDebug", true);
            options.AddAdditionalCapability("name", "Lambdatest", true);
            options.AddAdditionalCapability("console",false, true);
            options.AddAdditionalCapability("network",false, true);
            options.PlatformName = "MacOS Big sur";
            options.BrowserVersion = "92.0";
            
            driver = new RemoteWebDriver(
                new Uri("https://" + username + ":" + accessKey + "@hub.lambdatest.com/wd/hub"), options);
        }

        [Fact]
        public void Test()
        {
            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(_implicitWait);
            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(_pageLoad);

            driver.Navigate().GoToUrl("https://testsoftware.projectmanager.com/login");

            var loginPage = new LoginPage(driver);
            loginPage.EnterEmail("pmautot+temp+xpmox202111031113276306400@gmail.com");
            loginPage.EnterPassword("IsCOqI4BG6");
            loginPage.Submit();
            
            driver.Navigate().GoToUrl("https://testsoftware.projectmanager.com/project/plan/T2");
            
            var ganttPage = new GanttPage(driver);
            Assert.True(ganttPage.GanttPageIsLoaded());
            ganttPage.RemoveAllTasks();
            
            string taskName = $"Task{DateTime.Now.Minute.ToString()}";
            ganttPage.CreateOneTaskFromToolbar(taskName);
            ganttPage.SelectSingleTask(taskName);
            ganttPage.ClickOnButtonFromToolBar("shareTask");
            
            var shareTaskPopup = new ShareTaskPopup(driver);
            Assert.True(shareTaskPopup.IsShareTaskPopupDisplayed());
            
            driver.Navigate().GoToUrl(shareTaskPopup.GetShareTaskUrl());
            
            var taskDetailPage = new TaskDetailPage(driver);
            Assert.True(taskDetailPage.IsTaskDetailPageDisplayed(taskName));
            taskDetailPage.MoreActions("delete");
            
            var genericPopup = new GenericPopup(driver);
            Assert.True(genericPopup.GetPopupMessage("Are you sure you want to delete this task permanently?"));
            genericPopup.ClickOnPopupMessage("Yes, Delete Task");
        }

        public void Dispose()
        {
            driver.Quit();
        }
    }
}