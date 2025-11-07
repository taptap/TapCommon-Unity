using TapTap.UI;
using UnityEngine;

namespace TapTap.Common {
    internal sealed class UnityMonobehaviorWrapper : MonoSingleton<UnityMonobehaviorWrapper> {

        private bool isPause = false;

        private void OnApplicationPause(bool pauseStatus) {

            if (pauseStatus && isPause == false) {
                isPause = true;
            }
            else if (!pauseStatus && isPause) {
                isPause = false;
            }
        }

        private void OnApplicationQuit(){
        }
        
    }
}

