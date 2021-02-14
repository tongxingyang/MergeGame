using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MainObject : MonoBehaviour {
	string TAG = "mainObject";

	public enum Type {
		s,
		m,
		l,
		xl
	}

	public Type type;
	private Rigidbody2D rigidbody;

	private void Start() {
		rigidbody = this.GetComponent<Rigidbody2D>();
	}

	private void OnCollisionEnter2D(Collision2D collision) { 
		if (collision.gameObject.CompareTag(TAG)) {
			if (collision.gameObject.GetComponent<MainObject>().type == this.type) {

				if (rigidbody.velocity.x > collision.gameObject.GetComponent<Rigidbody2D>().velocity.x || rigidbody.velocity.y > collision.gameObject.GetComponent<Rigidbody2D>().velocity.y) {
					this.gameObject.SetActive(false);
					return;
				} else {
					Debug.Log(rigidbody.velocity);
					if (this.type == Type.m) return;
					this.spawn();
					this.gameObject.SetActive(false);
				}
			}
		}
	}

	public void spawn() {
		SpwanManager.init.objectSpawn((int)this.type + 1, this.transform.position);
	}
}
