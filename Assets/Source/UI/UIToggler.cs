using UnityEngine;

namespace Hotel.UI
{
    public class UIToggler : MonoBehaviour
    {
        [SerializeField] private GameObject _targetObject;

        public void Enable()
        {
            _targetObject.SetActive(true);
        }

        public void Disable()
        {
            _targetObject.SetActive(false);
        }

        public void Toggle()
        {
            _targetObject.SetActive(!_targetObject.activeSelf);
        }

        public void SetActive(bool value)
        {
            if (value) Enable();
            else Disable();
        }
    }
}