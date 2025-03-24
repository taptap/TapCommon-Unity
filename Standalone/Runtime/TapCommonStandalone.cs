using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using TapTap.Common.Internal;
using TapTap.UI;
using UnityEngine;

namespace TapTap.Common.Standalone
{
    public class TapCommonStandalone : ITapCommonPlatform
    {

        // 初始化校验结果
        private class TapInitResult
        {
            internal TapSDKInitResult result;
            internal string errorMsg;

            internal bool needQuitGame = false;

            public TapInitResult(TapSDKInitResult result, string errorMsg)
            {
                this.result = result;
                this.errorMsg = errorMsg;
            }

            public TapInitResult(bool needQuitGame)
            {
                this.needQuitGame = needQuitGame;
            }
        }

        public class TapLoginResponseByTapClient
        {

            public bool isCancel = false;

            public string redirectUri;

            public bool isFail = false;

            public string errorMsg;

            public TapLoginResponseByTapClient(bool isCancel, string redirctUri)
            {
                this.redirectUri = redirctUri;
                this.isCancel = isCancel;
            }

            public TapLoginResponseByTapClient(string errorMsg)
            {
                isFail = true;
                isCancel = false;
                this.errorMsg = errorMsg;
            }


        }

        public interface TapLoginCallbackWithTapClient
        {
            void onSuccess(TapLoginResponseByTapClient response);

            void onFailure(string error);

            void onCancel();
        }

        // 是否是渠道服游戏包
        private static bool isChannelPackage = false;

        // -1 未执行 0 失败  1 成功
        private static int lastIsLaunchedFromTapTapPCResult = -1;
        private static bool isRuningIsLaunchedFromTapTapPC = false;


        // 当为渠道游戏包时，与启动器的初始化校验结果
        private TapInitResult tapInitResult;

        private TapConfig currentTapConfig;




        public void AddHost(string host, string replaceHost)
        {
        }

        public void GetRegionCode(Action<bool> callback)
        {
        }

        public void Init(TapConfig config)
        {
            currentTapConfig = config;
        }

        public void IsTapTapGlobalInstalled(Action<bool> callback)
        {
        }

        public void IsTapTapInstalled(Action<bool> callback)
        {
        }

        public void OpenReviewInTapGlobal(string appId, Action<bool> callback)
        {
        }

        public void OpenReviewInTapTap(string appId, Action<bool> callback)
        {
        }

        public Task<bool> OpenWebDownloadUrl(string url)
        {
            return Task.FromResult(true);
        }

        public Task<bool> OpenWebDownloadUrlOfTapGlobal(string appId)
        {
            return Task.FromResult(true);
        }

        public Task<bool> OpenWebDownloadUrlOfTapTap(string appId)
        {
            return Task.FromResult(true);
        }

        public void RegisterProperties(string key, ITapPropertiesProxy proxy)
        {
        }

        public void UseNativeDataInCore(bool enable)
        {
        }

        public void SetDurationStatisticsEnabled(bool enable)
        {
            TapCommon.DisableDurationStatistics = !enable;
        }

        public void SetLanguage(TapLanguage language)
        {
        }

        public void SetXua()
        {
        }

        public Task<bool> UpdateGameAndFailToWebInTapGlobal(string appId)
        {
            return Task.FromResult(true);
        }

        public Task<bool> UpdateGameAndFailToWebInTapGlobal(string appId, string webUrl)
        {
            return Task.FromResult(true);
        }

        public Task<bool> UpdateGameAndFailToWebInTapTap(string appId)
        {
            return Task.FromResult(true);
        }

        public Task<bool> UpdateGameAndFailToWebInTapTap(string appId, string webUrl)
        {
            return Task.FromResult(true);
        }

