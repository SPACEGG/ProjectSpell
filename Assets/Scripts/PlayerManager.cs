using Cysharp.Threading.Tasks;
using UnityEngine;
using Spell.Model.Core;
using Spell.Model.Data;
using Spell.Model.Enums;

public class PlayerManager : MonoBehaviour
{
    public KeyCode attackKey = KeyCode.Mouse0;
    public KeyCode recordKey = KeyCode.Mouse1;
    public UIController uiController;
    public SpellData defaultSpell;      // TODO: 이거는 나중에 ScriptableObject로 할 것

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
    }

    private void Update()
    {
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
        SpellData spellData = await spellController.BuildSpellDataAsync(
            voiceRecorder.VoiceClip,
            1,
            Camera.main != null ? Camera.main.transform.position : Vector3.zero,
            transform.position
        );

        spellCaster.CastSpell(spellData);
    }
}
