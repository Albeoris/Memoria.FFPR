using System;
using Memoria.FF2.Internal.Core;
using Newtonsoft.Json.Linq;

namespace Memoria.FFPR;

public sealed partial class SaveManager
{
    private static partial void SaveData(JObject root)
    {
        ConversationManager.SaveData(root);
    }

    private static partial void LoadData(JObject root)
    {
        ConversationManager.LoadData(root);
    }

    private static partial void ResetData()
    {
        ConversationManager.ResetData();
    }
}