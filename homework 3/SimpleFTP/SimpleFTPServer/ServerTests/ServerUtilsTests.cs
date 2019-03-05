namespace ServerTests
{
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ServerSource;

    /// <summary>
    /// Класс, проверяющий утилитные функции работы сервера
    /// </summary>
    [TestClass]
    public class ServerUtilsTests
    {
        /// <summary>
        /// Проверяет корректность получения содержимого директории
        /// </summary>
        [TestMethod]
        public void GetListOfElementsInDirShouldReturnCorrectData()
        {
            var expected = new HashSet<(string, bool)>()
            {
                ("NestedFolder1", true),
                ("NestedFolder2", true),
                ("1", false),
                ("2", false),
                ("3", false)
            };
            
            var data = SimpleFTPServerUtils.GetListOfElementsInDir(@"../../../TestFolder");
            var actual = new HashSet<(string, bool)>(data);

            Assert.IsTrue(expected.IsSubsetOf(actual) && actual.IsSubsetOf(expected));
        }
    }
}