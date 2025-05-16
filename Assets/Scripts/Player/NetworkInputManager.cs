using Common.Utils;
using Cysharp.Threading.Tasks;
using Record;
using Spell.Model.Core;
using Unity.Netcode;
using UnityEngine;

namespace Player
{
    public class NetworkInputManager : NetworkBehaviour
    {
        [SerializeField]
        private KeyCode attackKey = KeyCode.Mouse0;
        [SerializeField]
        private KeyCode recordKey = KeyCode.Mouse1;
        [SerializeField]
        private float recordIgnoreDuration = 0.5f;

        private float _recordStartTime;

        private RecordController _recordController;
        private SpellDataController _spellController;
        private NetworkSpellCaster _spellCaster;

        private void Awake()
        {
            _recordController = new();
            _spellController = SpellDataController.Singleton;
        }

        public override void OnNetworkSpawn()
        {
            _spellCaster = GetComponent<NetworkSpellCaster>();
        }

        private void Update()
        {
            if (!IsLocalPlayer) return;

            if (Input.GetKeyDown(attackKey))
            {
                _spellCaster.CastDefaultSpellRpc();
            }

            if (Input.GetKeyDown(recordKey))
            {
                _recordController.StartRecording();
                // uiController.ShowRecordIcon();
                _recordStartTime = Time.time;
            }

            if (Input.GetKeyUp(recordKey))
            {
                _recordController.StopRecording();
                // uiController.HideRecordIcon();

                if (Time.time - _recordStartTime >= recordIgnoreDuration)
                {
                    _ = CastSpell();
                }
            }
        }

        private async UniTaskVoid CastSpell()
        {
            var recordingClip = _recordController.GetRecordingClip();
            var recordingWav = await WavUtility.FromAudioClipAsync(recordingClip);

            // TODO: 마나 소모
            // healthManaManager.ManaModel.UseMana(200f);

            _spellCaster.CastUltimateSpellRpc(
                new CastUltimateSpellRpcArgs
                {
                    Recording = recordingWav
                }
            );
        }
    }
}