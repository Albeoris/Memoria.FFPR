using System.Diagnostics;
using Microsoft.CodeAnalysis;

namespace Memoria.CodeGenerator;

[Generator]
public sealed class ConfigurationScopeGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // if (!Debugger.IsAttached)
        //     Debugger.Launch();
        
        var syntaxProvider = context.SyntaxProvider.CreateSyntaxProvider(
                predicate: ConfigurationScopeDescriptorFactory.IsSuitableNode,
                transform: ConfigurationScopeDescriptorFactory.TryCreateScope)
            .Where(x => x != null);

        context.RegisterSourceOutput(syntaxProvider, ConfigurationScopeEmitter.Emit!);
    }
}