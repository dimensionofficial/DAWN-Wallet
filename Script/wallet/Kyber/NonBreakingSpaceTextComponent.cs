/* ==============================================================================
 * 功能描述：将Text组件的space(0x0020)替换为No-break-space(0x00A0)，避免中文使用半角空格导致的提前换行问题
 * 创 建 者：shuchangliu
 * ==============================================================================*/

using UnityEngine.UI;
using UnityEngine;

[RequireComponent(typeof(Text))]
public class NonBreakingSpaceTextComponent : MonoBehaviour
{
    public static readonly string no_breaking_space = "\u00A0";

    protected Text text;
    // Use this for initialization
    void Awake()
    {
        text = this.GetComponent<Text>();
        text.RegisterDirtyVerticesCallback(OnTextChange);
    }

    public void OnTextChange()
    {
        if (text.text.Contains(" "))
        {
            text.text = text.text.Replace(" ", no_breaking_space);
        }
    }

}
