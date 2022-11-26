using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using Hotel.Models;
using Hotel.Validation;
using Hotel.Networking;
using Hotel.Containers;

namespace Hotel.ViewModels
{
    public class LoginViewModel : MonoBehaviour, IViewModel
    {
        [SerializeField] private ClientHandler _clientHandler;
        [SerializeField] private SharedData _sharedData;
        [Space(10)]
        [SerializeField] private TMP_InputField _emailInputField;
        [SerializeField] private TMP_InputField _passwordInputField;
        [SerializeField] private TMP_Text _errorTextField;
        [Space(10)]
        [SerializeField] private UnityEvent _OnSuccess;

        private static readonly Dictionary<string, string> _exceptions = new Dictionary<string, string>()
        {
            ["NO_CONNECTION"] = "Соединение с сервером не установлено. Попробуйте еще раз",
            ["INCORRECT_DATA"] = "Неверный адрес электронной почты или пароль",
            ["NOT_VALID_EMAIL"] = "Поле \"электронная почта\" заполнено некорректно",
        };

        public void OnShow()
        {
            _emailInputField.text = "";
            _passwordInputField.text = "";
            _errorTextField.text = "";
        }

        public void OnHide()
        {

        }

        public async void Submit()
        {
            if (!DataValidation.IsEMailValid(_emailInputField.text))
            {
                _errorTextField.text = _exceptions["NOT_VALID_EMAIL"];
                return;
            }

            (ServerResponse, User) result = await _clientHandler.Client.GetUser(_emailInputField.text, _passwordInputField.text);
            switch (result.Item1)
            {
                case ServerResponse.Success:
                    {
                        _sharedData.User = result.Item2;
                        _OnSuccess.Invoke();
                        break;
                    }
                case ServerResponse.ConnectionError:
                    {
                        _errorTextField.text = _exceptions["NO_CONNECTION"];
                        break;
                    }
                case ServerResponse.DataError:
                    {
                        _errorTextField.text = _exceptions["INCORRECT_DATA"];
                        break;
                    }
            }
        }
    }
}