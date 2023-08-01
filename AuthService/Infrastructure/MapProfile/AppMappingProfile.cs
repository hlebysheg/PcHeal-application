using AuthService.Models;
using AutoMapper;
using System;
using WordBook.Model.dto;

namespace AuthService.Infrastructure.MapProfile
{
	public class AppMappingProfile: Profile
	{
		public AppMappingProfile()
		{
			CreateMap<PCInfoMessage, PcStatistic>().ReverseMap();
		}
	}
}
