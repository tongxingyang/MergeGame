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
	private static readonly int EVALUTION_MAX_COUNT = 8;
	private static readonly int MAX_ITEM_ONEGAME = 2;

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
	private int _currEvalutionCount = 0;
	public int currEvalutionCount {
		get {
			return _currEvalutionCount;
		}
		set {
			if (value == EVALUTION_MAX_COUNT) {
				UIManager.init.evalutionMassage.SetActive(true);
			}
			_currEvalutionCount = value;
		}
	}

	private int _currAdsCount = 0;
	public int currAdsCount {
		get {
			return _currAdsCount;
		}
		set {
			if (value >= ADS_MAX_COUNT) {
				_currAdsCount = 0;
				AdsManager.init.ShowInterstitialAd();
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
		get {
			if (_rankupItemCount.text.Equals("ADS"))
				return 0;
			else if (_rankupItemCount.text.Equals(""))
				return -1;
			else
				return int.Parse(_rankupItemCount.text);
		}
		set {
			UIManager.init.rankItemBtn.enabled = true;
			if (value == 0)
				_rankupItemCount.text = "ADS";
			else if (value < 0) {
				_rankupItemCount.text = "";
				UIManager.init.rankItemBtn.enabled = false;
			} else
				_rankupItemCount.text = value.ToString();
		}
	}
	public int destroyItemCount {
		get {
			if (_destroyItemCount.text.Equals("ADS"))
				return 0;
			else if (_destroyItemCount.text.Equals(""))
				return -1;
			else
				return int.Parse(_destroyItemCount.text);
		}
		set {
			UIManager.init.destroyItemBtn.enabled = true;
			if (value == 0)
				_destroyItemCount.text = "ADS";
			else if (value < 0) {
				_destroyItemCount.text = "";
				UIManager.init.destroyItemBtn.enabled = false;
			} else
				_destroyItemCount.text = value.ToString();
		}
	}

	public void setSaveBestScore() {
		finalBestScore = bestScore;
		GameManager.init.ScoreFirebaseSync(finalBestScore);
	}

	public void InitScore() {
		bestScore = finalBestScore;
		currScore = 0;
		currAdsCount++;
		currEvalutionCount++;
	}

	public void AddScore(int type, ObjectManager.MergeLevel mergeLevel = ObjectManager.MergeLevel.one) {

		currScore += (type * (int)mergeLevel * BASE_SCORE);
		if (bestScore < currScore)
			bestScore = currScore;
	}

	public void AddCoin(int num) {
		coin += num;
		GameManager.init.CoinFirebaseSync(coin);
		UIManager.init.PlayAddCoinSound();
    }

	private int totalRankUpItem;
	private int totalDestroyItem;

	public void SetGameStart() {
		if (rankItemCount < 0) rankItemCount = 0;
		if (destroyItemCount < 0) destroyItemCount = 0;
		totalRankUpItem = rankItemCount;
		totalDestroyItem = destroyItemCount;

		if (totalRankUpItem > MAX_ITEM_ONEGAME) rankItemCount = MAX_ITEM_ONEGAME;
		if (totalDestroyItem > MAX_ITEM_ONEGAME) destroyItemCount = MAX_ITEM_ONEGAME;
    }

	public void SetGameOver() {
		if (totalRankUpItem == 1) totalRankUpItem++;
		if (totalDestroyItem == 1) totalDestroyItem++;

		rankItemCount = totalRankUpItem - (MAX_ITEM_ONEGAME - rankItemCount);
		destroyItemCount = totalDestroyItem - (MAX_ITEM_ONEGAME - destroyItemCount);

		if (rankItemCount < 0) rankItemCount = 0;
		if (destroyItemCount < 0) destroyItemCount = 0;
	}
}