using System;
using UnityEngine;

public class HeartItem : MonoBehaviour, IItem
{
    public static event Action<int> OnHeartCollected;
    public int healAmount = 1;  
    public void Collect()
    {
        OnHeartCollected?.Invoke(healAmount);
        Destroy(gameObject);
    }
}
