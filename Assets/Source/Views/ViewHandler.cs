using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Hotel.Views
{
    public class ViewHandler : MonoBehaviour
    {
        [SerializeField] private View _defaultView;
        [SerializeField] private UnityEvent<int> _OnViewStackChanged;
        private Stack<View> _views = new Stack<View>();

        private void Start()
        {
            OpenView(_defaultView);
        }

        public void OpenView(View view)
        {
            if (_views.Count > 0)
            {
                View openedView = _views.Peek();
                openedView.Hide();
            }
            _views.Push(view);
            view.Show();
            _OnViewStackChanged.Invoke(_views.Count);
        }

        public void MoveBack()
        {
            if (_views.Count <= 1) return;

            View openedView = _views.Pop();
            openedView.Hide();
            View previousView = _views.Peek();
            previousView.Show();
            _OnViewStackChanged.Invoke(_views.Count);
        }

        public void MoveBackTo(View view)
        {
            View openedView = _views.Peek();
            if (openedView == view || !_views.Contains(view)) return;

            openedView.Hide();
            while (_views.Peek() != view)
            {
                _views.Pop();
            }
            view.Show();
            _OnViewStackChanged.Invoke(_views.Count);
        }
    }
}