using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

namespace TapTap.Common.Internal {
    public static class TapDuration {
        
        private static bool isRndEnvironment;
        private static bool initialized;
        private static UnityTDSUser unityTdsUser;

        private static string deviceId;
        private static int? deviceType;

        private static string commonLogParams;

        public static bool isCn;

        private static string clientId;

        private static string clientToken;
        
        public static void RndInit() {
            EventManager.AddListener(EventConst.SetRND, (_) => {
                isRndEnvironment = true;
                TapLogger.Debug($"[TapSDK::Duration] 设置 RND 环境 Application.identifier: {Application.identifier} Application.productName: {Application.productName} Application.installerName: {Application.installerName}");
            });
        }
        
        public static void Init(string clientId, string clientToken, bool isCn,
         string commonParams) {
            Debug.Log("sdkcore: current project has intergrated TapLogin, so upload duration");
            deviceType = null;
            TapDuration.clientId = clientId;
            TapDuration.clientToken = clientToken;
            if (!TapCommon.DisableDurationStatistics && !initialized) {
                deviceId = TapCommon.DeiveId;
                deviceType = TapCommon.DeviceType;
                TapDuration.isCn = isCn;
                commonLogParams = commonParams;
                DurationInit();
                initialized = true;
            }   
        }

        public static void TapLoginInfoInit(string platform, string userId) {
            #if UNITY_IOS || UNITY_ANDROID
                // 已经登录内建账号了,则不再监听 Tap 登录结果
                if (platform.Equals(UnityTDSUser.TAP_AUTH_CHANNEL) && unityTdsUser.TryGetChannelValue(UnityTDSUser.TDS_CHANNEL, out string value) &&!string.IsNullOrEmpty(value)) return;
                InternalLogin(platform, userId);
                TapLogger.Debug($"[TapSDK::Duration] TapLoginInfoInit channel: {platform} userId: {userId} user_id: {unityTdsUser.GetUserInfo()} contain_tap_info: {unityTdsUser.ContainTapInfo()}");
            #endif
        }
        
        private static void DurationInit() {
            try {
                if (SupportDurationStatistics()) {
                    DurationBindingInit();
                    TapLogger.Debug($"[TapSDK::Duration] Init Success!");
                }
                   
            }
            catch (Exception e) {
                while (e.InnerException != null) {
                    e = e.InnerException;
                }
                TapLogger.Error("[TapSDK::Duration] Init Error Won't statistic duration info! Error info: " + e.ToString() + "\n" + e.StackTrace);
            }
        }
        
        public static bool SupportDurationStatistics() {
    #if UNITY_EDITOR
            return false;
    #elif UNITY_STANDALONE
            return false;
    #else
            return true;
    #endif
        }

        private static void DurationBindingInit() {
            #if UNITY_IOS || UNITY_ANDROID
                BindGameConfig();
                BindUserInfo();
                //这里在移动端中应用退出事件可能无法触发 OnApplicationQuit 回调
                BindWindowChange();
            #endif
        }

       #if UNITY_IOS || UNITY_ANDROID    

        private static void BindGameConfig() {
            var dir = new DirectoryInfo(Path.Combine(Application.persistentDataPath, "tapsdk"));
            if (!dir.Exists)
                dir.Create();
            var region = isRndEnvironment ? 2 : (isCn ? (int)RegionType.CN : (int)RegionType.IO);
            string ua = "TapSDK-Unity/" + TapCommon.SDKVersion;
            var dict = TapTap.Common.Json.Deserialize(commonLogParams) as Dictionary<string, object>;

#if UNITY_ANDROID           
           string mobileSdkVersion = SafeDictionary.GetValue<string>(dict, "sdk_version_name");
           ua = ua + " TapSDK-Android/" + mobileSdkVersion;
#elif UNITY_IOS
           string mobileSdkVersion = SafeDictionary.GetValue<string>(dict, "sdk_version_name");
           ua = ua + " TapSDK-iOS/" + mobileSdkVersion;
#endif
           string env = deviceType.Value > 1 ? "cloud" :(deviceType.Value > 0 ? "sandbox" : "local");      
            var data = new Dictionary<string, object>
                {
                    {"env",env},
                    {"region", region},
                    {"client_id", clientId},
                    {"client_token", clientToken ?? ""},
                    {"log_level", 1},
                    {"log_to_console", 1},
                    {"data_dir", dir.FullName},
                    {"ua",ua},
                    {"common",dict}
                };
            Bindings.TdkOnAppStarted(TapTap.Common.Json.Serialize(data));    
        }


