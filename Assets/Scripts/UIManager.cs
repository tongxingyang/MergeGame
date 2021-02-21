﻿using System;
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
	public GameObject canvas;
	public GameObject gameOverPanel;
	public GameObject MainUI;
	public GameObject shopIcon;
	public GameObject settingIcon;

	private ScoreManager scoreManager;
	private Animator animator;

    private void Start() {
		scoreManager = new ScoreManager(coin, shopCoin, currScore, bestScore);
		animator = canvas.GetComponent<Animator>();
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
	}

	public void ToBallAsShop() {
		animator.SetInteger(TO_SHOP_CONTANTS, 1);
	}

	public void ShopActiveFalse() {
		settingIcon.SetActive(false);
		shopIcon.SetActive(false);
	}
}
