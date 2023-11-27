using FrooxEngine;
using HarmonyLib;
using ResoniteModLoader;

namespace SoNoHeadCrash
{
    public static class ModInfo
    {
        public const string NAME = "So No Head Crash?";
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

        [AutoRegisterConfigKey]
        private static readonly ModConfigurationKey<bool> FIX_479 = new ModConfigurationKey<bool>("Fixes issue #479", "Fix issue #479", () => true);

        private static ModConfiguration config;

        public override void OnEngineInit()
        {
            config = GetConfiguration();
            Harmony harmony = new Harmony("dev.lecloutapnda.SoNoHeadCrash");
            harmony.PatchAll();
        }

        [HarmonyPatch(typeof(InteractionHandler), nameof(InteractionHandler.Dequip))]
        private static class FIX_479_PATCH
        {
            [HarmonyPrefix]
            public static bool Prefix(InteractionHandler __instance, bool popOff)
            {
                if (!config.GetValue(FIX_479)) return true;

                if (__instance.World.HostUser == __instance.LocalUser)
                {
                    if (__instance.LocalUser.HeadDevice == HeadOutputDevice.Headless)
                    {
                        __instance.Dequip(false);
                    }
                    else
                    {
                        __instance.Dequip(true);
                    }
                }

                return false;
            }
        }
    }
}
