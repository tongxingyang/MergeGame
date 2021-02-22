using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
	private static readonly int SHOP_OPEN = Animator.StringToHash("isShop");
	private static readonly int TO_SHOP_CONTANTS = Animator.StringToHash("shopVal");

	public static UIManager init = null;
	private void Awake() {
		if (init == null) {
			init = this;
		} else if (init != this) {
			Destroy(this.gameObject);
		}
		DontDestroyOnLoad(this.gameObject);
	}

	public Text coin, shopCoin, currScore, bestScore;
	public Animator animator;
	public GameObject gameOverPanel;
	public GameObject MainUI;
	public GameObject shopIcon;
	public GameObject settingIcon;
	public GameObject shopBallImagesObjContainer;

	private ScoreManager scoreManager;
	private Image[] shopBallImages;

    private void Start() {
		scoreManager = new ScoreManager(coin, shopCoin, currScore, bestScore);
		shopBallImages = shopBallImagesObjContainer.GetComponentsInChildren<Image>();
	}

	public void AddScore(float type, ObjectManager.MergeLevel mergeLevel = ObjectManager.MergeLevel.one) {
		scoreManager.AddScore(type, mergeLevel);
    }

	public void setGameOverPanel(bool isOver) {
		if (isOver) {
			gameOverPanel.SetActive(true);
			gameOverPanel.GetComponentInChildren<CircleProgressBar>().StartProgress();
		} else {
			gameOverPanel.SetActive(false);
		}
    }

	public void OpenShop() {
		ObjectManager.init.objParent.SetActive(false);
		animator.SetBool(SHOP_OPEN, true);
	}

	public void CloseShop() {
		ObjectManager.init.objParent.SetActive(true);
		animator.SetBool(SHOP_OPEN, false);
		animator.SetInteger(TO_SHOP_CONTANTS, 0);
	}

	public void ToContentsAnimation(int value) {
		animator.SetInteger(TO_SHOP_CONTANTS, value);
	}

	public void ShopActiveFalse() {
		settingIcon.SetActive(false);
		shopIcon.SetActive(false);
	}

	public void ChoceObjItem(int num) {
		ChangeShopObjImage(num);
	}

	private void ChangeShopObjImage(int num) {
		for(int i = 1; i < shopBallImages.Length; ++i) {
			String path = "obj/objects" + num + "_" + (i - 1);
			Sprite sprite = Resources.Load<Sprite>(path);
			shopBallImages[i].sprite = sprite;
		}
	}
}
