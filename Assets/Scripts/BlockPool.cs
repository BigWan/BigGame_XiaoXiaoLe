﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockPool : UnitySingleton<BlockPool> {

	[Header("#Pooled Prefab")]
	public Block[] blockPrefabs;

	[Header("#Pooled Count")]
	[Range(30,100)]public int MaxStored = 30;
	[Range(0,20)]public int MinStored = 10;

	public Stack<Block>[] remainBlocks;

	void Awake(){
		if (blockPrefabs.Length == 0)
			return;
		remainBlocks = new Stack<Block>[blockPrefabs.Length];
        for (int i = 0; i < blockPrefabs.Length; i++) {
            remainBlocks[i] = new Stack<Block>();
        }
	}

    /// <summary>
    /// 从池里获取一个Block
    /// </summary>
    /// <param name="idx">Block的颜色ID</param>
    /// <returns>一个block实例,获取后需要重新设置transform</returns>
    public Block Pop(int idx){
		int count = remainBlocks[idx].Count;
		if(count < MinStored){
			for (int i = 0; i <= MinStored - count; i++) {
				Block b = Instantiate<Block>(blockPrefabs[idx]) as Block;
				Push(b,idx);
			}
		}
		Block pb = remainBlocks[idx].Pop();
		pb.gameObject.SetActive(true);
		return pb;
	}

    /// <summary>
    /// 从池里随机获取一个Block
    /// </summary>
    /// <returns> 一个block实例,获取后需要重新设置transform </returns>
    public Block RandomPop() {
        int idx = Random.Range(0, blockPrefabs.Length);
        return Pop(idx);
    }

	/// <summary>
    /// 销毁一个Block对象
    /// </summary>
    /// <param name="b">Block对象</param>
    /// <param name="idx">Block对象的idx</param>
	public void Push(Block b,int idx){
		if(remainBlocks[idx].Count >= MaxStored){
			Destroy(b);
		}else{
			remainBlocks[idx].Push(b);
			b.gameObject.SetActive(false);
			b.transform.Reset(transform);
            b.Reset();
		}
	}
}