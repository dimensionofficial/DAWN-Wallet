using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using cn.sharesdk.unity3d;
using System;

public class ShareManager : MonoBehaviour
{
    public ShareSDK ssdk;
    public Text txtName;
    public RawImage imgHeadIcon;
    public Image imgShare;
    public static ShareManager init;
    public string shareStr;
    public Texture2D tex;
    public GameObject sharePage;
    private void Awake()
    {
        init = this;
    }
    // Use this for initialization
    void Start()
    {
        //ssdk = GetComponent<ShareSDK>();

        ssdk.authHandler = OnAuthResultHandler;
        ssdk.showUserHandler = OnGetUserInfoResultHandler;
        ssdk.shareHandler = OnShareResultHandler;
    }

    public void LoginWeChat()
    {
        ssdk.Authorize(PlatformType.WeChat);
    }



    void OnAuthResultHandler(int reqID, ResponseState state, PlatformType type, Hashtable result)
    {
        if (state == ResponseState.Success)
        {
            if (result.Count > 0)
            {
                print("authorize success !" + "Platform :" + type + "result:" + MiniJSON.jsonEncode(result));
            }
            else
            {
                print("authorize success !" + "Platform :" + type);
            }

            ssdk.GetUserInfo(PlatformType.WeChat);
        }
        else if (state == ResponseState.Fail)
        {
#if UNITY_ANDROID
            print("fail! throwable stack = " + result["stack"] + "; error msg = " + result["msg"]);
#elif UNITY_IPHONE
			print ("fail! error code = " + result["error_code"] + "; error msg = " + result["error_msg"]);
#endif
        }
        else if (state == ResponseState.Cancel)
        {
            print("cancel !");
        }
    }

    void OnGetUserInfoResultHandler(int reqID, ResponseState state, PlatformType type, Hashtable result)
    {
        if (state == ResponseState.Success)
        {
            print("get user info result :");
            print(MiniJSON.jsonEncode(result));
            print("AuthInfo:" + MiniJSON.jsonEncode(ssdk.GetAuthInfo(PlatformType.WeChat)));
            print("Get userInfo success !Platform :" + type);


        }
        else if (state == ResponseState.Fail)
        {
#if UNITY_ANDROID
            print("fail! throwable stack = " + result["stack"] + "; error msg = " + result["msg"]);
#elif UNITY_IPHONE
			print ("fail! error code = " + result["error_code"] + "; error msg = " + result["error_msg"]);
#endif
        }
        else if (state == ResponseState.Cancel)
        {
            print("cancel !");
        }
    }

    void OnShareResultHandler(int reqID, ResponseState state, PlatformType type, Hashtable result)
    {
        HideSharePage();
        if (state == ResponseState.Success)
        {
            print("share successfully - share result :");
            print(MiniJSON.jsonEncode(result));
        }
        else if (state == ResponseState.Fail)
        {
#if UNITY_ANDROID
            print("fail! throwable stack = " + result["stack"] + "; error msg = " + result["msg"]);
#elif UNITY_IPHONE
			print ("fail! error code = " + result["error_code"] + "; error msg = " + result["error_msg"]);
#endif
        }
        else if (state == ResponseState.Cancel)
        {
            print("cancel !");
        }
        
    }

