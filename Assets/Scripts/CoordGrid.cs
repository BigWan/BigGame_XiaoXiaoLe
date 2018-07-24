using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using System.Linq;


public delegate void SelectedBlockChangeHandler(Vector2Int selectPos);


[SelectionBase, DisallowMultipleComponent]
public class CoordGrid : UnitySingleton<CoordGrid> {

    [Header("# Size")]
    public int width;
    public int height;

    /// <summary>
    /// 可用的格子列表(场景编辑器就是编辑这个)
    /// 会用作大部分对象字典的key值
    /// </summary>
    public List<Vector2Int> coords;

    [Header("# ResRef")]
    public Cell cellPrefab;         // 单元格
    public Block blockPrefab;       // Block对象

    public GameObject selectedBlockBorderPrefab;

    public GameObject blocksHolder;

    public Queue<Vector2Int> selectedBlocks = new Queue<Vector2Int>();
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
    public Vector3 origin{
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
        Block b = BlockPool.Instance.RandomPopAt(coord,p);
        //b.Reset();
        //b.transform.Reset(p);
        b.transform.localPosition = new Vector3(coord.x, height);
        b.pos = coord;
        b.name = coord.ToString();
        blocks.Add(b.pos, b);
        // Debug.Log("Spawn" + b.name);
        //matrix[x, y] = (int)b.colorType;
    }

    public bool InBound(Vector2Int pos) {
        return coords.Contains(pos);
    }




    // 获取炸弹并爆炸
    public void ClearAll(){
        CheckBlocks();
        ClearSameBlockAndSpawnBomb();
    }

    /// <summary>
    ///  重新生成小动物
    /// </summary>
    public void ReSpawnBlocks() {
        if (currentSelectedBlock) {
            currentSelectedBlock.Deselect();
        }
        foreach (var item in blocks) {
            BlockPool.Instance.Push(item.Value,(int)item.Value.colorType);
        }

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

        // 炸弹引用列表
        for (int i = 0; i < 10; i++) {
            spawnBombBlock[i] = new List<Block>();
        }
    }


    public List<Block>[] spawnBombBlock = new List<Block>[10];
    /// <summary>
    /// 检查小动物是否能形成炸弹
    /// </summary>
    public void CheckBlocks() {
        Debug.Log("==开始遍历检查小动物" + blocks.Count.ToString());

        // 调查邻边
        foreach (var b in blocks) {
            b.Value.CheckNeightbourColor();
        }

        foreach (var b in blocks) {
            b.Value.ResetConcolor();
        }
        // 计算同色长度
        foreach (var b in blocks) {
            b.Value.CalcConcolorLength();
        }
        // 预先统计炸弹类型
        foreach (var b in blocks) {
            b.Value.CalcBombType();
        }

        // 统计炸弹
        for (int i = 0; i < 10; i++) {
            spawnBombBlock[i].Clear();
        }
        foreach (var b in blocks.Values) {
            if (b.spawnBombType != BombType.None) {
                spawnBombBlock[(int)b.CalcBombType()].Add(b);
            }
        }
    }


    /// <summary>
    /// 计算当前场面上需要消除的东西
    /// </summary>
    /// <returns></returns>
    public int GetSpawnBombCount() {
        CheckBlocks();

        int spawnCount = 0;
        for (int i = 0; i < 10; i++) {
            spawnCount += spawnBombBlock[i].Count;
        }
        return spawnCount;
    }

    /// <summary>
    /// 消除可以炸掉的敌方，并在需要生成炸弹的敌方生成炸弹
    /// </summary>
    void ClearSameBlockAndSpawnBomb() {
        for (int i = 0; i < 10; i++) {
            if(spawnBombBlock[i].Count > 0){
                foreach (var item in spawnBombBlock[i]) {
                    item.Clear();
                }
                CheckBlocks();
            }
        }
    }


    /// <summary>
    /// 获取空洞
    /// </summary>
    /// <returns></returns>
    private List<Vector2Int> GetHoles() {
        List<Vector2Int> holes = new List<Vector2Int>();  //空格
        foreach (var coord in coords) {
            if (!blocks.ContainsKey(coord)) {
                holes.Add(coord);
            }
        }
        return holes;
    }

    // 下落并生成随机块填充
    public void FallDown(){
        // 获取空洞
        List<Vector2Int> holes = GetHoles();  //空格

        Debug.Log("空洞数量：" + holes.Count);

        foreach (var hole in holes) {
            foreach (var block in blocks.Values) {
                if(block.pos.x == hole.x && block.pos.y > hole.y){
                    block.needFallHeight+=1;
                }
            }
        }

        foreach (var block in blocks.Values) {
            block.Fall();
        }


        Dictionary<Vector2Int, Block> newdic = new Dictionary<Vector2Int, Block>();
        foreach (var item in blocks) {
            newdic.Add(item.Value.pos, item.Value);
        }
        blocks.Clear();
        blocks = newdic;

        holes.Clear();
        foreach (var coord in coords) {
            if (!blocks.ContainsKey(coord)) {
                holes.Add(coord);  
            }
        }

        foreach (var hole in holes) {
            // 生成随机的块
            SpawnBlock(hole, transform);
        }
    }


    /// <summary>
    /// 开始消除
    /// </summary>
    public IEnumerator StartClear() {
        int count = 0;
        int spawnCount = GetSpawnBombCount();
        while(spawnCount > 0) {
            count += 1;
            ClearSameBlockAndSpawnBomb();
            yield return new WaitForSeconds(0.5f);
            FallDown();
            yield return new WaitForSeconds(1.5f);
            spawnCount = GetSpawnBombCount();
        }

        Debug.Log( "发生这么多次变换" + count);
       
    }


    public void Startt() {
        StartCoroutine(StartClear());
    }

    /// <summary>
    /// 交换
    /// </summary>
    /// <param name="a">一个格子坐标</param>
    /// <param name="b">另一个格子坐标</param>
    public IEnumerator Switch(Vector2Int a, Vector2Int b) {

        (blocks[a], blocks[b]) = (blocks[b], blocks[a]);
        (blocks[a].pos,blocks[b].pos) = (blocks[b].pos, blocks[a].pos); 
       

        if (GetSpawnBombCount() > 0) {
            StartCoroutine(StartClear());
        } else {
            yield return new WaitForSeconds(0.25f);
            (blocks[a], blocks[b]) = (blocks[b], blocks[a]);
            (blocks[a].pos, blocks[b].pos) = (blocks[b].pos, blocks[a].pos);
        }
    }


    public void AddSelectedBlock(Vector2Int x) {
        selectedBlocks.Enqueue(x);
        if (selectedBlocks.Count >= 2) {
            StartCoroutine(Switch(selectedBlocks.Dequeue(), selectedBlocks.Dequeue()));
        }
    }



}
