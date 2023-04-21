using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RagDoller.MonoScripts;
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
                __instance.gameObject.AddComponent<Ragdoller>();
                __instance.m_collider.center = new Vector3(-0.00128828f, 0.9708157f, 0);
                __instance.m_collider.radius = 0.08848964f;
                __instance.m_collider.height = 1.941631f;
            }
        }
        
        [HarmonyPatch(typeof(Player), nameof(Player.OnRespawn))]
        public static class InitialRagdollPatch2
        {
            public static void Prefix(Player __instance)
            {
                __instance.gameObject.AddComponent<Ragdoller>();
                __instance.m_collider.center = new Vector3(-0.01229408f, 0.09949544f, 0);
                __instance.m_collider.radius = 0.09949544f;
                __instance.m_collider.height = 0.1989909f;
            }
        }

        [HarmonyPatch(typeof(Terminal), nameof(Terminal.InputText))]
        public static class chatPatchTest
        {
            public static bool Prefix(Terminal __instance)
            {
                string lower = __instance.m_input.text.ToLower();
                if (lower.Equals("/ragdoll"))
                {
                    Player.m_localPlayer.gameObject.GetComponent<Ragdoller>().SetRagDoll(Player.m_localPlayer.transform.position);
                    return false;
                }
                return true;
            }
        }
    }
}