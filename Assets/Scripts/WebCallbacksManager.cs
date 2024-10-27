using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Runtime.InteropServices;
using UnityEngine.UI;
using TMPro;
using System.Numerics;


public class WebCallbacksManager : MonoBehaviour
{
    public int seed;
    public string result;
    public static WebCallbacksManager instance;
    public string resultString;
    string postURL = "http://localhost:59506/";
    public TextMeshProUGUI serverUrlText;

    private void Awake()
    {
        instance = this;
        serverUrlText.text = PlayerPrefs.GetString("ServerURL", postURL);
        postURL = PlayerPrefs.GetString("ServerURL", postURL);
    }

    public void ReceiveSeed(string seed)
    {
        GameManager.instance.ReceiveSeedFromReact(seed);
    }


    public void ReceiveServerURL(string serverURL)
    {
        postURL = serverURL;
        serverUrlText.text = postURL;
        PlayerPrefs.SetString("ServerURL", serverURL);
    }

    public void SendResultsToTheServer()
    {
        StartCoroutine(Upload());
    }

    IEnumerator Upload()
    {
        using (UnityWebRequest www = UnityWebRequest.Post(postURL, "Winners: " + resultString, "application/json"))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
#if UNITY_WEBGL && !UNITY_EDITOR
        ShowMessage(www.error);
#endif

            }
            else
            {
                Debug.Log("Form upload complete!");
#if UNITY_WEBGL && !UNITY_EDITOR
                ShowMessage("Submitting data complete!");
#endif
            }
        }
        yield return null;
    }


#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void ShowMessage(string message);
#endif


    public TMP_InputField TextInput;
    public TextMeshProUGUI DisplayText;

    // Start is called before the first frame update
    void Start()
    {
#if !UNITY_EDITOR && UNITY_WEBGL
        WebGLInput.captureAllKeyboardInput = false;
#endif
    }


    public void SubmitResultsToJS()
    {
        Invoke("DelaySubmittingResults", 0.5f);
        SendResultsToTheServer();
    }

    void DelaySubmittingResults()
    {
        string MessageToSend = "Winners: " + resultString;
        Debug.Log("Sending message to JavaScript: " + MessageToSend);
#if UNITY_WEBGL && !UNITY_EDITOR
        ShowMessage(MessageToSend);
#endif
    }

    public void SendToUnity(string message)
    {
        DisplayText.text = message;
    }
}
