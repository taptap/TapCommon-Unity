using UnityEngine;

namespace TapTap.Common
{
    public class TapLocalizeManager
    {
        private static volatile TapLocalizeManager _instance;
        private static readonly object ObjLock = new object();

        public static TapLocalizeManager Instance
        {
            get
            {
                if (_instance != null) return _instance;
                lock (ObjLock)
                {
                    if (_instance == null)
                    {
                        _instance = new TapLocalizeManager();
                    }
                }

                return _instance;
            }
        }

        private bool _regionIsCn;

        public static void SetCurrentRegion(bool isCn)
        {
            Instance._regionIsCn = isCn;
        }

        private TapLanguage _language = TapLanguage.AUTO;

        public static void SetCurrentLanguage(TapLanguage language)
        {
            Instance._language = language;
        }

        public static TapLanguage GetCurrentLanguage()
        {
            return Instance._language != TapLanguage.AUTO ? Instance._language : GetSystemLanguage();
        }

        public static string GetCurrentLanguageString() {
            TapLanguage lang = GetCurrentLanguage();
            switch (lang) {
                case TapLanguage.ZH_HANS:
                    return "zh_CN";
                case TapLanguage.EN:
                    return "en_US";
                case TapLanguage.ZH_HANT:
                    return "zh_TW";
                case TapLanguage.JA:
                    return "ja_JP";
                case TapLanguage.KO:
                    return "ko_KR";
                case TapLanguage.TH:
                    return "th_TH";
                case TapLanguage.ID:
                    return "id_ID";
                case TapLanguage.DE:
                    return "de_DE";
                case TapLanguage.ES:
                    return "es_ES";
                case TapLanguage.FR:
                    return "fr_FR";
                case TapLanguage.PT:
                    return "pt_PT";
                case TapLanguage.RU:
                    return "ru_RU";
                case TapLanguage.TR:
                    return "tr_TR";
                case TapLanguage.VI:
                    return "vi_VN";
                default:
                    return Instance._regionIsCn ? "zh_CN" : "en_US";
            }
        }

        public static string GetCurrentLanguageString2()
        {
            return GetCurrentLanguageString().Replace("_", "-");
        }

        private static TapLanguage GetSystemLanguage()
        {
            var lang = TapLanguage.AUTO;
            var sysLanguage = Application.systemLanguage;

            switch (sysLanguage)
            {
                case SystemLanguage.ChineseSimplified:
                    lang = TapLanguage.ZH_HANS;
                    break;
                case SystemLanguage.English:
                    lang = TapLanguage.EN;
                    break;
                case SystemLanguage.ChineseTraditional:
                    lang = TapLanguage.ZH_HANT;
                    break;
                case SystemLanguage.Japanese:
                    lang = TapLanguage.JA;
                    break;
                case SystemLanguage.Korean:
                    lang = TapLanguage.KO;
                    break;
                case SystemLanguage.Thai:
                    lang = TapLanguage.TH;
                    break;
                case SystemLanguage.Indonesian:
                    lang = TapLanguage.ID;
                    break;
                default:
                    lang = Instance._regionIsCn ? TapLanguage.ZH_HANS : TapLanguage.EN;
                    break;
            }

            return lang;
        }
    }
}