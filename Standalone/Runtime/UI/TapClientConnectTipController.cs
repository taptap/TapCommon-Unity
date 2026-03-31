using System;
using UnityEngine.UI;
using TapTap.UI;
using UnityEngine;
using System.Diagnostics;
using TapTap.Common.Standalone.Internal;

namespace TapTap.Common.Standalone {
    public class TapClientConnectTipController : BasePanelController
    {
        public Button installTipButton;
        public Button okButton;

        public Text tipText;


        /// <summary>
        /// bind ugui components for every panel
        /// </summary>
        protected override void BindComponents()
        {
            okButton = transform.Find("Root/OKButton").GetComponent<Button>();
            installTipButton = transform.Find("Root/InstallTipBtn").GetComponent<Button>();
        }

        protected override void OnLoadSuccess()
        {
            base.OnLoadSuccess();

            installTipButton.onClick.AddListener(OnInstallButtonClicked);
            okButton.onClick.AddListener(OnOKButtonClicked);
        }

        internal void Show(int errorType)
        {
            if (errorType == (int)TapSDKInitResult.NoPlatform){
                tipText.text = "获取游戏信息失败，请下载 TapTap 客户端后重新启动游戏";
            }else if (errorType == (int)TapSDKInitResult.NotLaunchedByPlatform){
                tipText.text = "获取游戏信息失败，请从 TapTap 客户端重新启动游戏";
            }else if (errorType == (int)TapSDKInitResult.Unknown){
                tipText.text = "本地发生未知错误，请从 TapTap 客户端重新启动游戏";
            }else if (errorType == (int)TapSDKInitResult.PlatformVersionMismatch){
                tipText.text = "客户端版本不匹配，请将 TapTap 客户端升级到最新版本";    
            }else {
                tipText.text = "发生未知错误，请从 TapTap 客户端重新启动游戏";
            }
        }

        private void OnOKButtonClicked()
        {
            Close();
           // 由于兼容问题 #TAP-72138  Application.Quit 部分场景无法退出，所以杀进程
            Process.GetCurrentProcess().Kill();
        }

        private void OnInstallButtonClicked()
        {
            Application.OpenURL("https://www.taptap.cn/mobile?utm_medium=coop&utm_source=pc_toStart");
        }
        
    }
}