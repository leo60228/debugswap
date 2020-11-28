using System;
using System.Reflection;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using AdventureCore;

namespace Debugswap {
    public class Patches {
        internal static ManualLogSource Logger;

        [HarmonyPatch(typeof(SceneNames), "Awake")]
        [HarmonyPostfix]
        private static void SceneNamesAwake(ref string[] ___scenes) {
            Logger.LogInfo($"Loading {DebugswapPlugin.SceneNames.Length} scene names...");
            int len = ___scenes.Length;
            Array.Resize(ref ___scenes, len + DebugswapPlugin.SceneNames.Length);
            DebugswapPlugin.SceneNames.CopyTo(___scenes, len);
        }
    }
}
