using System;
using System.Runtime.InteropServices;
using System.Text;


namespace TapTap.Common.Standalone.Internal
{

    internal enum TapSDKInitResult
    {
        // 初始化成功
        OK = 0,
        // 其他错误
        FailedGeneric = 1,
        // 未找到 TapTap，用户可能未安装，请引导用户下载安装 TapTap
        NoPlatform = 2,
        // 已安装 TapTap，游戏未通过 TapTap 启动
        NotLaunchedByPlatform = 3,

        PlatformVersionMismatch = 4, // 平台版本不匹配，请引导用户升级 TapTap 与游戏至最新版本，再重新运行游戏


        // SDK 本地执行时未知错误
        Unknown = -1
    };

    public enum TapEventID
    {
        Unknown = 0,

        // [1, 2000), reserved for TapTap platform events
        // 1 reserved for TapTap platform events
        // SystemStateChanged = 1,

        // [2001, 4000), reserved for TapTap user events
        AuthorizeFinished_internal = 2001,

        AuthorizeFinished = 2002,

        // [4001, 6000), reserved for TapTap ownership events
        GamePlayableStatusChanged = 4001,
        DLCPlayableStatusChanged = 4002,


    };

    // 系统事件类型
    internal enum SystemState
    {
        kSystemState_Unknown = 0,            // 未知
        kSystemState_PlatformExit = 1,          // 平台退出
    };

    // 是否触发授权的返回结果
    public enum AuthorizeResult
    {
        UNKNOWN = 0, // 未知
        OK = 1, // 成功触发授权
        FAILED = 2, // 授权失败
    };

   #if UNITY_STANDALONE_WIN

    public class TapClientBridge
    {


        public const string DLL_NAME = "taptap_api";



