
using UnityEngine;

namespace TapTap.Common.Standalone
{
    /// <summary>
    /// 防沉迷轮询器
    /// </summary>
    internal class TapCommonBridgePoll : MonoBehaviour 
    {
        static readonly string TAP_COMMON_POLL_NAME = "TapCommonBridgePoll";

        static TapCommonBridgePoll current;

    
        internal static void StartUp() 
        {
            TapLogger.Debug("TapCommonBridgePoll StartUp " );
            if (current == null) 
            {
                GameObject pollGo = new GameObject(TAP_COMMON_POLL_NAME);
                DontDestroyOnLoad(pollGo);
                current = pollGo.AddComponent<TapCommonBridgePoll>();
            }
        }
        
        
        private void Update()
        {
#if UNITY_STANDALONE_WIN
           TapClientBridge.TapSDK_RunCallbacks();
#endif           
        }
    }

}
