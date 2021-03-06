﻿using AutoMapper;
using HomeCenter.Model.Areas;
using HomeCenter.Model.Components;
using HomeCenter.Model.Conditions;
using HomeCenter.Model.Core;
using HomeCenter.Model.Messages.Commands;
using HomeCenter.Model.Messages.Events;
using HomeCenter.Model.Triggers;
using HomeCenter.Services.Configuration.DTO;
using HomeCenter.Utils;

namespace HomeCenter.Services.Profiles
{
    public class HomeCenterMappingProfile : Profile
    {
        public HomeCenterMappingProfile()
        {
            ShouldMapProperty = propInfo => (propInfo.CanWrite && propInfo.GetGetMethod(true).IsPublic) || propInfo.IsDefined(typeof(MapAttribute), false);

            CreateMap<ComponentDTO, ComponentProxy>().ConstructUsingServiceLocator();
            CreateMap<AdapterReferenceDTO, AdapterReference>();
            CreateMap<TriggerDTO, Trigger>().ForMember(s => s.Commands, d => d.MapFrom<CommandResolver>()).ConstructUsingServiceLocator();
            CreateMap<EventDTO, Event>();
            CreateMap<AreaDTO, Area>();
            CreateMap<ScheduleDTO, Schedule>();
            CreateMap<ConditionContainerDTO, ConditionContainer>().ForMember(s => s.Conditions, d => d.MapFrom<ConditionResolver>()).ConstructUsingServiceLocator();

            var commandTypes = AssemblyHelper.GetAllTypes<Command>();
            foreach (var commandType in commandTypes)
            {
                CreateMap(typeof(CommandDTO), commandType);
            }
        }
    }
}