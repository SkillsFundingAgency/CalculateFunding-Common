namespace CalculateFunding.Common.Graph.Interfaces
{
    public interface ICosmosGraphDbSettings
    {
        string EndPointUrl { get; set; }
        int Port { get; set; }
        string ApiKey { get; set; }
        string ContainerPath { get; set; }
    }
}