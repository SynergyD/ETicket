﻿using System;
using System.Windows.Input;
using Android.Util;
using ETicketMobile.Views.Registration;
using ETicketMobile.WebAccess.DTO;
using ETicketMobile.WebAccess.Network;
using ETicketMobile.WebAccess.Network.WebService;
using Prism.Navigation;
using Xamarin.Forms;

namespace ETicketMobile.ViewModels.Registration
{
    public class EmailRegistrationViewModel : ViewModelBase
    {
        #region Constants

        private const int EmailMaxLength = 50;

        #endregion

        #region Fields

        protected INavigationService navigationService;

        private readonly HttpClientService httpClient;

        private ICommand navigateToPhoneRegistrationView;

        private string emailWarning;

        #endregion

        #region Properties

        public ICommand NavigateToPhoneRegistrationView => navigateToPhoneRegistrationView 
            ?? (navigateToPhoneRegistrationView = new Command<string>(OnMoveToPhoneRegistrationView));

        public string EmailWarning
        {
            get => emailWarning;
            set => SetProperty(ref emailWarning, value);
        }

        #endregion

        public EmailRegistrationViewModel(INavigationService navigationService) 
            : base(navigationService)
        {
            this.navigationService = navigationService
                ?? throw new ArgumentNullException(nameof(navigationService));

            httpClient = new HttpClientService();
        }

        private void OnMoveToPhoneRegistrationView(string email)
        {
            if (!IsValid(email))
                return;

            if (CheckUserExists(email))
                return;

            var navigationParams = new NavigationParameters { { "email", email } };
            navigationService.NavigateAsync(nameof(PhoneRegistrationView), navigationParams);
        }

        private bool RequestUserExists(string email)
        {
            var signUpRequestDto = new SignUpRequestDto { Email = email };

            var isUserExists = httpClient.PostAsync<SignUpRequestDto, SignUpResponseDto>(TicketsEndpoint.CheckUserExsists, signUpRequestDto).Result;

            return isUserExists.Succeeded;
        }

        #region Validation

        private bool IsValid(string email)
        {
            if (IsEmailEmpty(email))
            {
                EmailWarning = ErrorMessage.EmailCorrect;

                return false;
            }

            if (!IsEmailValid(email))
            {
                EmailWarning = ErrorMessage.EmailInvalid;

                return false;
            }

            if (!IsEmailConstainsCorrectLong(email))
            {
                EmailWarning = ErrorMessage.EmailCorrectLong;

                return false;
            }

            return true;
        }

        private bool CheckUserExists(string email)
        {
            var isUserExists = RequestUserExists(email);

            if (isUserExists)
            {
                EmailWarning = ErrorMessage.EmailTaken;

                return true;
            }

            return false;
        }

        private bool IsEmailEmpty(string email)
        {
            return string.IsNullOrEmpty(email);
        }

        private bool IsEmailValid(string email)
        {
            return Patterns.EmailAddress.Matcher(email).Matches();
        }

        private bool IsEmailConstainsCorrectLong(string email)
        {
            return email.Length <= EmailMaxLength;
        }

        #endregion
    }
}