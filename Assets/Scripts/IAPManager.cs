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
		Debug.Log("IAP??????");
		storeController = controller;
		extensionProvider = extension;

		if (HadPurchased(PREMIUM)) {
			GameManager.init.isPremium = true;
		}
	}

	public void OnInitializeFailed(InitializationFailureReason error) {
		Debug.LogError($"IAP?????? ???? {error}");
	}

	//???? ???? ???? ????
	public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent) {
		Debug.Log($"????????{purchaseEvent.purchasedProduct.definition.id}");

		if(purchaseEvent.purchasedProduct.definition.id == PREMIUM) {
			GameManager.init.isPremium = true;
		}

		return PurchaseProcessingResult.Complete;
	}

	public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason) {
		Debug.LogError($"???? {product.definition.id}, {failureReason}");
	}

	public void Purchase(string productId) {
		if (!isInit) return;

		var product = storeController.products.WithID(productId);

		if(product != null && product.availableToPurchase) {
			Debug.Log($"???????? = {product.definition.id}");
			storeController.InitiatePurchase(product);
		} else {
			Debug.Log($"???? ???? ???? {productId}");
		}
	}

	public void RestorePurchase() {
		if (!isInit) return;
		if(Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer) {
			Debug.Log("???? ????");

			var appleExt = extensionProvider.GetExtension<IAppleExtensions>();
			appleExt.RestoreTransactions(
				result => Debug.Log($"???? ???? ???? ???? - {result}"));
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
