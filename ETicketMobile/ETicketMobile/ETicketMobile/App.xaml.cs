﻿using System.Globalization;
using System.Threading.Tasks;
using ETicketMobile.DataAccess.Interfaces;
using ETicketMobile.DataAccess.LocalAPI;
using ETicketMobile.DataAccess.LocalAPI.Interfaces;
using ETicketMobile.DataAccess.Repositories;
using ETicketMobile.Resources;
using ETicketMobile.UserInterface.Localization.Interfaces;
using ETicketMobile.ViewModels;
using ETicketMobile.ViewModels.ForgotPassword;
using ETicketMobile.ViewModels.Login;
using ETicketMobile.ViewModels.Payment;
using ETicketMobile.ViewModels.Registration;
using ETicketMobile.ViewModels.Settings;
using ETicketMobile.ViewModels.Tickets;
using ETicketMobile.ViewModels.UserAccount;
using ETicketMobile.Views;
using ETicketMobile.Views.ForgotPassword;
using ETicketMobile.Views.Login;
using ETicketMobile.Views.Payment;
using ETicketMobile.Views.Registration;
using ETicketMobile.Views.Settings;
using ETicketMobile.Views.Tickets;
using ETicketMobile.Views.UserActions;
using Prism;
using Prism.Ioc;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace ETicketMobile
{
    public partial class App
    {
        public App() : this(null) { }

        public App(IPlatformInitializer initializer) : base(initializer) { }

        protected override async void OnInitialized()
        {
            InitializeComponent();

            await NavigationService.NavigateAsync(nameof(NavigationPage) + "/" + nameof(LoginView));
            //await NavigationService.NavigateAsync(nameof(NavigationPage) + "/" + nameof(CreateNewPasswordView));
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterInstance<ITokenRepository>(new TokenRepository());

            var localApi = LocalApi.GetInstance();
            var localize = DependencyService.Get<ILocalize>();

            InitCulture(localApi, localize).Wait();

            containerRegistry.RegisterInstance<ILocalApi>(localApi);
            containerRegistry.RegisterInstance<ILocalize>(localize);

            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<MainView, MainViewModel>();
            containerRegistry.RegisterForNavigation<LoginView, LoginViewModel>();
            containerRegistry.RegisterForNavigation<EmailRegistrationView, EmailRegistrationViewModel>();
            containerRegistry.RegisterForNavigation<NameRegistrationView, NameRegistrationViewModel>();
            containerRegistry.RegisterForNavigation<PasswordRegistrationView, PasswordRegistrationViewModel>();
            containerRegistry.RegisterForNavigation<BirthdayRegistrationView, BirthdayRegistrationViewModel>();
            containerRegistry.RegisterForNavigation<ForgotPasswordView, ForgotPasswordViewModel>();
            containerRegistry.RegisterForNavigation<TicketsView, TicketsViewModel>();
            containerRegistry.RegisterForNavigation<PhoneRegistrationView, PhoneRegistrationViewModel>();
            containerRegistry.RegisterForNavigation<BuyTicketView, BuyTicketViewModel>();
            containerRegistry.RegisterForNavigation<ConfirmEmailView, ConfirmEmailViewModel>();
            containerRegistry.RegisterForNavigation<ConfirmForgotPasswordView, ConfirmForgotPasswordViewModel>();
            containerRegistry.RegisterForNavigation<CreateNewPasswordView, CreateNewPasswordViewModel>();
            containerRegistry.RegisterForNavigation<AreasView, AreasViewModel>();
            containerRegistry.RegisterForNavigation<LiqPayView, LiqPayViewModel>();
            containerRegistry.RegisterForNavigation<ReceiptView, ReceiptViewModel>();
            containerRegistry.RegisterForNavigation<MainMenuView, MainMenuViewModel>();
            containerRegistry.RegisterForNavigation<UserAccountView, UserAccountViewModel>();
            containerRegistry.RegisterForNavigation<SettingsView, SettingsViewModel>();
            containerRegistry.RegisterForNavigation<UserTransactionsView, UserTransactionsViewModel>();
            containerRegistry.RegisterForNavigation<LocalizationView, LocalizationViewModel>();
        }

        private async Task InitCulture(ILocalApi localApi, ILocalize localize)
        {
            var localization = await localApi.GetLocalizationAsync().ConfigureAwait(false);

            if (localize != null)
            {
                var currentCulture = new CultureInfo(localization.Culture);

                localize.CurrentCulture = currentCulture;
                AppResource.Culture = currentCulture;
            }
        }
    }
}