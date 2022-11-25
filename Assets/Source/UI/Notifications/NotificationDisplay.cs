using UnityEngine;
using TMPro;

public class NotificationDisplay : MonoBehaviour
{
    [SerializeField] private GameObject _targetObject;
    [SerializeField] private TMP_Text _textField;
    [SerializeField] private CanvasGroup _canvasGroup;

    public void Show(string message, float startAlpha = 1f)
    {
        SetAlpha(startAlpha);
        _textField.text = message;
        _targetObject.SetActive(true);
    }

    public void Hide()
    {
        _targetObject.SetActive(false);
    }

    public void SetAlpha(float value)
    {
        _canvasGroup.alpha = value;
    }
}
