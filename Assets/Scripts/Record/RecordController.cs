using Common.Models;
using UnityEngine;

namespace Record
{
    public class RecordController
    {
        public bool IsRecording { get; private set; }

        private AudioClip _recordingClip;

        public void StartRecording()
        {
            IsRecording = true;

            if (Microphone.devices.Length == 0)
            {
                Debug.LogError("No microphone found.");
                return;
            }

            _recordingClip = Microphone.Start(Microphone.devices[0], true, 10, 44100);
        }

        public void StopRecording()
        {
            if (!IsRecording) return;

            IsRecording = false;
            Microphone.End(Microphone.devices[0]);
        }

        public AudioClip GetRecordingClip()
        {
            StopRecording();

            return _recordingClip;
        }
    }
}