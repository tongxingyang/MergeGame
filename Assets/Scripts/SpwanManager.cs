using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpwanManager : MonoBehaviour {
	private float INIT_Y_POSITION = 4;

	public static SpwanManager init = null;
	private void Awake() {
		if(init == null) {
			init = this;
		} else if (init != this) {
			Destroy(this.gameObject);
		}
		DontDestroyOnLoad(this.gameObject);
	}

	public GameObject[] mainObjects;
	private GameObject currObject;

	private void Start() {
		objectSpawn();
	}

	public void objectSpawn() {
		int rand = Random.Range(0, mainObjects.Length);
		currObject = Instantiate(mainObjects[rand], new Vector3(0, INIT_Y_POSITION, 0), Quaternion.identity);
	}

	public void objectSpawn(int type, Vector3 position) {
		currObject = Instantiate(mainObjects[type], position, Quaternion.identity);
	}

	private void OnMouseDrag() {
		Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		currObject.transform.position = new Vector3(mousePosition.x, INIT_Y_POSITION, 0);
	}

	private void OnMouseUp() {
		currObject.GetComponent<Rigidbody2D>().gravityScale = 1;
		objectSpawn();
	}
}
