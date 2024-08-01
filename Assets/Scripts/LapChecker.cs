using Fusion;
using UnityEngine;

public class LapChecker : NetworkBehaviour
{
    public int check, checkNeeded;
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Player player))
        {
            if (player.MapZone != checkNeeded)
            {
                return;
            }

            player.MapZone = check;
            if (checkNeeded == 3)
            {
                print("LapTerminada");
                player.LapFinish = !player.LapFinish;
            }
        }
    }
}
