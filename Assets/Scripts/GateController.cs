using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateController : MonoBehaviour
{
    private Animator anim;
    private BoxCollider2D gateCollider;
    private bool isOpen = false;

    void Awake()
    {
        anim = GetComponent<Animator>();
        gateCollider = GetComponent<BoxCollider2D>();
    }

    // Hàm này sẽ được gọi khi người chơi nhấn nút hoặc tiêu diệt hết quái
    public void OpenGate()
    {
        if (!isOpen)
        {
            isOpen = true;
            anim.SetTrigger("IsOpen"); // Chạy animation mở cổng
            gateCollider.enabled = false; // Tắt va chạm để người chơi đi qua
            Debug.Log("Cổng đã mở!");
        }
    }

    public void CloseGate()
    {
        if (isOpen)
        {
            isOpen = false;
            anim.SetTrigger("IsClose"); // Chạy animation đóng cổng
            gateCollider.enabled = true; // Bật lại va chạm
        }
    }
}
