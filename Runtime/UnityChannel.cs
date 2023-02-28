namespace Bica.MessageSystem.Runtime {

    public abstract partial class Channel {

        public readonly static Channel<UnityET> Unity = new Channel<UnityET>();
    }

    public enum UnityET {
        None,
        OnApplicationFocus,
        OnApplicationPause,
        OnApplicationQuit,
        SceneLoaded,
    }
}