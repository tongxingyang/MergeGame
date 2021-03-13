using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemInfo : MonoBehaviour {
	public enum ProductType {
		style,
		wallpaper,
		def
    }

	public int productNum;
	public ProductType productType;

	public bool _isBuy = false;
	public bool isBuy {
		get { return _isBuy; }
		set {
			_isBuy = value;
			if (value) {
				if (productType == ProductType.style || productType == ProductType.wallpaper) {
					foreach (Transform children in this.GetComponentsInChildren<Transform>()) {
						if (children.name == this.transform.name) continue;
						children.gameObject.SetActive(false);
					}
				}
			}
		}
	}

	public TextMeshProUGUI priceText;

	private void Start() {
		if (priceText == null)
			priceText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();

		if (productType == ProductType.style || productType == ProductType.wallpaper) {
			GetParent().GetComponent<Toggle>().onValueChanged.AddListener(
				(bool isOn) => {
					if (isOn) {
						if (productType == ProductType.style)
							ShoppingManager.init.StyleClick(productNum);
						else if (productType == ProductType.wallpaper)
							ShoppingManager.init.WallpaperClick(productNum);
					}
				}
				);
		} else {
			this.GetComponent<Button>().onClick.AddListener(DefClick);
		}
	}

	private void DefClick() {
		ShoppingManager.init.DefClick(productNum, this);
	}

	public GameObject GetParent() {
		return this.transform.parent.gameObject;
	}


}