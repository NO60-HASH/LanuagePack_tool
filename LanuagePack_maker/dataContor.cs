using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
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
        /// <summary>
        /// 比较两个json文件中的dataList是否相同
        /// </summary>
        /// <param name="list1"></param>
        /// <param name="list2"></param>
        /// <returns></returns>
        public static bool AreListsEqualUnordered(List<JObject> list1, List<JObject> list2)
        {
            if (list1.Count != list2.Count) return false;

            for (int i = 0; i < list1.Count; i++)
            {
                if (list1[i].ToString() != list2[i].ToString())
                    return false;
            }
            return true;
        }
        ///<summary>
        ///将旧json文件中经过修改的内容写入新json文件
        ///</summary>
        public static int update_pack(string old_path,string new_path)
        {
            if(!File.Exists(new_path))
            {
                //新json文件不存在
                return -1;
            }
            //读取旧json文件
            List<JObject> oldDataList = read_json(old_path);
            //读取新json文件
            List<JObject> newDataList = read_json(new_path);
            if(AreListsEqualUnordered(newDataList, oldDataList))
            {
                //两个json文件相同
                return 0;
            }
            //对比新旧json文件，将旧json文件中经过修改的内容写入新json文件
            for (int i = 0; i < oldDataList.Count; i++)
            {
                var oldItem = oldDataList[i];
                var newItem = newDataList.FirstOrDefault(x => x["id"]?.ToString() == oldItem["id"]?.ToString());
                if(newItem is null)
                {
                    //找不到对应id
                    return -1;
                }
                if (oldItem != null && !JToken.DeepEquals(oldItem, newItem))
                {
                    // 直接替换列表中的对象
                    newDataList[i] = oldItem;
                }
            }
            //保存新json文件
            JObject jsonObject = new JObject();
            jsonObject["dataList"] = JArray.FromObject(newDataList);
            File.WriteAllText(new_path, jsonObject.ToString());

            return 1;
        }
    }
}
