using Cysharp.Threading.Tasks;
using Record;
using Spell.Model.Core;
using UnityEngine;
using Spell.Model.Data; // 추가

namespace Spell.Dev.UI
{
    public class DevSpellUIPresenter
    {
        private readonly DevSpellUIView _view;
        private readonly SpellDataController _spellController;
        private readonly RecordController _recordController;

        private AudioClip _recordingClip;
        private SpellData _currentSpellData; // 추가

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

        public async void OnApiRequestButtonClicked(int powerLevel)
        {
            if (_recordingClip == null)
            {
                Debug.LogWarning("녹음된 오디오가 없습니다.");
                return;
            }

            var spellData = await _spellController.BuildSpellDataAsync(_recordingClip, powerLevel);
            if (spellData != null)
            {
                _currentSpellData = spellData; // API 결과 저장
                Debug.Log($"SpellData 생성 성공: {spellData.Name}");
                // 필요하다면 _view에 spellData 전달
            }
            else
            {
                Debug.LogWarning("SpellData 생성 실패");
            }
        }

        public void OnCastSpellButtonClicked()
        {
            var spellData = _currentSpellData ?? SpellDataFactory.Create(); // API 결과 우선 사용
            _view.CastSpellFromView(spellData);
        }
    }
}