using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Intenso : MonoBehaviour
{
    [SerializeField] private GameObject audioManager;
    private AudioController audioController;
    [SerializeField] private GameObject bubbleSpawner;
    private SpawnBubble bubbleController;
    [SerializeField] private GameObject scoreManager;
    private ScoreController scoreController;

    [SerializeField] private float hellLength = 10f;
    private int previousLives;

    private bool hellUnleashed = false;

    void Start()
    {
        audioController = audioManager.GetComponent<AudioController>();
        bubbleController = bubbleSpawner.GetComponent<SpawnBubble>();
        scoreController = scoreManager.GetComponent<ScoreController>();
    }

    public void UnleashHell()
    {
        DestroyAllBubbles();
        hellUnleashed = true;
        previousLives = scoreController.GetCurrentLives();
        audioManager.GetComponents<AudioSource>()[0].Stop();
        audioManager.GetComponents<AudioSource>()[2].Play();
        scoreController.SubstractLives(-999999999);
        bubbleController.SetCubicProgression();
        ResetToNormal();
    }

    private void ResetToNormal()
    {
        StartCoroutine(ResetToNormalAfterDelay(hellLength));
    }

    private IEnumerator ResetToNormalAfterDelay(float time)
    {
        yield return new WaitForSeconds(time);
        DestroyAllBubbles();
        bubbleController.SetLogarithmicProgression();
        bubbleController.enabled = false;
        ResetToPreviousLives();
        yield return new WaitForSeconds(1f);
        bubbleController.enabled = true;
        audioManager.GetComponents<AudioSource>()[0].Play();
        audioManager.GetComponents<AudioSource>()[2].Stop();
        hellUnleashed = false;
    }

    private void DestroyAllBubbles()
    {
        List<GameObject> bubbles = new List<GameObject>();
        bubbles = FindAllBubblesWithTag("Bubble").ToList<GameObject>();
        bubbles.AddRange<GameObject>(FindAllBubblesWithTag("Red"));
        bubbles.AddRange<GameObject>(FindAllBubblesWithTag("Green"));
        bubbles.AddRange<GameObject>(FindAllBubblesWithTag("Black"));
        foreach (GameObject bubble in bubbles)
        {
            Destroy(bubble.gameObject);
        }
    }

    private void DestroyAllPaws()
    {
        List<GameObject> paws = new List<GameObject>();
        paws = FindAllBubblesWithTag("Paw").ToList<GameObject>();
        foreach (GameObject paw in paws)
        {
            Destroy(paw.gameObject);
        }
    }

    private GameObject[] FindAllBubblesWithTag(string tag)
    {
        return GameObject.FindGameObjectsWithTag(tag);
    }

    private void ResetToPreviousLives()
    {
        scoreController.SetCurrentLives(previousLives);
    }

    public bool IsHellActive()
    {
        return hellUnleashed;
    }
}
