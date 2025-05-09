using Cysharp.Threading.Tasks;
using UnityEngine;
<<<<<<< HEAD
using Spell.Model.Core; // 추가
using Common.Models;
using Common.Data;
=======
using Spell.Model.Core;
using Spell.Model.Data;
using Spell.Model.Enums;
>>>>>>> feat/projectile-behavior

public class PlayerManager : MonoBehaviour
{
    public KeyCode attackKey = KeyCode.Mouse0;
    public KeyCode recordKey = KeyCode.Mouse1;
    public UIController uiController;
    public SpellData defaultSpell;      // TODO: 이거는 나중에 ScriptableObject로 할 것

    [SerializeField] private HealthData healthData;
    public HealthModel HealthModel;
    [SerializeField] private ManaData manaData;
    public ManaModel ManaModel;

    // temp
    public ElementType elementType = ElementType.Fire;
    public ShapeType shapeType = ShapeType.Sphere;
    public Vector3 offset = new(0f, 0f, 2f);
    public float speed = 12f;
    public float scale = 0.15f;
    public int count = 30;
    public float spreadAngle = 30f;
    public float spreadRange = 0f;
    public bool hasGravity = false;

    private VoiceRecorder voiceRecorder;
    private SpellDataController spellController;
    private SpellCaster spellCaster;

    private void Start()
    {
        voiceRecorder = new();
<<<<<<< HEAD
        _spellController = new SpellDataController();

        HealthModel = new HealthModel(healthData);
        ManaModel = new ManaModel(manaData);
=======
        spellController = new SpellDataController();
        spellCaster = GetComponent<SpellCaster>();

        defaultSpell = SpellDataFactory.Create(
            "Default Spell",
            elementType,
            BehaviorType.Projectile,
            null,
            offset,
            Vector3.forward,
            count,
            shapeType,
            Vector3.one * scale,
            hasGravity,
            speed,
            5f
        );
>>>>>>> feat/projectile-behavior
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
            Debug.Log("녹음 시작");
        }

        if (Input.GetKeyUp(recordKey))
        {
            // TODO: 0.5초 미만이면 무시
            voiceRecorder.StopRecord();
            uiController.HideRecordIcon();
            Debug.Log("녹음 종료. Whisper API 호출...");
            UseSpell().Forget();
        }
    }

    private async UniTaskVoid UseSpell()
    {
<<<<<<< HEAD
        // spell 생성 (이게 시간이 좀 걸림)
        await _spellController.BuildSpellDataAsync(
            voiceRecorder.VoiceClip,
            1,
            Camera.main != null ? Camera.main.transform.position : Vector3.zero,
            this.transform.position // 캐스터 위치 추가
        ); // powerLevel 기본값 1, cameraTargetPosition, casterPosition 추가
        // TODO: spell 생성 후 바로 skill을 실행할 것인가? 아니면 skillReady 플래그 같은걸 두는가?
    } // yield return StartCoroutine(스킬실행);
} // 클래스 종료
=======
        SpellData spellData = await spellController.BuildSpellDataAsync(
            voiceRecorder.VoiceClip,
            1,
            Camera.main != null ? Camera.main.transform.position : Vector3.zero,
            transform.position
        );

        spellCaster.CastSpell(spellData);
    }
}
>>>>>>> feat/projectile-behavior
