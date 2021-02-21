using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

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
		}
	}

	private void Awake() {
		if (init == null) {
			init = this;
		} else if (init != this) {
			Destroy(this.gameObject);
		}
		DontDestroyOnLoad(this.gameObject);
	}

	public void GameOver() {
		isGameOver = true;
    }

	public void GameStart() {
		UIManager.init.ShopActiveFalse();
	}

	public void GameReset() {
		isGameOver = false;
	}
}