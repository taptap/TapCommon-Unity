using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using com.taptap.tapsdk.bindings.csharp;
using UnityEngine;
using DeviceType = com.taptap.tapsdk.bindings.csharp.DeviceType;

namespace TapTap.Common.Internal {
    public static class TapDuration {
        
        private static bool isRndEnvironment;
        private static bool initialized;
        private static UnityTDSUser unityTdsUser;
        private static BridgeUser bridgeUser;

        private static string deviceId;
        private static int? deviceType;

        public static bool isCn;

        private static string clientId;
        
        public static void RndInit() {
            EventManager.AddListener(EventConst.SetRND, (_) => {
                isRndEnvironment = true;
                TapLogger.Debug($"[TapDuration] 设置 RND 环境 Application.identifier: {Application.identifier} Application.productName: {Application.productName} Application.installerName: {Application.installerName}");
            });
        }
        
        public static void Init(string clientId, bool isCn) {
            deviceType = null;
            TapDuration.clientId = clientId;
            if (!TapCommon.DisableDurationStatistics && !initialized) {
                TapCommon.GetDeviceId((x)=> deviceId = x);
                TapCommon.GetDeviceType((x)=> deviceType = x);
                TapDuration.isCn = isCn;
                DurationInit();
                initialized = true;
            }   
        }
        
        private static void DurationInit() {
            try {
                if (SupportDurationStatistics())
                    DurationBindingInit();
            }
            catch (Exception e) {
                while (e.InnerException != null) {
                    e = e.InnerException;
                }
                TapLogger.Error("[TapSDK::Duration] Init Error Won't statistic duration info! Error info: " + e.ToString() + "\n" + e.StackTrace);
            }
        }
        
        private static bool SupportDurationStatistics() {
    #if UNITY_EDITOR
            return false;
    #elif UNITY_STANDALONE_OSX
            return false;
    #else
            return true;
    #endif
        }

        private static void DurationBindingInit() {
            BindGameConfig();
            BindUserInfo();
            BindWindowChange();
        }

        private static async void BindGameConfig() {
            while(string.IsNullOrEmpty(deviceId) || deviceType == null) {
                await Task.Yield();
            }
            var bridgeConfig = new BridgeConfig();
            var dir = new DirectoryInfo(Path.Combine(Application.persistentDataPath, "tapsdk"));
            if (!dir.Exists)
                dir.Create();
            bridgeConfig.cache_dir = dir.FullName;
            bridgeConfig.ca_dir = "";
            bridgeConfig.device_id = string.IsNullOrEmpty(deviceId) ? "unity_device_id_error" : deviceId;
            bridgeConfig.enable_duration_statistics = true;
            bridgeConfig.device_type = deviceType.Value;
            bridgeConfig.region = isRndEnvironment ? (int)Region.RND : (isCn ? (int)Region.CN : (int)Region.Global);
            bridgeConfig.model = SystemInfo.deviceModel;
        #if UNITY_ANDROID
            bridgeConfig.platform = "Android";
        #elif UNITY_IOS
            bridgeConfig.platform = "iOS";
        #elif UNITY_STANDALONE_WIN
            bridgeConfig.platform = "Windows";
        #endif
            bridgeConfig.engine = $@"Unity";
            bridgeConfig.sdk_version = TapCommon.SDKVersion;
            Bindings.InitSDK(bridgeConfig);
            // Set Game
            var bridgeGame = new BridgeGame();
            bridgeGame.client_id = clientId;
            bridgeGame.identify = GetIdentifier();
            Bindings.SetCurrentGame(bridgeGame);
            TapLogger.Debug($"[TapDuration] cache_dir: {bridgeConfig.cache_dir} device_id: {bridgeConfig.device_id} " +
                                          $"device_type: {bridgeConfig.device_type} region: {bridgeConfig.region} model: {bridgeConfig.model} " +
                                          $"platform: {bridgeConfig.platform} engine: {bridgeConfig.engine} sdk_version: {bridgeConfig.sdk_version} " +
                                          $"client_id: {bridgeGame.client_id} identify: {bridgeGame.identify}");
        }

        private static string GetIdentifier() {
            var result = Application.identifier;
            if (string.IsNullOrEmpty(result)) {
                result = Application.productName;
            }
            if (string.IsNullOrEmpty(result)) {
                result = Application.installerName;
            }
            if (string.IsNullOrEmpty(result)) {
                result = "UNITY_NULL_IDENTIFIER";
            }

            return result;
        }
        
        private static void BindUserInfo() {
            unityTdsUser = new UnityTDSUser();
            EventManager.AddListener(EventConst.OnTapLogin, (loginParameter) => {
                var kv = loginParameter is KeyValuePair<string, string> ? (KeyValuePair<string, string>)loginParameter : default;
                if (!string.IsNullOrEmpty(kv.Key)) {
                    bridgeUser = new BridgeUser();
                    if (unityTdsUser.IsEmpty) {
                        Bindings.SetCurrentUser(bridgeUser);
                    }
                    unityTdsUser.UpdateUserInfo(kv.Key, kv.Value);
                    bridgeUser.user_id = unityTdsUser.GetUserInfo();
                    bridgeUser.contain_tap_info = unityTdsUser.ContainTapInfo();
                    Bindings.SetCurrentUser(bridgeUser);
                }
            });
            EventManager.AddListener(EventConst.OnTapLogout, (logoutChannel) => {
                if (logoutChannel is string channel && !string.IsNullOrEmpty(channel)) {
                    unityTdsUser.Logout();
                    Bindings.SetCurrentUser(null);
                }
            });
            
            EventManager.AddListener(EventConst.OnBind, (kv) => {
                if (!(kv is KeyValuePair<string, string>)) return;
                if (unityTdsUser.IsEmpty) return;
                var bindInfo = (KeyValuePair<string, string>)kv;
                if (!string.IsNullOrEmpty(bindInfo.Key)) {
                    bridgeUser = new BridgeUser();
                    unityTdsUser.UpdateUserInfo(bindInfo.Key, bindInfo.Value);
                    bridgeUser.user_id = unityTdsUser.GetUserInfo();
                    bridgeUser.contain_tap_info = unityTdsUser.ContainTapInfo();
                    Bindings.SetCurrentUser(bridgeUser);
                }
            });
        }
        
        private static void BindWindowChange() {
            EventManager.AddListener(EventConst.OnApplicationPause, (isPause) => {
                var isPauseBool = (bool)isPause;
                if (isPauseBool) {
                    Bindings.OnWindowBackground();
                }
                else {
                    Bindings.OnWindowForeground();
                }
            });
        }
    }
}