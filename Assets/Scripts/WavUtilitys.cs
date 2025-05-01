using System;
using System.IO;
using UnityEngine;

public static class WavUtility
{
    /* WAV는 고음질 무압축 디지털 오디오 파일 포맷이다. 주로 원본에 가까운 사운드를 저장할 때 쓴다.
    */

    // 오디오 클립 -> WAV Byte[] 변환 함수
    // 오디오클립을 받아서 WAV 파일 구조에 맞춰 바이트 스트림을 만들고, 최종적으로 byte[]를 반환한다. 
    public static byte[] FromAudioClip(AudioClip clip)
    {
        // 널 체크
        if (clip == null) 
            throw new ArgumentNullException("clip");
        // 클립의 샘플과 채널을 곱해 WAV의 배열 크기를 계산
        var samples = new float[clip.samples * clip.channels];
        // GetData로 PCM 샘플(-1.0 ~ +1.0)을 samples에 채움
        // 소리의 진폭이 float 값으로 저장
        clip.GetData(samples, 0);

        byte[] wav;  // WAV를 담을 바이트스트림 선언
        using (var stream = new MemoryStream())
        {
            // RIFF header
            WriteString(stream, "RIFF");
            WriteInt(stream, 0); // placeholder for file size
            WriteString(stream, "WAVE");

            // fmt chunk
            WriteString(stream, "fmt ");
            WriteInt(stream, 16);                       // chunk size
            WriteShort(stream, 1);                       // audio format = PCM
            WriteShort(stream, (short)clip.channels);
            WriteInt(stream, clip.frequency);
            WriteInt(stream, clip.frequency * clip.channels * 2); // byte rate
            WriteShort(stream, (short)(clip.channels * 2));      // block align
            WriteShort(stream, 16);                       // bits per sample

            // data chunk
            WriteString(stream, "data");
            WriteInt(stream, samples.Length * 2);

            // write samples
            foreach (var sample in samples)
            {
                short intSample = (short)(Mathf.Clamp(sample, -1f, 1f) * short.MaxValue);
                WriteShort(stream, intSample);
            }

            // finalize file length
            stream.Seek(4, SeekOrigin.Begin);
            WriteInt(stream, (int)stream.Length - 8);
            wav = stream.ToArray();
        }

        return wav;
    }

    // WAV 헤더/데이터 작성용 헬퍼 메소드드
    static void WriteString(Stream s, string str)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(str);
        s.Write(bytes, 0, bytes.Length);
    }

    static void WriteInt(Stream s, int value)
    {
        var bytes = BitConverter.GetBytes(value);
        s.Write(bytes, 0, 4);
    }

    static void WriteShort(Stream s, short value)
    {
        var bytes = BitConverter.GetBytes(value);
        s.Write(bytes, 0, 2);
    }
}
