using System;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using TapTap.Common.Internal;
using System.Collections.Generic;
using UnityEngine;

namespace TapTap.Common.Mobile
{
    public class TapCommonMobile : ITapCommonPlatform
    {
        public void Init(TapConfig config)
        {
            TapCommonImpl.GetInstance().Init(config);
        }

        public void GetRegionCode(Action<bool> callback)
        {
            TapCommonImpl.GetInstance().GetRegionCode(callback);
        }

        public void IsTapTapInstalled(Action<bool> callback)
        {
            TapCommonImpl.GetInstance().IsTapTapInstalled(callback);
        }

        public void IsTapTapGlobalInstalled(Action<bool> callback)
        {
            TapCommonImpl.GetInstance().IsTapTapGlobalInstalled(callback);
        }

        public void UpdateGameInTapTap(string appId, Action<bool> callback)
        {
            TapCommonImpl.GetInstance().UpdateGameInTapTap(appId, callback);
        }

        public void UpdateGameInTapGlobal(string appId, Action<bool> callback)
        {
            TapCommonImpl.GetInstance().UpdateGameInTapGlobal(appId, callback);
        }

        public void OpenReviewInTapTap(string appId, Action<bool> callback)
        {
            TapCommonImpl.GetInstance().OpenReviewInTapTap(appId, callback);
        }

        public void OpenReviewInTapGlobal(string appId, Action<bool> callback)
        {
            TapCommonImpl.GetInstance().OpenReviewInTapGlobal(appId, callback);
        }

        public void SetXua()
        {
            TapCommonImpl.GetInstance().SetXua();
        }

        public void SetLanguage(TapLanguage language)
        {
            TapCommonImpl.GetInstance().SetLanguage(language);
        }

        public void RegisterProperties(string key, ITapPropertiesProxy proxy)
        {
            TapCommonImpl.GetInstance().RegisterProperties(key, proxy);
        }

        public void SetDurationStatisticsEnabled(bool enable)
        {
            TapCommonImpl.GetInstance().SetDurationStatisticsEnabled(enable);
        }

        public void AddHost(string host, string replaceHost)
        {
            TapCommonImpl.GetInstance().AddHost(host, replaceHost);
        }

        public Task<bool> UpdateGameAndFailToWebInTapTap(string appId)
        {
            return TapCommonImpl.GetInstance().UpdateGameAndFailToWebInTapTap(appId);
        }

        public Task<bool> UpdateGameAndFailToWebInTapGlobal(string appId)
        {
            return TapCommonImpl.GetInstance().UpdateGameAndFailToWebInTapGlobal(appId);
        }

        public Task<bool> UpdateGameAndFailToWebInTapTap(string appId, string webUrl)
        {
            return TapCommonImpl.GetInstance().UpdateGameAndFailToWebInTapTap(appId, webUrl);
        }

        public Task<bool> UpdateGameAndFailToWebInTapGlobal(string appId, string webUrl)
        {
            return TapCommonImpl.GetInstance().UpdateGameAndFailToWebInTapGlobal(appId, webUrl);
        }

        public Task<bool> OpenWebDownloadUrlOfTapTap(string appId)
        {
            return TapCommonImpl.GetInstance().OpenWebDownloadUrlOfTapTap(appId);
        }

        public Task<bool> OpenWebDownloadUrlOfTapGlobal(string appId)
        {
            return TapCommonImpl.GetInstance().OpenWebDownloadUrlOfTapGlobal(appId);
        }

        public Task<bool> OpenWebDownloadUrl(string url)
        {
            return TapCommonImpl.GetInstance().OpenWebDownloadUrl(url);
        }

        public Task<bool> IsLaunchedFromTapTapPC()
        {
            return Task.FromResult(false);
        }


        public void GetOpenLogCommonParams(int region, Action<string> action)
        {
#if UNITY_ANDROID
                TapCommonImpl.GetInstance().getOpenLogCommonParams(action);

#elif UNITY_IOS
                IntPtr ptr = TDSCommonBridgeGetOpenLogCommonParams(region);
                action(Marshal.PtrToStringAnsi(ptr));
#endif
        }

        public string DeviceId
        {
            get
            {
#if UNITY_ANDROID
                using (AndroidJavaClass CommonBridgeClass = new AndroidJavaClass("com.tds.common.bridge.CommonBridge")) {
                    return CommonBridgeClass.CallStatic<string>("getDeviceId");
                }
#elif UNITY_IOS
                IntPtr ptr = TDSCommonBridgeGetDeviceId();
                return Marshal.PtrToStringAnsi(ptr);
#endif
            }
        }

        public int DeviceType
        {
            get
            {
#if UNITY_ANDROID
                using (AndroidJavaClass CommonBridgeClass = new AndroidJavaClass("com.tds.common.bridge.CommonBridge")) {
                    return CommonBridgeClass.CallStatic<int>("getDeviceType");
                }
#elif UNITY_IOS
                return TDSCommonBridgeGetDeviceType();
#endif   
            }
        }

#if UNITY_IOS
        [DllImport("__Internal")]
        private static extern IntPtr TDSCommonBridgeGetDeviceId();

        [DllImport("__Internal")]
        private static extern int TDSCommonBridgeGetDeviceType();

        [DllImport("__Internal")]
        private static extern IntPtr TDSCommonBridgeGetOpenLogCommonParams(int region);
#endif
    }
}