using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEditor;


public class RecordController : MonoBehaviour
{
    [SerializeField]
    AudioSource clickSE;
    [SerializeField]
    AudioManager audioManager;
    [SerializeField]
    Button recordButton;
    [SerializeField]
    Button playButton;
    [SerializeField]
    Button[] noteButtons;
    [SerializeField]
    Text time;
    [SerializeField]
    TextAsset preloadSongDataAsset;

    bool isRecording = false;
    float previousTime = 0f;
    SongData song;

    void Start()
    {
        Application.targetFrameRate = 60;

        recordButton.onClick.AddListener(OnRecordButtonClick);
        playButton.onClick.AddListener(OnPlayButtonClick);
        for (var i = 0; i < noteButtons.Length; i++)
        {
            noteButtons[i].onClick.AddListener(GetOnNoteButtonClickAction(i));
        }

        if (null != preloadSongDataAsset)
        {
            song = SongData.LoadFromJson(preloadSongDataAsset.text);
            playButton.interactable = song.HasNote;
        }
        else
        {
            song = new SongData();
        }
    }

    void Update()
    {
        var bgmTime = audioManager.bgm.time;
        time.text = string.Format("{0:00}:{1:00}:{2:000}",
            Mathf.FloorToInt(bgmTime / 60f),
            Mathf.FloorToInt(bgmTime % 60f),
            Mathf.FloorToInt(bgmTime % 1f * 1000));

        var keys = new KeyCode[]
        {
            KeyCode.Z, KeyCode.X
        };
        for (var i = 0; i < keys.Length; i++)
        {
            if (Input.GetKeyDown(keys[i]))
            {
                noteButtons[i].onClick.Invoke();
            }
        }

        if (isRecording)
        {
            if (!audioManager.bgm.isPlaying)
            {
                isRecording = false;
                playButton.interactable = song.HasNote;

                var path = string.Format("Assets/Resources/{0}.txt", DateTime.Now.ToString("yyyyMMddHHmmss"));
                File.WriteAllText(path, JsonUtility.ToJson(song));
                AssetDatabase.Refresh();
            }
        }
        else if (audioManager.bgm.isPlaying)
        {
            foreach (var note in song.GetNotesBetweenTime(previousTime, bgmTime))
            {
                clickSE.Play();
                noteButtons[note.NoteNumber].Select();
                StartCoroutine(DeselectCoroutine(noteButtons[note.NoteNumber]));
            }
            previousTime = bgmTime;
        }
    }

    IEnumerator DeselectCoroutine(Button button)
    {
        yield return new WaitForSeconds(0.1f);
        if (EventSystem.current.currentSelectedGameObject == button.gameObject)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    void OnRecordButtonClick()
    {
        if (audioManager.bgm.isPlaying)
        {
            audioManager.bgm.Stop();
        }
        else
        {
            song.ClearNotes();
            audioManager.bgm.Play();
            isRecording = true;
        }
    }

    void OnPlayButtonClick()
    {
        if (isRecording)
        {
            return;
        }

        if (audioManager.bgm.isPlaying)
        {
            audioManager.bgm.Stop();
        }
        else
        {
            previousTime = audioManager.bgm.time;
            audioManager.bgm.Play();
        }
    }

    UnityAction GetOnNoteButtonClickAction(int noteNo)
    {
        return () =>
        {
            if (!audioManager.bgm.isPlaying)
            {
                return;
            }

            song.AddNote(audioManager.bgm.time, noteNo);
            clickSE.Play();
            noteButtons[noteNo].Select();
            StartCoroutine(DeselectCoroutine(noteButtons[noteNo]));
        };
    }
}

