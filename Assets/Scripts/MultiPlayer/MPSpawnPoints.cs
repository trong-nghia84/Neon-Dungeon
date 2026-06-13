using UnityEngine;

public class MPSpawnPoints : MonoBehaviour
{
    public static MPSpawnPoints Instance;

    public Transform hostSpawn;
    public Transform clientSpawn;

    private void Awake()
    {
        Instance = this;
    }
}