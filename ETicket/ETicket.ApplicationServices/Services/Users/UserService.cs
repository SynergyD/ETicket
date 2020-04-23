﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ETicket.ApplicationServices.DTOs;
using ETicket.ApplicationServices.Services.Users.Interfaces;
using ETicket.DataAccess.Domain.Entities;
using ETicket.DataAccess.Domain.Interfaces;

namespace ETicket.ApplicationServices.Services.Users
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork uow;
        private readonly MapperService mapper;

        public UserService(IUnitOfWork uow)
        {
            this.uow = uow;
            mapper = new MapperService();
        }

        public void CreateUser(UserDto userDto)
        {
            var user = mapper.Map<UserDto, User>(userDto);
            user.Id = Guid.NewGuid();
            uow.Users.Create(user);
            uow.Save();
        }

        public void CreateUserWithDocument(DocumentDto documentDto, UserDto userDto)
        {
            var document = mapper.Map<DocumentDto, Document>(documentDto);
            document.Id = Guid.NewGuid();
            uow.Documents.Create(document);
            uow.Save();

            var user = mapper.Map<UserDto, User>(userDto);
            user.DocumentId = document.Id;
            uow.Users.Create(user);
            uow.Save();
        }

        public void Delete(Guid id)
        {
            uow.Users.Delete(id);
            uow.Save();
        }

        public IEnumerable<User> GetAll()
        {
            return uow.Users.GetAll().ToList();
        }

        public User GetById(Guid id)
        {
            return uow.Users.Get(id);
        }

        public void SendMessage(Guid id, string message)
        {
            var user = GetById(id);

            MailService emailService = new MailService();
            emailService.SendEmail(user.Email, message);
        }

        public void Update(UserDto userDto)
        {
            var user = mapper.Map<UserDto, User>(userDto);
            uow.Users.Update(user);
            uow.Save();
        }

        public bool Exists(Guid id)
        {
            return uow.Users.UserExists(id);
        }
    }
}
