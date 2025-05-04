using Common.Utils;
using Cysharp.Threading.Tasks;
using Spell.Apis;
using Spell.Model;
using UnityEngine;

namespace Spell
{
    public class SpellDataController
    {
        private readonly WavToTextApi _wavToTextApi = new();
        private readonly TextToSpellApi _textToSpellApi = new();

        public async UniTask<Model.Spell> BuildSpellAsync(AudioClip audioClip)
        {
            // Step 1: Convert audio file to wav
            var wav = await WavUtility.FromAudioClipAsync(audioClip);

            // Step 2: Convert wav to text
            var text = await _wavToTextApi.WavToTextAsync(wav);

            // Step 3: Convert text to spell
            var spellJson = await _textToSpellApi.TextToSpellAsync(text);

            // TODO: Step 4: Instantiate the spell
            Debug.Log(spellJson);

            return new FailureSpell();
        }
    }
}