using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBottom : MonoBehaviour {
	public RectTransform rec;
	public RectTransform bot;
	// Use this for initialization
	void Start () {
		#if UNITY_IOS
		var type = UnityEngine.iOS.Device.generation;
		if (type == UnityEngine.iOS.DeviceGeneration.iPhoneX) {
			bot.gameObject.SetActive (true);
			rec.anchoredPosition = new Vector2 (0, bot.sizeDelta.y);
		}
		#endif
	}
	

	// Update is called once per frame
	void Update () {
		
	}
}
