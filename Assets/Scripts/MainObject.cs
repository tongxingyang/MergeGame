using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MainObject : MonoBehaviour {
	private static readonly int GRAVITY_SCALE = 2;

	public ObjectManager.MergeLevel mergeLevel;

	public float radius {
		get { return _radius; }
    }

	private float _radius;
	private Sprite sprite;
	private bool isDrop = false;
	
    private void Update() {
		if (isDrop) {
			MaxLine.init.WaringLine(this.transform.position.y);
		} else if (isGameOver()) {
			GameManager.init.GameOver();
		} else {
			MaxLine.init.StopFlickerAnim();
        } 
    }

	private bool isGameOver() {
		return (MaxLine.init.OVER_LINE < this.transform.position.y && isDrop);
	}

	public void Setting() {
		try {
			sprite = GetComponent<SpriteRenderer>().sprite;
			_radius = sprite.rect.width / (sprite.pixelsPerUnit * 0.01f) * 0.005f;

			gameObject.AddComponent<CircleCollider2D>();
			gameObject.GetComponent<CircleCollider2D>().radius = radius;
			gameObject.GetComponent<Rigidbody2D>().gravityScale = GRAVITY_SCALE;

		} catch (System.NullReferenceException e) {
			Debug.Log(e.StackTrace);
		}
	}

	private void OnCollisionEnter2D(Collision2D collision) {
		isDrop = true;

		if (isBothObjects(collision.gameObject)) {
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
}