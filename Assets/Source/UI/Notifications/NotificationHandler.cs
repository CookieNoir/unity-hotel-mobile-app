using System.Collections;
using UnityEngine;

public class NotificationHandler : MonoBehaviour
{
    [SerializeField, Min(0.5f)] private float _animationLength;
    [SerializeField] private AnimationCurve _animationCurve;
    [SerializeField] private NotificationDisplay _notificationDisplay;
    private IEnumerator _coroutine;

    private void Awake()
    {
        _coroutine = _NotifyCoroutine(null);
    }

    public void Notify(string message)
    {
        StopCoroutine(_coroutine);
        _coroutine = _NotifyCoroutine(message);
        StartCoroutine(_coroutine);
    }

    private IEnumerator _NotifyCoroutine(string message)
    {
        _notificationDisplay.Show(message, _animationCurve.Evaluate(0f));
        float _timeSpent = 0f;
        while (_timeSpent < _animationLength)
        {
            yield return null;
            _timeSpent += Time.deltaTime;
            _notificationDisplay.SetAlpha(_animationCurve.Evaluate(_timeSpent / _animationLength));
        }
        _notificationDisplay.Hide();
    }
}
