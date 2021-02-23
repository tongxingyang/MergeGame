using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
	private static readonly float GAME_OVER_DELAY = 3.0f;

	public static GameManager init = null;

	private bool _isGameOver;
	public bool isGameOver {
		set {
			if (value) {
				ObjectManager.init.StopObj();
			} else {
				ObjectManager.init.Restart();
			}

			MouseControl.init.GameOver(value);
			MaxLine.init.gameObject.SetActive(!value);
			UIManager.init.setGameOverPanel(value);
			Camera.main.GetComponent<CameraControl>().isGameOver = value;

			_isGameOver = value;
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

    public void GameOver(GameObject overTerget) {
		overTerget.GetComponent<MainObject>().ObjStateWhenGameOver();
		StartCoroutine(nameof(GameOverDelay));
    }

	public void GameStart() {
		UIManager.init.ShopActiveFalse();
	}

	public void GameReset() {
		isGameOver = false;
	}

	public void AtHome() {
		isGameOver = false;
		UIManager.init.MainUI.SetActive(true);
    }

	IEnumerator GameOverDelay() {
		Camera.main.GetComponent<CameraControl>().isGameOver = true;
		MouseControl.init.GameOver(true);

		yield return new WaitForSeconds(GAME_OVER_DELAY);
		isGameOver = true;
	}
}