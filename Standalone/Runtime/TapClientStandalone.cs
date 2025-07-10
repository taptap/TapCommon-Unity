using UnityEngine;
using System.Threading.Tasks;
using TapTap.Common.Standalone.Internal;
using TapTap.UI;
using System;
using System.Runtime.InteropServices;
using System.Threading;



namespace TapTap.Common.Standalone
{
#if UNITY_STANDALONE_WIN
    public class TapClientStandalone
    {

        // 是否是渠道服游戏包
        private static bool isChannelPackage = false;

        // -1 未执行 0 失败  1 成功
        private static int lastIsLaunchedFromTapTapPCResult = -1;
        private static bool isRuningIsLaunchedFromTapTapPC = false;


        // 当为渠道游戏包时，与启动器的初始化校验结果
        private static TapInitResult tapInitResult;

        // <summary>
        // 校验游戏是否通过启动器唤起，建立与启动器通讯
        //</summary>
        public static async Task<bool> IsLaunchedFromTapTapPC()
        {
            // 正在执行中
            if (isRuningIsLaunchedFromTapTapPC)
            {
                UIManager.Instance.OpenToast("IsLaunchedFromTapTapPC 正在执行，请勿重复调用", UIManager.GeneralToastLevel.Error);
                Log(true, "IsLaunchedFromTapTapPC 正在执行，请勿重复调用");
                return false;
            }
            // 多次执行时返回上一次结果
            if (lastIsLaunchedFromTapTapPCResult != -1)
            {
                Log(false, "IsLaunchedFromTapTapPC duplicate invoke return " + lastIsLaunchedFromTapTapPCResult);
                return lastIsLaunchedFromTapTapPCResult > 0;
            }

            isChannelPackage = true;
            if (TapCommonStandalone.currentTapConfig == null)
            {
                UIManager.Instance.OpenToast("IsLaunchedFromTapTapPC 调用必须在初始化之后", UIManager.GeneralToastLevel.Error);
                 Log(true, "IsLaunchedFromTapTapPC 调用必须在初始化之后");
                return false;
            }
            string clientId = TapCommonStandalone.currentTapConfig.ClientID;
            string pubKey = TapCommonStandalone.currentTapConfig.ClientPublicKey;
            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(pubKey))
            {
                UIManager.Instance.OpenToast("clientId 及 TapPubKey 参数都不能为空, clientId =" + clientId + ", TapPubKey = " + pubKey, UIManager.GeneralToastLevel.Error);
                 Log(true, "clientId 或 TapPubKey 无效, clientId = " + clientId + ", TapPubKey = " + pubKey);
                return false;
            }
            isRuningIsLaunchedFromTapTapPC = true;
            try
            {
                TapInitResult result = await RunClientBridgeMethod(clientId, pubKey);
                Log(false, "check startupWithClientBridge finished thread = " + Thread.CurrentThread.ManagedThreadId);
                isRuningIsLaunchedFromTapTapPC = false;
                if (result.needQuitGame)
                {
                    lastIsLaunchedFromTapTapPCResult = 0;
                    Log(false, "IsLaunchedFromTapTapPC Quit game");
                    Application.Quit();
                    return false;
                }
                else
                {
                    if (result.result == (int)TapSDKInitResult.OK)
                    {
                        string currentClientId;
                        bool isFetchClientIdSuccess = TapClientBridge.GetClientId(out currentClientId);
                        Log(false, "IsLaunchedFromTapTapPC get  clientId = " + currentClientId, false);
                        if (isFetchClientIdSuccess && !string.IsNullOrEmpty(currentClientId) && currentClientId != clientId)
                        {
                            UIManager.Instance.OpenToast("SDK 中配置的 clientId = " + clientId + "与 Tap 启动器中" + currentClientId + "不一致", UIManager.GeneralToastLevel.Error);
                            Log(true, "SDK 中配置的 clientId = " + clientId + "与 Tap 启动器中" + currentClientId + "不一致");
                            lastIsLaunchedFromTapTapPCResult = 0;
                            return false;
                        }
                        string openId;
                        bool fetchOpenIdSuccess = TapClientBridge.GetTapUserOpenId(out openId);
                        if (fetchOpenIdSuccess)
                        {
                            Log(false, "IsLaunchedFromTapTapPC get  openId = " + openId, false);
                            EventManager.TriggerEvent(EventConst.IsLaunchedFromTapTapPCFinished, openId);
                        }
                        else
                        {
                            Log(false, "IsLaunchedFromTapTapPC get  openId failed");
                        }
                        lastIsLaunchedFromTapTapPCResult = 1;
                        TapCommonBridgePoll.StartUp();
                        Log(false, "IsLaunchedFromTapTapPC check success");
                        return true;
                    }
                    else
                    {
                        lastIsLaunchedFromTapTapPCResult = 0;
                        Log(false, "IsLaunchedFromTapTapPC show TapClient tip Pannel " + result.result + " , error = " + result.errorMsg);
                        string tipPannelPath = "Prefabs/TapClient/TapClientConnectTipPanel";
                        if (Resources.Load<GameObject>(tipPannelPath) != null)
                        {
                            var pannel = UIManager.Instance.OpenUI<TapClientConnectTipController>(tipPannelPath);
                            pannel.Show(result.result);
                        }
                        return false;
                    }
                }
            }
            catch (Exception e)
            {
                lastIsLaunchedFromTapTapPCResult = 0;
                Log(false, "IsLaunchedFromTapTapPC check exception = " + e.Message + " \n" + e.StackTrace);
                string tipPannelPath = "Prefabs/TapClient/TapClientConnectTipPanel";
                if (Resources.Load<GameObject>(tipPannelPath) != null)
                {
                    var pannel = UIManager.Instance.OpenUI<TapClientConnectTipController>(tipPannelPath);
                    pannel.Show((int)TapSDKInitResult.Unknown);
                }
                return false;
            }
        }

