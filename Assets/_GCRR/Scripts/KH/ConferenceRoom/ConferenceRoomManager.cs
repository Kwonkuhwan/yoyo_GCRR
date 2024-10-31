using Photon.Pun;
using System.Collections;
using UnityEngine;

namespace GCRR.VirtualMap
{
    public class ConferenceRoomManager : MonoBehaviourPun
    {
        private static ConferenceRoomManager instance;
        public static ConferenceRoomManager Instance => instance;

        [SerializeField] private SpawnerManager spawnerManager;

        public SpawnerManager spawnerMger { get => spawnerManager; set => spawnerManager = value; }

        private PlayerManager playerManager;

        private bool isSetSpawn;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
        }

        private void Start()
        {
            
        }

        private void Update()
        {
            if (!isSetSpawn)
            {
                if ((GameManager.Instance.isCloud || GameManager.Instance.isLocation || !PhotonNetwork.IsConnected))
                {
                    Set_User();
                }
            }
        }

        /// 작성 : KKH
        /// <summary>
        /// 유저 배치 딜레이
        /// </summary>
        /// <returns></returns>
        public IEnumerator Delay_Set_User()
        {
            yield return null;

            // [추가] KKH : NetWork 플레이어 생성
            NetworkManager_.Instance.SpawnedPlayer(Vector3.zero, Quaternion.identity);

            // [추가] KKH : 플레이어 배치
            Set_User();
        }

        /// 작성 : KKH
        /// <summary>
        /// 유저 배치
        /// </summary>
        public void Set_User()
        {
            // [추가] KKH : 플레이어 검색
            GameObject orgPalyer = GameObject.FindGameObjectWithTag("Player");

            if(orgPalyer == null)
            {
                return;
            }

            isSetSpawn = true;
            // [추가] KKH : 플레이어 매니저 가져오기
            playerManager = orgPalyer.transform.root.GetComponent<PlayerManager>();

            // [추가] KKH : 비어있는 Spawner 검색
            int spawnerID = spawnerMger.EmptySpawner();

            // [추가] KKH : 플레이어 스포너에 배치
            InitUserPos(orgPalyer, spawnerID);
        }

        /// <summary>
        /// 플레이어 위치 초기화
        /// </summary>
        /// <param name="player">플레이어 오브젝트</param>
        /// <param name="spawnerID">스포너 ID</param>
        private void InitUserPos(GameObject player, int spawnerID)
        {
            // [추가] KKH : 플레이어 스포너 ID 설정
            playerManager.SpawnerID = spawnerID;

            // [추가] KKH : 스포너의 위치 가져오기
            Transform spawnPos = spawnerMger.GetSpawnPos(spawnerID);

            // [추가] KKH : 플레이어 위치 변경
            Vector3 pos = spawnPos.position;
            pos.y = 0;
            player.transform.position = pos;
            player.transform.rotation = spawnPos.rotation;
        }
    }
}