using NLog;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Controls;
using HarmonyLib;
using NLog.Fluent;
using Sandbox.Engine.Multiplayer;
using Sandbox.Game.World;
using Torch;
using Torch.API;
using Torch.API.Managers;
using Torch.API.Plugins;
using Torch.API.Session;
using Torch.Session;

namespace ChatIconPatch
{
    public class ChatIconPatch : TorchPluginBase, IWpfPlugin
    {

        public static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private static readonly string CONFIG_FILE_NAME = "ChatIconPatchConfig.cfg";

        private ChatIconPatchControl _control;
        public UserControl GetControl() => _control ?? (_control = new ChatIconPatchControl(this));

        private Persistent<ChatIconPatchConfig> _config;
        public ChatIconPatchConfig Config => _config?.Data;

        public override void Init(ITorchBase torch)
        {
            base.Init(torch);

            SetupConfig();

            var sessionManager = Torch.Managers.GetManager<TorchSessionManager>();
            if (sessionManager != null)
                sessionManager.SessionStateChanged += SessionChanged;
            else
                Log.Warn("No session manager loaded!");

            Save();
            
            new Harmony("ChatIconPatch").PatchAll();

            
        }
        
        

        private void SessionChanged(ITorchSession session, TorchSessionState state)
        {

            switch (state)
            {

                case TorchSessionState.Loaded:
                    Log.Info("Session Loaded!");
                    
                    break;

                case TorchSessionState.Unloading:
                    Log.Info("Session Unloading!");
                    break;
            }
        }

        public async Task awaitThenDo(ulong steamid, string s)
        {
            await Task.Delay(5000);

            var plr = MySession.Static.Players.GetPlayerByName(s);
            if (plr != null)
            {
                Log.Warn($"Fixed Player from name of {s}");
                Traverse.Create(plr).Field("DisplayName").SetValue(s.Substring(1));
                Traverse.Create(plr.Identity).Field("DisplayName").SetValue(s.Substring(1));
            }
        }

        private void SetupConfig()
        {

            var configFile = Path.Combine(StoragePath, CONFIG_FILE_NAME);

            try
            {

                _config = Persistent<ChatIconPatchConfig>.Load(configFile);

            }
            catch (Exception e)
            {
                Log.Warn(e);
            }

            if (_config?.Data == null)
            {

                Log.Info("Create Default Config, because none was found!");

                _config = new Persistent<ChatIconPatchConfig>(configFile, new ChatIconPatchConfig());
                _config.Save();
            }
        }

        public void Save()
        {
            try
            {
                _config.Save();
                Log.Info("Configuration Saved.");
            }
            catch (IOException e)
            {
                Log.Warn(e, "Configuration failed to save");
            }
        }
    }
}
