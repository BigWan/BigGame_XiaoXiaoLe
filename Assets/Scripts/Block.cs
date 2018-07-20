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
[DisallowMultipleComponent]
[RequireComponent(typeof(Animation))]
public class Block : MonoBehaviour {


    public ColorType colorType;

    public Vector2Int pos;

    public int[] concolorLength;       // 四方向同色块数量,根据这个判断炸弹类型
    public bool[] isEdgeConcolor;      // 四条边是否和neighbour同色

    [NonSerialized]
    private BombType _bombType;           // 检查前的炸弹类型
    public BombType bombType {
        get { return _bombType; }
        set {
            _bombType = value;
            switch (_bombType) {
                case BombType.SuperH:
                case BombType.SuperV:
                    _sr.sprite = sp_S;
                    break;
                case BombType.Circle1:
                case BombType.Circle2:
                case BombType.Circle3:
                case BombType.Circle4:
                    _sr.sprite = sp_C;
                    break;
                case BombType.LineH:
                    _sr.sprite = sp_V;
                    break;
                case BombType.LineV:
                    _sr.sprite = sp_V;
                    break;
                case BombType.NormalH:
                case BombType.NormalV:
                case BombType.None:
                    _sr.sprite = sp_N2;
                    break;
                default:
                    break;
            }
        }
    }
    public BombType afterBombType;      // 计算后的炸弹类型

    [Header("资源")]
    public AnimationClip selectedClip;
    public Sprite sp_N;
    public Sprite sp_N2;
    public Sprite sp_H;
    public Sprite sp_V;
    public Sprite sp_C;
    public Sprite sp_S;


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

        // Reset();
    }

    private void Update() {
        transform.localPosition = new Vector3(pos.x, pos.y);
    }

    // 重置数据
    public void Reset() {
        _selected = false;
        isEdgeConcolor = new bool[4] { false, false, false, false };
        concolorLength = new int[4] { 0, 0, 0, 0 };
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
        if(concolorLength[0] == 2 && concolorLength[2] >= 2){ bombType = BombType.SuperV;return bombType; }
        if(concolorLength[3] == 2 && concolorLength[1] >= 2){ bombType = BombType.SuperH;return bombType; }
        // 圆炸
        if(concolorLength[0] >= 2 && concolorLength[1] >= 2){ bombType = BombType.Circle1;return bombType;}
        if(concolorLength[1] >= 2 && concolorLength[2] >= 2){ bombType = BombType.Circle2;return bombType;}
        if(concolorLength[2] >= 2 && concolorLength[3] >= 2){ bombType = BombType.Circle3;return bombType;}
        if(concolorLength[3] >= 2 && concolorLength[0] >= 2){ bombType = BombType.Circle4;return bombType;}
        // 线炸
        if(concolorLength[0] == 1 && concolorLength[2] == 2){ bombType = BombType.LineV;return bombType;}
        if(concolorLength[3] == 1 && concolorLength[1] == 2){ bombType = BombType.LineH;return bombType;}
        // 普通消失
        if(concolorLength[0] == 1 && concolorLength[2] == 1){ bombType = BombType.NormalV;return bombType;}
        if(concolorLength[1] == 1 && concolorLength[3] == 1){ bombType = BombType.NormalH;return bombType;}

        bombType = BombType.None; return bombType;
    }

    public void Bomb() {
        switch (bombType) {
            case BombType.SuperH: // 垂直五连
                bombType = BombType.None;

                break;
            case BombType.SuperV:
                break;
            case BombType.Circle1:
                break;
            case BombType.Circle2:
                break;
            case BombType.Circle3:
                break;
            case BombType.Circle4:
                break;
            case BombType.LineH:
                break;
            case BombType.LineV:
                break;
            case BombType.NormalH:
                break;
            case BombType.NormalV:
                break;
            case BombType.None:
                break;
            default:
                break;
        }
    }

    // 方块移动并合成炸弹
    public void Disappear(Vector2Int pos) {

    }

    /// <summary>
    /// 获取某个方向的邻居单元格
    /// </summary>
    /// <param name="dir"> 上下左右 </param>
    /// <returns>Block,可能会为空,因为消除后空格上是没有Block的</returns>
    public Block GetNeighbour(Vector2Int dir) {
        return GetNeighbour(dir, 1);
    }
    public Block GetNeighbour(int idx) {
        return GetNeighbour(neighboursOffset[idx], 1);
    }
    public Block GetNeighbour(int idx,int dis) {
        return GetNeighbour(neighboursOffset[idx],dis);
    }
    public Block GetNeighbour(Vector2Int dir, int distance) {
        Vector2Int npos = pos + dir * distance;
        if (CoordGrid.Instance.blocks.ContainsKey(npos))
            return CoordGrid.Instance.blocks[npos];
        return null;
    }

    // 检测邻居是否同色,填充 isEdgeConcolor 数组
    public void CheckNeightbourColor() {
        Block b;
        for (int i = 0; i < 4; i++) {
            b = GetNeighbour(neighboursOffset[i]);
            if (b)
                isEdgeConcolor[i] = b.colorType == colorType;
            else
                isEdgeConcolor[i] = false;
        }
    }


    // 计算同色块的长度
    public void CalcConcolorLength() {
        Block b;
        for (int i = 0; i < 4; i++) {
            b = GetNeighbour(i);
            if (b && b.colorType == colorType) {
                AddConcolorLength(i);
            }
        }
    }

    public void AddConcolorLength(int dirIdx) {
        concolorLength[dirIdx] += 1;
        if (isEdgeConcolor[(dirIdx+2)%4]) {
            GetNeighbour((dirIdx+2)%4).AddConcolorLength(dirIdx);
        }
    }



}
