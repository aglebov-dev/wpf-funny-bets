namespace Company.Client.Common.Interfaces
{
    using System;
    using System.Threading.Tasks;

    public interface IPersistenceProvider
    {
        Task Write<T>(string topic, Guid persistenceID, T data);

        Task<string[]> Read(string topic);

        Task Delete(string topic, Guid persistenceID);
    }
}
