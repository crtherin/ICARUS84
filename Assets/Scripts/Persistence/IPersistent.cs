public interface IPersistent<T>
{
    T Save();
    void Load(T data);
}