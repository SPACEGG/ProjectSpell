using TMPro;
using UnityEngine;
using DG.Tweening;

namespace Gameplay.UI.Damage
{
    public class DamagePopup : MonoBehaviour
    {
        private TextMeshProUGUI _textMesh;
        private Canvas _canvas;
        private float _duration;
        private float _fadeSpeed;
        private Color _textColor;
        private Camera _mainCamera;

        private void Awake()
        {
            _textMesh = GetComponentInChildren<TextMeshProUGUI>();
            _canvas = GetComponent<Canvas>();
            _mainCamera = Camera.main;

            if (_textMesh is null || _canvas is null)
            {
                enabled = false;
                return;
            }

            _canvas.renderMode = RenderMode.WorldSpace;
            _canvas.worldCamera = _mainCamera;
            _canvas.sortingOrder = 32767;
            _canvas.transform.localScale = Vector3.one * 0.01f;

            var rectTransform = GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.sizeDelta = new Vector2(25, 12);
            }

            _textMesh.fontSize = 36;
            _textMesh.alignment = TextAlignmentOptions.Center;
            _textMesh.enableWordWrapping = false;
            _textMesh.overflowMode = TextOverflowModes.Overflow;
            _textMesh.raycastTarget = false;
            _textMesh.fontStyle = FontStyles.Bold;
            _textMesh.outlineWidth = 0.2f;
            _textMesh.outlineColor = new Color(0, 0, 0, 1);
        }

        public void Initialize(string text, Color color, float duration, float moveSpeed, float fadeSpeed)
        {
            _duration = duration;
            _fadeSpeed = fadeSpeed;
            _textColor = color;

            _textMesh.text = text;
            _textMesh.color = color;

            if (_mainCamera != null)
            {
                transform.forward = _mainCamera.transform.forward;
            }
        }

        private void LateUpdate()
        {
            if (_mainCamera is null)
            {
                _mainCamera = Camera.main;
                if (_mainCamera is null) return;
            }

            transform.forward = _mainCamera.transform.forward;

            float distance = Vector3.Distance(transform.position, _mainCamera.transform.position);
            float scale = Mathf.Clamp(distance * 0.005f, 0.002f, 0.02f);
            transform.localScale = new Vector3(scale, scale, scale);
        }

        public void Animate()
        {
            var startPosition = transform.position;
            var targetPosition = startPosition + Vector3.up * 3f;

            var sequence = DOTween.Sequence();

            sequence.Append(transform.DOMove(targetPosition, _duration)
                .SetEase(Ease.OutQuad));

            sequence.Join(_textMesh.DOFade(0f, _duration * _fadeSpeed)
                .SetEase(Ease.InQuad));

            sequence.Join(transform.DOScale(transform.localScale * 1.2f, _duration * 0.2f)
                .SetEase(Ease.OutQuad)
                .SetLoops(2, LoopType.Yoyo));

            sequence.OnComplete(() => Destroy(gameObject));
        }
    }
}