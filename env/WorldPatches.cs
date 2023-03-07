using HarmonyLib;
using UnityEngine;

namespace VivoVR.env
{
    [HarmonyPatch]
    internal class WorldPatches
    {
        // In the menu screen, the camera spawns too close to the tunnel so instead of having to 
        // constantly set the camera position backwards on an update function, I use this once on start
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Menu_tunnel), "Start")]
        private static void moveMenuTunnel(Menu_tunnel __instance)
        {
            Vector3 menuTunnelPos = __instance.transform.position;
            menuTunnelPos.y = menuTunnelPos.y + 0.4f;
            __instance.transform.position = menuTunnelPos;
        }
    }
}
