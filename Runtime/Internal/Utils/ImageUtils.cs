using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace TapTap.Common.Internal.Utils {
    public class ImageUtils {
        private static Dictionary<string, WeakReference<Texture>> cachedTextures = new Dictionary<string, WeakReference<Texture>>();

        public static async Task<Texture> LoadImage(string url) {
            if (cachedTextures.TryGetValue(url, out WeakReference<Texture> refTex) &&
                refTex.TryGetTarget(out Texture tex)) {
                return tex;
            } else {
                Texture newTex = await FetchImage(url);
                cachedTextures[url] = new WeakReference<Texture>(newTex);
                return newTex;
            }
        }

        public static async Task<Texture> FetchImage(string url) {
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
            UnityWebRequestAsyncOperation operation = www.SendWebRequest();
            while (!operation.isDone) {
                await Task.Delay(50);
            }

            if (www.isNetworkError || www.isHttpError) {
                throw new Exception("Fetch image error.");
            } else {
                return ((DownloadHandlerTexture)www.downloadHandler).texture;
            }
        }
    }
}
