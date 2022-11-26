using System.Collections;
using UnityEngine;

namespace Hotel.UI
{
    public class NotificationHandler : MonoBehaviour
    {
        [SerializeField, Min(0.5f)] private float _animationDuration;
        [SerializeField] private AnimationCurve _animationCurve;
        [SerializeField] private NotificationDisplay _notificationDisplay;
        private IEnumerator _coroutine;

        public void Notify(string message)
        {
            if (_coroutine != null) StopCoroutine(_coroutine);
            _coroutine = _NotifyCoroutine(message);
            StartCoroutine(_coroutine);
        }

        private IEnumerator _NotifyCoroutine(string message)
        {
            _notificationDisplay.Show(message, _animationCurve.Evaluate(0f));
            float _timeSpent = 0f;
            while (_timeSpent < _animationDuration)
            {
                _notificationDisplay.SetAlpha(_animationCurve.Evaluate(_timeSpent / _animationDuration));
                yield return null;
                _timeSpent += Time.deltaTime;
            }
            _notificationDisplay.Hide();
        }
    }
}