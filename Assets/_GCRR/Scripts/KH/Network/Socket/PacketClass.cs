using System;

namespace GCRR.VirtualMap
{
    public class PacketData
    {
        public ushort packetSize { get; set; }
        public PacketID packetID { get; set; }
        public byte[] dataBytes { get; set; }

        public static PacketData DeSerialization(byte[] bytes)
        {
            int parsCnt = 0;
            PacketData packetData = new PacketData();

            packetData.packetSize = UTILS.BytesToUShort(bytes, ref parsCnt);

            packetData.packetID = (PacketID)UTILS.BytesToUShort(bytes, ref parsCnt);

            byte[] result = new byte[packetData.packetSize];
            Array.Copy(bytes, parsCnt, result, 0, packetData.packetSize);
            packetData.dataBytes = result;

            return packetData;
        }
    }
    public class S_MyPlayerID
    {
        private PacketID packetID = PacketID.S_MyPlayerID;
        public int playerID { get; set; }

        public S_MyPlayerID() { }

        public S_MyPlayerID(int playerID_)
        {
            Init(playerID_);
        }

        public void Init(int playerID_)
        {
            this.playerID = playerID_;
        }

        public byte[] Serialize()
        {
            byte[] packetIDBytes = BitConverter.GetBytes((ushort)packetID);
            byte[] playerIDBytes = BitConverter.GetBytes(playerID);

            ushort len = (ushort)(playerIDBytes.Length);
            byte[] lengthBytes = BitConverter.GetBytes(len);

            byte[] result = UTILS.AddBytes(lengthBytes, packetIDBytes);
            result = UTILS.AddBytes(result, playerIDBytes);

            return result;
        }

        public static S_MyPlayerID DeSerialization(PacketData packetData)
        {
            int parsCnt = 0;
            S_MyPlayerID myPlayerID = new S_MyPlayerID();
            if (packetData.packetID != PacketID.S_MyPlayerID) return null;

            #region playerID
            myPlayerID.playerID = UTILS.BytesToInt(packetData.dataBytes, ref parsCnt);
            #endregion

            return myPlayerID;
        }
    }

    public class C_ChangeObjInfo
    {
        private PacketID packetID = PacketID.C_ChangeObjInfo;
        public int playerID { get; set; }
        public ScenarioObjectInteractionType interactionType { get; set; }     // 상호작용 유형
        public int objSequence { get; set; }      // 오브젝트 순서
        public ObjectData objectData { get; set; }

        public C_ChangeObjInfo() { }

        public C_ChangeObjInfo(int playerID_, ScenarioObjectInteractionType interactionType_, int objSequence_, ObjectData objectData_)
        {
            Init(playerID_, interactionType_, objSequence_, objectData_);
        }

        public void Init(int playerID_, ScenarioObjectInteractionType interactionType_, int objSequence_, ObjectData objectData_)
        {
            this.playerID = playerID_;
            this.interactionType = interactionType_;
            this.objSequence = objSequence_;
            this.objectData = objectData_;
        }

