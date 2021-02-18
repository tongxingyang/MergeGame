using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaxLine : MonoBehaviour {
    private static readonly int DELAY_ANIMATION = 5;

    int isWaring = Animator.StringToHash("isWaring");
    public static MaxLine init;
    private void Awake() {
		if (init == null) {
			init = this;
		} else if (init != this) {
			Destroy(this.gameObject);
		}
	}

    public float y {
        get { return _y; }
    }

    private float _y;
    private Animator animator;
    private GameObject preObj = null;
    private GameObject currObj = null;

    private void Start() {
        _y = transform.position.y;
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (currObj == null) {
            currObj = collision.gameObject;
            StartCoroutine(nameof(delayToAnimation));
        }
    }

    IEnumerator delayToAnimation() {
        yield return new WaitForSeconds(DELAY_ANIMATION);
        if (currObj != null) {
            StartFlickerAnim();
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (currObj == collision.gameObject) {
            currObj = null;
        }
    }

    public void StartFlickerAnim() {
        animator.SetBool(isWaring, true);
    }

    public void StopFlickerAnim() {
        animator.SetBool(isWaring, false);
    }
}
