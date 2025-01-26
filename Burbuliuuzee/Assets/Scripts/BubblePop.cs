using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class BubblePop : MonoBehaviour
{
    private const int simplePopScore = 1;
    [SerializeField] private GameObject paw;

    [SerializeField] private GameObject audioManager;
    [SerializeField] private GameObject ScoreManager;

    void Update()
    {
        HandleTouchInput();
    }

    private void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            foreach (UnityEngine.Touch touch in Input.touches)
            {
                Vector3 touchPosition = TouchPosition(touch);
                if (touch.phase == TouchPhase.Began)
                {
                    Vector3 newPosition = new Vector3(touchPosition.x, touchPosition.y, 1);
                    GameObject pawGO = Instantiate(paw, touchPosition, Quaternion.identity);
                    FadeGameObject(pawGO, 0f, 1f);

                    RaycastHit2D hit = Physics2D.Raycast(touchPosition, Vector2.zero);
                    if (hit.collider != null)
                    {
                        if (hit.collider.tag == "Bubble" || hit.collider.tag == "Red" || hit.collider.tag == "Green" || hit.collider.tag == "Black")
                        {
                            ScoreController scoreController = GameObject.Find("ScoreController").GetComponent<ScoreController>();
                            Destroy(hit.collider.gameObject);
                            audioManager.GetComponents<AudioSource>()[1].Play();
                            if(hit.collider.tag == "Bubble" || hit.collider.tag == "Green" || hit.collider.tag == "Black")
                            {
                                scoreController.AddScore(simplePopScore);
                                if (hit.collider.tag == "Green") scoreController.SubstractLives(-1);
                                else if (hit.collider.tag == "Black") GetComponent<Intenso>().UnleashHell();
                            }
                            else scoreController.SubstractLives(1);
                        }
                    }
                }
            }
        }
    }

    private Vector3 TouchPosition(UnityEngine.Touch touch)
    {
        Vector3 position = Camera.main.ScreenToWorldPoint(touch.position);
        position.z = 0;
        return position;
    }

    private void FadeGameObject(GameObject gameObject, float targetAlpha, float duration)
    {
        SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) StartCoroutine(FadeCoroutine(spriteRenderer, targetAlpha, duration));
        Destroy(gameObject, duration + 0.5f);
    }

    private IEnumerator FadeCoroutine(SpriteRenderer spriteRenderer, float targetAlpha, float duration)
    {
        Color startColor = spriteRenderer.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, targetAlpha);
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            if (spriteRenderer.sprite != null)
            {
                spriteRenderer.color = Color.Lerp(startColor, targetColor, timeElapsed / duration);
                timeElapsed += Time.deltaTime;

            }
            yield return null;
        }

        spriteRenderer.color = targetColor;
    }
}
