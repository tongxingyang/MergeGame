using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurchaseButton : MonoBehaviour {
	public string targetProductId;

	public void HandleClick() { 
		if (targetProductId == IAPManager.PREMIUM) {
			if (IAPManager.init.HadPurchased(targetProductId)) {
				Debug.Log("���� �Ϸ��� ��ǰ");
				GameManager.init.BuyPremium();
				return;
			}
		}

		IAPManager.init.Purchase(targetProductId);
	}
}