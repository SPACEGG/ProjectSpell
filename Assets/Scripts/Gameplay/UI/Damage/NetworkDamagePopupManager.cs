using Player;
using UnityEngine;

namespace Gameplay.UI.Damage
{
    [RequireComponent(typeof(NetworkHealthManaManager))]
    public class NetworkDamagePopupManager : MonoBehaviour
    {
        [Header("Health Provider")]
        [SerializeField] private NetworkHealthManaManager networkHealthManaManager;

        [Header("Prefab Settings")]
        [SerializeField] private GameObject damagePopupPrefab;

        [Header("Popup Settings")]
        [SerializeField] private Transform playerArmature;
        [SerializeField] private float popupDuration = 1f;
        [SerializeField] private float popupMoveSpeed = 1f;
        [SerializeField] private float popupFadeSpeed = 1f;
        [SerializeField] private Vector3 popupOffset = new(0, 3f, 0);
        [SerializeField] private Color damageColor = Color.red;
        [SerializeField] private Color healColor = Color.green;

        private float _previousHealth;

        private void Start()
        {
            if (networkHealthManaManager == null)
            {
                networkHealthManaManager = GetComponent<NetworkHealthManaManager>();
            }

            if (damagePopupPrefab == null)
            {
                Debug.LogError("Damage popup prefab is not assigned!");
                enabled = false;
                return;
            }

            if (playerArmature == null)
            {
                Debug.LogError("Player armature is not assigned!");
                enabled = false;
                return;
            }

            networkHealthManaManager.OnHealthChanged += NetworkHealthManaManager_OnOnHealthChanged;
            _previousHealth = networkHealthManaManager.HealthModel.Value.CurrentHealth;
        }

        private void OnDestroy()
        {
            if (networkHealthManaManager != null)
            {
                networkHealthManaManager.OnHealthChanged -= NetworkHealthManaManager_OnOnHealthChanged;
            }
        }

        private void NetworkHealthManaManager_OnOnHealthChanged(object sender, NetworkHealthManaManager.OnHealthChangedEventArgs e)
        {
            float healthDifference = e.CurrentHealth - _previousHealth;

            if (healthDifference > 0)
            {
                ShowDamagePopup(healthDifference, healColor);
            }
            else if (healthDifference < 0)
            {
                ShowDamagePopup(Mathf.Abs(healthDifference), damageColor);
            }

            _previousHealth = e.CurrentHealth;
        }

        private void ShowDamagePopup(float amount, Color color)
        {
            var spawnPosition = playerArmature.position + popupOffset;
            var popupObj = Instantiate(damagePopupPrefab, spawnPosition, Quaternion.identity);
            var popup = popupObj.GetComponent<DamagePopup>();

            if (popup == null)
            {
                Destroy(popupObj);
                return;
            }

            popup.Initialize(amount.ToString("0"), color, popupDuration, popupMoveSpeed, popupFadeSpeed);
            popup.Animate();
        }
    }
}