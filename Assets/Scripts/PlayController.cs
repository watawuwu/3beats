using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UniRx;

public class PlayController : MonoBehaviour
{
    public const float PRE_NOTE_SPAWN_TIME = 3f;
    public const float PERFECT_BORDER = 0.05f;
    public const float GREAT_BORDER = 0.1f;
    public const float GOOD_BORDER = 0.2f;
    public const float BAD_BORDER = 0.5f;

    [SerializeField]
    AudioManager audioManager;
    [SerializeField]
    TextAsset songDataAsset;
    [SerializeField]
    NoteObject noteObjectPrefab;
    [SerializeField]
    Transform messageObjectContainer;
    [SerializeField]
    MessageObject messageObjectPrefab;
    [SerializeField]
    GameObject gameOverPanel;
    [SerializeField]
    Text scoreText;
    [SerializeField]
    Text lifeText;
    [SerializeField]
    AudioSource hitSE;
    [SerializeField]
    AudioSource missSE;
    
    float previousTime = 0f;
    SongData song;
    readonly List<NoteObject> noteObjectPool = new List<NoteObject>();
    readonly List<MessageObject> messageObjectPool = new List<MessageObject>();
    int life;
    int score;
    readonly double fadeOutSeconds = 2.0;
    double fadeDeltaTime = 0;

    static readonly KeyCode[] keys = {
            KeyCode.Z, KeyCode.X
    };

    Dictionary<KeyCode, int> noteNumbers = new Dictionary<KeyCode, int>() { {keys[0], 0}, { keys[1], 1 } };

    int Life
    {
        set
        {
            life = value;
            if (life <= 0)
            {
                life = 0;
                gameOverPanel.SetActive(true);
            }
            lifeText.text = string.Format("Life: {0}", life);
        }
        get { return life; }
    }

    int Score
    {
        set
        {
            score = value;
            scoreText.text = string.Format("Score: {0}", score);
        }
        get { return score; }
    }

    public Transform MessageObjectContainer { get => messageObjectContainer; set => messageObjectContainer = value; }

    void Start()
    {
        // フレームレート設定
        Application.targetFrameRate = 60;
        hitSE.volume = 0.5f;
        missSE.volume = 0.5f;

        Score = 0;
        Life = 10;

        // ノートオブジェクトのプール
        for (var i = 0; i < 100; i++)
        {
            var obj = Instantiate(noteObjectPrefab, new Vector3(0, 0, 120), Quaternion.identity);
            obj.gameObject.SetActive(false);
            noteObjectPool.Add(obj);
        }
        noteObjectPrefab.gameObject.SetActive(false);

        // メッセージオブジェクトのプール
        for (var i = 0; i < 50; i++)
        {
            var obj = Instantiate(messageObjectPrefab, MessageObjectContainer);
            obj.gameObject.SetActive(false);
            messageObjectPool.Add(obj);
        }
        messageObjectPrefab.gameObject.SetActive(false);

        // 楽曲データのロード
        song = SongData.LoadFromJson(songDataAsset.text);

        audioManager.bgm.PlayDelayed(1f);
    }

    void Update()
    {
        // ノートを生成
        var bgmTime = audioManager.bgm.time;
        foreach (var note in song.GetNotesBetweenTime(previousTime + PRE_NOTE_SPAWN_TIME, bgmTime + PRE_NOTE_SPAWN_TIME))
        {
            var obj = noteObjectPool.FirstOrDefault(x => !x.gameObject.activeSelf);
            obj.Initialize(this, audioManager.bgm, note);
        }
        previousTime = bgmTime;

        var source = audioManager.bgm;
        if (gameOverPanel.activeSelf && source.isPlaying)
        {
            fadeDeltaTime += Time.deltaTime;
            if (fadeDeltaTime >= fadeOutSeconds)
            {
                fadeDeltaTime = fadeOutSeconds;
                source.Stop();
                Observable.Timer(TimeSpan.FromMilliseconds(1000))
                   .Subscribe(_ => FadeManager.Instance.LoadScene("Result", 1.0f));
            }
            source.pitch = (float)(1.0 - fadeDeltaTime / fadeOutSeconds) * 1;
            source.volume = (float)(1.0 - fadeDeltaTime / fadeOutSeconds) * 1;           
        }
    }

    void OnNotePerfect(Vector2 vec)
    {
        ShowMessage("Perfect", Color.yellow, vec);
        Score += 1000;
    }

    void OnNoteGreat(Vector2 ved)
    {
        ShowMessage("Great", Color.magenta, ved);
        Score += 500;
    }

    void OnNoteGood(Vector2 vec)
    {
        ShowMessage("Perfect", Color.green, vec);
        Score += 300;
    }

    void OnNoteNoGood(Vector2 vec)
    {
        ShowMessage("NoGood", Color.gray, vec);
        Life--;
    }

    public void OnNoteBad(Vector2 vec)
    {
        ShowMessage("Bad", Color.yellow, vec);
        Life--;
    }

    public void OnNoteMiss(Vector2 vec)
    {
        ShowMessage("Miss", Color.red, vec);
        Life--;
    }

    void ShowMessage(string message, Color color, Vector2 vec)
    {
        if (gameOverPanel.activeSelf)
        {
            return;
        }
        var obj = messageObjectPool.FirstOrDefault(m => !m.gameObject.activeSelf);
        obj.Initialize(message, color, vec);
    }


    public void Hit(NoteObject note)
    {
        if (gameOverPanel.activeSelf)
        {
            return;
        }

        hitSE.Play();

        //var targetNoteObject = noteObjectPool.Where(x => x.NoteNumber == noteNo)
        //                                     .OrderBy(x => x.AbsoluteTimeDiff)
        //                                     .FirstOrDefault(x => x.AbsoluteTimeDiff <= BAD_BORDER);

        if (null == note)
        {
            return;
        }

        var timeDiff = note.AbsoluteTimeDiff;

        if (timeDiff <= PERFECT_BORDER)
        {
            OnNotePerfect(note.transform.position);
        }
        else if (timeDiff <= GREAT_BORDER)
        {
            OnNoteGreat(note.transform.position);
        }
        else if (timeDiff <= GOOD_BORDER)
        {
            OnNoteGood(note.transform.position);
        }
        else
        {
            OnNoteNoGood(note.transform.position);
        }
        note.gameObject.SetActive(false);
    }

    public void Miss(NoteObject note)
    {
        if (gameOverPanel.activeSelf)
        {
            return;
        }

        missSE.Play();
        OnNoteBad(note.transform.position);       
        note.gameObject.SetActive(false);
    }
   
}
