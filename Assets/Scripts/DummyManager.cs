using Common.Data;
using Common.Models;
using UnityEngine;

public class DummyManager : MonoBehaviour, IHealthProvider
{
    private static readonly int TrPushed = Animator.StringToHash("TrPushed");

    [SerializeField] private Animator animator;

    [SerializeField] private HealthData healthData;
    public HealthModel HealthModel { get; private set; }

    private void Awake()
    {
        HealthModel = new HealthModel(healthData);

        HealthModel.OnDamageTaken += (_) => { animator.SetTrigger(TrPushed); };
    }
}