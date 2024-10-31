using System.Security.Cryptography;
using System.Threading.Tasks;
using UnityEngine;

namespace GCRR.VirtualMap {
    public class LocalSave : MonoBehaviour
    {
        [SerializeField] private VirtualMapUIManager virtualMapUIManager;

        public void LocalExistSave()
        {
            string decs = string.Empty;

            bool error = false;
            if (DataSyncManager.Instance.Save_LocalData())
            {
                decs = $"로컬에 정상적으로 저장되었습니다.";
            }
            else
            {
                decs = $"로컬 저장에 문제가 있거나, 클라우드 모드에서는 사용할 수 없습니다.";
                error = true;
            }

            virtualMapUIManager.ShowMessagePanelAction(decs, error);
            //StartCoroutine(virtualMapUIManager.ShowMessagePanel(decs, error));

            Task.Delay(2000).Wait();
            gameObject.SetActive(false);
        }

        public void LocalNewSave()
        {
            string decs = string.Empty;

            bool error = false;
            if (DataSyncManager.Instance.Save_NewLocalData())
            {
                decs = $"로컬에 정상적으로 저장되었습니다.";
            }
            else
            {
                decs = $"로컬 저장에 문제가 있습니다.";
                error = true;
            }

            virtualMapUIManager.ShowMessagePanelAction(decs, error);
            //StartCoroutine(virtualMapUIManager.ShowMessagePanel(decs, error));

            Task.Delay(2000).Wait();
            gameObject.SetActive(false);
        }
    }
}