        // <summary>
        // 校验游戏是否通过启动器唤起，建立与启动器通讯
        //</summary>
        public async Task<bool> IsLaunchedFromTapTapPC()
        {
#if UNITY_STANDALONE_WIN
            // 正在执行中
            if(isRuningIsLaunchedFromTapTapPC){
                UIManager.Instance.OpenToast("IsLaunchedFromTapTapPC 正在执行，请勿重复调用", UIManager.GeneralToastLevel.Error);
                TapLogger.Error("IsLaunchedFromTapTapPC 正在执行，请勿重复调用");
                return false;
            }
            // 多次执行时返回上一次结果
            if(lastIsLaunchedFromTapTapPCResult != -1){
                TapLogger.Debug("IsLaunchedFromTapTapPC duplicate invoke return " + lastIsLaunchedFromTapTapPCResult);
                return lastIsLaunchedFromTapTapPCResult > 0;
            }
            
            isChannelPackage = true;
            if (currentTapConfig == null)
            {
                UIManager.Instance.OpenToast("IsLaunchedFromTapTapPC 调用必须在初始化之后", UIManager.GeneralToastLevel.Error);
                TapLogger.Error("IsLaunchedFromTapTapPC 调用必须在初始化之后");
                return false;
            }
            string clientId = currentTapConfig.ClientID;
            string pubKey = currentTapConfig.ClientPublicKey;
            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(pubKey))
            {
                UIManager.Instance.OpenToast("clientId 及 TapPubKey 参数都不能为空, clientId =" +  clientId + ", TapPubKey = " + pubKey, UIManager.GeneralToastLevel.Error);
                TapLogger.Error("clientId 或 TapPubKey 无效, clientId = " + clientId + ", TapPubKey = " + pubKey);
                return false;
            }
            isRuningIsLaunchedFromTapTapPC = true;
            try
            {
                TapInitResult result = await RunClientBridgeMethodWithTimeout(clientId, pubKey);
                isRuningIsLaunchedFromTapTapPC = false;
                if (result.needQuitGame)
                {
                    lastIsLaunchedFromTapTapPCResult = 0;
                    TapLogger.Debug("IsLaunchedFromTapTapPC Quit game");
                    Application.Quit();
                    return false;
                }
                else
                {
                    if (result.result == TapSDKInitResult.kTapSDKInitResult_OK)
                    {
                        string currentClientId;
                        bool isFetchClientIdSuccess = TapClientBridge.GetClientId(out currentClientId);
                        TapLogger.Debug("IsLaunchedFromTapTapPC get  clientId = " + currentClientId);
                        if (isFetchClientIdSuccess && !string.IsNullOrEmpty(currentClientId) && currentClientId != clientId ){
                             UIManager.Instance.OpenToast("SDK 中配置的 clientId = " + clientId + "与 Tap 启动器中" + currentClientId + "不一致", UIManager.GeneralToastLevel.Error);
                             TapLogger.Error("SDK 中配置的 clientId = " + clientId + "与 Tap 启动器中" + currentClientId + "不一致");
                             lastIsLaunchedFromTapTapPCResult = 0;
                            return false;
                        }
                        string openId ;
                        bool fetchOpenIdSuccess = TapClientBridge.GetTapUserOpenId(out openId);
                        if (fetchOpenIdSuccess){
                            TapLogger.Debug("IsLaunchedFromTapTapPC get  openId = " + openId);
                            EventManager.TriggerEvent(EventConst.IsLaunchedFromTapTapPCFinished, openId);
                        }else{
                            TapLogger.Debug("IsLaunchedFromTapTapPC get  openId failed" );
                        }
                        lastIsLaunchedFromTapTapPCResult = 1;
                        TapCommonBridgePoll.StartUp();
                        TapLogger.Debug("IsLaunchedFromTapTapPC check success");
                        return true;
                    }
                    else
                    {
                        lastIsLaunchedFromTapTapPCResult = 0;
                        TapLogger.Debug("IsLaunchedFromTapTapPC show TapClient tip Pannel " + result.result + " , error = " + result.errorMsg);
                        string tipPannelPath = "Prefabs/TapClient/TapClientConnectTipPanel";
                        if (Resources.Load<GameObject>(tipPannelPath) != null)
                        {
                            var pannel = UIManager.Instance.OpenUI<TapClientConnectTipController>(tipPannelPath);
                            pannel.Show();
                        }
                        return false;
                    }
                }
            }
            catch (Exception e)
            {
                lastIsLaunchedFromTapTapPCResult = 0;
                TapLogger.Debug("IsLaunchedFromTapTapPC check exception = " + e.StackTrace);
                string tipPannelPath = "Prefabs/TapClient/TapClientConnectTipPanel";
                if (Resources.Load<GameObject>(tipPannelPath) != null)
                {
                    var pannel = UIManager.Instance.OpenUI<TapClientConnectTipController>(tipPannelPath);
                    pannel.Show();
                }
                return false;
            }

#else
            UIManager.Instance.OpenToast("IsLaunchedFromTapTapPC 仅支持 Windows PC 端", UIManager.GeneralToastLevel.Error);
            TapLogger.Error("IsLaunchedFromTapTapPC 仅支持 Windows PC 端");
            return false;
#endif
        }

