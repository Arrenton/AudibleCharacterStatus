using AudibleCharacterStatus.Attributes;
using AudibleCharacterStatus.Windows;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using System;
using System.Linq;
using Dalamud.Plugin.Services;
using Dalamud.Bindings.ImGui;

namespace AudibleCharacterStatus
{
    public class AudibleCharacterStatus : IDalamudPlugin
    {
        private readonly IDalamudPluginInterface pluginInterface;

        private readonly PluginCommandManager<AudibleCharacterStatus> commandManager;
        private readonly WindowSystem windowSystem;

        public string Name => "Audible Character Status";

        public AudibleCharacterStatus(
            IDalamudPluginInterface pi,
            ICommandManager commands,
            IFramework framework,
            IClientState clientState,
            ICondition condition,
            IPluginLog pluginLog)
        {
            this.pluginInterface = pi;
            Service.ClientState = clientState;
            Service.Framework = framework;
            Service.Condition = condition;
            Service.PluginLog = pluginLog;

            // Get or create a configuration object
            Service.Config = (Configuration)this.pluginInterface.GetPluginConfig() ?? new Configuration();
            Service.Config.Initialize(pi);

            // Initialize the UI
            this.windowSystem = new WindowSystem(typeof(AudibleCharacterStatus).AssemblyQualifiedName);

            var window = this.pluginInterface.Create<PluginConfiguriation>();
            if (window is not null)
            {
                this.windowSystem.AddWindow(window);
            }

            this.pluginInterface.UiBuilder.OpenConfigUi += ToggleConfigWindow;
            this.pluginInterface.UiBuilder.OpenMainUi += ToggleConfigWindow;
            this.pluginInterface.UiBuilder.Draw += this.windowSystem.Draw;
            this.pluginInterface.UiBuilder.Draw += SoundController.Update;

            // Load all of our commands
            this.commandManager = new PluginCommandManager<AudibleCharacterStatus>(this, commands);
        }

        [Command("/pacs")]
        [HelpMessage("Configuration Window Toggle")]
        public void ConfigCommand(string command, string args)
        {
            ToggleConfigWindow();
        }

        private void ToggleConfigWindow()
        {
            //var configWindow = this.windowSystem.GetWindow("Audible Character Status Configuration");
            var configWindow = this.windowSystem.Windows.FirstOrDefault(x => x.WindowName == "Audible Character Status Configuration");
            
            configWindow.IsOpen = !configWindow.IsOpen;
        }

        #region IDisposable Support
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;

            this.commandManager.Dispose();

            this.pluginInterface.SavePluginConfig(Service.Config);

            this.pluginInterface.UiBuilder.OpenConfigUi -= ToggleConfigWindow;
            this.pluginInterface.UiBuilder.OpenMainUi -= ToggleConfigWindow;
            this.pluginInterface.UiBuilder.Draw -= this.windowSystem.Draw;
            this.pluginInterface.UiBuilder.Draw -= SoundController.Update;
            this.windowSystem.RemoveAllWindows();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
