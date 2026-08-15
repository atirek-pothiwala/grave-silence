#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using GraveSilence.Core;
using GraveSilence.Systems;
using GraveSilence.Player;
using GraveSilence.UI;

namespace GraveSilence.Editor
{
    public static class GraveSilenceSetup
    {
        [MenuItem("Grave Silence/Create Systems Hierarchy")]
        public static void CreateSystemsHierarchy()
        {
            var root = new GameObject("--- Systems ---");

            CreateSingleton<GameManager>(root.transform);
            CreateSingleton<InputManager>(root.transform);
            CreateSingleton<NoiseSystem>(root.transform);
            CreateSingleton<AlertSystem>(root.transform);
            CreateSingleton<ObjectiveTracker>(root.transform);
            CreateSingleton<MissionScore>(root.transform);

            Selection.activeGameObject = root;
            Debug.Log("Grave Silence systems hierarchy created.");
        }

        [MenuItem("Grave Silence/Create Player")]
        public static void CreatePlayer()
        {
            var player = new GameObject("Player");
            player.tag = GameConstants.PlayerTag;

            player.AddComponent<CharacterController>().height = 1.8f;
            player.AddComponent<PlayerReference>();
            player.AddComponent<ThirdPersonController>();
            player.AddComponent<StealthController>();
            player.AddComponent<UmbralAbilities>();
            player.AddComponent<StealthTakedown>();
            player.AddComponent<SpiritVision>();
            player.AddComponent<PlayerHealth>();
            player.AddComponent<PlayerInputHandler>();

            Selection.activeGameObject = player;
            Debug.Log("Grave Silence player created. Assign camera target on ThirdPersonController.");
        }

        private static void CreateSingleton<T>(Transform parent) where T : Component
        {
            var go = new GameObject(typeof(T).Name);
            go.transform.SetParent(parent);
            go.AddComponent<T>();
        }
    }
}
#endif
