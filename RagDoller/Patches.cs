using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace RagDoller
{
    public class Patches
    {
        [HarmonyPatch(typeof(Player), nameof(Player.OnSpawned))]
        public static class InitialRagdollPatch
        {
            public static void Prefix(Player __instance)
            {
                if (Player.m_localPlayer != null && Player.m_localPlayer == __instance)
                {
                    var t = __instance.transform.Find("Visual/Armature").gameObject;
                    Object.DestroyImmediate(t);
                    var a = InstantiatePrefab.Instantiate(RagDollerMod.RagDollObj, __instance.transform.Find("Visual"),
                        false);
                    var replace = a!.name.Replace("(Clone)", "");
                    a!.name = replace;
                }
            }
        }

        [HarmonyPatch(typeof(Player), nameof(Player.OnRespawn))]
        public static class RespawnRagdollPatch
        {
            public static void Prefix(Player __instance)
            {
                if (Player.m_localPlayer != null && Player.m_localPlayer == __instance)
                {
                    var t = __instance.transform.Find("Visual/Armature").gameObject;
                    Object.DestroyImmediate(t);
                    var a = InstantiatePrefab.Instantiate(RagDollerMod.RagDollObj, __instance.transform.Find("Visual"),
                        false);
                    var replace = a!.name.Replace("(Clone)", "");
                    a!.name = replace;
                }
            }
        }
    }
}