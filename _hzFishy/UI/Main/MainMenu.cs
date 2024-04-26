using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private PlayerController _playerC;
    private Animator _animator;
    [SerializeField] private GameObject _statsCanvas;
    [SerializeField] private GameObject _levelGenerationManager;
    public GameObject PlayButton;


    [SerializeField] private NavigationSelectables _navigationSelectables;
    private void Start()
    {
        _playerC = FindFirstObjectByType<PlayerController>();
        _animator = GetComponent<Animator>();
        Cursor.visible = false;
        if (CrossSceneDataHolder.InstantStart)
        {
            CrossSceneDataHolder.InstantStart = false;
            if (CrossSceneDataHolder.StartInTuto)
            {
                CrossSceneDataHolder.StartInTuto = false;
                StartTutorial();
            }
            else
            {
                StartClick();
            }
        }
        else
        {
            if (!PlayerPrefs.HasKey("NewPlayer"))
            {
                _navigationSelectables.RemoveSelectable(0);
                PlayButton.SetActive(false);
                PlayerPrefs.SetInt("NewPlayer", 1);
            }
        }
    }

    public void StartClick()
    {
        _playerC.UpdateMenuStatus(false);
        _animator.SetBool("Fade", true);
        _statsCanvas.GetComponent<Stats>().Fade(true);
        _navigationSelectables.DisableInputs();
        _playerC.InTutoLevel = false;
    }

    public void StartTutorial()
    {
        _levelGenerationManager.SetActive(false);
        SceneManager.LoadScene("Tuto", LoadSceneMode.Additive);
        _playerC.UpdateMenuStatus(false);
        _animator.SetBool("Fade", true);
        _navigationSelectables.DisableInputs();
        _playerC.InTutoLevel = true;
    }

    public void QuitLeave()
    {
        Application.Quit();
    }

    public void FadeMenuFinish()
    {
        gameObject.SetActive(false);
    }
}
