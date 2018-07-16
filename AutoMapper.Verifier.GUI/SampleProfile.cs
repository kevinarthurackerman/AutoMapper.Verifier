using AutoMapper.Verifier.GUI.Models;

namespace AutoMapper.Verifier.GUI
{
    internal class SampleProfile : Profile
    {
        internal SampleProfile()
        {
            CreateMap<ClassA, ClassB>().ReverseMap();
            CreateMap<ClassB, ClassC>();
        }
    }
}
