using Cysharp.Threading.Tasks;
using Entity;
using Spell;
using UnityEngine;
using Spell.Model.Core; // 추가
using Spell.Model.Data; // 추가

public class PlayerManager : MonoBehaviour
{
    public KeyCode attackKey = KeyCode.Mouse0;
    public KeyCode recordKey = KeyCode.Mouse1;
    public UIController uiController;

    // temp
    public Spell.Model.Enums.ElementType elementType = Spell.Model.Enums.ElementType.Fire;
    public Spell.Model.Enums.ShapeType shapeType = Spell.Model.Enums.ShapeType.Sphere;
    public Vector3 offset = new(0f, 0f, 2f);
    public float speed = 12f;
    public float scale = 0.15f;
    public int count = 30;
    public float spreadAngle = 30f;
    public float spreadRange = 0f;
    public bool hasGravity = false;

    private VoiceRecorder voiceRecorder;
    private SpellDataController _spellController;
    [SerializeField] private SpellCaster spellCaster; // SpellCaster 연결

    private void Start()
    {
        voiceRecorder = new();
        _spellController = new SpellDataController();
    }

    private void Update()
    {
        
        if (Input.GetKeyDown(attackKey))
        {
            // FIXME: 기본공격
            GameObject projectileEntity = new GameObject("ProjectileEntity");
            projectileEntity.transform.SetPositionAndRotation(Camera.main.transform.position, Camera.main.transform.rotation);
            projectileEntity.AddComponent<ProjectileEntity>()
                .SetElementType(elementType)
                .SetShapeType(shapeType)
                .SetOffset(offset)
                .SetSpeed(speed)
                .SetScale(scale)
                .SetCount(count)
                .SetSpreadAngle(spreadAngle)
                .SetSpreadRange(spreadRange)
                .SetGravity(hasGravity);
        }

        // 마우스 우클릭(녹음 시작)
        if (Input.GetKeyDown(recordKey))
        {
            voiceRecorder.StartRecord();
            uiController.ShowRecordIcon();
            Debug.Log("녹음 시작");
        }

        // 마우스 우클릭 해제(녹음 종료 및 주문 생성/시전)
        if (Input.GetKeyUp(recordKey))
        {
            voiceRecorder.StopRecord();
            uiController.HideRecordIcon();
            Debug.Log("녹음 종료. Whisper API 호출...");
            UseSpell().Forget();
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
