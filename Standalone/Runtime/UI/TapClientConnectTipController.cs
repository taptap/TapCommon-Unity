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

        internal void Show()
        {
            
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