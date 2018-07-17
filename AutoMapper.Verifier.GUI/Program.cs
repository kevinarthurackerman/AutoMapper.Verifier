using AutoMapper.Verifier.GUI.Models;

namespace AutoMapper.Verifier.GUI
{
    class Program
    {
        static void Main(string[] args)
        {
            var mappings = Verifier.VerifyMappings();
        }

        public void FakeUseMaps()
        {
            var test1 = Mapper.Map<ClassA, ClassB>(new ClassA());
            var test3 = Mapper.Map<ClassB, ClassA>(new ClassB());
            var test2 = Mapper.Map<ClassC, ClassB>(new ClassC());
            var test4 = Mapper.Map<ClassB, ClassC>(new ClassB());
            var test5 = Mapper.Map<ClassC>(new ClassB());
        }
    }
}
