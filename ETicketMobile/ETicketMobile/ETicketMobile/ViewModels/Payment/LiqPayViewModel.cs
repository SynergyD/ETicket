﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using ETicketMobile.Business.Model.Registration;
using ETicketMobile.Business.Validators;
using ETicketMobile.Views.Payment;
using ETicketMobile.WebAccess.DTO;
using ETicketMobile.WebAccess.Network.Endpoints;
using ETicketMobile.WebAccess.Network.WebServices.Interfaces;
using Java.Net;
using Prism.Navigation;
using Prism.Services;
using Xamarin.Forms;

namespace ETicketMobile.ViewModels.Payment
{
    public class LiqPayViewModel : ViewModelBase
    {
        #region Fields

        private readonly IPageDialogService dialogService;
        private readonly IHttpService httpService;

        private IEnumerable<int> areasId;
        private int ticketTypeId;
        private string email;

        private ICommand pay;

        private decimal amount;
        private string description;

        private string cardNumber;
        private string expirationDate;
        private string cvv2;

        private bool cardNumberWarningIsVisible;
        private bool expirationDateWarningIsVisible;
        private bool cvv2WarningIsVisible;

        #endregion

        #region Properties

        public ICommand Pay => pay 
            ??= new Command(OnPay);

        public decimal Amount
        {
            get => amount;
            set => SetProperty(ref amount, value);
        }

        public string Description
        {
            get => description;
            set => SetProperty(ref description, value);
        }

        public bool CardNumberWarningIsVisible
        {
            get => cardNumberWarningIsVisible;
            set => SetProperty(ref cardNumberWarningIsVisible, value);
        }

        public bool ExpirationDateWarningIsVisible
        {
            get => expirationDateWarningIsVisible;
            set => SetProperty(ref expirationDateWarningIsVisible, value);
        }

        public bool CVV2WarningIsVisible
        {
            get => cvv2WarningIsVisible;
            set => SetProperty(ref cvv2WarningIsVisible, value);
        }

        public string CardNumber
        {
            get => cardNumber;
            set => SetProperty(ref cardNumber, value);
        }

        public string ExpirationDate
        {
            get => expirationDate;
            set => SetProperty(ref expirationDate, value);
        }

        public string CVV2
        {
            get => cvv2;
            set => SetProperty(ref cvv2, value);
        }

        #endregion

        public LiqPayViewModel(
            INavigationService navigationService,
            IPageDialogService dialogService,
            IHttpService httpService
        ) : base(navigationService)
        {
            this.dialogService = dialogService
                ?? throw new ArgumentNullException(nameof(dialogService));

            this.httpService = httpService
                ?? throw new ArgumentNullException(nameof(httpService));
        }

        public override void OnAppearing()
        {
            Init();
        }

        private void Init()
        {
            ExpirationDate = string.Empty;
            CVV2 = string.Empty;

            expirationDate = string.Empty;
            cvv2 = string.Empty;
        }

        public override void OnNavigatedTo(INavigationParameters navigationParameters)
        {
            Description = navigationParameters.GetValue<string>("ticketName");
            ticketTypeId = navigationParameters.GetValue<int>("ticketId");
            areasId = navigationParameters["areas"] as IEnumerable<int>;
            email = navigationParameters.GetValue<string>("email");

            Amount = navigationParameters.GetValue<decimal>("totalPrice");
        }

        private async Task<GetTicketPriceResponseDto> RequestGetTicketPriceAsync()
        {
            var getTicketPriceRequestDto = new GetTicketPriceRequestDto
            {
                AreasId = areasId,
                TicketTypeId = ticketTypeId
            };

            var response = await httpService.PostAsync<GetTicketPriceRequestDto, GetTicketPriceResponseDto>(
                    TicketsEndpoint.GetTicketPrice, getTicketPriceRequestDto);

            return response;
        }

        private async void OnPay()
        {
            await PayAsync();
        }

        private async Task PayAsync()
        {
            var cardNumber = GetStringWithoutMask(this.cardNumber);

            if (!IsValid(cardNumber))
                return;

            var expirationDateDescriptor = GetExpirationDateDescriptor();

            var buyTicketRequestDto = CreateBuyTicketRequestDto(
                    cardNumber,
                    expirationDateDescriptor.ExpirationMonth,
                    expirationDateDescriptor.ExpirationYear,
                    cvv2);

            try
            {
                var response = await RequestBuyTicketAsync(buyTicketRequestDto);
            }
            catch (WebException)
            {
                await dialogService.DisplayAlertAsync("Error", "Check connection with server", "OK");

                return;
            }
            catch (SocketException)
            {
                await dialogService.DisplayAlertAsync("Error", "Check connection with server", "OK");

                return;
            }

            var navigationParameters = new NavigationParameters { { "email", email } };
            await NavigationService.NavigateAsync(nameof(TransactionCompletedView), navigationParameters);
        }

        private async Task<BuyTicketResponseDto> RequestBuyTicketAsync(BuyTicketRequestDto buyTicketRequestDto)
        {
            var response = await httpService.PostAsync<BuyTicketRequestDto, BuyTicketResponseDto>(
                TicketsEndpoint.BuyTicket, buyTicketRequestDto);

            return response;
        }

        private BuyTicketRequestDto CreateBuyTicketRequestDto(
            string cardNumber,
            string expirationMonth,
            string expirationYear,
            string cvv2
        )
        {
            return new BuyTicketRequestDto
            {
                TicketTypeId = ticketTypeId,
                Email = email,
                Price = Amount,
                Description = Description,
                CardNumber = cardNumber,
                ExpirationMonth = expirationMonth,
                ExpirationYear = expirationYear,
                CVV2 = cvv2
            };
        }


        #region Validation

        private bool IsValid(string cardNumber)
        {
            if (!Validator.HasCardNumberCorrectLength(cardNumber))
            {
                CardNumberWarningIsVisible = true;

                return false;
            }

            if (!Validator.HasExpirationDateCorrectLength(expirationDate))
            {
                CardNumberWarningIsVisible = false;

                ExpirationDateWarningIsVisible = true;

                return false;
            }

            var cvv2 = GetStringWithoutMask(CVV2);

            if (cvv2 != CVV2 || !Validator.HasCVV2CorrectLength(cvv2))
            {
                CardNumberWarningIsVisible = false;
                ExpirationDateWarningIsVisible = false;

                CVV2WarningIsVisible = true;

                return false;
            }

            return true;
        }

        #endregion

        private string GetStringWithoutMask(string str)
        {
            return Regex.Replace(str, @"[^\d]", string.Empty);
        }

        private ExpirationDateDescriptor GetExpirationDateDescriptor()
        {
            var expirationDate = ExpirationDate.Split('/');

            return new ExpirationDateDescriptor
            {
                ExpirationMonth = expirationDate[0],
                ExpirationYear = expirationDate[1]
            };
        }
    }
}