        private async Task<TapInitResult> RunClientBridgeMethodWithTimeout(string clientId, string pubKey)
        {
#if UNITY_STANDALONE_WIN            
            TaskCompletionSource<TapInitResult> task = new TaskCompletionSource<TapInitResult>();
            try
            {
                TapInitResult result = await ExecuteWithTimeoutAsync(() =>
                {
                    bool needQuitGame = TapClientBridge.TapSDK_RestartAppIfNecessary(clientId);
                    TapLogger.Debug("RunClientBridgeMethodWithTimeout invoke  TapSDK_RestartAppIfNecessary result = " + needQuitGame);
                    if (needQuitGame)
                    {
                        tapInitResult = new TapInitResult(needQuitGame);
                    }
                    else
                    {
                        string outputError;
                        TapSDKInitResult tapSDKInitResult = TapClientBridge.CheckInitState(out outputError, pubKey);
                        TapLogger.Debug("RunClientBridgeMethodWithTimeout invoke  CheckInitState result = " + tapSDKInitResult + ", error = " + outputError);
                        tapInitResult = new TapInitResult(tapSDKInitResult, outputError);
                    }
                    return tapInitResult;
                }, TimeSpan.FromSeconds(5));

                
                    Debug.Log($"C 方法执行完成，结果: {result}（主线程ID: {Thread.CurrentThread.ManagedThreadId}）");
                    task.TrySetResult(tapInitResult);
                
            }
            catch (TimeoutException)
            {
                   TapLogger.Debug("RunClientBridgeMethodWithTimeout invoke  CheckInitState 方法执行超时！");
                   task.TrySetException(new Exception("init TapClient Bridge Timeout"));
            }
            catch (Exception ex)
            {
                 TapLogger.Debug("RunClientBridgeMethodWithTimeout invoke C 方法出错！" + ex.StackTrace);
                 task.TrySetException(ex);
            }
            return await task.Task;
#else
            throw new Exception("当前平台不支持该操作，仅支持 Windows PC");
#endif                        
        }

        /// <summary>
        /// 在 IO 线程执行 C 方法，超时 5 秒后切回主线程
        /// </summary>
        private static async Task<T> ExecuteWithTimeoutAsync<T>(Func<T> cMethod, TimeSpan timeout)
        {
            using (var cts = new CancellationTokenSource())
            {
                Task<T> ioTask = Task.Run(cMethod); // 在后台线程执行 C 方法
                Task delayTask = Task.Delay(timeout); // 超时任务

                Task completedTask = await Task.WhenAny(ioTask, delayTask);

                if (completedTask == delayTask)
                {
                    cts.Cancel(); // 取消 C 方法任务
                    throw new TimeoutException("C 方法执行超时！");
                }
                else
                {
                    cts.Cancel();
                    return await ioTask;
                }
            }
        }

        /// <summary>
        /// 是否需要从启动器登录
        /// </summary>
        public static bool IsNeedLoginByTapClient()
        {
            return isChannelPackage;
        }


#if UNITY_STANDALONE_WIN  
        private static TaskCompletionSource<TapLoginResponseByTapClient> taskCompletionSource;
        private static TapClientBridge.CallbackDelegate currentLoginDelegate;
#endif

