using System;
using UnityEngine.UI;
using TapTap.UI;
using UnityEngine;
using TapTap.Common;

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

        internal void Show(TapSDKInitResult errorType)
        {
            if (errorType == TapSDKInitResult.kTapSDKInitResult_NoPlatform){
                tipText.text = "获取游戏信息失败，请下载 TapTap 客户端后重新启动游戏";
            }else if (errorType == TapSDKInitResult.kTapSDKInitResult_NotLaunchedByPlatform){
                     tipText.text = "获取游戏信息失败，请从 TapTap 客户端重新启动游戏";
            }else {
                 tipText.text = "发生未知错误，请从 TapTap 客户端重新启动游戏";
            }
        }

        private void OnOKButtonClicked()
        {
            Close();
            Application.Quit();
        }

        private void OnInstallButtonClicked()
        {
            Application.OpenURL("https://www.taptap.cn/mobile?utm_medium=coop&utm_source=pc_toStart");
        }
        
    }
}