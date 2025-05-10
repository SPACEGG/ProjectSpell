using System;
using UnityEngine;

[Obsolete("이거 말고 RecordController을 쓰세요")]
public class VoiceRecorder
{

    public AudioClip VoiceClip { get; private set; }

    private int maxRecordDuration = 10;
    private int recordFreq = 44100;

    public VoiceRecorder()
    {
        if (Microphone.devices.Length == 0)
        {
            Debug.LogError("마이크 장치를 찾을 수 없습니다.");
            return;
        }
    }

    public void StartRecord()
    {
        VoiceClip = Microphone.Start(null, false, maxRecordDuration, recordFreq);
    }
    public void StopRecord()
    {
        Microphone.End(null);
    }
}