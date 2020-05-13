﻿using System.Threading.Tasks;
using ETicketMobile.Data.Entities;

namespace ETicketMobile.DataAccess.Interfaces
{
    public interface ITokenRepository
    {
        Task<Token> GetTokenAsync();

        Task SaveTokenAsync(Token token);
    }
}