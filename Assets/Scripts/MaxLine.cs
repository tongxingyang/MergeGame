using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaxLine : MonoBehaviour {
    private static readonly float DELAY_ANIMATION = 1.5f;
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
    }

    public void WaringLine(float y) {
        if (gameObject.activeSelf) {
            if (WARING_LINE < y && OVER_LINE > y) {
                StartFlickerAnim();
            } else if (OVER_LINE < y) {
                GameManager.init.GameOver();
            }
        }
    }

    public void StartFlickerAnim() {
        StartCoroutine(nameof(delayToWaringLine));
    }

    public void StopFlickerAnim() {
        animator.SetBool(isWaring, false);
    }

    IEnumerator delayToWaringLine() {
        yield return new WaitForSeconds(DELAY_ANIMATION);
        animator.SetBool(isWaring, true);
    }
}
