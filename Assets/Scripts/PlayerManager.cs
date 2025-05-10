using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Spell.Model.Core;
using Common.Models;
using Common.Data;
using Spell.Model.Data;
using Spell.Model.Enums;
using System;

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
    private SpellCaster spellCaster;

    private float recordStartTime;

    private void Awake()
    {
        HealthModel = new HealthModel(healthData);
        ManaModel = new ManaModel(manaData);
    }

    private void Start()
    {
        voiceRecorder = new();
        _spellController = new SpellDataController();

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

        if (Input.GetKeyDown(recordKey))
        {
            voiceRecorder.StartRecord();
            uiController.ShowRecordIcon();
            recordStartTime = Time.time;
            Debug.Log("녹음 시작");
        }

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
        // spell 생성
        SpellData spelldata = await _spellController.BuildSpellDataAsync(
            voiceRecorder.VoiceClip,
            1,
            Camera.main != null ? Camera.main.transform.position : Vector3.zero,
            transform.position // 캐스터 위치 추가
        ); // powerLevel 기본값 1, cameraTargetPosition, casterPosition 추가

        spellCaster.CastSpell(spelldata);

        // 마나 소모
        ManaModel.UseMana(manaCost);

        // TODO: spell 생성 후 바로 skill을 실행할 것인가? 아니면 skillReady 플래그 같은걸 두는가?
    } // yield return StartCoroutine(스킬실행);
}