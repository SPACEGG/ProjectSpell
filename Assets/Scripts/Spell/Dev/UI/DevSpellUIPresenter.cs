using Cysharp.Threading.Tasks;
using Record;
using Spell.Model.Core;
using UnityEngine;

namespace Spell.Dev.UI
{
    public class DevSpellUIPresenter
    {
        private readonly DevSpellUIView _view;
        private readonly SpellDataController _spellController;
        private readonly RecordController _recordController;

        private AudioClip _recordingClip;

        public DevSpellUIPresenter(DevSpellUIView view, SpellDataController spellController,
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
            if (_recordingClip != null)
                _view.PlayRecording(_recordingClip);
        }

        public void OnApiRequestButtonClicked()
        {
            if (_recordingClip != null)
                _spellController.BuildSpellDataAsync(_recordingClip).Forget();
        }

        public void OnCastSpellButtonClicked()
        {
            var spellData = SpellDataFactory.Create();
            // spellData.Offset 등 필요한 값 세팅

            _view.CastSpellFromView(spellData);
        }
    }
}