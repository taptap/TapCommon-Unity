using TapTap.UI;
using UnityEngine;

namespace TapTap.Common {
    internal sealed class UnityMonobehaviorWrapper : MonoSingleton<UnityMonobehaviorWrapper> {

        private bool isPause = false;

        private void OnApplicationPause(bool pauseStatus) {

            if (pauseStatus && isPause == false) {
                isPause = true;
                EventManager.TriggerEvent(EventConst.OnApplicationPause, true);
            }
            else if (!pauseStatus && isPause) {
                isPause = false;
                EventManager.TriggerEvent(EventConst.OnApplicationPause, false);
            }
        }

        private void OnApplicationQuit(){
            EventManager.TriggerEvent(EventConst.OnApplicationQuit, true);
        }
        
    }
}

