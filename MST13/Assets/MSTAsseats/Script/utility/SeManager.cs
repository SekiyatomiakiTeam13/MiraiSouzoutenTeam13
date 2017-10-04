using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// SEの再生、停止を制御します。
public class SeManager : MonoBehaviour
{

    #region Singleton

    private static SeManager instance;

    public static SeManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = (SeManager)FindObjectOfType(typeof(SeManager));

                if (instance == null)
                {
                    Debug.LogError(typeof(SeManager) + "is nothing");
                }
            }

            return instance;
        }
    }

    #endregion Singleton

    /// デバッグモード
    public bool DebugMode = true;

    /// 最大同時再生数
    public int MaxAudioSources = 10;

    /// SE再生音量
    [Range(0f, 1f)]
    public float Volume = 1.0f;
    private List<AudioSource> AudioSources = null;

    /// 再生可能なSE(AudioClip)のリストです。
    /// 実行時に Resources/Audio/SE フォルダから自動読み込みされます。
    private Dictionary<string, AudioClip> AudioClipDict = null;

    public void Awake()
    {
        //シングルトンのためのコード
        if (this != Instance)
        {
            Destroy(this.gameObject);
            return;
        }
        DontDestroyOnLoad(this.gameObject);

        this.AudioSources = new List<AudioSource>();

        //[Resources/Audio/BGM]フォルダからBGMを探す
        this.AudioClipDict = new Dictionary<string, AudioClip>();
        foreach (AudioClip bgm in Resources.LoadAll<AudioClip>("Sounds/SE"))
        {
            this.AudioClipDict.Add(bgm.name, bgm);
        }

        //有効なAudioListenerが一つも無い場合は生成する。（大体はMainCameraについてる）
        if (FindObjectsOfType(typeof(AudioListener)).All(o => !((AudioListener)o).enabled))
        {
            this.gameObject.AddComponent<AudioListener>();
        }
    }

    /// デバッグ用操作パネルを表示
    public void OnGUI()
    {
        if (this.DebugMode)
        {
            //AudioClipが見つからなかった場合
            if (this.AudioClipDict.Count == 0)
            {
                GUI.Box(new Rect(10, 10, 200, 50), "SE Manager(Debug Mode)");
                GUI.Label(new Rect(10, 35, 1000, 20), "Audio clips not found.");
                return;
            }

            //枠
            GUI.Box(new Rect(10, 10, 200, 120 + this.AudioClipDict.Count * 25), "SE Manager(Debug Mode)");
            int i = 0;
            GUI.Label(new Rect(20, 30 + i++ * 20, 180, 20), "Volume : " + this.Volume.ToString("0.00"));
            GUI.Label(new Rect(20, 30 + i++ * 20, 180, 20), "Max Play : " + this.MaxAudioSources.ToString("0"));

            i = 0;
            //再生ボタン
            foreach (AudioClip se in this.AudioClipDict.Values)
            {
                if (GUI.Button(new Rect(20, 80 + i * 25, 40, 20), "Play"))
                {
                    this.Play(se.name);
                }
                string txt = string.Format("{0}", se.name);
                GUI.Label(new Rect(70, 80 + i * 25, 1000, 20), txt);
                i++;
            }

            //停止ボタン
            if (GUI.Button(new Rect(20, 80 + i++ * 25, 180, 20), "Stop"))
            {
                this.StopImmediately();
            }

            int playingSources = this.AudioSources.Count(s => s.isPlaying);
            if (playingSources == 1)
            {
                GUI.Label(new Rect(20, 80 + i * 25, 1000, 20), string.Format("{0} audio source is playing.", playingSources));
            }
            else if (playingSources > 1)
            {
                GUI.Label(new Rect(20, 80 + i * 25, 1000, 20), string.Format("{0} audio sources are playing.", playingSources));
            }





        }
    }

    /// SEを再生します。
    public void Play(string seName)
    {
        this.Play(seName, this.Volume, 1.0f);
    }


    /// SEを再生します。
    public void Play(string seName, float volume, float pitch)
    {
        if (!this.AudioClipDict.ContainsKey(seName))
        {
            Debug.LogError(string.Format("SE名[{0}]が見つかりません。", seName));
            return;
        }
        if (volume < 0)
            volume = 0;
        if (volume > 1)
            volume = 1;

        //空いているAudioSourceを探す
        AudioSource source = this.AudioSources.FirstOrDefault(s => !s.isPlaying);
        if (source == null)
        {
            if (this.AudioSources.Count >= this.MaxAudioSources)
            {
                Debug.LogWarning("最大同時再生数を超えました。");
                return;
            }

            source = this.gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.loop = false;
            this.AudioSources.Add(source);
        }

        source.clip = this.AudioClipDict[seName];
        source.volume = volume;
        source.pitch = pitch;
        source.Play();
    }

    /// 全てのSEをただちに停止します。
    public void StopImmediately()
    {
        foreach (var source in this.AudioSources)
        {
            source.Stop();
        }
    }

    /// 特定のSEをただちに停止します。
    public void StopImmediately(string seName)
    {
        foreach (var source in this.AudioSources)
        {
            if (source.clip.name == seName)
            {
                source.Stop();
            }
        }
    }
}
