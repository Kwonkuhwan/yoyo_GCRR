using System;

namespace GCRR.VirtualMap
{
    [Serializable]
    public class ObjectData
    {
        public int objectID;             // 오브젝트 ID
        public int forceSize;            // 부대 규모
        public string forceCode;         // 군대 부호 코드
        public string unitName;          // 부대명
        public string unitCode;          // 부대 약어명
        public bool isAlly;              // 적/아 구분
        public double unitLat;            // 부대 위도
        public double unitLon;            // 부대 경도
        public float height;             // 부대 고도
        public bool showForceMark;       // 군대 부호 도시 여부
        public string mission;           // 임무내용
        public int movingDirection;      // 이동 방향
        
        public byte[] Serialization()
        {
            byte[] objectIDBytes = BitConverter.GetBytes(objectID);
            byte[] forceSizeBytes = BitConverter.GetBytes(forceSize);
            byte[] forceCodeBytes = UTILS.StringToBytes(forceCode);
            byte[] unitNameBytes = UTILS.StringToBytes(unitName);
            byte[] unitCodeBytes = UTILS.StringToBytes(unitCode);
            byte[] allyBytes = BitConverter.GetBytes(isAlly);
            byte[] latBytes = BitConverter.GetBytes(unitLat);
            byte[] lonBytes = BitConverter.GetBytes(unitLon);
            byte[] heightBytes = BitConverter.GetBytes(height);
            byte[] showForceMarkBytes = BitConverter.GetBytes(showForceMark);
            byte[] missionBytes = UTILS.StringToBytes(mission);
            byte[] movingDirectionbBytes = BitConverter.GetBytes(movingDirection);

            ushort len = (ushort)(objectIDBytes.Length + forceSizeBytes.Length + forceCodeBytes.Length + unitNameBytes.Length + unitCodeBytes.Length
                                    + allyBytes.Length + latBytes.Length + lonBytes.Length + heightBytes.Length + showForceMarkBytes.Length + missionBytes.Length + movingDirectionbBytes.Length);

            byte[] lenBytes = BitConverter.GetBytes(len);

            byte[] result = UTILS.AddBytes(lenBytes, objectIDBytes);
            result = UTILS.AddBytes(result, forceSizeBytes);
            result = UTILS.AddBytes(result, forceCodeBytes);
            result = UTILS.AddBytes(result, unitNameBytes);
            result = UTILS.AddBytes(result, unitCodeBytes);
            result = UTILS.AddBytes(result, allyBytes);
            result = UTILS.AddBytes(result, latBytes);
            result = UTILS.AddBytes(result, lonBytes);
            result = UTILS.AddBytes(result, heightBytes);
            result = UTILS.AddBytes(result, showForceMarkBytes);
            result = UTILS.AddBytes(result, missionBytes);
            result = UTILS.AddBytes(result, movingDirectionbBytes);
            return result;
        }

        public static ObjectData DeSerialization(byte[] bytes)
        {
            ObjectData objectData = new ObjectData();
            int stringLen = 0;
            int parsCnt = 0;

            #region objectID
            objectData.objectID = (UTILS.BytesToInt(bytes, ref parsCnt));
            #endregion

            #region forceSize
            objectData.forceSize = (UTILS.BytesToInt(bytes, ref parsCnt));
            #endregion

            #region forceCode
            stringLen = UTILS.BytesToUShort(bytes, ref parsCnt);

            objectData.forceCode = (UTILS.BytesToString(bytes, stringLen, ref parsCnt));
            #endregion

            #region unitName
            stringLen = UTILS.BytesToUShort(bytes, ref parsCnt);

            objectData.unitName = (UTILS.BytesToString(bytes, stringLen, ref parsCnt));
            #endregion

            #region unitCode
            stringLen = UTILS.BytesToUShort(bytes, ref parsCnt);

            objectData.unitCode = (UTILS.BytesToString(bytes, stringLen, ref parsCnt));
            #endregion

            #region isAlly
            objectData.isAlly = (UTILS.BytesToBoolean(bytes, ref parsCnt));
            #endregion

            #region lat    
            objectData.unitLat = (UTILS.BytesToDouble(bytes, ref parsCnt));
            #endregion

            #region lon
            objectData.unitLon = (UTILS.BytesToDouble(bytes, ref parsCnt));
            #endregion

            #region height
            objectData.height = (UTILS.BytesToFloat(bytes, ref parsCnt));
            #endregion

            #region showForceMark
            objectData.showForceMark = (UTILS.BytesToBoolean(bytes, ref parsCnt));
            #endregion

            #region mission
            stringLen = UTILS.BytesToUShort(bytes, ref parsCnt);

            objectData.mission = (UTILS.BytesToString(bytes, stringLen, ref parsCnt));
            #endregion

            #region movingdirection
            objectData.movingDirection = (UTILS.BytesToInt(bytes, ref parsCnt));
            #endregion

            return objectData;
        }
    }
}