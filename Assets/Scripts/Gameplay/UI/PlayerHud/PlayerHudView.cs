using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI.PlayerHud
{
    public class PlayerHudView : MonoBehaviour
    {
        [SerializeField] private Slider hpSlider;
        [SerializeField] private TextMeshProUGUI hpText;

        public void SetHp(float current, float max)
        {
            hpSlider.value = current / max;
            hpText.text = $"{current} / {max}";
        }
    }
}