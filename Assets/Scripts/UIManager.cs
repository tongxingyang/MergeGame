using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public static UIManager init = null;
	private void Awake() {
		if (init == null) {
			init = this;
		} else if (init != this) {
			Destroy(this.gameObject);
		}
		DontDestroyOnLoad(this.gameObject);
	}

	public Text coin, currScore, bestScore;
	public GameObject gameOverPanel;

	private ScoreManager scoreManager;

    private void Start() {
		scoreManager = new ScoreManager(coin, currScore, bestScore);
	}

	public void AddScore(float type, ObjectManager.MergeLevel mergeLevel = ObjectManager.MergeLevel.one) {
		scoreManager.AddScore(type, mergeLevel);
    }

	public void setGameOverPanel() {
		gameOverPanel.SetActive(true);
		gameOverPanel.GetComponentInChildren<CircleProgressBar>().StartProgress();
    }
}
