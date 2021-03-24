using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour {
	private static readonly int COIN_ABOUT_SCORE = 55;
	private static readonly double ADD_COIN_ADS_DELAY = 5;
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
	public GameObject gameoverMessage;
	public GameObject gameOverPanel;
	public GameObject resultCoinPanel;
	public GameObject MainUI;
	public GameObject menuPanel;
	public GameObject settingPanel;
	public GameObject lisensePanel;
	public GameObject pausePanel;
	public GameObject maxLevelEffectPanel;
	public GameObject pauseBtn;
	public GameObject rankingPanel;
	public GameObject buyMessagePanel;
	public GameObject itemUnavailableMessage;
	public Button adsCoinAddBtn;
	public GameObject addCoinAdsTimerText;

	public GameObject rankUpItemPanelOnMain;
	public GameObject destroyItemPanelOnMain;
	public GameObject rankUpItemPanelOnGame;
	public GameObject destroyItemPanelOnGame;
	public GameObject evalutionMassage;

	public Button rankItemBtn;
	public Button destroyItemBtn;

	public GameObject rankUpItemPanel;
	public GameObject destroyItemPanel;

	public AudioClip uiBtn;
	public AudioClip gameOver;
	public AudioClip maxlevel;
	public AudioClip effectSound;
	public AudioClip destroymaxlevel;
	public AudioClip addCoin;

	public AudioSource audioSource;

	public TextMeshProUGUI errorLogPanel;

	private bool _isEnableCoinAds = true;
	public bool isEnableCoinAds {
		get { return _isEnableCoinAds; }
		set {
			if (value) {
				//adsCoinAddBtn.transform.GetChild(0).gameObject.SetActive(false);
				adsCoinAddBtn.enabled = true;
				adsCoinAddBtn.GetComponent<RawImage>().color = Color.white;
			} else {
				try {
				//	adsCoinAddBtn.transform.GetChild(0).gameObject.SetActive(true);
				} catch (NullReferenceException e) {
					UIManager.init.errorLogPanel.text += e.StackTrace;
				}
				adsCoinAddBtn.enabled = false;
				adsCoinAddBtn.GetComponent<RawImage>().color = new Color(1, 1, 1, 0.3f);
			}
			_isEnableCoinAds = value;
		}
	}

	[HideInInspector]
	private DateTime _initTime;
	public DateTime initTime {
		get { return _initTime; }
		set {
			_initTime = value;
			if ((value - DateTime.Now).TotalSeconds > 0) {
				_initTime = value;
				isEnableCoinAds = false;
			}
		}
	}
	private TimeSpan timer;

    private void Start() {
		rankItemBtn.onClick.AddListener(OpenRankUpItemPanelOnMain);
		destroyItemBtn.onClick.AddListener(OpenDestroyItemPanelOnMain);
	}

	private void Update() {
		if (!isEnableCoinAds) {
			timer = initTime - DateTime.Now;
			if (addCoinAdsTimerText.activeSelf)
				addCoinAdsTimerText.GetComponent<TextMeshProUGUI>().text = timer.Minutes + ":" + string.Format("{0:D2}", timer.Seconds);

			if (timer.TotalSeconds <= 0) {
				isEnableCoinAds = true;
			}
		}

		if (Input.GetKeyDown(KeyCode.Escape)) {
			if (GameManager.init.isEnterGame) {
				if (!menuPanel.activeSelf) {
					GameManager.init.isPauseGame(!pausePanel.activeSelf);
				}
			}
			else {
				if (MainUI.activeSelf) {
					gameoverMessage.SetActive(!gameoverMessage.activeSelf);
				}
			}
		}
	}

	public void AddScore(int type, ObjectManager.MergeLevel mergeLevel = ObjectManager.MergeLevel.one) {
		ScoreManager.init.AddScore(type, mergeLevel);
	}

	public void setGameOverPanel(bool isOver) {
		if (isOver) {
			if (!audioSource.isPlaying && !gameOverPanel.activeSelf)
				PlayAudioClip(gameOver);

			OpenPanel(gameOverPanel);
			StartCoroutine(nameof(OpenCoinResult));
			ScoreManager.init.setSaveBestScore();
			//gameOverPanel.GetComponentInChildren<CircleProgressBar>().StartProgress();
		} else {
			resultCoinPanel.SetActive(false);
			gameOverPanel.SetActive(false);
		}
	}

	public void IsGameStart(bool _active) {
		rankItemBtn.onClick.RemoveAllListeners();
		destroyItemBtn.onClick.RemoveAllListeners();

		if (_active) {
			rankItemBtn.onClick.AddListener(OpenRankUpItemPanelOnGame);
			destroyItemBtn.onClick.AddListener(OpenDestroyItemPanelOnGame);
		} else {
			rankItemBtn.onClick.AddListener(OpenRankUpItemPanelOnMain);
			destroyItemBtn.onClick.AddListener(OpenDestroyItemPanelOnMain);
		}

		pauseBtn.SetActive(_active);
		menuPanel.SetActive(!_active);
		settingPanel.SetActive(false);
	}

	public void OpenPanel(GameObject gameObject) {
		gameObject.SetActive(true);
		animator.SetBool(OPEN_UI_ANIM, true);
	}

	private IEnumerator OpenCoinResult() {
		int coin = GetCoinAboutScore();
		resultCoinPanel.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = coin.ToString();
		yield return new WaitForSeconds(1.7f);
		PlayAddCoinSound();
		ScoreManager.init.AddCoin(coin);
		resultCoinPanel.SetActive(true);
	}

	private int GetCoinAboutScore() {
		try {
			if (GameManager.init.isDoubleCoin)
				return (ScoreManager.init.currScore / COIN_ABOUT_SCORE) * 2;
			else
				return (ScoreManager.init.currScore / COIN_ABOUT_SCORE);
		} catch (Exception e) {
			Debug.LogError(e.StackTrace);
			return 0;
		}
	}

	public void PlayShopAnim(int val) {
		animator.SetInteger(SHOP_VAL, val);
	}

	public void PlayUIBtnSound() {
		PlayAudioClip(uiBtn);
	}

	private void PlayAudioClip(AudioClip audioClip) {
		if (audioSource.clip == maxlevel && audioSource.isPlaying) return;

		audioSource.clip = audioClip;
		audioSource.Play();
	}

	public void PlayEffectSouned() {
		PlayAudioClip(effectSound);
	}

	public void PlayDestroySound() {
		PlayAudioClip(destroymaxlevel);
	}

	public void PlayAddCoinSound() {
		PlayAudioClip(addCoin);
	}

	public void OnSettingClick(bool isOn) {
		settingPanel.SetActive(isOn);
		SettingManager.init.SetText();
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
		MaxLine.init.SetColor(false);
		ScoreManager.init.currScore += 5000;
	}

	public void SetCoinAdsTimer() {
		initTime = DateTime.Now.AddMinutes(ADD_COIN_ADS_DELAY);
		isEnableCoinAds = false;
	}

	//ads message panel
	public void OpenRankUpItemPanel() {
		rankUpItemPanel.SetActive(true);
	}

	public void OpenDestroyItemPanel() {
		destroyItemPanel.SetActive(true);
	}
	//-----------------

	private void OpenRankUpItemPanelOnMain() {
		rankUpItemPanelOnMain.SetActive(true);
	}

	private void OpenRankUpItemPanelOnGame() {
		rankUpItemPanelOnGame.SetActive(true);
		ObjectManager.init.TargetOfItemFadeOutOnRankUp();
	}

	private void OpenDestroyItemPanelOnMain() {
		destroyItemPanelOnMain.SetActive(true);
	}

	private void OpenDestroyItemPanelOnGame() {
		destroyItemPanelOnGame.SetActive(true);
		ObjectManager.init.TargetOfItemFadeOutOnDestroy();
	}

	public void SendToeMail() {
		string Emailadd = "ssunine21@gmail.com";
		string Emailname = EscapeURL("문의사항");

		Application.OpenURL("mailto:" + Emailadd + "?subject=" + Emailname);
	}

	private string EscapeURL(string url) {
		return WWW.EscapeURL(url).Replace("+", "%20");
	}

	public void GoToStore() {
#if UNITY_ANDROID
		Application.OpenURL("market://details?id=com.bognstudio.mergegame");

#elif UNITY_IPHONE
		Application.OpenURL("itms-apps://itunes.apple.com/app/id1556222154");

#else
		Application.OpenURL("https://play.google.com/store/apps/details?id=com.bognstudio.mergegame");

#endif
	}
}