        private static async Task<TapInitResult> RunClientBridgeMethod(string clientId, string pubKey)
        {
            TaskCompletionSource<TapInitResult> task = new TaskCompletionSource<TapInitResult>();
            try
            {
                await Task.Run(() =>
                {
                    Log(false, "check startupWithClientBridge start thread = " + Thread.CurrentThread.ManagedThreadId);
                    bool needQuitGame = TapClientBridge.TapSDK_RestartAppIfNecessary(clientId);
                    Log(false, "RunClientBridgeMethodWithTimeout invoke  TapSDK_RestartAppIfNecessary result = " + needQuitGame);
                    if (needQuitGame)
                    {
                        tapInitResult = new TapInitResult(needQuitGame);
                    }
                    else
                    {
                        string outputError;
                        int tapSDKInitResult = TapClientBridge.CheckInitState(out outputError, pubKey);
                        Log(false, "RunClientBridgeMethodWithTimeout invoke  CheckInitState result = " + tapSDKInitResult + ", error = " + outputError);
                        tapInitResult = new TapInitResult(tapSDKInitResult, outputError);
                    }
                    task.TrySetResult(tapInitResult);
                });
                

            }
            catch (Exception ex)
            {
                Log(false, "RunClientBridgeMethodWithTimeout invoke C 方法出错！" + ex.Message);
                task.TrySetException(ex);
            }
            return await task.Task;
        }


        /// <summary>
        /// 是否需要从启动器登录
        /// </summary>
        public static bool IsNeedLoginByTapClient()
        {
            return isChannelPackage;
        }


        private static Action<bool, string> currentLoginCallback;

        /// <summary>
        /// 发起登录授权
        /// </summary>
        public static bool StartLoginWithScopes(string[] scopes, string responseType, string redirectUri,
    string codeChallenge, string state, string codeChallengeMethod, string versonCode, string sdkUa, string info, Action<bool, string> callback)
        {
            if (lastIsLaunchedFromTapTapPCResult == -1)
            {
                // UIManager.Instance.OpenToast("IsLaunchedFromTapTapPC 正在执行，请在完成后调用授权接口", UIManager.GeneralToastLevel.Error);
                Log(true, " login must be invoked after IsLaunchedFromTapTapPC success");
                throw new Exception("login must be invoked after IsLaunchedFromTapTapPC success");
            }
            Log(false, "LoginWithScopes start login by tapclient thread = " + Thread.CurrentThread.ManagedThreadId);
            try
            {
                TapClientBridge.RegisterCallback(TapEventID.AuthorizeFinished_internal, loginCallbackDelegate);
                AuthorizeResult authorizeResult = TapClientBridge.LoginWithScopesInternal(scopes, responseType, redirectUri,
     codeChallenge, state, codeChallengeMethod, versonCode, sdkUa, info);
                Log(false, "LoginWithScopes start result = " + authorizeResult);
                if (authorizeResult != AuthorizeResult.OK)
                {
                    TapClientBridge.UnRegisterCallback(TapEventID.AuthorizeFinished_internal,loginCallbackDelegate);
                    return false;
                }
                else
                {
                    currentLoginCallback = callback;
                    return true;
                }

            }
            catch (Exception ex)
            {
                Log(false, "LoginWithScopes start login by tapclient error = " + ex.Message);
                TapClientBridge.UnRegisterCallback(TapEventID.AuthorizeFinished_internal,loginCallbackDelegate);
                return false;
            }

        }