        public byte[] Serialize()
        {
            byte[] packetIDBytes = BitConverter.GetBytes((ushort)packetID);
            byte[] playerIDBytes = BitConverter.GetBytes(playerID);
            byte[] interactionBytes = BitConverter.GetBytes((int)interactionType);
            byte[] objSequenceBytes = BitConverter.GetBytes(objSequence);

            byte[] objectIDBytes = BitConverter.GetBytes(objectData.objectID);
            byte[] forceSizeBytes = BitConverter.GetBytes(objectData.forceSize);
            byte[] forceCodeBytes = UTILS.StringToBytes(objectData.forceCode);
            byte[] unitNameBytes = UTILS.StringToBytes(objectData.unitName);
            byte[] unitCodeBytes = UTILS.StringToBytes(objectData.unitCode);
            byte[] allyBytes = BitConverter.GetBytes(objectData.isAlly);
            byte[] latBytes = BitConverter.GetBytes((float)objectData.unitLat);
            byte[] lonBytes = BitConverter.GetBytes((float)objectData.unitLon);
            byte[] heightBytes = BitConverter.GetBytes(objectData.height);
            byte[] showForceMarkBytes = BitConverter.GetBytes(objectData.showForceMark);
            byte[] missionBytes = UTILS.StringToBytes(objectData.mission);
            byte[] movingDirectionbBytes = BitConverter.GetBytes(objectData.movingDirection);

            ushort len = (ushort)(playerIDBytes.Length + interactionBytes.Length + objSequenceBytes.Length
                                    + objectIDBytes.Length + forceSizeBytes.Length + forceCodeBytes.Length
                                    + unitNameBytes.Length + unitCodeBytes.Length + allyBytes.Length
                                    + latBytes.Length + lonBytes.Length + heightBytes.Length
                                    + showForceMarkBytes.Length + missionBytes.Length + movingDirectionbBytes.Length);
            byte[] lengthBytes = BitConverter.GetBytes(len);

            byte[] result = UTILS.AddBytes(lengthBytes, packetIDBytes);
            result = UTILS.AddBytes(result, playerIDBytes);
            result = UTILS.AddBytes(result, interactionBytes);
            result = UTILS.AddBytes(result, objSequenceBytes);
            result = UTILS.AddBytes(result, objectIDBytes);
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

        public static C_ChangeObjInfo DeSerialization(PacketData packetData)
        {
            int parsCnt = 0;
            int stringLen = 0;

            byte[] bytes = packetData.dataBytes;

            C_ChangeObjInfo c_ChangeObjInfo = new C_ChangeObjInfo();
            if (packetData.packetID != PacketID.C_ChangeObjInfo) return null;

            #region playerID
            c_ChangeObjInfo.playerID = UTILS.BytesToInt(bytes, ref parsCnt);
            #endregion

            #region Interaction
            c_ChangeObjInfo.interactionType = (ScenarioObjectInteractionType)UTILS.BytesToInt(bytes, ref parsCnt);
            #endregion

            #region ObjSequence
            c_ChangeObjInfo.objSequence = UTILS.BytesToInt(bytes, ref parsCnt);
            #endregion

            #region objectID
            c_ChangeObjInfo.objectData.objectID = UTILS.BytesToInt(bytes, ref parsCnt);
            #endregion

            #region forceSize
            c_ChangeObjInfo.objectData.forceSize = UTILS.BytesToInt(bytes, ref parsCnt);
            #endregion

            #region forceCode
            stringLen = UTILS.BytesToUShort(bytes, ref parsCnt);

            c_ChangeObjInfo.objectData.forceCode = UTILS.BytesToString(bytes, stringLen, ref parsCnt);
            #endregion

            #region unitName
            stringLen = UTILS.BytesToUShort(bytes, ref parsCnt);

            c_ChangeObjInfo.objectData.unitName = UTILS.BytesToString(bytes, stringLen, ref parsCnt);
            #endregion

            #region unitCode
            stringLen = UTILS.BytesToUShort(bytes, ref parsCnt);

            c_ChangeObjInfo.objectData.unitCode = UTILS.BytesToString(bytes, stringLen, ref parsCnt);
            #endregion

            #region isAlly
            c_ChangeObjInfo.objectData.isAlly = UTILS.BytesToBoolean(bytes, ref parsCnt);
            #endregion

            #region lat    
            c_ChangeObjInfo.objectData.unitLat = UTILS.BytesToFloat(bytes, ref parsCnt);
            #endregion

            #region lon
            c_ChangeObjInfo.objectData.unitLon = UTILS.BytesToFloat(bytes, ref parsCnt);
            #endregion

            #region height
            c_ChangeObjInfo.objectData.height = (UTILS.BytesToFloat(bytes, ref parsCnt));
            #endregion

            #region showForceMark
            c_ChangeObjInfo.objectData.showForceMark = (UTILS.BytesToBoolean(bytes, ref parsCnt));
            #endregion

            #region mission
            stringLen = UTILS.BytesToUShort(bytes, ref parsCnt);

            c_ChangeObjInfo.objectData.mission = (UTILS.BytesToString(bytes, stringLen, ref parsCnt));
            #endregion

            #region movingdirection
            c_ChangeObjInfo.objectData.movingDirection = (UTILS.BytesToInt(bytes, ref parsCnt));
            #endregion

            return c_ChangeObjInfo;
        }
    }

