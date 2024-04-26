using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class IntroVideo : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public RawImage rawImage;
    AsyncOperation asyncOperation;
    void Start()
    {
        Cursor.visible = false;
        videoPlayer.prepareCompleted += VideoStarted;
        videoPlayer.loopPointReached += CompleteLoadScene;
    }
    private void VideoStarted(VideoPlayer vp)
    {
        rawImage.color = Color.white;
        StartCoroutine(StartLoadScene());
    }

    private void CompleteLoadScene(VideoPlayer vp)
    {
        asyncOperation.allowSceneActivation = true;
    }

    private IEnumerator StartLoadScene()
    {
        asyncOperation = SceneManager.LoadSceneAsync("Game");
        asyncOperation.allowSceneActivation = false;
        while (!asyncOperation.isDone)
        {
            yield return null;
        }
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            CompleteLoadScene(new VideoPlayer());
        }
    }
}
