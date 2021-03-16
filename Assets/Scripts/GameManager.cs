﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
	private static readonly float GAME_OVER_DELAY = 2.3f;

	public static GameManager init = null;

	public GameObject premiumGround;

	private bool isEnterGame = false;

	private bool _isPremium;
	public bool isPremium {
        get { return _isPremium; }
		set {
			if (value) {
				_isPremium = true;
				BuyPremium();
			}
		}
	}

	public bool isDoubleCoin;

	private bool _isGameOver;
	public bool isGameOver {
		set {
			_isGameOver = value;

			Camera.main.GetComponent<CameraControl>().BGMFadeOut(value);
			MouseControl.init.GameOver(value);

			if (value) {
				StartCoroutine(nameof(GameOverDelay));
				ObjectManager.init.StopObj();
			} else {
				ObjectManager.init.InitObj();
				ScoreManager.init.InitScore();
			}

			MaxLine.init.SetColor(isGameOver);

			if (!value) {
				UIManager.init.setGameOverPanel(value);
				isEnterGame = false;
			}
		}
		get { return _isGameOver; }
	}

	private void Awake() {
		if (init == null) {
			init = this;
		} else if (init != this) {
			Destroy(this.gameObject);
		}
		DontDestroyOnLoad(this.gameObject);
	}

	private void Update() {

#if UNITY_ANDROID
		if (Input.GetKeyDown(KeyCode.Escape)) {
			Application.Quit();
		}
#endif

	}

	public void GameStart() {
		if (!isEnterGame) {
			isEnterGame = true;
			ScoreManager.init.SetGameStart();
			UIManager.init.IsGameStart(true);
		}
	}

	public void GameRestart() {
		AtHome();
		GameStart();
	}

	public void AtHome() {
		if (Time.timeScale < 1f) Time.timeScale = 1f;
		isGameOver = false;
		UIManager.init.IsGameStart(false);
		ScoreManager.init.SetGameOver();
		ObjectManager.init.StopAllCoroutines();
	}

	public void GameOver() {
		isGameOver = true;
	}

	IEnumerator GameOverDelay() {
		yield return new WaitForSeconds(GAME_OVER_DELAY);
		UIManager.init.setGameOverPanel(true);
	}

	public void isPauseGame(bool on) {
		UIManager.init.pausePanel.SetActive(on);
		Time.timeScale = on ? 0.0f : 1.0f;
	}

	private void OnApplicationQuit() {
		DataManager.init.Save();
	}

	public void BuyPremium() {
		AdsManager.init.DestroyBannerAd();
		premiumGround.SetActive(true);
	}
}