using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Database;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class DataManager : MonoBehaviour {
	static readonly private string TITLE = "RANK";
	static readonly private string COIN = "coin";
	static readonly private string SCORE = "score";

	public static DataManager init = null;
	private void Awake() {
		if (init == null) {
			init = this;
		}
		else if (init != this) {
			Destroy(this.gameObject);
		}
		DontDestroyOnLoad(this.gameObject);
	}

	private string dataPath;
	public DataInfo.GameData gameData;
	public DataInfo.GameData tempData;

	private void Start() {

		dataPath = Application.persistentDataPath + "/gameData.dat";
		tempData = new DataInfo.GameData();
		Load();
	}

	public void Save() {
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		FileStream file = File.Create(dataPath);

		gameData = new DataInfo.GameData();
		gameData = tempData;

		gameData.evalutionCount = ScoreManager.init.currEvalutionCount;
		gameData.adsCount = ScoreManager.init.currAdsCount;
		gameData.bestScore = ScoreManager.init.finalBestScore;
		gameData.coin = ScoreManager.init.coin;
		gameData.rankupItemCount = ScoreManager.init.rankItemCount <= 0 ? 0 : ScoreManager.init.rankItemCount;
		gameData.destroyItemCount = ScoreManager.init.destroyItemCount <= 0 ? 0 : ScoreManager.init.destroyItemCount;

		gameData.styleNum = ObjectManager.init.currStyleNum;
		gameData.wallpaperNum = ObjectManager.init.currBackgroundNum;
		gameData.styleProducts = new bool[ShoppingManager.init.style.Length];
		gameData.wallpaperProducts = new bool[ShoppingManager.init.wallpaper.Length];

		for (int i = 0; i < gameData.styleProducts.Length; ++i) {
			gameData.styleProducts[i] = ShoppingManager.init.style[i].isBuy;
			gameData.wallpaperProducts[i] = ShoppingManager.init.wallpaper[i].isBuy;
		}

		gameData.isPremium = GameManager.init.isPremium;
		gameData.isDoubleCoin = GameManager.init.isDoubleCoin;

		gameData.isBGMVolum = CameraControl.init.audioSource.mute;
		gameData.isEffectVolum = UIManager.init.audioSource.mute;

		gameData.initTimer = UIManager.init.initTime;

		gameData.lastLanguageFileName = LocalizationManager.init.lastLanguageFileName;
		gameData.key = GameManager.init.key;

		binaryFormatter.Serialize(file, gameData);

		file.Close();

		SetFirebaseData();
	}

	public void Load() {

		if (File.Exists(dataPath)) {

			BinaryFormatter binaryFormatter = new BinaryFormatter();
			FileStream file = File.OpenRead(dataPath);

			if (file.Length <= 0) return;

			gameData = (DataInfo.GameData)binaryFormatter.Deserialize(file);

			ScoreManager.init.finalBestScore = gameData.bestScore;
			ScoreManager.init.bestScore = gameData.bestScore;
			ScoreManager.init.currEvalutionCount = gameData.evalutionCount;
			ScoreManager.init.currAdsCount = gameData.adsCount;
			ScoreManager.init.coin = gameData.coin;
			ScoreManager.init.rankItemCount = gameData.rankupItemCount;
			ScoreManager.init.destroyItemCount = gameData.destroyItemCount;

			for (int i = 0; i < ShoppingManager.init.style.Length; ++i) {
				ShoppingManager.init.style[i].isBuy = gameData.styleProducts[i];
				ShoppingManager.init.wallpaper[i].isBuy = gameData.wallpaperProducts[i];
			}

			ShoppingManager.init.ApplyItem(ShoppingManager.init.style[gameData.styleNum]);
			ShoppingManager.init.ApplyItem(ShoppingManager.init.wallpaper[gameData.wallpaperNum]);

			GameManager.init.isPremium = gameData.isPremium;
			GameManager.init.isDoubleCoin = gameData.isDoubleCoin;

			SettingManager.init.BGMOn(gameData.isBGMVolum);
			SettingManager.init.EffectOn(gameData.isEffectVolum);

			UIManager.init.initTime = gameData.initTimer;
			LocalizationManager.init.lastLanguageFileName = gameData.lastLanguageFileName ?? "";

			if (gameData.key.Equals("")) {
				InitFirebaseData();
			}
			else {
				GameManager.init.key = gameData.key;
				LoadFirebaseDate();
			}

			file.Close();

		}
		else {
			LocalizationManager.init.lastLanguageFileName = "";
			ScoreManager.init.finalBestScore = 0;
			ScoreManager.init.bestScore = 0;
			ScoreManager.init.currAdsCount = 0;
			ScoreManager.init.coin = 0;

			InitFirebaseData();
		}
	}

	private void LoadFirebaseDate() {
		FirebaseDatabase.DefaultInstance.GetReference(TITLE)
			.OrderByKey()
			.EqualTo(gameData.key)
			.ChildAdded += HandleChildAddedUserData;
	}
	private void HandleChildAddedUserData(object sender, ChildChangedEventArgs arge) {
		if (arge.DatabaseError != null) {
			Debug.LogError(arge.DatabaseError.Message);
			return;
		};

		IDictionary data = (IDictionary)arge.Snapshot.Value;
		ScoreManager.init.coin = int.Parse(data[COIN].ToString());
		ScoreManager.init.bestScore = int.Parse(data[SCORE].ToString());
		ScoreManager.init.finalBestScore = int.Parse(data[SCORE].ToString());
	}

	public void InitFirebaseData() {
		GameManager.init.key = GameManager.init.databaseReference.Child(TITLE).Push().Key;
		User user = new User(SystemInfo.deviceModel, 0, "", int.Parse(ScoreManager.init.bestScoreText.text), gameData.coin);
		string json = JsonUtility.ToJson(user);
		GameManager.init.databaseReference.Child(TITLE).Child(GameManager.init.key).SetRawJsonValueAsync(json);
	}

	public void SetFirebaseData() {
		User user = new User(
			SystemInfo.deviceModel,
			gameData.rankFlag,
			gameData.rankName,
			gameData.bestScore,
			gameData.coin
			);
		GameManager.init.SetFirebaseData(user);
	}
}