using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowWebView : MonoBehaviour
{
    public RectTransform backGround;
    public string webObjectName;
    public string url;
    private UniWebView _webView;
    private bool _webViewReady = false;
    private Timer timer;
    public float tempHeight;//真实像素

    public void ShowClicked()
    {
        RectTransform t = GetComponent<RectTransform>();

        tempHeight = (backGround.sizeDelta.y + NewWalletManager.statusBarOffset)/ 1600*Screen.height;
        //backGround.sizeDelta = new Vector2(backGround.sizeDelta.x, tempHeight);
#if UNITY_ANDROID
       //backGround.sizeDelta = new Vector2(backGround.sizeDelta.x, tempHeight);
#endif

#if UNITY_IPHONE
		tempHeight = Convert.ToInt32(Convert.ToDouble(tempHeight/CMGE_Clipboard._getPointPx()));
        //backGround.sizeDelta = new Vector2(backGround.sizeDelta.x, tempHeight);
#endif

        if (_webViewReady)
        {
            // Call Show() or hide on the web view with transition option.
            // fade: Whether you need a fade effect when showing this web view.
            // transitionEdge: From which side of the screen you want the web view transit in.
            // duration: The duration of your animation effect.
            // Callback: will be called after the transition animation finishes.
			gameObject.SetActive(true);
            _webView.Show();
			//_webView.OnLoadComplete += LoadComplete;
			_webView.Load(url);
			_webView.HideToolBar(true);

        }
        else
        {
            //PanelManager._Instance.loadingPanel.SetActive(true);
			gameObject.SetActive(true);
            _webView = CreateWebView();
			_webView.HideToolBar(true);
            _webView.Load(url);
        }
    }

	private void LoadComplete(UniWebView w, bool b, string s)
	{
		//_webViewReady = true;
		//PanelManager._Instance.loadingPanel.SetActive(false);
		w.Show ();
	}

    UniWebView CreateWebView() {
        var webViewGameObject = GameObject.Find(webObjectName);
        if (webViewGameObject == null) {
            webViewGameObject = new GameObject(webObjectName);
        }

		var webView = webViewGameObject.GetComponent<UniWebView> ();

        if (webView == null)
        {
            webView = webViewGameObject.AddComponent<UniWebView>();
            webView.insets = new UniWebViewEdgeInsets((int)tempHeight, 0, 0, 0);
        }
        _webViewReady = true;
        // We just set a ready flag to make sure we could show a page when the "Open" button is clicked.
        webView.OnLoadComplete += LoadComplete;

        // The `OnWebViewShouldClose` will be called when user pressed Back button or Done button to exit the web view.
        // It will hide the web view without animation if you do not listen to this method or return true in this method.

        // If you want customized transition for hide,
        // You should at least listen to OnWebViewShouldClose, call Hide() with animation options and return false. 
        //webView.OnWebViewShouldClose += (view) => {
        //    return true;
        //};

        return webView;
    }

    public void Back()
    {
       // if (timer != null)
      //  {
      //      TimerManager.Instance.RemoveTimer(timer);
      //      timer = null;
      //  }
		this.gameObject.SetActive(false);
		_webView.OnLoadComplete -= LoadComplete;
        _webView.Hide();
		_webView.HideToolBar(true);
        this.gameObject.SetActive(false);
    }

	void OnDisable()
	{
		Back ();
	}

    
	void OnApplicationPause(bool isPause)
	{
		if (isPause) {
			_webView.Hide ();
		} else {
			if (this.gameObject.activeSelf && _webView != null) {
			    _webView.Show ();
			}
		}
	}
}
