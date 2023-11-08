using System.Collections.Generic;
using com.taptap.tapsdk.bindings.csharp;
using LC.Newtonsoft.Json;
using UnityEngine;

namespace TapTap.Common {
    public sealed class UnityTDSUser
    {
        public const string TAP_AUTH_CHANNEL = "open_id";
        public const string TDS_CHANNEL = "tds_id";
        private readonly Dictionary<string, string> _userInfo = new Dictionary<string, string>();
        public bool IsEmpty => _userInfo == null || _userInfo.Count == 0;
        
        public void UpdateUserInfo(string channel, string userId) {
            TapLogger.Debug($"[TapDuration] UpdateUserInfo. channel: {channel} userId: {userId}");
            _userInfo[channel] = userId;

            if (string.IsNullOrEmpty(userId) && _userInfo.ContainsKey(channel))
                _userInfo.Remove(channel);
        }

        public void Logout() {
            _userInfo.Clear();
        }
        
        public bool ContainTapInfo() {
            return _userInfo.ContainsKey(TAP_AUTH_CHANNEL) || _userInfo.ContainsKey(TDS_CHANNEL);
        }

        public string GetUserInfo() {
            var userInfo= JsonConvert.SerializeObject(_userInfo);
            TapLogger.Debug(string.Format("[TapDuration] GetUserInfo: {0}", userInfo));
            return userInfo;
        }
        
        public override string ToString() {
            return GetUserInfo();
        }
        
        // TODO: Implement the following methods
        public string GetUserName() {
            return "";
        }
        
        
    }
}