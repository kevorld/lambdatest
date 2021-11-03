using System;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using Xunit;

namespace LambdaTest
{
    public class LocalTest : IDisposable
    {
        private RemoteWebDriver driver = null;
        private int _implicitWait = 3;
        private int _pageLoad = 15;

        public LocalTest()
        {
            ChromeOptions options = new ChromeOptions();
            new DriverManager().SetUpDriver(new ChromeConfig()); 
            driver = new ChromeDriver(options);
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