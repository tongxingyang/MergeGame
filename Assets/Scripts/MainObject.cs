using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MainObject : MonoBehaviour {
	//readonly string TAG = "mainObject";

	public ObjectManager.Type type;

	private bool _thisObjectIsCrash = false;
	public bool thisObjectIsCrash {
		get { return _thisObjectIsCrash; }
		set { _thisObjectIsCrash = value; }
    }

	private void OnCollisionEnter2D(Collision2D collision) {
		Vector2 velocity = this.GetComponent<Rigidbody2D>().velocity;
		Vector2 targetVelocity = collision.gameObject.GetComponent<Rigidbody2D>().velocity;

		nextObjectSpawn();

		if (collision.gameObject.CompareTag("object") && CompareTag("object")) {
			if (velocity.sqrMagnitude > targetVelocity.sqrMagnitude) {
				ObjectManager.init.objectCrash(collision.gameObject.GetComponent<MainObject>(), this.GetComponent<MainObject>());
			}
		}
	}

	private void nextObjectSpawn() {
        if (this.thisObjectIsCrash) {
			this.thisObjectIsCrash = false;
			GameManager.init.isNextObjectSpawn();
        }
	}
}