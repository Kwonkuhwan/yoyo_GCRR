using System;
using System.Collections.Generic;

namespace GCRR.VirtualMap
{
    [Serializable]
    public class ScenarioData 
    {
        public string scenarioID;               // 시나리오ID    
        public string scenarioName;             // 시나리오명    
        public string folderName;               // 폴더명
        public string writer;                   // 작성자
        public string mapType;                  // 맵 타입
        public float latitude;                  // 위도
        public float longitude;                 // 경도
        public int scale;                       // 스케일
        public int range;                       // 반경
        public int objectCount;                 // 오브젝트 수
        public string description;              // 설명
        public string rightToControl;             // 소유권
        public string writeDay;                 // 작성일
        public string revisionDay;              // 수정일
        public List<ObjectData> objects;        // 오브젝트 리스트

        public ScenarioData() { Clear(); }

        public ScenarioData(string scenarioID, string scenrioName, string folderName, string writer, string mapType,
            float latitude, float lonitude, int scale, int range, int objectCount, string description, bool rightToControl,
            string writeDay, string revisionDay, List<ObjectData> objects)
        {
            Init(scenarioID, scenrioName, folderName, writer, mapType, latitude, lonitude,
                scale, range, objectCount, description, rightToControl, writeDay, revisionDay, objects);
        }

        public void Init(string scenarioID, string scenrioName, string folderName, string writer, string mapType,
            float latitude, float lonitude, int scale, int range, int objectCount, string description, bool rightToControl,
            string writeDay, string revisionDay, List<ObjectData> objects)
        {
            SetScenarioID(scenarioID);
            SetScenarioName(scenrioName);
            SetFolderName(folderName);
            SetWriter(writer);
            SetMapType(mapType);
            SetLatitude(latitude);
            SetLongitude(lonitude);
            SetScale(scale);
            SetRange(range);
            SetObjectCount(objectCount);
            SetDescription(description);
            SetRightToControl(rightToControl);
            SetWriteDay(writeDay);
            SetRevisionDay(revisionDay);
            SetObjects(objects);
            objects = new List<ObjectData>();
        }

        public byte[] Serialization()
        {
            byte[] scenarioIDBytes = UTILS.StringToBytes(scenarioID);
            byte[] scenarioNameBytes = UTILS.StringToBytes(scenarioName);
            byte[] folderNameBytes = UTILS.StringToBytes(folderName);
            byte[] writerBytes = UTILS.StringToBytes(writer);
            byte[] mapTypeBytes = UTILS.StringToBytes(mapType);
            byte[] latBytes = BitConverter.GetBytes(latitude);
            byte[] lonBytes = BitConverter.GetBytes(longitude);
            byte[] scaleBytes = BitConverter.GetBytes(scale);
            byte[] rangeBytes = BitConverter.GetBytes(range);
            byte[] objectCountBytes = BitConverter.GetBytes(objectCount);
            byte[] descriptionBytes = UTILS.StringToBytes(description);
            byte[] rightToControlBytes = BitConverter.GetBytes(rightToControl == string.Empty? false : Convert.ToBoolean(rightToControl));
            byte[] writeDayBytes = UTILS.StringToBytes(writeDay);
            byte[] revisionDayBytes = UTILS.StringToBytes(revisionDay);
            byte[] objectsBytes = new byte[0];

            foreach (var obj in objects)
            {
                objectsBytes = UTILS.AddBytes(objectsBytes, obj.Serialization());
            }

            byte[] result = UTILS.AddBytes(scenarioIDBytes, scenarioNameBytes);
            result = UTILS.AddBytes(result, folderNameBytes);
            result = UTILS.AddBytes(result, writerBytes);
            result = UTILS.AddBytes(result, mapTypeBytes);
            result = UTILS.AddBytes(result, latBytes);
            result = UTILS.AddBytes(result, lonBytes);
            result = UTILS.AddBytes(result, scaleBytes);
            result = UTILS.AddBytes(result, rangeBytes);
            result = UTILS.AddBytes(result, objectCountBytes);
            result = UTILS.AddBytes(result, descriptionBytes);
            result = UTILS.AddBytes(result, rightToControlBytes);
            result = UTILS.AddBytes(result, writeDayBytes);
            result = UTILS.AddBytes(result, revisionDayBytes);
            result = UTILS.AddBytes(result, objectsBytes);

            return result;
        }

