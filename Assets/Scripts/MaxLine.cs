using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaxLine : MonoBehaviour {
    private static readonly float DELAY_ANIMATION = 1f;
    private static readonly float DELAY_GAMEOVER = 1.5f;
    private static readonly int isWaring = Animator.StringToHash("isWaring");

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
        ObjectHeightAsync();
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

    public void StopFlickerAnim() {
        animator.SetBool(isWaring, false);
    }

    private void ObjectHeightAsync() {
        float maxWidth = Screen.width > 1080 ? 1080 : Screen.width;
        float hight = maxWidth * 1.3f;
        Vector2 pos = Camera.main.ScreenToWorldPoint(Vector2.one * hight);
        pos.x = 0;
        this.transform.position = pos;
    }

    IEnumerator delayToWaringLine() {
        yield return new WaitForSeconds(DELAY_ANIMATION);
        animator.SetBool(isWaring, true);
    }

    IEnumerator delayToGameOver(MainObject obj) {
        yield return new WaitForSeconds(DELAY_GAMEOVER);
        try {
            if ((OVER_LINE < obj.GetComponent<MainObject>().returnYPos()) && obj.gameObject.activeSelf) {
                obj.ObjStateWhenGameOver();
                GameManager.init.GameOver();
            }
        }catch(Exception e) {
            Debug.Log(e.StackTrace);
        }
    }

    public void SetColor(bool isGameOver) {
        animator.SetBool(isWaring, false);
        GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
        gameObject.SetActive(!isGameOver);
    }
}
