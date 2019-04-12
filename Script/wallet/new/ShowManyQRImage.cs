using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowManyQRImage : MonoBehaviour
{
	public List<QRImagePicter> qrImageList = new List<QRImagePicter>();
	public Transform parent;
	public QRImagePicter cloneItem;

	public void ShowMangQr(string info)
	{
        int oneCount = 400;

        int tempCount = info.Length / oneCount;

		if (info.Length % oneCount != 0)
		{
			tempCount += 1;
		}

		List<string> infos = new List<string> ();
		string str = "";
		foreach (char a in info)
		{
			str += a;
			if (str.Length == oneCount)
			{
				infos.Add (str);
				str = "";
			}
		}
		infos.Add (str);

		for (int i = 1; i <= infos.Count; i++) 
		{
			GameObject go = GameObject.Instantiate (cloneItem.gameObject);
			go.transform.SetParent (parent,false);
			QRImagePicter qrPic = go.GetComponent<QRImagePicter> ();
			qrPic.ShowMe (infos[i - 1], i, infos.Count);
			qrImageList.Add (qrPic);
		}
	}

    public void OnClickOnUpPicter(QRImagePicter pic)
    {
        int upIndex = pic.index - 1;
        if (upIndex > 0)
        {
            pic.gameObject.SetActive(false);

            if (qrImageList.Count > upIndex)
            {
                qrImageList[upIndex-1].gameObject.SetActive(true);
            }
        }
       
    }

	public void OnClickNextPicter(QRImagePicter pic)
	{
		pic.gameObject.SetActive (false);

		if (qrImageList.Count > pic.index) 
		{
			qrImageList [pic.index].gameObject.SetActive (true);
		}
	}


	public void ClosedMe()
	{
		for (int i = 0; i < qrImageList.Count; i++)
		{
			Destroy (qrImageList[i].gameObject);
		}
		qrImageList.Clear ();
	}
}
