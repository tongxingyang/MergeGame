using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaxLine : MonoBehaviour {
    private static readonly float DELAY_ANIMATION = 1f;
    private static readonly float DELAY_GAMEOVER = 2f;
    private static readonly int isWaring = Animator.StringToHash("isWaring");

    private GameObject temp;

    public static MaxLine init;
    private void Awake() {
		if (init == null) {
			init = this;
		} else if (init != this) {
			Destroy(this.gameObject);
		}
	}

    private float _waringLine;
    public float WARING_LINE {
        get {
            return this.transform.position.y - boxCollider2D.size.y;
        }
    }

    private float _overLine;
    public float OVER_LINE {
        get {
            return this.transform.position.y;
        }
    }

    private Animator animator;
    private BoxCollider2D boxCollider2D = null;

    private void Start() {
        animator = GetComponent<Animator>();
        boxCollider2D = this.GetComponent<BoxCollider2D>();
    }

    public void WaringLine(MainObject obj) {
        float y = obj.GetComponent<MainObject>().returnYPos();

        if (gameObject.activeSelf) {
            if (WARING_LINE < y && OVER_LINE > y) {
                StartCoroutine(nameof(delayToWaringLine));
            } else if (OVER_LINE < y) {
                StartCoroutine(nameof(delayToGameOver), obj);
            }
        }
    }

    public void StartFlickerAnim() {
    }

    public void StopFlickerAnim() {
        animator.SetBool(isWaring, false);
    }

    IEnumerator delayToWaringLine() {
        yield return new WaitForSeconds(DELAY_ANIMATION);
        animator.SetBool(isWaring, true);
    }

    IEnumerator delayToGameOver(MainObject obj) {
        yield return new WaitForSeconds(DELAY_GAMEOVER);
        try {
            if (OVER_LINE < obj.gameObject.transform.position.y) {
                obj.ObjStateWhenGameOver();
                GameManager.init.GameOver();
            }
        }catch(Exception e) {
            Debug.Log(e.StackTrace);
        }
    }

    public void setColor(bool isGameOver) {
        animator.SetBool(isWaring, false);
        GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
        gameObject.SetActive(!isGameOver);
    }
}
