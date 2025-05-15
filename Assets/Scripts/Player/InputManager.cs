using System.Collections.Generic;
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
        // 조작 키 세팅
        [SerializeField] private KeyCode attackKey = KeyCode.Mouse0;
        [SerializeField] private KeyCode recordKey = KeyCode.Mouse1;
        [SerializeField] private KeyCode level1SelectKey = KeyCode.Alpha1;
        [SerializeField] private KeyCode level2SelectKey = KeyCode.Alpha2;
        [SerializeField] private KeyCode level3SelectKey = KeyCode.Alpha3;

        // 녹음키 무시 딜레이
        [SerializeField] private float recordIgnoreDuration = 0.5f;

        // 기본공격 데미지, 마나충전량
        [SerializeField] private float defaultAttackDamage = 10f;
        [SerializeField] private float defaultAttackManaRegen = 30f;

        private SpellData defaultSpell;
        private float recordStartTime;

        private RecordController recordController;
        private SpellDataController spellController;
        private SpellCaster spellCaster;
        private HealthManaManager healthManaManager;
        private int selectedManaLevel = 1;

        private void Awake()
        {
            recordController = new();
            spellController = SpellDataController.Singleton;
        }

        private void Start()
        {
            spellCaster = GetComponent<SpellCaster>();
            healthManaManager = GetComponent<HealthManaManager>();

            // 좌클릭시 나갈 기본공격 SpellData
            defaultSpell = SpellDataFactory.Create(
                "Default Spell",
                ElementType.Earth,
                BehaviorType.Projectile,
                new List<SpellActionData>() {
                    new(ActionType.Damage, TargetType.Enemy, defaultAttackDamage),
                    new(ActionType.ManaRegen, TargetType.Self, defaultAttackManaRegen),
                },
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
            // 키 입력 받기
            DefaultAttackKeyInput();
            RecordKeyInput();
            ManaLevelSelectKeyInput();
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

            // 선택된 레벨만큼 마나 소모
            healthManaManager.ManaModel.UseMana(selectedManaLevel);
            spellCaster.CastSpell(spelldata);
        }

        #region Key Inputs

        private void DefaultAttackKeyInput()
        {
            if (Input.GetKeyDown(attackKey))
            {
                spellCaster.CastSpell(defaultSpell);
            }
        }

        private void RecordKeyInput()
        {
            if (Input.GetKeyDown(recordKey))
            {
                recordController.StartRecording();
                // TODO: uiController.ShowRecordIcon();
                recordStartTime = Time.time;
            }

            if (Input.GetKeyUp(recordKey))
            {
                recordController.StopRecording();
                // TODO: uiController.HideRecordIcon();

                if (Time.time - recordStartTime >= recordIgnoreDuration)
                {
                    UseSpell().Forget();
                }
            }
        }

        private void ManaLevelSelectKeyInput()
        {
            if (Input.GetKeyDown(level1SelectKey))
            {
                selectedManaLevel = 1;
                // TODO: 선택된 마나 레벨 HUD에 표시
            }
            if (Input.GetKeyDown(level2SelectKey))
            {
                selectedManaLevel = 2;
                // TODO: 선택된 마나 레벨 HUD에 표시
            }
            if (Input.GetKeyDown(level3SelectKey))
            {
                selectedManaLevel = 3;
                // TODO: 선택된 마나 레벨 HUD에 표시
            }
        }

        #endregion
    }
}