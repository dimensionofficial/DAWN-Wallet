using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;

public class TokenPrefab : MonoBehaviour {
    public string symbol;
    public string _name;
    public int decimals;
    public string contractAddress;
    public BigInteger allowance;
    public Text tokenName;
    public Image isSelected;
    public Button _button;
    public Image tokenImage;
    public Text balance;
}
