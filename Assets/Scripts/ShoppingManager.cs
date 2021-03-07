using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShoppingManager : MonoBehaviour{

	public Image[] style;
	public Image[] wallpaper;

	public static ShoppingManager init = null;
	private void Awake() {
		if (init == null) {
			init = this;
		}
		else if (init != this) {
			Destroy(this.gameObject);
		}
		DontDestroyOnLoad(this.gameObject);
	}

	public void StyleClick(int num) {
		Click(num, style);
	}

	public void WallpaperClick(int num) {
		Click(num, wallpaper);
	}

	private void Click(int num, Image[] products) {

		foreach (Image image in products) {
			if (image.GetComponent<ItemInfo>().isBuy) {
				image.GetComponent<ItemInfo>().GetParent().GetComponent<RectTransform>().localScale = new Vector3(0.95f, 0.95f, 0.95f);
				image.color = new Color(1, 1, 1, 0);
			}
		}

		if (products[num].GetComponent<ItemInfo>().isBuy) {
			products[num].GetComponent<ItemInfo>().GetParent().GetComponent<RectTransform>().localScale = Vector3.one;
			products[num].sprite = Resources.Load<Sprite>("uiOB");
			products[num].color = new Color(1, 1, 1, 1);
		}
		else {
			products[num].GetComponent<ItemInfo>().Buy();
		}
	}
}