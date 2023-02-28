using Bica.MessageSystem.Runtime;
using UnityEditor;
using UnityEngine;

namespace Bica.MessageSystem.Editor {

    [CustomEditor(typeof(MessageSystemInstance))]
    public class MessageInstanceEditor : UnityEditor.Editor {

        UnityET messageType = UnityET.None;

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            EditorGUILayout.BeginHorizontal();
            {
                messageType = (UnityET)EditorGUILayout.EnumPopup("Unity Message Type", messageType);

                if (messageType != UnityET.None) {
                    if (GUILayout.Button("Send Message")) {
                        IMessage<UnityET> message = null;
                        Channel.Unity.Send(messageType, message);
                    }
                }
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}