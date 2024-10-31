using UnityEngine;

namespace GCRR.VirtualMap
{
    public class UIManager : MonoBehaviour
    {
        private static UIManager instance;
        public static UIManager Instance => instance;

        [SerializeField] private VirtualMapUIManager vmUIM;
        public VirtualMapUIManager VMUIM => vmUIM;

        [SerializeField] private ScenarioUIManager sUIM;
        public ScenarioUIManager SUIM => sUIM;

        [SerializeField] private ObjectSelectUIManager osUIM;
        public ObjectSelectUIManager OSUIM=>osUIM;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
        }
    }
}