        public static ScenarioData DeSerialization(byte[] bytes)
        {
            int parsCnt = 0;
            int stringLen = 0;
            ScenarioData scenarioData = new ScenarioData();

            #region ScenarioID
            stringLen = UTILS.BytesToUShort(bytes, ref parsCnt);
            scenarioData.SetScenarioID(UTILS.BytesToString(bytes, stringLen, ref parsCnt));
            #endregion

            #region ScenarioName
            stringLen = UTILS.BytesToUShort(bytes, ref parsCnt);
            scenarioData.SetScenarioName(UTILS.BytesToString(bytes, stringLen, ref parsCnt));
            #endregion

            #region FolderName
            stringLen = UTILS.BytesToUShort(bytes, ref parsCnt);
            scenarioData.SetFolderName(UTILS.BytesToString(bytes, stringLen, ref parsCnt));
            #endregion

            #region Writer
            stringLen = UTILS.BytesToUShort(bytes, ref parsCnt);
            scenarioData.SetWriter(UTILS.BytesToString(bytes, stringLen, ref parsCnt));
            #endregion

            #region MapType
            stringLen = UTILS.BytesToUShort(bytes, ref parsCnt);
            scenarioData.SetMapType(UTILS.BytesToString(bytes, stringLen, ref parsCnt));
            #endregion

            #region Lat
            scenarioData.SetLatitude(UTILS.BytesToFloat(bytes, ref parsCnt));
            #endregion

            #region Lon
            scenarioData.SetLongitude(UTILS.BytesToFloat(bytes, ref parsCnt));
            #endregion

            #region Scale
            scenarioData.SetScale(UTILS.BytesToInt(bytes, ref parsCnt));
            #endregion

            #region Range
            scenarioData.SetRange(UTILS.BytesToInt(bytes, ref parsCnt));
            #endregion

            #region ObjectCount
            scenarioData.SetObjectCount(UTILS.BytesToInt(bytes, ref parsCnt));
            #endregion

            #region Description
            stringLen = UTILS.BytesToUShort(bytes, ref parsCnt);
            scenarioData.SetDescription(UTILS.BytesToString(bytes, stringLen, ref parsCnt));
            #endregion

            #region RightToControl
            scenarioData.SetRightToControl(UTILS.BytesToBoolean(bytes, ref parsCnt));
            #endregion

            #region WriteDay
            stringLen = UTILS.BytesToUShort(bytes, ref parsCnt);
            scenarioData.SetWriteDay(UTILS.BytesToString(bytes, stringLen, ref parsCnt));
            #endregion

            #region RevisionDay
            stringLen = UTILS.BytesToUShort(bytes, ref parsCnt);
            scenarioData.SetRevisionDay(UTILS.BytesToString(bytes, stringLen, ref parsCnt));
            #endregion

            #region Objects
            List<ObjectData> objects = new List<ObjectData>();

            for (int i = 0; i < scenarioData.objectCount; i++)
            {
                int objectLen = UTILS.BytesToUShort(bytes, ref parsCnt);
                byte[] objectBytes = new byte[objectLen];
                Array.Copy(bytes, parsCnt, objectBytes, 0, objectLen);

                objects.Add(ObjectData.DeSerialization(objectBytes));
                parsCnt += objectLen;
            }

            scenarioData.SetObjects(objects);
            //stringLen = UTILS.BytesToUShort(bytes, ref parsCnt);
            #endregion

            return scenarioData;
        }

        public void Clear()
        {
            scenarioID = string.Empty;
            scenarioName = string.Empty;
            folderName = string.Empty;
            writer = string.Empty;
            mapType = string.Empty;
            latitude = 0;
            longitude = 0;
            range = 0;
            objectCount = 0;
            description = string.Empty;
            rightToControl = "false";
            writeDay = string.Empty;
            revisionDay = string.Empty;

            if(objects == null) objects = new List<ObjectData>();
            objects.Clear();
        }

        /// <summary>
        /// 시나리오 ID 설정
        /// </summary>
        /// <param name="scenarioID"></param>
        public void SetScenarioID(string scenarioID) { this.scenarioID = scenarioID; }

        /// <summary>
        /// 시나리오 명 설정
        /// </summary>
        /// <param name="scenarioName"></param>
        public void SetScenarioName(string scenarioName) { this.scenarioName = scenarioName; }

        /// <summary>
        /// 폴더 명 설정
        /// </summary>
        /// <param name="folderName"></param>
        public void SetFolderName(string folderName) { this.folderName = folderName; }

        /// <summary>
        /// 작성자 설정
        /// </summary>
        /// <param name="writer"></param>
        public void SetWriter(string writer) { this.writer = writer; }

        /// <summary>
        /// 맵 타입 설정
        /// </summary>
        /// <param name="mapType"></param>
        public void SetMapType(string mapType) { this.mapType = mapType; }

        /// <summary>
        /// 위도 설정
        /// </summary>
        /// <param name="latitude"></param>
        public void SetLatitude(float latitude) { this.latitude = latitude; }

        /// <summary>
        /// 경도 설정
        /// </summary>
        /// <param name="longitude"></param>
        public void SetLongitude(float longitude) { this.longitude = longitude; }

        /// <summary>
        /// 스케일 설정
        /// </summary>
        /// <param name="scale"></param>
        public void SetScale(int scale) { this.scale = scale; }

        /// <summary>
        /// 반경 설정
        /// </summary>
        /// <param name="range"></param>
        public void SetRange(int range) { this.range = range; }

        /// <summary>
        /// 오브젝트 수 설정
        /// </summary>
        /// <param name="objectCount"></param>
        public void SetObjectCount(int objectCount) { this.objectCount = objectCount; }

        /// <summary>
        /// 설명 설정
        /// </summary>
        /// <param name="description"></param>
        public void SetDescription(string description) { this.description = description; }

        /// <summary>
        /// 소유권 설정
        /// </summary>
        /// <param name="rightToControl"></param>
        public void SetRightToControl(bool rightToControl) { this.rightToControl = Convert.ToString(rightToControl); }

        /// <summary>
        /// 작성일 설정
        /// </summary>
        /// <param name="writeDay"></param>
        public void SetWriteDay(string writeDay) { this.writeDay = writeDay; }

        /// <summary>
        /// 수정일 설정
        /// </summary>
        /// <param name="revisionDay"></param>
        public void SetRevisionDay(string revisionDay) { this.revisionDay = revisionDay; }

        /// <summary>
        /// 오브젝트 정보 리스트 설정
        /// </summary>
        /// <param name="_objectData"></param>
        public void SetObjects(List<ObjectData> objects)
        {
            this.objects = objects;
        }
    }
}