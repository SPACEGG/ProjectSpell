using Common.Utils;
using Cysharp.Threading.Tasks;
using Spell.Apis;
using UnityEngine;
using Spell.Model.Data;

namespace Spell.Model.Core
{
    public class SpellDataController
    {
        private readonly WavToTextApi _wavToTextApi = new();
        private readonly TextToSpellApi _textToSpellApi = new();

        public async UniTask<SpellData> BuildSpellDataAsync(AudioClip audioClip, int powerLevel, Vector3 cameraTargetPosition, Vector3 casterPosition)
        {
            // Step 1: Convert audio file to wav
            var wav = await WavUtility.FromAudioClipAsync(audioClip);

            // Step 2: Convert wav to text
            var text = await _wavToTextApi.WavToTextAsync(wav);

            // Step 3: Convert text to spell (GPT JSON응답)
            var spellJson = await _textToSpellApi.TextToSpellAsync(text, powerLevel, cameraTargetPosition, casterPosition);

            // Step 4: Parse JSON to SpellData (예외 처리 포함)
            var spellData = SpellDataFactory.SafeFromJson(spellJson);

            return spellData;
        }
    }
}
