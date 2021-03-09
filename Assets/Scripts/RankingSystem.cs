using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Firebase;
using Firebase.Database;
using System.Text.RegularExpressions;

public class RankingSystem : MonoBehaviour {
	static readonly private string TITLE = "Ranking";
	static readonly private string FLAG = "flag";
	static readonly private string NAME = "name";
	static readonly private string SCORE = "score";
	static readonly private int LIMIT_USER_DATA = 10;

	public enum COUNTRY {
		korea,
		US
    }

	public string key = "";
	public Transform rankList;
	public GameObject rankPrefab;
	public List<Sprite> flags;

	public TextMeshProUGUI userRank;
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
		public COUNTRY flag;
		public string name;
		public int score;

		public User(COUNTRY flag, string name, int score) {
			this.flag = flag;
			this.name = name;
			this.score = score;
		}
	}

	private int _myRank = 123;
	public int myRank {
		get { return _myRank; }
        set {
			if(value < 1) {
				_myRank = 1;
				return;
            }
			_myRank = value;
			userRank.text = $"{value} ";
		}
    }
	private int _myRankingScore = 123410;
	public int myRankingScore {
		get { return _myRankingScore; }
        set {
			if(value < 0 ) {
				_myRankingScore = 0;
				return;
            }
        }
    }

	private void OnEnable() {
		//Need to DatabaseReference for write Data
		databaseReference = FirebaseDatabase.DefaultInstance.RootReference;

		SetUserNameTextField();
		SetUserRank();
		SetUserFlag();
		OnLoad();
	}

	private void SetUserNameTextField() {
		if (isGameData()) 
			userName.text = DataManager.init.gameData.userName;

		userName.characterLimit = 10;
		userName.onValueChanged.AddListener(
			(word) => userName.text = Regex.Replace(word, @"[^0-9a-zA-Z°¡-ÆR]", "")
			);
	}

	private void SetUserRank() {
		if (isGameData()) {
			if (DataManager.init.gameData.userRank == 0)
				userRank.text = "-";
			else
				userRank.text = DataManager.init.gameData.userRank.ToString();
		} else {
			userRank.text = "-";
		}
	}

	private void SetUserFlag() {
		userFlag.AddOptions(flags);
		if (isGameData()) {
			
		}
	}

	public void OnSave() {
		SaveUserRanking(COUNTRY.US, "Test01", 111111);
	}

	public void OnLoad() {
		//if (myRank <= -1)
			LoadUserRankingByTopClass();
		//else
			//LoadUserRankingByMyClass();
	}

	private void SaveUserRanking(COUNTRY country, string userName, int score) {
		User user = new User(country, userName, Random.Range(10, 10000));
		string json = JsonUtility.ToJson(user);

		if (null == key) {
			key = databaseReference.Child(TITLE).Push().Key;
		}

		databaseReference.Child(TITLE).Child(key).SetRawJsonValueAsync(json);
	}

	private void LoadUserRankingByTopClass() {
		FirebaseDatabase.DefaultInstance
			.GetReference(TITLE)
			.OrderByChild(SCORE)
			.LimitToLast(LIMIT_USER_DATA)
			.ValueChanged += HandleValueChangedByTopClass;
	}

	private void LoadUserRankingByMyClass() {
		FirebaseDatabase.DefaultInstance
			.GetReference(TITLE)
			.OrderByChild(SCORE)
			.StartAt(myRankingScore)
			.ValueChanged += HandleValueChangedByMyClass;
	}
	//-MVKWfnN8H8VEIIEmxCa



	private void HandleValueChangedByMyClass(object sender, ValueChangedEventArgs arge) {
		if (arge.DatabaseError != null) {
			Debug.LogError(arge.DatabaseError.Message);
			return;
		};

		List<DataSnapshot> list = new List<DataSnapshot>();
		list.AddRange(arge.Snapshot.Children);

		myRank = list.Count;

		if (myRank < LIMIT_USER_DATA) {
			LoadUserRankingByTopClass();
			return;
		}
		list.Reverse();

		int count = 0;
		foreach (DataSnapshot data in list) {
			IDictionary rank = (IDictionary)data.Value;
			GameObject tempRank = Instantiate(rankPrefab);
			tempRank.transform.SetParent(rankList);
			tempRank.GetComponent<RectTransform>().localScale = Vector3.one;
			tempRank.GetComponent<RankUserData>().SetRankData(rank[FLAG], rank[NAME], rank[SCORE]);

			Debug.Log($"????:{rank[FLAG]} / ????:{rank[NAME]} / ??????:{rank[SCORE]}");
			if (++count > LIMIT_USER_DATA) return;
		}
	}

	private void HandleValueChangedByTopClass(object sender, ValueChangedEventArgs arge) {
		if (arge.DatabaseError != null) {
			Debug.LogError(arge.DatabaseError.Message);
			return;
		};

		List<DataSnapshot> list = new List<DataSnapshot>();
		list.AddRange(arge.Snapshot.Children);
		list.Reverse();

		foreach (DataSnapshot data in list) {
			IDictionary rank = (IDictionary)data.Value;
			GameObject tempRank = Instantiate(rankPrefab);
			tempRank.transform.SetParent(rankList);
			tempRank.GetComponent<RectTransform>().localScale = Vector3.one;
			tempRank.GetComponent<RankUserData>().SetRankData(rank[FLAG], rank[NAME], rank[SCORE]);

			Debug.Log($"????:{rank[FLAG]} / ????:{rank[NAME]} / ??????:{rank[SCORE]}");
		}
	}

	private void CreateRankObj() {

    }

	private bool isGameData() {
		if(null == DataManager.init.gameData) {
			return false;
		}
		else {
			return true;
		}
	}
}
