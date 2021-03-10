using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class DataManager : MonoBehaviour {

	public static DataManager init = null;
	private void Awake() {
		dataPath = Application.persistentDataPath + "/gameData.dat";

		if (init == null) {
			init = this;
		} else if (init != this) {
			Destroy(this.gameObject);
		}
		DontDestroyOnLoad(this.gameObject);
	}

	private string dataPath;
	public DataInfo.GameData gameData;

	private void Start() {
		Load();
	}

	public void Save() {
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		FileStream file = File.Create(dataPath);

		gameData = new DataInfo.GameData();

		gameData.adsCount = ScoreManager.init.currAdsCount;
		gameData.bestScore = ScoreManager.init.finalBestScore;
		gameData.coin = ScoreManager.init.coin;

		gameData.styleNum = ObjectManager.init.currStyleNum;
		gameData.wallpaperNum = ObjectManager.init.currBackgroundNum;
		gameData.styleProducts = new bool[ShoppingManager.init.style.Length];
		gameData.wallpaperProducts = new bool[ShoppingManager.init.wallpaper.Length];
		for (int i = 0; i < gameData.styleProducts.Length; ++i) {
			gameData.styleProducts[i] = ShoppingManager.init.style[i].isBuy;
			gameData.wallpaperProducts[i] = ShoppingManager.init.wallpaper[i].isBuy;
		}

		gameData.isPremium = GameManager.init.isPremium;

		gameData.isBGMVolum = CameraControl.init.audioSource.mute;
		gameData.isEffectVolum = UIManager.init.audioSource.mute;


		binaryFormatter.Serialize(file, gameData);
		file.Close();
	}

	public void Load() {
		if (File.Exists(dataPath)) {
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			FileStream file = File.OpenRead(dataPath);

			if (file.Length <= 0) return;

			DataInfo.GameData gameData = (DataInfo.GameData)binaryFormatter.Deserialize(file);

			ScoreManager.init.finalBestScore = gameData.bestScore;
			ScoreManager.init.bestScore = gameData.bestScore;
			ScoreManager.init.currAdsCount = gameData.adsCount;
			ScoreManager.init.coin = gameData.coin;

			//Ranking--------------
			//RankingSystem.init.key = gameData.key;
			//---------------------

			for (int i = 0; i < ShoppingManager.init.style.Length; ++i) {
				ShoppingManager.init.style[i].isBuy = gameData.styleProducts[i];
				ShoppingManager.init.wallpaper[i].isBuy = gameData.wallpaperProducts[i];
			}
			ShoppingManager.init.ApplyItem(ShoppingManager.init.style[gameData.styleNum]);
			ShoppingManager.init.ApplyItem(ShoppingManager.init.wallpaper[gameData.wallpaperNum]);

			GameManager.init.isPremium = gameData.isPremium;

			SettingManager.init.BGMOn(gameData.isBGMVolum);
			SettingManager.init.EffectOn(gameData.isEffectVolum);

			file.Close();

		} else {
			ScoreManager.init.finalBestScore = 0;
			ScoreManager.init.bestScore = 0;
			ScoreManager.init.currAdsCount = 0;
			ScoreManager.init.coin = 0;
		}
	}
}