using UnityEngine;

public class HideableObject : MonoBehaviour
{
    [SerializeField] private GameObject _targetObject;
    [SerializeField] private int _thresholdValue = 0;

    public void ChangeVisibilityWithThreshold(int value)
    {
        ChangeVisibility(value >= _thresholdValue);
    }

    public void ChangeVisibility(bool value)
    {
        _targetObject.SetActive(value);
    }
}