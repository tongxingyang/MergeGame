using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

	private void Start() {
		Load();
	}

	public void Save() {
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		FileStream file = File.Create(dataPath);

		DataInfo.GameData gameData = new DataInfo.GameData();

		gameData.adsCount = ScoreManager.init.currAdsCount;
		gameData.bestScore = ScoreManager.init.finalBestScore;

		gameData.bgmVolume = UIManager.init.bgmVolume.value;
		gameData.effectVolume = UIManager.init.effectVolume.value;

		gameData.isPremium = GameManager.init.isPremium;

		binaryFormatter.Serialize(file, gameData);
		file.Close();
	}

	public void Load() {
		if (File.Exists(dataPath)) {
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			FileStream file = File.OpenRead(dataPath);

			DataInfo.GameData gameData = (DataInfo.GameData)binaryFormatter.Deserialize(file);
			file.Close();

			ScoreManager.init.finalBestScore = gameData.bestScore;
			ScoreManager.init.bestScore = gameData.bestScore;
			ScoreManager.init.currAdsCount = gameData.adsCount;

			UIManager.init.bgmVolume.value = gameData.bgmVolume;
			UIManager.init.effectVolume.value = gameData.effectVolume;

			GameManager.init.isPremium = gameData.isPremium;

		} else {
			ScoreManager.init.finalBestScore = 0;
			ScoreManager.init.bestScore = 0;
			ScoreManager.init.currAdsCount = 0;
		}
	}
}