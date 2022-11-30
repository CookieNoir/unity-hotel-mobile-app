using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Hotel.UI
{
    public class UserBookingStage : MonoBehaviour
    {
        [Serializable]
        public class StringColorPair
        {
            public string stringValue;
            public Color color;
        }

        [SerializeField] private TMP_Text _textField;
        [SerializeField] private MaskableGraphic _maskableGraphic;
        [SerializeField] private StringColorPair[] _pairs;

        public void SetValue(int value)
        {
            _textField.text = _pairs[value].stringValue;
            _maskableGraphic.color = _pairs[value].color;
        }
    }
}