        [global::System.Runtime.InteropServices.DllImport(TapClientBridge.DLL_NAME, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool TapSDK_RestartAppIfNecessary([MarshalAs(UnmanagedType.LPStr)] string clientId);

        [global::System.Runtime.InteropServices.DllImport(TapClientBridge.DLL_NAME, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int TapSDK_Init(StringBuilder errMsg, [MarshalAs(UnmanagedType.LPStr)] string pubKey);

        [global::System.Runtime.InteropServices.DllImport(TapClientBridge.DLL_NAME, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void TapSDK_Shutdown();

        // 定义与 C 兼容的委托
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void CallbackDelegate(int id, IntPtr userData);

        // 系统状态返回结果结构体
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        internal struct SystemStateResponse
        {
            public SystemState state; // 枚举直接映射
        }

        [global::System.Runtime.InteropServices.DllImport(TapClientBridge.DLL_NAME, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void TapSDK_RegisterCallback(int callbackId, IntPtr callback);

        [global::System.Runtime.InteropServices.DllImport(TapClientBridge.DLL_NAME, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void TapSDK_RunCallbacks();

        [global::System.Runtime.InteropServices.DllImport(TapClientBridge.DLL_NAME, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void TapSDK_UnregisterCallback(int callbackId, IntPtr callback);


        // 登录相关接口

        // 授权返回结果结构体
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct AuthorizeFinishedResponse_internal
        {
            public int is_cancel; // 是否取消

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
            public string callback_uri; 

        }


        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct AuthorizeFinishedResponse
        {
            public byte is_cancel;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
            public string error;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string token_type;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8 * 1024)]
            public string kid;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8 * 1024)]
            public string mac_key;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string mac_algorithm;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
            public string scope;
        };

        [global::System.Runtime.InteropServices.DllImport(TapClientBridge.DLL_NAME, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool TapUser_GetOpenID(StringBuilder openId);

        [global::System.Runtime.InteropServices.DllImport(TapClientBridge.DLL_NAME, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool TapSDK_GetClientID(StringBuilder clientId);


        [global::System.Runtime.InteropServices.DllImport(TapClientBridge.DLL_NAME, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int TapUser_AsyncAuthorize_internal([MarshalAs(UnmanagedType.LPStr)] string scopeStrings, [MarshalAs(UnmanagedType.LPStr)] string responseType,
        [MarshalAs(UnmanagedType.LPStr)] string redirectUri, [MarshalAs(UnmanagedType.LPStr)] string codeChallenge, [MarshalAs(UnmanagedType.LPStr)] string state,
        [MarshalAs(UnmanagedType.LPStr)] string codeChallengeMethod, [MarshalAs(UnmanagedType.LPStr)] string versonCode, [MarshalAs(UnmanagedType.LPStr)] string sdkUa, [MarshalAs(UnmanagedType.LPStr)] string info);

        [global::System.Runtime.InteropServices.DllImport(TapClientBridge.DLL_NAME, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int TapUser_AsyncAuthorize([MarshalAs(UnmanagedType.LPStr)] string scopeStrings);

        // DLC 接口
        // 检查是否拥有当前游戏
        [global::System.Runtime.InteropServices.DllImport(TapClientBridge.DLL_NAME, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool TapApps_IsOwned();

        // 游戏本体可玩状态变更事件响应结构体
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct GamePlayableStatusChangedResponse
        {
            public byte is_playable;            // 游戏本体是否可玩
        };

        // 显示指定 DLC 的商店页面
        [global::System.Runtime.InteropServices.DllImport(TapClientBridge.DLL_NAME, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool TapDLC_ShowStore([MarshalAs(UnmanagedType.LPStr)] string dlcId);

        // 查询用户是否拥有指定的 DLC
        [global::System.Runtime.InteropServices.DllImport(TapClientBridge.DLL_NAME, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool TapDLC_IsOwned([MarshalAs(UnmanagedType.LPStr)] string dlcId);

        // DLC 授权完成响应结果
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct DLCPlayableStatusChangedResponse
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string dlc_id; // DLC ID

            public byte is_playable; // 是否可玩，当用户购买 DLC （外置 DLC 为购买且下载完成后），此值返回 true。其他情况返回 false

        }

        // 初始化检查
        internal static int CheckInitState(out string errMessage, string key)
        {
            TapLogger.Debug("CheckInitState start ");
            StringBuilder errMsgBuffer = new StringBuilder(1024); // 分配 1024 字节缓冲区
            int result = TapSDK_Init(errMsgBuffer, key);
            errMessage = errMsgBuffer.ToString();
            TapLogger.Debug("CheckInitState result = " + result);
            return result;
        }

        // 预防 GC 回收的静态变量
        private static CallbackDelegate _dlcCallbackInstance;

        private static CallbackDelegate _userCallbackInternalInstance;

        private static CallbackDelegate _userCallbackInstance;

        private static CallbackDelegate _licenseCallbackInstance;

        public static void RegisterCallback(TapEventID eventID, CallbackDelegate callback)
        {

            IntPtr funcPtr = Marshal.GetFunctionPointerForDelegate(callback);
            switch (eventID)
            {
                case TapEventID.DLCPlayableStatusChanged:
                    if (_dlcCallbackInstance != null)
                    {
                        UnRegisterCallback(eventID, _dlcCallbackInstance);
                    }
                    _dlcCallbackInstance = callback;
                    break;
                case TapEventID.GamePlayableStatusChanged:
                    if (_licenseCallbackInstance != null)
                    {
                        UnRegisterCallback(eventID, _licenseCallbackInstance);
                    }
                    _licenseCallbackInstance = callback;
                    break;
                case TapEventID.AuthorizeFinished_internal:
                    if (_userCallbackInternalInstance != null)
                    {
                        UnRegisterCallback(eventID, _userCallbackInternalInstance);
                    }
                    _userCallbackInternalInstance = callback;
                    break;
                case TapEventID.AuthorizeFinished:
                    if (_userCallbackInstance != null)
                    {
                        UnRegisterCallback(eventID, _userCallbackInstance);
                    }
                    _userCallbackInstance = callback;
                    break;
            }

            TapSDK_RegisterCallback((int)eventID, funcPtr);
        }

        // 移除回调
        public static void UnRegisterCallback(TapEventID eventID, CallbackDelegate callback)
        {

            IntPtr funcPtr = Marshal.GetFunctionPointerForDelegate(callback);
            switch (eventID)
            {
                case TapEventID.DLCPlayableStatusChanged:
                    _dlcCallbackInstance = null;
                    break;
                case TapEventID.GamePlayableStatusChanged:
                    _licenseCallbackInstance = null;
                    break;
                case TapEventID.AuthorizeFinished_internal:
                    _userCallbackInternalInstance = null;
                    break;
                case TapEventID.AuthorizeFinished:
                    _userCallbackInstance = null;
                    break;
            }

            TapSDK_UnregisterCallback((int)eventID, funcPtr);
        }

        internal static AuthorizeResult LoginWithScopesInternal(string[] scopeStrings, string responseType, string redirectUri,
        string codeChallenge, string state, string codeChallengeMethod, string versonCode, string sdkUa, string info)
        {
            try
            {
                int result = TapUser_AsyncAuthorize_internal(string.Join(",", scopeStrings), responseType, redirectUri,
     codeChallenge, state, codeChallengeMethod, versonCode, sdkUa, info);
                return (AuthorizeResult)result;
            }
            catch (Exception ex)
            {
                TapLogger.Debug("login crash = " + ex.Message);
                return AuthorizeResult.UNKNOWN;
            }
        }

        public static AuthorizeResult LoginWithScopes(string[] scopeStrings)
        {
            try
            {
                int result = TapUser_AsyncAuthorize(string.Join(",", scopeStrings));
                return (AuthorizeResult)result;
            }
            catch (Exception ex)
            {
                TapLogger.Debug("login crash = " + ex.Message);
                return AuthorizeResult.UNKNOWN;
            }
        }

        internal static bool GetTapUserOpenId(out string openId)
        {
            StringBuilder openIdBuffer = new StringBuilder(256); // 分配一个足够大的缓冲区
            bool success = TapUser_GetOpenID(openIdBuffer);  // 调用 C 函数
            openId = openIdBuffer.ToString();
            return success;
        }

        internal static bool GetClientId(out string clientId)
        {
            StringBuilder clientIDBuffer = new StringBuilder(256); // 分配一个足够大的缓冲区
            bool success = TapSDK_GetClientID(clientIDBuffer);  // 调用 C 函数
            clientId = clientIDBuffer.ToString();
            return success;
        }

        internal static bool QueryDLC(string skuId)
        {
            return TapDLC_IsOwned(skuId);
        }

        internal static bool ShowDLCStore(string skuId)
        {
            return TapDLC_ShowStore(skuId);
        }

        internal static bool HasLicense()
        {
            return TapApps_IsOwned();
        }


    }
#endif


}