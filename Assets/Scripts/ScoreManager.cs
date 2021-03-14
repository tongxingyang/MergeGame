using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

class ScoreManager : MonoBehaviour {
	public static readonly int DROP_SCORE = 1;
	public static readonly int MARGE_SCORE = 2;
	private static readonly int BASE_SCORE = 13;
	private static readonly int ADS_MAX_COUNT = 3;

	public TextMeshProUGUI currScoreText, bestScoreText, scoreViewText, adsCount, coinText, shopCoinText;
	public TextMeshProUGUI _rankupItemCount, _destroyItemCount;


	public static ScoreManager init = null;
	public int finalBestScore;

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
				currAdsCount = 2;
				AdsManager.init.ShowRewardedInterstitialAd();
			} else _currAdsCount = value;
		}
	}

	private int _coin, _currScore, _bestScore;

	//public float coin;
	private void Start() {
		currScore = 0;
		coin = 0;
		destroyItemCount = 1;
		rankItemCount = 1;
	}

	public int coin {
		get { return _coin; }
        set {
			_coin = value;
			coinText.text = string.Format("{0}", value);
			shopCoinText.text = string.Format("{0}", value);
		}
    }

	public int currScore {
		get { return _currScore; }
		set {
			_currScore = value;
			currScoreText.text = string.Format("{0}", value);
			scoreViewText.text = string.Format("{0}", value);
		}
	}
	public int bestScore {
		get { return _bestScore; }
		set {
			_bestScore = value;
			bestScoreText.text = string.Format("{0}", value);
		}
	}

	public int rankItemCount {
		get { return int.Parse(_rankupItemCount.text); }
		set { _rankupItemCount.text = value.ToString(); }
	}
	public int destroyItemCount {
		get { return int.Parse(_destroyItemCount.text); }
		set { _destroyItemCount.text = value.ToString(); }
	}

	public void setSaveBestScore() {
		finalBestScore = bestScore;
	}

	public void InitScore() {
		bestScore = finalBestScore;
		currScore = 0;
		currAdsCount++;
	}

	public void AddScore(int type, ObjectManager.MergeLevel mergeLevel = ObjectManager.MergeLevel.one) {

		currScore += (type * (int)mergeLevel * BASE_SCORE);
		if (bestScore < currScore)
			bestScore = currScore;
	}

	public void AddCoin(int num) {
		coin += num;
    }
}