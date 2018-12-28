using System;

namespace Source.Attributes
{
    /// <summary>
    /// Атрибут, которым помечается метод, выполняющий тестирование
    /// </summary>
    public sealed class TestAttribute : Attribute
    {
        /// <summary>
        /// Причина игнорирования теста
        /// </summary>
        public string Ignore { get; }

        /// <summary>
        /// Ожидаемое исключение при выполнении теста
        /// </summary>
        public Type ExpectedException { get; }
        
        public TestAttribute(Type expectedException = null, string ignoreReason = null)
        {
            this.ExpectedException = expectedException;
            this.Ignore = ignoreReason;
        }
    }
}