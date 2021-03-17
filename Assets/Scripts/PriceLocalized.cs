using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PriceLocalized : IAPManager {
	//[SerializeField]
	public string products;

	private void OnEnable() {
		this.GetComponent<TextMeshProUGUI>().text =
			string.Format("{0} {1}", storeController.products.WithID(products).metadata.localizedPrice,
			storeController.products.WithID(products).metadata.isoCurrencyCode);
	}
}
