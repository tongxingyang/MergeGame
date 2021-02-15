﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour {
	private static readonly float INIT_Y_POSITION = 4;
	private static readonly int GRAVITY_SCALE = 2;
	private static readonly int MAX_CREATE_OBJECT_NUMBER = 5;

	public static GameManager init = null;
	private void Awake() {
		if (init == null) {
			init = this;
		} else if (init != this) {
			Destroy(this.gameObject);
		}
		DontDestroyOnLoad(this.gameObject);
	}

	public GameObject[] objects;
	private GameObject newSpawnObject;

	private void Start() {
		createObject(0);
	}

	public void isNextObjectSpawn() {
		int rand = Random.Range(0, MAX_CREATE_OBJECT_NUMBER);
		newSpawnObject.GetComponent<MainObject>().thisObjectIsCrash = false;

		createObject(rand);
	}

	public void createObject(int rand) {
		newSpawnObject = Instantiate(objects[rand], new Vector3(0, INIT_Y_POSITION, 0), Quaternion.identity);
		newSpawnObject.GetComponent<MainObject>().thisObjectIsCrash = true;
	}

	public void createMargeObject(int type, Vector3 position) {
		Instantiate(objects[type], position, Quaternion.identity).GetComponent<Rigidbody2D>().gravityScale = GRAVITY_SCALE;
	}

	private void OnMouseDown() {
		objectControlWhenOnTouch();
	}

	private void OnMouseDrag() {
		objectControlWhenOnTouch();
	}

	private void objectControlWhenOnTouch() {
		Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		newSpawnObject.transform.position = new Vector3(mousePosition.x, INIT_Y_POSITION, 0);
	}

	private void OnMouseUp() {
		newSpawnObject.GetComponent<Rigidbody2D>().gravityScale = GRAVITY_SCALE;
	}
}
