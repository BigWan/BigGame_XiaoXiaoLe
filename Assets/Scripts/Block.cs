using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum ColorType {
    Blue,
    Green,
    Orange,
    Red,
    Violet,
    White
}

/// <summary>
/// 消消乐里面的块,小动物,蔬菜,或是其他的东西
/// </summary>
public class Block : MonoBehaviour {

    //private int hp = 1; // 生命值,消除一次hp-1,hp = 0死亡

    public ColorType colorType; // 同色才能爆炸
    public Sprite[] sprites;    // 不同HP的时候显示不同的Sprite

    //public bool CanFall = true; // 是否能自由下落
    public Vector2Int pos;

    public bool selected;
    private EventHandler selectEvent;

    void Awake(){
        selectEvent += OnSelected;
    }

    private void Update() {
        transform.localPosition = new Vector3(pos.x, pos.y);
        if (selected) Select();
    }

    public void Select(){
        selected = true;
        selectEvent(this,EventArgs.Empty);
    }

    public void OnSelected(object o,EventArgs e){
        PlayeSelectAnim();
    }
    public void PlayeSelectAnim(){
        Debug.Log("Selcted");
        GetComponent<Animation>().Play("BlockSelected");
    }



}