    public class S_ChangeObjInfo
    {
        private PacketID packetID = PacketID.S_ChangeObjInfo;
        public int playerID { get; set; }
        public ScenarioObjectInteractionType interactionType { get; set; }     // 상호작용 유형
        public int objSequence { get; set; }      // 오브젝트 순서
        public ObjectData objectData { get; set; }

        public S_ChangeObjInfo() { objectData = new ObjectData(); }

        public S_ChangeObjInfo(int playerID_, ScenarioObjectInteractionType interactionType_, int objSequence_, ObjectData objectData_)
        {
            Init(playerID_, interactionType_, objSequence_, objectData_);
        }

        public void Init(int playerID_, ScenarioObjectInteractionType interactionType_, int objSequence_, ObjectData objectData_)
        {
            this.playerID = playerID_;
            this.interactionType = interactionType_;
            this.objSequence = objSequence_;
            this.objectData = objectData_;
        }

        public byte[] Serialize()
        {
            byte[] packetIDBytes = BitConverter.GetBytes((ushort)packetID);
            byte[] playerIDBytes = BitConverter.GetBytes(playerID);
            byte[] interactionBytes = BitConverter.GetBytes((int)interactionType);
            byte[] objSequenceBytes = BitConverter.GetBytes(objSequence);

            byte[] objectIDBytes = BitConverter.GetBytes(objectData.objectID);
            byte[] forceSizeBytes = BitConverter.GetBytes(objectData.forceSize);
            byte[] forceCodeBytes = UTILS.StringToBytes(objectData.forceCode);
            byte[] unitNameBytes = UTILS.StringToBytes(objectData.unitName);
            byte[] unitCodeBytes = UTILS.StringToBytes(objectData.unitCode);
            byte[] allyBytes = BitConverter.GetBytes(objectData.isAlly);
            byte[] latBytes = BitConverter.GetBytes((float)objectData.unitLat);
            byte[] lonBytes = BitConverter.GetBytes((float)objectData.unitLon);
            byte[] heightBytes = BitConverter.GetBytes(objectData.height);
            byte[] showForceMarkBytes = BitConverter.GetBytes(objectData.showForceMark);
            byte[] missionBytes = UTILS.StringToBytes(objectData.mission);
            byte[] movingDirectionbBytes = BitConverter.GetBytes(objectData.movingDirection);

            ushort len = (ushort)(playerIDBytes.Length + interactionBytes.Length + objSequenceBytes.Length
                                    + objectIDBytes.Length + forceSizeBytes.Length + forceCodeBytes.Length
                                    + unitNameBytes.Length + unitCodeBytes.Length + allyBytes.Length
                                    + latBytes.Length + lonBytes.Length + heightBytes.Length
                                    + showForceMarkBytes.Length + missionBytes.Length + movingDirectionbBytes.Length);

            byte[] lengthBytes = BitConverter.GetBytes(len);

            byte[] result = UTILS.AddBytes(lengthBytes, packetIDBytes);
            result = UTILS.AddBytes(result, playerIDBytes);
            result = UTILS.AddBytes(result, interactionBytes);
            result = UTILS.AddBytes(result, objSequenceBytes);
            result = UTILS.AddBytes(result, objectIDBytes);
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

        public static S_ChangeObjInfo DeSerialization(PacketData packetData)
        {
            int parsCnt = 0;
            int stringLen = 0;

            byte[] bytes = packetData.dataBytes;

            S_ChangeObjInfo s_ChangeObjInfo = new S_ChangeObjInfo();
            if (packetData.packetID != PacketID.S_ChangeObjInfo) return null;

            #region playerID
            s_ChangeObjInfo.playerID = UTILS.BytesToInt(bytes, ref parsCnt);
            #endregion

            #region Interaction
            s_ChangeObjInfo.interactionType = (ScenarioObjectInteractionType)UTILS.BytesToInt(bytes, ref parsCnt);
            #endregion

            #region ObjSequence
            s_ChangeObjInfo.objSequence = UTILS.BytesToInt(bytes, ref parsCnt);
            #endregion

            #region objectID
            s_ChangeObjInfo.objectData.objectID = (UTILS.BytesToInt(bytes, ref parsCnt));
            #endregion

            #region forceSize
            s_ChangeObjInfo.objectData.forceSize = (UTILS.BytesToInt(bytes, ref parsCnt));
            #endregion

            #region forceCode
            stringLen = UTILS.BytesToUShort(bytes, ref parsCnt);

            s_ChangeObjInfo.objectData.forceCode = (UTILS.BytesToString(bytes, stringLen, ref parsCnt));
            #endregion

            #region unitName
            stringLen = UTILS.BytesToUShort(bytes, ref parsCnt);

            s_ChangeObjInfo.objectData.unitName = (UTILS.BytesToString(bytes, stringLen, ref parsCnt));
            #endregion

            #region unitCode
            stringLen = UTILS.BytesToUShort(bytes, ref parsCnt);

            s_ChangeObjInfo.objectData.unitCode = (UTILS.BytesToString(bytes, stringLen, ref parsCnt));
            #endregion

            #region isAlly
            s_ChangeObjInfo.objectData.isAlly = (UTILS.BytesToBoolean(bytes, ref parsCnt));
            #endregion

            #region lat    
            s_ChangeObjInfo.objectData.unitLat = (UTILS.BytesToFloat(bytes, ref parsCnt));
            #endregion

            #region lon
            s_ChangeObjInfo.objectData.unitLon = (UTILS.BytesToFloat(bytes, ref parsCnt));
            #endregion

            #region height
            s_ChangeObjInfo.objectData.height = (UTILS.BytesToFloat(bytes, ref parsCnt));
            #endregion

            #region showForceMark
            s_ChangeObjInfo.objectData.showForceMark = (UTILS.BytesToBoolean(bytes, ref parsCnt));
            #endregion

            #region mission
            stringLen = UTILS.BytesToUShort(bytes, ref parsCnt);

            s_ChangeObjInfo.objectData.mission = (UTILS.BytesToString(bytes, stringLen, ref parsCnt));
            #endregion

            #region movingdirection
            s_ChangeObjInfo.objectData.movingDirection = (UTILS.BytesToInt(bytes, ref parsCnt));
            #endregion

            return s_ChangeObjInfo;
        }
    }

