using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundControl : MonoBehaviour
{
    public SpriteRenderer rend;

    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnMouseDown()
    {
        if (GameControl.PickCardAudioReminderEnabled)
        {
            GameControl.PickCardAudioReminderEnabled = false;
            rend.color = new Color(1f, 1f, 1f, 0.5f);
        }
        else
        {
            GameControl.PickCardAudioReminderEnabled = true;
            rend.color = new Color(1f, 1f, 1f, 1f);
        }
    }
}
