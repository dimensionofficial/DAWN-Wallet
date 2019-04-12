using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TestCl1 : MonoBehaviour {


	// Use this for initialization
	void Start ()
    {
        //initialize a dictionary with keys and values.  
        Dictionary<int, string> plants = new Dictionary<int, string>() {
  {1,"Speckled Alder"},
  {2,"Apple of Sodom"},
  {3,"Hairy Bittercress"},
  {4,"Pennsylvania Blackberry"},
  {5,"Apple of Sodom"},
  {6,"Water Birch"},
  {7,"Meadow Cabbage"},
  {8,"Water Birch"}
};

        Debug.Log("<b>dictionary elements........ www.jb51.net </b><br />");

        //loop dictionary all elements  
        foreach (KeyValuePair<int, string> pair in plants)
        {
            Debug.Log(pair.Key + "....." + pair.Value + "<br />");
        }

        //find dictionary duplicate values. 
        var duplicateValues = plants.GroupBy(x => x.Value).Where(x => x.Count() > 1);
        //loop dictionary duplicate values only      
        foreach (var item in duplicateValues)
        {
            Debug.Log(item.Key + "<br />");
        }




    }

    // Update is called once per frame
    void Update () {
		
	}
}