    public class C_SaveScenario
    {
        private PacketID packetID = PacketID.C_SaveScenario;
        public int playerID { get; set; }
        public string scenarioID { get; set; }
        public string scenarioName { get; set; }
        public string folderName { get; set; }
        public float lat { get; set; }
        public float lon { get; set; }
        public string writer { get; set; }
        public string mapType { get; set; }
        public int scale { get; set; }
        public int range { get; set; }
        public string description { get; set; }
        public string writeDay { get; set; }

        public C_SaveScenario() { }

        public C_SaveScenario(int playerID_, string scenarioID_, string scenarioName_, string folderName_,
            float lat_, float lon_, string writer_, string mapType_, int scale_, int range_, string description_, string writeDay_)
        {
            Init(playerID_, scenarioID_, scenarioName_, folderName_, lat_, lon_, writer_, mapType_, scale_, range_, description_, writeDay_);
        }

        public void Init(int playerID_, string scenarioID_, string scenarioName_, string folderName_,
            float lat_, float lon_, string writer_, string mapType_, int scale_, int range_, string description_, string writeDay_)
        {
            this.playerID = playerID_;
            this.scenarioID = scenarioID_;
            this.scenarioName = scenarioName_;
            this.folderName = folderName_;
            this.lat = lat_;
            this.lon = lon_;
            this.writer = writer_;
            this.mapType = mapType_;
            this.scale = scale_;
            this.range = range_;
            this.description = description_;
            this.writeDay = writeDay_;
        }

