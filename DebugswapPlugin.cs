using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using BepInEx;
using AdventureCore;
using UnityEngine;
using UnityEngine.SceneManagement;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Debugswap {
    [BepInPlugin("space.leo60228.plugins.debugswap", "Debugswap", "1.0.0.0")]
    public class DebugswapPlugin : BaseUnityPlugin {
        internal static string[] SceneNames;

        void Awake() {
            Logger.LogInfo("Debugswap awake");
            int count = UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;
            SceneNames = new string[count];
            for (int i = 0; i < count; i++) {
                Logger.LogInfo($"loading scene #{i}");
                string path = SceneUtility.GetScenePathByBuildIndex(i);
                Logger.LogInfo($"path is {path}");
                string name = Path.GetFileNameWithoutExtension(path);
                Logger.LogInfo($"name is {name}");
                SceneNames[i] = name;
            }
            Logger.LogInfo("Patching SceneNames...");
            Patches.Logger = Logger;
            Harmony.CreateAndPatchAll(typeof(Patches));
        }

        void Update() {
            Hud hud = GameManager.instance.Hud;
            if (Input.GetKeyDown("f3")) {
                if (hud.State == HudState.Debug) {
                    hud.ReturnToDefaultState();
                } else {
                    hud.ChangeState(HudState.Debug);
                }
            } else if (Input.GetKeyDown("f5")) {
                GameObject menu = hud.DebugCameraMenu.gameObject;
                menu.SetActive(!menu.activeSelf);
            } else if (Input.GetKeyDown("f4")) {
                Scene scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
                Logger.LogInfo($"current scene is {scene.name}");
                Logger.LogInfo($"transitioning scene is {SceneWrangler.instance.TransitionScene ?? "(null)"}");
                Logger.LogInfo($"current state is {hud.State}");
            }
        }

        void OnGUI() {
            if (Input.GetKeyDown("f6")) {
                Logger.LogInfo($"dumping hints...");
                HintButton hints = UnityEngine.Object.FindObjectsOfType<HintButton>()[0];
                ISerializer serializer = new SerializerBuilder()
                    .IncludeNonPublicProperties()
                    .WithTypeConverter(new NonSerializableTypeConverter())
                    .Build();
                string yaml = serializer.Serialize(hints);
                File.WriteAllText("hints.yml", yaml);
                Logger.LogInfo("dumped!");
            }
        }

        public class NonSerializableTypeConverter : IYamlTypeConverter {
            public bool Accepts(Type type) {
                return typeof(UnityEngine.Color).IsAssignableFrom(type);
            }

            public object ReadYaml(IParser parser, Type type) {
                throw new NotImplementedException();
            }

            public void WriteYaml(IEmitter emitter, object value, Type type) {
                emitter.Emit(new Scalar(((UnityEngine.Color)value).ToString()));
            }
        }
    }
}
