using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour {
	private static readonly int SHOP_OPEN = Animator.StringToHash("isShop");
	private static readonly int TO_SHOP_CONTANTS = Animator.StringToHash("shopVal");
	private static readonly int IS_GAME_OVER_PANEL = Animator.StringToHash("isGameOver");

	public static UIManager init = null;
	private void Awake() {
		if (init == null) {
			init = this;
		} else if (init != this) {
			Destroy(this.gameObject);
		}
		DontDestroyOnLoad(this.gameObject);
	}

	//public TextMeshProUGUI coin, shopCoin;
	public TextMeshProUGUI currScore, bestScore;
	public Animator animator;
	public GameObject gameOverPanel;
	public GameObject MainUI;
	public GameObject shopIcon;
	public GameObject settingIcon;

	private ScoreManager scoreManager;

    private void Start() {
		scoreManager = new ScoreManager(currScore, bestScore);
	}

	public void AddScore(float type, ObjectManager.MergeLevel mergeLevel = ObjectManager.MergeLevel.one) {
		scoreManager.AddScore(type, mergeLevel);
    }

	public void setGameOverPanel(bool isOver) {
		if (isOver) {
			gameOverPanel.SetActive(true);
			gameOverPanel.GetComponent<Animator>().SetBool(IS_GAME_OVER_PANEL, true);
			GetComponent<AudioSource>().Play();
			//gameOverPanel.GetComponentInChildren<CircleProgressBar>().StartProgress();
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

	
}
