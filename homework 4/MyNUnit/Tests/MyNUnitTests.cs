using Microsoft.VisualStudio.TestTools.UnitTesting;
using Source;


namespace Tests
{
    /// <summary>
    /// Tests for MuyNUnit Test Launcher
    /// </summary>
    [TestClass]
    public class MyNUnitTests
    {
        private TestLauncher _launcher;

        private static string FormPathToTestProject(string projName)
        {
            const string pathPrefix = "../../../../TestProjects/";
            const string pathPostfix = "/bin/Debug/netcoreapp2.1";

            return pathPrefix + projName + pathPostfix;
        }

        /// <summary>
        /// BeforeClassAttribute behavior test
        /// </summary>
        [TestMethod]
        public void BeforeClassShouldBeExecutedProperly()
        {
            string testPath = FormPathToTestProject("BeforeClassTest");
            _launcher = new TestLauncher(testPath);
            _launcher.LaunchTesting();
            Assert.AreEqual(2, BeforeClassTest.BeforeClassTest.CheckValue);
        }

        /// <summary>
        /// AfterClassAttribute behavior test
        /// </summary>
        [TestMethod]
        public void AfterClassShouldBeExecutedProperly()
        {
            string testPath = FormPathToTestProject("AfterClassTest");
            _launcher = new TestLauncher(testPath);
            _launcher.LaunchTesting();
            Assert.AreEqual(2, AfterClassTest.AfterClassTest.CheckValue);
        }

        /// <summary>
        /// BeforeClassAttribute and AfterClassAttribute behavior test
        /// </summary>
        [TestMethod]
        public void BeforeClassAndAfterClassShouldBeExecutedProperly()
        {
            string testPath = FormPathToTestProject("BeforeClassAndAfterClassTest");
            _launcher = new TestLauncher(testPath);
            _launcher.LaunchTesting();
            Assert.AreEqual(4, BeforeClassAndAfterClassTest.BeforeClassAndAfterClassTest.CheckValue);
        }

        /// <summary>
        /// BeforeAttribute and AfterAttribute behavior test
        /// </summary>
        [TestMethod]
        public void BeforeAndAfterShouldbeExecutedProperly()
        {
            string testPath = FormPathToTestProject("BeforeAndAfterTest");
            _launcher = new TestLauncher(testPath);
            _launcher.LaunchTesting();
            Assert.AreEqual(4 * 3, BeforeAndAfterTest.BeforeAndAfterTest.CheckValue);
        }

        /// <summary>
        /// BeforeAttribute methods should be executed on same instance
        /// </summary>
        [TestMethod]
        public void BeforeAndTestShouldbeExecutedOnTheSameInstance()
        {
            string testPath = FormPathToTestProject("BeforeInstanceTest");
            _launcher = new TestLauncher(testPath);
            _launcher.LaunchTesting();
            Assert.AreEqual(BeforeInstanceTest.BeforeInstanceTest.TestHash, BeforeInstanceTest.BeforeInstanceTest.BeforeHash);
        }

        /// <summary>
        /// AfterAttribute methods should be executed on same instance
        /// </summary>
        [TestMethod]
        public void AfterAndTestShouldbeExecutedOnTheSameInstance()
        {
            string testPath = FormPathToTestProject("AfterInstanceTest");
            _launcher = new TestLauncher(testPath);
            _launcher.LaunchTesting();
            Assert.AreEqual(AfterInstanceTest.AfterInstanceTest.TestHash, AfterInstanceTest.AfterInstanceTest.AfterHash);
        }

        /// <summary>
        /// Smoke tests
        /// </summary>
        [TestMethod]
        public void TestsResultsShouldBeCorrect()
        {
            string testPath = FormPathToTestProject("SucceededFailedIgnoredTest");
            _launcher = new TestLauncher(testPath);
            _launcher.LaunchTesting();
            Assert.AreEqual(2, _launcher.Succeeded);
            Assert.AreEqual(1, _launcher.Failed);
            Assert.AreEqual(2, _launcher.Ignored);
        }

        /// <summary>
        /// Test methods with expected exception behavior tests
        /// </summary>
        [TestMethod]
        public void TestsWithExpectedExceptionResultsShouldBeCorrect()
        {
            string testPath = FormPathToTestProject("ExpectedExceptionTest");
            _launcher = new TestLauncher(testPath);
            _launcher.LaunchTesting();
            Assert.AreEqual(1, _launcher.Succeeded);
            Assert.AreEqual(1, _launcher.Failed);
        }
    }
}
