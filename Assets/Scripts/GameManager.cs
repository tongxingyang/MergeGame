using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
	private static readonly float GAME_OVER_DELAY = 3.0f;
	private static readonly string PREMIUM = "premium";

	public static GameManager init = null;

	public GameObject premiumGround;

	private bool _isGameOver;
	public bool isGameOver {
		set {
			_isGameOver = value;

			if (value) {
				ObjectManager.init.StopObj();
			} else {
				ObjectManager.init.Restart();
			}

			MouseControl.init.GameOver(value);
			MaxLine.init.setColor(isGameOver);
			UIManager.init.setGameOverPanel(value);
			Camera.main.GetComponent<CameraControl>().isGameOver = value;

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

	private void Start() {
		if (IAPManager.init.HadPurchased(PREMIUM)) {
			BuyPremium();
		}
	}

	public void GameOver(GameObject overTerget) {
		overTerget.GetComponent<MainObject>().ObjStateWhenGameOver();
		StartCoroutine(nameof(GameOverDelay));
	}

	public void GameStart() {
		UIManager.init.MainUIActive(false);
	}

	public void GameRestart() {
		isGameOver = false;
		ScoreManager.init.currAdsCount++;
		ScoreManager.init.currScore = 0;
	}

	public void AtHome() {
		isGameOver = false;
		ScoreManager.init.currAdsCount++;
		UIManager.init.MainUIActive(true);
		ScoreManager.init.currScore = 0;

		ObjectManager.init.StopAllCoroutines();

		if (Time.timeScale < 1.0f)
			Time.timeScale = 1.0f;
	}

	public void isPauseGame(bool on) {
		UIManager.init.pausePanel.SetActive(on);
		Time.timeScale = on ? 0.0f : 1.0f;
	}

	IEnumerator GameOverDelay() {
		Camera.main.GetComponent<CameraControl>().isGameOver = true;
		MouseControl.init.GameOver(true);

		yield return new WaitForSeconds(GAME_OVER_DELAY);
		isGameOver = true;
	}

	private void OnApplicationQuit() {
		DataManager.init.Save();
	}

	public void BuyPremium() {
		AdsManager.init.closeAds();
		premiumGround.SetActive(true);
	}
}