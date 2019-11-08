using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField]
    private List<AudioClip> BGMList=null;
    [SerializeField]
    private List<AudioClip> SEList=null;
    [SerializeField]
    private int maxSE = 10;

    private AudioSource bgmSource = null;
    private List<AudioSource> seSource = null;
    private Dictionary<string, AudioClip> bgmDict = null;
    private Dictionary<string, AudioClip> seDict = null;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        DontDestroyOnLoad(this);
    }

    void Start()
    {
        //create audio sources
        this.bgmSource = this.gameObject.GetComponent<AudioSource>();
        this.seSource = new List<AudioSource>();

        //sreate clip dictionaries
        this.bgmDict = new Dictionary<string, AudioClip>();
        this.seDict = new Dictionary<string, AudioClip>();

        //Dictionaryに追加
        Action<Dictionary<string, AudioClip>, AudioClip> addClipDict = (dict, c) =>
          {
              if (!dict.ContainsKey(c.name))
                  dict.Add(c.name, c);
          };

        this.BGMList.ForEach(bgm => addClipDict(this.bgmDict, bgm));
        this.SEList.ForEach(se => addClipDict(this.seDict, se));
    }

    public void PlaySE(string seName,bool roop,float volume)
    {
        if (!this.seDict.ContainsKey(seName))
            throw new ArgumentException(seName + "not found", "seName");

        AudioSource source = this.seSource.FirstOrDefault(s => !s.isPlaying);
        if (source == null)
        {
            if (this.seSource.Count >= this.maxSE)
            {
                Debug.Log("SE AudioSource is full");
                return;
            }
            source = this.gameObject.AddComponent<AudioSource>();
            this.seSource.Add(source);
        }

        source.clip = this.seDict[seName];
        source.Play();
        source.loop = roop;
        source.volume = volume;
    }

    public void SeStop(string seName)
    {
        var sorce = seSource.Where(s => s.clip.name == seName).ToList();
        sorce.ForEach(s => s.Stop());
    }

    public void PlayBGM(string bgmName)
    {
        if (!this.bgmDict.ContainsKey(bgmName))
            throw new ArgumentException(bgmName + "not found", "bgmName");
        if (this.bgmSource.clip == this.bgmDict[bgmName]) return;
        this.bgmSource.Stop();
        this.bgmSource.clip = this.bgmDict[bgmName];
        this.bgmSource.Play();
    }

    public void StopBGM()
    {
        this.bgmSource.Stop();
        this.bgmSource.clip = null;
    }

    //すべてのSE停止
    public void StopFullSE()
    {
        seSource.ForEach(s => s.Stop());
    }

    public bool GetBGM(string bgmName)
    {
        return this.bgmSource.clip.name==bgmName;
    }

    public bool SePlayCheck(string seName)
    {
        var check = false;
        foreach(var s in seSource)
        {
            if (s.clip.name == seName)
                check = s.isPlaying;
            else continue;
        }
        return check;
    }
}
