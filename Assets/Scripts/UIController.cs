using UnityEngine;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
{
    private VisualElement recordingIcon;

    private void Start()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        recordingIcon = root.Q<VisualElement>("RecordingIcon");

        recordingIcon.style.display = DisplayStyle.None;
    }

    public void ShowRecordIcon()
    {
        recordingIcon.style.display = DisplayStyle.Flex;
    }
    public void HideRecordIcon()
    {
        recordingIcon.style.display = DisplayStyle.None;
    }
}