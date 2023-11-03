using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

public class WatchList_Object : UdonSharpBehaviour
{
    [HideInInspector] public AdminUtilExtended root;
    [HideInInspector] public VRCPlayerApi Player;

    public float FollowSpeed = 3.25f;
    public Vector3 Offset = new Vector3(0, 1, 0);

    private float MinDistance = 0.02f;
    void Awake()
    {
        if (root == null)
            root = gameObject.GetComponentInParent<AdminUtilExtended>();
    }
    public void FixedUpdate()
    {
        if (Player != null)
        {
            Vector3 targetPosition = Player.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position + Offset;
            transform.LookAt(Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position, Vector3.up);
            transform.Rotate(new Vector3(90f, 0, 0));
            if (Vector3.Distance(targetPosition, transform.position) < MinDistance)
                return;
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * FollowSpeed);
        }
        else
            gameObject.SetActive(false);
    }
    public override void OnPlayerLeft(VRCPlayerApi player)
    {
        if (player.playerId == Player.playerId)
        {
            Player = null;
            transform.position = new Vector3(0, -10, 0);
        }
    }

}
