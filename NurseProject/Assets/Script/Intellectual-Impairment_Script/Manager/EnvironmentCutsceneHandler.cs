using UnityEngine;
using UnityEngine.Video;
using UnityEngine.Events;

public class EnvironmentCutsceneHandler : MonoBehaviour
{
    [Header("Cutscene Components")]
    public GameObject cutscenePanel;
    public VideoPlayer videoPlayer;

    [Header("Events")]
    public UnityEvent onCutsceneFinished;

    private void OnEnable()
    {
        if (videoPlayer)
        {
            videoPlayer.loopPointReached += OnVideoFinished;
        }
    }

    private void OnDisable()
    {
        if (videoPlayer)
        {
            videoPlayer.loopPointReached -= OnVideoFinished;
        }
    }

    public string videoFileName = "animated.mp4";

    public void PlayCutscene()
    {
        if (cutscenePanel) cutscenePanel.SetActive(true);

        if (videoPlayer)
        {
            // Set URL for WebGL compatibility
            string videoPath = Application.streamingAssetsPath + "/" + videoFileName;
            videoPlayer.url = videoPath;
            videoPlayer.source = VideoSource.Url;
            
            videoPlayer.time = 0;
            videoPlayer.Play();
        }
        else
        {
            Debug.LogWarning("Video Player not assigned. Skipping cutscene.");
            OnVideoFinished(null);
        }
    }

    public void StopCutscene()
    {
        if (videoPlayer && videoPlayer.isPlaying)
        {
            videoPlayer.Stop();
        }
        OnVideoFinished(videoPlayer);
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        Debug.Log("Video Finished (or Skipped) - Handler");
        if (cutscenePanel) cutscenePanel.SetActive(false);
        onCutsceneFinished?.Invoke();
    }

    public bool IsPlaying()
    {
        return videoPlayer && videoPlayer.isPlaying;
    }
}
