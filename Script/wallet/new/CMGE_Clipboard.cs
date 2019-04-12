using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class CMGE_Clipboard : MonoBehaviour {
#if UNITY_IPHONE
    [DllImport("__Internal")]
    public static extern void _copyTextToClipboard(string text);//对ios剪切板的调用  

	[DllImport("__Internal")]
	public static extern int _getStatuBarIOS();//对ios剪切板的调用  

	[DllImport("__Internal")]
	public static extern int _getPointPx ();//获取iPhone像素比
#endif  


    public static void CopyToClipboard(string input)
    {
#if UNITY_ANDROID
        AndroidJavaObject androidObject = new AndroidJavaObject("com.example.mylibrary.ClipboardTools");
        AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
        if (activity == null)
            return;

        // 复制到剪贴板
        androidObject.Call("copyTextToClipboard", activity, input);
#elif UNITY_IPHONE
        _copyTextToClipboard(input);
#elif UNITY_EDITOR
        TextEditor t = new TextEditor();
        t.content = new GUIContent(input);
        t.OnFocus();
        t.Copy();
#endif
    }

}
