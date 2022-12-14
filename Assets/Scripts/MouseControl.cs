using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class MouseControl : MonoBehaviour {
	public static MouseControl init = null;

	public bool isGameStart = false;
	public bool isTouchAction = true;

	private bool isUIClicking;
	private bool isGameClicking;

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
				isDropCurrObj = true;
			} else if (value.GetType() == typeof(GameObject)) {
				isDropCurrObj = false;
				_currObject = value;
			}
        }
    }

	private bool isDropCurrObj = false;

	public void SetCurrObject(GameObject temp = null) {
		currObject = temp;
    }

    private void Start() {
		UIManager.init.OnShopClick += () => { isTouchAction = false; };
    }

    private void Update() {
		if(Input.touchCount > 0) {
			if(Input.GetTouch(0).phase == TouchPhase.Began) {
				ObjectControlWhenOnTouch();
			} else if (Input.GetTouch(0).phase == TouchPhase.Moved) {
				ObjectControlWhenOnTouch();
			} else if (Input.GetTouch(0).phase == TouchPhase.Ended) {
				OnTouchUp();
			}
		}

		 else if (Input.GetMouseButtonDown(0)) {
				ObjectControlWhenOnTouch();
		} else if ( Input.GetMouseButton(0)) {
				ObjectControlWhenOnTouch();
		} else if (Input.GetMouseButtonUp(0)) {
				OnTouchUp();
		}
	}

	private void OnTouchUp() {
		if (isGameStart && !isUITouch() && !isUIClicking) {
			if (!isDropCurrObj) {
				SetCurrObject();
				ObjectManager.init.RespawnCurrObject();
				isGameStart = false;
			}
		}
		isUIClicking = false;
		isGameClicking = false;
	}

	private void ObjectControlWhenOnTouch() {
		if (UIManager.init.lisensePanel.activeSelf) {
			UIManager.init.lisensePanel.SetActive(false);
		}

		if (!isUITouch() && isTouchAction && !isUIClicking) {
			isGameClicking = true;

            if (!isGameStart) {
				GameManager.init.OnGameStart.Invoke();
				isGameStart = true;
			}

			if (!isDropCurrObj) {
				Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				mousePosition.y = currObject.transform.position.y;

				currObject.transform.position = new Vector3(mousePosition.x, mousePosition.y, 0);
			}
		} else {
			isUIClicking = true;
		}
	}

	private bool isUITouch() {
		try {
			return (EventSystem.current.IsPointerOverGameObject() || EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId) && !isGameClicking);
		} catch {
			return false;
		}
	}

	public void GameOver(bool isOver) {
		if (isOver) {
			isGameStart = false;
			currObject = null;
			this.gameObject.SetActive(false);
		} else {
			this.gameObject.SetActive(true);
		}
	}

	public void IsTouchAction(bool action) {
		isTouchAction = action;
	}
}
