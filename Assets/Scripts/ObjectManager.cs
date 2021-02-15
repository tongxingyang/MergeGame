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

	private Queue<MainObject> garbageObject;

    private void Start() {
		garbageObject = new Queue<MainObject>();
    }

    public void objectCrash(MainObject target, MainObject temp) {
		GameManager.init.isNextObjectSpawn = true;

		if(target.type == temp.type) {
			margeObject(target, temp);
        }
    }

	private void margeObject(MainObject target, MainObject temp) {
		if (target.type == Type.max) return;

		GameManager.init.createMargeObject((int)target.type + 1, target.transform.position);

		garbageObjectClearAndAdd(target, temp);
    }

	private void garbageObjectClearAndAdd(MainObject target, MainObject temp) {
		if(garbageObject.Count > GARBAGE_COUNT) {
			for(int i = 0; i < GARBAGE_COUNT; ++i) {
				Destroy(garbageObject.Dequeue().gameObject);
            }
        }

		garbageObject.Enqueue(target);
		garbageObject.Enqueue(temp);

		target.gameObject.SetActive(false);
		temp.gameObject.SetActive(false);

		Debug.Log(garbageObject.Count);
	}
}
