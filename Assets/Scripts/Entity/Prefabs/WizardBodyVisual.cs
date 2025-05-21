using UnityEngine;

namespace Entity.Prefabs
{
    public class WizardBodyVisual : MonoBehaviour
    {
        [SerializeField] private Renderer wizardBodyRenderer;

        public void SetPlayerColor(Material material)
        {
            if (material)
            {
                wizardBodyRenderer.material = material;
            }
            else
            {
                Debug.LogWarning("Player color material is not assigned.");
            }
        }
    }
}