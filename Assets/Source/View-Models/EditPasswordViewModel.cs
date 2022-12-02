using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using Hotel.Networking;
using Hotel.Models;
using Hotel.UI;
using Hotel.Validation;

namespace Hotel.ViewModels
{
    public class EditPasswordViewModel : MonoBehaviour, IViewModel
    {
        [SerializeField] private ClientHandler _clientHandler;
        [SerializeField] private SharedData _sharedData;
        [Space(10)]
        [SerializeField] private TMP_InputField _oldPasswordInputField;
        [SerializeField] private TMP_InputField _newPasswordInputField;
        [SerializeField] private TMP_Text _errorTextField;
        [Space(10)]
        [SerializeField] private NotificationHandler _notificationHandler;
        [SerializeField] private UnityEvent _OnSuccess;

        private static readonly Dictionary<string, string> _messages = new Dictionary<string, string>()
        {
            ["SUCCESS"] = "Пароль успешно изменен",
            ["NO_CONNECTION"] = "Соединение с сервером не установлено. Попробуйте еще раз",
            ["NOT_VALID_OLD_PASSWORD"] = "Поле \"старый пароль\" должно содержать не менее 3 и не более 64 символов",
            ["NOT_VALID_NEW_PASSWORD"] = "Поле \"новый пароль\" должно содержать не менее 3 и не более 64 символов",
            ["PASSWORDS_MATCH"] = "Старый и новый пароли не должны совпадать",
            ["INVALID_PASSWORD"] = "Указанный пароль не совпадает с паролем пользователя",
        };

        public void OnShow()
        {
            _oldPasswordInputField.text = "";
            _newPasswordInputField.text = "";
            _errorTextField.text = "";
        }

        public void OnHide()
        {

        }

        public async void Submit()
        {
            if (!DataValidation.IsLengthValid(_oldPasswordInputField.text, 3, 64))
            {
                _errorTextField.text = _messages["NOT_VALID_OLD_PASSWORD"];
                return;
            }

            if (!DataValidation.IsLengthValid(_newPasswordInputField.text, 3, 64))
            {
                _errorTextField.text = _messages["NOT_VALID_NEW_PASSWORD"];
                return;
            }

            if (_oldPasswordInputField.text == _newPasswordInputField.text)
            {
                _errorTextField.text = _messages["PASSWORDS_MATCH"];
                return;
            }

            switch (await _clientHandler.Client.EditPassword(_sharedData.User.UserId, _oldPasswordInputField.text, _newPasswordInputField.text))
            {
                case ServerResponse.Success:
                    {
                        _notificationHandler.Notify(_messages["SUCCESS"]);
                        _OnSuccess.Invoke();
                        return;
                    }
                case ServerResponse.DataError:
                    {
                        _errorTextField.text = _messages["INVALID_PASSWORD"];
                        return;
                    }
                case ServerResponse.ConnectionError:
                    {
                        _errorTextField.text = _messages["NO_CONNECTION"];
                        return;
                    }
            }
        }
    }
}