        public byte[] Serialize()
        {
            byte[] packetIDBytes = BitConverter.GetBytes((ushort)packetID);
            byte[] playerIDBytes = BitConverter.GetBytes(playerID);
            byte[] scenarioIDBytes = UTILS.StringToBytes(scenarioID);
            byte[] scenarioNameBytes = UTILS.StringToBytes(scenarioName);
            byte[] folderNameBytes = UTILS.StringToBytes(folderName);
            byte[] latBytes = BitConverter.GetBytes(lat);
            byte[] lonBytes = BitConverter.GetBytes(lon);
            byte[] writerBytes = UTILS.StringToBytes(writer);
            byte[] mapTypeBytes = UTILS.StringToBytes(mapType);
            byte[] scaleBytes = BitConverter.GetBytes(scale);
            byte[] rangeBytes = BitConverter.GetBytes(range);
            byte[] descriptionBytes = UTILS.StringToBytes(description);
            byte[] writeDayBytes = UTILS.StringToBytes(writeDay);


            ushort len = (ushort)(playerIDBytes.Length + scenarioIDBytes.Length + scenarioNameBytes.Length
                                    + folderNameBytes.Length + latBytes.Length + lonBytes.Length
                                    + writerBytes.Length + mapTypeBytes.Length + scaleBytes.Length
                                    + rangeBytes.Length + descriptionBytes.Length
                                    + writeDayBytes.Length);
            byte[] lengthBytes = BitConverter.GetBytes(len);

            byte[] result = UTILS.AddBytes(lengthBytes, packetIDBytes);
            result = UTILS.AddBytes(result, playerIDBytes);
            result = UTILS.AddBytes(result, scenarioIDBytes);
            result = UTILS.AddBytes(result, scenarioNameBytes);
            result = UTILS.AddBytes(result, folderNameBytes);
            result = UTILS.AddBytes(result, latBytes);
            result = UTILS.AddBytes(result, lonBytes);
            result = UTILS.AddBytes(result, writerBytes);
            result = UTILS.AddBytes(result, mapTypeBytes);
            result = UTILS.AddBytes(result, scaleBytes);
            result = UTILS.AddBytes(result, rangeBytes);
            result = UTILS.AddBytes(result, descriptionBytes);
            result = UTILS.AddBytes(result, writeDayBytes);

            return result;
        }

