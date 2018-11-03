using System;

namespace Source
{
    /// <summary>
    /// Класс, реализующий ленивое вычисление объекта однопоточно
    /// </summary>
    /// <typeparam name="T">Тип возвращаемого объекта</typeparam>
    internal class OneThreadLazy<T> : ILazy<T>
    {
        /// <summary>
        /// Вычисление, предоставляющее объект
        /// </summary>
        private Func<T> _supplier = null;

        /// <summary>
        /// true, если объект создан, false иначе
        /// </summary>
        private bool _isCreated = false;

        /// <summary>
        /// Ссылка на созданный объект
        /// </summary>
        private T _createdObject;

        public OneThreadLazy(Func<T> supplier) => _supplier = supplier;

        public T Get() 
        {
            if (!_isCreated)
            {
                _createdObject = _supplier();
                _isCreated = true;
                _supplier = null;
            }
            
            return _createdObject;
        }
    }
}
