using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuHandler : MonoBehaviour
{
    [SerializeField] private NetworkRunnerHandler _networkHandler;

    [Header("Panels")]
    [SerializeField] private GameObject _initialPanel;
    [SerializeField] private GameObject _sessionBrowserPanel;
    [SerializeField] private GameObject _hostGamePanel;
    [SerializeField] private GameObject _statusPanel;
    [SerializeField] private GameObject _controlsPanel;

    [Header("Buttons")]
    [SerializeField] private Button _joinLobbyBTN;
    [SerializeField] private Button _goToHostPanelBTN;
    [SerializeField] private Button _hostBTN;
    [SerializeField] private Button _controlsBTN;
    [SerializeField] private Button _exitBTN;
    [SerializeField] private Button _closeControlsBTN;
    [SerializeField] private Button _backFromSessionListBTN;
    [SerializeField] private Button _backFromHostPanel;
    
    [Header("InputFields")]
    [SerializeField] private TMP_InputField _hostSessionName;
    
    [Header("Texts")]
    [SerializeField] private TMP_Text _statusText;
    
    void Start()
    {
        _joinLobbyBTN.onClick.AddListener(Btn_JoinLobby);
        _goToHostPanelBTN.onClick.AddListener(Btn_ShowHostPanel);
        _hostBTN.onClick.AddListener(Btn_CreateGameSession);
        _controlsBTN.onClick.AddListener(Btn_ShowControls);
        _exitBTN.onClick.AddListener(() => Application.Quit());
        _closeControlsBTN.onClick.AddListener(Btn_HideControls);
        _backFromSessionListBTN.onClick.AddListener(Btn_BackFromSessionList);
        _backFromHostPanel.onClick.AddListener(Btn_BackFromHostPanel);

        _networkHandler.OnJoinedLobby += () =>
        {
            _statusPanel.SetActive(false);
            _sessionBrowserPanel.SetActive(true);
        };
    }

    void Btn_JoinLobby()
    {
        SoundManager.Instance.PlayClickSound();
        _networkHandler.JoinLobby();

        _initialPanel.SetActive(false);
        _statusPanel.SetActive(true);

        _statusText.text = "Joining Lobby...";
    }

    void Btn_BackFromSessionList()
    {
        SoundManager.Instance.PlayClickSound();
        _sessionBrowserPanel.SetActive(false);

        _initialPanel.SetActive(true);
    }
    
    void Btn_ShowHostPanel()
    {
        SoundManager.Instance.PlayClickSound();
        _sessionBrowserPanel.SetActive(false);
        _hostGamePanel.SetActive(true);
    }

    void Btn_BackFromHostPanel()
    {
        SoundManager.Instance.PlayClickSound();
        _sessionBrowserPanel.SetActive(true);
        _hostGamePanel.SetActive(false);
    }
    
    void Btn_CreateGameSession()
    {
        SoundManager.Instance.PlayClickSound();
        _hostBTN.interactable = false;
        _networkHandler.CreateGame(_hostSessionName.text, "Lobby");
    }

    void Btn_ShowControls()
    {
        SoundManager.Instance.PlayClickSound();
        _controlsPanel.SetActive(true);
    }

    void Btn_HideControls()
    {
        SoundManager.Instance.PlayClickSound();
        _controlsPanel.SetActive(false);
    }
    
}
