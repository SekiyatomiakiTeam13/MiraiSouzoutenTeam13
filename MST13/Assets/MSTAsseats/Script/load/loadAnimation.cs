using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class loadAnimation : MonoBehaviour {
	[SerializeField]
	Sprite[] 	images;
	[SerializeField]
	float		waitTime = 0;

	int			imageIndex = 0;

	Image		img;
	// Use this for initialization
	void Start () {
		imageIndex 	= 0;
		img = GetComponent<Image> ();
		StartCoroutine ("changeSprite");
	}
	
	// Update is called once per frame
	void Update () {
		if (touchForUniv.OnTouch()) {
			waitTime = 0.05f;
		}
		if (touchForUniv.OnUp()) {
			waitTime = 1f;
		}
	}
	IEnumerator changeSprite(){
		img.sprite = images [imageIndex];
		imageIndex++;

		if (imageIndex == images.Length) {
			imageIndex = 0;
		}

		yield return new WaitForSeconds (waitTime);

		StartCoroutine ("changeSprite");
	}
}
