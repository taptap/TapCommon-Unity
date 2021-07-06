using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor.iOS.Xcode;
using UnityEngine;

namespace TapTap.Common.Editor
{
    public static class TapCommonCompile
    {
        public static string GetProjPath(string path)
        {
            return PBXProject.GetPBXProjectPath(path);
        }

        public static PBXProject ParseProjPath(string path)
        {
            var proj = new PBXProject();
            proj.ReadFromString(File.ReadAllText(path));
            return proj;
        }

        public static string GetUnityFrameworkTarget(PBXProject proj)
        {
#if UNITY_2019_3_OR_NEWER
            string target = proj.GetUnityFrameworkTargetGuid();
            return target;
#endif
            var unityPhoneTarget = proj.TargetGuidByName("Unity-iPhone");
            return unityPhoneTarget;
        }

        public static string GetUnityTarget(PBXProject proj)
        {
#if UNITY_2019_3_OR_NEWER
            string target = proj.GetUnityMainTargetGuid();
            return target;
#endif
            var unityPhoneTarget = proj.TargetGuidByName("Unity-iPhone");
            return unityPhoneTarget;
        }

        public static bool CheckTarget(string target)
        {
            return string.IsNullOrEmpty(target);
        }

        public static bool HandlerIOSSetting(string path, string appDataPath, string resourceName,
            string modulePackageName,
            string moduleName, string[] bundleNames, string target, string projPath, PBXProject proj)
        {
            var resourcePath = Path.Combine(path, resourceName);

            var parentFolder = Directory.GetParent(appDataPath).FullName;

            Debug.Log($"ProjectFolder path:{parentFolder}");

            if (Directory.Exists(resourcePath))
            {
                Directory.Delete(resourcePath, true);
            }

            Directory.CreateDirectory(resourcePath);

            var remotePackagePath =
                TapFileHelper.FilterFile(parentFolder + "/Library/PackageCache/", $"{modulePackageName}@");

            var assetLocalPackagePath = TapFileHelper.FilterFile(parentFolder + "/Assets/TapTap/", moduleName);

            var localPackagePath = TapFileHelper.FilterFile(parentFolder, moduleName);

            var tdsResourcePath = "";

            if (!string.IsNullOrEmpty(remotePackagePath))
            {
                tdsResourcePath = remotePackagePath;
            }
            else if (!string.IsNullOrEmpty(assetLocalPackagePath))
            {
                tdsResourcePath = assetLocalPackagePath;
            }
            else if (!string.IsNullOrEmpty(localPackagePath))
            {
                tdsResourcePath = localPackagePath;
            }

            if (string.IsNullOrEmpty(tdsResourcePath))
            {
                Debug.LogError("tdsResourcePath is NUll");
                return false;
            }

            tdsResourcePath = $"{tdsResourcePath}/Plugins/iOS/Resource";

            Debug.Log($"Find {moduleName} path:{tdsResourcePath}");

            if (!Directory.Exists(tdsResourcePath))
            {
                Debug.LogError($"Can't Find {bundleNames}");
                return false;
            }

            TapFileHelper.CopyAndReplaceDirectory(tdsResourcePath, resourcePath);

            foreach (var name in bundleNames)
            {
                proj.AddFileToBuild(target,
                    proj.AddFile(Path.Combine(resourcePath, name), Path.Combine(resourcePath, name),
                        PBXSourceTree.Source));
            }

            File.WriteAllText(projPath, proj.WriteToString());
            return true;
        }

        public static bool HandlerPlist(string pathToBuildProject, string infoPlistPath)
        {
            //添加info
            var plistPath = pathToBuildProject + "/Info.plist";
            var plist = new PlistDocument();
            plist.ReadFromString(File.ReadAllText(plistPath));
            var rootDic = plist.root;

            var items = new List<string>
            {
                "tapsdk",
                "tapiosdk",
            };
            var plistElementList = rootDic.CreateArray("LSApplicationQueriesSchemes");
            foreach (var t in items)
            {
                plistElementList.AddString(t);
            }

            if (string.IsNullOrEmpty(infoPlistPath)) return false;
            var dic = (Dictionary<string, object>) Plist.readPlist(infoPlistPath);
            var taptapId = "";

            foreach (var item in dic)
            {
                if (item.Key.Equals("taptap"))
                {
                    var taptapDic = (Dictionary<string, object>) item.Value;
                    foreach (var taptapItem in taptapDic.Where(taptapItem => taptapItem.Key.Equals("client_id")))
                    {
                        taptapId = "tt" + (string) taptapItem.Value;
                    }
                }
                else
                {
                    rootDic.SetString(item.Key, item.Value.ToString());
                }
            }

            //添加url
            var dict = plist.root.AsDict();
            var array = dict.CreateArray("CFBundleURLTypes");
            var dict2 = array.AddDict();
            dict2.SetString("CFBundleURLName", "TapTap");
            var array2 = dict2.CreateArray("CFBundleURLSchemes");
            array2.AddString(taptapId);

            Debug.Log("TapSDK change plist Success");
            File.WriteAllText(plistPath, plist.WriteToString());
            return true;
        }

        public static string GetValueFromPlist(string infoPlistPath, string key)
        {
            if (infoPlistPath == null)
            {
                return null;
            }

            var dic = (Dictionary<string, object>) Plist.readPlist(infoPlistPath);
            return (from item in dic where item.Key.Equals(key) select (string) item.Value).FirstOrDefault();
        }
    }
}