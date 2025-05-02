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

        private DevSpellUIPresenter _presenter;

        public void Initialize(DevSpellUIPresenter presenter)
        {
            _presenter = presenter;

            recordButton.onClick.AddListener(_presenter.OnRecordButtonClicked);
            playButton.onClick.AddListener(_presenter.OnPlayButtonClicked);
            apiRequestButton.onClick.AddListener(_presenter.OnApiRequestButtonClicked);
        }

        public void ToggleRecordButton(bool isRecording)
        {
            recordButtonLabel.text = isRecording ? "Stop" : "Record";
        }

        public void PlayRecording(AudioClip recordingClip)
        {
            var audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.PlayOneShot(recordingClip);
        }
    }
}