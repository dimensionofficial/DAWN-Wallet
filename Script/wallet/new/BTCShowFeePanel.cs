using System.Collections;
using System.Collections.Generic;
using NBitcoin;
using UnityEngine;
using UnityEngine.UI;

public class BTCShowFeePanel : MonoBehaviour
{
    public GameObject ensurePanel;
    public GameObject btns;
    public SendCoinPanel sendPanel;
    private MoneyUnit moneyType = MoneyUnit.Satoshi;

    public Text feeMoneyText;
    public Text feeLevelText;

    private int feeLevel = 1;
    private int fastestFee;
    private int halfHourFee;
    private int hourFee;
    private int currentFeeType;
    private Transaction transaction;
    private Money m_fee;
    public Money fee
    {
        get { return m_fee; }
    }
    public int txSizeInBytes = 250;
    int fastestSatoshiPerByteFee;

    public void LeftBtn()
    {
        feeLevel--;
        if (feeLevel < 0)
        {
            feeLevel = 0;
        }
        feeLevelText.text = feeLevel.ToString();
    }

    public void RightBtn()
    {
        feeLevel++;
        if (feeLevel > 2)
        {
            feeLevel = 2;
        }
        feeLevelText.text = feeLevel.ToString();
    }

    /// <summary>
    /// 获取旷工费
    /// </summary>
    public void InitMoneyFee()
    {
        QBitNinja4Unity.QBitNinjaClient.GetFee(NBitcoin.Network.Main, GetFee);
    }

    void GetFee(QBitNinja4Unity.Models.Fees result)
    {
        fastestFee = result.fastestFee;
        halfHourFee = result.halfHourFee;
        hourFee = result.hourFee;

        feeLevel = 1;
        feeLevelText.text = feeLevel.ToString();
    }

    public void FeeNumber(int txSizeInBytes)
    {
        gameObject.SetActive(true);
        if (feeLevel == 0)
        {
            fastestSatoshiPerByteFee = hourFee;
            moneyType = MoneyUnit.Satoshi;
        }
        else if (feeLevel == 1)
        {
            fastestSatoshiPerByteFee = fastestFee;
            moneyType = MoneyUnit.Satoshi;
        }
        else if (feeLevel == 2)
        {
            fastestSatoshiPerByteFee = halfHourFee;
            moneyType = MoneyUnit.Bit;
        }
        m_fee = new Money(fastestSatoshiPerByteFee * txSizeInBytes, moneyType);
        feeMoneyText.gameObject.SetActive(true);
        feeMoneyText.text = m_fee.ToString();
        feeLevelText.gameObject.SetActive(true);
        feeLevelText.text = feeLevel.ToString();
        btns.SetActive(true);
        ensurePanel.SetActive(true);
    }


    public void OnClickCancl()
    {
        ensurePanel.SetActive(false);
    }

    public void OnClickSend()
    {
        btns.SetActive(false);
     //   sendPanel.Send2(fee);
        gameObject.SetActive(false);
    }

}
