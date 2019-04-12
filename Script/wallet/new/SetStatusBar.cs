using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SetStatusBar : MonoBehaviour
{
    //public Button toggleDimmedButton;
    //public Button visibleButton;
    //public Button visibleOverContentButton;
    //public Button translucentOverContentButton;
    //public Button hiddenButton;
    // Use this for initialization
    void Start()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidStatusBar.dimmed = false;
        AndroidStatusBar.statusBarState = AndroidStatusBar.States.TranslucentOverContent;
#endif

        ////显示状态栏，占用屏幕最上方的一部分像素
        //if (visibleButton != null)
        //{
        //    visibleButton.onClick.AddListener(delegate
        //    {
        //        AndroidStatusBar.statusBarState = AndroidStatusBar.States.Visible;
        //    });
        //}
        ////悬浮显示状态栏，不占用屏幕像素
        //if (visibleOverContentButton != null)
        //{
        //    visibleOverContentButton.onClick.AddListener(delegate
        //    {
        //        AndroidStatusBar.statusBarState = AndroidStatusBar.States.QuanTouMing;
        //    });
        //}
        ////透明悬浮显示状态栏，不占用屏幕像素
        //if (translucentOverContentButton != null)
        //{
        //    translucentOverContentButton.onClick.AddListener(delegate
        //    {
        //        AndroidStatusBar.statusBarState = AndroidStatusBar.States.TranslucentOverContent;
        //    });
        //}
        ////隐藏状态栏
        //if (hiddenButton != null)
        //{
        //    hiddenButton.onClick.AddListener(delegate
        //    {
        //        AndroidStatusBar.statusBarState = AndroidStatusBar.States.QuanTouMingA;
        //    });
        //}
    }

}