namespace TownOfBlakulla.Core.Handlers
{
    // NOTE(Zerratar): propertyName collisions could be a potential problem
    public interface IPropertyRepository
    {
        T Load<T>(string propertyName);
        void Save<T>(string propertyName, T value);
    }
}