using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class AddRoomViewModel : MonoBehaviour, IViewModel
{
    [SerializeField] private ClientHandler _clientHandler;
    [Space(10)]
    [SerializeField] private TMP_InputField _bedsNumberInputField;
    [SerializeField] private TMP_InputField _priceInputField;
    [SerializeField] private TMP_InputField _imagePathInputField;
    [SerializeField] private TMP_Text _errorTextField;
    [Space(10)]
    [SerializeField] private NotificationHandler _notificationHandler;
    [SerializeField] private UnityEvent _OnSuccess;

    private static readonly Dictionary<string, string> _messages = new Dictionary<string, string>()
    {
        ["SUCCESS"] = "Комната успешно добавлена",
        ["NO_CONNECTION"] = "Соединение с сервером не установлено. Попробуйте еще раз",
        ["NOT_VALID_BEDS_NUMBER"] = "Поле \"макс. гостей\" заполнено некорректно. Убедитесь, что ввели в этом поле натуральное число",
        ["NOT_VALID_PRICE"] = "Поле \"цена за сутки\" заполнено некорректно. Убедитесь, что ввели в этом поле натуральное число",
    };

    public void OnShow()
    {
        _bedsNumberInputField.text = "";
        _priceInputField.text = "";
        _imagePathInputField.text = "";
        _errorTextField.text = "";
    }

    public void OnHide()
    {

    }

    public async void Submit()
    {
        int bedsNumber;
        try
        {
            bedsNumber = Convert.ToInt32(_bedsNumberInputField.text);
            if (!DataValidation.IsNaturalNumber(bedsNumber)) throw new Exception();
        }
        catch
        {
            _errorTextField.text = _messages["NOT_VALID_BEDS_NUMBER"];
            return;
        }

        int price;
        try
        {
            price = Convert.ToInt32(_priceInputField.text);
            if (!DataValidation.IsNaturalNumber(price)) throw new Exception();
        }
        catch
        {
            _errorTextField.text = _messages["NOT_VALID_PRICE"];
            return;
        }

        if (await _clientHandler.Client.AddRoom(bedsNumber, price, _imagePathInputField.text) != ServerResponse.Success)
        {
            _errorTextField.text = _messages["NO_CONNECTION"];
            return;
        }

        _notificationHandler.Notify(_messages["SUCCESS"]);
        _OnSuccess.Invoke();
    }
}
