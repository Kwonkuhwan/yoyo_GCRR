using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GCRR.VirtualMap
{
    public static class JsonManager
    {
#if UNITY_EDITOR
        private static string defaultDataListPath = $"{Application.dataPath}\\JsonData";
        private static string scenarioListPath = $"{Application.dataPath}\\JsonData";
        private static string scenarioDataPath = $"{Application.dataPath}\\JsonData\\ScenarioData";
#else
        private static string defaultDataListPath = $"{Application.dataPath}\\..\\JsonData";
        private static string scenarioListPath = $"{Application.dataPath}\\..\\JsonData";
        private static string scenarioDataPath = $"{Application.dataPath}\\..\\JsonData\\ScenarioData";
#endif

        #region Wirte
        private static bool Write(string jsonData, string fullpath)
        {
            try
            {
                //if(!File.Exists(_fullpath))
                //{
                //    File.Create(_fullpath);
                //}

                FileStream fileStream = new FileStream(fullpath, FileMode.Create);
                byte[] data = Encoding.UTF8.GetBytes(jsonData);
                fileStream.Write(data, 0, data.Length);
                fileStream.Close();
                return true;
            }
            catch(Exception e)
            {
                UTILS.LogError(e.ToString());
                return false;
            }
        }

        public static bool ScenarioListJsonWirte(string jsonName)
        {
            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(scenarioListPath);
                if (!directoryInfo.Exists)
                {
                    directoryInfo.Create();
                }

                string fullpath = $"{scenarioListPath}\\{jsonName}.Json";

                string jsonData = JsonConvert.SerializeObject(ScenarioInfoClass.Instance.scenarioLists);

                Write(jsonData, fullpath);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool ScenarioListJsonWirte(string jsonName, List<ScenarioList> scenarioLists)
        {
            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(scenarioListPath);
                if (!directoryInfo.Exists)
                {
                    directoryInfo.Create();
                }

                string fullpath = $"{scenarioListPath}\\{jsonName}.Json";

                string jsonData = JsonConvert.SerializeObject(scenarioLists);

                Write(jsonData, fullpath);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool ScenarioDataJsonWirte(string jsonName)
        {
            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(scenarioDataPath);
                if (!directoryInfo.Exists)
                {
                    directoryInfo.Create();
                }

                string fullpath = $"{scenarioDataPath}\\{jsonName}.Json";

                //string jsonData = JsonUtility.ToJson(ScenarioInfoClass.Instance.scenarioData);
                string jsonData = JsonConvert.SerializeObject(ScenarioInfoClass.Instance.scenarioData);

                Write(jsonData, fullpath);
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region Load
        public static byte[] Load(string dirPath, string jsonName)
        {
            string fullpath = $"{dirPath}\\{jsonName}.json";

            FileStream fileStream = new FileStream(fullpath, FileMode.Open);
            byte[] data = new byte[fileStream.Length];
            fileStream.Read(data, 0, data.Length);
            fileStream.Close();
            return data;
        }

        public static DefaultData DefaultDataJsonLoad(string jsonName)
        {
            return DefaultDataJsonLoad(Load(defaultDataListPath, jsonName));
        }

        public static DefaultData DefaultDataJsonLoad(byte[] bytes)
        {
            try
            {
                string data = UTILS.BytesToString(bytes);
                //return JsonUtility.FromJson<DefaultData>(data);
                return JsonConvert.DeserializeObject<DefaultData>(data);

            }
            catch
            {
                return null;
            }
        }

        public static List<ScenarioList> ScenarioListsJsonLoad(string jsonName)
        {
            return ScenarioListsJsonLoad(Load(scenarioListPath, jsonName));
        }

        public static List<ScenarioList> ScenarioListsJsonLoad(byte[] bytes)
        {
            List<ScenarioList> list = new List<ScenarioList>();

            try
            {
                JArray jArray = JArray.Parse(UTILS.BytesToString(bytes));
                foreach (var item in jArray.Children())
                {
                    ScenarioList listItem = item.ToObject<ScenarioList>();
                    list.Add(listItem);
                }
            }
            catch(Exception e)
            {
                UTILS.LogError($"{e}");
                return null;
            }

            return list;
        }

        public static ScenarioData ScenarioDataJsonLoad(string jsonName)
        {
            return ScenarioDataJsonLoad(Load(scenarioDataPath, jsonName));
        }

        public static ScenarioData ScenarioDataJsonLoad(byte[] bytes)
        {
            try
            {
                //return JsonUtility.FromJson<ScenarioData>(UTILS.BytesToString(bytes));
                return JsonConvert.DeserializeObject<ScenarioData>(UTILS.BytesToString(bytes));
            }
            catch(Exception e)
            {
                UTILS.LogError($"{e}");
                return null;
            }
        }

        public static TargetData TargetDataJsonLoad(string jsonName)
        {
            return TargetDataJsonLoad(Load(defaultDataListPath, jsonName));
        }

        public static TargetData TargetDataJsonLoad(byte[] bytes)
        {
            try
            {
                //TargetData targetData = JsonUtility.FromJson<TargetData>(UTILS.BytesToString(bytes));
                TargetData targetData = JsonConvert.DeserializeObject<TargetData>(UTILS.BytesToString(bytes));

                return targetData;
            }
            catch(Exception e)
            {
                UTILS.LogError($"{e}");
                return null;
            }
        }

        public static List<TargetMetaData> TargetMetaDataJsonLoad(byte[] bytes)
        {
            try
            {
                JArray jArray = JArray.Parse(UTILS.BytesToString(bytes));
                if (jArray == null) return null;

                List<TargetMetaData> targetMetaDatas = new List<TargetMetaData>();
                foreach(var obj in jArray)
                {
                    try
                    {
                        targetMetaDatas.Add(JsonUtility.FromJson<TargetMetaData>(obj.ToString()));
                    }
                    catch
                    {
                        continue;
                    }
                }
                return targetMetaDatas;
            }
            catch
            {
                return null;
            }
        }

        public static TimeSeriesData TimeSeriesDataLoad(byte[] bytes)
        {
            try
            {
                //TimeSeriesData timeSeriesData = JsonUtility.FromJson<TimeSeriesData>(UTILS.BytesToString(bytes));
                TimeSeriesData timeSeriesData = JsonConvert.DeserializeObject<TimeSeriesData>(UTILS.BytesToString(bytes));
                return timeSeriesData;
            }
            catch
            {
                return null;
            }
        }
        #endregion
    }
}