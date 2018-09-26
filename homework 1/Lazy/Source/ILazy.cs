namespace Source
{
    public interface ILazy<T> 
    {
        T Get();
    }
}