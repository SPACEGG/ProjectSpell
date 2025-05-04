using Cysharp.Threading.Tasks;
using Spell;
using UnityEngine;
using Spell.Model.Core; // 추가

public class PlayerManager : MonoBehaviour
{
    public KeyCode attackKey = KeyCode.Mouse0;
    public KeyCode recordKey = KeyCode.Mouse1;
    public UIController uiController;

    private string spellText;
    private SSpell sSpell;
    private VoiceRecorder voiceRecorder;
    private SpellDataController _spellController;

    private void Start()
    {
        voiceRecorder = new();
        _spellController = new SpellDataController();
    }

    private void Update()
    {
        if (Input.GetKeyDown(attackKey))
        {
            // 기본공격
        }

        if (Input.GetKeyDown(recordKey))
        {
            voiceRecorder.StartRecord();
            uiController.ShowRecordIcon();
            Debug.Log("녹음 시작");
        }

        if (Input.GetKeyUp(recordKey))
        {
            voiceRecorder.StopRecord();
            uiController.HideRecordIcon();
            Debug.Log("녹음 종료. Whisper API 호출...");
            UseSpell().Forget();
        }
    }

    // TODO: 이 함수 이름이 UseSpell이 적절한가? (스킬실행?스펠실행?스펠저장?)
    private async UniTaskVoid UseSpell()
    {
        // spell 생성 (이게 시간이 좀 걸림)
        await _spellController.BuildSpellDataAsync(voiceRecorder.VoiceClip, 1); // powerLevel 기본값 1 전달
        // TODO: spell 생성 후 바로 skill을 실행할 것인가? 아니면 skillReady 플래그 같은걸 두는가?
    }   // yield return StartCoroutine(스킬실행);
} // 클래스 종료
