using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;
using GameAnalyticsSDK;

public class GameManager : MonoBehaviour, IGameAnalyticsATTListener
{
    public static GameManager instance;

    public GameObject[] horses;
    public TextMeshProUGUI fundsText;
    public TextMeshProUGUI[] horseResultTexts; // Array to hold result text for each horse
    public Image[] charcaterResultIcons; // Array to hold result icon
    public Sprite[] charcaterSprites; // Array to hold character icon sprites
    public TMP_InputField betAmountInput;
    public TMP_Dropdown betHorseDropdown;
    public TMP_InputField numLapsInput;
    public Button startButton;
    public TextMeshProUGUI seedText; // Reference to the Text component for displaying the seed
    private int racesCount;
    private int funds = 100000000;
    private int betHorse;
    private string betHorseName;
    public int numLaps;
    private int horseIndex;
    private bool[] horsesFinished; // Boolean array to track finished horses
    public TextMeshProUGUI errorText;
    string isToggledOn = "0";
    public TextMeshProUGUI remoteConfigsStatusText;
    public Button IncreaseLapButton;
    public Button DecreaseLapButton;
    public Button IncreaseBetButton;
    public Button DecreaseBetButton;
    private float speedMultiplier = 1; // Must be more than 1 and in decimals like for example => 1.1, 1.2, 1.5 etc.
    int currentBetValue;
    int currentLapsValue;
    public int seed; // Editable seed field

    public System.Random random; // Seeded random number generator

    private void Awake()
    {
        instance = this;
        InitializeUI();
        LoadGameSettings();
        InitializeRandomWithSeed(seed);
    }

    #region GameAnalytics Initialization Code
    void Start()
    {
        InitializeGameAnalytics();
        Invoke(nameof(RecheckRemoteConfigs), 1f);
        Invoke(nameof(CheckIfRemoteConfigsAreReady), 0.5f);
        GameAnalytics.NewDesignEvent("RaceScene_Opened");
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

    void InitializeUI()
    {
        startButton.onClick.AddListener(StartRace);
        IncreaseBetButton.onClick.AddListener(IncreaseBetAmount);
        DecreaseBetButton.onClick.AddListener(DecreaseBetAmount);
        IncreaseLapButton.onClick.AddListener(IncreaseLapCount);
        DecreaseLapButton.onClick.AddListener(DecreaseLapCount);
    }

    void LoadGameSettings()
    {
        racesCount = PlayerPrefs.GetInt("RacesCount", 0);
        funds = PlayerPrefs.GetInt("Money", 1000000);
        currentBetValue = 1;
        currentLapsValue = 1;
        fundsText.text = "$" + funds;
    }

    public void CheckIfRemoteConfigsAreReady()
    {
        // Check if Remote Configs is ready and has been loaded with values
        // It should be ready after initialize.
        if (GameAnalytics.IsRemoteConfigsReady())
        {
            // Call Remote Configs
            Debug.Log("Remote Configs ready!");
            isToggledOn = GameAnalytics.GetRemoteConfigsValueAsString("isToggledOn", "1");
            remoteConfigsStatusText.text = "R";
        }
        else
        {
            Debug.Log("Configs not ready!");
            isToggledOn = GameAnalytics.GetRemoteConfigsValueAsString("isToggledOn", "1");
            remoteConfigsStatusText.text = "N";
        }
    }

    void RecheckRemoteConfigs()
    {
        CheckIfRemoteConfigsAreReady();
    }

    void InitializeGameAnalytics()
    {
        // Added gameanalytics to test API's functionality for remotely controlling horse speeds and to test pseudo random numbers
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            GameAnalytics.RequestTrackingAuthorization(this);
        }
        else
        {
            GameAnalytics.Initialize();
        }
    }

    public void StartRace()
    {
        CheckIfRemoteConfigsAreReady();
        Invoke(nameof(RunHorses), 0.1f);
        WebCallbacksManager.instance.resultString = "";
        InitializeRandomWithSeed(seed);
    }

    void RunHorses()
    {
        GameAnalytics.NewDesignEvent("Race_Started");
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, racesCount.ToString());

        if (!ValidateBetAmount()) return;
        if (!ValidateLaps()) return;

        betHorse = betHorseDropdown.value;
        betHorseName = betHorseDropdown.options[betHorse].text;