        /// <summary>
        /// 发起登录授权
        /// </summary>
        public static async Task<TapLoginResponseByTapClient> LoginWithScopesAsync(string[] scopes, string responseType, string redirectUri,
    string codeChallenge, string state, string codeChallengeMethod, string versonCode, string sdkUa, string info)
        {
#if UNITY_STANDALONE_WIN 
            if(lastIsLaunchedFromTapTapPCResult == -1){
                // UIManager.Instance.OpenToast("IsLaunchedFromTapTapPC 正在执行，请在完成后调用授权接口", UIManager.GeneralToastLevel.Error);
                TapLogger.Error("IsLaunchedFromTapTapPC 正在执行，请在完成后调用授权接口");
                throw new Exception("操作异常: IsLaunchedFromTapTapPC 正在执行，请在完成后调用授权接口");
            }
            TapLogger.Debug("LoginWithScopes start login by tapclient mainthread = " + Thread.CurrentThread.ManagedThreadId);

            taskCompletionSource = new TaskCompletionSource<TapLoginResponseByTapClient>();
            
             currentLoginDelegate = loginCallbackDelegate;
            try
            {
                
                // if(currentLoginDelegate == null){
                //     currentLoginDelegate = loginCallbackDelegate;
                    TapLogger.Debug("LoginWithScopes setDelegate ");
                    TapClientBridge.RegisterCallback((int)TapCallbackID.kTapCallbackIDAuthorizeFinished, currentLoginDelegate);
                // }
                TapLogger.Debug("LoginWithScopes try get login result ");
                AuthorizeResult authorizeResult =  TapClientBridge.LoginWithScopes(scopes, responseType,  redirectUri, 
     codeChallenge,  state,  codeChallengeMethod,  versonCode,  sdkUa,  info);
                   
                TapLogger.Debug("LoginWithScopes in mainthread = " + authorizeResult);
                if (authorizeResult != AuthorizeResult.kAuthorizeResult_OK)
                {
                    TapClientBridge.UnRegisterCallback(currentLoginDelegate, true);
                    taskCompletionSource.TrySetResult(new TapLoginResponseByTapClient("发起授权失败，请确认 Tap 客户端是否正常运行"));
                }
                
            }
            catch (Exception ex)
            {
                TapLogger.Debug("LoginWithScopes start login by tapclient error = " + ex.StackTrace);
                TapClientBridge.UnRegisterCallback(currentLoginDelegate, true);
                taskCompletionSource.TrySetResult(new TapLoginResponseByTapClient(false, ex.StackTrace));
            }
            return await taskCompletionSource.Task;
#else
            throw new Exception("当前平台不支持该授权操作，仅支持 Windows PC ");
#endif          
        }

#if UNITY_STANDALONE_WIN
        [AOT.MonoPInvokeCallback(typeof(TapClientBridge.CallbackDelegate))]
        static void loginCallbackDelegate(int id, IntPtr userData)
        {

            TapLogger.Debug("LoginWithScopes recevie callback " + id);
            if (id == (int)TapCallbackID.kTapCallbackIDAuthorizeFinished)
            {
                TapLogger.Debug("LoginWithScopes callback mainthread = " + Thread.CurrentThread.ManagedThreadId);
                    TapClientBridge.AuthorizeFinishedResponse response = Marshal.PtrToStructure<TapClientBridge.AuthorizeFinishedResponse>(userData);
                    TapLogger.Debug("LoginWithScopes callback = " + response.is_cancel + " uri = " + response.callback_uri);
                    if (taskCompletionSource != null) {
                        taskCompletionSource.TrySetResult(new TapLoginResponseByTapClient(response.is_cancel > 0, response.callback_uri));
                        taskCompletionSource = null;
                    }
                        TapLogger.Debug("LoginWithScopes callback finish and will remove delegate " );
                        if (currentLoginDelegate != null){
                        TapLogger.Debug("LoginWithScopes callback finish and will remove delegate and not null" );
                        TapClientBridge.UnRegisterCallback(currentLoginDelegate, true);
                        currentLoginDelegate = null;
                        }

                TapLogger.Debug("LoginWithScopes callback finish and will remove delegate finish" );

            }
        }
#endif

        public void UpdateGameInTapGlobal(string appId, Action<bool> callback)
        {
        }

        public void UpdateGameInTapTap(string appId, Action<bool> callback)
        {
        }

        public string DeviceId => UnityEngine.SystemInfo.deviceUniqueIdentifier;

        public int DeviceType => 0;

        public void SetCurrentTdsId(string tdsId) { }

        public void GetOpenLogCommonParams(int region, Action<string> action) { action(""); }
    }

}
