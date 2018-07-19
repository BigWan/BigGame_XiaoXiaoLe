using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour {

    [SerializeField]
    private Vector2Int _pos;
    public Vector2Int pos {
        get {
            return _pos;
        }
        set {
            _pos = value;
            transform.localPosition = new Vector3(_pos.x, _pos.y);
        }
    }


    [SerializeField]
    private bool _lord;
    public bool lord {
        get {
            return _lord;
        }
        set {
            _lord = value;
            _spriteRender.sprite = _lord ? lightSprite : darkSprite;
        }
    }


    public Sprite lightSprite;      // 
    public Sprite darkSprite;

    private SpriteRenderer _spriteRender;

    public void Awake() {
        _spriteRender = GetComponent<SpriteRenderer>();
    }

    
}
