using System.Reflection;
using System.Reflection.Emit;
using Tq.Realizer.Core.Program;
using Tq.Realizer.Core.Program.Builder;
using Tq.Realizer.Core.Program.Member;
using Tq.Realizer.Core.Configuration.LangOutput;

namespace Tq.Module.CLR;

public class Compiler
{
    private MembersMap _membersMap = new(); 
    
    public void Compile(RealizerProgram program, IOutputConfiguration config)
    {
        Console.WriteLine("Compiling to .NET CLR...");
        DefineModules(program);

    }

    private void DefineModules(RealizerProgram program)
    {
        foreach (var i in program.Modules)
        {
            if (i.Name == "System") continue;
            
            var asmName = new AssemblyName(program.Name);
            
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.RunAndCollect);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule(i.Name);
            var moduleRoot = moduleBuilder.DefineType("<root>", TypeAttributes.Sealed | TypeAttributes.Class);
            
            foreach (var j in i.GetMembers<RealizerStructure>())
                DefineTypesRecursive(moduleBuilder, moduleRoot, j);

            foreach (var j in i.GetMembers())
                DefineMembersRecursive(moduleBuilder, moduleRoot, j);
        }
    }

    private void DefineTypesRecursive(ModuleBuilder mbuilder, TypeBuilder tbuilder, RealizerStructure structure)
    {
        var typeBuilder = mbuilder.DefineType(structure.Name, TypeAttributes.Class);
        _membersMap.Add(structure, typeBuilder);
    }

    private void DefineMembersRecursive(ModuleBuilder mbuilder, TypeBuilder tbuilder, RealizerMember member)
    {
        switch (member)
        {
            case RealizerStructure @struct: break;
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
