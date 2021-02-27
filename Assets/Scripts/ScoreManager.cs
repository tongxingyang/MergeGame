using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

class ScoreManager : MonoBehaviour {
	public static readonly float DROP_SCORE = 1f;
	public static readonly float MARGE_SCORE = 2f;
	private static readonly float BASE_SCORE = 13f;
	private static readonly int ADS_MAX_COUNT = 3;

	public TextMeshProUGUI currScoreText, bestScoreText, scoreViewText, adsCount;

	public static ScoreManager init = null;
	public float finalBestScore;

	private void Awake() {
		if (init == null) {
			init = this;
		} else if (init != this) {
			Destroy(this.gameObject);
		}
		DontDestroyOnLoad(this.gameObject);
	}

	private int _currAdsCount = 1;
	public int currAdsCount {
		get {
			return _currAdsCount;
		}
		set {
			if (value > ADS_MAX_COUNT) {
				currAdsCount = 0;
				AdsManager.init.UserChoseToWatchAd();
			} else _currAdsCount = value;
		}
	}

	private DataInfo.GameData gameData;
	private float _coin, _currScore, _bestScore;
	//public float coin;

	private void Start() {
		currScore = 0;
	}

	public float currScore {
		get { return _currScore; }
		set {
			_currScore = value;
			currScoreText.text = string.Format("{0}", value);
			scoreViewText.text = string.Format("{0}", value);
		}
	}
	public float bestScore {
		get { return _bestScore; }
		set {
			_bestScore = value;
			bestScoreText.text = string.Format("{0}", value);
		}
	}

	private void initScore() {
		//shopCoin.text = "0";
		//coinText.text = "0";
		currScoreText.text = "0";
		bestScoreText.text = "0";
	}

	public void setSaveBestScore() {
		finalBestScore = bestScore;
	}

	public void AddScore(float type, ObjectManager.MergeLevel mergeLevel = ObjectManager.MergeLevel.one) {

		currScore += (type * (int)mergeLevel * BASE_SCORE);
		if (bestScore < currScore)
			bestScore = currScore;
	}

	private void setGoldText() {
		//coinText.text = String.Format("{0}", coin);
		//shopCoin.text = String.Format("{0}", coin);
	}
}