﻿using System;

namespace ETicketMobile.WebAccess.Network
{
    public static class TransactionsEndpoint
    {
        public static Uri Post = new Uri("http://192.168.1.102:50887/api/TransactionHistory/transactions");
    }
}