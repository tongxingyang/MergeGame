using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour {


    [SerializeField] private bool _isLeft;
    [SerializeField] private BoxCollider2D _boxCollider;

    private float _x;

    private void Start() {
        transform.position = GetPos();
    }

    public float GetX() {
        return _x;
    }

    private Vector2 GetPos() {
        float halfRect = _boxCollider.size.x * 0.5f;
        Vector2 pos;
        float maxWidth = Screen.width;
        float maxHeight = Screen.height;

        if (maxHeight < maxWidth) maxWidth = 1080;

        float realHalf = Screen.width * 0.5f;
        float ingameHalf = maxWidth * 0.5f;// > 540 ? 540 : realHalf;


        if (_isLeft) {
            pos = Camera.main.ScreenToWorldPoint(new Vector2(realHalf - ingameHalf, 0));
            _x = pos.x;
            pos.x -= halfRect;
        } else {
            pos = Camera.main.ScreenToWorldPoint(new Vector2(realHalf + ingameHalf, 0));
            _x = pos.x;
            pos.x += halfRect;
        }
        pos.y = 0;

        return pos;
    }
}