using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputMgr : MonoBehaviour {

	void Update(){
		if(Input.GetMouseButton(0)){

			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			Debug.Log("按下左键" + Input.mousePosition.ToString());
			RaycastHit hit;
			if(Physics.Raycast(ray,out hit)){
				Debug.Log (hit.collider.gameObject.name);
				if(hit.collider.gameObject.tag == "Block"){
					hit.collider.gameObject.GetComponent<Block>().Select();
					Debug.Log(hit.collider.gameObject.name);
				}
			}
		}
	}
}
