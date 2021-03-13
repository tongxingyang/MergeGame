using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour {
	private static readonly float MAX_LEVEL_PANEL_OFF_DELAY = 2.5f;
	private static readonly float DESTROY_MAX_OBJECT_DELAY = 1.0f;
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
	public GameObject menuPanel;
	public GameObject settingPanel;
	public GameObject lisensePanel;
	public GameObject pausePanel;
	public GameObject maxLevelEffectPanel;
	public GameObject pauseBtn;
	public GameObject rankingPanel;
	
	public AudioClip uiBtn;
	public AudioClip gameOver;
	public AudioClip maxlevel;
	public AudioClip effectSound;
	public AudioClip destroymaxlevel;

	public AudioSource audioSource;


	public void AddScore(int type, ObjectManager.MergeLevel mergeLevel = ObjectManager.MergeLevel.one) {
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
		menuPanel.SetActive(!_active);
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

	public void PlayEffectSouned() {
		PlayAudioClip(effectSound);
	}

	public void PlayDestroySound() {
		PlayAudioClip(destroymaxlevel);
	}

	public void OnSettingClick(bool isOn) {
		settingPanel.SetActive(isOn);
		MainUI.SetActive(!isOn);
		MouseControl.init.isTouchAction = !isOn;
		ObjectManager.init.objParent.SetActive(!isOn);

		PlayAudioClip(uiBtn);
	}
	public void OnRankingClick(bool isOn) {
		rankingPanel.SetActive(isOn);
		MainUI.SetActive(!isOn);
		MouseControl.init.isTouchAction = !isOn;
		ObjectManager.init.objParent.SetActive(!isOn);

		PlayAudioClip(uiBtn);
	}

	public void OnMaxLevelPanel(GameObject obj) {
		MaxLine.init.gameObject.SetActive(false);
		PlayAudioClip(maxlevel);
		maxLevelEffectPanel.SetActive(true);
		maxLevelEffectPanel.transform.GetChild(1).GetComponent<Image>().sprite =
			obj.GetComponent<SpriteRenderer>().sprite;

		StartCoroutine(OnMaxLevelEffect(obj));
	}

	private IEnumerator OnMaxLevelEffect(GameObject obj) {
		yield return new WaitForSeconds(MAX_LEVEL_PANEL_OFF_DELAY);
		maxLevelEffectPanel.SetActive(false);

		yield return new WaitForSeconds(DESTROY_MAX_OBJECT_DELAY);
		PlayDestroySound();
		obj.GetComponent<CircleCollider2D>().enabled = false;
		obj.GetComponent<MainObject>().DestroyObj();
		MaxLine.init.gameObject.SetActive(true);
	}
}