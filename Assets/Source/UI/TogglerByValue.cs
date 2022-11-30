using UnityEngine;

namespace Hotel.UI
{
    public class TogglerByValue : UIToggler
    {
        [SerializeField] private int _thresholdValue = 0;

        public void ChangeVisibilityWithThreshold(int value)
        {
            SetActive(value >= _thresholdValue);
        }
    }
}