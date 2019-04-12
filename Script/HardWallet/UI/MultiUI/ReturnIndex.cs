using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnIndex : MonoBehaviour {
    public int index;
	public void Returnindex()
    {
        impowerManager.instance.curSN = index;
    }
}
