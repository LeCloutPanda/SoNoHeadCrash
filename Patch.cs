﻿using FrooxEngine;
using FrooxEngine.CommonAvatar;
using HarmonyLib;
using ResoniteModLoader;

namespace SoNoHeadCrash
{

    /* Credits
     * CoolyMike(https://github.com/coolymike) and Raidriar(https://github.com/Raidriar796) for the name suggestion
     * CoolyMike(https://github.com/coolymike), JanoshcABR(https://github.com/JanoschABR) for helping me to test it
     */

    public static class ModInfo
    {
        public const string NAME = "SoNoHeadCrash";
        public const string DESCRIPTION = "Fixes a series of Headless related bugs.";
        public const string COMPANY = "Pandas Hell Hole";
        public const string URL = "https://github.com/LeCloutPanda/SoNoHeadCrash/";
        public const string AUTHOR = "LeCloutPanda";
        public const string VERSION = "0.0.1";
    }

    public class Patch : ResoniteMod
    {
        public override string Name => ModInfo.NAME;
        public override string Link => ModInfo.URL;
        public override string Author => ModInfo.AUTHOR;
        public override string Version => ModInfo.VERSION;

        [AutoRegisterConfigKey] private static readonly ModConfigurationKey<bool> FIX_479 = new ModConfigurationKey<bool>("", "Fix issue #479", () => true);
        [AutoRegisterConfigKey] private static readonly ModConfigurationKey<bool> FIX_399 = new ModConfigurationKey<bool>("", "Fix issue #399", () => true);
        [AutoRegisterConfigKey] private static readonly ModConfigurationKey<bool> FIX_SCALECOMP = new ModConfigurationKey<bool>("", "FIX_SCALECOMP", () => true);

        private static ModConfiguration config;

        public override void OnEngineInit()
        {
            config = GetConfiguration();
            Harmony harmony = new Harmony("dev.lecloutapnda.SoNoHeadCrash");
            harmony.PatchAll();
        }

        // Fix issue 479(https://github.com/Yellow-Dog-Man/Resonite-Issues/issues/479)
        [HarmonyPatch(typeof(InteractionHandler), "ToolDequipped")]
        private static class FIX_479_PATCH
        {
            [HarmonyPrefix]
            public static void Prefix(InteractionHandler __instance, ITool tooltip, ref bool popOff)
            {
                if (!config.GetValue(FIX_479)) return;
                
                if (__instance.World.HostUser.HeadDevice == HeadOutputDevice.Headless)
                {
                    popOff = false;
                    Msg("Prevented a Headless crash.");
                
                }
                else
                {
                    popOff = true;
                    Msg("Didn't need to save the headless");
                }
            }
        }

        // Fixes issue 399(https://github.com/Yellow-Dog-Man/Resonite-Issues/issues/399)
        [HarmonyPatch(typeof(Slider), "OnAwake")]
        private static class FIX_399_SLIDER_PATCH
        {
            [HarmonyPostfix]
            public static void Postfix(Slider __instance)
            {
                __instance.RunInUpdates(3, () =>
                {
                    if (!config.GetValue(FIX_399)) return;

                    __instance.DontDrive.Value = true;
                    Msg($"Set DontDrive for {__instance.ReferenceID} to true.");
                });
            }
        }

        // Fixes issue 399(https://github.com/Yellow-Dog-Man/Resonite-Issues/issues/399)
        [HarmonyPatch(typeof(Joint), "OnAwake")]
        private static class FIX_399_JOINT_PATCH
        {
            [HarmonyPostfix]
            public static void Postfix(Slider __instance)
            {
                __instance.RunInUpdates(3, () =>
                {
                    if (!config.GetValue(FIX_399)) return;

                    __instance.DontDrive.Value = true;
                    Msg($"Set DontDrive for {__instance.ReferenceID} to true.");
                });
            }
        }

        // Not an issue yet
        // Not working, needs valueUserOverride
        [HarmonyPatch(typeof(AvatarAudioOutputManager), nameof(AvatarAudioOutputManager.OnEquip))]
        private static class FIX_SCALECOMP_PATCH
        {
            [HarmonyPostfix]
            public static void Postfix(AvatarAudioOutputManager __instance, Sync<float> ____scaleCompensation)
            {
                if (!config.GetValue(FIX_SCALECOMP)) return;

                __instance.RunInUpdates(3, () =>
                {
                    ValueUserOverride<float> valueOverride = ____scaleCompensation.OverrideForUser(__instance.LocalUser, 1f);
                    valueOverride.Persistent = false;
                    valueOverride.Default.Value = 1;
                    Msg($"Setting scaleCompensation for user {__instance.Slot.ActiveUser.UserName} to 1");
                });
            }
        }
    }
}
