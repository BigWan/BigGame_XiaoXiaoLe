using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAnimation : MonoBehaviour {

	public AnimationClip v;
	public AnimationClip h;
	public AnimationClip circle;

	public Animation _anim;

	private void Awake() {
		_anim = GetComponent<Animation>();
	}

	public void PlayV(){
		_anim.CrossFade(v.name);
	}
	public void PlayH(){
		_anim.CrossFade(h.name);
	}

	public void PlayC(){
		_anim.CrossFade(circle.name);
	}
}
