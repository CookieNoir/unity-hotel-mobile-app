using System.Collections.Generic;
using UnityEngine;
using Hotel.Networking;
using Hotel.Models;
using Hotel.UI;
using TMPro;
using Hotel.Validation;

namespace Hotel.ViewModels
{
    public class EditRoleViewModel : MonoBehaviour, IViewModel
    {
        [SerializeField] private ClientHandler _clientHandler;
        [SerializeField] private SharedData _sharedData;
        [Space(10)]
        [SerializeField] private TMP_InputField _emailInputField;
        [SerializeField] private TMP_Dropdown _roleDropdown;
        [SerializeField] private TMP_Text _errorTextField;
        [Space(10)]
        [SerializeField] private NotificationHandler _notificationHandler;

        private static readonly Dictionary<string, string> _messages = new Dictionary<string, string>()
        {
            ["SUCCESS"] = "���� ������������ ������� ��������",
            ["NO_CONNECTION"] = "���������� � �������� �� �����������. ���������� ��� ���",
            ["INCORRECT_DATA"] = "������������ � ��������� ������� ����������� ����� �� ������",
            ["NOT_VALID_EMAIL"] = "���� \"����������� �����\" ��������� �����������. ���������, ��� ��������� ���� ������ ������������� ������� local-part@domain",
            ["EQUAL_EMAIL"] = "�� �� ������ �������� ���� ����",
        };

        public void OnShow()
        {
            _emailInputField.text = "";
            _roleDropdown.value = 0;
            _errorTextField.text = "";
        }

        public void OnHide()
        {

        }

        public async void Submit()
        {
            if (!DataValidation.IsEMailValid(_emailInputField.text))
            {
                _errorTextField.text = _messages["NOT_VALID_EMAIL"];
                return;
            }
            if (_emailInputField.text == _sharedData.User.Email)
            {
                _errorTextField.text = _messages["EQUAL_EMAIL"];
                return;
            }

            Role role = Role.Customer;
            switch (_roleDropdown.value)
            {
                case 0: { role = Role.Customer; break; }
                case 1: { role = Role.Receptionist; break; }
                case 2: { role = Role.Admin; break; }
            }

            switch (await _clientHandler.Client.EditRole(_emailInputField.text, role))
            {
                case ServerResponse.Success:
                    {
                        _notificationHandler.Notify(_messages["SUCCESS"]);
                        _errorTextField.text = "";
                        return;
                    }
                case ServerResponse.DataError:
                    {
                        _errorTextField.text = _messages["INCORRECT_DATA"];
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