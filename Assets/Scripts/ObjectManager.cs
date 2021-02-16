using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour {
	private static readonly int GARBAGE_COUNT = 10;


    public enum Type {
		one, two, three, four, five, six, seven, eight, nine, ten, max
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

	private static Queue<GameObject> garbageObjectContainer;

    private void Start() {
		garbageObjectContainer = new Queue<GameObject>();
    }

    public void mergeObject(MainObject target, MainObject curr) {
		if (target.type == Type.max) return;
		addGarbageObject(new GameObject[] { target.gameObject, curr.gameObject });
		GameManager.init.createMargeObject((int)target.type + 1, target.transform.position);
	}

	public void addGarbageObject(GameObject[] gameObjects) {
		if (garbageObjectContainer.Count > GARBAGE_COUNT) {
			for (int i = 0; i < GARBAGE_COUNT; ++i) {
				Destroy(garbageObjectContainer.Dequeue());
			}
		}

		foreach(GameObject gameObject in gameObjects) {

			garbageObjectContainer.Enqueue(gameObject);
			gameObject.SetActive(false);
		}

		Debug.Log(garbageObjectContainer.Count);
	}
}
