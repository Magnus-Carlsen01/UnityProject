using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource backgroundMusic; // Nguồn âm thanh cho nhạc nền
    public AudioSource soundEffect; // Nguồn âm thanh cho hiệu ứng âm thanh

    public AudioClip backgroundMusicClip; // Clip âm thanh cho nhạc nền
    public AudioClip gemCollectedClip; // Clip âm thanh cho hiệu ứng âm thanh
    public AudioClip jumpSoundClip; // Clip âm thanh cho tiếng nhảy
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayBackgroundMusic(); // Gọi hàm để phát nhạc nền
    }

    public void PlayBackgroundMusic()
    {
        backgroundMusic.clip = backgroundMusicClip; // Gán clip âm thanh cho nhạc nền
        backgroundMusic.Play();
    }
    public void PlayGemCollectedSound()
    {
        soundEffect.PlayOneShot(gemCollectedClip); // Phát hiệu ứng âm thanh khi thu thập viên ngọc
    }
    public void PlayJumpSound()
    {
        soundEffect.PlayOneShot(jumpSoundClip); // Phát hiệu ứng âm thanh khi nhảy
    }
}
