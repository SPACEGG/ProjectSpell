using Common.Models;
using Common.Utils;
using Cysharp.Threading.Tasks;
using Spell.Apis;
using UnityEngine;
using Spell.Model.Data;

namespace Spell.Model.Core
{
    public class SpellDataController
    {
        private static SpellDataController _instance;
        public static SpellDataController Singleton => _instance ??= new SpellDataController();

        private readonly WavToTextApi _wavToTextApi = new();
        private readonly TextToSpellApi _textToSpellApi = new();

        private SpellDataController()
        {
        }

        public async UniTask<SpellData> BuildSpellDataAsync(AudioClip audioClip, int powerLevel, Vector3 cameraTargetPosition,
            Vector3 casterPosition)
        {
            // 카메라 타겟 포지션과 캐스터 포지션 출력
            Debug.Log($"[SpellDataController] cameraTargetPosition: {cameraTargetPosition}, casterPosition: {casterPosition}");

            // Step 1: Convert audio file to wav
            var wav = await WavUtility.FromAudioClipAsync(audioClip);

            return await BuildSpellDataAsyncByWav(wav, powerLevel, cameraTargetPosition, casterPosition);
        }

        public async UniTask<SpellData> BuildSpellDataAsyncByWav(Wav wav, int powerLevel, Vector3 cameraTargetPosition,
            Vector3 casterPosition)
        {
            // Step 2: Convert wav to text
            var text = await _wavToTextApi.WavToTextAsync(wav);

            // 예시: maxMana, maxHealth 값을 준비
            float maxMana = 300f;    // 실제 최대 마나 값으로 교체 필요
            float maxHealth = 500f;  // 실제 최대 체력 값으로 교체 필요

            // Step 3: Convert text to spell (GPT JSON응답)
            var spellJson = await _textToSpellApi.TextToSpellAsync(
                text,
                powerLevel,
                cameraTargetPosition,
                casterPosition,
                maxMana,         // 추가
                maxHealth        // 추가
            );

            // GPT 응답 본문(JSON) 실제로 출력
            Debug.Log(spellJson);

            // Step 4: Parse JSON to SpellData (예외 처리 포함)
            var spellData = SpellDataFactory.SafeFromJson(spellJson);

            return spellData; // Direction 설정 코드 제거
        }
    }
}