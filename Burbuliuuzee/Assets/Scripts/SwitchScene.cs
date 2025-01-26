using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class SwitchScene : MonoBehaviour
{
    [SerializeField] VideoPlayer video;

    void Start()
    {
        video = GetComponent<VideoPlayer>();
        StartCoroutine(DelaySceneSwitch((float)video.length));
    }

    private IEnumerator DelaySceneSwitch(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
