using UnityEngine;
using System;

public class Gem : MonoBehaviour, IItem
{
    public static event Action<int> OnCollect;
    public int worth = 5;
    public AudioManager audioManager;
    void Start()
    {
        audioManager = FindFirstObjectByType<AudioManager>();
    }
    public void Collect()
    {
        audioManager.PlayGemCollectedSound(); // Phát âm thanh khi thu thập viên ngọc
        OnCollect.Invoke(worth);
        Destroy(gameObject);
        Debug.Log("Gem collected");
    }
}
