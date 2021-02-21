using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class MouseControl : MonoBehaviour {
	public static MouseControl init = null;
	private void Awake() {
		if (init == null) {
			init = this;
		} else if (init != this) {
			Destroy(this.gameObject);
		}
		DontDestroyOnLoad(this.gameObject);
	}

	private GameObject _currObject;
	public GameObject currObject {
		get { return _currObject; }
        set {
			if (value == null) {
				isDropCurrObejct = true;
			} else if (value.GetType() == typeof(GameObject)) {
				isDropCurrObejct = false;
				_currObject = value;
			}
        }
    }

	private bool isDropCurrObejct = false;

	public void SetCurrObject(GameObject temp = null) {
		currObject = temp;
    }

	private void OnMouseUp() {
		if (!isUITouch()) {
			if (!isDropCurrObejct) {
				SetCurrObject();
				ObjectManager.init.RespawnCurrObject();
			}
		}
	}

	private void OnMouseDown() {
		ObjectControlWhenOnTouch();
	}

	private void OnMouseDrag() {
		ObjectControlWhenOnTouch();
	}

	private void ObjectControlWhenOnTouch() {
		if (!isUITouch()) {
			GameManager.init.GameStart();

			if (!isDropCurrObejct) {

				Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				mousePosition.y = currObject.transform.position.y;

				currObject.transform.position = new Vector3(mousePosition.x, mousePosition.y, 0);
			}
		}
	}

	private bool isUITouch() {
		return EventSystem.current.IsPointerOverGameObject();
	}

	public void GameOver(bool isOver) {
		if (isOver) {
			currObject = null;
			this.gameObject.SetActive(false);
		} else {
			this.gameObject.SetActive(true);
		}
	}
}