    public void ShareText()
    {
        sharePage.SetActive(true);
        return;
        ShareContent content = new ShareContent();
        
        //(Android only) 隐藏九宫格里面不需要用到的平台（仅仅是不显示平台）
        //(Android only) 也可以把jar包删除或者把Enabl属性e改成false（对应平台的全部功能将用不了）
        //String[] platfsList = { ((int)PlatformType.QQ).ToString(), ((int)PlatformType.Facebook).ToString(), ((int)PlatformType.TencentWeibo).ToString() };
        //content.SetHidePlatforms(platfsList);

        content.SetText(shareStr);
        //content.SetImageUrl("http://ww3.sinaimg.cn/mw690/be159dedgw1evgxdt9h3fj218g0xctod.jpg");
        //content.SetTitle(shareStr);

        ////(Android only) 针对Android绕过审核的多图分享，传图片String数组 
        //String[] imageArray = { "/sdcard/test.jpg", "http://f1.webshare.mob.com/dimgs/1c950a7b02087bf41bc56f07f7d3572c11dfcf36.jpg", "/sdcard/test.jpg" };
        //content.SetImageArray(imageArray);

        //content.SetTitleUrl("http://www.mob.com");
        //content.SetSite("Mob-ShareSDK");
        //content.SetSiteUrl("http://www.mob.com");
        //content.SetUrl("http://www.mob.com");
        //content.SetComment("test description");
        //content.SetMusicUrl("http://mp3.mwap8.com/destdir/Music/2009/20090601/ZuiXuanMinZuFeng20090601119.mp3");
        content.SetShareType(ContentType.Text);

        

        //朋友圈分享

        //将图片信息编码为字节信息  
        byte[] bytes = tex.EncodeToPNG();
        string fileStr = Application.persistentDataPath + "/tex.png";
        //保存  
        System.IO.File.WriteAllBytes(fileStr, bytes);
        //ShareContent weChatMovmentContent = new ShareContent();
        //weChatMovmentContent.SetText(shareStr);
        //weChatMovmentContent.SetTitle(shareStr);
        //weChatMovmentContent.SetImagePath(fileStr);
        //weChatMovmentContent.SetShareType(ContentType.Image);
        //content.SetShareContentCustomize(PlatformType.WeChatMoments, weChatMovmentContent);


        //不同平台分享不同内容
        ShareContent customizeShareParams = new ShareContent();
        customizeShareParams.SetText(shareStr);
        customizeShareParams.SetTitle(shareStr);
        customizeShareParams.SetImagePath(fileStr);
        customizeShareParams.SetShareType(ContentType.Image);
        customizeShareParams.SetObjectID("SinaID");
        content.SetShareContentCustomize(PlatformType.SinaWeibo, customizeShareParams);
        //优先客户端分享
        // content.SetEnableClientShare(true);

        //使用微博API接口应用内分享 iOS only
        // content.SetEnableSinaWeiboAPIShare(true);

        //通过分享菜单分享
        PlatformType[] list = new PlatformType[] { PlatformType.WeChat,PlatformType.WeChatMoments,PlatformType.SMS,PlatformType.Mail,PlatformType.SinaWeibo,PlatformType.QQ};
        ssdk.ShowPlatformList(list, content, 100, 100);
    }

    public void ShowShareContent(int _index)
    {
        //1微信 2朋友圈 3微博 4QQ 5短信 6Email

        ShareContent content = new ShareContent();
        content.SetText(shareStr);
        content.SetShareType(ContentType.Text);

        //微博使用的分享内容
        byte[] bytes = tex.EncodeToPNG();
        string fileStr = Application.persistentDataPath + "/tex.png";
        //保存  
        System.IO.File.WriteAllBytes(fileStr, bytes);
        ShareContent customizeShareParams = new ShareContent();
        customizeShareParams.SetText(shareStr);
        customizeShareParams.SetTitle(shareStr);
        customizeShareParams.SetImagePath(fileStr);
        customizeShareParams.SetShareType(ContentType.Image);
        customizeShareParams.SetObjectID("SinaID");
        customizeShareParams.SetEnableClientShare(true);

        ShareContent qqContent = new ShareContent();
        qqContent.SetText(shareStr);
        qqContent.SetTitle(shareStr);
        qqContent.SetImagePath(fileStr);
        customizeShareParams.SetEnableClientShare(true);
        qqContent.SetTitleUrl("http://dimensionchain.io/");


        switch (_index)
        {
            case 1:
                ssdk.ShareContent(PlatformType.WeChat, content);
                break;
            case 2:
                ssdk.ShareContent(PlatformType.WeChatMoments, content);
                break;
            case 3:
                ssdk.ShareContent(PlatformType.SinaWeibo, customizeShareParams);
                break;
            case 4:
                ssdk.ShareContent(PlatformType.QQ, qqContent);
                break;
            case 5:
                ssdk.ShareContent(PlatformType.SMS, content);
                break;
            case 6:
                ssdk.ShareContent(PlatformType.Mail, customizeShareParams);
                break;
            default:
                break;
        }
    }
    public void HideSharePage()
    {
        sharePage.SetActive(false);
    }
}
