using Cysharp.Threading.Tasks;
using Record;
using Spell.Dev.Premades;
using UnityEngine;
using Spell.Model.Core;

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
            _view.PlayRecording(_recordingClip);
        }

        public void OnApiRequestButtonClicked()
        {
            _spellController.BuildSpellDataAsync(_recordingClip).Forget();
        }

        public void OnCastSpellButtonClicked()
        {
            var spellData = SpellDataFactory.Create(); // 올바른 사용 (실제 팩토리/메서드명에 맞게 수정)
            // spellData.Offset 등 필요한 값 세팅

            _view.CastSpellFromView(spellData);
        }
    }
}