        [AOT.MonoPInvokeCallback(typeof(TapClientBridge.CallbackDelegate))]
        static void loginCallbackDelegate(int id, IntPtr userData)
        {
            Log(false, "LoginWithScopes recevie callback " + id);
            if (id == (int)TapEventID.AuthorizeFinished_internal)
            {
                Log(false, "LoginWithScopes callback thread = " + Thread.CurrentThread.ManagedThreadId);
                TapClientBridge.AuthorizeFinishedResponse_internal response = Marshal.PtrToStructure<TapClientBridge.AuthorizeFinishedResponse_internal>(userData);
                Log(false, "LoginWithScopes callback = " + response.is_cancel + " uri = " + response.callback_uri, false);
                if (currentLoginCallback != null)
                {
                    currentLoginCallback(response.is_cancel != 0, response.callback_uri);
                    TapClientBridge.UnRegisterCallback(TapEventID.AuthorizeFinished_internal,loginCallbackDelegate);
                    currentLoginCallback = null;
                }
            }
        }


        // DLC 相关功能
        private static Action<string, bool> currentDlcDelegate;
        private static Action<bool> currentLicenseDelegate;

        /// 查询是否购买 DLC , 未调用 isLaunchFromPC 会抛异常
        public static bool QueryDLC(string skuId)
        {
            if (lastIsLaunchedFromTapTapPCResult != 1)
            {
                throw new Exception("queryDLC must be invoked after IsLaunchedFromTapTapPC success");
            }
            bool success = TapClientBridge.QueryDLC(skuId);
            return success;
        }

        /// 跳转到 TapTap 客户端 DLC 购买页面 , 未调用 isLaunchFromPC 会抛异常
        public static bool ShowStore(string skuId)
        {
            if (lastIsLaunchedFromTapTapPCResult != 1)
            {
                throw new Exception("ShowStore must be invoked after IsLaunchedFromTapTapPC success");
            }
            Log(false, "ShowStore start = " + skuId);
            return TapClientBridge.TapDLC_ShowStore(skuId);
        }

        /// 注册 DLC 购买状态变更回调，包括购买成功和退款
        public static void RegisterDLCOwnedCallback(Action<string, bool> dlcDelegate)
        {
            currentDlcDelegate = dlcDelegate;
            TapClientBridge.RegisterCallback(TapEventID.DLCPlayableStatusChanged, DLCCallbackDelegate);
        }

        /// DLC 回调
        [AOT.MonoPInvokeCallback(typeof(TapClientBridge.CallbackDelegate))]
        static void DLCCallbackDelegate(int id, IntPtr userData)
        {
            Log(false, "queryDlC recevie callback " + id);
            if (currentDlcDelegate != null)
            {
                TapClientBridge.DLCPlayableStatusChangedResponse response = Marshal.PtrToStructure<TapClientBridge.DLCPlayableStatusChangedResponse>(userData);
                Log(false, "queryDlC callback =  " + response.dlc_id + " isOwn = " + response.is_playable);
                currentDlcDelegate(response.dlc_id, response.is_playable != 0);
            }
        }

         /// 注册 License 购买状态变更回调，包括购买成功和退款
        public static void RegisterLicenseCallback(Action<bool> licensecDelegate)
        {
            currentLicenseDelegate = licensecDelegate;
            TapClientBridge.RegisterCallback(TapEventID.GamePlayableStatusChanged, LicenseCallbackDelegate);
        }

        /// License 回调
        [AOT.MonoPInvokeCallback(typeof(TapClientBridge.CallbackDelegate))]
        static void LicenseCallbackDelegate(int id, IntPtr userData)
        {
            Log(false, "License recevie callback " + id);
            if (currentLicenseDelegate != null)
            {
                TapClientBridge.GamePlayableStatusChangedResponse response = Marshal.PtrToStructure<TapClientBridge.GamePlayableStatusChangedResponse>(userData);
                Log(false, "License callback  isOwn changed " + response.is_playable );
                currentLicenseDelegate(response.is_playable != 0);
            }
        }

        public static bool HasLicense()
        {
            if (lastIsLaunchedFromTapTapPCResult != 1)
            {
                throw new Exception("ShowStore must be invoked after IsLaunchedFromTapTapPC success");
            }
            return TapClientBridge.HasLicense();
        }

        private static void Log(bool isError, string data, bool alwaysShow = true)
        {
            if (!string.IsNullOrEmpty(data))
            {
                if (TapLogger.LogDelegate != null)
                {
                    if (isError) {
                        TapLogger.Error(data);
                    } else {
                        TapLogger.Debug(data);
                    }
                    
                }
                else
                {
                    if (alwaysShow)
                    {
                        if (isError)
                        {
                            UnityEngine.Debug.LogErrorFormat($"[TapSDK] ERROR: {data}");
                        }
                        else
                        {
                            UnityEngine.Debug.LogFormat($"[TapSDK] INFO: {data}");
                        }
                    }
                }
            }
        }

        // 初始化校验结果
        private class TapInitResult
        {
            internal int result;
            internal string errorMsg;

            internal bool needQuitGame = false;

            public TapInitResult(int result, string errorMsg)
            {
                this.result = result;
                this.errorMsg = errorMsg;
            }

            public TapInitResult(bool needQuitGame)
            {
                this.needQuitGame = needQuitGame;
            }
        }
    }
#endif
}