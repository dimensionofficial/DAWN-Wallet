using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class RawImagePivot : MonoBehaviour {

    public RectTransform target;

	// Use this for initialization
	void Start ()
    {
#if UNITY_ANDROID
        target.anchorMin = new Vector2(0.5F, 0.5F);
        target.anchorMax = new Vector2(0.5F, 0.5F);
#endif
#if UNITY_IPHONE
        target.anchorMin = new Vector2(0F, 0F);
        target.anchorMax = new Vector2(1F, 1F);
#endif
    }
}
