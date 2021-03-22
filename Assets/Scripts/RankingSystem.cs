using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.Database;
using System.Text.RegularExpressions;

public class RankingSystem : MonoBehaviour {
	static readonly private string TITLE = "RANK";
	static readonly private string FLAG = "flag";
	static readonly private string NAME = "name";
	static readonly private string SCORE = "score";
	static readonly private int LIMIT_USER_DATA = 100;

	public Transform rankList;
	public GameObject rankPrefab;
	public Sprite[] flags;

	public TextMeshProUGUI userRank;
	public TextMeshProUGUI bestScore, rankScore;
	public TMP_Dropdown userFlag;
	public TMP_InputField userName;

	public static RankingSystem init = null;

	private int rankingCount = 0;
	private int myRankingCount = 1;

	private void Awake() {
		if (init == null) {
			init = this;
		}
	}

	private User currUserData;

	private void OnEnable() {
		SetFlagOption();
		bestScore.text = ScoreManager.init.bestScoreText.text;

		string name = DataManager.init.tempData.rankName ?? "";

		if (!name.Equals("")) {
			currUserData = new User(
				SystemInfo.deviceModel,
				DataManager.init.tempData.rankFlag,
				DataManager.init.tempData.rankName,
				DataManager.init.tempData.rankScore,
				ScoreManager.init.coin);
		} else {
			currUserData = new User(
				SystemInfo.deviceModel,
				DataManager.init.gameData.rankFlag,
				DataManager.init.gameData.rankName,
				DataManager.init.gameData.rankScore,
				ScoreManager.init.coin);
		}

		SetUserRankingData("-", currUserData.flag, currUserData.name, currUserData.score);

		LoadUserRanking();
	}

	private void SetFlagOption() {
		var flagList = new List<Sprite>();
		flagList.AddRange(flags);
		userFlag.AddOptions(flagList);
	}

	private void SetUserRankingData(string rank = "-", int flag = 0, string name = "", int score = 0) {
		userRank.text = rank;
		userFlag.value = flag;
		SetUserNameTextField(name);
		rankScore.text = score.ToString();

		DataManager.init.tempData.rankFlag = flag;
		DataManager.init.tempData.rankName = name;
		DataManager.init.tempData.rankScore = score;
	}

    private void SetUserNameTextField(string name) {
		userName.text = name;
		userName.characterLimit = 10;
		userName.onValueChanged.AddListener(
			(word) => userName.text = Regex.Replace(word, @"[^0-9a-zA-Z¤¡-¤¾°¡-ÆR??-?R]", "")
			);
	}

	public void UploadUserData() {
		UploadUserDataWithFirebase(userFlag.value, userName.text);
	}

	private void UploadUserDataWithFirebase(int flag, string userName) {
		User user = new User(SystemInfo.deviceModel, flag, userName, int.Parse(ScoreManager.init.bestScoreText.text), ScoreManager.init.coin);
		currUserData = user;
		SetUserRankingData("-", currUserData.flag, currUserData.name, currUserData.score);

		string json = JsonUtility.ToJson(user);

		GameManager.init.databaseReference.Child(TITLE).Child(GameManager.init.key).SetRawJsonValueAsync(json).ContinueWith(task => {
			if (task.IsCompleted) {
				LoadUserRanking();
			}
		});
	}

	private void LoadUserRanking() {
		DeleteCurrRankingObject();
		rankingCount = LIMIT_USER_DATA;
		FirebaseDatabase.DefaultInstance.GetReference(TITLE)
			.OrderByChild(SCORE)
			.LimitToLast(LIMIT_USER_DATA)
			.ChildAdded += HandleChildAddedRanking;

		LoadMyData();
	}

	private void LoadMyData() {
		myRankingCount = 1;
		FirebaseDatabase.DefaultInstance.GetReference(TITLE)
			.OrderByChild(SCORE)
			.StartAt(currUserData.score)
			.ChildAdded += HandleChildAddedUserData;
	}

	private void HandleChildAddedRanking(object sender, ChildChangedEventArgs arge) {
		if (rankingCount <= 0) return;

		if (arge.DatabaseError != null) {
			Debug.LogError(arge.DatabaseError.Message);
			return;
		};

		IDictionary rank = (IDictionary)arge.Snapshot.Value;

		GameObject tempRank = Instantiate(rankPrefab, rankList);
		tempRank.transform.SetAsFirstSibling();
		tempRank.GetComponent<RankUserData>().SetRankData(rankingCount--,
			flags[int.Parse(rank[FLAG].ToString())], rank[NAME], rank[SCORE]);
		Debug.Log("AllRank");
	}

	private void HandleChildAddedUserData(object sender, ChildChangedEventArgs arge) {
		if (arge.DatabaseError != null) {
			Debug.LogError(arge.DatabaseError.Message);
			return;
		};

		userRank.text = myRankingCount++.ToString();
	}

	private void DeleteCurrRankingObject() {
		foreach(Transform iter in rankList.GetComponentsInChildren<Transform>()) {
			if(iter.gameObject != rankList.gameObject) {
				Destroy(iter.gameObject);
			}
        }
    }
}