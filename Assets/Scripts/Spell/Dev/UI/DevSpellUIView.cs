using Spell.Model.Core;
using Spell.Model.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Spell.Dev.UI
{
    public class DevSpellUIView : MonoBehaviour
    {
        [SerializeField] private Button recordButton;
        [SerializeField] private TMP_Text recordButtonLabel;
        [SerializeField] private Button playButton;
        [SerializeField] private Button apiRequestButton;
        [SerializeField] private Button castSpellButton;
        [SerializeField] private SpellCaster spellCaster;
        [SerializeField] private Button level1Button;
        [SerializeField] private Button level2Button;
        [SerializeField] private Button level3Button;

        private int _selectedPowerLevel = 1;

        private DevSpellUIPresenter _presenter;

        private void Awake()
        {
            if (GetComponent<AudioSource>() == null)
                gameObject.AddComponent<AudioSource>();
        }

        public void Initialize(DevSpellUIPresenter presenter)
        {
            _presenter = presenter;

            recordButton.onClick.AddListener(_presenter.OnRecordButtonClicked);
            playButton.onClick.AddListener(_presenter.OnPlayButtonClicked);
            apiRequestButton.onClick.AddListener(() => _presenter.OnApiRequestButtonClicked(_selectedPowerLevel).Forget());
            castSpellButton.onClick.AddListener(_presenter.OnCastSpellButtonClicked);

            level1Button.onClick.AddListener(() => SetPowerLevel(1));
            level2Button.onClick.AddListener(() => SetPowerLevel(2));
            level3Button.onClick.AddListener(() => SetPowerLevel(3));
        }

        private void SetPowerLevel(int powerLevel)
        {
            _selectedPowerLevel = powerLevel;
            Debug.Log($"선택된 파워레벨: {powerLevel}");

            // 버튼 시각적 강조 (예시: 선택된 버튼만 interactable=false)
            level1Button.interactable = powerLevel != 1;
            level2Button.interactable = powerLevel != 2;
            level3Button.interactable = powerLevel != 3;

        }

        public void ToggleRecordButton(bool isRecording)
        {
            if (recordButtonLabel != null)
                recordButtonLabel.text = isRecording ? "Stop" : "Record";
        }

        public void PlayRecording(AudioClip recordingClip)
        {
            if (recordingClip == null) return;
            var audioSource = GetComponent<AudioSource>();
            audioSource.PlayOneShot(recordingClip);
        }

        public void CastSpellFromView(SpellData spellData)
        {
            if (spellCaster != null)
            {
                spellCaster.CastSpell(spellData, gameObject);
            }
            else
            {
                Debug.LogWarning("SpellCaster is not assigned.");
            }
        }
    }
}