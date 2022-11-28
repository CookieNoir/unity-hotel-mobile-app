using UnityEngine;
using UnityEngine.UI;

namespace Hotel.UI
{
    public class BookingStageButton : MonoBehaviour
    {
        [SerializeField] private MaskableGraphic _maskableGraphic;
        [SerializeField] private Color _defaultColor;
        [SerializeField] private Color _selectedColor;

        public void Select()
        {
            _maskableGraphic.color = _selectedColor;
        }

        public void Deselect()
        {
            _maskableGraphic.color = _defaultColor;
        }
    }
}