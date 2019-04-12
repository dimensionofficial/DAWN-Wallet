using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QRImagePicter : MonoBehaviour
{
    public Vector3 leftPos;

	public Text currentIndex;
	public Image qrImage;
	public string info;
	public Button nextBtn;
    public Button onUpBtn;
    private QRCodeManager qrCodeManager = new QRCodeManager(); 
    public Button nextStepBtn;

	public int index;

	public void ShowMe(string _info, int _index, int totalCount)
	{
        nextStepBtn.gameObject.SetActive(false);

        if (_index == 1)
		{
            onUpBtn.interactable = false;

            gameObject.SetActive (true);

		}

        string one = _index.ToString();
        if (_index < 10)
        {
            one = "00" + _index.ToString();
        }
        else if(_index < 100)
        {
            one = "0" + _index.ToString();
        }
        string two = totalCount.ToString();
        if (totalCount < 10)
        {
            two = "00" + totalCount.ToString();
        }
        else if (totalCount < 100)
        {
            two = "0" + totalCount.ToString();
        }

        _info = _info + one + "/" + two;


        qrCodeManager.EncodeQRCode(qrImage, _info);

		index = _index;
		info = _info;
		currentIndex.text = index + "/" + totalCount;



		if (index == totalCount)
		{
			nextBtn.gameObject.SetActive (false);
            nextStepBtn.gameObject.SetActive(true);
            if (index == 1)
            {
                onUpBtn.gameObject.SetActive(false);
                nextStepBtn.transform.localPosition = new Vector3(0, leftPos.y, leftPos.z);
            }
            else
            {
                nextStepBtn.transform.localPosition = leftPos;
                nextBtn.gameObject.SetActive(false);
            }
        }
	}

}
