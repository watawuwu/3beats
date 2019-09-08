using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class SongData
{
    public enum NoteType
    {
        Whole = 1,
        Half = 2,
        Quarter = 4,
        Eighth = 8,
        Sixteenth = 16,
        ThirtySecond = 32,
    }

    public int bpm = 120;

    public NoteType minNoteType = NoteType.Sixteenth;

    [SerializeField]
    List<Note> notes = new List<Note>();

    public bool HasNote
    {
        get { return notes.Count > 0; }
    }

    public static SongData LoadFromJson(string json)
    {
        return JsonUtility.FromJson<SongData>(json);
    }

    public void AddNote(float time, int noteNumber)
    {
        var minNoteLength = 2f / (float)minNoteType;
        var roundedTime = Mathf.Round(time / minNoteLength) * minNoteLength;
        if (!notes.Any(x => Mathf.Abs(x.Time - roundedTime) <= Mathf.Epsilon && x.NoteNumber == noteNumber))
        {
            notes.Add(new Note(roundedTime, noteNumber));
        }
    }

    public IEnumerable<Note> GetNotesBetweenTime(float start, float end)
    {
        return notes.Where(x => start < x.Time && x.Time <= end);
    }

    public void ClearNotes()
    {
        notes.Clear();
    }

    [Serializable]
    public struct Note
    {
        [SerializeField]
        float time;
        [SerializeField]
        int noteNumber;
        [SerializeField]
        Vector2 position;
        [SerializeField]
        Vector3 rotation;

        public float Time
        {
            get { return time; }
        }

        public int NoteNumber
        {
            get { return noteNumber; }
        }

        public Vector3 Rotation
        {
            get { return rotation; }
        }

        public Vector2 Postition
        {
            get { return position; }
        }


        public Note(float time, int noteNumber)
        {
            this.time = time;
            this.noteNumber = noteNumber;
            this.rotation = Vector3.zero;
            this.position = Vector2.zero;
        }
    }
}


