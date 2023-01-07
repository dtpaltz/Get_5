using UnityEngine;

public class PickCardAudio : MonoBehaviour
{
    public static AudioClip PickCardClip;
    private static AudioSource AudioSrc;

    // Start is called before the first frame update
    void Start()
    {
        PickCardClip = Resources.Load<AudioClip>("Clips/pick_a_card");
        AudioSrc = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void Play()
    {
        AudioSrc.PlayOneShot(PickCardClip);
    }
}
