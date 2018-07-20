using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;


public delegate void SelectedBlockChangeHandler(Vector2Int selectPos);


[SelectionBase, DisallowMultipleComponent]
public class CoordGrid : UnitySingleton<CoordGrid> {

    [Header("# Size")]
    public int width;
    public int height;

    /// <summary>
    /// 可用的格子列表(后面场景编辑器就是编辑这个)
    /// 会用作大部分对象字典的key值
    /// </summary>
    public List<Vector2Int> coords;

    [Header("# ResRef")]
    public Cell cellPrefab;         // 单元格
    public Block blockPrefab;       // Block对象

    public GameObject selectedBlockBorderPrefab;

    public GameObject blocksHolder;

    // 变量

    public Dictionary<Vector2Int, Block> blocks;  // Block 字典,key是坐标1
    public Dictionary<Vector2Int, Cell> cells; // Cell字典
    //private int[,] matrix;              // 矩阵信息

    private GameObject selectedBlockBorder; // 边框物体引用

    // Event
    private SelectedBlockChangeHandler SelectedBlockChange;



    private Block _currentSelectedBlock; // 当前选择对象的引用
    public Block currentSelectedBlock {
        get {
            return _currentSelectedBlock;
        }
        set {
            _currentSelectedBlock = value;

            if (SelectedBlockChange != null)
                if (_currentSelectedBlock != null)
                    SelectedBlockChange(_currentSelectedBlock.pos);
                else
                    SelectedBlockChange(Vector2Int.one*10000); // 放在外地看不见
        }
    }

    /// <summary>
    /// 父物体中心点
    /// </summary>
    private Vector3 origin{
        get{
            return new Vector3(-(float)width*0.5f+0.5f,-(float)height*0.5f+0.5f);
        }
    }



    // 初始化场景坐标信息
    void CreateCoords() {
        coords.Clear();
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                coords.Add(new Vector2Int(x, y));
            }
        }
        //for (int i = 0; i < 6; i++) {
        //    coords.RemoveAt(Random.Range(0,coords.Count));
        //}
    }

    /// <summary>
    /// 创建格子
    /// </summary>
    void CreateGrid() {
        bool lord = false;
        
        cells.Clear();
        foreach (var coord in coords) {
            lord = Convert.ToBoolean((coord.x + coord.y) & 1);
            CreateCell(coord, lord, transform);
        }

    }

    /// <summary>
    /// 创建格子单元格
    /// </summary>
    /// <param name="coord">格子坐标</param>
    /// <param name="lord">light or dark</param>
    /// <param name="parent">父物体</param>
    void CreateCell(Vector2Int coord,bool lord,Transform parent) {
        Cell c = Instantiate(cellPrefab) as Cell;
        c.lord = lord;
        c.transform.SetParent(parent,false);
        c.pos = coord;
        cells.Add(coord,c);
    }

    /// <summary>
    /// 用Block填充场景
    /// </summary>
    void CreateBlocks() {
        blocks.Clear();
        if (blocksHolder == null) {
            blocksHolder = new GameObject() {
                name = "BlockHolder",
            };
        }
        blocksHolder.transform.localPosition = origin;
        foreach (var coord in coords) {
            SpawnBlock(coord, blocksHolder.transform);
        }

    }

    /// <summary>
    /// 填充每个单元格
    /// </summary>
    /// <param name="coord">格子坐标</param>
    /// <param name="p">父物体</param>
    void SpawnBlock(Vector2Int coord,Transform p) {

        Block b = BlockPool.Instance.RandomPop();

        b.transform.Reset(p);
        b.pos = coord;
        b.name = coord.ToString();
        blocks.Add(b.pos, b);
        //matrix[x, y] = (int)b.colorType;
    }

    public bool InBound(Vector2Int pos) {
        return coords.Contains(pos);
    }

    public void CheckBlocks() {
        Debug.Log("开始遍历检查小动物" + blocks.Count.ToString());
        foreach (var b in blocks) {
            b.Value.CheckNeightbourColor();
        }

        foreach (var b in blocks) {
            b.Value.CalcConcolorLength();
        }
        foreach (var b in blocks) {
            b.Value.CalcBombType();
        }
    }


    public void ReSpawnBlocks() {
        if (currentSelectedBlock) {
            currentSelectedBlock.Deselect();
        }
        foreach (var item in blocks) {
            BlockPool.Instance.Push(item.Value,(int)item.Value.colorType);
        }

        //Debug.Break();

        CreateBlocks();

    }


    /// <summary>
    /// 选择框改变事件
    /// </summary>
    /// <param name="pos">选择框的格子坐标</param>
    public void OnSelectedBlockChange(Vector2Int pos) {
        Debug.Log("Set Selected");
        if (selectedBlockBorder == null) {
            selectedBlockBorder = Instantiate(selectedBlockBorderPrefab);
        }
        selectedBlockBorder.transform.SetParent(transform);
        selectedBlockBorder.transform.localPosition = new Vector3(pos.x, pos.y);
    }

    //// Unity消息

    private void Awake() {
        SelectedBlockChange += OnSelectedBlockChange;
        transform.localPosition = origin;
        coords = new List<Vector2Int>();
        cells = new Dictionary<Vector2Int, Cell>();
        blocks = new Dictionary<Vector2Int, Block>();
        CreateCoords();
        CreateGrid();
        CreateBlocks();
        
        for (int i = 0; i < 10; i++) {
            bombsBlocks[i] = new List<Block>();
        }
    }


    public List<Block>[] bombsBlocks = new List<Block>[10];
    public void GetAllBombs() {
        for (int i = 0; i < 10; i++) {
            bombsBlocks[i].Clear();
        }
        foreach (var b in blocks.Values) {
            if (b.bombType != BombType.None) {
                bombsBlocks[(int)b.CalcBombType()].Add(b);
            }
        }
    }
    void Bombs() {
        GetAllBombs();
        foreach (var item in bombsBlocks[(int)BombType.NormalV]) {
            Block[] db = new Block[3];
            db[0] = item.GetNeighbour(2);
            db[1] = item.GetNeighbour(0);
            db[2] = item;

            for (int i = 0; i < 3; i++) {
                blocks.Remove(db[i].pos);
                BlockPool.Instance.Push(db[i]);
            }

            item.afterBombType = item.bombType;
            item.bombType = BombType.None;
        }

        foreach (var item in bombsBlocks[(int)BombType.NormalH]) {

            Block[] db = new Block[3];
            db[0] = item.GetNeighbour(1);
            db[1] = item.GetNeighbour(3);
            db[2] = item;

            for (int i = 0; i < 3; i++) {
                if (db[i]) {
                    blocks.Remove(db[i].pos);
                    BlockPool.Instance.Push(db[i]);
                }
                // db[i].transform.localScale = Vector3.one*0.1f;
            }

            item.afterBombType = item.bombType;
            item.bombType = BombType.None;
        }
    }
}
