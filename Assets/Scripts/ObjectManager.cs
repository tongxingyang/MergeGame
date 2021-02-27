using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour {
	private static readonly int GARBAGE_COUNT = 10;
	private static readonly int MAX_CREATE_OBJECT_NUMBER = 5;
	private static readonly float TIME_TO_NEXT_OBJECT = 0.7f;
	private static readonly float INIT_Y_POSITION = 4;

	public enum MergeLevel {
		none, one, two, three, four, five, six, seven, eight, nine, ten, max
	}

	public static ObjectManager init = null;
	private void Awake() {
		if (init == null) {
			init = this;
		} else if (init != this) {
			Destroy(this.gameObject);
		}
		DontDestroyOnLoad(this.gameObject);
	}

	public GameObject[] objects;

	[HideInInspector]
	public GameObject objParent;

	private static Queue<GameObject> garbageObjectContainer;
	private GameObject currObject;

	private void Start() {
		objParent = new GameObject("objParent");
		garbageObjectContainer = new Queue<GameObject>();

		CreateObject(0);
	}

	public void CreateObject(int rand) {
		if (!GameManager.init.isGameOver) {
			currObject = Instantiate(objects[rand], new Vector3(0, INIT_Y_POSITION, 0), Quaternion.identity);
			currObject.transform.parent = objParent.transform;
			MouseControl.init.SetCurrObject(currObject);
		}
	}

	public void RespawnCurrObject() {
		currObject.GetComponent<MainObject>().Setting();
		StartCoroutine(nameof(TimeToNextObject));
	}

	public void MergeObject(MainObject target, MainObject curr) {
		if ((target.mergeLevel == MergeLevel.max) || GameManager.init.isGameOver) 
			return;

		StartMergeAudio();

		GameObject tempObj = Instantiate(objects[(int)target.mergeLevel], target.transform.position, Quaternion.identity);
		tempObj.GetComponent<MainObject>().Setting();
		tempObj.transform.parent = objParent.transform;

		AddMergedObjectToGarbage(new GameObject[] { target.gameObject, curr.gameObject });
		UIManager.init.AddScore(ScoreManager.MARGE_SCORE, target.mergeLevel);
	}

	public void AddMergedObjectToGarbage(GameObject[] gameObjects) {
		if (garbageObjectContainer.Count > GARBAGE_COUNT) {
			for (int i = 0; i < GARBAGE_COUNT; ++i) {
				Destroy(garbageObjectContainer.Dequeue());
			}
		}

		foreach (GameObject gameObject in gameObjects) {

			garbageObjectContainer.Enqueue(gameObject);
			gameObject.SetActive(false);
		}
	}

	IEnumerator TimeToNextObject() {
		yield return new WaitForSeconds(TIME_TO_NEXT_OBJECT);
		CreateObject(Random.Range(0, MAX_CREATE_OBJECT_NUMBER));
		UIManager.init.AddScore(ScoreManager.DROP_SCORE);
	}

	public void StopObj() {
		foreach (Transform gameObject in objParent.GetComponentsInChildren<Transform>()) {
			if (gameObject.gameObject.GetComponent<Rigidbody2D>() != null) {
				gameObject.gameObject.GetComponent<Rigidbody2D>().gravityScale = 0;
				gameObject.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
			}
		}
	}

	public void Restart() {
		garbageObjectContainer.Clear();
		foreach (Transform gameObject in objParent.GetComponentsInChildren<Transform>()) {
			Destroy(gameObject.gameObject);
		}

		Start();
	}

	private void StartMergeAudio() {
		GetComponent<AudioSource>().Play();
    }
}