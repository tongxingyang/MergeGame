using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour {
	private static readonly float INIT_Y_POSITION = 4;
	private static readonly int GRAVITY_SCALE = 2;
	private static readonly int MAX_CREATE_OBJECT_NUMBER = 5;
	private static readonly float TIME_TO_NEXT_OBJECT = 0.7f;

	public static GameManager init = null;
	public Text coin, currScore, bestScore;

	private ScoreManager scoreManager;

	private void Awake() {
		if (init == null) {
			init = this;
		} else if (init != this) {
			Destroy(this.gameObject);
		}
		DontDestroyOnLoad(this.gameObject);
	}

	public GameObject[] objects;
	private GameObject newSpawnObject;
	private bool isPrepareNextObject = false;

	private void Start() {
		createObject(0);
		scoreManager = new ScoreManager(coin, currScore, bestScore);
	}

	public void createObject(int rand) {
		newSpawnObject = Instantiate(objects[rand], new Vector3(0, INIT_Y_POSITION, 0), Quaternion.identity);
		isPrepareNextObject = true;
	}

	public void createMargeObject(int type, Vector3 position) {
		setGravityScale(Instantiate(objects[type], position, Quaternion.identity));
		scoreManager.changingScore(type);
	}

	private void setGravityScale(GameObject gameObject) {
		try {
			gameObject.GetComponent<Rigidbody2D>().gravityScale = GRAVITY_SCALE;
		} catch (NullReferenceException e) {
			Debug.Log(e.StackTrace);
        }
	}

	private void OnMouseDown() {
		objectControlWhenOnTouch();
	}

	private void OnMouseDrag() {
		objectControlWhenOnTouch();
	}

	private void objectControlWhenOnTouch() {
		if (isPrepareNextObject) {
			Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			newSpawnObject.transform.position = new Vector3(mousePosition.x, INIT_Y_POSITION, 0);
		}
	}

	private void OnMouseUp() {
		if (isPrepareNextObject) {
			setGravityScale(newSpawnObject);
			StartCoroutine("timeToNextObject");
		}
	}

	private void destroyObject() {
		newSpawnObject = null;
		isPrepareNextObject = false;
	}

	IEnumerator timeToNextObject() {
		destroyObject();
		yield return new WaitForSeconds(TIME_TO_NEXT_OBJECT);
		createObject(Random.Range(0, MAX_CREATE_OBJECT_NUMBER));
    }
}
