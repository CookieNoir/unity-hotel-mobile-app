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
            ["SUCCESS"] = "������������ ������� ���������������",
            ["NO_CONNECTION"] = "���������� � �������� �� �����������. ���������� ��� ���",
            ["CONTAINS_USER"] = "������������ � ��������� ������� ����������� ����� ��� ����������",
            ["NOT_VALID_NAME"] = "���� \"��� ������������\" ������ ��������� �� ����� 3 � �� ����� 128 ��������",
            ["NOT_VALID_EMAIL"] = "���� \"����������� �����\" ��������� �����������. ���������, ��� ��������� ���� ������ ������������� ������� local-part@domain",
            ["NOT_VALID_PHONENUMBER"] = "���� \"����� ��������\" ��������� �����������. ���������, ��� ��������� ���� ������ ������������� �������� ������ ���������� �������",
            ["NOT_VALID_PASSWORD"] = "���� \"������\" ������ ��������� �� ����� 3 � �� ����� 64 ��������",
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