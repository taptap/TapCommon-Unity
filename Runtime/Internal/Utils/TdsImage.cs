using System;
using System.Collections;
using System.Collections.Generic;
using TapTap.UI;
using UnityEngine;
using UnityEngine.Networking;

namespace TapTap.Common.Runtime.Internal.Utils {
    public class TdsImage : MonoSingleton<TdsImage> {

        private static Dictionary<string, Texture2D> cacheDict = new Dictionary<string, Texture2D>();
        
        public void Load(string url, int maxLoadingTime, Action<Texture2D> callback) {
            if (cacheDict.TryGetValue(url, out var value)) {
                callback?.Invoke(value);
            }
            else {
                StartCoroutine(DownloadImage(url, maxLoadingTime, callback));
            }
        }
        
        private static IEnumerator DownloadImage(string url, float maxLoadingTime, Action<Texture2D> callback) {
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
            DateTime startTime = DateTime.Now;

            www.SendWebRequest();
            
            TapLogger.Debug($"开始下载图片! url: {url}");
            yield return new WaitForSeconds(maxLoadingTime / 1000.0f);
            
            if (www.isNetworkError || www.isHttpError) {
                callback?.Invoke(null); // 下载错误，返回空
                
                TapLogger.Warn($"下载图片的时候发生错误!: {www.error}");
            }
            else if (www.isDone) {
                OnDownloadSuccess(www, url, callback);
                TapLogger.Debug($"下载图片顺利完成! url: {url}");
            }
            else {
                TapLogger.Debug($"下载超时,直接返回空! url: {url}");
                callback?.Invoke(null); // 下载超时,直接返回空
                while (!www.isDone) {
                    yield return null;
                }
                
                if (!www.isNetworkError && !www.isHttpError) {
                    OnDownloadSuccess(www, url, null);
                    TapLogger.Debug($"下载超时完成! url: {url}");
                }
            }
        }

        private static void OnDownloadSuccess(UnityWebRequest www, string url, Action<Texture2D> callback) {
            Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            // 添加到缓存
            cacheDict.Add(url, texture);
            callback?.Invoke(texture); // 返回加载成功的图片
        }
    }
}