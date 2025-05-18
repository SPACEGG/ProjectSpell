using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI.PlayerHud
{
    public class PlayerHudView : MonoBehaviour
    {
        [Header("Health")]
        [SerializeField] private Slider hpSlider;
        [SerializeField] private TextMeshProUGUI hpText;

        [Header("Mana")]
        [SerializeField] private Slider[] mpSliders;
        [SerializeField] private Image[] mpBorders;
        [SerializeField] private float animationSpeed = 8f;

        [Header("Spells")]
        [SerializeField] private TextMeshProUGUI spellLevelIndicator;

        private bool _isAnimatingHp;
        private float _targetHpValue;
        private bool _isAnimatingMp;
        private float[] _targetMpValues;

        private void Awake()
        {
            _targetMpValues = new float[mpSliders.Length];
        }

        private void Update()
        {
            UpdateHpAnimation();
            UpdateMpAnimation();
        }

        public void SetHp(float current, float max)
        {
            _targetHpValue = current / max;
            hpText.text = $"{current} / {max}";
            _isAnimatingHp = true;
        }

        public void SetMp(float currentMana, float manaPerLevel)
        {
            var filledManaLevelCount = (int)Math.Truncate(currentMana / manaPerLevel);
            var manaPercentageInLastLevel = currentMana % manaPerLevel;

            for (var i = 0; i < mpSliders.Length; i++)
            {
                if (i < filledManaLevelCount)
                {
                    _targetMpValues[i] = 1f;
                    mpBorders[i].enabled = true;
                }
                else if (i == filledManaLevelCount)
                {
                    _targetMpValues[i] = manaPercentageInLastLevel / manaPerLevel;
                    mpBorders[i].enabled = false;
                }
                else
                {
                    _targetMpValues[i] = 0f;
                    mpBorders[i].enabled = false;
                }
            }

            _isAnimatingMp = true;
        }

        public void SetSpellLevel(int level)
        {
            spellLevelIndicator.text = level.ToString();
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void UpdateHpAnimation()
        {
            if (!_isAnimatingHp) return;

            float currentSpeed = hpSlider.value > _targetHpValue ? animationSpeed * 2 : animationSpeed;
            hpSlider.value = Mathf.Lerp(hpSlider.value, _targetHpValue, Time.deltaTime * currentSpeed);

            _isAnimatingHp = Mathf.Abs(hpSlider.value - _targetHpValue) > 0.001f;
        }

        private void UpdateMpAnimation()
        {
            if (!_isAnimatingMp) return;

            bool stillAnimating = false;
            for (int i = 0; i < mpSliders.Length; i++)
            {
                float currentSpeed = mpSliders[i].value > _targetMpValues[i] ? animationSpeed * 2 : animationSpeed;
                mpSliders[i].value = Mathf.Lerp(mpSliders[i].value, _targetMpValues[i], Time.deltaTime * currentSpeed);

                if (Mathf.Abs(mpSliders[i].value - _targetMpValues[i]) > 0.001f)
                {
                    stillAnimating = true;
                }
            }

            _isAnimatingMp = stillAnimating;
        }
    }
}