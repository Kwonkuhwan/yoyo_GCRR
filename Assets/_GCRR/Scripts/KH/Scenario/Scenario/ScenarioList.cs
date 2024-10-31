using System;

namespace GCRR.VirtualMap
{
    [Serializable]
    public class ScenarioList
    {
        //public string scenarioID;           // 시나리오 ID (서버 자동생성)
        //public string scenarioName;         // 시나리오 명
        //public string folderName;           // 폴더 명
        //public float lat;                   // 위도
        //public float lon;                   // 경도
        //public string writer;               // 작성자
        //public string mapType;              // 맵 타입(VR 3D만 전시)
        //public int scale;                   // 축척
        //public int range;                   // 전시반경
        //public string description;          // 설명
        //public string rightToControl;       // 소유권
        //public string writeDay;             // 작성일
        //public string revisionDate;         // 수정일

        public string scenarioName;     // 시나리오 명
        public string folderName;       // 폴더 명
        public double lat;              // 위도
        public double lon;              // 경도
        public string description;      // 설명
        public string scenarioID;       // 시나리오 ID
        public string creationDate;     // 생성 날짜
        public string revisionDate;     // 수정 날짜
        public int range;               // 반경
        public string control;          // 제어권
        public int isPlaying;

        public object Clone()
        {
            return MemberwiseClone();
        }

        public void Clear()
        {
            scenarioName = string.Empty;
            folderName = string.Empty;
            lat = 0;
            lon = 0;
            description = string.Empty;
            scenarioID = string.Empty;
            creationDate = string.Empty;
            revisionDate = string.Empty;
            range = 1;
            control = string.Empty;
            isPlaying = 0;
        }

        public byte[] Serialize()
        {
            byte[] scenarioNameBytes = UTILS.StringToBytes(scenarioName);
            byte[] folderNameBytes = UTILS.StringToBytes(folderName);
            byte[] latBytes = BitConverter.GetBytes(lat);
            byte[] lonBytes = BitConverter.GetBytes(lon);
            byte[] descriptionBytes = UTILS.StringToBytes(description);
            byte[] scenarioIDBytes = UTILS.StringToBytes(scenarioID);
            byte[] creationDateBytes = UTILS.StringToBytes(creationDate);
            byte[] revisionDateBytes = UTILS.StringToBytes(revisionDate);
            byte[] rangeBytes = BitConverter.GetBytes(range);
            byte[] controlBytes = UTILS.StringToBytes(control);
            byte[] isPlayingBytes = BitConverter.GetBytes(isPlaying);

            byte[] result = UTILS.AddBytes(scenarioNameBytes, folderNameBytes);
            result = UTILS.AddBytes(result, latBytes);
            result = UTILS.AddBytes(result, lonBytes);
            result = UTILS.AddBytes(result, descriptionBytes);
            result = UTILS.AddBytes(result, scenarioIDBytes);
            result = UTILS.AddBytes(result, creationDateBytes);
            result = UTILS.AddBytes(result, revisionDateBytes);
            result = UTILS.AddBytes(result, rangeBytes);
            result = UTILS.AddBytes(result, controlBytes);
            result = UTILS.AddBytes(result, isPlayingBytes);

            return result;
        }

        static public ScenarioList DeSerialization(byte[] bytes)
        {
            int parsCnt = 0;

            ScenarioList scenarioList = new ScenarioList();

            int lenLength = UTILS.BytesToUShort(bytes, ref parsCnt);
            scenarioList.scenarioName = UTILS.BytesToString(bytes, lenLength, ref parsCnt);

            lenLength = UTILS.BytesToUShort(bytes, ref parsCnt);
            scenarioList.folderName = UTILS.BytesToString(bytes, lenLength, ref parsCnt);

            scenarioList.lat = UTILS.BytesToDouble(bytes, ref parsCnt);

            scenarioList.lon = UTILS.BytesToDouble(bytes, ref parsCnt);

            lenLength = UTILS.BytesToUShort(bytes, ref parsCnt);
            scenarioList.description = UTILS.BytesToString(bytes, lenLength, ref parsCnt);

            lenLength = UTILS.BytesToUShort(bytes, ref parsCnt);
            scenarioList.scenarioID = UTILS.BytesToString(bytes, lenLength, ref parsCnt);

            lenLength = UTILS.BytesToUShort(bytes, ref parsCnt);
            scenarioList.creationDate = UTILS.BytesToString(bytes, lenLength, ref parsCnt);

            lenLength = UTILS.BytesToUShort(bytes, ref parsCnt);
            scenarioList.revisionDate = UTILS.BytesToString(bytes, lenLength, ref parsCnt);

            scenarioList.range = UTILS.BytesToInt(bytes, ref parsCnt);

            lenLength = UTILS.BytesToUShort(bytes, ref parsCnt);
            scenarioList.control = UTILS.BytesToString(bytes, lenLength, ref parsCnt);

            scenarioList.isPlaying = UTILS.BytesToInt(bytes, ref parsCnt);

            return scenarioList;
        }
    }
}