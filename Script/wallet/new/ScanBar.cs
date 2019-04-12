using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ScanBar : MonoBehaviour
{

	public RectTransform target;
    public float startY;
    public float endY;
 //   public Ease easeType;

    public void StartDo()
    {
        gameObject.SetActive(true);
		target.anchoredPosition = new Vector3(0, startY, 0);
		StartCoroutine (Move());
   //     target.DOLocalMoveY(endY, 3).SetEase(easeType).SetLoops(-1, LoopType.Restart);
    }

	IEnumerator Move()
	{
		yield return 0;
		int tempY = 600;
		while (true)
		{
			yield return new WaitForFixedUpdate();
			tempY -= 6;
			if (tempY < 0)
			{
				tempY = 600;
			}
			target.anchoredPosition = new Vector3(0, tempY, 0);
		}
	}
}
