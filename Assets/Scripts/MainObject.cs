using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MainObject : MonoBehaviour {
	private static readonly int GRAVITY_SCALE = 2;
	private static readonly int DELETE_OBJ = Animator.StringToHash("delete");
	private static readonly int FLICKER_OBJ = Animator.StringToHash("flicker");

	public ObjectManager.MergeLevel mergeLevel;

	private float _radius;
	public float radius {
		get { return _radius; }
		set {
			_radius = value;
        }
    }

	private Sprite sprite;
	private Animator animator;
	private Vector3 fixedPos, inCameraPos;

	private bool isDrop = false;
	private bool isFixed = false;
	private bool isMerging = false;


	private void Start() {
		animator = GetComponent<Animator>();
	}

	private void Update() {
        if (isFixed) {
			this.transform.position = fixedPos;
        } else if (isDrop) {
			MaxLine.init.WaringLine(this);
		} else {
			MaxLine.init.StopFlickerAnim();
        }

		inCameraPos = Camera.main.WorldToViewportPoint(transform.position);

		if (inCameraPos.x < 0f) inCameraPos.x = 0f;
		if (inCameraPos.x > 1f) inCameraPos.x = 1f;
		if (inCameraPos.y < 0f) inCameraPos.y = 0f;
		if (inCameraPos.y > 1f) inCameraPos.y = 1f;

		transform.position = Camera.main.ViewportToWorldPoint(inCameraPos);
	}

	public void Setting() {
		try {
			sprite = GetComponent<SpriteRenderer>().sprite;
			radius = sprite.rect.width / (sprite.pixelsPerUnit * 0.01f) * 0.005f;

			gameObject.AddComponent<CircleCollider2D>();
			gameObject.GetComponent<CircleCollider2D>().radius = radius;
			gameObject.GetComponent<Rigidbody2D>().gravityScale = GRAVITY_SCALE;

		} catch (System.NullReferenceException e) {
			Debug.Log(e.StackTrace);
		}
	}

	private void OnCollisionEnter2D(Collision2D collision) {
		if (!collision.gameObject.CompareTag("Untagged"))
			isDrop = true;

		if (isBothObjects(collision.gameObject) && !isMerging) {
			isMerging = true;
			targetPosCheckAndMerge(collision.gameObject);
		}
	}

	private bool isBothObjects(GameObject collision) {
		if (collision.CompareTag("object") && CompareTag("object")) {
			if (this.mergeLevel == collision.GetComponent<MainObject>().mergeLevel)
				return true;
		}
		return false;
	}

	private void targetPosCheckAndMerge(GameObject collision) {
		Vector2 velocity = this.GetComponent<Rigidbody2D>().velocity;
		Vector2 targetVelocity = collision.gameObject.GetComponent<Rigidbody2D>().velocity;

		if(this.transform.position.y > collision.transform.position.y) {
			mergeObject(collision);
		} else if(this.transform.position.y == collision.transform.position.y) {
			if (velocity.sqrMagnitude > targetVelocity.sqrMagnitude) {
				mergeObject(collision);
			}
		}
	}

	private void mergeObject(GameObject collision) {
		ObjectManager.init.MergeObject(collision.GetComponent<MainObject>(), this);
	}

	public void ObjStateWhenGameOver() {
		StartFlickerAnim();
		fixedPos = this.transform.position;
		isFixed = true;
	}

	public float returnYPos() {
		return this.transform.position.y + radius;
    }

	private void StartFlickerAnim() {
		animator.SetBool(FLICKER_OBJ, true);
	}

	private void OnDisable() {
		animator.SetBool(DELETE_OBJ, true);
	}
}