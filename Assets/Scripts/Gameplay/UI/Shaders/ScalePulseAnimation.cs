using DG.Tweening;
using UnityEngine;

namespace Gameplay.UI.Shaders
{
    public class ScalePulseAnimation : MonoBehaviour
    {
        [Header("Scale Settings")]
        [SerializeField] private float scaleDuration = 1f;
        [SerializeField] private float minScale = 1f;
        [SerializeField] private float maxScale = 2f;
        [SerializeField] private Ease scaleEaseType = Ease.InOutSine;

        [Header("Rotation Settings")]
        [SerializeField] private float rotationDuration = 0.5f;
        [SerializeField] private float rotationAngle = 30f;
        [SerializeField] private Ease rotationEaseType = Ease.InOutSine;

        private void Start()
        {
            StartAnimations();
        }

        private void StartAnimations()
        {
            // Scale animation
            transform.DOScale(maxScale, scaleDuration)
                .SetEase(scaleEaseType)
                .SetLoops(-1, LoopType.Yoyo);

            // Rotation animation
            Sequence rotationSequence = DOTween.Sequence();
            rotationSequence.Append(transform.DORotate(new Vector3(0, 0, rotationAngle), rotationDuration, RotateMode.LocalAxisAdd).SetEase(rotationEaseType))
                           .Append(transform.DORotate(new Vector3(0, 0, -rotationAngle * 2), rotationDuration * 2, RotateMode.LocalAxisAdd).SetEase(rotationEaseType))
                           .Append(transform.DORotate(new Vector3(0, 0, rotationAngle), rotationDuration, RotateMode.LocalAxisAdd).SetEase(rotationEaseType))
                           .SetLoops(-1);
        }

        private void OnDestroy()
        {
            transform.DOKill();
        }
    }
}
