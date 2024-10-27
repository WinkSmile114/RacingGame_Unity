using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using GameAnalyticsSDK;
using System;
using System.Collections;
using TMPro;
//using SupersonicWisdomSDK;
using DG.Tweening;

public class LoadingScreen : MonoBehaviour, IGameAnalyticsATTListener
{
    [SerializeField] int reservedLevelCount = 13;
    [SerializeField] Image loadingBar;
    [SerializeField] TextMeshProUGUI loadingBarText;
    [SerializeField] TextMeshProUGUI tipText;
    [SerializeField] float waitTime = 2f;
    [SerializeField] String[] randomTips;
    void Awake()
    {

        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            GameAnalytics.RequestTrackingAuthorization(this);
        }
        else
        {
            GameAnalytics.Initialize();
        }
        int randomTipIndex = UnityEngine.Random.Range(0, randomTips.Length);

        tipText.text = randomTips[randomTipIndex].ToString();
    }

    #region game analytics SDK
    void Start()
    {
        //  LoadCurrentLevel();
        StartCoroutine(ChangeTexts());
        loadingBar.DOFillAmount(1, waitTime).OnComplete(() => { isLoadingNextLevelEnabled = true; });

    }
    IEnumerator ChangeTexts()
    {
        float textStringChangeDuration = waitTime / 4;
        string[] strings = { "Loading configurations..", "Preparing horses..", "Almost Ready!", "Please Wait.." };
        for (int i = 0; i < 4; i++)
        {
            loadingBarText.text = strings[i];
            yield return new WaitForSeconds(textStringChangeDuration);
        }
    }

    bool isLoadingNextLevelEnabled = false;
    void Update()
    {

        if (isLoadingNextLevelEnabled)
        {
            LoadCurrentLevel();
            enabled = false;
            AudioManager.instance.PlayHorseBigul();
        }
    }
    public void GameAnalyticsATTListenerNotDetermined()
    {
        GameAnalytics.Initialize();
    }
    public void GameAnalyticsATTListenerRestricted()
    {
        GameAnalytics.Initialize();
    }
    public void GameAnalyticsATTListenerDenied()
    {
        GameAnalytics.Initialize();
    }
    public void GameAnalyticsATTListenerAuthorized()
    {
        GameAnalytics.Initialize();
    }
#endregion

    public void LoadCurrentLevel()
    {
        gameObject.SetActive(false);
    }
}