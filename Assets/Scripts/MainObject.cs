using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MainObject : MonoBehaviour {
	private static readonly float DESTROY_ITEM_DELAY = 1.5f;
	private static readonly float GRAVITY_SCALE = 3.2f;
	private static readonly int FLICKER_OBJ = Animator.StringToHash("flicker");
	private static readonly int DESTROY_MAX_LEVEL = Animator.StringToHash("destroy");
	private static readonly int FADE_OUT = Animator.StringToHash("fadeout");

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
	private Rigidbody2D _rigidbody;
	private Vector3 fixedPos, dynamicPos;
	private Vector3 screenPos;

	private bool isDrop = false;
	private bool isFixed = false;
	private bool isMerging = false;


	private void Start() {
		animator = GetComponent<Animator>();
		_rigidbody = GetComponent<Rigidbody2D>();
		radius = GetRadius();
		screenPos.x = Camera.main.ViewportToWorldPoint(Vector2.zero).x;
		screenPos.y = Camera.main.ViewportToWorldPoint(Vector2.one).x;
		StartCoroutine(CoAsyncPosition());
	}

	private void Update() {
		if(animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.destroymaxlevel") &&
			animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f){
			gameObject.SetActive(false);
		}


		dynamicPos = transform.position;

        if (isFixed) {
			dynamicPos = fixedPos;
        } else if (isDrop) {
			MaxLine.init.WaringLine(this);
		} else {
			MaxLine.init.StopFlickerAnim();
        }
		//TODO : 밖으로 나가지 않게 하는 함수 수정
		//if (transform.position.x <= ObjectManager.init.backgroundLeft) dynamicPos.x = ObjectManager.init.backgroundLeft;
		//if (transform.position.x >= ObjectManager.init.backgroundRight) dynamicPos.x = ObjectManager.init.backgroundRight;

		transform.position = dynamicPos;
	}

	private IEnumerator CoAsyncPosition() {
		while(true) {
			if (transform.position.x - radius < ObjectManager.init.backgroundLeft) {
				Vector2 temp = new Vector2(ObjectManager.init.backgroundLeft + radius, transform.position.y);
				transform.position = temp;
				_rigidbody.angularVelocity = 0;
			}
			else if (transform.position.x + radius > ObjectManager.init.backgroundRight) {
				Vector2 temp = new Vector2(ObjectManager.init.backgroundRight - radius, transform.position.y);
				transform.position = temp;
				_rigidbody.angularVelocity = 0;
			}
			yield return null;
        }
    }

	public void Setting() {
		try {
			radius = GetRadius();
			gameObject.AddComponent<CircleCollider2D>();
			gameObject.GetComponent<CircleCollider2D>().radius = radius;
			gameObject.GetComponent<Rigidbody2D>().gravityScale = GRAVITY_SCALE;

		} catch (System.NullReferenceException e) {
			Debug.Log(e.StackTrace);
		}
	}

	private float GetRadius() {
		sprite = GetComponent<SpriteRenderer>().sprite;
		return sprite.rect.width / (sprite.pixelsPerUnit * 0.01f) * 0.005f;
	}

	private void OnCollisionEnter2D(Collision2D collision) {
		if (!collision.gameObject.CompareTag("Untagged"))
			isDrop = true;

		if (isBothObjects(collision.gameObject) && !isMerging) {
			isMerging = true;
			targetPosCheckAndMerge(collision.gameObject);
		}
	}

	private void OnCollisionStay2D(Collision2D collision) {
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
		if(this.transform.position.y >= collision.transform.position.y) {
			mergeObject(collision);
		}
	}

	private void mergeObject(GameObject collision) {
		ObjectManager.init.MergeObject(collision.GetComponent<MainObject>(), this);
	}

	public void ObjStateWhenGameOver() {
		StartFlickerAnim();
		GetComponent<Rigidbody2D>().gravityScale = 0;
		fixedPos = this.transform.position;
		isFixed = true;
	}

	public float returnYPos() {
		return (this.transform.position.y + radius);
    }

	private void StartFlickerAnim() {
		animator.SetBool(FLICKER_OBJ, true);
	}

	public void DestroyObj() {
		this.GetComponent<CircleCollider2D>().enabled = false;
		animator.SetTrigger(DESTROY_MAX_LEVEL);
	}

	public void IsFadeOut(bool isFadeOut) {
		if(isFadeOut)
			animator.SetBool(FADE_OUT, true);
		else
			animator.SetBool(FADE_OUT, false);
	}

	public void OnRankUpItem() {
		StartCoroutine(nameof(RankUpCor));
	}

	IEnumerator RankUpCor() {
		ObjStateWhenGameOver();
		yield return new WaitForSeconds(DESTROY_ITEM_DELAY);
		ObjectManager.init.CreateMergeObject(this);
	}

	public void OnDestroyItem() {
		StartCoroutine(nameof(DestroyItemCor));
	}

	IEnumerator DestroyItemCor() {
		ObjStateWhenGameOver();
		yield return new WaitForSeconds(DESTROY_ITEM_DELAY);
		DestroyObj();
		UIManager.init.PlayDestroySound(); 
	}
}