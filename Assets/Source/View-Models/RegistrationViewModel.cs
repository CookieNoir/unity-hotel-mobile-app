using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using Hotel.Validation;
using Hotel.Networking;
using Hotel.UI;

namespace Hotel.ViewModels
{
    public class RegistrationViewModel : MonoBehaviour, IViewModel
    {
        [SerializeField] private ClientHandler _clientHandler;
        [Space(10)]
        [SerializeField] private TMP_InputField _nameInputField;
        [SerializeField] private TMP_InputField _emailInputField;
        [SerializeField] private TMP_InputField _phoneNumberInputField;
        [SerializeField] private TMP_InputField _passwordInputField;
        [SerializeField] private TMP_Text _errorTextField;
        [Space(10)]
        [SerializeField] private NotificationHandler _notificationHandler;
        [SerializeField] private UnityEvent _OnSuccess;


        private static readonly Dictionary<string, string> _messages = new Dictionary<string, string>()
        {
            ["SUCCESS"] = "Пользователь успешно зарегистрирован",
            ["NO_CONNECTION"] = "Соединение с сервером не установлено. Попробуйте еще раз",
            ["CONTAINS_USER"] = "Пользователь с указанным адресом электронной почты уже существует",
            ["NOT_VALID_NAME"] = "Поле \"имя пользователя\" должно содержать не менее 3 и не более 128 символов",
            ["NOT_VALID_EMAIL"] = "Поле \"электронная почта\" заполнено некорректно. Убедитесь, что указанные вами данные соответствуют формату local-part@domain",
            ["NOT_VALID_PHONENUMBER"] = "Поле \"номер телефона\" заполнено некорректно. Убедитесь, что указанные вами данные соответствуют форматам записи телефонных номеров",
            ["NOT_VALID_PASSWORD"] = "Поле \"пароль\" должно содержать не менее 3 и не более 64 символов",
        };

        public void OnShow()
        {
            _nameInputField.text = "";
            _emailInputField.text = "";
            _phoneNumberInputField.text = "";
            _passwordInputField.text = "";
            _errorTextField.text = "";
        }

        public void OnHide()
        {

        }

        public async void Submit()
        {
            if (!DataValidation.IsLengthValid(_nameInputField.text, 3, 128))
            {
                _errorTextField.text = _messages["NOT_VALID_NAME"];
                return;
            }
            if (!DataValidation.IsEMailValid(_emailInputField.text))
            {
                _errorTextField.text = _messages["NOT_VALID_EMAIL"];
                return;
            }
            if (!DataValidation.IsPhoneNumberValid(_phoneNumberInputField.text))
            {
                _errorTextField.text = _messages["NOT_VALID_PHONENUMBER"];
                return;
            }
            if (!DataValidation.IsLengthValid(_passwordInputField.text, 3, 64))
            {
                _errorTextField.text = _messages["NOT_VALID_PASSWORD"];
                return;
            }

            switch (await _clientHandler.Client.RegisterUser(_nameInputField.text, _emailInputField.text, _phoneNumberInputField.text, _passwordInputField.text))
            {
                case ServerResponse.Success:
                    {
                        _notificationHandler.Notify(_messages["SUCCESS"]);
                        _OnSuccess.Invoke();
                        break;
                    }
                case ServerResponse.ConnectionError:
                    {
                        _errorTextField.text = _messages["NO_CONNECTION"];
                        break;
                    }
                case ServerResponse.DataError:
                    {
                        _errorTextField.text = _messages["CONTAINS_USER"];
                        break;
                    }
            }
        }
    }
}