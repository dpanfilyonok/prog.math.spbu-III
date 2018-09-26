using System;

namespace Source
{
    internal class MultiThreadLazy<T> : ILazy<T>
    {
        private Func<T> _supplier = null;
        // volatile чтобы избежать ситуации, когда поле закэшировалось, 
        // но другие потоки ещё не знают об изменении в нем
        private volatile bool _isObjectCreated = false;
        private T _createdObject;
        private object _lockObject = new object();

        public MultiThreadLazy(Func<T> supplier)
            // запись ссылки - атомарная операция
            => _supplier = supplier;

        public T Get()
        {
            if (!_isObjectCreated)
            {
                lock (_lockObject)
                {
                    // может быть такая ситуация, когда мы вошли в if, перешли на другой поток,
                    // там создали объект и выставили флаг _isObjectCreated = true, но возвратившись
                    // в исходный поток мы снова создадим объект (на самом деле нет, тк _supplier == null и все сломается)
                    if (!_isObjectCreated)
                    {
                        _createdObject = _supplier();
                        _isObjectCreated = true;
                        _supplier = null;
                    }
                }
            }

            return _createdObject;
        }
    }
}