using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI.PlayerHud
{
    public class PlayerHudView : MonoBehaviour
    {
        [SerializeField] private Slider hpSlider;
        [SerializeField] private TextMeshProUGUI hpText;
        [SerializeField] private Slider[] mpSliders;

        public void SetHp(float current, float max)
        {
            hpSlider.value = current / max;
            hpText.text = $"{current} / {max}";
        }

        public void SetMp(float currentMana, float manaPerLevel)
        {
            var filledManaLevelCount = (int)Math.Truncate(currentMana / manaPerLevel);
            var manaPercentageInLastLevel = currentMana % manaPerLevel;

            for (var i = 0; i < mpSliders.Length; i++)
            {
                if (i < filledManaLevelCount)
                {
                    mpSliders[i].value = 1;
                }
                else if (i == filledManaLevelCount)
                {
                    mpSliders[i].value = manaPercentageInLastLevel / manaPerLevel;
                }
                else
                {
                    mpSliders[i].value = 0;
                }
            }
        }
    }
}