using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBubbleFactory
{
    GameObject CreateBubble(Camera mainCamera, float bubbleSpeed, float bubbleMaxSpeed, bool preventBlackSpawn);
}

public class ScaledBubbleFactory : Object, IBubbleFactory
{
    private const int cameraOffset = 2;
    private List<Sprite> BubblePrefabs;

    public ScaledBubbleFactory(List<Sprite> BubblePrefabs)
    {
        this.BubblePrefabs = BubblePrefabs;
    }

    public GameObject CreateBubble(Camera mainCamera, float bubbleSpeed, float bubbleMaxSpeed, bool preventBlackSpawn)
    {
        float cameraHeight = mainCamera.orthographicSize;
        float cameraWidth = cameraHeight * mainCamera.aspect;
        System.Random randomNumber = new System.Random();

        int positiveOrNegativeNumber = randomNumber.Next(0, 2) * 2 - 1;
        float spawnY = UnityEngine.Random.Range(-cameraHeight, cameraHeight - cameraOffset);

        Vector3 spawnPosition = new Vector3(positiveOrNegativeNumber * cameraWidth, spawnY, 0f);

        int spawnScaleRNG = Random.Range(0, 100);
        float spawnScale = spawnScaleRNG switch
        {
            > 75 => 1.5f,  // 24%
            > 50 => 1f,    // 25%
            > 25 => 1.2f,  // 25%
            > 4 => 1.1f,   // 25%
            _ => 1.4f      // 4%
        };

        GameObject bubblePrefab = Resources.Load<GameObject>("Bubble");
        GameObject bubbleGO = Instantiate(bubblePrefab, spawnPosition, Quaternion.identity);
        bubbleGO.GetComponent<SpriteRenderer>().sprite = BubblePrefabs[Random.Range(0, BubblePrefabs.Count)];
        if (spawnScale == 1.2f)
        {
            bubbleGO.tag = "Red";
            bubbleGO.GetComponent<SpriteRenderer>().material = Resources.Load<Material>("Materials/Red");
        }
        else if (spawnScale == 1.1f)
        {
            bubbleGO.tag = "Green";
            bubbleGO.GetComponent<SpriteRenderer>().material = Resources.Load<Material>("Materials/Green");
        }
        else if (spawnScale == 1.4f && !preventBlackSpawn)
        {
            bubbleGO.tag = "Black";
            bubbleGO.GetComponent<SpriteRenderer>().material = Resources.Load<Material>("Materials/Black");
        }
        else bubbleGO.AddComponent<Pop>();

        bubbleGO.transform.localScale *= spawnScale;
        bubbleGO.GetComponent<Rigidbody2D>().AddForceX(CalculateRandomForce((int)bubbleSpeed, (int)bubbleMaxSpeed) * -positiveOrNegativeNumber);
        bubbleGO.GetComponent<Rigidbody2D>().AddTorque(CalculateRandomForce((int)bubbleSpeed / 4, (int)bubbleMaxSpeed / 4) * -positiveOrNegativeNumber);

        return bubbleGO;
    }

    private float CalculateRandomForce(int bubbleSpeed, int bubbleMaxSpeed)
    {
        System.Random randomNumber = new System.Random();
        return (float)randomNumber.Next(bubbleSpeed, bubbleSpeed * bubbleMaxSpeed);
    }
}

public class Pop : MonoBehaviour
{
    private void Start()
    {
        Invoke(nameof(SelfDestruct), 2f);
    }

    private void SelfDestruct()
    {
        GameObject.Find("ScoreController").GetComponent<ScoreController>().SubstractLives(1);
        Destroy(gameObject);
    }
}

public class SpawnBubble : MonoBehaviour
{
    private IBubbleFactory bubbleFactory;
    [SerializeField] List<Sprite> bubbleList;

    [SerializeField] private GameObject touchController;
    private Intenso intenso;

    private Camera mainCamera;

    private float nextSpawnTime = 0.0f;
    [SerializeField] private float spawnDelay = 1.0f;
    [SerializeField] private float spawnSpeed = 2f;

    [SerializeField] private float bubbleSpeed = 2f;
    [SerializeField] private int bubbleMaxSpeed = 4;

    [SerializeField] private bool logarithmicProgression = false;
    [SerializeField] private bool sigmoidProgression = false;
    [SerializeField] private bool rootProgression = false;
    [SerializeField] private bool cubicProgression = false;
    [SerializeField] private bool powProgression = false;

    void Start()
    {
        mainCamera = Camera.main;
        bubbleFactory = new ScaledBubbleFactory(bubbleList);
        intenso = touchController.GetComponent<Intenso>();
    }

    void Update()
    {
        if (Time.time > nextSpawnTime)
        {
            StartCoroutine(SpawnBubbles());
            nextSpawnTime = Time.time + spawnDelay / spawnSpeed;
        }
    }

    IEnumerator SpawnBubbles()
    {
        if (intenso.IsHellActive()) bubbleFactory.CreateBubble(mainCamera, bubbleSpeed, bubbleMaxSpeed, true);
        else bubbleFactory.CreateBubble(mainCamera, bubbleSpeed, bubbleMaxSpeed, false);

        IncreaseSpawnSpeed();
        yield return null;
    }

    private void IncreaseSpawnSpeed()
    {
        if (rootProgression) spawnSpeed = Mathf.Ceil(Mathf.Sqrt(Time.time));
        else if (powProgression) spawnSpeed = Mathf.Ceil(Mathf.Pow(Time.time, 2));
        else if (cubicProgression) spawnSpeed = Mathf.Ceil(Mathf.Pow(Time.time, 3));
        else if (sigmoidProgression) spawnSpeed = Mathf.Lerp(1f, 10f, Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(0f, 10f, Time.time)));
        else if (logarithmicProgression) spawnSpeed = Mathf.Lerp(1f, Mathf.Log(Time.time + 1f) * 5f, 0.05f);
        else spawnSpeed = Mathf.Ceil(Time.time);
    }

    private void RemoveAllOtherProgressions()
    {
        rootProgression = false;
        powProgression = false;
        cubicProgression = false;
        sigmoidProgression = false;
        logarithmicProgression = false;
    }

    public void SetCubicProgression()
    {
        RemoveAllOtherProgressions();
        cubicProgression = true;
    }

    public void SetLogarithmicProgression()
    {
        RemoveAllOtherProgressions();
        logarithmicProgression = true;
    }
}



