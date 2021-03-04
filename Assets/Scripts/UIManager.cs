using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour {
	private static readonly int SHOP_OPEN = Animator.StringToHash("isShop");
	private static readonly int TO_SHOP_CONTANTS = Animator.StringToHash("shopVal");
	private static readonly int OPEN_UI_ANIM = Animator.StringToHash("isGameOver");

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
	public Animator animator;
	public GameObject gameOverPanel;
	public GameObject MainUI;
	public GameObject homePanel;
	public GameObject settingPanel;
	public GameObject lisensePanel;
	public GameObject pausePanel;
	public GameObject pauseBtn;

	public Scrollbar bgmVolume, effectVolume;

    private void Start() {
	}

	public void AddScore(float type, ObjectManager.MergeLevel mergeLevel = ObjectManager.MergeLevel.one) {
		ScoreManager.init.AddScore(type, mergeLevel);
    }

	public void setGameOverPanel(bool isOver) {
		if (isOver) {
			if (!GetComponent<AudioSource>().isPlaying && !gameOverPanel.activeSelf)
				GetComponent<AudioSource>().Play();

			OpenPanel(gameOverPanel);
			ScoreManager.init.setSaveBestScore();
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

	public void IsGameStart(bool _active) {
		pauseBtn.SetActive(_active);
		homePanel.SetActive(!_active);
		settingPanel.SetActive(false);
	}

	public void OpenPanel(GameObject gameObject) {
		gameObject.SetActive(true);
		gameObject.GetComponent<Animator>().SetBool(OPEN_UI_ANIM, true);
	}
}
