using System.Threading.Tasks;

namespace PocketDungeons.Core.Services
{
    public interface ISaveService
    {
        Task SaveAsync();
        Task LoadAsync();
        T GetData<T>(string key, T defaultValue = default);
        void SetData<T>(string key, T value);
        void DeleteData(string key);
        bool HasData(string key);
    }
}
