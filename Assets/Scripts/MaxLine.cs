using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaxLine : MonoBehaviour {
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

    private void Start() {
        _y = transform.position.y;
        animator = GetComponent<Animator>();
    }

    public void StartFlickerAnim() {
        animator.SetBool(isWaring, true);
    }
    public void StopFlickerAnim() {
        animator.SetBool(isWaring, false);
    }
}
