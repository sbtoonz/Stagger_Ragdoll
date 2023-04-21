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
        
        [HarmonyPatch(typeof(Player), nameof(Player.Start))]
        public static class InitialRagdollPatch
        {
            public static void Prefix(Player __instance)
            {
                __instance.gameObject.AddComponent<Ragdoller>();
                /*__instance.m_collider.center = new Vector3(-0.00128828f, 0.9708157f, 0);
                __instance.m_collider.radius = 0.08848964f;
                __instance.m_collider.height = 1.941631f;*/
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

        [HarmonyPatch(typeof(Character), nameof(Character.ApplyDamage))]
        public static class RagDollerPatch
        {
            public static void Postfix(Player __instance, HitData hit,
                bool showDamageText,
                bool triggerEffects,
                HitData.DamageModifier mod)
            {
                if (__instance == Player.m_localPlayer)
                {
                    var rd = __instance.gameObject.GetComponent<Ragdoller>();
                    if(rd.isRagDollActive)return;
                    var target = hit.m_point - __instance.transform.position;
                    rd.SetRagDoll(target.normalized);
                }
            }
        }

        [HarmonyPatch(typeof(PlayerController), nameof(PlayerController.TakeInput))]
        public static class InputPatch
        {
            public static void Postfix(PlayerController __instance,ref bool __result)
            {
                if (__instance.gameObject.GetComponent<Ragdoller>().isRagDollActive) __result = false;
            }
        }
    }
}