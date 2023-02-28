using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bica.MessageSystem.Runtime {

    public class Channel<T> : Channel where T : Enum {

        #region Static

        private static readonly List<Channel<T>> channels = new List<Channel<T>>();
        public static Channel<T>[] Channels => channels.ToArray();

        #endregion Static

        private bool globalUpdateAttached = false;
        private readonly Dictionary<T, List<Action<IMessage<T>>>> listeners = new Dictionary<T, List<Action<IMessage<T>>>>();
        private readonly List<IMessage<T>> pendingMessages = new List<IMessage<T>>();

        public Channel() {
            channels.Add(this);
        }

        public void Attach(T enumType, Action<IMessage<T>> handler) {
            if (handler == null) {
                Debug.LogError($"[{nameof(Channel<T>)}.{nameof(Attach)}] {nameof(Action<IMessage<T>>)} is null");
                return;
            }

            MessageSystemInstance.CreateIfNotExist();

            if (!listeners.ContainsKey(enumType)) {
                listeners.Add(enumType, new List<Action<IMessage<T>>>());
            }

            List<Action<IMessage<T>>> listenerList = listeners[enumType];

            if (listenerList.Contains(handler)) {
                Debug.LogWarning($"[{nameof(Channel<T>)}.{nameof(Attach)}] Attached existant listener: {enumType}");
            } else {
                listenerList.Add(handler);
            }
        }

        public void Detach(T enumType, Action<IMessage<T>> handler) {
            if (!listeners.ContainsKey(enumType)) {
                Debug.LogWarning($"[{nameof(Channel<T>)}.{nameof(Detach)}] Detached non-existant listener: {enumType}");
            } else {
                listeners[enumType].Remove(handler);
                if (listeners[enumType].Count == 0) {
                    listeners.Remove(enumType);
                }
            }
        }

        public bool Send(T enumType, object content = null, bool isSenderAsync = false) {
            return Send(new Message<T>(enumType, content, isSenderAsync));
        }

        public bool Send(IMessage<T> message) {
            if (message == null) {
                Debug.LogError($"[{nameof(Channel<T>)}.{nameof(Send)}] {nameof(IMessage<T>)} is null");
                return false;
            }

            message.SetChannel(this);

            if (!message.IsSenderAsync && !listeners.ContainsKey(message.EnumType)) {
                return false;
            }

            if (message.IsSenderAsync) {
                if (!globalUpdateAttached) {
                    globalUpdateAttached = true;
                    MessageSystemInstance.Add(OnUpdateGlobal);
                }

                MessageSystemInstance.CreateIfNotExist();
                pendingMessages.Add(message);
            } else {
                SendMessage(message, listeners[message.EnumType].ToArray());
            }

            return true;
        }

        private void OnUpdateGlobal() {
            globalUpdateAttached = false;
            MessageSystemInstance.Remove(OnUpdateGlobal);

            IMessage<T>[] messagesToSend = pendingMessages.ToArray();

            for (int i = 0; i < messagesToSend.Length; i++) {
                IMessage<T> message = messagesToSend[i];

                if (!listeners.ContainsKey(message.EnumType)) {
                    continue;
                }

                Action<IMessage<T>>[] handlers = listeners[message.EnumType].ToArray();
                SendMessage(message, handlers);
            }

            pendingMessages.Clear();
        }

        private void SendMessage(IMessage<T> message, Action<IMessage<T>>[] handlers) { // handlers should be an Array to avoid a Detach when we are calling the listeners.
            for (int i = 0; i < handlers.Length; i++) {
                try {
                    handlers[i](message); // We do not need check if this action is null because we did it in the Attach function.
                } catch (Exception ex) {
                    string error = $"[{nameof(Channel<T>)}.{nameof(SendMessage)}] {nameof(SendMessage)}({message.EnumType}, {handlers}, {message.Content}); ";
                    if (ex == null) {
                        error += "Exception is null";
                    } else {
                        error += ex.ToString();
                    }

                    Debug.LogError(error);
                }
            }
        }

        public void DetachAll(bool clearPendings = true) {
            listeners.Clear();

            if (clearPendings) {
                pendingMessages.Clear();
            }
        }
    }
}