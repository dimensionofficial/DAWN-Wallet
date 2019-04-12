using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridRank : MonoBehaviour {
    public int x;
    public int y;
    // Use this for initialization
    void Start () {
        var grid = transform.GetComponent<GridLayoutGroup>();
        var rect = transform.GetComponent<RectTransform>().rect;
        grid.cellSize = new Vector2(Screen.width/x, Screen.height /y);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
