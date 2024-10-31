using Photon.Pun;
using UnityEngine;

namespace GCRR.VirtualMap
{
    public class PlayerManager : MonoBehaviourPunCallbacks
    {
        [SerializeField] private int spawnerID = -1;
        public int SpawnerID
        {
            get => spawnerID;
            set
            {
                if (PhotonNetwork.IsConnected)
                    ConferenceRoomManager.Instance.spawnerMger?.SetSpawner(spawnerID, value);
                spawnerID = value;
            }
        }
    }
}