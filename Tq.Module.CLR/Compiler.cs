using System.Reflection;
using System.Reflection.Emit;
using Tq.Realizeer.Core.Program;
using Tq.Realizeer.Core.Program.Builder;
using Tq.Realizeer.Core.Program.Member;
using Tq.Realizer.Core.Configuration.LangOutput;

namespace Tq.Module.CLR;

public class Compiler
{
    private MembersMap _membersMap = new(); 
    
    public void Compile(RealizerProgram program, IOutputConfiguration config)
    {
        Console.WriteLine("Compiling to .NET CLR...");

        var asmName = new AssemblyName(program.Name);
        var asmBuilder = AssemblyBuilder.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.RunAndCollect);
        
        DefineModules(program, asmBuilder);

    }

    private void DefineModules(RealizerProgram program, AssemblyBuilder assemblyBuilder)
    {
        foreach (var i in program.Modules)
        {
            var moduleBuilder = assemblyBuilder.DefineDynamicModule(i.Name);
            foreach (var j in i.GetMembers()) DefineTypesRecursive(moduleBuilder, j);
        }
    }

    private void DefineTypesRecursive(ModuleBuilder mbuilder, RealizerMember member)
    {
        switch (member)
        {
            case RealizerStructure @structure:
            {
                var typeBuilder = mbuilder.DefineType(structure.Name, TypeAttributes.Class);
                _membersMap.Add(structure, typeBuilder);
            } break;
            
            //case RealizerTypedef @typedef:
            //{
            //    var enumBuilder = mbuilder.DefineEnum(typedef.Name, TypeAttributes.Public, null!);
            //} break;
            
            default: throw new NotImplementedException();
        }
    }
    
    
    private struct MembersMap()
    {
        private Dictionary<RealizerStructure, TypeBuilder> _structToBuilder = [];
        private Dictionary<TypeBuilder, RealizerStructure> _builderToStruct = [];

        public void TrimExcess()
        {
            _structToBuilder.TrimExcess();
            _builderToStruct.TrimExcess();
        }
        
        public void Add(RealizerStructure struc, TypeBuilder builder)
        {
            _structToBuilder.Add(struc, builder);
            _builderToStruct.Add(builder, struc);
        }

        public TypeBuilder Get(RealizerStructure struc) => _structToBuilder[struc];
        public RealizerStructure Get(TypeBuilder builder) => _builderToStruct[builder];
    }
}
