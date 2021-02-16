using UnityEngine;
using Mirror;

public class SPlayerController : NetworkBehaviour
{
    private CharacterController cc;

    private Vector3 playerVelocity;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        cc = GetComponent<CharacterController>();
        Camera.main.transform.SetParent(this.transform);
        Camera.main.transform.localPosition = new Vector3(0, 0.6f, 0);
    }

    [Client]
    private void Update()
    {
        if (!isLocalPlayer) return;

        Vector3 wishDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

        CmdMovement(wishDir);
    }

    [Command]
    private void CmdMovement(Vector3 wishDir)
    {
        playerVelocity += wishDir * 5;

        RpcMove(playerVelocity);
    }

    [ClientRpc]
    private void RpcMove(Vector3 velocity)
    {
        cc.Move(velocity * Time.deltaTime);
    }
}
