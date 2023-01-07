using UnityEngine;

public class CongratsAudio : MonoBehaviour
{
    public static AudioClip CongratulationsClip;
    private static AudioSource AudioSrc;

    // Start is called before the first frame update
    void Start()
    {
        CongratulationsClip = Resources.Load<AudioClip>("Clips/congratulations");
        AudioSrc = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void Play()
    {
        AudioSrc.PlayOneShot(CongratulationsClip);
    }
}
