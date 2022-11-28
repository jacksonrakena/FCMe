using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Dalamud;
using Dalamud.Data;
using Dalamud.Game;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Party;
using Dalamud.Game.Command;
using Dalamud.Game.Gui;
using Dalamud.IoC;
using Dalamud.Logging;
using Dalamud.Plugin;
using Discord;
using Discord.WebSocket;
using Lumina;
using Lumina.Excel.GeneratedSheets;
using RemoteQueuePop.Configuration;
using RemoteQueuePop.Interface;
using RemoteQueuePop.Sinks;


namespace RemoteQueuePop
{
    internal class RemoteQueuePopPlugin : IDalamudPlugin, IDisposable
    {
        [PluginService]
        internal static DalamudPluginInterface DalamudPluginInterface { get; private set; }

        [PluginService]
        internal static ClientState ClientState { get; private set; }

        [PluginService]
        internal static ChatGui Chat { get; private set; }
        
        [PluginService]
        internal static CommandManager CommandManager { get; private set; }

        [PluginService]
        internal static DataManager DataManager { get; private set; }

        [PluginService]
        internal static Framework Framework { get; private set; }

        [PluginService]
        internal static PartyList PartyList { get; private set; }

        private static RemoteQueuePopConfigWindow configWindow;
        internal static RemoteQueuePopConfig RemoteQueuePopConfig { get; set; }

        private static List<IRemoteSink> Sinks = new List<IRemoteSink>
        {
            new DiscordSink(),
            new TwilioSink()
        };

        private List<TerritoryType> Territories;

        public RemoteQueuePopPlugin()
        {
            RemoteQueuePopConfig = DalamudPluginInterface.GetPluginConfig() as RemoteQueuePopConfig ?? new RemoteQueuePopConfig();
            configWindow = new RemoteQueuePopConfigWindow();
            DalamudPluginInterface.UiBuilder.Draw += configWindow.DrawRichPresenceConfigWindow;
            DalamudPluginInterface.UiBuilder.OpenConfigUi += configWindow.Open;
            
            foreach (var sink in Sinks)
            {
                sink.StartAsync(RemoteQueuePopConfig).GetAwaiter().GetResult();
            }
            
            ClientState.CfPop += ClientStateOnCfPop;
            
            RegisterCommand();
            
            DalamudPluginInterface.LanguageChanged += ReregisterCommand;

            Territories = DataManager.GetExcelSheet<TerritoryType>().ToList();
        }

        public async Task TryAuthorizeAsync()
        {
            Chat.
        }

        private void ClientStateOnCfPop(object sender, ContentFinderCondition e)
        {
            Task.Run(async () =>
            {
                foreach (var sink in Sinks)
                {
                    try
                    {
                        await sink.HandleQueuePopAsync(e);
                    }
                    catch (Exception exc)
                    {
                        PluginLog.LogInformation("Sink " + sink.GetType().Name + " failed: " + exc.Message);
                    }
                }
            });
        }

        public string Name => "Remote Queue Pop Alerts";

        public void Dispose()
        {
            DalamudPluginInterface.LanguageChanged -= ReregisterCommand;
            UnregisterCommand();
            ClientState.CfPop -= ClientStateOnCfPop;
            DalamudPluginInterface.UiBuilder.OpenConfigUi -= configWindow.Open;
            DalamudPluginInterface.UiBuilder.Draw -= configWindow.DrawRichPresenceConfigWindow;
            foreach (var sink in Sinks)
            {
                sink.StopAsync(RemoteQueuePopConfig).GetAwaiter().GetResult();
            }
        }


        private void ReregisterCommand(string langCode)
        {
            this.UnregisterCommand();
            this.RegisterCommand();
        }

        private void UnregisterCommand()
        {
            CommandManager.RemoveHandler("/prp");
        }

        private void RegisterCommand()
        {
            CommandManager.AddHandler("/prqp",
                new CommandInfo((string cmd, string args) => configWindow.Toggle())
                {
                    HelpMessage = "Opens the configuration window."
                }
            );
        }
    }
}