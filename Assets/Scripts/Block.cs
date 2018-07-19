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
    None  , // 没有炸弹
    Normal, // 正常
    Super,  // 变色炸弹
    HLine,  // 水平炸弹
    VLine,  // 垂直炸弹
    Circle  // 圆形炸弹
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

    public BombType bombType;

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
        selectedClip.wrapMode = WrapMode.Once;
        _anim.Stop(selectedClip.name);
    }

    public void PlayeSelectAnim() {
        selectedClip.wrapMode = WrapMode.Loop;
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
    public BombType CheckBombType() {

        if(depths[0] + depths[2] >=4 || depths[1]+depths[3]>=4) {
            //万能炸弹
            bombType = BombType.Super;
            return bombType;
        }
        if(depths[0]+depths[1] >=4 || 
            depths[1] + depths[2] >= 4|| 
            depths[2] + depths[3] >= 4|| 
            depths[3] + depths[0] >= 4) {
            bombType = BombType.Circle;
            return bombType;
        }
        if (depths[0] + depths[2] >= 3 ) {
            // 炸一列
            bombType = BombType.VLine;
            return bombType;
        }
        if( depths[1] + depths[3] >= 3)        {
            // 炸一排
            bombType = BombType.HLine;
            return bombType;
        }
        if (depths[0] + depths[2] >= 2 ||
            depths[1] + depths[3] >= 2 ||
            depths[2] + depths[0] >= 2 ||
            depths[3] + depths[1] >= 2) {
            // 正常消
            bombType = BombType.Normal;
            return bombType;
        }
        bombType = BombType.None;
        return bombType;
    }

    /// <summary>
    /// 获取某个方向的邻居单元格
    /// </summary>
    /// <param name="dir">v2int 上下左右</param>
    /// <returns>Block</returns>
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
