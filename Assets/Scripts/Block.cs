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
/// 消消乐里面的块,小动物,蔬菜,或是其他能被消除的实体
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(Animation))]
public class Block : MonoBehaviour {

    public ColorType colorType;

    public Vector2Int pos;

    public int needFallHeight;

    [SerializeField]
    public int[] concolorLength;       // 四方向同色块数量,根据这个判断炸弹类型
    public bool[] isEdgeConcolor;      // 四条边是否和neighbour同色

    [SerializeField]
    private BombType _spawnBombType;           // 检查前的炸弹类型
    public BombType spawnBombType {
        get { return _spawnBombType; }
        set {
            _spawnBombType = value;
            
        }
    }
    [SerializeField]
    private BombType _bombType;      // 计算后的炸弹类型
    public BombType bombType {
        get {
            return _bombType;
        }
        set {
            _bombType = value;
            if (_bombType == BombType.SuperH ||
                _bombType == BombType.SuperV) {
                _spriteRenderer.sprite = sp_S;
                _animator.SetInteger("BombType", 4);
            } else if (_bombType == BombType.Circle1 ||
                         _bombType == BombType.Circle2 ||
                         _bombType == BombType.Circle3 ||
                         _bombType == BombType.Circle4) {
                _spriteRenderer.sprite = sp_N;
                _animator.SetInteger("BombType", 3);
            } else if (_bombType == BombType.LineH) {
                _spriteRenderer.sprite = sp_N;
                _animator.SetInteger("BombType", 2);
            } else if (_bombType == BombType.LineV) {
                _spriteRenderer.sprite = sp_N;
                _animator.SetInteger("BombType", 1);
            } else {
                _spriteRenderer.sprite = sp_N;
                _animator.SetInteger("BombType", 0);
            }
        }
    }

    [Header("资源")]
    public AnimationClip selectedClip;
    public Sprite sp_N;
    public Sprite sp_N2;
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
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;

    private static Vector2Int[] neighboursOffset = new Vector2Int[] {
        Vector2Int.up,
        Vector2Int.right,
        Vector2Int.down,
        Vector2Int.left
    };


    void Awake(){
        _anim = GetComponent<Animation>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponentInChildren <Animator>();
        selectEvent += OnSelected;
        deselectEvent += OnDeselect;
    }

    private void Update() {
        transform.localPosition = Vector3.MoveTowards(transform.localPosition,new Vector3(pos.x, pos.y),0.15f);
    }

    // 重置数据
    public void Reset() {
        _selected = false;
        isEdgeConcolor = new bool[4] { false,false,false,false };
        concolorLength = new int[4] { 0, 0, 0, 0 };
        //pos = Vector2Int.zero;
        pos = new Vector2Int(4,5);
        name = "pooled" + colorType.ToString();

        spawnBombType = BombType.None;
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
        if(concolorLength[0] == 2 && concolorLength[2] >= 2){ spawnBombType = BombType.SuperV;return spawnBombType; }
        if(concolorLength[3] == 2 && concolorLength[1] >= 2){ spawnBombType = BombType.SuperH;return spawnBombType; }
        // 圆炸
        if(concolorLength[0] >= 2 && concolorLength[1] >= 2){ spawnBombType = BombType.Circle1;return spawnBombType;}
        if(concolorLength[1] >= 2 && concolorLength[2] >= 2){ spawnBombType = BombType.Circle2;return spawnBombType;}
        if(concolorLength[2] >= 2 && concolorLength[3] >= 2){ spawnBombType = BombType.Circle3;return spawnBombType;}
        if(concolorLength[3] >= 2 && concolorLength[0] >= 2){ spawnBombType = BombType.Circle4;return spawnBombType;}
        // 线炸
        if(concolorLength[0] == 1 && concolorLength[2] == 2){ spawnBombType = BombType.LineV;return spawnBombType;}
        if(concolorLength[3] == 1 && concolorLength[1] == 2){ spawnBombType = BombType.LineH;return spawnBombType;}
        // 普通消失
        if(concolorLength[0] == 1 && concolorLength[2] == 1){ spawnBombType = BombType.NormalV;return spawnBombType;}
        if(concolorLength[1] == 1 && concolorLength[3] == 1){ spawnBombType = BombType.NormalH;return spawnBombType;}

        spawnBombType = BombType.None; return spawnBombType;
    }

