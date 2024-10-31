using Photon.Pun;
using System.Collections;
using UnityEngine;

namespace GCRR.VirtualMap
{
    public class SpawnerManager : MonoBehaviourPun
    {
        [SerializeField] private PhotonView pv;
        private PlayerManager pm;

        [SerializeField] private Transform[] tr_Spawners;

        private void Awake()
        {
            pm = GameObject.FindGameObjectWithTag("Player").transform.root.GetComponent<PlayerManager>();
            StartCoroutine(UpdateSpawner());
        }

        /// <summary>
        /// 스포너 위치정보를 반환
        /// </summary>
        /// <param name="chairID"></param>
        /// <returns></returns>
        public Transform GetSpawnPos(int chairID)
        {
            return tr_Spawners[chairID].transform;
        }

        /// <summary>
        /// 빈 스포너 반환
        /// </summary>
        /// <returns></returns>
        public int EmptySpawner()
        {
            if (PhotonNetwork.IsConnected)
            {
                PlayerSpawner spawner = tr_Spawners[PhotonNetwork.PlayerList.Length - 1].GetComponent<PlayerSpawner>();
                return spawner.spID;
            }
            else return 0;
        }

        /// <summary>
        /// 스포너 정보 변경(RPC로 공유해서 사용중인지 판단)
        /// </summary>
        /// <param name="spawnerID"></param>
        public void InitSpawner(int spawnerID)
        {
            if (spawnerID < 0) return;

            if (PhotonNetwork.IsConnected)
            {
                if (pv == null || !pv.IsMine) return;
                pv?.RPC("InitSpawnerRPC", RpcTarget.All, spawnerID);
            }
        }

        /// <summary>
        /// 플레이어가 위치를 이동했을때 스포너 사용 변경
        /// </summary>
        /// <param name="oldSpawnerID"></param>
        /// <param name="newSpawnerID"></param>
        public void SetSpawner(int oldSpawnerID, int newSpawnerID)
        {
            if (PhotonNetwork.IsConnected)
            {
                if (pv == null || !pv.IsMine) return;
                pv?.RPC("SetSpawnerRPC", RpcTarget.All, oldSpawnerID, newSpawnerID);
            }
            else
            {
                SetSpawnerRPC(oldSpawnerID, newSpawnerID);
            }
        }

        /// <summary>
        /// RPC를 통한 스포너 사용 유무 변경
        /// </summary>
        /// <param name="spawnerID"></param>
        [PunRPC]
        void InitSpawnerRPC(int spawnerID)
        {
            tr_Spawners[spawnerID].GetComponent<PlayerSpawner>().EnableSpawner();
        }

        /// <summary>
        /// RPC를 통한 스포너 이동 사용 유무 변경
        /// </summary>
        /// <param name="oldSpawnerID"></param>
        /// <param name="newSpawnerID"></param>
        [PunRPC]
        void SetSpawnerRPC(int oldSpawnerID, int newSpawnerID)
        {
            if (oldSpawnerID != -1) tr_Spawners[oldSpawnerID].GetComponent<PlayerSpawner>().DisableSpawner();
            tr_Spawners[newSpawnerID].GetComponent<PlayerSpawner>().EnableSpawner();
        }

        /// <summary>
        /// 스포너 사용 유무 업데이트
        /// </summary>
        /// <returns></returns>
        IEnumerator UpdateSpawner()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.5f);
                InitSpawner(pm.SpawnerID);
            }
        }
    }
}