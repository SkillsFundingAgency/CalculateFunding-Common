namespace CalculateFunding.Common.Graph.Interfaces
{
    public interface ICosmosGraphDbSettings
    {
        string EndPointUrl { get; set; }
        int Port { get; set; }
        string ApiKey { get; set; }
        string ContainerPath { get; set; }
        int DegreeOfParallelism { get; set; }
        int? MaxInProcessPerConnection { get; set; }
        int? PoolSize { get; set; }
        int? ReconnectionAttempts { get; set; }
        int? ReconnectionBaseDelay { get; set; }
        int? KeepAliveInterval { get; set; }
    }
}