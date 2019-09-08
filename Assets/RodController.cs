using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RodController : MonoBehaviour
{
    public PlayController playController;
    public int rodNumber;

    void OnTriggerEnter(Collider other)
    {
        var tag = other.gameObject.tag;
        NoteObject note = other.gameObject.GetComponentInParent<NoteObject>();

        if (tag == "Hit" && note.NoteNumber == rodNumber)
        {            
            playController.Hit(note);
        }
        else if (tag == "Hit")
        {
            playController.Miss(note);
        }        
    }
}
