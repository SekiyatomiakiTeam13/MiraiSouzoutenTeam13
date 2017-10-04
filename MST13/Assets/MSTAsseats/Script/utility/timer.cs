using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class timer : MonoBehaviour {

	static public 	float 		timeNum;
	public Text		timeText;

	// Use this for initialization
	void Start () {
		Application.targetFrameRate = 60;
		timeNum = 0;
	}
	
	// Update is called once per frame
	void Update () {
		timeNum += Time.deltaTime;
		timeText.text = ((int)(timeNum)).ToString ();
	}
}
