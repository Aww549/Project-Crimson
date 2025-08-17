using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    private static SceneTransitionManager _instance;
    public static SceneTransitionManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<SceneTransitionManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("SceneTransitionManager");
                    _instance = go.AddComponent<SceneTransitionManager>();
                    DontDestroyOnLoad(go);
                }
            }
            return _instance;
        }
    }

    public const string CAMP_SCENE = "CampScene";
    public const string MISSIONS_SCENE = "MissionsScene";

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadMissionsScene()
    {
        SceneManager.LoadScene(MISSIONS_SCENE);
    }

    public void LoadCampScene()
    {
        SceneManager.LoadScene(CAMP_SCENE);
    }
}