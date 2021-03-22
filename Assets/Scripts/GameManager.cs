using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
	static readonly private string TITLE = "RANK";
	static readonly private string COIN = "coin";
	static readonly private string SCORE = "score";
	private static readonly float GAME_OVER_DELAY = 2.3f;

	public DatabaseReference databaseReference;

	public string key = "";
	public static GameManager init = null;

	public GameObject premiumGround;

	public bool isEnterGame = false;

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

		databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
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

    private void OnApplicationPause(bool pause) {
		if (pause) {
			DataManager.init.Save();
		}
	}

    public void BuyPremium() {
		AdsManager.init.DestroyBannerAd();
		premiumGround.SetActive(true);
	}

	public void QuitGame() {
		Application.Quit();
	}

	private bool isKey() {
		return null != key && !key.Equals("");
	}

	public void SetFirebaseData(User user) {
		if (!key.Equals("")) {
			string json = JsonUtility.ToJson(user);
			databaseReference.Child(TITLE).Child(key).SetRawJsonValueAsync(json);
		}
	}

	public void CoinFirebaseSync(int num) {
		databaseReference.Child(TITLE).Child(key).Child(COIN).SetValueAsync(num);
	}

	public void ScoreFirebaseSync(int num) {
		databaseReference.Child(TITLE).Child(key).Child(SCORE).SetValueAsync(num);
	}
}