    public void Clear() {
        switch (spawnBombType) {
            case BombType.SuperH: // 水平五连
                for(int i=0;i<concolorLength[3];i++){
                    GetNeighbour(Vector2Int.left,i+1).Bomb();
                }
                for(int i=0;i<concolorLength[1];i++){
                    GetNeighbour(Vector2Int.right,i+1).Bomb();
                }
                bombType = spawnBombType;
                spawnBombType = BombType.None;
                break;
            case BombType.SuperV:
                for(int i=0;i<concolorLength[0];i++){
                    GetNeighbour(Vector2Int.up,i+1).Bomb();
                }
                for(int i=0;i<concolorLength[2];i++){
                    GetNeighbour(Vector2Int.down,i+1).Bomb();
                }
                bombType = spawnBombType;
                spawnBombType = BombType.None;
                break;
            case BombType.Circle1:
                for(int i=0;i<concolorLength[0];i++){
                    GetNeighbour(Vector2Int.up,i+1).Bomb();
                }
                for(int i=0;i<concolorLength[1];i++){
                    GetNeighbour(Vector2Int.right,i+1).Bomb();
                }
                bombType = spawnBombType;
                spawnBombType = BombType.None;
                break;
            case BombType.Circle2:
                for(int i=0;i<concolorLength[2];i++){
                    GetNeighbour(Vector2Int.down,i+1).Bomb();
                }
                for(int i=0;i<concolorLength[1];i++){
                    GetNeighbour(Vector2Int.right,i+1).Bomb();
                }
                bombType = spawnBombType;
                spawnBombType = BombType.None;
                break;
            case BombType.Circle3:
                for(int i=0;i<concolorLength[2];i++){
                    GetNeighbour(Vector2Int.down,i+1).Bomb();
                }
                for(int i=0;i<concolorLength[3];i++){
                    GetNeighbour(Vector2Int.left,i+1).Bomb();
                }
                bombType = spawnBombType;
                spawnBombType = BombType.None;
                break;
            case BombType.Circle4:
                for(int i=0;i<concolorLength[0];i++){
                    GetNeighbour(Vector2Int.up,i+1).Bomb();
                }
                for(int i=0;i<concolorLength[3];i++){
                    GetNeighbour(Vector2Int.left,i+1).Bomb();
                }
                bombType = spawnBombType;
                spawnBombType = BombType.None;
                break;
            case BombType.LineH:
                for(int i=0;i<concolorLength[3];i++){
                    GetNeighbour(Vector2Int.left,i+1).Bomb();
                }
                for(int i=0;i<concolorLength[1];i++){
                    GetNeighbour(Vector2Int.right,i+1).Bomb();
                }
                bombType = spawnBombType;
                spawnBombType = BombType.None;
                break;
            case BombType.LineV:
                for(int i=0;i<concolorLength[0];i++){
                    GetNeighbour(Vector2Int.up,i+1).Bomb();
                }
                for(int i=0;i<concolorLength[2];i++){
                    GetNeighbour(Vector2Int.down,i+1).Bomb();
                }
                bombType = spawnBombType;
                spawnBombType = BombType.None;
                break;
            case BombType.NormalH:
                GetNeighbour(Vector2Int.left).Bomb();
                GetNeighbour(Vector2Int.right).Bomb();
                this.Bomb();
                break;
            case BombType.NormalV:
                GetNeighbour(Vector2Int.up).Bomb();
                GetNeighbour(Vector2Int.down).Bomb();
                this.Bomb();
                break;
            case BombType.None:
                break;
            default:
                break;
        }

    }

