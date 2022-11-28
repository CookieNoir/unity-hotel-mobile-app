using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Hotel.UI
{
    public class CalendarDay : MonoBehaviour
    {
        [SerializeField] private TMP_Text _dayText;
        [Space(10)]
        [SerializeField] private MaskableGraphic _maskableGraphic;
        [SerializeField] private Color _defaultColor;
        [SerializeField] private Color _selectedColor;
        private Action<int> _OnSelectAction;
        private int _dayIndex;
        private DateTime _dayValue;

        public void SetValues(int dayIndex, DateTime dayValue, string dayText, Action<int> onSelectAction)
        {
            _dayIndex = dayIndex;
            _dayValue = dayValue;
            _dayText.text = dayText;
            _OnSelectAction = onSelectAction;
        }

        public void OnClick()
        {
            _OnSelectAction(_dayIndex);
        }

        public DateTime Select()
        {
            _maskableGraphic.color = _selectedColor;
            return _dayValue;
        }

        public void Deselect()
        {
            _maskableGraphic.color = _defaultColor;
        }
    }
}