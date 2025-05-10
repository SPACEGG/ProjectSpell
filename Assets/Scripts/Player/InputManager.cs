using Cysharp.Threading.Tasks;
using UnityEngine;
using Spell.Model.Core;
using Spell.Model.Data;
using Spell.Model.Enums;
using Record;
using Common.Models;

namespace Player
{
    // 플레이어의 마우스 클릭을 담당하는 컴포넌트
    public class InputManager : MonoBehaviour
    {
        [SerializeField]
        private KeyCode attackKey = KeyCode.Mouse0;
        [SerializeField]
        private KeyCode recordKey = KeyCode.Mouse1;
        [SerializeField]
        private float recordIgnoreDuration = 0.5f;

        private SpellData defaultSpell;
        private float recordStartTime;

        private RecordController recordController;
        private SpellDataController spellController;
        private SpellCaster spellCaster;
        private HealthManaManager healthManaManager;

        private void Awake()
        {
            recordController = new();
            spellController = new();
        }

        private void Start()
        {
            spellCaster = GetComponent<SpellCaster>();
            healthManaManager = GetComponent<HealthManaManager>();

            defaultSpell = SpellDataFactory.Create(
                "Default Spell",
                ElementType.Earth,
                BehaviorType.Projectile,
                null,
                Vector3.zero,
                Vector3.forward,
                1,
                ShapeType.Sphere,
                Vector3.one,
                true,
                10f,
                5f
            );
        }

        private void Update()
        {
            if (Input.GetKeyDown(attackKey))
            {
                spellCaster.CastSpell(defaultSpell);
            }

            if (Input.GetKeyDown(recordKey))
            {
                recordController.StartRecording();
                // uiController.ShowRecordIcon();
                recordStartTime = Time.time;
            }

            if (Input.GetKeyUp(recordKey))
            {
                recordController.StopRecording();
                // uiController.HideRecordIcon();

                if (Time.time - recordStartTime >= recordIgnoreDuration)
                {
                    UseSpell().Forget();
                }
            }
        }

        // Spell 사용
        private async UniTaskVoid UseSpell()
        {
            SpellData spelldata = await spellController.BuildSpellDataAsync(
                recordController.GetRecordingClip(),
                1,
                Camera.main != null ? Camera.main.transform.position : Vector3.zero,
                transform.position
            );

            // TODO: 마나 소모
            // healthManaManager.ManaModel.UseMana(200f);
            spellCaster.CastSpell(spelldata);
        }
    }
}