        public static C_SaveScenario DeSerialization(PacketData packetData)
        {
            int parsCnt = 0;
            int stringLen = 0;

            C_SaveScenario c_SaveScenario = new C_SaveScenario();
            if (packetData.packetID != PacketID.C_SaveScenario) return null;

            #region playerID
            c_SaveScenario.playerID = UTILS.BytesToInt(packetData.dataBytes, ref parsCnt);
            #endregion

            #region ScenarioID
            stringLen = UTILS.BytesToUShort(packetData.dataBytes, ref parsCnt);

            c_SaveScenario.scenarioID = UTILS.BytesToString(packetData.dataBytes, stringLen, ref parsCnt);
            #endregion

            #region ScenarioName
            stringLen = UTILS.BytesToUShort(packetData.dataBytes, ref parsCnt);

            c_SaveScenario.scenarioName = UTILS.BytesToString(packetData.dataBytes, stringLen, ref parsCnt);
            #endregion

            #region FolderName
            stringLen = UTILS.BytesToUShort(packetData.dataBytes, ref parsCnt);

            c_SaveScenario.folderName = UTILS.BytesToString(packetData.dataBytes, stringLen, ref parsCnt);
            #endregion

            #region Lat
            c_SaveScenario.lat = UTILS.BytesToFloat(packetData.dataBytes, ref parsCnt);
            #endregion

            #region Lon
            c_SaveScenario.lat = UTILS.BytesToFloat(packetData.dataBytes, ref parsCnt);
            #endregion

            #region Writer
            stringLen = UTILS.BytesToUShort(packetData.dataBytes, ref parsCnt);

            c_SaveScenario.writer = UTILS.BytesToString(packetData.dataBytes, stringLen, ref parsCnt);
            #endregion

            #region MapType
            stringLen = UTILS.BytesToUShort(packetData.dataBytes, ref parsCnt);

            c_SaveScenario.mapType = UTILS.BytesToString(packetData.dataBytes, stringLen, ref parsCnt);
            #endregion

            #region Scale
            c_SaveScenario.scale = UTILS.BytesToInt(packetData.dataBytes, ref parsCnt);
            #endregion

            #region Range
            c_SaveScenario.range = UTILS.BytesToInt(packetData.dataBytes, ref parsCnt);
            #endregion

            #region Description
            stringLen = UTILS.BytesToUShort(packetData.dataBytes, ref parsCnt);

            c_SaveScenario.description = UTILS.BytesToString(packetData.dataBytes, stringLen, ref parsCnt);
            #endregion

            #region WriteDay
            stringLen = UTILS.BytesToUShort(packetData.dataBytes, ref parsCnt);

            c_SaveScenario.description = UTILS.BytesToString(packetData.dataBytes, stringLen, ref parsCnt);
            #endregion

            return c_SaveScenario;
        }
    }

    public class S_SaveScenarioResult
    {
        private PacketID packetID = PacketID.S_SaveScenarioResult;
        public int playerID { get; set; }
        public bool saveResult { get; set; }

        public S_SaveScenarioResult() { }

        public S_SaveScenarioResult(int playerID, bool saveResult)
        {
            Init(playerID, saveResult);
        }

        public void Init(int playerID, bool saveResult)
        {
            this.playerID = playerID;
            this.saveResult = saveResult;
        }

        public byte[] Serialize()
        {
            byte[] packetIDBytes = BitConverter.GetBytes((ushort)packetID);
            byte[] playerIDBytes = BitConverter.GetBytes(playerID);
            byte[] saveResultBytes = BitConverter.GetBytes(saveResult);

            ushort len = (ushort)(playerIDBytes.Length + saveResultBytes.Length);
            byte[] lengthBytes = BitConverter.GetBytes(len);

            byte[] result = UTILS.AddBytes(lengthBytes, packetIDBytes);
            result = UTILS.AddBytes(result, playerIDBytes);
            result = UTILS.AddBytes(result, saveResultBytes);

            return result;
        }


        public static S_SaveScenarioResult DeSerialization(PacketData packetData)
        {
            int parsCnt = 0;

            S_SaveScenarioResult s_SaveScenarioResult = new S_SaveScenarioResult();
            if (packetData.packetID != PacketID.S_SaveScenarioResult) return null;

            #region playerID
            s_SaveScenarioResult.playerID = UTILS.BytesToInt(packetData.dataBytes, ref parsCnt);
            #endregion

            #region SaveResult
            s_SaveScenarioResult.saveResult = UTILS.BytesToBoolean(packetData.dataBytes, ref parsCnt);
            #endregion

            return s_SaveScenarioResult;
        }
    }

    public class C_LeaveGame
    {
        private PacketID packetID = PacketID.C_LeaveGame;
        public int playerID { get; set; }

        public C_LeaveGame() { }

        public C_LeaveGame(int playerID_)
        {
            Init(playerID_);
        }

        public void Init(int playerID_)
        {
            this.playerID = playerID_;
        }

        public byte[] Serialize()
        {
            byte[] packetIDBytes = BitConverter.GetBytes((ushort)packetID);
            byte[] playerIDBytes = BitConverter.GetBytes(playerID);

            ushort len = (ushort)(playerIDBytes.Length);
            byte[] lengthBytes = BitConverter.GetBytes(len);

            byte[] result = UTILS.AddBytes(lengthBytes, packetIDBytes);
            result = UTILS.AddBytes(result, playerIDBytes);

            return result;
        }

