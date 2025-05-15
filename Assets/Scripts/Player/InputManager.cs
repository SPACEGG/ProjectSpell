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

            // 파라미터 없이 Create() 호출 시 SpellData의 모든 값이 기본값으로 설정됨
            defaultSpell = SpellDataFactory.Create();
        }

        private void Update()
        {
            if (Input.GetKeyDown(attackKey))
            {
                Debug.Log("Attack key pressed"); // 입력 확인
                if (spellCaster == null)
                {
                    Debug.LogError("SpellCaster is null!");
                }
                else if (defaultSpell == null)
                {
                    Debug.LogError("defaultSpell is null!");
                }
                else
                {
                    // 카메라 타겟 방향을 계산해서 defaultSpell.Direction에 반영
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
                    defaultSpell.Direction = direction;

                    Debug.Log("CastSpell 호출");
                    spellCaster.CastSpell(defaultSpell);
                }
            }

            if (Input.GetKeyDown(recordKey))
            {
                Debug.Log("Record key pressed");
                recordController.StartRecording();
                // uiController.ShowRecordIcon();
                recordStartTime = Time.time;
            }

            if (Input.GetKeyUp(recordKey))
            {
                Debug.Log("Record key released");
                recordController.StopRecording();
                // uiController.HideRecordIcon();

                if (Time.time - recordStartTime >= recordIgnoreDuration)
                {
                    Debug.Log("Spell() 호출");
                    Spell().Forget();
                }
            }
        }

        // Spell 사용
        private async UniTaskVoid Spell()
        {
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
                1,
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
            spellCaster.CastSpell(spelldata);
        }
    }
}