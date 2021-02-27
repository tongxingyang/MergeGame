using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class IAPManager : MonoBehaviour, IStoreListener {
	public const string PREMIUM = "premium";

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
			PREMIUM, ProductType.NonConsumable
		);

		UnityPurchasing.Initialize(this, builder);
	}

	public void OnInitialized(IStoreController controller, IExtensionProvider extension) {
		storeController = controller;
		extensionProvider = extension;
	}

	public bool HadPurchased(string productId) {
		if (!isInit) return false;
		var product = storeController.products.WithID(PREMIUM);
		if(product != null) {
			return product.hasReceipt;
		}

		return false;
	}

	public void OnInitializeFailed(InitializationFailureReason error) {
		((IStoreListener)init).OnInitializeFailed(error);
	}

	public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent) {
		if(purchaseEvent.purchasedProduct.definition.id == PREMIUM) {
			GameManager.init.BuyPremium();
		}

		return PurchaseProcessingResult.Complete;
	}

	public void Purchase(string productId) {
		if (!init) return;

		var product = storeController.products.WithID(productId);

		if(product != null && product.availableToPurchase) {
			Debug.Log("구매시도");
			storeController.InitiatePurchase(product);
		}
	}

	public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason) {
		((IStoreListener)init).OnPurchaseFailed(product, failureReason);
	}
}
