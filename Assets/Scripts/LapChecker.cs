using Fusion;
using UnityEngine;

public class LapChecker : NetworkBehaviour
{
    [Networked] private bool lapFinish { get; set; }
    public int check, checkNeeded;
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Player player))
        {
            if (player.mapZone != checkNeeded)
            {
                return;
            }

            player.mapZone = check;
            if (checkNeeded == 3)
            {
                player.LapFinish = !player.LapFinish;
            }
        }
    }
}
