using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public class ScoreController : MonoBehaviour
{
    [SerializeField] private TMP_Text gameOverUI;

    [SerializeField] private int lives = 9;
    [SerializeField] private TMP_Text livesUI;

    [SerializeField] private int score = 0;
    [SerializeField] private TMP_Text scoreUI;

    [SerializeField] private float burstScaleFactor = 1.2f;
    [SerializeField] private float burstDuration = 0.2f;

    [SerializeField] GameObject audioController;

    private Vector3 originalScale;

    private void Start()
    {
        if (scoreUI == null) scoreUI = GetComponent<TMP_Text>();

        originalScale = scoreUI.transform.localScale;

        UpdateScore();
        UpdateLives();
    }

    private void Update()
    {
        if(lives == 0) RestartLevel();
    }

    public void SubstractLives(int substractor)
    {
        if (lives > 0)
        {
            lives -= substractor;
            UpdateLives();
        }
        else HandleGameOver();
    }

    private void HandleGameOver()
    {
        livesUI.text = "RIP :3";
        scoreUI.gameObject.SetActive(false);
        GameObject touchController;
        if (touchController = GameObject.Find("TouchController")) touchController.SetActive(false);
        gameOverUI.text = $"GAME OVER\nYOU SCORED: {score}\n\nTOUCH TO RESTART";
        gameOverUI.gameObject.SetActive(true);
    }

    private void UpdateLives()
    {
        livesUI.text = $"[LIVES ] {lives}";
    }

    public void AddScore(int addative)
    {
        score += addative;
        UpdateScore();
    }

    private void UpdateScore()
    {
        if (score % 10 == 0)
        {
            audioController.GetComponent<AudioController>().PlayRandomCatMeow();
        }
        scoreUI.text = $"< POPS> {score}";
        TriggerBurst();
    }

    public void TriggerBurst()
    {
        StartCoroutine(BurstEffect());
    }

    private IEnumerator BurstEffect()
    {
        scoreUI.transform.localScale = originalScale;

        Vector3 targetScale = originalScale * burstScaleFactor;
        float elapsedTime = 0f;

        while (elapsedTime < burstDuration)
        {
            float lerpValue = elapsedTime / burstDuration;
            scoreUI.transform.localScale = Vector3.Lerp(originalScale, targetScale, lerpValue);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        scoreUI.transform.localScale = targetScale;

        elapsedTime = 0f;
        while (elapsedTime < burstDuration)
        {
            float lerpValue = elapsedTime / burstDuration;
            scoreUI.transform.localScale = Vector3.Lerp(targetScale, originalScale, lerpValue);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        scoreUI.transform.localScale = originalScale;
    }

    private void RestartLevel()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public int GetCurrentLives()
    {
        return lives;
    }

    public void SetCurrentLives(int newLives)
    {
        lives = newLives;
        UpdateLives();
    }
}
