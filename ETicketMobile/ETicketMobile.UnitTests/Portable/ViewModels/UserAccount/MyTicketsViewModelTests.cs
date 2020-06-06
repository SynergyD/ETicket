﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using ETicketMobile.Business.Model.Tickets;
using ETicketMobile.Business.Services.Interfaces;
using ETicketMobile.Data.Entities;
using ETicketMobile.DataAccess.LocalAPI.Interfaces;
using ETicketMobile.UnitTests.Portable.Comparer;
using ETicketMobile.ViewModels.BoughtTickets;
using ETicketMobile.WebAccess.DTO;
using ETicketMobile.WebAccess.Network.WebServices.Interfaces;
using Moq;
using Prism.Navigation;
using Prism.Services;
using Xunit;

namespace ETicketMobile.UnitTests.Portable.ViewModels.UserAccount
{
    public class MyTicketsViewModelTests
    {
        #region Fields

        private readonly MyTicketsViewModel myTicketsViewModel;

        private readonly TicketsEqualityComparer ticketsEqualityComparer;

        private readonly INavigationParameters navigationParameters;

        private readonly Mock<IPageDialogService> dialogServiceMock;
        private readonly Mock<ITokenService> tokenServiceMock;
        private readonly Mock<IHttpService> httpServiceMock;
        private readonly Mock<ILocalApi> localApiMock;

        private readonly IEnumerable<TicketDto> ticketsDto;

        private readonly IEnumerable<Ticket> tickets;
        private readonly IEnumerable<Ticket> unusedTickets;
        private readonly IEnumerable<Ticket> activatedTickets;
        private readonly IEnumerable<Ticket> expiredTickets;

        private readonly Token token;

        #endregion

        public MyTicketsViewModelTests()
        {
            dialogServiceMock = new Mock<IPageDialogService>();
            tokenServiceMock = new Mock<ITokenService>();
            httpServiceMock = new Mock<IHttpService>();
            localApiMock = new Mock<ILocalApi>();

            ticketsDto = new List<TicketDto>
            {
                new TicketDto
                {
                    TicketType = "Unused",
                    ReferenceNumber = "ReferenceNumber1",
                    TicketAreas = new List<string> { "A" },
                    CreatedAt = DateTime.Parse("02/01/20 12:00:00")
                },
                new TicketDto
                {
                    TicketType = "Activated",
                    ReferenceNumber = "ReferenceNumber2",
                    TicketAreas = new List<string> { "A", "B" },
                    CreatedAt = DateTime.Parse("02/02/20 13:00:00"),
                    ActivatedAt = DateTime.Parse("02/02/20 13:00:00"),
                    ExpiredAt = DateTime.Parse("02/07/20 14:00:00")
                },
                new TicketDto
                {
                    TicketType = "Expired",
                    ReferenceNumber = "ReferenceNumber3",
                    TicketAreas = new List<string> { "A", "B", "C" },
                    CreatedAt = DateTime.Parse("02/03/20 14:00:00"),
                    ActivatedAt = DateTime.Parse("02/03/20 14:00:00"),
                    ExpiredAt = DateTime.Parse("02/06/20 15:00:00")
                }
            };

            tickets = new List<Ticket>
            {
                new Ticket
                {
                    TicketType = "Unused",
                    ReferenceNumber = "ReferenceNumber1",
                    TicketAreas = new List<string> { "A" },
                    CreatedAt = DateTime.Parse("02/01/20 12:00:00")
                },
                new Ticket
                {
                    TicketType = "Activated",
                    ReferenceNumber = "ReferenceNumber2",
                    TicketAreas = new List<string> { "A", "B" },
                    CreatedAt = DateTime.Parse("02/02/20 13:00:00"),
                    ActivatedAt = DateTime.Parse("02/02/20 13:00:00"),
                    ExpiredAt = DateTime.Parse("02/07/20 14:00:00")
                },
                new Ticket
                {
                    TicketType = "Expired",
                    ReferenceNumber = "ReferenceNumber3",
                    TicketAreas = new List<string> { "A", "B", "C" },
                    CreatedAt = DateTime.Parse("02/03/20 14:00:00"),
                    ActivatedAt = DateTime.Parse("02/03/20 14:00:00"),
                    ExpiredAt = DateTime.Parse("02/06/20 15:00:00")
                }
            };

            unusedTickets = new List<Ticket> { tickets.ElementAt(0) };

            activatedTickets = new List<Ticket> { tickets.ElementAt(1) };

            expiredTickets = new List<Ticket> { tickets.ElementAt(2) };

            token = new Token
            {
                AcessJwtToken = "AccessToken",
                RefreshJwtToken = "RefreshToken"
            };

            localApiMock
                    .Setup(l => l.GetTokenAsync())
                    .ReturnsAsync(token);

            tokenServiceMock.Setup(ts => ts.RefreshTokenAsync());

            httpServiceMock
                    .SetupSequence(hs => hs.PostAsync<GetTicketsByEmailRequestDto, IEnumerable<TicketDto>>(
                        It.IsAny<Uri>(), It.IsAny<GetTicketsByEmailRequestDto>(), It.IsAny<string>()))
                    .ReturnsAsync(ticketsDto)
                    .Throws(new WebException());

            myTicketsViewModel = new MyTicketsViewModel(null, dialogServiceMock.Object, tokenServiceMock.Object,
                httpServiceMock.Object, localApiMock.Object);

            ticketsEqualityComparer = new TicketsEqualityComparer();

            var email = "email";
            navigationParameters = new NavigationParameters { { email, "email" } };
        }

