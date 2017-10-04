using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class async : MonoBehaviour {

	// Use this for initialization
	void Start () {
		FadeManager fm = GameObject.Find ("FadeManager").GetComponent<FadeManager>();
		fm.LoadLevelAsync ("main", 1f);
	}
}
