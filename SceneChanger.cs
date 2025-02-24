using UnityEngine.SceneManagement;

public class SceneChanger : Singleton<SceneChanger>
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void CharacterSelect()
    {
        SceneManager.LoadScene("Select");
    }

    public void Game()
    {
        SceneManager.LoadScene("Game");
        SceneManager.LoadScene("UI", LoadSceneMode.Additive);
    }
}
