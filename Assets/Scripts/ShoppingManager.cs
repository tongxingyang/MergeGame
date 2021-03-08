using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShoppingManager : MonoBehaviour{
	static readonly private int IS_OPEN = Animator.StringToHash("isOpen");

	public ItemInfo[] style;
	public ItemInfo[] wallpaper;
	public GameObject buyUI;
	public GameObject notEnoughtMessageUI;
	public GameObject buyMessageUI;

	private ItemInfo currItem;

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

    private void Start() {
		InitImage(style);
		InitImage(wallpaper);
    }

	private void InitImage(ItemInfo[] products) {
		foreach (ItemInfo product in products) {
			if (product.isBuy) {
				SetProductUI(product, new Color(1, 1, 1, 0), new Vector3(0.95f, 0.95f, 0.95f));
			}
		}
	}

    private void Click(int num, ItemInfo[] products) {
        if (!products[num].isBuy) {
			IsBuy(products[num]);
			return;
		} else {
			InitImage(products);
			SetProductUI(products[num], new Color(1, 1, 1, 1), Vector3.one);
			ApplyItem(products[num]);
			products[num].GetComponent<Image>().sprite = Resources.Load<Sprite>("uiOB");
		}
	}

	private void SetProductUI(ItemInfo product, Color color, Vector3 scale) {
		product.GetParent().GetComponent<RectTransform>().localScale = scale;
		product.GetComponent<Image>().color = color;
	}


	private void ApplyItem(ItemInfo product) {
		if(product.productType == ItemInfo.ProductType.style) {
			ObjectManager.init.ChangeObjectSpriteImage(product.productNum);
        } else if (product.productType == ItemInfo.ProductType.wallpaper) {
			ObjectManager.init.ChangeBackgroundImage(product.productNum);
		}
	}

	private void IsBuy(ItemInfo item) {
		currItem = item;

		OpenMessageUI(buyUI);
		buyUI.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text =
			item.priceText.text + "(??)?? ???? ???? ?????????????";
	}

	private void OpenMessageUI(GameObject gameObject) {
		gameObject.SetActive(true);
		gameObject.GetComponent<Animator>().SetBool(IS_OPEN, true);
    }

	public void CloseMessageUI(GameObject gameObject) {
		gameObject.GetComponent<Animator>().SetBool(IS_OPEN, false);
	}

	public void Buy() {
		if(CheckCoin(ScoreManager.init.coin, ProductPrice())) {
			ScoreManager.init.coin -= ProductPrice();
        } else {
			CloseMessageUI(buyUI);
			OpenMessageUI(notEnoughtMessageUI);
			return;
        }

		currItem.isBuy = true;
		if(currItem.productType == ItemInfo.ProductType.style) {
			StyleClick(currItem.productNum);
        } else if (currItem.productType == ItemInfo.ProductType.wallpaper) {
			WallpaperClick(currItem.productNum);
        }

		OpenMessageUI(buyMessageUI);
		CloseMessageUI(buyUI);
	}

	private int ProductPrice() {
		if (int.TryParse(currItem.priceText.text.Replace(",", ""), out int value)) {
			return value;
		} else {
			return 0;
		}
	}

	private bool CheckCoin(int currCoin, int price) {
		if (currCoin < price || price == 0) return false;
		else {
			return true;
		}
    }
}