        public static C_LeaveGame DeSerialization(PacketData packetData)
        {
            int parsCnt = 0;

            C_LeaveGame c_LeaveGame = new C_LeaveGame();
            if (packetData.packetID != PacketID.C_LeaveGame) return null;

            #region playerID
            c_LeaveGame.playerID = UTILS.BytesToInt(packetData.dataBytes, ref parsCnt);
            #endregion

            return c_LeaveGame;
        }
    }

    public class S_BroadcastNewOwner
    {
        private PacketID packetID = PacketID.S_BroadcastNewOwner;
        public int playerID { get; set; }

        public S_BroadcastNewOwner() { }

        public S_BroadcastNewOwner(int playerID_)
        {
            Init(playerID_);
        }

        public void Init(int playerID_)
        {
            this.playerID = playerID_;
        }

        public byte[] Serialize()
        {
            byte[] packetIDBytes = BitConverter.GetBytes((ushort)packetID);
            byte[] playerIDBytes = BitConverter.GetBytes(playerID);

            ushort len = (ushort)(playerIDBytes.Length);
            byte[] lengthBytes = BitConverter.GetBytes(len);

            byte[] result = UTILS.AddBytes(lengthBytes, packetIDBytes);
            result = UTILS.AddBytes(result, playerIDBytes);

            return result;
        }

        public static S_BroadcastNewOwner DeSerialization(PacketData packetData)
        {
            int parsCnt = 0;

            S_BroadcastNewOwner s_BroadcastNewOwner = new S_BroadcastNewOwner();
            if (packetData.packetID != PacketID.S_BroadcastNewOwner) return null;

            #region playerID
            s_BroadcastNewOwner.playerID = UTILS.BytesToInt(packetData.dataBytes, ref parsCnt);
            #endregion

            return s_BroadcastNewOwner;
        }
    }

    public class Response_ScenarioList
    {
        private PacketID packetID = PacketID.Response_ScenrioList;
        public ScenarioList scenarioList { get; set; }

        public Response_ScenarioList() { }

        public Response_ScenarioList(ScenarioList scenarioList_)
        {
            Init(scenarioList = scenarioList_);
        }

        public void Init(ScenarioList scenarioList_)
        {
            scenarioList = scenarioList_;
        }

        public byte[] Serialize()
        {
            byte[] packetIDBytes = BitConverter.GetBytes((ushort)packetID);

            byte[] listBytes = scenarioList.Serialize();

            ushort len = (ushort)listBytes.Length;
            byte[] lengthBytes = BitConverter.GetBytes(len);
            byte[] result = UTILS.AddBytes(lengthBytes, packetIDBytes);
            result = UTILS.AddBytes(result, listBytes);

            return result;
        }

        public static Response_ScenarioList DeSerialization(PacketData packetData)
        {
            Response_ScenarioList response_ScenarioLists = new Response_ScenarioList();
            if (packetData.packetID != PacketID.Response_ScenrioList) return null;

            response_ScenarioLists.scenarioList = ScenarioList.DeSerialization(packetData.dataBytes);

            return response_ScenarioLists;
        }
    }

    public class Request_ScenarioList
    {
        private PacketID packetID = PacketID.Request_ScenarioList;

        public Request_ScenarioList() { }

        public byte[] Serialization()
        {
            byte[] packetIDBytes = BitConverter.GetBytes((ushort)packetID);

            ushort len = 0;
            byte[] lengthBytes = BitConverter.GetBytes(len);
            byte[] result = UTILS.AddBytes(lengthBytes, packetIDBytes);

            return result;
        }

        public static Request_ScenarioList DeSerialization(PacketData packetData)
        {
            Request_ScenarioList request_ScenarioLists = new Request_ScenarioList();
            if (packetData.packetID != PacketID.Request_ScenarioList) return null;

            return request_ScenarioLists;
        }
    }

