using UnityEngine;
using TMPro;
using System.Collections;

public class MessageManager : MonoBehaviour
{
    [Header("Configs")]
    public TextMeshProUGUI messageText;
    public float defaultDuration = 2f;

    private Coroutine currentMessage;

    public void ShowMessage(string message, float duration = -1) 
    {
        if(duration < 0) duration = defaultDuration;

        if(currentMessage != null) StopCoroutine(currentMessage);

        currentMessage = StartCoroutine(DisplayMessage(message, duration));
    }

    private IEnumerator DisplayMessage(string message, float duration) 
    {
        if(messageText != null) 
        {
            messageText.text = message;
            yield return new WaitForSeconds(duration);
            messageText.text = "";
        }
        currentMessage = null;
    }

    public void ClearMessage() 
    {
        if (currentMessage != null) 
        {
            StopCoroutine(currentMessage);
            currentMessage = null;
        }
        if (messageText != null) messageText.text = "";
    }
}
