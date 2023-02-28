using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bica.MessageSystem.Runtime {

    public interface IMessage {

        public object Content { get; }
        public bool IsSenderAsync { get; }

        public bool GetContent<T>(out T content);

        public static bool GetContent<T>(IMessage message, out T content) {
            if (message == null) {
                content = default;
                Debug.LogError($"[{nameof(IMessage)}.{nameof(GetContent)}] {nameof(message)} is null");
                return false;
            }

            return message.GetContent(out content);
        }
    }

    public interface IMessage<TEnum> : IMessage where TEnum : Enum {

        public TEnum EnumType { get; }
        public Channel<TEnum> Channel { get; }

        public void SetChannel(Channel<TEnum> channel);
    }

    public class Message<TEnum> : IMessage<TEnum> where TEnum : Enum {

        public TEnum EnumType { get; private set; }
        public object Content { get; private set; }
        public bool IsSenderAsync { get; private set; }
        public Channel<TEnum> Channel { get; set; }

        public Message(TEnum enumType, object content = null, bool isSenderAsync = false) {
            EnumType = enumType;
            Content = content;
            IsSenderAsync = isSenderAsync;
        }

        public void SetChannel(Channel<TEnum> channel) {
            if (Channel == null || channel == null) {
                return;
            }

            Channel = channel;
        }

        public bool GetContent<T>(out T content) {
            bool isCastable = Content is T;

            if (isCastable) {
                content = (T)Content;
                return true;
            }

            content = default;
            Debug.LogError($"[{nameof(Message<TEnum>)}.{nameof(GetContent)}] Error Casting: {Content} to {typeof(T).FullName}");
            return false;
        }
    }

    public sealed class TwoValues<T1, T2> {
        public T1 Value1 { get; private set; }
        public T2 Value2 { get; private set; }

        public TwoValues(T1 value1, T2 value2) {
            Value1 = value1;
            Value2 = value2;
        }
    }

    public sealed class ItemsContainer<T> {

        private readonly List<T> items = new List<T>();

        public ItemsContainer(List<T> items = null) {
            if (items == null) {
                this.items = new List<T>();
            } else {
                this.items = items;
            }
        }

        public void AddItem(T item) => items.Add(item);
        public T[] GetItems() => items.ToArray();
    }

    public sealed class CallbackContainer {

        public event Action callback;

        public CallbackContainer(Action callback) {
            this.callback = callback;
        }

        public void ExecuteCallback() {
            callback?.Invoke();
        }

        public void ClearCallback() {
            callback = null;
        }
    }

    public sealed class CallbackContainer<T> {

        public event Action<T> callback;

        public CallbackContainer(Action<T> callback) {
            this.callback = callback;
        }

        public void ExecuteCallback(T obj) {
            callback?.Invoke(obj);
        }

        public void ClearCallback() {
            callback = null;
        }
    }
}