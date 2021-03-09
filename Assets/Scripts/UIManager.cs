using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour {
	private static readonly int OPEN_UI_ANIM = Animator.StringToHash("isGameOver");
	private static readonly int SHOP_VAL = Animator.StringToHash("toMenu");

	public static UIManager init = null;
	private void Awake() {
		if (init == null) {
			init = this;
			audioSource = GetComponent<AudioSource>();
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
	

	public AudioClip uiBtn;
	public AudioClip gameOver;

	public AudioSource audioSource;


	public void AddScore(float type, ObjectManager.MergeLevel mergeLevel = ObjectManager.MergeLevel.one) {
		ScoreManager.init.AddScore(type, mergeLevel);
    }

	public void setGameOverPanel(bool isOver) {
		if (isOver) {
			if (!audioSource.isPlaying && !gameOverPanel.activeSelf)
				PlayAudioClip(gameOver);

			OpenPanel(gameOverPanel);
			ScoreManager.init.setSaveBestScore();
			//gameOverPanel.GetComponentInChildren<CircleProgressBar>().StartProgress();
		} else {
			gameOverPanel.SetActive(false);
		}
    }

	public void IsGameStart(bool _active) {
		pauseBtn.SetActive(_active);
		homePanel.SetActive(!_active);
		settingPanel.SetActive(false);
	}

	public void OpenPanel(GameObject gameObject) {
		gameObject.SetActive(true);
		animator.SetBool(OPEN_UI_ANIM, true);
	}

	public void PlayShopAnim(int val) {
		animator.SetInteger(SHOP_VAL, val);
	}

	public void PlayUIBtnSound() {
		PlayAudioClip(uiBtn);
	}

	private void PlayAudioClip(AudioClip audioClip) {
		audioSource.clip = audioClip;
		audioSource.Play();
	}
}