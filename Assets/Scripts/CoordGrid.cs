using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//[ExecuteInEditMode]
[SelectionBase]
public class CoordGrid : MonoBehaviour {

    [Header("# Size")]
    public int width;
    public int height;

    [Header("# ResRef")]
    public GameObject cellPrefab;
    public Block blockPrefab;
    public Sprite lightSprite;
    public Sprite darkSprite;

    public Sprite[] blockSprites;

    [Header("# Variant")]
    private List<Transform> cells; // 背景块引用
    private List<Block> blocks;


    private Vector3 origin{
        get{
            return new Vector3(-(float)width*0.5f+0.5f,-(float)height*0.5f+0.5f);
        }
    }

    private void Awake() {
        transform.localPosition = origin;
        cells = new List<Transform>();
        blocks = new List<Block>();
        InitCoordGrid();
        Spwan();
    }
    private void Update() {
        //if(width*height != cells.Count) {
        //    Debug.Log(width * height);
        //    InitCoordGrid();
        //}
    }

    private void InitCoordGrid() {
        ClearGrid();
        CreateGrid();
    }

    private void ClearGrid() {
        for (int i = cells.Count-1; i >= 0 ; i--) {
            DestroyImmediate(cells[i].gameObject);
        }
        cells.Clear();
    }

    private void Start() {

    }

    void CreateCell(int x,int y,bool lord,Transform parent) {
        GameObject go = Instantiate(cellPrefab);
        go.GetComponent<SpriteRenderer>().sprite = lord ? lightSprite : darkSprite;
        go.transform.SetParent(parent,false);
        go.transform.localPosition = new Vector3(x, y, 0);
        cells.Add(go.transform);
    }

    void CreateGrid() {
        bool lord = false;
        for (int y = 0, i = 0; y < height; y++) {
            GameObject parent = new GameObject(){
                name = "row_" + y.ToString()
            };
            parent.transform.SetParent(transform,false);
            parent.transform.Reset();

            for (int x = 0; x < width; x++,i++) {
                lord = Convert.ToBoolean((x + y) & 1);
                CreateCell(x, y, lord,parent.transform);
            }
        }
    }

    // 生成关卡
    void Spwan() {
        GameObject parent = new GameObject() {
            name = "BlockHolder",
        };
        parent.transform.localPosition = origin-Vector3.forward;
        for (int x = 0,i = 0; x < width; x++) {
            for (int y = 0; y < height; y++,i++) {
                SpawnBlock(x, y, i ,parent);
            }
        }
    }

    void SpawnBlock(int x,int y,int i,GameObject p) {
        int idx = UnityEngine.Random.Range(0,blockSprites.Length);
        // Block go = Instantiate(blockPrefab) as Block;
        Block b = BlockPool.Instance.Pop();
        b.GetComponent<SpriteRenderer>().sprite = blockSprites[idx];
        //go.transform.SetParent(transform);
        b.transform.Reset(p.transform);
        b.pos = new Vector2Int(x, y);
    }


}
