﻿using AutoMapper;
using ETicket.ApplicationServices.DTOs;
using ETicket.DataAccess.Domain.Entities;
using ETicketAdmin.Mapping;

namespace ETicket.ApplicationServices.Services
{
    public class MapperService
    {
        private readonly IMapper mapper;

        public MapperService()
        {
            mapper = ConfigureMapper();
        }

        private IMapper ConfigureMapper()
        {
            var config = new MapperConfiguration(cfg =>  cfg.AddProfile<MapperProfile>());

            var mapper = config.CreateMapper();

            return mapper;
        }

        public TDestination Map<TSource, TDestination>(TSource source)
        {
            return mapper.Map<TSource, TDestination>(source);
        }
    }
}