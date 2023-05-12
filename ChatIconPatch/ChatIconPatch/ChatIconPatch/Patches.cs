using System;
using System.Collections.Generic;
using HarmonyLib;
using Sandbox;
using Sandbox.Engine.Multiplayer;
using Sandbox.Engine.Networking;
using Sandbox.Game;
using Sandbox.Game.Multiplayer;
using Sandbox.Game.World;
using Torch.API.Managers;
using Torch.Managers;
using Torch.Server.Managers;
using VRage.Dedicated.RemoteAPI;
using VRage.GameServices;
using VRage.Network;
using VRage.Utils;

namespace ChatIconPatch
{
    [HarmonyPatch]
    public static class Patches
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(MyDedicatedServerBase), "OnConnectedClient")]
        public static bool Prefix(ref MyDedicatedServerBase __instance, ref ConnectedClientDataMsg msg, ulong steamId)
        {
            if (!Char.IsLetter(msg.Name[0]) && !Char.IsNumber(msg.Name[0]))
            {
                msg.Name = msg.Name.Substring(1);
                MyIdentity identity = MySession.Static.Players.TryGetPlayerIdentity(new MyPlayer.PlayerId(steamId));
                if (identity != null)
                {
                    identity.SetDisplayName(msg.Name);
                }
                
            }
            
            return true;
        }
    }
}