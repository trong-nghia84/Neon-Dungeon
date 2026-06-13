using UnityEngine;
using TMPro;

public class TutorialTrigger : MonoBehaviour
{
    [Header("Nội dung hướng dẫn")]
    [TextArea(3, 5)] 
    public string tutorialMessage;

    private TextMeshProUGUI uiText;
    private bool hasTriggered = false; 

    void Awake()
    {
        GameObject textObj = GameObject.Find("TutorialText");
        if (textObj != null)
        {
            uiText = textObj.GetComponent<TextMeshProUGUI>();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (uiText != null)
            {
                uiText.text = tutorialMessage; 
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (uiText != null && uiText.text == tutorialMessage)
            {
                uiText.text = ""; 
            }

        }
    }
}