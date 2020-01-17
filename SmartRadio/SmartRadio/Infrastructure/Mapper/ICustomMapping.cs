using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

namespace SmartRadio.Infrastructure.Mapper
{
    public interface ICustomMapping
    {
        void ConfigureMapping(Profile profile);
    }
}
