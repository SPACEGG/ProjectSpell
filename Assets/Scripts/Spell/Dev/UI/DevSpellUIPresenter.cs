using Cysharp.Threading.Tasks;
using Record;
using Spell.Dev.Premades;
using UnityEngine;

namespace Spell.Dev.UI
{
    public class DevSpellUIPresenter
    {
        private readonly DevSpellUIView _view;

        private readonly SpellController _spellController;
        private readonly RecordController _recordController;

        private AudioClip _recordingClip;

        public DevSpellUIPresenter(DevSpellUIView view, SpellController spellController,
            RecordController recordController)
        {
            _view = view;
            _spellController = spellController;
            _recordController = recordController;
        }

        public void OnRecordButtonClicked()
        {
            if (_recordController.IsRecording)
            {
                _recordController.StopRecording();
                _recordingClip = _recordController.GetRecordingClip();
            }
            else
            {
                _recordController.StartRecording();
            }

            _view.ToggleRecordButton(_recordController.IsRecording);
        }

        public void OnPlayButtonClicked()
        {
            _view.PlayRecording(_recordingClip);
        }

        public void OnApiRequestButtonClicked()
        {
            _spellController.BuildSpellAsync(_recordingClip).Forget();
        }

        public void OnCastSpellButtonClicked()
        {
            var spellData = FireballSpellDataFactory.Create();
            var offset = Vector3.zero; // TODO: GPT 응답으로 대체

            _view.CastSpellFromView(spellData, offset);
        }
    }
}