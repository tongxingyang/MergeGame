using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemInfo : MonoBehaviour {

	private bool _isBuy = false;
	public bool isBuy{
		get { return _isBuy; }
		set {
			_isBuy = value;
		}
	}

	public GameObject buyUI;
	private TextMeshProUGUI priceText;

	private void Start() {
		priceText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
	}

	public GameObject GetParent() {
		return this.transform.parent.gameObject;
	}

	public void Buy() {
		buyUI.SetActive(true);
		buyUI.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = 
			priceText.text + "(으)로 잠금 해제 하시겠습니까?";
	}

}