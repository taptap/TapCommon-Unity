using System;
using System.Threading.Tasks;
using TapTap.Common.Internal;
using TapTap.UI;
using UnityEngine;

namespace TapTap.Common.Standalone
{
    public class TapCommonStandalone : ITapCommonPlatform
    {

        internal static TapConfig currentTapConfig;

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

        public Task<bool> IsLaunchedFromTapTapPC()
        {
#if UNITY_STANDALONE_WIN
            return TapClientStandalone.IsLaunchedFromTapTapPC();
#endif
            throw new System.NotImplementedException();
        }
    }

}
