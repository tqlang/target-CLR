using Tq.Module.LLVM.Targets;
using Tq.Realizer.Core.Configuration;
using Tq.Realizer.Core.Configuration.LangOutput;

namespace Tq.Module.CLR;


public class Module: IModule
{
    public ModuleConfiguration Config { get; } = new()
    {
        Name = "CLR",
        Description = "Provides targets for compiling to microsoft's Common Language Runtime assemblies",
        
        Author = "lumi2021",
        Version = "1.0.0",
        
        Targets = [
            new TargetConfiguration
            {
                TargetName = ".NET CLR",
                TargetDescription = "Compiles to microsoft's Common Language Runtime assemblies",
                TargetIdentifier = "dotnet",
                
                LanguageOutput = new OmegaOutputConfiguration() {
                    BakeGenerics = true,
                    AbstractingOptions = AbstractingOptions.NoNamespaces,
                    
                    MemoryUnit = 8,
                    NativeIntegerSize = 32,
                    
                    GenericAllowedFeatures = GenericAllowedFeatures.None, 
                    OmegaAllowedFeatures = OmegaAllowedFeatures.None,
                },
                
                CompilerInvoke = (a, b) => new Compiler().Compile(a, b)
            },
        ]
    };
}
