using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.IO;
using System.Windows.Forms;

namespace Assets.Script
{
    class RWJson
    {
        public class JsonData
        {
            public List<Config> lsConfig;
        }
        JsonData jsonData = new JsonData();

        [Serializable]
        public class Config
        {
            public string pythonScriptPath;
            public List<string> cmdName;
            public List<string> cmdPath;
            public int optionselect;
            public int LpCounter;
            public string StartKey;
            public string StopKey;
        }

        //获取路径
        static string JsonPath()
        {
            return System.Windows.Forms.Application.StartupPath + @"\config\config.json";
        }

        public static void SaveJson(JsonData jsonData)
        {
            //检查存不存在文件
            if (!File.Exists(JsonPath()))
            {
                return;
            }
            string json = JsonUtility.ToJson(jsonData, true);
            File.WriteAllText(JsonPath(), json);
            Debug.Log("保存成功");
        }

        public static void ReadJson(ref JsonData jsonData)
        {
            if(!File.Exists(JsonPath()))
            {
                return;
            }
            string json = File.ReadAllText(JsonPath());
            jsonData = JsonUtility.FromJson<JsonData>(json);
        }
    }
}
