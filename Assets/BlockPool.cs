using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockPool : UnitySingleton<BlockPool> {

	[Header("#Pooled Prefab")]
	public Block blockPrefab;

	[Header("#Pooled Count")]
	[Range(100,1024)]public int MaxStored = 100;
	[Range(0,20)]public int MinStored = 10;

	public Stack<Block> remainBlocks;

	void Awake(){
		if (blockPrefab == null)
			return;
		remainBlocks = new Stack<Block>();
	}

	// 获取Block
	public Block Pop(){
		int count = remainBlocks.Count;
		if(count < MinStored){
			for (int i = 0; i <= MinStored - count; i++) {
				Block b = GameObject.Instantiate<Block>(blockPrefab) as Block;
				Push(b);
			}
		}
		Block pb = remainBlocks.Pop();
		pb.gameObject.SetActive(true);
		return pb;
	}

	// 归还BLock
	public void Push(Block b){
		if(remainBlocks.Count >= MaxStored){
			Destroy(b);
		}else{
			remainBlocks.Push(b);
			b.gameObject.SetActive(false);
			b.transform.Reset(transform);
		}
	}
}
