using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Hotel.UI
{
    public class SliderLabel : MonoBehaviour
    {
        [SerializeField] private RectTransform _transform;
        [SerializeField] private TMP_Text _labelText;
        [Space(10)]
        [SerializeField] private MaskableGraphic _maskableGraphic;
        [SerializeField] private Color _defaultLabelColor;
        [SerializeField] private Vector3 _defaultScale;
        [SerializeField] private Color _selectedLabelColor;
        [SerializeField] private Vector3 _selectedScale;
        [SerializeField, Min(0.5f)] private float _animationDuration;
        private IEnumerator _coroutine;
        private float _animationValue;

        public void SetValues(float normalizedValue, string textValue)
        {
            _transform.anchorMin = new Vector2(normalizedValue, _transform.anchorMin.y);
            _transform.anchorMax = new Vector2(normalizedValue, _transform.anchorMax.y);
            _labelText.text = textValue;
            DeselectImmediately();
        }

        public void SelectImmediately()
        {
            _maskableGraphic.color = _selectedLabelColor;
            _maskableGraphic.transform.localScale = _selectedScale;
            _animationValue = 1f;
        }

        public void DeselectImmediately()
        {
            _maskableGraphic.color = _defaultLabelColor;
            _maskableGraphic.transform.localScale = _defaultScale;
            _animationValue = 0f;
        }

        public void Select()
        {
            _StartCoroutine(_SelectCoroutine());
        }

        public void Deselect()
        {
            _StartCoroutine(_DeselectCoroutine());
        }

        private void _StartCoroutine(IEnumerator routine)
        {
            if (_coroutine != null) StopCoroutine(_coroutine);
            _coroutine = routine;
            StartCoroutine(_coroutine);
        }

        private void _SetMaskableGraphicValues(float t)
        {
            _maskableGraphic.color = Color.Lerp(_defaultLabelColor, _selectedLabelColor, t);
            _maskableGraphic.transform.localScale = Vector3.Lerp(_defaultScale, _selectedScale, t);
        }

        private IEnumerator _SelectCoroutine()
        {
            while (_animationValue < 1f)
            {
                _SetMaskableGraphicValues(_animationValue);
                yield return null;
                _animationValue += Time.deltaTime / _animationDuration;
            }
            SelectImmediately();
        }

        private IEnumerator _DeselectCoroutine()
        {
            while (_animationValue > 0f)
            {
                _SetMaskableGraphicValues(_animationValue);
                yield return null;
                _animationValue -= Time.deltaTime / _animationDuration;
            }
            DeselectImmediately();
        }
    }
}