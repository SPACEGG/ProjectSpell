using System.Collections;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public KeyCode attackKey = KeyCode.Mouse0;
    public KeyCode recordKey = KeyCode.Mouse1;
    public UIController uiController;

    private string spellText;
    private Spell spell;
    private VoiceRecorder voiceRecorder;

    private void Start()
    {
        voiceRecorder = new();
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
            StartCoroutine(UseSpell());
        }
    }

    // TODO: 이 함수 이름이 UseSpell이 적절한가? (스킬실행?스펠실행?스펠저장?)
    private IEnumerator UseSpell()
    {
        // spell 생성 (이게 시간이 좀 걸림)
        yield return StartCoroutine(VoiceToSpell(voiceRecorder.VoiceClip));
        // TODO: spell 생성 후 바로 skill을 실행할 것인가? 아니면 skillReady 플래그 같은걸 두는가?
        // yield return StartCoroutine(스킬실행);
    }

    private IEnumerator VoiceToSpell(AudioClip clip)
    {
        // 오디오를 텍스트로 변환
        yield return StartCoroutine(VoiceToWhisper.SendToWhisper(clip, (result) =>
        {
            Debug.Log("Whisper returns: " + result);
            spellText = result;
        }));

        // 텍스트를 spell로 변환
        // yield return StartCoroutine(TextToSpell.SendToGPT(spellText, (result) =>
        // {
        //     spell = result;
        // }));
    }
}
