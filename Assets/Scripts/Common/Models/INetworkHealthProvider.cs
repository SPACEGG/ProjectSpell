using Unity.Netcode;

namespace Common.Models
{
    public interface INetworkHealthProvider
    {
        NetworkVariable<NewNetworkHealthModel> HealthModel { get; }

        public void TakeDamage(float damage);
        public void Heal(float contextValue);
    }
}