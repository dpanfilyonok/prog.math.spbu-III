using System;

namespace Source
{
    public static class LazyFactory<T>
    {
        public static ILazy<T> CreateOneThreadLazy(Func<T> supplier)
        {
            if (supplier == null)
            {
                throw new ArgumentNullException(nameof(supplier), "Supplier cannot be null");
            }

            return new OneThreadLazy<T>(supplier);
        }
        public static ILazy<T> CreateMultiThreadLazy(Func<T> supplier)
        {
            if (supplier == null)
            {
                throw new ArgumentNullException(nameof(supplier), "Supplier cannot be null");
            }

            return new MultiThreadLazy<T>(supplier);
        }
    }
}