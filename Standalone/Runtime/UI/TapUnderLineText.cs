using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace TapTap.Common.Standalone {
public enum UnderLineType
{
    //画线位置
    Bottom = 0,
    Center
}

//文本编辑扩展
public class TapUnderLineText : MonoBehaviour
{
    public Text linkText;
    public UnderLineType underLineType;
    public bool autoLink = true;
    private string underLineText = "_";

    private void Awake()
    {
        if (underLineType == UnderLineType.Bottom)
        {
            underLineText = "_";
        }
        else
        {
            underLineText = "-";
        }
    }

    private void Start()
    {
        if (autoLink)
            CreateLink(linkText, null);
    }

    public void CreateLink(Text text, UnityEngine.Events.UnityAction onClickBtn = null)
    {
        if (text == null)
            return;

        //克隆Text，获得相同的属性
        Text underline = Instantiate(text) as Text;
        underline.name = "Underline";
        underline.transform.SetParent(text.transform);
        underline.transform.localScale = Vector3.one;
        RectTransform rt = underline.rectTransform;
        //设置下划线坐标和位置
        rt.anchoredPosition3D = Vector3.zero;
        rt.offsetMax = Vector2.zero;
        rt.offsetMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.anchorMin = Vector2.zero;
        underline.text = underLineText;
        float perlineWidth = underline.preferredWidth;      //单个下划线宽度
        float width = text.preferredWidth;
        int lineCount = (int)Mathf.Round(width / perlineWidth);
        for (int i = 1; i < lineCount; i++)
        {
            underline.text += underLineText;
        }
        if (onClickBtn != null)
        {
            var btn = text.gameObject.AddComponent<Button>();
            btn.onClick.AddListener(onClickBtn);
        }
        var underLine = underline.GetComponent<TapUnderLineText>();
        if (underLine)
        {
            underLine.autoLink = false;
        }
    }
}
}