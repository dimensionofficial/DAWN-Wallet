using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EOSResourcesOverPanel : MonoBehaviour
{
    public Image titleIamge;
    public Text titleText;
    public Text titleCountText;
    public Text toCount;
    public Text recerAccountText;
    public Text meanText;
    public EOSSendScanSing.LastPanel lastPanel = EOSSendScanSing.LastPanel.none;
    public void Show(EOSSendScanSing.LastPanel _panel, string _toCount, string _Account, string _mean)
    {
        gameObject.SetActive(true);
        lastPanel = _panel;
        titleCountText.text = _toCount;
        toCount.text = _toCount;
        recerAccountText.text = _Account;
        meanText.text = _mean;
        switch (_panel)
        {
            case EOSSendScanSing.LastPanel.buyram:
            case EOSSendScanSing.LastPanel.buyrambytes:
                titleIamge.overrideSprite = TextureUIAsset._Instance.eos_ram_in_big;
                break;
            case EOSSendScanSing.LastPanel.delegatebw:
                titleIamge.overrideSprite = TextureUIAsset._Instance.eos_delegatebw_in_big;
                break;
            case EOSSendScanSing.LastPanel.sellram:
                titleIamge.overrideSprite = TextureUIAsset._Instance.eos_ram_out_big;
                break;
            case EOSSendScanSing.LastPanel.undelegatebw:
                titleIamge.overrideSprite = TextureUIAsset._Instance.eos_delegatebw_out_big;
                break;
           
        }
    }


}
