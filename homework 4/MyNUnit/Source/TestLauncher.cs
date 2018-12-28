using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Source.Attributes;
using Source.Exceptions;

namespace Source
{
    /// <summary>
    /// Class which launch test on mentioned directory
    /// </summary>
    public class TestLauncher
    {
        private ConcurrentBag<TestInfo> _executedTestInfos;
        private ManualResetEvent _testsExecuted;
        private readonly string _pathToTestDir;

        /// <summary>
        /// Amount of succeeded tests
        /// </summary>
        public int Succeeded { get; private set; } = 0;

        /// <summary>
        /// Amount of failed tests
        /// </summary>
        public int Failed { get; private set; } = 0;

        /// <summary>
        /// Amount of ignored tests
        /// </summary>
        public int Ignored { get; private set; } = 0;

        public TestLauncher(string pathToDir)
        {
            _executedTestInfos = new ConcurrentBag<TestInfo>();
            _testsExecuted = new ManualResetEvent(false);
            _pathToTestDir = pathToDir;
        }

        /// <summary>
        /// Launch testing
        /// </summary>
        public void LaunchTesting()
        {
            var types = GetAssembliesInDir(_pathToTestDir);
            Parallel.ForEach(types, RunTests);
            _testsExecuted.Set();
        }

        /// <summary>
        /// Print results of tests to console
        /// </summary>
        public void PrintResults()
        {
            _testsExecuted.WaitOne();

            var succeeded = 0;
            var failed = 0;
            var ignored = 0;
            foreach (var testInfo in _executedTestInfos)
            {
                Console.WriteLine($"Test name : {testInfo.Name}");
                Console.WriteLine($"Test result : {testInfo.Result}");
                switch (testInfo.Result)
                {
                    case Results.Succeeded:
                        succeeded++;
                        Console.WriteLine($"Completion time : {testInfo.CompletionTime} ms");
                        break;
                    case Results.Failed:
                        failed++;
                        Console.WriteLine($"Completion time : {testInfo.CompletionTime} ms");
                        break;
                    case Results.Ignored:
                        ignored++;
                        Console.WriteLine($"Ignore reason : {testInfo.IgnoreReason}");
                        break;
                }

                Console.WriteLine('\n');
            }

            Console.WriteLine("Total:");
            Console.WriteLine($"Succeeded - {succeeded}");
            Console.WriteLine($"Failed: - {failed}");
            Console.WriteLine($"Ignored: - {ignored}");
        }

        private IEnumerable<Type> GetAssembliesInDir(string pathToDir)
        {
            if (!Directory.Exists(pathToDir))
            {
                throw new DirectoryNotFoundException($"Directory on {pathToDir} was not found");
            }

            var dirInfo = new DirectoryInfo(pathToDir);
            return dirInfo.EnumerateFiles()
              .Where(fileInfo => fileInfo.Extension == ".dll")
              .Select(fileInfo => Assembly.LoadFrom(fileInfo.FullName))
              .ToHashSet()
              .SelectMany(assembly => assembly.ExportedTypes);
        }

        private void RunTests(Type testClass)
        {
            ExecuteAllMethodsWithAttribute<BeforeClassAttribute>(testClass);
            ExecuteAllMethodsWithAttribute<TestAttribute>(testClass);
            ExecuteAllMethodsWithAttribute<AfterClassAttribute>(testClass);
        }

        private void ExecuteAllMethodsWithAttribute<T>(Type testClass, object testClassInstance = null)
            where T : Attribute
        {
            testClass
                .GetMethods()
                .Where(mInfo => mInfo.GetCustomAttributes()
                    .Select(attr => attr.GetType())
                    .Contains(typeof(T)))
                .AsParallel()
                .ForAll(methodInfo =>
                {
                    var instance = testClassInstance ?? Activator.CreateInstance(testClass);
                    ValidateMethodForAttribute<T>(methodInfo);
                    switch (typeof(T))
                    {
                        case Type testAttr when (testAttr == typeof(TestAttribute)):
                            ExecuteTestMethod(methodInfo, instance);
                            break;
                        case Type simpleAttr when (simpleAttr == typeof(BeforeAttribute) || simpleAttr == typeof(AfterAttribute)):
                            methodInfo.Invoke(instance, null);
                            break;
                        case Type classAttr when (classAttr == typeof(BeforeClassAttribute) || classAttr == typeof(AfterClassAttribute)):
                            methodInfo.Invoke(null, null);
                            break;
                        default:
                            throw new Exception();
                    }
                });
        }

        private void ExecuteTestMethod(MethodInfo mInfo, object instance)
        {
            var testAttribute = Attribute.GetCustomAttribute(mInfo, typeof(TestAttribute)) as TestAttribute;
            TestInfo testInfo;

            if (testAttribute.Ignore != null)
            {
                testInfo = new TestInfo(mInfo.Name, Results.Ignored, ignoreReason: testAttribute.Ignore);
                _executedTestInfos.Add(testInfo);
                Ignored++;
                return;
            }

            ExecuteAllMethodsWithAttribute<BeforeAttribute>(mInfo.DeclaringType, instance);

            var succeeded = false;
            var stopWatch = Stopwatch.StartNew();
            try
            {
                mInfo.Invoke(instance, null);
                succeeded = testAttribute.ExpectedException == null;
            }
            catch (Exception e)
            {
                succeeded = testAttribute.ExpectedException != null &&
                    e.InnerException.GetType() == testAttribute.ExpectedException.GetType();
            }

            stopWatch.Stop();
            testInfo = new TestInfo(mInfo.Name, succeeded ? Results.Succeeded : Results.Failed, completionTime: stopWatch.ElapsedMilliseconds);
            if (testInfo.Result == Results.Succeeded)
            {
                Succeeded++;
            }
            else
            {
                Failed++;
            }

            _executedTestInfos.Add(testInfo);

            ExecuteAllMethodsWithAttribute<AfterAttribute>(mInfo.DeclaringType, instance);
        }

        private static void ValidateMethodForAttribute<T>(MethodInfo mInfo)
        {
            bool isValid = false;
            switch (typeof(T))
            {
                case Type testAttr when (testAttr == typeof(TestAttribute)):
                    isValid = mInfo.ReturnType == typeof(void) && mInfo.GetParameters().Length == 0;
                    break;
                case Type classAttr when (classAttr == typeof(BeforeClassAttribute) || classAttr == typeof(AfterClassAttribute)):
                    isValid = mInfo.IsStatic && mInfo.ReturnType == typeof(void) && mInfo.GetParameters().Length == 0;
                    break;
                case Type simpleAttr when (simpleAttr == typeof(BeforeAttribute) || simpleAttr == typeof(AfterAttribute)):
                    isValid = mInfo.ReturnType == typeof(void) && mInfo.GetParameters().Length == 0;
                    break;
                default:
                    break;
            }

            if (!isValid)
            {
                throw new InvalidTestMethodSignatureException();
            }
        }
    }
}