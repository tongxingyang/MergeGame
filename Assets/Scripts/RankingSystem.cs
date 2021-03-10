using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Firebase;
using Firebase.Database;
using System.Text.RegularExpressions;

public class RankingSystem : MonoBehaviour {
	static readonly private string TITLE = "RANK";
	static readonly private string FLAG = "flag";
	static readonly private string NAME = "name";
	static readonly private string SCORE = "score";
	static readonly private int LIMIT_USER_DATA = 10;

	public string key;
	public Transform rankList;
	public GameObject rankPrefab;
	public Sprite[] flags;

	public TextMeshProUGUI userRank;
	public TextMeshProUGUI bestScore, rankScore;
	public TMP_Dropdown userFlag;
	public TMP_InputField userName;

	public static RankingSystem init = null;

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

	private void Start() {
		databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
	}

    private void OnEnable() {
		SetFlagOption();
		bestScore.text = ScoreManager.init.bestScoreText.text;

		if (null == key || key.Equals("")) {
			SetUserRankingData();
		} else {
			LoadUserRankingByMyClass();
		}

		if (rankList.childCount <= 1) {
			LoadUserRankingByTopClass();
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

	public void OnSave() {
		SaveUserRanking(userFlag.value, userName.text);
	}

	private void SaveUserRanking(int flag, string userName) {
		User user = new User(flag, userName, int.Parse(bestScore.text));
		string json = JsonUtility.ToJson(user);

		if (null == key || key.Equals("")) {
			key = databaseReference.Child(TITLE).Push().Key;
		}

		databaseReference.Child(TITLE).Child(key).SetRawJsonValueAsync(json);
		LoadUserRankingByMyClass();
	}

	private void LoadUserRankingByTopClass() {
		FirebaseDatabase.DefaultInstance.GetReference(TITLE)
			.OrderByChild(SCORE)
			.LimitToLast(LIMIT_USER_DATA)
			.ValueChanged += HandleValueChangedByTopClass;
	}

	private void LoadUserRankingByMyClass() {
        FirebaseDatabase.DefaultInstance
            .GetReference(TITLE)
            .OrderByChild(SCORE)
            .StartAt(int.Parse(bestScore.text))
            .ValueChanged += HandleValueChangedByMyClass;
    }

	private void HandleValueChangedByMyClass(object sender, ValueChangedEventArgs arge) {
		if (arge.DatabaseError != null) {
			Debug.LogError(arge.DatabaseError.Message);
			return;
		};

		List<DataSnapshot> list = new List<DataSnapshot>();
		list.AddRange(arge.Snapshot.Children);

		foreach (DataSnapshot data in list) {
			IDictionary rank = (IDictionary)data.Value;

			if (key.Equals(arge.Snapshot.Key)) {
				SetUserRankingData(list.Count.ToString(), int.Parse(rank[FLAG].ToString()),
					rank[NAME].ToString(), rank[SCORE].ToString());

				break;
			}
			//Debug.Log("????: " + rank[key].ToString() + " / ????:{rank[NAME]} / ??????:{rank[SCORE]}");
		}
		if (list.Count < LIMIT_USER_DATA)
			LoadUserRankingByTopClass();

	}

	private void HandleValueChangedByTopClass(object sender, ValueChangedEventArgs arge) {
		if (arge.DatabaseError != null) {
			Debug.LogError(arge.DatabaseError.Message);
			return;
		};

		List<DataSnapshot> list = new List<DataSnapshot>();
		list.AddRange(arge.Snapshot.Children);
		list.Reverse();

		int rankingCount = 1;
		DeleteCurrRankingObject();
		foreach (DataSnapshot data in list) {
			IDictionary rank = (IDictionary)data.Value;
			GameObject tempRank = Instantiate(rankPrefab);
			tempRank.transform.SetParent(rankList);
			tempRank.GetComponent<RectTransform>().localScale = Vector3.one;
			tempRank.GetComponent<RankUserData>().SetRankData(rankingCount++,
				flags[int.Parse(rank[FLAG].ToString())], rank[NAME], rank[SCORE]);

			Debug.Log($"????:{rank[FLAG]} / ????:{rank[NAME]} / ??????:{rank[SCORE]}");
		}
	}

	private void DeleteCurrRankingObject() {
		foreach(Transform iter in rankList.GetComponentsInChildren<Transform>()) {
			if(iter.gameObject != rankList.gameObject) {
				Destroy(iter.gameObject);
			}
        }
    }
}
