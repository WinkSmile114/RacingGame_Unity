using UnityEngine;

public class Horse : MonoBehaviour
{
    [Range(1f, 10f)]
    public float speed = 4f; // Base speed
    public float maxSpeedBoost = 0.5f; // Base speed
    public float minSpeedReduce = -0.5f; // Base speed
    private float initialSpeed = 4f; // Base speed
    public float speedMultiplier = 1;
    private Vector3 startPosition;
    [SerializeField] private int boostProbability = 6; // Lower is better
    private int lapsCompleted = 0;
    public float positiveXBounds;
    public float negativeXBounds;
    public float positiveYBounds;
    public float negativeYBounds;
    public float finishLineBounds;
    public float finishLineOffset = 2f; // Offset after the finish line
    private bool hasReachedFinishLine;
    public string characterName;
    public Sprite characterSprite;
    private bool isMovementEnabled;
    public int horseIndex;
    private bool extraBoostEnabled;
    private bool hasRecordedResult = false;
    private bool reachedFinishLine = false;
    public PlayerAnimationController playerAnimationController;
    public AudioSource characterAudioSource;
    public GameObject dirtParticlesHorizontal;
    public GameObject dirtParticlesVertical;
    public float[] allSideSpeeds;
    public bool[] allSideExtraBoosts;
    public bool[] missExtraBoosts;
    public int currentSpeedIndex = -1;


    public bool IsMovementEnabled
    {
        get => isMovementEnabled;
        set
        {
            if (value)
            {
                currentDirection = Direction.Right;
                ResetRaceState();
            }
            else
            {
                characterAudioSource.Stop();
            }
            isMovementEnabled = value;
        }
    }

    public enum Direction
    {
        Right,
        Down,
        Left,
        Up,
        Idle
    }

    public Direction currentDirection = Direction.Idle;

    void Start()
    {
        startPosition = transform.localPosition;
        initialSpeed = speed;
        // random = GameManager.instance.GetRandom(); // Get the seeded random generator from GameManager
        //RandomizeSpeed();
    }

    void Update()
    {
        if (isMovementEnabled)
            Move();
    }

    void Move()
    {
        // Remove random reinitialization here
        float moveAmount = speed * Time.deltaTime;

        switch (currentDirection)
        {
            case Direction.Right:
                transform.Translate(Vector3.right * moveAmount);
                if (!hasReachedFinishLine && transform.localPosition.x > positiveXBounds)
                {
                    SwitchDirection(Direction.Down);
                }
                else if (hasReachedFinishLine && transform.localPosition.x > finishLineBounds)
                {
                    HandleFinishLine();
                }
                break;

            case Direction.Down:
                transform.Translate(Vector3.down * moveAmount);
                if (transform.localPosition.y < negativeYBounds)
                {
                    SwitchDirection(Direction.Left);
                }
                break;

            case Direction.Left:
                transform.Translate(Vector3.left * moveAmount);
                if (transform.localPosition.x < negativeXBounds)
                {
                    SwitchDirection(Direction.Up);
                }
                break;

            case Direction.Up:
                transform.Translate(Vector3.up * moveAmount);
                if (transform.localPosition.y > positiveYBounds)
                {
                    SwitchDirection(Direction.Right);
                    hasReachedFinishLine = true;
                }
                break;
        }
    }

    void SwitchDirection(Direction newDirection)
    {
        currentDirection = newDirection;
        RandomizeSpeed();
    }

    void HandleFinishLine()
    {
        if (!reachedFinishLine)
        {
            reachedFinishLine = true;
        }
        if (transform.localPosition.x > finishLineBounds + finishLineOffset)
        {
            if (lapsCompleted >= GameManager.instance.numLaps - 1)  // Check if final lap
            {
                if (!hasRecordedResult)
                {
                    GameManager.instance.RegisterResult(characterName, horseIndex, characterSprite);
                    hasRecordedResult = true;
                }
                Arrive();
            }
            lapsCompleted++;
            hasReachedFinishLine = false;  // Prepare for the next lap
            reachedFinishLine = false;
        }
    }

    void Arrive()
    {
        // Stop the horse
        speed = 0;
        Debug.Log("Horse has stopped moving: " + characterName);
        IsMovementEnabled = false;
        SwitchDirection(Direction.Idle);
    }

    public void CalculateSpeedsAndBoostsForAllSides()
    {
        for (int i = 0; i <= 4; i++)
        {
            allSideSpeeds[i] = (initialSpeed + (float)(GameManager.instance.random.NextDouble() * (maxSpeedBoost - minSpeedReduce) + minSpeedReduce)) * speedMultiplier;
            allSideExtraBoosts[i] = (GameManager.instance.random.Next(1, boostProbability) == 1);
            missExtraBoosts[i] = 1 == GameManager.instance.random.Next(1, 4);
        }
    }

    public void RandomizeSpeed()
    {
        currentSpeedIndex++;
        dirtParticlesHorizontal.SetActive(false);
        dirtParticlesVertical.SetActive(false);
        //extraBoostEnabled = (GameManager.instance.random.Next(1, boostProbability) == 1);
        extraBoostEnabled = allSideExtraBoosts[currentSpeedIndex];
        if (extraBoostEnabled)
        {
            ApplyExtraBoost();
        }
        else
        {
            // speed = (initialSpeed + (float)(GameManager.instance.random.NextDouble() * (maxSpeedBoost - minSpeedReduce) + minSpeedReduce)) * speedMultiplier;
            speed = allSideSpeeds[currentSpeedIndex];
        }
    }

    void ApplyExtraBoost()
    {
        Debug.Log("Extra boost enabled for " + characterName);
        float extraBoostValue = 0.75f;
        if (missTheBoost())
        {
            extraBoostValue = 0.1f;
            Debug.Log("Extra boost missed by " + characterName);
        }
        else
        {
            Debug.Log("Current direction for " + transform.name + " " + currentDirection.ToString());
            if (currentDirection == Direction.Right || currentDirection == Direction.Left || currentDirection == Direction.Idle)
                dirtParticlesHorizontal.SetActive(true);
            else
                dirtParticlesVertical.SetActive(true);
        }
        speed = (initialSpeed + (float)(GameManager.instance.random.NextDouble() * (maxSpeedBoost - minSpeedReduce) + minSpeedReduce) + extraBoostValue) * speedMultiplier;
        extraBoostEnabled = false;
    }

    bool missTheBoost()
    {
        return missExtraBoosts[currentSpeedIndex];
    }

    void ResetRaceState()
    {
        currentDirection = Direction.Right;
        hasReachedFinishLine = false;
        lapsCompleted = 0;
        hasRecordedResult = false;
        reachedFinishLine = false;
        characterAudioSource.Play();
    }
}
