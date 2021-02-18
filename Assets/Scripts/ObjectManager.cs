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

	private static Queue<GameObject> garbageObjectContainer;

	private GameObject currObject;

	private void Start() {
		CreateObject(0);
		garbageObjectContainer = new Queue<GameObject>();
	}

	public void CreateObject(int rand) {
		currObject = Instantiate(objects[rand], new Vector3(0, INIT_Y_POSITION, 0), Quaternion.identity);
		MouseControl.init.SetCurrObject(currObject);
	}

	public void RespawnCurrObject() {
		currObject.GetComponent<MainObject>().Setting();
		StartCoroutine(nameof(TimeToNextObject));
	}

	public void MergeObject(MainObject target, MainObject curr) {
		if (target.mergeLevel == MergeLevel.max) return;

		Instantiate(objects[(int)target.mergeLevel], target.transform.position, Quaternion.identity)
			.GetComponent<MainObject>().Setting();

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
}