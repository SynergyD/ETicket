﻿using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using ETicketMobile.Views.Login;
using ETicketMobile.WebAccess.DTO;
using ETicketMobile.WebAccess.Network;
using ETicketMobile.WebAccess.Network.WebService;
using Prism.Navigation;
using Xamarin.Forms;

namespace ETicketMobile.ViewModels.ForgotPassword
{
    public class CreateNewPasswordViewModel : ViewModelBase
    {
        #region Constants

        private const int PasswordMinLength = 8;
        private const int PasswordMaxLength = 100;

        #endregion

        #region Fields

        private readonly INavigationService navigationService;
        private INavigationParameters navigationParameters;

        private readonly HttpClientService httpClient;

        private ICommand navigateToSignInView;

        private string passwordWarning;

        private string confirmPasswordWarning;

        private string confirmPassword;

        #endregion

        #region Properties

        public ICommand NavigateToSignInView => navigateToSignInView
            ?? (navigateToSignInView = new Command<string>(OnNavigateToSignInView));

        public string PasswordWarning
        {
            get => passwordWarning;
            set => SetProperty(ref passwordWarning, value);
        }

        public string ConfirmPasswordWarning
        {
            get => confirmPasswordWarning;
            set => SetProperty(ref confirmPasswordWarning, value);
        }

        public string ConfirmPassword
        {
            get => confirmPassword;
            set => SetProperty(ref confirmPassword, value);
        }

        #endregion

        public CreateNewPasswordViewModel(INavigationService navigationService) 
            : base(navigationService)
        {
            this.navigationService = navigationService
                ?? throw new ArgumentNullException(nameof(navigationService));

            httpClient = new HttpClientService();
        }

        public override void OnNavigatedTo(INavigationParameters navigationParameters)
        {
            this.navigationParameters = navigationParameters;
        }

        private async void OnNavigateToSignInView(string password)
        {
            if (!IsValid(password))
                return;

            if (!await RequestChangePassword(password))
                return;

            await navigationService.NavigateAsync(nameof(LoginView));
        }

        private async Task<bool> RequestChangePassword(string password)
        {
            var email = navigationParameters.GetValue<string>("email");

            var createNewPasswordDto = CreateNewPasswordDto(email, password);
            var response = await httpClient.PostAsync<CreateNewPasswordRequestDto, CreateNewPasswordResponseDto>(
                TicketsEndpoint.ChangePassword,
                createNewPasswordDto
            );

            return response.Succeeded;
        }

        private CreateNewPasswordRequestDto CreateNewPasswordDto(string email, string password)
        {
            return new CreateNewPasswordRequestDto
            {
                Email = email,
                Password = password
            };
        }

        #region Validation

        private bool IsValid(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                PasswordWarning = ErrorMessage.PasswordEmpty;

                return false;
            }

            if (!IsPasswordShort(password))
            {
                PasswordWarning = ErrorMessage.PasswordShort;

                return false;
            }

            if (!IsPasswordLong(password))
            {
                PasswordWarning = ErrorMessage.PasswordLong;

                return false;
            }

            if (IsPasswordWeak(password))
            {
                PasswordWarning = ErrorMessage.PasswordStrong;

                return false;
            }

            if (!PasswordsMatched(password))
            {
                ConfirmPasswordWarning = ErrorMessage.PasswordsMatch;

                return false;
            }

            return true;
        }

        private bool IsPasswordShort(string password)
        {
            return password.Length >= PasswordMinLength;
        }

        private bool IsPasswordLong(string password)
        {
            return password.Length <= PasswordMaxLength;
        }

        private bool IsPasswordWeak(string password)
        {
            return password.All(ch => char.IsDigit(ch));
        }

        private bool PasswordsMatched(string password)
        {
            return string.Equals(password, ConfirmPassword);
        }

        #endregion
    }
}