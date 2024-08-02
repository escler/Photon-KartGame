using System.Collections;
using Fusion;
using UnityEngine;

public class LapChecker : NetworkBehaviour
{
    public int check, checkNeeded;
    private bool stop;
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Player player) && !stop)
        {
            if (player.MapZone != checkNeeded)
            {
                return;
            }

            player.MapZone = check;
            if (checkNeeded == 3 && !stop)
            {
                stop = true;
                StartCoroutine(Refresh());
                print("LapTerminada");
                player.UpdateLapInfo();
                player.MapZone = 0;
            }
        }
    }

    IEnumerator Refresh()
    {
        yield return new WaitForSeconds(4f);
        stop = false;
    }
}
