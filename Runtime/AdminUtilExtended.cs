using MagmaMc.AdminUtil;
using System;
using UnityEngine;
using VRC.SDKBase;

public class AdminUtilExtended : ConfigRequestHandler
{
    public GameObject BannedObject;

    public string[] AdminList = new string[0];
    public string[] BannedList = new string[0];
    public string[] WatchList = new string[0];

    public WatchList_Object[] WatchListObjectPool = new WatchList_Object[25];
    private bool ShowingWatchList = true;
    public void Awake()
    {
        BannedObject.SetActive(false);
    }
    public override void OnStart()
    {
        base.OnStart();
    }

    public override void OnStringDownloaded(string DownloadedString)
    {
        AdminList = DownloadedString.GetFromKey("Administrators").ToLower();
        BannedList = DownloadedString.GetFromKey("BannedPlayers").ToLower();
        WatchList = DownloadedString.GetFromKey("WatchList").ToLower();
        if (AdminListContains(Networking.LocalPlayer))
        {
            for (int i = 0; i < WatchListObjectPool.Length; i++)
                WatchListObjectPool[i].Player = null;

            // Get all players from the watchlist that are currently in the server

            VRCPlayerApi[] players = VRCPlayerApi.GetPlayers(new VRCPlayerApi[VRCPlayerApi.GetPlayerCount()]);
            VRCPlayerApi[] watchListPlayers = new VRCPlayerApi[players.Length];
            int count = 0;

            foreach (VRCPlayerApi player in players)
            {
                string playerName = player.displayName.ToLower();
                if (WatchList.Contains(playerName))
                {
                    watchListPlayers[count] = player;
                    count++;
                }
            }
            Resize(ref watchListPlayers, count);

            for (int i = 0; i < watchListPlayers.Length; i++)
            {
                WatchListObjectPool[i].Player = watchListPlayers[i];
                WatchListObjectPool[i].gameObject.SetActive(true);
            }
        }
    }
    private static void Resize<T>(ref T[] array, int newSize)
    {
        if (array.Length != newSize)
        {
            T[] newArray = new T[newSize];
            Array.Copy(array, 0, newArray, 0, (array.Length > newSize) ? newSize : array.Length);
            array = newArray;
        }
    }
    public void FixedUpdate()
    {
        if (BannedListContains(Networking.LocalPlayer) && !BannedObject.activeInHierarchy)
        {
            BannedObject.SetActive(true);
            BannedObject.transform.position = new Vector3(2000, 2000, 2000);
        }
    }


    public bool AdminListContains(string Player) => ArrayContains(AdminList, Player, false);
    public bool AdminListContains(VRCPlayerApi Player) => ArrayContains(AdminList, Player.displayName, false);
    public bool AdminListContains(int Player) => ArrayContains(AdminList, VRCPlayerApi.GetPlayerById(Player).displayName, false);

    public bool BannedListContains(string Player) => ArrayContains(BannedList, Player, false);
    public bool BannedListContains(VRCPlayerApi Player) => ArrayContains(BannedList, Player.displayName, false);
    public bool BannedListContains(int Player) => ArrayContains(BannedList, VRCPlayerApi.GetPlayerById(Player).displayName, false);

    public bool WatchListContains(string Player) => ArrayContains(WatchList, Player, false);
    public bool WatchListContains(VRCPlayerApi Player) => ArrayContains(WatchList, Player.displayName, false);
    public bool WatchListContains(int Player) => ArrayContains(WatchList, VRCPlayerApi.GetPlayerById(Player).displayName, false);
}
