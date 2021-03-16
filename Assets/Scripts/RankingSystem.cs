using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Firebase;
using Firebase.Database;
using System.Text.RegularExpressions;

public class RankingSystem : MonoBehaviour {
	static readonly private float UPLOAD_BOTTON_DELAY = 5f;
	static readonly private string TITLE = "RANK";
	static readonly private string FLAG = "flag";
	static readonly private string NAME = "name";
	static readonly private string SCORE = "score";
	static readonly private int LIMIT_USER_DATA = 5;

	public string key;
	public Transform rankList;
	public GameObject rankPrefab;
	public Sprite[] flags;

	public TextMeshProUGUI userRank;
	public TextMeshProUGUI bestScore, rankScore;
	public TMP_Dropdown userFlag;
	public TMP_InputField userName;

	public static RankingSystem init = null;

	private bool isCoolTime = false;
	private bool firstEntry = true;
	private int rankingCount = 0;
	private int myRankingCount = 1;

	DatabaseReference databaseReference;
	private void Awake() {
		if (init == null) {
			init = this;
		}
	}

	public class User {
		public int flag;
		public string name;
		public int score;

		public User(int flag, string name, int score) {
			this.flag = flag;
			this.name = name;
			this.score = score;
		}
	}

	private User currUserData;

	private void Start() {
		databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
	}

    private void OnEnable() {
		SetFlagOption();
		bestScore.text = ScoreManager.init.bestScoreText.text;
		key = DataManager.init.gameData.key ?? "";

		DataManager.init.tempData.key = key;

		isCoolTime = false;
		if (firstEntry) {
			currUserData = new User(
				DataManager.init.gameData.rankFlag,
				DataManager.init.gameData.rankName,
				DataManager.init.gameData.rankScore);

			LoadUserRanking();
			firstEntry = false;
		}
	}

	private void SetFlagOption() {
		var flagList = new List<Sprite>();
		flagList.AddRange(flags);
		userFlag.AddOptions(flagList);
	}

	private void SetUserRankingData(string rank = "-", int flag = 0, string name = "", string score = "0") {
		userRank.text = rank;
		userFlag.value = flag;
		SetUserNameTextField(name);
		rankScore.text = score;
	}

    private void SetUserNameTextField(string name) {
		userName.text = name;
		userName.characterLimit = 10;
		userName.onValueChanged.AddListener(
			(word) => userName.text = Regex.Replace(word, @"[^0-9a-zA-Z??-?R]", "")
			);
	}

	public void UploadUserData() {
		if (!isCoolTime) {
			UploadUserDataWithFirebase(userFlag.value, userName.text);
			StartCoroutine(nameof(BtnDelay));
		}
	}

	private IEnumerator BtnDelay() {
		isCoolTime = true;
		yield return new WaitForSeconds(UPLOAD_BOTTON_DELAY);
		isCoolTime = false;
	}

	private void UploadUserDataWithFirebase(int flag, string userName) {
		User user = new User(flag, userName, int.Parse(bestScore.text));
		currUserData = user;

		DataManager.init.tempData.rankFlag = currUserData.flag;
		DataManager.init.tempData.rankName = currUserData.name;
		DataManager.init.tempData.rankScore = currUserData.score;

		string json = JsonUtility.ToJson(user);

		if (!isKey()) {
			key = databaseReference.Child(TITLE).Push().Key;
			DataManager.init.tempData.key = key ?? "";
		}

		databaseReference.Child(TITLE).Child(key).SetRawJsonValueAsync(json);
		LoadUserRanking();
	}

	private bool isKey() {
		return null != key && !key.Equals("");
    }

	private void LoadUserRanking() {
		DeleteCurrRankingObject();
		rankingCount = LIMIT_USER_DATA;
		FirebaseDatabase.DefaultInstance.GetReference(TITLE)
			.OrderByChild(SCORE)
			.LimitToLast(LIMIT_USER_DATA)
			.ChildAdded += HandleChildAddedRanking;
	}

	private void LoadMyData() {
		FirebaseDatabase.DefaultInstance.GetReference(TITLE)
			.OrderByChild(SCORE)
			.StartAt(currUserData.score)
			.ChildAdded += HandleChildAddedUserData;
	}

	private void HandleChildAddedRanking(object sender, ChildChangedEventArgs arge) {
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

		if (rankingCount <= 1)
			LoadMyData();
	}
	private void HandleChildAddedUserData(object sender, ChildChangedEventArgs arge) {
		if (arge.DatabaseError != null) {
			Debug.LogError(arge.DatabaseError.Message);
			return;
		};
		Debug.Log($"my rank : {myRankingCount++}");
	}


	private void DeleteCurrRankingObject() {
		foreach(Transform iter in rankList.GetComponentsInChildren<Transform>()) {
			if(iter.gameObject != rankList.gameObject) {
				Destroy(iter.gameObject);
			}
        }
    }
}