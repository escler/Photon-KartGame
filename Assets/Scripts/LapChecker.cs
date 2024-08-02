using System.Collections;
using Fusion;
using UnityEngine;

public class LapChecker : MonoBehaviour
{
    public int check, checkNeeded;
    private bool stop;
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Player player))
        {
            if (player.MapZone != checkNeeded)
            {
                return;
            }

            if (checkNeeded == 3 && !stop)
            {
                StartCoroutine(Refresh());
                print("LapTerminada");
                player.UpdateLapInfo();
            }
            player.MapZone = check;
        }
    }

    IEnumerator Refresh()
    {
        yield return new WaitForSeconds(4f);
        stop = false;
    }
}
