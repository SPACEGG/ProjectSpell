using Cysharp.Threading.Tasks;
using UnityEngine;
using Spell.Model.Core;
using Spell.Model.Data;
using Record;

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
            spellController = new();
        }

        private void Start()
        {
            spellCaster = GetComponent<SpellCaster>();
            healthManaManager = GetComponent<HealthManaManager>();

            // 파라미터 없이 Create() 호출 시 SpellData의 모든 값이 기본값으로 설정됨
            defaultSpell = SpellDataFactory.Create();
        }

        private void Update()
        {
            // 키 입력 받기
            DefaultAttackKeyInput();
            RecordKeyInput();
            ManaLevelSelectKeyInput();
        }

        #region Key Inputs

        private void DefaultAttackKeyInput()
        {
            if (Input.GetKeyDown(attackKey))
            {
                Debug.Log("Attack key pressed"); // 입력 확인
                if (spellCaster == null)
                {
                    Debug.LogError("SpellCaster is null!");
                }
                else
                {
                    // 카메라 타겟 방향을 계산해서 direction 설정
                    Vector3 cameraTargetPosition = Vector3.zero;
                    if (Camera.main != null)
                    {
                        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
                        int layerMask = ~LayerMask.GetMask("Player"); // "Player" 레이어 무시
                        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
                        {
                            cameraTargetPosition = hit.point;
                        }
                        else
                        {
                            cameraTargetPosition = ray.GetPoint(100f);
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Camera.main is null!");
                    }

                    // direction을 카메라 타겟 방향으로 설정
                    Vector3 direction = (cameraTargetPosition - transform.position).normalized;

                    // === 매번 새로운 SpellData 생성 ===
                    var spell = SpellDataFactory.Create();
                    spell.Direction = direction;

                    Debug.Log("CastSpell 호출");
                    spellCaster.CastSpell(spell, gameObject);
                }
            }
        }

        private void ManaLevelSelectKeyInput()
        {
            if (Input.GetKeyDown(level1SelectKey))
            {
                // TODO: 레벨1 선택 ui
                selectedManaLevel = 1;
            }
            if (Input.GetKeyDown(level2SelectKey))
            {
                // TODO: 레벨2 선택 ui
                selectedManaLevel = 2;
            }
            if (Input.GetKeyDown(level3SelectKey))
            {
                // TODO: 레벨3 선택 ui
                selectedManaLevel = 3;
            }
        }

        private void RecordKeyInput()
        {
            if (Input.GetKeyDown(recordKey))
            {
                Debug.Log("Record key pressed");
                recordController.StartRecording();
                // TODO: uiController.ShowRecordIcon();
                recordStartTime = Time.time;
            }

            if (Input.GetKeyUp(recordKey))
            {
                Debug.Log("Record key released");
                recordController.StopRecording();
                // TODO: uiController.HideRecordIcon();

                if (Time.time - recordStartTime >= recordIgnoreDuration)
                {
                    Debug.Log("Spell() 호출");
                    Spell().Forget();
                }
            }
        }

        #endregion

        private async UniTaskVoid Spell()
        {
            // 마나 소모
            healthManaManager.ManaModel.UseMana(selectedManaLevel);

            Debug.Log("Spell() 진입");
            // direction 계산 및 전달 제거

            // 카메라 타겟 위치 계산 (레이캐스트 사용)
            Vector3 cameraTargetPosition = Vector3.zero;
            if (Camera.main != null)
            {
                Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
                int layerMask = ~LayerMask.GetMask("Player"); // "Player" 레이어 무시
                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
                {
                    cameraTargetPosition = hit.point; // 충돌 지점
                }
                else
                {
                    cameraTargetPosition = ray.GetPoint(100f); // 충돌이 없으면 레이 방향으로 100 유닛 떨어진 지점
                }
            }
            else
            {
                Debug.LogWarning("Camera.main is null!");
            }

            SpellData spelldata = await spellController.BuildSpellDataAsync(
                recordController.GetRecordingClip(),
                selectedManaLevel,
                cameraTargetPosition, // 카메라 타겟 위치 전달
                transform.position    // 시전자 위치 전달
                                      // direction 파라미터 제거
            );

            if (spelldata == null)
            {
                Debug.LogError("SpellData 생성 실패!");
                return;
            }

            Debug.Log("SpellData 생성 성공, CastSpell 호출");
            spellCaster.CastSpell(spelldata, gameObject);
        }
    }
}