        ResetRaceResults();
        InitializeHorses();
        startButton.interactable = false; // Disable start button while race is on
        horseIndex = 0; // Reset the horse index for each race
    }

    bool ValidateBetAmount()
    {
        int betAmount = int.Parse(betAmountInput.text);
        if (betAmount <= 0)
        {
            Debug.Log("Please enter a positive bet amount.");
            ShowPopup("Please enter a positive bet amount.");
            return false;
        }

        if (funds < betAmount)
        {
            Debug.Log("Not enough funds.");
            ShowPopup("Not enough funds.");
            return false;
        }
        return true;
    }

    bool ValidateLaps()
    {
        numLaps = int.Parse(numLapsInput.text);
        if (numLaps < 1)
        {
            Debug.Log("Laps must be greater than 0");
            ShowPopup("Laps must be greater than 0");
            return false;
        }
        return true;
    }

    void ResetRaceResults()
    {
        for (int i = 0; i < horseResultTexts.Length; i++)
        {
            horseResultTexts[i].text = "";
            charcaterResultIcons[i].gameObject.SetActive(false);
        }
        horsesFinished = new bool[horses.Length]; // Initialize horsesFinished array
    }

    void InitializeHorses()
    {
        foreach (GameObject horse in horses)
        {
            Horse horseScript = horse.GetComponent<Horse>();
            horseScript.currentSpeedIndex = -1;
            horseScript.CalculateSpeedsAndBoostsForAllSides();
            horseScript.RandomizeSpeed();
            horseScript.IsMovementEnabled = true;
        }
    }

    public void ReceiveSeedFromReact(string seedString)
    {
        if (int.TryParse(seedString, out int seed))
        {
            InitializeRandomWithSeed(seed);
        }
        else
        {
            Debug.LogError("Received invalid seed from React");
        }
    }

    public void InitializeRandomWithSeed(int? seed = null)
    {
        if (seed.HasValue)
        {
            this.seed = seed.Value;
        }
        else
        {
            this.seed = -1;
        }

        random = new System.Random(this.seed);
        Debug.Log($"Random initialized with seed: {this.seed}");

        // Display the seed on the UI
        if (seedText != null)
        {
            seedText.text = $"Seed: {this.seed}";
        }
    }

    public System.Random GetRandom()
    {
        return random;
    }

    void ShowPopup(string errorMessage)
    {
        errorText.transform.parent.parent.gameObject.SetActive(true);
        errorText.text = errorMessage;
    }

    public void RegisterResult(string horseName, int index, Sprite characterSprite)
    {
        horseResultTexts[horseIndex].text = horseName; // Update the specific horse result text
        charcaterResultIcons[horseIndex].gameObject.SetActive(true);
        WebCallbacksManager.instance.resultString += horseName + ", ";
        charcaterResultIcons[horseIndex].sprite = characterSprite;
        horsesFinished[horseIndex] = true; // Mark this horse as finished
        horseIndex++;
        AudioManager.instance.PlayHorseWhiney();
        if (AllHorsesFinished())
        {
            FinishRace();
        }
    }

    private bool AllHorsesFinished()
    {
        foreach (bool finished in horsesFinished)
        {
            if (!finished)
            {
                return false;
            }
        }
        return true;
    }

    private void FinishRace()
    {
        startButton.interactable = true; // Enable start button

        // Determine if the bet was successful
        string firstHorseName = horseResultTexts[0].text;

        if (firstHorseName == betHorseName)
        {
            funds += int.Parse(betAmountInput.text);
        }
        else
        {
            funds -= int.Parse(betAmountInput.text);
        }
        PlayerPrefs.SetInt("Money", funds);
        fundsText.text = "$" + funds;

        AudioManager.instance.PlayHorseBigul();
        GameAnalytics.NewDesignEvent("Race_Finished");
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, racesCount.ToString());
        racesCount++;
        PlayerPrefs.SetInt("RacesCount", racesCount);
        WebCallbacksManager.instance.SubmitResultsToJS();
    }

    private int ExtractHorseNumber(string horseName)
    {
        // Extract the horse number using regular expression
        Match match = Regex.Match(horseName, @"(\d+)");
        if (match.Success)
        {
            return int.Parse(match.Groups[1].Value);
        }
        else
        {
            throw new System.FormatException("Horse number not found in the result string.");
        }
    }

    void IncreaseBetAmount()
    {
        currentBetValue++;
        betAmountInput.text = currentBetValue.ToString();
    }

    void DecreaseBetAmount()
    {
        currentBetValue--;
        if (currentBetValue < 0)
        {
            currentBetValue = 0;
        }
        betAmountInput.text = currentBetValue.ToString();
    }

    void IncreaseLapCount()
    {
        currentLapsValue++;
        numLapsInput.text = currentLapsValue.ToString();
    }

    void DecreaseLapCount()
    {
        currentLapsValue--;
        if (currentLapsValue < 0)
        {
            currentLapsValue = 0;
        }
        numLapsInput.text = currentLapsValue.ToString();
    }

    public void ControlHorseSpeedMultiplier(float multiPlier, int horseIndex)
    {
        horses[horseIndex].GetComponent<Horse>().speedMultiplier = multiPlier;
    }
}
