using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// シーン遷移時のフェードイン・フェードアウトの制御を行うクラス
public class FadeManager : SingletonBase<FadeManager>
{
    // 暗転用黒テクスチャ
    private Texture2D blackTexture;

    // フェード中の透明度
    private float fadeAlpha = 0.0f;

    // フェード中か否か
    private bool isFading = false;

	Coroutine col = null;


    public void Awake()
    {
		StartCoroutine ("awake");
    }

	IEnumerator awake(){
		if(this != Instance)
		{
			Destroy(this);
 			yield return null;
		}

		DontDestroyOnLoad(this.gameObject);

		// 黒テクスチャの生成
		this.blackTexture = new Texture2D(32, 32, TextureFormat.RGB24, false);
		yield return new WaitForEndOfFrame();
		this.blackTexture.ReadPixels(new Rect(0, 0, 32, 32), 0, 0, false);
		this.blackTexture.SetPixel(0, 0, Color.white);
		this.blackTexture.Apply();
	}

    public void OnGUI()
    {
        if(!this.isFading)
        {
            return;
        }

        // 透明度を更新して黒テクスチャを描画
        GUI.color = new Color(0, 0, 0, this.fadeAlpha);
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), this.blackTexture);
    }

    /// <summary>
    /// 画面遷移
    /// </summary>
    /// <param name='scene'>シーン名</param>
    /// <param name='interval'>暗転にかかる時間(秒)</param>
    public void LoadLevel(string scene, float interval)
    {
		StartCoroutine(TransScene(scene, interval));
    }
	public void LoadLevelAsync(string scene, float interval)
	{
		if (col == null) {
			col = StartCoroutine (TransSceneAsync (scene, interval));
		}
	}

    /// <summary>
    /// シーン遷移用コルーチン
    /// </summary>
    /// <param name='scene'>シーン名</param>
    /// <param name='interval'>暗転にかかる時間(秒)</param>
    private IEnumerator TransScene(string scene, float interval)
    {
        //だんだん暗く
        this.isFading = true;
        float time = 0;
        while (time <= interval)
        {
            this.fadeAlpha = Mathf.Lerp(0f, 1f, time / interval);
            time += Time.deltaTime;
            yield return 0;
        }

        //シーン切替
        SceneManager.LoadScene(scene);

        //だんだん明るく
        time = 0;
        while (time <= interval)
        {
            this.fadeAlpha = Mathf.Lerp(1f, 0f, time / interval);
            time += Time.deltaTime;
            yield return 0;
        }

        this.isFading = false;
		col = null;
    }
	private IEnumerator TransSceneAsync (string scene, float interval){
		//だんだん暗く
		this.isFading = true;
		float time = 0;
		while (time <= interval)
		{
			this.fadeAlpha = Mathf.Lerp(0f, 1f, time / interval);
			time += Time.deltaTime;
			yield return 0;
		}

		AsyncOperation async = Application.LoadLevelAsync(scene);
		async.allowSceneActivation = false;    // シーン遷移をしない

		while (async.progress < 0.9f) {
			yield return new WaitForEndOfFrame();
		}

		yield return new WaitForSeconds(1);

		async.allowSceneActivation = true;    // シーン遷移許可

		//だんだん明るく
		time = 0;
		while (time <= interval)
		{
			this.fadeAlpha = Mathf.Lerp(1f, 0f, time / interval);
			time += Time.deltaTime;
			yield return 0;
		}

		this.isFading = false;
		col = null;
	}
}