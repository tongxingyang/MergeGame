using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MainObject : MonoBehaviour {

	public ObjectManager.Type type;

	private Sprite sprite;

    private void Start() {
		setColliderRadius();
    }

	private void setColliderRadius() {
		sprite = this.GetComponent<SpriteRenderer>().sprite;

		this.GetComponent<CircleCollider2D>().radius =
			sprite.rect.width / (sprite.pixelsPerUnit * 0.01f) * 0.005f;
    }

    private void OnCollisionEnter2D(Collision2D collision) {

		if (isBothObjects(collision.gameObject)) {
			targetPosCheckAndMerge(collision.gameObject);
		}
	}

	private bool isBothObjects(GameObject collision) {
		if (collision.CompareTag("object") && CompareTag("object")) {
			if (this.type == collision.GetComponent<MainObject>().type)
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
		ObjectManager.init.mergeObject(collision.GetComponent<MainObject>(), this);
	}
}