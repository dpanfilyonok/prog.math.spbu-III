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

namespace Source
{
    public class TestLauncher
    {
        private ConcurrentBag<TestInfo> _executedTestInfos;
        private ManualResetEvent _testsExecuted;
        private readonly string _pathToTestDir;

        public int Succeeded { get; private set; } = 0;
        public int Failed { get; private set; } = 0;
        public int Ignored { get; private set; } = 0;


        public TestLauncher(string pathToDir)
        {
            _executedTestInfos = new ConcurrentBag<TestInfo>();
            _testsExecuted = new ManualResetEvent(false);
            _pathToTestDir = pathToDir;
        }

        public void LaunchTesting()
        {
            var types = GetAssembliesInDir(_pathToTestDir);
            Parallel.ForEach(types, RunTests);
        }

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
            var testClassInstance = Activator.CreateInstance(testClass);
            ExecuteAllMethodsWithAttribute<BeforeClassAttribute>(testClass, testClassInstance);
            ExecuteAllMethodsWithAttribute<TestAttribute>(testClass, testClassInstance);
            ExecuteAllMethodsWithAttribute<AfterClassAttribute>(testClass, testClassInstance);
            _testsExecuted.Set();
        }

        private void ExecuteAllMethodsWithAttribute<T>(Type testClass, object testClassInstance)
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
                    if (typeof(T) == typeof(TestAttribute))
                    {
                        ExecuteTestMethod(methodInfo, testClassInstance);
                    }
                    else
                    {
                        methodInfo.Invoke(testClassInstance, null);
                    }
                });
        }

        private void ExecuteTestMethod(MethodInfo mInfo, object testClassInstance)
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

            var testClass = mInfo.DeclaringType;
            var instance = Activator.CreateInstance(testClass);

            ExecuteAllMethodsWithAttribute<BeforeAttribute>(testClass, testClassInstance);

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
                    e.GetType() == testAttribute.ExpectedException.GetType();
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
            ExecuteAllMethodsWithAttribute<AfterAttribute>(testClass, testClassInstance);
        }
    }
}