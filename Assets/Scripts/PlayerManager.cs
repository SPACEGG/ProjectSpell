/*
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Common.Models;
using Spell.Model.Enums;
using Spell.Model.Data;
using Common.Data;
using Spell.Model.Core;

[Obsolete("이거는 너무 복잡해서 안씁니다. Scripts/Player에 있는 컴포넌트들을 대신 쓰세요.")]

public class PlayerManager : MonoBehaviour
{
    public KeyCode attackKey = KeyCode.Mouse0;
    public KeyCode recordKey = KeyCode.Mouse1;
    public UIController uiController;
    public SpellData defaultSpell;
    public float recordIgnoreDuration = 0.5f;

    [SerializeField] private HealthData healthData;
    public HealthModel HealthModel;
    [SerializeField] private ManaData manaData;
    public ManaModel ManaModel;

    // temp
    public ElementType elementType = ElementType.Fire;
    public ShapeType shapeType = ShapeType.Sphere;
    public Vector3 offset = Vector3.zero;
    public float speed = 12f;
    public float scale = 0.15f;
    public int count = 30;
    public float spreadAngle = 30f;
    public float spreadRange = 0f;
    public bool hasGravity = false;
    public float manaCost = 200f;

    private VoiceRecorder voiceRecorder;
    private SpellDataController _spellController;
    [SerializeField] private SpellCaster spellCaster; // SpellCaster 연결

    private float recordStartTime;

    private void Awake()
    {
        HealthModel = new HealthModel(healthData);
        ManaModel = new ManaModel(manaData);
    }

    private void Start()
    {
        voiceRecorder = new();
        _spellController = SpellDataController.Singleton;

        spellCaster = GetComponent<SpellCaster>();

        defaultSpell = SpellDataFactory.Create(
            "Default Spell",
            elementType,
            BehaviorType.Projectile,
            new List<SpellActionData> { new(ActionType.Damage, TargetType.Enemy, 10f) },
            offset,
            Vector3.forward,
            count,
            shapeType,
            Vector3.one * scale,
            hasGravity,
            speed,
            5f
        );
    }

    private void Update()
    {

        // Regenerate mana
        ManaModel.RegenerateMana(Time.deltaTime);

        if (Input.GetKeyDown(attackKey))
        {
            // 기본공격
            spellCaster.CastSpell(defaultSpell);
        }

        // 마우스 우클릭(녹음 시작)
        if (Input.GetKeyDown(recordKey))
        {
            voiceRecorder.StartRecord();
            uiController.ShowRecordIcon();
            recordStartTime = Time.time;
            Debug.Log("녹음 시작");
        }

        // 마우스 우클릭 해제(녹음 종료 및 주문 생성/시전)
        if (Input.GetKeyUp(recordKey))
        {
            voiceRecorder.StopRecord();
            uiController.HideRecordIcon();
            Debug.Log("녹음 종료");

            if (Time.time - recordStartTime >= recordIgnoreDuration)
            {
                Debug.Log("Whisper API 호출...");
                UseSpell().Forget();
            }
        }
    }

    private async UniTaskVoid UseSpell()
    {
        Vector3 cameraTargetPosition = CameraUtil.GetCameraTargetPosition();
        Vector3 direction = CameraUtil.GetCameraForward();

        var spellData = await _spellController.BuildSpellDataAsync(
            voiceRecorder.VoiceClip,
            1,
            cameraTargetPosition,
            this.transform.position,
            direction
        );
        // spell 생성 후 바로 시전
        if (spellData != null && spellCaster != null)
        {
            spellCaster.CastSpell(spellData);
            Debug.Log($"플레이어가 주문 시전: {spellData.Name}");
        }
        else
        {
            Debug.LogWarning("SpellData 생성 실패 또는 SpellCaster 미할당");
        }
    }
} // 클래스 종료
*/