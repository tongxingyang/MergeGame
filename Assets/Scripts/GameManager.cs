using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour {
	private static readonly float INIT_Y_POSITION = 4;
	private static readonly int GRAVITY_SCALE = 4;
	private static readonly int MAX_CREATE_OBJECT_NUMBER = 3;

	public static GameManager init = null;
	private void Awake() {
		if(init == null) {
			init = this;
		} else if (init != this) {
			Destroy(this.gameObject);
		}
		DontDestroyOnLoad(this.gameObject);
	}

	public GameObject[] objects;

	private bool _isNextObjectSpawn;
	public bool isNextObjectSpawn {
        set {
			if (value) {
				int rand = Random.Range(0, MAX_CREATE_OBJECT_NUMBER);
				createObject(rand);
			}
        }
    }

	private GameObject newSpawnObject;

	private void Start() {
		createObject(0);
	}

	public void createObject(int rand) {
		if (newSpawnObject == null) {
			newSpawnObject = Instantiate(objects[rand], new Vector3(0, INIT_Y_POSITION, 0), Quaternion.identity);
		}
	}

	public void createMargeObject(int type, Vector3 position) {
		Instantiate(objects[type], position, Quaternion.identity).GetComponent<Rigidbody2D>().gravityScale = GRAVITY_SCALE;
	}

	private void OnMouseDrag() {
		try {
			Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			newSpawnObject.transform.position = new Vector3(mousePosition.x, INIT_Y_POSITION, 0);
		} catch (NullReferenceException e) {
			Debug.Log(e.StackTrace);
        }
	}

	private void OnMouseUp() {
		try {
			newSpawnObject.GetComponent<Rigidbody2D>().gravityScale = GRAVITY_SCALE;
			newSpawnObject = null;
		} catch (NullReferenceException e) {
			Debug.Log(e.StackTrace);
        }
	}
}
