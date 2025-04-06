using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace LanuagePack_tool
{
    internal class dataContor
    {
        /// <summary>
        /// 读取指定path的json文件中的数据,返回json中的dataList
        /// </summary>
        public static List<JObject> read_json(string path)
        {
            try
            {
                StringBuilder builder = new StringBuilder();
                using (StreamReader reader = new StreamReader(path))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        builder.Append(line);
                    }
                }
                string json = builder.ToString();
                JObject jsonObject = JObject.Parse(json);
                List<JObject> dataList = jsonObject["dataList"].ToObject<List<JObject>>();
                return dataList;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return null;
            }
        }
        /// <summary>
        /// 修改指定path的json文件中key_word的数据
        /// </summary>
        public static bool change_value(string path,string key_word,string id,string changed_value)
        {
            try
            {
                List<JObject> dataList = read_json(path);

                var item = dataList.FirstOrDefault(x => x["id"].ToString() == id);
                if(item != null)
                {
                    item[key_word] = changed_value;
                    //保存json
                    JObject jsonObject = new JObject();
                    jsonObject["dataList"] = JArray.FromObject(dataList);
                    File.WriteAllText(path, jsonObject.ToString());

                    return true;
                }
                else
                {
                    Console.WriteLine("Item with id " + id + " not found.");
                    return false;
                }
            }catch(Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return false;
            }
        }
        /// <summary>
        /// 修改技能json文件中key_word的数据
        /// </summary>
        public static bool change_skill_value(string path,string key_word,string id,string changed_value)
        {
            try
            {
                List<JObject> dataList = read_json(path);

                var skills = dataList.Where(u => u["id"].ToString().Equals(id.ToString())).ToList();
                if (skills.Any())
                {
                    //遍历技能的所有阶段并修改名称
                    foreach (JObject item in skills)
                    {
                        JArray levelList = (JArray)item["levelList"];
                        foreach (JObject level in levelList)
                        {
                            level[key_word] = changed_value;
                        }
                    }
                    //保存json
                    JObject jsonObject = new JObject();
                    jsonObject["dataList"] = JArray.FromObject(dataList);
                    File.WriteAllText(path, jsonObject.ToString());

                    return true;
                }
                else
                {
                    Console.WriteLine("Item with id " + id + " not found.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return false;
            }
        }
        /// <summary>
        /// 修改ego技能的数据
        /// </summary>
        public static bool change_ego_skill_value(string path, string id, string name, string abname)
        {
            try
            {
                List<JObject> dataList = read_json(path);
                var skills = dataList.Where(u => u["id"].ToString().StartsWith(id.ToString())).ToList();
                if (skills.Any())
                {
                    //遍历技能的所有阶段并修改名称
                    foreach (JObject item in skills)
                    {
                        JArray levelList = (JArray)item["levelList"];
                        foreach (JObject level in levelList)
                        {
                            level["name"] = name;
                            level["abName"] = abname;
                        }
                    }
                    //保存json
                    JObject jsonObject = new JObject();
                    jsonObject["dataList"] = JArray.FromObject(dataList);
                    File.WriteAllText(path, jsonObject.ToString());
                    return true;
                }
                else
                {
                    Console.WriteLine("Item with id " + id + " not found.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return false;
            }
        }
        /// <summary>
        /// 修改ego语音文字数据
        /// </summary>
        public static bool change_ego_voice_value(string path, string id, string desc, string dlg)
        {
            try
            {
                List<JObject> dataList = read_json(path);

                var item = dataList.FirstOrDefault(x => x["id"].ToString() == id);
                if (item != null)
                {
                    item["desc"] = desc;
                    item["dlg"] = dlg;
                    //保存json
                    JObject jsonObject = new JObject();
                    jsonObject["dataList"] = JArray.FromObject(dataList);
                    File.WriteAllText(path, jsonObject.ToString());

                    return true;
                }
                else
                {
                    Console.WriteLine("Item with id " + id + " not found.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return false;
            }
        }
    }
}
