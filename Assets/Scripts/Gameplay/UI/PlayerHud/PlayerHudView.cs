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
        [SerializeField] private float animationSpeed = 8f;

        private bool isAnimatingHp;
        private float targetHpValue;
        private bool isAnimatingMp;
        private float[] targetMpValues;

        private void Awake()
        {
            targetMpValues = new float[mpSliders.Length];
        }

        private void Update()
        {
            UpdateHpAnimation();
            UpdateMpAnimation();
        }

        public void SetHp(float current, float max)
        {
            targetHpValue = current / max;
            hpText.text = $"{current} / {max}";
            isAnimatingHp = true;
        }

        public void SetMp(float currentMana, float manaPerLevel)
        {
            var filledManaLevelCount = (int)Math.Truncate(currentMana / manaPerLevel);
            var manaPercentageInLastLevel = currentMana % manaPerLevel;

            for (var i = 0; i < mpSliders.Length; i++)
            {
                if (i < filledManaLevelCount)
                {
                    targetMpValues[i] = 1f;
                }
                else if (i == filledManaLevelCount)
                {
                    targetMpValues[i] = manaPercentageInLastLevel / manaPerLevel;
                }
                else
                {
                    targetMpValues[i] = 0f;
                }
            }

            isAnimatingMp = true;
        }

        private void UpdateHpAnimation()
        {
            if (!isAnimatingHp) return;

            float currentSpeed = hpSlider.value > targetHpValue ? animationSpeed * 2 : animationSpeed;
            hpSlider.value = Mathf.Lerp(hpSlider.value, targetHpValue, Time.deltaTime * currentSpeed);
            
            isAnimatingHp = Mathf.Abs(hpSlider.value - targetHpValue) > 0.001f;
        }

        private void UpdateMpAnimation()
        {
            if (!isAnimatingMp) return;

            bool stillAnimating = false;
            for (int i = 0; i < mpSliders.Length; i++)
            {
                float currentSpeed = mpSliders[i].value > targetMpValues[i] ? animationSpeed * 2 : animationSpeed;
                mpSliders[i].value = Mathf.Lerp(mpSliders[i].value, targetMpValues[i], Time.deltaTime * currentSpeed);
                
                if (Mathf.Abs(mpSliders[i].value - targetMpValues[i]) > 0.001f)
                {
                    stillAnimating = true;
                }
            }
            isAnimatingMp = stillAnimating;
        }
    }
}