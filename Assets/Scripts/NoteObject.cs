using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoteObject : MonoBehaviour
{
	public float baseZ;
    public List<Texture> noteTextures;

	PlayController playController;
	AudioSource bgm;
	SongData.Note note;

    public float speed = 50f;

    public int NoteNumber
	{
		get { return gameObject.activeSelf ? note.NoteNumber : int.MinValue; }
	}

	public float AbsoluteTimeDiff
	{
		get { return Mathf.Abs(note.Time - bgm.time); }
	}

    void Update()
	{
		var timeDiff = note.Time - bgm.time;
		if (timeDiff <- PlayController.BAD_BORDER)
		{
            playController.OnNoteMiss(new Vector2(0, 1f));
            gameObject.SetActive(false);
		}

        Transform trans = this.transform;
        Vector3 pos = trans.position;
        pos.z = baseZ + timeDiff * speed;
        trans.position = pos;
    }

	public void Initialize(PlayController playController, AudioSource bgm, SongData.Note note)
	{
        Transform trans = this.transform;
        Vector3 pos = trans.position;
        pos.x = note.Postition.x;
        pos.y = note.Postition.y;
        trans.position = pos;
        trans.rotation = Quaternion.Euler(note.Rotation);


        gameObject.SetActive(true);

		this.playController = playController;
		this.bgm = bgm;
		this.note = note;
        

        GetComponent<Renderer>().material.mainTexture = this.noteTextures[note.NoteNumber];

        Update();
	}


}