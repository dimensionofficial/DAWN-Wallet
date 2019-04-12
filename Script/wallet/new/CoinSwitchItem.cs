using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class CoinSwitchItem : MonoBehaviour
{
    public bool isUsed = false;

    public Text coinName;
    public Text coinFullName;

	/// <summary>
	/// 数量
	/// </summary>
	public decimal blance = -1;
  //public Toggle switchToggle;

    public GameObject checkMark;
	public bool isOn;

    public string tokenFullName;
    public string tokenName;
    public string tokenAddress;
    public string iconPath = "";

    public EthcoinInfoItem currentItem;

    public Image image;
    bool isLoading = false;

    public void Show(EthcoinInfoItem item, string _iconPath, string address, string name, string fullName, bool _isOn = false)
    {
		EthcoinInfoItem ethItem = PanelManager._Instance._WalletInfoPanel.currentItem as EthcoinInfoItem;
		if (ethItem.tokenDicBalance.ContainsKey (address)) {
			blance = ethItem.tokenDicBalance [address];
		} else
		{
			if (_isOn)
			{
				blance = 0;
			}
		}

        image.gameObject.SetActive(false);
        currentItem = item;
        gameObject.SetActive(true);
        tokenName = name;
        iconPath = _iconPath;
        coinName.text = name;
        tokenFullName = fullName;
        coinFullName.text = fullName;
        tokenAddress = address;
        isOn = _isOn;
        checkMark.SetActive(isOn);
        if (isLoading)
        {
            StopAllCoroutines();
            isLoading = false;
        }
        StartCoroutine(TextureUIAsset._Instance.LoadImage(image, iconPath));
    }

    public void OnClickCheckBtn()
    {
        isOn = !isOn;
        checkMark.SetActive(isOn);
        currentItem.SetTokenAddress(tokenAddress, tokenName, iconPath, tokenFullName, isOn);
    }

    

}
