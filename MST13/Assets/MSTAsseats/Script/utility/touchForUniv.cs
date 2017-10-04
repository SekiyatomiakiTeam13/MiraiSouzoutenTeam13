using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class touchForUniv : MonoBehaviour {
	static private bool bDown;
	static private bool bUp;
	static private bool bTouch;
	void Update(){
		bUp = false;
		bDown = false;
		if (Input.GetMouseButtonDown (0)) {
			bTouch = true;
			bDown = true;
		}
		if (Input.GetMouseButtonUp (0)) {
			bTouch = false;
			bUp = true;
		}
		//タッチがあるかどうか？
		if (Input.touchCount > 0){
			Touch touch = Input.GetTouch(0);
			//タッチ直後
			if(touch.phase == TouchPhase.Began){
				bTouch = true;
				bDown = true;
			}
			if(touch.phase == TouchPhase.Ended){
				bTouch = false;
				bUp = true;
			}
		}
	}	
	static public bool OnTap(){
		return bDown;
	}
	static public bool OnTouch(){
		return bTouch;
	}
	static public bool OnUp(){
		return bUp;
	}
}
