using System;

namespace Source.Attributes
{
    /// <summary>
    /// Атрибут, которым помечается метод, выполняемый до выполнения всех тестов в классе
    /// </summary>
    public sealed class BeforeClassAttribute : Attribute { }
}