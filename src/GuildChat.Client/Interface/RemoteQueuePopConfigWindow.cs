using System.Numerics;
using Dalamud.Logging;
using GuildChat.Client.Configuration;
using ImGuiNET;

namespace GuildChat.Client.Interface
{
    internal class RemoteQueuePopConfigWindow
    {
        private bool IsOpen = false;
        private RemoteQueuePopConfig _remoteQueuePopConfig;

        public RemoteQueuePopConfigWindow()
        {
            _remoteQueuePopConfig = RemoteQueuePopPlugin.DalamudPluginInterface.GetPluginConfig() as RemoteQueuePopConfig ?? new RemoteQueuePopConfig();
        }

        public void DrawRichPresenceConfigWindow()
        {
            if (!IsOpen)
            {
                return;
            }

            ImGui.SetNextWindowSize(new Vector2(750, 520));
            var imGuiReady = ImGui.Begin("Remote Queue Pop Alerts",
                ref IsOpen);

            if (imGuiReady)
            {
                ImGui.Text("Welcome to the Remote Queue Pop Alert plugin settings.");
                ImGui.Separator();
                ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(1, 3));
                ImGui.Text("You'll need a Discord token from the Discord developer console. Make sure it's in one of your servers, so it can DM you.");
                ImGui.InputText("Discord token", ref _remoteQueuePopConfig.Token, 100);
                ImGui.Separator();

                ImGui.PopStyleVar();

                ImGui.Separator();

                if (ImGui.Button("Start"))
                {
                }
                if (ImGui.Button("Save and close"))
                {
                    this.Close();
                    RemoteQueuePopPlugin.DalamudPluginInterface.SavePluginConfig(_remoteQueuePopConfig);
                    RemoteQueuePopPlugin.RemoteQueuePopConfig = this._remoteQueuePopConfig;
                    PluginLog.Log("Settings saved.");
                }

                ImGui.End();
            }
        }

        public void Open()
        {
            this.IsOpen = true;
        }

        public void Close()
        {
            this.IsOpen = false;
        }

        public void Toggle()
        {
            this.IsOpen = !this.IsOpen;
        }
    }
}