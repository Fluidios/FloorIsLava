using Fusion;
using Game.FirebaseHandler;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Systems
{
    public class LobbyAuthUI : MonoBehaviour
    {
        [SerializeField] private GameObject _authentificationForm;
        [SerializeField] TMP_InputField _emailInput;
        [SerializeField] TMP_InputField _passwordInput;
        [SerializeField] TMP_InputField _nicknameInput;
        [SerializeField] GameObject _signUpForm;
        [SerializeField] Button _signUpButton;
        [SerializeField] GameObject _signInForm;
        [SerializeField] Button _signInButton;
        [SerializeField] Button _escapeButton;
        [SerializeField] GameObject _errorForm;
        [SerializeField] TextMeshProUGUI _errorText;
        [SerializeField] private TMP_InputField _nicknameText;
        public Action AuthPassed;

        private void Awake()
        {
            _escapeButton.onClick.AddListener(ShowForm);

            _nicknameText.onEndEdit.AddListener(TryUpdateNickname);

            _nicknameText.gameObject.SetActive(false);
            AuthPassed += () =>
            {
                _nicknameText.SetTextWithoutNotify(UserAuth.UserNickName);
                _nicknameText.gameObject.SetActive(true);
            };
        }

        public void Authentificate()
        {
            if(UserAuth.LocalEmail == string.Empty)
            {
                ShowForm();
            }
            else
            {
                TrySignInWithLocalRequisites();
            }
        }

        private void ShowForm()
        {
            _authentificationForm.SetActive(true);

            _escapeButton.gameObject.SetActive(false);

            _signInForm.SetActive(true);
            _signInButton.gameObject.SetActive(true);

            _signUpForm.SetActive(true);
            _signUpButton.gameObject.SetActive(true);

            _emailInput.gameObject.SetActive(false);
            _passwordInput.gameObject.SetActive(false);
            _nicknameInput.gameObject.SetActive(false);

            _errorForm.SetActive(false);

            _signInButton.onClick.RemoveAllListeners();
            _signUpButton.onClick.RemoveAllListeners();

            _signInButton.onClick.AddListener(ShowSignInForm);
            _signUpButton.onClick.AddListener(ShowSignUpForm);
        }

        private void ShowSignInForm()
        {
            _escapeButton.gameObject.SetActive(true);

            _emailInput.gameObject.SetActive(true);
            _passwordInput.gameObject.SetActive(true);
            _nicknameInput.gameObject.SetActive(false);

            _signInForm.SetActive(true);
            _signInButton.gameObject.SetActive(false);

            _signUpForm.SetActive(false);
            _signUpButton.gameObject.SetActive(false);

            _signInButton.onClick.RemoveAllListeners();
            _signUpButton.onClick.RemoveAllListeners();

            _emailInput.onEndEdit.RemoveAllListeners();
            _passwordInput.onEndEdit.RemoveAllListeners();
            _nicknameInput.onEndEdit.RemoveAllListeners();

            _emailInput.onEndEdit.AddListener(ValidateSignInData);
            _passwordInput.onEndEdit.AddListener(ValidateSignInData);
            _signInButton.onClick.AddListener(TrySignInWithPassedData);
        }

        private void ShowSignUpForm()
        {
            _escapeButton.gameObject.SetActive(true);

            _emailInput.gameObject.SetActive(true);
            _passwordInput.gameObject.SetActive(true);
            _nicknameInput.gameObject.SetActive(true);

            _signInForm.SetActive(false);
            _signInButton.gameObject.SetActive(false);

            _signUpForm.SetActive(true);
            _signUpButton.gameObject.SetActive(false);

            _signInButton.onClick.RemoveAllListeners();
            _signUpButton.onClick.RemoveAllListeners();

            _emailInput.onEndEdit.RemoveAllListeners();
            _passwordInput.onEndEdit.RemoveAllListeners();
            _nicknameInput.onEndEdit.RemoveAllListeners();

            _emailInput.onEndEdit.AddListener(ValidateSignUpData);
            _passwordInput.onEndEdit.AddListener(ValidateSignUpData);
            _nicknameInput.onEndEdit.AddListener(ValidateSignUpData);
            _signUpButton.onClick.AddListener(CreateNewProfileWithPassedData);
        }

        private void HideForm()
        {
            _escapeButton.gameObject.SetActive(false);
            _authentificationForm.gameObject.SetActive(false);
        }

        private async void TrySignInWithLocalRequisites()
        {
            var signInTask = UserAuth.TrySignIn(UserAuth.LocalEmail, UserAuth.LocalPassword);
            await signInTask;
            if (signInTask.Result == Firebase.Auth.AuthError.None)
            {
                AuthPassed?.Invoke();
                Debug.Log("Signed in with local requisites.");
            }
            else
            {
                ShowForm();
            }
        }
        private async void TrySignInWithPassedData()
        {
            _signInButton.interactable = false;
            var signInTask = UserAuth.TrySignIn(_emailInput.text, _passwordInput.text);
            await signInTask;
            if (signInTask.Result == Firebase.Auth.AuthError.None)
            {
                HideForm();
                _signInButton.interactable = true;
                AuthPassed?.Invoke();
            }
            else
            {
                _signInButton.interactable = true;
                _errorText.text = signInTask.Result.ToString();
                _errorForm.SetActive(true);
            }
        }
        private async void CreateNewProfileWithPassedData()
        {
            _signUpButton.interactable = false;
            var signUpTask = UserAuth.CreateUser(_emailInput.text, _passwordInput.text, _nicknameInput.text);
            await signUpTask;
            if (signUpTask.Result == Firebase.Auth.AuthError.None)
            {
                HideForm();
                _signInButton.interactable = true;
                AuthPassed?.Invoke();
            }
            else
            {
                _signInButton.interactable = true;
                _errorText.text = signUpTask.Result.ToString();
                _errorForm.SetActive(true);
            }
        }
        private void ValidateSignInData(string str)
        {
            bool validation = _emailInput.text.Length > 0 && _passwordInput.text.Length >= 8;
            _signInButton.gameObject.SetActive(validation);
        }
        private void ValidateSignUpData(string str)
        {
            bool validation = _emailInput.text.Length > 0 && _nicknameInput.text.Length > 0 && _passwordInput.text.Length >= 8;
            _signUpButton.gameObject.SetActive(validation);
        }

        private async void TryUpdateNickname(string newNickname)
        {
            var task = UserAuth.UpdateNickname(newNickname);
            await task;
            if(task.Result)
            {
                Debug.Log("Nickname updated!");
            }
            else
            {
                _nicknameText.SetTextWithoutNotify(UserAuth.UserNickName);
            }
        }
    }
}
