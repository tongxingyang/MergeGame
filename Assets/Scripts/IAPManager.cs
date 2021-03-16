using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class IAPManager : MonoBehaviour, IStoreListener {
	public const string PREMIUM = "premium";
	public const string DOUBLE_COIN = "com.bognstudio.mergegame.doublecoin";
	public const string COIN_DUMMY = "com.bognstudio.mergegame.coindummy";
	public const string COIN_POKET = "com.bognstudio.mergegame.coinpoket";
	public const string COIN_BOX = "com.bognstudio.mergegame.coinbox";

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
			PREMIUM, ProductType.NonConsumable, new IDs() {
				{ PREMIUM, GooglePlay.Name }
			}
		);

		builder.AddProduct(
			DOUBLE_COIN, ProductType.NonConsumable, new IDs() {
				{ DOUBLE_COIN, GooglePlay.Name }
			}
		);

		builder.AddProduct(
			COIN_DUMMY, ProductType.Consumable, new IDs() {
				{ COIN_DUMMY, GooglePlay.Name }
			}
		);

		builder.AddProduct(
			COIN_POKET, ProductType.Consumable, new IDs() {
				{ COIN_POKET, GooglePlay.Name }
			}
		);

		builder.AddProduct(
			COIN_BOX, ProductType.Consumable, new IDs() {
				{ COIN_BOX, GooglePlay.Name }
			}
		);

		UnityPurchasing.Initialize(this, builder);
	}

	public void OnInitialized(IStoreController controller, IExtensionProvider extension) {
		Debug.Log("IAP initalized");
		storeController = controller;
		extensionProvider = extension;

		if (HadPurchased(PREMIUM)) {
			GameManager.init.isPremium = true;
		}
		
		if (HadPurchased(DOUBLE_COIN)) {
			GameManager.init.isDoubleCoin = true;
		}
	}

	public void OnInitializeFailed(InitializationFailureReason error) {
		Debug.LogError($"IAP failed : {error}");
	}

	//purchased reward
	public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent) {
		Debug.Log($"PurchaseResult : {purchaseEvent.purchasedProduct.definition.id}");

		switch (purchaseEvent.purchasedProduct.definition.id) {
			case PREMIUM:
				GameManager.init.isPremium = true;
				UIManager.init.OpenPanel(UIManager.init.buyMessagePanel);
				break;

			case DOUBLE_COIN:
				GameManager.init.isDoubleCoin = true;
				UIManager.init.OpenPanel(UIManager.init.buyMessagePanel);
				break;

			case COIN_DUMMY:
				ScoreManager.init.AddCoin(2000);
				break;

			case COIN_POKET:
				ScoreManager.init.AddCoin(8000);
				break;

			case COIN_BOX:
				ScoreManager.init.AddCoin(14000);
				break;
		}

		return PurchaseProcessingResult.Complete;
	}

	public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason) {
		Debug.LogError($"PurchaseFailed : {product.definition.id}, {failureReason}");
	}

	public void Purchase(string productId) {
		if (!isInit) return;

		var product = storeController.products.WithID(productId);

		if(product != null && product.availableToPurchase) {
			Debug.Log($"productID : {product.definition.id}");
			storeController.InitiatePurchase(product);
		} else {
			Debug.Log($"not productId {productId}");
		}
	}

	public void RestorePurchase() {
		if (!isInit) return;
		if(Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer) {
			Debug.Log("restorePurchase");

			var appleExt = extensionProvider.GetExtension<IAppleExtensions>();
			appleExt.RestoreTransactions(
				result => Debug.Log($"restorePurchase result - {result}"));
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
