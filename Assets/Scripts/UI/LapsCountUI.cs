using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LapsCountUI : NetworkBehaviour
{
    private Player _player;
    [SerializeField] private TextMeshProUGUI _tmp;
    private int _currentLaps;

    public void OnEnable()
    {
        _player = NetworkPlayer.Local.GetComponent<Player>();
        _player.OnLapFinish += UpdateLaps;
        UpdateLaps();
    }

    public void HideUI()
    {
        _tmp.enabled = false;
    }

    public void OnDestroy()
    {
        _player.OnLapFinish -= UpdateLaps;
    }

    public void StartRace()
    {
        _tmp.enabled = true;
    }

    public void FinishRace()
    {
        gameObject.SetActive(false);
    }
    private void UpdateLaps()
    {
        _currentLaps = _player.lapsCount + 1;
        _tmp.text = "Laps \n" + _currentLaps + "/3";
    }

}
