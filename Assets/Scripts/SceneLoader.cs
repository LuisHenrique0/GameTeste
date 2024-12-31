using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private AudioSource battleMusic;

    void Start()
    {
        battleMusic = GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "BattleScene")
        {
            battleMusic.Play();
        }
        else
        {
            battleMusic.Stop();
        }
    }
}
