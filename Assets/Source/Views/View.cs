using UnityEngine;
[RequireComponent(typeof(IViewModel))]
public class View : MonoBehaviour
{
    private IViewModel _viewModel;

    private void Awake()
    {
        _viewModel = GetComponent<IViewModel>();
    }

    public void Show()
    {
        gameObject.SetActive(true);
        _viewModel.OnShow();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        _viewModel.OnHide();
    }
}
