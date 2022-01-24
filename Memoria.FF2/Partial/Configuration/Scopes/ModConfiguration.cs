namespace Memoria.FFPR.Configuration.Scopes;

public sealed partial class ModConfiguration
{
    public ConversationConfiguration Conversation { get; private set; }

    private partial void InitializeGameSpecificOptions(ConfigFileProvider provider)
    {
        Conversation = new ConversationConfiguration(provider);
    }
}