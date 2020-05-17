﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using ETicketMobile.Business.Mapping;
using ETicketMobile.Business.Model.Tickets;
using ETicketMobile.Data.Entities;
using ETicketMobile.DataAccess.LocalAPI.Interfaces;
using ETicketMobile.Resources;
using ETicketMobile.Views.Payment;
using ETicketMobile.WebAccess.DTO;
using ETicketMobile.WebAccess.Network;
using ETicketMobile.WebAccess.Network.WebService;
using Prism.Navigation;
using Prism.Services;
using Xamarin.Forms;

namespace ETicketMobile.ViewModels.Tickets
{
    public class AreasViewModel : ViewModelBase
    {
        #region Fields

        private readonly INavigationService navigationService;
        private readonly IPageDialogService dialogService;
        private INavigationParameters navigationParameters;

        private readonly ILocalApi localApi;
        private readonly HttpClientService httpClient;

        private string accessToken;

        private ICommand buy;

        private IEnumerable<Area> areas;

        #endregion

        #region Properties

        public ICommand Buy => buy
            ?? (buy = new Command(OnBuy));

        public IEnumerable<Area> Areas
        {
            get => areas;
            set => SetProperty(ref areas, value);
        }

        #endregion

        public AreasViewModel(INavigationService navigationService, IPageDialogService dialogService, ILocalApi localApi)
             : base(navigationService)
        {
            this.navigationService = navigationService
                ?? throw new ArgumentNullException(nameof(navigationService));

            this.dialogService = dialogService
                ?? throw new ArgumentNullException(nameof(dialogService));

            this.localApi = localApi
                ?? throw new ArgumentNullException(nameof(localApi));

            httpClient = new HttpClientService();
        }

        public async override void OnAppearing()
        {
            accessToken = await GetAccessToken();
            Areas = await GetAreas();
        }

        public override void OnNavigatedTo(INavigationParameters navigationParameters)
        {
            this.navigationParameters = navigationParameters;
        }

        private async Task<string> GetAccessToken()
        {
            var token = await localApi.GetTokenAsync().ConfigureAwait(false);

            return token.AcessJwtToken;
        }

        private async Task<IEnumerable<Area>> GetAreas()
        {
            var areaDto = await httpClient.GetAsync<IEnumerable<AreaDto>>(
                    AreasEndpoint.Get, accessToken).ConfigureAwait(false);

            if (areaDto == null)
            {
                accessToken = await RefreshToken();

                areaDto = await httpClient.GetAsync<IEnumerable<AreaDto>>(
                        AreasEndpoint.Get, accessToken).ConfigureAwait(false);
            }

            var areas = AutoMapperConfiguration.Mapper.Map<IEnumerable<Area>>(areaDto);

            return areas;
        }

        private async Task<string> RefreshToken()
        {
            var refreshToken = localApi.GetTokenAsync().Result.RefreshJwtToken;

            var tokenDto = await httpClient.PostAsync<string, TokenDto>(
                    TicketsEndpoint.RefreshToken, refreshToken);

            var token = AutoMapperConfiguration.Mapper.Map<Token>(tokenDto);

            await localApi.AddAsync(token);

            return token.AcessJwtToken;
        }

        private void OnBuy(object obj)
        {
            var selectedAreasId = Areas
                    .Where(a => a.Selected)
                    .Select(a => a.Id);

            if (!IsValid(selectedAreasId.Count()))
                return;

            navigationParameters.Add("areas", selectedAreasId);
            navigationService.NavigateAsync(nameof(LiqPayView), navigationParameters);
        }

        private bool IsValid(int count)
        {
            if (!TicketChoosed(count))
            {
                dialogService.DisplayAlertAsync("Alert", AppResource.CountTicketsWrong, "OK");

                return false;
            }

            return true;
        }

        private bool TicketChoosed(int count)
        {
            return count != 0;
        }
    }
}