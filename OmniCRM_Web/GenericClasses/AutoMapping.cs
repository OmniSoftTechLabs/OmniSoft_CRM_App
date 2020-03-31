using AutoMapper;
using OmniCRM_Web.Models;
using OmniCRM_Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OmniCRM_Web.GenericClasses
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            CreateMap<UserMaster, UserMasterViewModel>(); 
        }
    }
}
