using System.Collections;
using TMPro;
using UnityEngine;

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

            if (_textMesh is null)
            {
                Debug.LogError("TextMeshProUGUI component not found in children!");
                enabled = false;
                return;
            }

            if (_canvas is null)
            {
                Debug.LogError("Canvas component not found!");
                enabled = false;
                return;
            }

            // Set up the canvas
            _canvas.renderMode = RenderMode.WorldSpace;
            _canvas.worldCamera = _mainCamera;
        }

        public void Initialize(string text, Color color, float duration, float moveSpeed, float fadeSpeed)
        {
            _duration = duration;
            _fadeSpeed = fadeSpeed;
            _textColor = color;

            _textMesh.text = text;
            _textMesh.color = color;
        }

        private void LateUpdate()
        {
            if (_mainCamera is null) return;

            transform.forward = _mainCamera.transform.forward;

            float distance = Vector3.Distance(transform.position, _mainCamera.transform.position);

            float scale = distance * 0.01f;
            transform.localScale = new Vector3(scale, scale, scale);
        }

        public IEnumerator Animate()
        {
            float elapsedTime = 0f;
            var startPosition = transform.position;
            var targetPosition = startPosition + Vector3.up * 2f;

            while (elapsedTime < _duration)
            {
                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / _duration;

                transform.position = Vector3.Lerp(startPosition, targetPosition, progress);

                _textColor.a = Mathf.Lerp(1f, 0f, progress * _fadeSpeed);
                _textMesh.color = _textColor;

                yield return null;
            }

            Destroy(gameObject);
        }
    }
}