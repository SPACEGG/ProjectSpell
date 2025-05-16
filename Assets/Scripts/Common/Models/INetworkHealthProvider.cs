using Unity.Netcode;

namespace Common.Models
{
    public interface INetworkHealthProvider
    {
        NetworkVariable<NetworkHealthModel> HealthModel { get; }
    }
}