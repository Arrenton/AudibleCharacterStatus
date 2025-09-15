using Dalamud.Plugin.Services;

namespace AudibleCharacterStatus
{
    internal static class Service
    {
        internal static IClientState ClientState;
        internal static IFramework Framework;
        internal static Configuration Config;
        internal static ICondition Condition;
        internal static IPluginLog PluginLog;
    }
}
