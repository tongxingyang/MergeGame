using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class IAPManager : MonoBehaviour, IStoreListener {
	public const string PREMIUM = "premium";
	public const string AND_PREMIUM = "premium";

	private static IAPManager _init;
	public static IAPManager init {
		get {
			if (_init != null) return _init;
			_init = FindObjectOfType<IAPManager>();

			if (_init == null)
				_init = new GameObject("IAPManager").AddComponent<IAPManager>();
			return _init;
		}
	}

	private IStoreController storeController;
	private IExtensionProvider extensionProvider;

	public bool isInit => storeController != null && extensionProvider != null;

	private void Awake() {
		if(_init != null && _init != this) {
			Destroy(gameObject);
			return;
		}
		DontDestroyOnLoad(gameObject);
		InitUnityIAP();
	}

	private void InitUnityIAP() {
		if (isInit) return;

		var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

		builder.AddProduct(
			PREMIUM, ProductType.NonConsumable,
			new IDs() {
				{ AND_PREMIUM, GooglePlay.Name }
			}
		);

		UnityPurchasing.Initialize(this, builder);
	}

	public void OnInitialized(IStoreController controller, IExtensionProvider extension) {
		Debug.Log("IAP초기화");
		storeController = controller;
		extensionProvider = extension;

		if (HadPurchased(PREMIUM)) {
			GameManager.init.BuyPremium();
		}
	}

	public void OnInitializeFailed(InitializationFailureReason error) {
		Debug.LogError($"IAP초기화 실패 {error}");
	}

	//구매 시도 완료 직전
	public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent) {
		Debug.Log($"구매결과{purchaseEvent.purchasedProduct.definition.id}");

		if(purchaseEvent.purchasedProduct.definition.id == PREMIUM) {
			GameManager.init.BuyPremium();
		}

		return PurchaseProcessingResult.Complete;
	}

	public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason) {
		Debug.LogError($"실패 {product.definition.id}, {failureReason}");
	}

	public void Purchase(string productId) {
		if (!isInit) return;

		var product = storeController.products.WithID(productId);

		if(product != null && product.availableToPurchase) {
			Debug.Log($"구매시도 = {product.definition.id}");
			storeController.InitiatePurchase(product);
		} else {
			Debug.Log($"구매 시도 불가 {productId}");
		}
	}

	public void RestorePurchase() {
		if (!isInit) return;
		if(Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer) {
			Debug.Log("구매 복구");

			var appleExt = extensionProvider.GetExtension<IAppleExtensions>();
			appleExt.RestoreTransactions(
				result => Debug.Log($"구매 복구 시도 결과 - {result}"));
		}
	}

	public bool HadPurchased(string productId) {
		if (!isInit) return false;
		var product = storeController.products.WithID(productId);

		if (product != null) {
			return product.hasReceipt;
		}

		return false;
	}
}
