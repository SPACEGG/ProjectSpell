using System;
using System.Collections.Generic;
using Common.Models;
using UnityEngine;

namespace Gameplay.UI.Damage
{
    [Obsolete("This class is obsolete. Use NetworkDamagePopupManager instead.")]
    public class DamagePopupManager : MonoBehaviour
    {
        [Header("Health Provider")]
        [SerializeField] private MonoBehaviour healthProviderComponent;

        [Header("Prefab Settings")]
        [SerializeField] private GameObject damagePopupPrefab;

        [Header("Popup Settings")]
        [SerializeField] private float popupDuration = 1f;
        [SerializeField] private float popupMoveSpeed = 1f;
        [SerializeField] private float popupFadeSpeed = 1f;
        [SerializeField] private Vector3 popupOffset = new(0, 1f, 0);
        [SerializeField] private Color damageColor = Color.red;
        [SerializeField] private Color healColor = Color.green;

        private IHealthProvider _healthProvider;
        private readonly Dictionary<HealthModel, List<DamagePopup>> _activePopups = new();

        private void Start()
        {
            _healthProvider = healthProviderComponent as IHealthProvider;
            if (_healthProvider is null)
            {
                Debug.LogError($"Health provider component does not implement IHealthProvider interface!");
                enabled = false;
                return;
            }

            _healthProvider.HealthModel.OnDamageTaken += HandleDamageTaken;
            _healthProvider.HealthModel.OnHealed += HandleHealed;
        }

        private void OnDestroy()
        {
            if (_healthProvider is not null)
            {
                _healthProvider.HealthModel.OnDamageTaken -= HandleDamageTaken;
                _healthProvider.HealthModel.OnHealed -= HandleHealed;
            }
        }

        private void HandleDamageTaken(float damage)
        {
            ShowDamagePopup(_healthProvider.HealthModel, damage, damageColor);
        }

        private void HandleHealed(float amount)
        {
            ShowDamagePopup(_healthProvider.HealthModel, amount, healColor);
        }

        private void ShowDamagePopup(HealthModel healthModel, float amount, Color color)
        {
            if (damagePopupPrefab is null)
            {
                Debug.LogWarning("Damage popup prefab is not assigned!");
                return;
            }

            var targetTransform = healthProviderComponent.transform;

            var popupObj = Instantiate(damagePopupPrefab, targetTransform.position + popupOffset, Quaternion.identity);
            var popup = popupObj.GetComponent<DamagePopup>() ?? popupObj.AddComponent<DamagePopup>();

            popup.Initialize(amount.ToString("0"), color, popupDuration, popupMoveSpeed, popupFadeSpeed);

            if (!_activePopups.ContainsKey(healthModel))
            {
                _activePopups[healthModel] = new List<DamagePopup>();
            }

            _activePopups[healthModel].Add(popup);
            // StartCoroutine(popup.Animate());
        }

        private void Update()
        {
            foreach (var targetPopups in _activePopups)
            {
                targetPopups.Value.RemoveAll(popup => popup is null);
                if (targetPopups.Value.Count == 0)
                {
                    _activePopups.Remove(targetPopups.Key);
                }
            }
        }
    }
}