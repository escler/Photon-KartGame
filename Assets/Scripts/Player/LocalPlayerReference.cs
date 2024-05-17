using Fusion;

public class LocalPlayerReference : NetworkBehaviour
{
    public static LocalPlayerReference Instance { get; private set; }
    public bool isLocal;
    public override void Spawned()
    {
        if (!HasStateAuthority) return;

        Instance = this;
        isLocal = true;
    }
}
