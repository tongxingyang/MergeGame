using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopObjMove : MonoBehaviour {
    private float rotateSpeed;
    private float moveRange;
    private float moveSpeed;

    private float runningTime;
    private float yPos;
    private RectTransform rectTransform;


    private void Start() {
        
        rotateSpeed = Random.Range(-0.3f, 0.3f);
        moveRange = Random.Range(0.1f, 0.2f);
        moveSpeed = Random.Range(0.8f, 1.2f);
        if (rotateSpeed == 0f) rotateSpeed = 0.1f;

        rectTransform = this.GetComponent<RectTransform>();
    }

    private void Update() {
        yPos = rectTransform.position.y;

        runningTime += Time.deltaTime * moveSpeed;
        yPos += moveRange * Mathf.Sin(runningTime);

        rectTransform.position = new Vector2(rectTransform.position.x, yPos);
        this.transform.Rotate(Vector3.back * 360f * Time.deltaTime * rotateSpeed);
    }

    private void OnEnable() {
        rotateSpeed = Random.Range(-0.3f, 0.3f);
        moveRange = Random.Range(0.02f, 0.05f);
        if (rotateSpeed == 0f) rotateSpeed = 0.1f;

        rectTransform = this.GetComponent<RectTransform>();
    }
}