        private static void InternalLogin(string platform, string id) {
            if (unityTdsUser.TryGetChannelValue(platform, out string value) && value.Equals(id)) {
                return;
            }
            unityTdsUser.UpdateUserInfo(platform, id);
            var userInfo = unityTdsUser.GetUserInfo();
            if (string.IsNullOrEmpty(unityTdsUser.GetUserInfo())){
                return;
            }
            Bindings.TdkOnLogin(unityTdsUser.GetUserInfo());
        }
        
        private static void BindUserInfo() {
            unityTdsUser = new UnityTDSUser();
            EventManager.AddListener(EventConst.OnTapLogin, (loginParameter) => {
                var kv = loginParameter is KeyValuePair<string, string> ? (KeyValuePair<string, string>)loginParameter : default;
                if (!string.IsNullOrEmpty(kv.Key)) {
                    // 已经登录内建账号了,则不再监听 Tap 登录结果
                    if (kv.Key.Equals(UnityTDSUser.TAP_AUTH_CHANNEL) && unityTdsUser.TryGetChannelValue(UnityTDSUser.TDS_CHANNEL, out string value) &&!string.IsNullOrEmpty(value)) return;
                    InternalLogin(kv.Key, kv.Value);
                    TapLogger.Debug($"[TapSDK::Duration] OnTapLogin. channel: {kv.Key} userId: {kv.Value} user_id: {unityTdsUser.GetUserInfo()} contain_tap_info: {unityTdsUser.ContainTapInfo()}");
                }
            });
            EventManager.AddListener(EventConst.OnTapLogout, (logoutChannel) => {
                if (logoutChannel is string channel && !string.IsNullOrEmpty(channel)) {
                    if (!unityTdsUser.TryGetChannelValue(channel, out _)) return;
                    unityTdsUser.Logout();
                    Bindings.TdkOnLogout();
                    TapLogger.Debug($"[TapSDK::Duration] OnTapLogout channel: {channel} {unityTdsUser.GetUserInfo()} contain_tap_info: {unityTdsUser.ContainTapInfo()}");
                }
            });
            
            EventManager.AddListener(EventConst.OnBind, (kv) => {
                if (!(kv is KeyValuePair<string, string>)) return;
                var bindInfo = (KeyValuePair<string, string>)kv;
                if (!string.IsNullOrEmpty(bindInfo.Key)) {
                    InternalLogin(bindInfo.Key, bindInfo.Value);
                    TapLogger.Debug($"[TapSDK::Duration] OnBind. channel: {bindInfo.Key} userId: {bindInfo.Value} user_id: {unityTdsUser.GetUserInfo()} contain_tap_info: {unityTdsUser.ContainTapInfo()}");
                }
            });
            
            EventManager.AddListener(EventConst.OnUnbind, (unbindChannel) => {
                if (unbindChannel is string channel && !string.IsNullOrEmpty(channel)) {
                    if (!unityTdsUser.TryGetChannelValue(channel, out _)) return;
                    unityTdsUser.UpdateUserInfo(channel, null);
                    if (string.IsNullOrEmpty(unityTdsUser.GetUserInfo()))
                        return;
                    Bindings.TdkOnLogin(unityTdsUser.GetUserInfo());    
                    TapLogger.Debug($"[TapSDK::Duration] OnUnbind. channel: {channel} user_id: {unityTdsUser.GetUserInfo()} contain_tap_info: {unityTdsUser.ContainTapInfo()}");
                }
            });
        }
        
        private static void BindWindowChange() {
            EventManager.AddListener(EventConst.OnApplicationPause, (isPause) => {
                var isPauseBool = (bool)isPause;
                if (isPauseBool) {
                    Bindings.TdkOnBackground();
                    TapLogger.Debug($"[TapSDK::Duration] OnWindowBackground");
                }
                else {
                    Bindings.TdkOnForeground();
                    TapLogger.Debug($"[TapSDK::Duration] OnWindowBackground");
                }
            });

            EventManager.AddListener(EventConst.OnApplicationQuit, (quit) => {
                Bindings.TdkOnAppStopped();
                TapLogger.Debug($"[TapSDK::Duration] OnWindowQuit");
                
            });
        }
    #endif    
    }
}