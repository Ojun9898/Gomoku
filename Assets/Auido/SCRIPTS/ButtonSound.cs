using UnityEngine;

public class ButtonSound : MonoBehaviour
{
    public static ButtonSound instance; 
    public AudioSource buttonAudio;
    public AudioClip buttonClick;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySound()
    {
        buttonAudio.PlayOneShot(buttonClick);
    }
}
