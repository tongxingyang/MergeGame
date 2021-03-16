using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurchaseButton : MonoBehaviour {
	public string targetProductId;

	public void HandleClick() {

		if(targetProductId == IAPManager.PREMIUM
			|| targetProductId == IAPManager.DOUBLE_COIN) {
			if (IAPManager.init.HadPurchased(targetProductId)) {
				Debug.LogError($"Is had Purchased : {targetProductId}");
				return;
			}
		}
		IAPManager.init.Purchase(targetProductId);
	}
}