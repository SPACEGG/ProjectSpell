using System;
using Unity.Netcode;
using UnityEngine;

[Obsolete("This class is obsolete. Use NetworkCapsuleTest instead.")]
public class NetworkCapsuleTest : NetworkBehaviour
{
    private Rigidbody rb;

    public override void OnNetworkSpawn()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (!IsLocalPlayer)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space)) rb.AddForce(Vector3.up * 10f, ForceMode.Impulse);
    }
}