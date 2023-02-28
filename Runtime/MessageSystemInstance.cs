using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bica.MessageSystem.Runtime {

    public class MessageSystemInstance : MonoBehaviour {

        public MessageSystemInstance prefab = default;

        private static MessageSystemInstance Instance { get; set; }
        private static readonly List<Action> actions = new List<Action>();

        private void Awake() {
            SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        }

        private void Update() {
            for (int i = actions.Count - 1; i >= 0; i--) {
                actions[i]?.Invoke();
            }
        }

        private void OnApplicationFocus(bool focus) {
            Channel.Unity.Send(UnityET.OnApplicationFocus, focus);
        }

        private void OnApplicationPause(bool pause) {
            Channel.Unity.Send(UnityET.OnApplicationPause, pause);
        }

        private void OnApplicationQuit() {
            Channel.Unity.Send(UnityET.OnApplicationQuit);
        }

        private void SceneManager_sceneLoaded(Scene scene, LoadSceneMode loadSceneMode) {
            Channel.Unity.Send(UnityET.SceneLoaded, new TwoValues<Scene, LoadSceneMode>(scene, loadSceneMode));
        }

        internal static void Add(Action action) {
            if (actions.Contains(action)) {
                Debug.LogError("[MessageSystem] Action duplicated");
                return;
            }

            actions.Add(action);
        }

        internal static void Remove(Action action) {
            if (!actions.Contains(action)) {
                Debug.LogError("[MessageSystem] Action non-existant");
                return;
            }

            actions.Remove(action);
        }

        public static void CreateIfNotExist() {
            if (Instance == null) {
                GameObject gameObject = new GameObject("[MessageSystem]");
                DontDestroyOnLoad(gameObject);
                Instance = gameObject.AddComponent<MessageSystemInstance>();
            }
        }
    }
}