    public class Response_ScenarioData
    {
        private PacketID packetID = PacketID.Response_ScenrioData;
        public ScenarioData scenarioData { get; set; }

        public Response_ScenarioData() { }

        public Response_ScenarioData(ScenarioData scenarioData_)
        {
            Init(scenarioData_);
        }

        public void Init(ScenarioData scenarioData_)
        {
            this.scenarioData = scenarioData_;
        }

        public byte[] Serialize()
        {
            byte[] packetIDBytes = BitConverter.GetBytes((ushort)packetID);
            byte[] scenarioDataBytes = scenarioData.Serialization();

            ushort len = (ushort)(scenarioDataBytes.Length);
            byte[] lengthBytes = BitConverter.GetBytes(len);

            byte[] result = UTILS.AddBytes(lengthBytes, packetIDBytes);
            result = UTILS.AddBytes(result, scenarioDataBytes);

            return result;
        }

        public static Response_ScenarioData DeSerialization(PacketData packetData)
        {
            Response_ScenarioData response_ScenarioData = new Response_ScenarioData();
            if (packetData.packetID != PacketID.Response_ScenrioData) return null;

            #region ScenarioData
            response_ScenarioData.scenarioData = ScenarioData.DeSerialization(packetData.dataBytes);
            #endregion

            return response_ScenarioData;
        }
    }

    public class Request_ScenarioData
    {
        private PacketID packetID = PacketID.Request_ScenarioData;
        public string scenarioName { get; set; }

        public Request_ScenarioData() { }

        public Request_ScenarioData(string scenarioName_)
        {
            Init(scenarioName_);
        }

        public void Init(string scenarioName_)
        {
            this.scenarioName = scenarioName_;
        }

        public byte[] Serialize()
        {
            byte[] packetIDBytes = BitConverter.GetBytes((ushort)packetID);
            byte[] scenarioNameBytes = UTILS.StringToBytes(scenarioName);

            ushort len = (ushort)(scenarioNameBytes.Length);
            byte[] lengthBytes = BitConverter.GetBytes(len);

            byte[] result = UTILS.AddBytes(lengthBytes, packetIDBytes);
            result = UTILS.AddBytes(result, scenarioNameBytes);

            return result;
        }

        public static Request_ScenarioData DeSerialization(PacketData packetData)
        {
            int parsCnt = 0;
            Request_ScenarioData request_ScenarioData = new Request_ScenarioData();
            if (packetData.packetID != PacketID.Request_ScenarioData) return null;

            #region ScenarioName
            int stringLen = UTILS.BytesToUShort(packetData.dataBytes, ref parsCnt);

            request_ScenarioData.scenarioName = UTILS.BytesToString(packetData.dataBytes, stringLen, ref parsCnt);
            #endregion

            return request_ScenarioData;
        }
    }

    public class ChangeMapMove
    {
        private PacketID packetID = PacketID.ChangeMapMove;
        public int direction { get; set; }

        public ChangeMapMove() { }

        public ChangeMapMove(int direction)
        {
            Init(direction);
        }

        public void Init(int direction)
        {
            this.direction = direction;
        }

        public byte[] Serialize()
        {
            byte[] packetIDBytes = BitConverter.GetBytes((ushort)packetID);
            byte[] directionBytes = BitConverter.GetBytes(direction);

            ushort len = (ushort)(directionBytes.Length);
            byte[] lengthBytes = BitConverter.GetBytes(len);

            byte[] result = UTILS.AddBytes(lengthBytes, packetIDBytes);
            result = UTILS.AddBytes(result, directionBytes);

            return result;
        }

        public static ChangeMapMove DeSerialization(PacketData packetData)
        {
            int parsCnt = 0;
            ChangeMapMove changeMapMove = new ChangeMapMove();
            if (packetData.packetID != PacketID.ChangeMapMove) return null;

            #region direction
            changeMapMove.direction = UTILS.BytesToInt(packetData.dataBytes, ref parsCnt);
            #endregion

            return changeMapMove;
        }
    }
}