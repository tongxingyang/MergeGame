using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Firebase;
using Firebase.Database;

public class RankingSystem : MonoBehaviour {
	static readonly private string TITLE = "Ranking";
	static readonly private string COUNTRY = "country";
	static readonly private string NAME = "name";
	static readonly private string SCORE = "score";

	public string key = "";

	public static RankingSystem init = null;
	private void Awake() {
		if (init == null) {
			init = this;
		}
	}

	public class User {
		public int country;
		public string name;
		public int score;

		public User(int country ,string name, int score) {
			this.country = country;
			this.name = name;
			this.score = score;
		}
	}

	DatabaseReference databaseReference;

	private void Start() {
		//Need to DatabaseReference for write Data
		databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
	}

	public void OnSave() {
		SaveUserRanking(1, "Test01", 111111);
	}

	public void OnLoad() {
		LoadUserRanking();
	}

	private void SaveUserRanking(int country, string userName, int score) {
		User user = new User(country, userName, Random.Range(10, 10000));
		string json = JsonUtility.ToJson(user);

		if (null == key) {
			key = databaseReference.Child(TITLE).Push().Key;
		}

		databaseReference.Child(TITLE).Child(key).SetRawJsonValueAsync(json);
	}

	private void LoadUserRanking() {
		databaseReference = FirebaseDatabase.DefaultInstance.GetReference(TITLE);
		databaseReference.GetValueAsync().ContinueWith(task => {
			if (task.IsCompleted) {
				DataSnapshot snapshot = task.Result;

				foreach(DataSnapshot data in snapshot.Children) {
					IDictionary rank = (IDictionary)data.Value;
					Debug.Log($"나라:{rank[COUNTRY]} / 이름:{rank[NAME]} / 스코어:{rank[SCORE]}");
				}
			}
		});
	}
}
