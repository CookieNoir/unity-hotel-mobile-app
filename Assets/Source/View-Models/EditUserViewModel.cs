using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using Hotel.Networking;
using Hotel.Models;
using Hotel.Validation;
using Hotel.UI;

namespace Hotel.ViewModels
{
    public class EditUserViewModel : MonoBehaviour, IViewModel
    {
        [SerializeField] private ClientHandler _clientHandler;
        [SerializeField] private SharedData _sharedData;
        [Space(10)]
        [SerializeField] private TMP_InputField _nameInputField;
        [SerializeField] private TMP_InputField _phoneNumberInputField;
        [SerializeField] private TMP_InputField _passwordInputField;
        [SerializeField] private TMP_Text _errorTextField;
        [Space(10)]
        [SerializeField] private NotificationHandler _notificationHandler;
        [SerializeField] private UnityEvent _OnSuccess;

        private static readonly Dictionary<string, string> _messages = new Dictionary<string, string>()
        {
            ["SUCCESS"] = "������ ������������ ������� ��������",
            ["NO_CONNECTION"] = "���������� � �������� �� �����������. ���������� ��� ���",
            ["NOT_VALID_NAME"] = "���� \"��� ������������\" ������ ��������� �� ����� 3 � �� ����� 128 ��������",
            ["NOT_VALID_PHONENUMBER"] = "���� \"����� ��������\" ��������� �����������. ���������, ��� ��������� ���� ������ ������������� �������� ������ ���������� �������",
            ["NOT_VALID_PASSWORD"] = "���� \"������\" ������ ��������� �� ����� 3 � �� ����� 64 ��������",
            ["NO_CHANGES"] = "�������� ����� ��������� � ���������� ���������� ������������",
            ["INVALID_PASSWORD"] = "��������� ������ �� ��������� � ������� ������������",
        };

        public void OnShow()
        {
            _nameInputField.text = _sharedData.User.Name;
            _phoneNumberInputField.text = _sharedData.User.PhoneNumber;
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

            if (_nameInputField.text == _sharedData.User.Name && _phoneNumberInputField.text == _sharedData.User.PhoneNumber)
            {
                _errorTextField.text = _messages["NO_CHANGES"];
                return;
            }

            switch (await _clientHandler.Client.EditUser(_sharedData.User.UserId, _nameInputField.text, _phoneNumberInputField.text, _passwordInputField.text))
            {
                case ServerResponse.Success:
                    {
                        _sharedData.User.EditNameAndPhoneNumber(_nameInputField.text, _phoneNumberInputField.text);
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
