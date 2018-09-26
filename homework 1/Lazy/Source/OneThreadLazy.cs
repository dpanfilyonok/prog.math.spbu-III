using System;

namespace Source
{
    internal class OneThreadLazy<T> : ILazy<T>
    {
        private Func<T> _supplier = null;
        private bool _isCreated = false;
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
