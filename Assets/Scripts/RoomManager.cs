using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public GateController[] gates; 
    private bool roomCleared = false;

    void Update()
    {
        if (roomCleared) return;

        if (transform.childCount == 0)
        {
            roomCleared = true;
            OpenRoomGate();
        }
    }

    void OpenRoomGate()
    {
        if (gates != null)
        {
            foreach (var gate in gates)
            {
                gate.OpenGate();
            }
        }
    }
}