    public void Bomb(int ct = 0){
        //transform.localScale = Vector3.one*0.5f;
        // 爆炸
        // 根据after bomb type die其他
        List<Block> readyBombBlocks;
        switch (bombType) {
            case BombType.SuperH:
            case BombType.SuperV:
                readyBombBlocks = GetSameColorTypeBlocks(ct);
                foreach (var item in readyBombBlocks) {
                    item.Bomb();
                }
                break;
            case BombType.Circle1:
            case BombType.Circle2:
            case BombType.Circle3:
            case BombType.Circle4:
                // 圆形炸弹
                readyBombBlocks = GetBombCircleBlocks();
                foreach (var item in readyBombBlocks) {
                    item.Bomb();
                }
                break;
            case BombType.LineH:
                // 炸一列
                readyBombBlocks = GetBombColumnBlocks();
                foreach (var item in readyBombBlocks) {
                    item.Bomb();
                }
                break;
            case BombType.LineV:
                // 炸一行
                readyBombBlocks = GetBombRowBlocks();
                foreach (var item in readyBombBlocks) {
                    item.Bomb();
                }
                break;
            case BombType.NormalH:
            case BombType.NormalV:
            case BombType.None:
            default:
                break;
        }
        StartCoroutine( Die ());
    }

    public IEnumerator Die() {
        CoordGrid.Instance.blocks.Remove(pos);
        _animator.Play("Fire");
        yield return new WaitForSeconds(0.5f);
        BlockPool.Instance.Push(this);
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


    // 获取在自己爆炸半径内的Block
    public List<Block> GetBombCircleBlocks() {
        List<Block> result = new List<Block>();
        foreach (var item in CoordGrid.Instance.blocks) {
            int dis = (item.Value.pos - pos).ManhattanDistance();
            if (dis > 0 && dis <= 2) {
                result.Add(item.Value);
            }
        }
        return result;
    }
    // 同行
    public List<Block> GetBombRowBlocks() {
        List<Block> result = new List<Block>();
        foreach (var item in CoordGrid.Instance.blocks) {            
            if (item.Value.pos.x !=pos.x && item.Value.pos.y==pos.y) {
                result.Add(item.Value);
            }
        }
        return result;
    }

    // 同列
    public List<Block> GetBombColumnBlocks() {
        List<Block> result = new List<Block>();
        foreach (var item in CoordGrid.Instance.blocks) {
            if (item.Value.pos.y != pos.y && item.Value.pos.x == pos.x) {
                result.Add(item.Value);
            }
        }
        return result;
    }

    // 同色
    public List<Block> GetSameColorTypeBlocks(int ct) {
        List<Block> result = new List<Block>();
        foreach (var item in CoordGrid.Instance.blocks) {
            if((int)item.Value.colorType == ct && item.Value.pos != pos) {
                result.Add(item.Value);
            }
        }
        return result;
    }

    // 检测邻居是否同色,填充 isEdgeConcolor 数组
    public void CheckNeightbourColor() {
        Block b;
        for (int i = 0; i < 4; i++) {
            b = GetNeighbour(neighboursOffset[i]);
            if (b){
                isEdgeConcolor[i] = b.colorType == colorType;
            }else {
                isEdgeConcolor[i] = false;
            }
        }
    }

    public void ResetConcolor(){
        for (int i = 0; i < 4; i++) {
            concolorLength[i] = 0;
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

    // 
    public void AddConcolorLength(int dirIdx) {
        concolorLength[dirIdx] += 1;
        if (isEdgeConcolor[(dirIdx+2)%4] == true) { // 传递同色信息
            GetNeighbour((dirIdx+2)%4).AddConcolorLength(dirIdx);
        }
    }

    
    public void Fall() {
        if (needFallHeight != 0) {
            pos = pos + Vector2Int.down * needFallHeight;
            needFallHeight = 0;
        }
    }

    
}
