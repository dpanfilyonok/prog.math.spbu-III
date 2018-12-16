using System;

namespace Source
{
    /// <summary>
    /// Фабрика, обеспечивающая создание ленивых объектов в однопоточном и многопоточном режиме
    /// </summary>
    /// <typeparam name="T">Тип возвращаемого объекта</typeparam>
    public static class LazyFactory<T>
    {
        /// <summary>
        /// Создает ленивый объект на основе вычисления с гарантией корректной работы в однопоточном режиме
        /// </summary>
        /// <param name="supplier">Вычисление, предоставляющее объект</param>
        public static ILazy<T> CreateOneThreadLazy(Func<T> supplier)
        {
            if (supplier == null)
            {
                throw new ArgumentNullException(nameof(supplier), "Supplier cannot be null");
            }

            return new OneThreadLazy<T>(supplier);
        }
        
        /// <summary>
        /// Создает ленивый объект на основе вычисления с гарантией корректной работы в многопоточном режиме
        /// </summary>
        /// <param name="supplier">Вычисление, предоставляющее объект</param>
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