        [Fact]
        public void CheckConstructorWithParameters_CheckNullableDialogService_ShouldThrowException()
        {
            // Assert
            Assert.Throws<ArgumentNullException>(
                () => new MyTicketsViewModel(null, null, tokenServiceMock.Object, httpServiceMock.Object, localApiMock.Object));
        }

        [Fact]
        public void CheckConstructorWithParameters_CheckNullableTokenService_ShouldThrowException()
        {
            // Assert
            Assert.Throws<ArgumentNullException>(
                () => new MyTicketsViewModel(null, dialogServiceMock.Object, null, httpServiceMock.Object, localApiMock.Object));
        }

        [Fact]
        public void CheckConstructorWithParameters_CheckNullableHttpService_ShouldThrowException()
        {
            // Assert
            Assert.Throws<ArgumentNullException>(
                () => new MyTicketsViewModel(null, dialogServiceMock.Object, tokenServiceMock.Object, null, localApiMock.Object));
        }

        [Fact]
        public void CheckConstructorWithParameters_CheckNullableLocalApi_ShouldThrowException()
        {
            // Assert
            Assert.Throws<ArgumentNullException>(
                () => new MyTicketsViewModel(null, dialogServiceMock.Object, tokenServiceMock.Object, httpServiceMock.Object, null));
        }

        [Fact]
        public void OnNavigatedTo_CompareTickets_ShouldBeEqual()
        {
            // Act
            myTicketsViewModel.OnNavigatedTo(navigationParameters);

            // Assert
            Assert.Equal(tickets, myTicketsViewModel.Tickets, ticketsEqualityComparer);

        }

        [Fact]
        public void OnNavigatedTo_CheckThrowWebException()
        {
            // Act
            myTicketsViewModel.OnNavigatedTo(navigationParameters);

            // Assert
            Assert.ThrowsAsync<WebException>(
                () => httpServiceMock.Object.PostAsync<GetTicketsByEmailRequestDto, IEnumerable<TicketDto>>(null, null, null));
        }

        [Fact]
        public void OnNavigatedTo_Tickets_CheckRefreshTokenWhenAccessTokenShouldBeNull()
        {
            // Arrange
            httpServiceMock
                .Setup(hs => hs.PostAsync<GetTicketsByEmailRequestDto, IEnumerable<TicketDto>>(
                    It.IsAny<Uri>(), It.IsAny<GetTicketsByEmailRequestDto>(), It.IsAny<string>()))
                .ReturnsAsync(() => null);

            // Act
            myTicketsViewModel.OnNavigatedTo(navigationParameters);

            // Assert
            httpServiceMock.Verify(hs => hs.PostAsync<GetTicketsByEmailRequestDto, IEnumerable<TicketDto>>(
                It.IsAny<Uri>(), It.IsAny<GetTicketsByEmailRequestDto>(), It.IsAny<string>()), Times.Exactly(2));
        }

        [Fact]
        public void OnNavigatedTo_CompareUnusedTickets_ShouldBeEqual()
        {
            // Act
            myTicketsViewModel.OnNavigatedTo(navigationParameters);

            // Assert
            Assert.Equal(unusedTickets, myTicketsViewModel.UnusedTickets, ticketsEqualityComparer);
        }

        [Fact]
        public void OnNavigatedTo_CompareActivatedTickets_ShouldBeEqual()
        {
            // Act
            myTicketsViewModel.OnNavigatedTo(navigationParameters);

            // Assert
            Assert.Equal(activatedTickets, myTicketsViewModel.ActivatedTickets, ticketsEqualityComparer);
        }

        [Fact]
        public void OnNavigatedTo_CompareExpiredTickets_ShouldBeEqual()
        {
            // Act
            myTicketsViewModel.OnNavigatedTo(navigationParameters);

            // Assert
            Assert.Equal(expiredTickets, myTicketsViewModel.ExpiredTickets, ticketsEqualityComparer);
        }
    }
}