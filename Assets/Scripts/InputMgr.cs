﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputMgr : MonoBehaviour {

	void Update(){
		if(Input.GetMouseButtonDown(0)){
			RaycastHit2D hit = Physics2D.Raycast(
                Camera.main.ScreenToWorldPoint(Input.mousePosition),
                Vector2.zero);
			if(hit.collider!=null && hit.collider.gameObject.tag=="Block"){
                Block b = hit.collider.gameObject.GetComponent<Block>();
                if (b.selected) {
                    b.Deselect();
                    CoordGrid.Instance.currentSelectedBlock = null;
                } else {
                    if(CoordGrid.Instance.currentSelectedBlock)
                        CoordGrid.Instance.currentSelectedBlock.Deselect();
                    b.Select();
                    CoordGrid.Instance.currentSelectedBlock = b;
                }
                Debug.Log(b.depths[0].ToString() + "_"+
                    b.depths[1].ToString() + "_" +
                    b.depths[2].ToString() + "_" +
                    b.depths[3].ToString() 
                    );
				
			}			
		}
        if (Input.GetMouseButtonDown(1)) {
            Collider2D[] cols = Physics2D.OverlapPointAll(Camera.main.ScreenToWorldPoint(Input.mousePosition),1<<LayerMask.NameToLayer("Block"));
            if (cols.Length > 0) {
                foreach (var item in cols) {
                    Debug.Log(item.name);
                }
            }
        }
	}
}