using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum ColorType {
    Blue   = 0,
    Green  = 1,
    Orange = 2,
    Red    = 3,
    Violet = 4,
    White  = 5,
}

public enum BombType {

    SuperH,  // 水平五连
    SuperV,  // 垂直五连

    Circle1,  // 圆形炸弹象限1
    Circle2,  // 圆形炸弹象限2
    Circle3,  // 圆形炸弹象限3
    Circle4,  // 圆形炸弹象限4

    LineH,  // 水平四个
    LineV,  // 垂直四个

    NormalH, // 水平三个
    NormalV, // 垂直三个

    None  , // 没有炸弹
}

public enum Direction{
    up    = 0,
    right = 1,
    down  = 2,
    left  = 3,
}



/// <summary>
/// 消消乐里面的块,小动物,蔬菜,或是其他的东西
/// </summary>
[RequireComponent(typeof(Animation))]
public class Block : MonoBehaviour {


    public ColorType colorType;

    public Vector2Int pos;

    public int[] depths;  // 四条边上每个方向同色数量
    public bool[] isSameColor;     // 四条边是否和周围的同色

    public BombType bombType;           // 计算前的炸弹类型
    public BombType afterBombType;        // 计算后的炸弹类型

    public AnimationClip selectedClip;


    /// <summary>
    /// 是否被选中
    /// </summary>
    [SerializeField]
    private bool _selected;
    public bool selected {
        get { return _selected; }
    }

    private EventHandler selectEvent;
    private EventHandler deselectEvent;

    private Animation _anim;
    private SpriteRenderer _sr;

    private static Vector2Int[] neighboursOffset = new Vector2Int[] {
        Vector2Int.up,
        Vector2Int.right,
        Vector2Int.down,
        Vector2Int.left
    };



    void Awake(){
        _anim = GetComponent<Animation>();
        _sr = GetComponent<SpriteRenderer>();
        selectEvent += OnSelected;
        deselectEvent += OnDeselect;

        Reset();
    }

    private void Update() {
        transform.localPosition = new Vector3(pos.x, pos.y);
    }

    // 重置数据
    public void Reset() {
        _selected = false;
        isSameColor = new bool[4] { false, false, false, false };
        depths = new int[4] { 0, 0, 0, 0 };
        pos = Vector2Int.zero;
        name = "pooled" + colorType.ToString();

        bombType = BombType.None;
        afterBombType = BombType.None;
    }

    public void Select(){
        _selected = true;
        if(selectEvent!=null)
            selectEvent(this,EventArgs.Empty);
    }

    public void Deselect() {
        _selected = false;
        if (deselectEvent != null)
            deselectEvent(this, EventArgs.Empty);
    }

    // 动画控制
    public void StopPlayAnim() {

    }

    public void PlayeSelectAnim() {
        _anim.Play(selectedClip.name);
    }

    // Event
    public void OnSelected(object o,EventArgs e){
        PlayeSelectAnim();
    }

    public void OnDeselect(object o,EventArgs e) {
        StopPlayAnim();
    }


    // 炸弹类型
    public BombType CalcBombType() {
        // 五连
        if(depths[0] == 2 && depths[2] == 4){ bombType = BombType.SuperV;return bombType; }
        if(depths[1] == 2 && depths[3] == 4){ bombType = BombType.SuperH;return bombType; }
        // 圆炸
        if(depths[0] ==2 && depths[1] == 2){ bombType = BombType.Circle1;return bombType;}
        if(depths[1] ==2 && depths[2] == 2){ bombType = BombType.Circle2;return bombType;}
        if(depths[2] ==2 && depths[3] == 2){ bombType = BombType.Circle3;return bombType;}
        if(depths[3] ==2 && depths[0] == 2){ bombType = BombType.Circle4;return bombType;}
        // 线炸
        if(depths[0] ==1 && depths[2] == 2){ bombType = BombType.LineV;return bombType;}
        if(depths[1] ==1 && depths[3] == 2){ bombType = BombType.LineH;return bombType;}
        // 普通消失
        if(depths[0] ==1 && depths[2] == 1){ bombType = BombType.NormalV;return bombType;}
        if(depths[1] ==1 && depths[3] == 1){ bombType = BombType.NormalH;return bombType;}

        bombType = BombType.None;        return bombType;
    }

    /// <summary>
    /// 获取某个方向的邻居单元格
    /// </summary>
    /// <param name="dir"> 上下左右</param>
    /// <returns>Block,可能会为空,因为消除后空格上是没有Block的</returns>
    public Block GetNeighbour(Vector2Int dir) {
        if (CoordGrid.Instance.InBound(pos + dir))
            return CoordGrid.Instance.blocks[pos + dir];
        else
            return null;
    }
    public Block GetNeighbour(int idx) {
        return GetNeighbour(neighboursOffset[idx]);
    }

    // 检测邻居是否同色,填充 isSameColor数组
    public void CheckNeightbourColor() {
        Block b;
        for (int i = 0; i < 4; i++) {
            b = GetNeighbour(neighboursOffset[i]);
            if (b)
                isSameColor[i] = b.colorType == colorType;
            else
                isSameColor[i] = false;
        }
    }



    // 计算同色连体块的长度
    public void CheckNeighbours() {
        Block b;
        for (int i = 0; i < 4; i++) {
            b = GetNeighbour(i);
            if (b && b.colorType == colorType) {
                AddLength(i);
            }
        }

    }
    public void AddLength(int idx) {
        depths[idx] += 1;
        if (isSameColor[(idx+2)%4]) {
            GetNeighbour((idx + 2) % 4).AddLength(idx);
        }
    }


}
