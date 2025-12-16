namespace EmployeeSystem.Services.Interfaces
{
    public interface IAppCache
    {
        Task<T?> GetAsync<T>(string key);

        Task SetAsync<T>(
            string key,
            T value,
            TimeSpan? absoluteExpiration = null
         );

        Task RemoveAsync(string key);
    }
}
