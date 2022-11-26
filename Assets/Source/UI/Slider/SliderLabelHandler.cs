using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Hotel.UI
{
    public class SliderLabelHandler : MonoBehaviour
    {
        [SerializeField] private Slider _slider;
        [SerializeField] private GameObject _sliderLabelPrefab;
        [SerializeField] private Transform _labelParent;
        private List<SliderLabel> _sliderLabels;
        private SliderLabel _selectedLabel;

        private void Awake()
        {
            _sliderLabels = new List<SliderLabel>();
        }

        private void Start()
        {
            if (!_slider.wholeNumbers)
            {
                Debug.LogError("Slider values should only be allowed to be whole numbers.");
                return;
            }
            Fill();
        }

        public void Fill()
        {
            _Drop();
            int minValue = (int)_slider.minValue;
            int maxValue = (int)_slider.maxValue;
            if (minValue == maxValue) return;

            for (int i = minValue; i <= maxValue; ++i)
            {
                GameObject labelObject = Instantiate(_sliderLabelPrefab, _labelParent);
                SliderLabel label = labelObject.GetComponent<SliderLabel>();
                float normalizedValue = (float)(i - minValue) / (maxValue - minValue);
                label.SetValues(normalizedValue, i.ToString());
                _sliderLabels.Add(label);
            }
            SelectByValue(_slider.value);
        }

        private void _Drop()
        {
            if (_sliderLabels.Count == 0) return;

            foreach (SliderLabel label in _sliderLabels)
            {
                Destroy(label.gameObject);
            }
            _sliderLabels.Clear();
        }

        public void SelectByValue(float value)
        {
            int minValue = (int)_slider.minValue;
            int currentValue = (int)value;
            int index = currentValue - minValue;
            if (_selectedLabel) _selectedLabel.Deselect();
            _sliderLabels[index].Select();
            _selectedLabel = _sliderLabels[index];
        }
    }
}