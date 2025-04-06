using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Ookii.Dialogs.WinForms;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace LanuagePack_tool
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        string lang_dir = null;

        private void Form1_Load(object sender, EventArgs e)
        {
            label2.Visible = false;
            MessageBox.Show("本工具的所有修改都无法撤回，在修改之前请做好备份", "提示");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            comboBox1.DataSource = null;
            MessageBox.Show("请做好语言包备份，以免出现错误后无法恢复", "提示");
            VistaFolderBrowserDialog dialog = new VistaFolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string dir = dialog.SelectedPath;
                if(Directory.Exists(dir+ "/StoryData") && Directory.Exists(dir + "/BattleAnnouncerDlg") && File.Exists(dir + "/Personalities.json"))
                {
                    label2.Visible=true;
                    lang_dir = dir;

                    load_Characters();
                    load_Personalities();
                    load_Egos();

                    //加载罪人列表
                    var list = Characters_list.Select(u => new
                    {
                        u.id,
                        u.name,
                    }).ToList();
                    comboBox1.ValueMember = "id";
                    comboBox1.DisplayMember = "name";
                    comboBox1.DataSource = list;
                }
                else
                {
                    MessageBox.Show("选中的文件夹可能不是语言包");
                }
            }
        }
        public class Characters_info
        {
            public int id { get; set; }
            public string name { get; set; }
            public string en_name { get; set; }
        }
        private static List<Characters_info> Characters_list = new List<Characters_info>();
        List<string> en_name_list = new List<string>
        {
            "YiSang",
            "Faust",
            "DonQuixote",
            "Ryoshu",
            "Meursault",
            "HongLu",
            "Heathcliff",
            "Ishmael",
            "Rodion",
            "Sinclair",
            "Outis",
            "Gregor",
        };

        private void load_Characters()
        {
            string json = File.ReadAllText(lang_dir + "/Characters.json");
            JObject root = JObject.Parse(json);
            List<JObject> dataList = root["dataList"].ToObject<List<JObject>>();

            Characters_list.Clear();
            foreach (var item in dataList)
            {
                Characters_info info = new Characters_info();
                info.id = (int)item["id"];
                info.name = (string)item["name"];
                info.en_name = en_name_list[info.id-1];
                Characters_list.Add(info);
            }
        }

        class Personalities_info
        {
            public int id { get; set; }
            public string title { get; set; }
            public string name { get; set; }
            public string nameWithTitle { get; set; }
            public string desc { get; set; }
        }
        private static List<Personalities_info> Personalities_list = new List<Personalities_info>();

        private void load_Personalities()
        {
            string json = File.ReadAllText(lang_dir + "/Personalities.json");
            JObject root = JObject.Parse(json);
            List<JObject> dataList = root["dataList"].ToObject<List<JObject>>();

            Personalities_list.Clear();
            foreach (var item in dataList)
            {
                Personalities_info info = new Personalities_info();
                info.id = (int)item["id"];
                info.title = (string)item["title"];
                info.name = (string)item["name"];
                info.nameWithTitle = (string)item["nameWithTitle"];
                info.desc = (string)item["desc"];
                Personalities_list.Add(info);
            }
        }
        class Egos_info
        {
            public int id { get; set; }
            public string name { get; set; }
            public string desc { get; set; }
        }
        private static List<Egos_info> Egos_list = new List<Egos_info>();
        private void load_Egos()
        {
            string json = File.ReadAllText(lang_dir + "/Egos.json");
            JObject root = JObject.Parse(json);
            List<JObject> dataList = root["dataList"].ToObject<List<JObject>>();

            Egos_list.Clear();
            foreach (var item in dataList)
            {
                Egos_info info = new Egos_info();
                info.id = (int)item["id"];
                info.name = (string)item["name"];
                info.desc = (string)item["desc"];
                Egos_list.Add(info);
            }
        }
        class Skills_info
        {
            public int id { get; set; }
            public List<Levels_info> levelList { get; set; } = new List<Levels_info>();
        }
        class Levels_info
        {
            public int level { get; set; }
            public string name { get; set; }
            public string desc { get; set; }
            public List<coin_info> coinlist { get; set; } = new List<coin_info>();
        }
        class coin_info
        {
            public List<coindescs> coindescs { get; set; } = new List<coindescs>();
        }
        public class coindescs
        {
            public string desc { get; set; }
        }
        private static List<Skills_info> Skills_list = new List<Skills_info>();
        class Voice_EGO
        {
            public string id { get; set; }
            public string desc { get; set; }
            public string dlg { get; set; }
        }
        private static List<Voice_EGO> Voice_list = new List<Voice_EGO>();
        private void load_Voice()
        {
            Voice_list.Clear();
            var selected_characters = Characters_list.First(u => u.id == (int)comboBox1.SelectedValue);
            string json = File.ReadAllText($"{lang_dir}/EGOVoiceDig/Voice_EGO_{selected_characters.en_name}_{selected_characters.id}.json");
            JObject root = JObject.Parse(json);
            List<JObject> dataList = root["dataList"].ToObject<List<JObject>>();
            foreach (var item in dataList)
            {
                Voice_EGO info = new Voice_EGO();
                info.id = (string)item["id"];
                info.desc = (string)item["desc"];
                info.dlg = (string)item["dlg"];
                Voice_list.Add(info);
            }
            //加载ego相关语音
            var list = Voice_list
                .Where(u=>u.id.Contains(comboBox4.SelectedValue.ToString()))
                .Select(u => new
            {
                u.id,
                u.desc,
                u.dlg,
            }).ToList();
            dataGridView1.DataSource = list;
        }
        class ego_skills_info
        {
            public int id { get; set; }
            public List<ego_levels_info> levelList { get; set; } = new List<ego_levels_info>();
        }
        class ego_levels_info
        {
            public int level { get; set; }
            public string name { get; set; }
            public string abName { get; set; }
            public string desc { get; set; }
            public List<coin_info> coinlist { get; set; } = new List<coin_info>();
        }
        List<ego_skills_info> ego_skills_list = new List<ego_skills_info>();
        bool ego_skills_from_early_version = false;
        private void Load_ego_skillst()
        {
            ego_skills_list.Clear();
            int id = (int)comboBox1.SelectedValue;
            //加载ego技能列表
            StringBuilder json = new StringBuilder();
            string path = $"{lang_dir}/Skills_Ego_personality-{id.ToString("00")}.json";
            if(ego_skills_from_early_version)
            {
                //如果是早期版本，则加载早期版本的json
                path = $"{lang_dir}/Skills_Ego.json";
            }
            using (var reader = new StreamReader(path))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    // 逐行处理
                    json.AppendLine(line);
                }
            }
            JObject root = JObject.Parse(json.ToString());
            List<JObject> dataList = root["dataList"].ToObject<List<JObject>>();
            foreach (var item in dataList)
            {
                ego_skills_info info = new ego_skills_info();
                info.id = (int)item["id"];
                info.levelList = new List<ego_levels_info>();
                foreach (var levelToken in item["levelList"])
                {
                    ego_levels_info levelInfo = new ego_levels_info();
                    levelInfo.level = (int)levelToken["level"];
                    levelInfo.name = (string)levelToken["name"];
                    levelInfo.abName = (string)levelToken["abName"];
                    levelInfo.desc = (string)levelToken["desc"];
                    levelInfo.coinlist = new List<coin_info>();
                    foreach (var coinToken in levelToken["coinlist"])
                    {
                        coin_info coinItem = new coin_info();
                        if (coinToken["coindescs"] != null)
                        {
                            foreach (var descToken in coinToken["coindescs"])
                            {
                                coinItem.coindescs.Add(new coindescs
                                {
                                    desc = (string)descToken["desc"]
                                });
                            }
                        }
                        levelInfo.coinlist.Add(coinItem);
                    }
                    info.levelList.Add(levelInfo);
                }
                ego_skills_list.Add(info);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(!(comboBox1.SelectedValue is int))
            {
                MessageBox.Show("请先选择一个罪人");
                return;
            }
            //修改角色名称
            if (Characters_list.First(u => u.id == (int)comboBox1.SelectedValue).name == textBox4.Text)
            {
                return;
            }
            Characters_list.First(u=>u.id == (int)comboBox1.SelectedValue).name = textBox4.Text;
            //保存修改至json中
            JObject root = new JObject();
            JArray temp_arry = new JArray();
            foreach (var item in Characters_list)
            {
                JObject jobj = new JObject();
                jobj["id"] = item.id;
                jobj["name"] = item.name;
                temp_arry.Add(jobj);
            }
            root["dataList"] = temp_arry;
            File.WriteAllText(lang_dir + "/Characters.json", root.ToString());
            //刷新列表中的信息
            int index = comboBox1.SelectedIndex;
            comboBox1.DataSource = null;
            comboBox1.ValueMember = "id";
            comboBox1.DisplayMember = "name";
            comboBox1.DataSource = Characters_list;
            comboBox1.SelectedIndex = index;

            MessageBox.Show("修改成功");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (!(comboBox2.SelectedValue is int))
            {
                MessageBox.Show("请先选择一个人格");
                return;
            }
            //修改人格名称
            int id = (int)comboBox2.SelectedValue;
            if(Personalities_list.First(u => u.id == id).title == richTextBox1.Text)
            {
                return;
            }
            Personalities_list.First(u=>u.id == id).title = richTextBox1.Text;
            //保存修改至json中
            JObject root = new JObject();
            JArray temp_arry = new JArray();
            foreach (var item in Personalities_list)
            {
                JObject jobj = new JObject();
                jobj["id"] = item.id;
                jobj["title"] = item.title;
                jobj["name"] = item.name;
                jobj["nameWithTitle"] = item.nameWithTitle;
                jobj["desc"] = item.desc;
                temp_arry.Add(jobj);
            }
            root["dataList"] = temp_arry;
            File.WriteAllText(lang_dir + "/Personalities.json", root.ToString());
            //刷新列表中的信息
            int index = comboBox2.SelectedIndex;
            var list = Personalities_list.Where(x => x.id.ToString().StartsWith("1" + ((int)comboBox1.SelectedValue).ToString("00"))).ToList();
            comboBox2.ValueMember = "id";
            comboBox2.DisplayMember = "title";
            comboBox2.DataSource = list;
            comboBox2.SelectedIndex = index;
            MessageBox.Show("修改成功");
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!(comboBox1.SelectedValue is int))
            {
                return;
            }
            //加载人格列表
            var list = Personalities_list.Where(x => x.id.ToString().StartsWith("1" + ((int)comboBox1.SelectedValue).ToString("00"))).ToList();
            comboBox2.ValueMember = "id";
            comboBox2.DisplayMember = "title";
            comboBox2.DataSource = list;
            //加载ego列表
            var ego_list = Egos_list.Where(x => x.id.ToString().StartsWith("2" + ((int)comboBox1.SelectedValue).ToString("00"))).ToList();
            comboBox4.ValueMember = "id";
            comboBox4.DisplayMember = "name";
            comboBox4.DataSource = ego_list;
            //向新的罪人名称中添加默认选项
            textBox4.Text = comboBox1.Text;
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            //根据人格id加载对应的技能


            //向新的人格名称中添加默认选项
            richTextBox1.Text = comboBox2.Text;
        }
        string ego_abName;
        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            //向新的ego名称中添加默认选项
            textBox1.Text = comboBox4.Text;
            //加载ego技能列表
            ego_skills_from_early_version = false;
            Load_ego_skillst();
            if (!ego_skills_list.Any(u=>(u.id / 100) == (int)comboBox4.SelectedValue))
            {
                //未找到ego技能信息，加载旧版
                ego_skills_from_early_version = true;
                Load_ego_skillst();
            }
            var item = ego_skills_list.FirstOrDefault(u => (u.id / 100) == (int)comboBox4.SelectedValue);
            if (item != null)
            {
                textBox3.Text = item.levelList.FirstOrDefault().abName;
                ego_abName = item.levelList.FirstOrDefault().abName;
            }
            else
            {
                //异常的ego信息
                return;
            }
            //加载ego相关语言
            load_Voice();
        }
        private List<Skills_info> load_skills(string json)
        {
            List<Skills_info> skills = new List<Skills_info>();
            JObject root = JObject.Parse(json);
            List<JObject> dataList = root["dataList"].ToObject<List<JObject>>();
            foreach (JObject data in dataList)
            {
                Skills_info skillData = new Skills_info
                {
                    id = (int)data["id"],
                    levelList = new List<Levels_info>()
                };
                foreach (var levelToken in data["levelList"])
                {
                    Levels_info levelInfo = new Levels_info
                    {
                        level = (int)levelToken["level"],
                        name = (string)levelToken["name"],
                        desc = (string)levelToken["desc"],
                        coinlist = new List<coin_info>()
                    };
                    foreach (var coinToken in levelToken["coinlist"])
                    {
                        coin_info coinItem = new coin_info();

                        if (coinToken["coindescs"] != null)
                        {
                            foreach (var descToken in coinToken["coindescs"])
                            {
                                coinItem.coindescs.Add(new coindescs
                                {
                                    desc = (string)descToken["desc"]
                                });
                            }
                        }
                        levelInfo.coinlist.Add(coinItem);
                    }
                    skillData.levelList.Add(levelInfo);
                }
                skills.Add(skillData);
            }

            return skills;
        }
        private void button7_Click(object sender, EventArgs e)
        {
            if (!(comboBox4.SelectedValue is int))
            {
                MessageBox.Show("请先选择一个ego");
                return;
            }
            //修改ego名称
            int id = (int)comboBox4.SelectedValue;
            if (Egos_list.First(u => u.id == id).name == textBox1.Text && ego_abName == textBox3.Text)
            {
                return;
            }
            Egos_list.First(u => u.id == id).name = textBox1.Text;
            //保存至ego列表json中
            JObject root = new JObject();
            JArray temp_arry = new JArray();
            foreach (var item in Egos_list)
            {
                JObject jobj = new JObject();
                jobj["id"] = item.id;
                jobj["name"] = item.name;
                jobj["desc"] = item.desc;
                temp_arry.Add(jobj);
            }
            root["dataList"] = temp_arry;
            File.WriteAllText(lang_dir + "/Egos.json", root.ToString());

            
            //保存至ego技能json中
            string path = $"{lang_dir}/Skills_Ego_personality-{((int)comboBox1.SelectedValue).ToString("00")}.json";
            if (ego_skills_from_early_version)
            {
                //如果是早期版本的json，则需要保存至早期版本的json
                path = $"{lang_dir}/Skills_Ego.json";
            }
            //修改所有名称
            var ego_list = ego_skills_list.Where(u => (u.id / 100) == (int)comboBox4.SelectedValue);
            foreach (var item in ego_list)
            {
                foreach(var level in item.levelList)
                {
                    level.name = textBox1.Text;
                    level.abName = textBox3.Text;
                }
            }
            //保存ego技能信息
            temp_arry = JArray.Parse(JsonConvert.SerializeObject(ego_skills_list));
            root["dataList"] = temp_arry;
            File.WriteAllText(path, root.ToString());


            //刷新列表中的信息
            int index = comboBox4.SelectedIndex;
            var list = Egos_list.Where(x => x.id.ToString().StartsWith("2" + ((int)comboBox1.SelectedValue).ToString("00"))).ToList();
            comboBox4.ValueMember = "id";
            comboBox4.DisplayMember = "name";
            comboBox4.DataSource = list;
            comboBox4.SelectedIndex = index;
            MessageBox.Show("修改成功");
        }
        bool skills_from_early_version = false;
        private void button3_Click(object sender, EventArgs e)
        {
            richTextBox2.Text = "";
            if (!(comboBox2.SelectedValue is int))
            {
                MessageBox.Show("请先选择一个人格");
                return;
            }
            load_skills_data();

        }
        private void load_skills_data()
        {
            //加载技能信息
            StringBuilder json = new StringBuilder();
            string path = $"{lang_dir}/Skills_personality-{((int)comboBox1.SelectedValue).ToString("00")}.json";
            using (var reader = new StreamReader(path))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    // 逐行处理
                    json.AppendLine(line);
                }
            }
            skills_from_early_version = false;
            Skills_list = load_skills(json.ToString());
            int id = (int)comboBox2.SelectedValue;
            //如果技能列表中没有该id，则说明是早期版本的json
            if (!Skills_list.Any(u => (u.id / 100) == id))
            {
                //从早期版本的json中加载技能
                //早期版本的json中技能过多，不使用list存储
                skills_from_early_version = true;
                StringBuilder sb = new StringBuilder();
                using (var reader = new StreamReader($"{lang_dir}/Skills.json"))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        sb.AppendLine(line);
                    }
                }
                JObject root = JObject.Parse(sb.ToString());
                List<JObject> dataList = root["dataList"].ToObject<List<JObject>>();
                var skills = dataList
                    .Where(u => u["id"].ToString().StartsWith(id.ToString()))
                    .Select(u => new
                    {
                        id = (int)u["id"],
                        name = u["levelList"][0]["name"].ToString(),
                    }).ToList();

                comboBox3.ValueMember = "id";
                comboBox3.DisplayMember = "name";
                comboBox3.DataSource = skills;

            }
            else
            {
                var skills = Skills_list.Where(u => (u.id / 100) == id).Select(u => new
                {
                    u.id,
                    name = u.levelList.FirstOrDefault().name,
                }).ToList();

                comboBox3.ValueMember = "id";
                comboBox3.DisplayMember = "name";
                comboBox3.DataSource = skills;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (!(comboBox3.SelectedValue is int))
            {
                MessageBox.Show("请先选择一个技能");
                return;
            }
            if (comboBox3.Text == richTextBox2.Text)
            {
                return;
            }
            int id = (int)comboBox3.SelectedValue;
            //从旧版本json中加载技能,需要保存至旧版本json
            if (skills_from_early_version)
            {
                StringBuilder sb = new StringBuilder();
                using (var reader = new StreamReader($"{lang_dir}/Skills.json"))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        sb.AppendLine(line);
                    }
                }
                JObject skills_root = JObject.Parse(sb.ToString());
                List<JObject> dataList = skills_root["dataList"].ToObject<List<JObject>>();
                var skills = dataList.Where(u => u["id"].ToString().StartsWith(id.ToString())).ToList();
                foreach (JObject item in skills)
                {
                    JArray levelList = (JArray)item["levelList"];
                    foreach (JObject level in levelList)
                    {
                        level["name"] = richTextBox2.Text;
                    }
                }
                skills_root["dataList"] = JArray.FromObject(dataList);
                File.WriteAllText($"{lang_dir}/Skills.json", skills_root.ToString());

                //刷新列表中的信息
                var skills_list = dataList
                    .Where(u => u["id"].ToString().StartsWith(comboBox2.SelectedValue.ToString()))
                    .Select(u => new
                    {
                        id = (int)u["id"],
                        name = u["levelList"][0]["name"].ToString(),
                    }).ToList();
                comboBox3.ValueMember = "id";
                comboBox3.DisplayMember = "name";
                comboBox3.DataSource = skills_list;
                comboBox3.SelectedValue = id;
                MessageBox.Show("修改成功");
                return;
            }

            //修改技能名称
            Skills_info skill = Skills_list.First(u => u.id == id);
            foreach (var level in skill.levelList)
            {
                level.name = richTextBox2.Text;
            }
            //保存修改至新版json中
            string jsonString = JsonConvert.SerializeObject(Skills_list);
            JObject root = new JObject();
            JArray temp_arry = JArray.Parse(jsonString);
            root["dataList"] = temp_arry;
            File.WriteAllText($"{lang_dir}/Skills_personality-{((int)comboBox1.SelectedValue).ToString("00")}.json", root.ToString());
            //刷新列表中的信息
            load_skills_data();
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            richTextBox2.Text = comboBox3.Text;
        }
        string selected_voice_ego_id = null;
        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if(dataGridView1.SelectedRows.Count == 0)
            {
                selected_voice_ego_id = null;
                return;
            }
            var selected_row = dataGridView1.SelectedRows[0];
            textBox5.Text = selected_row.Cells["desc"].Value.ToString();
            richTextBox3.Text = selected_row.Cells["dlg"].Value.ToString();
            selected_voice_ego_id = selected_row.Cells["id"].Value.ToString();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            //修改ego相关语音
            if (selected_voice_ego_id == null)
            {
                MessageBox.Show("请先选择一条语音");
                return;
            }
            var voice = Voice_list.First(u => u.id == selected_voice_ego_id);
            if (voice.desc == textBox5.Text && voice.dlg == richTextBox3.Text)
            {
                return;
            }
            voice.desc = textBox5.Text;
            voice.dlg = richTextBox3.Text;
            //保存修改至json中
            string jsonString = JsonConvert.SerializeObject(Voice_list);
            JObject root = new JObject();
            JArray temp_arry = JArray.Parse(jsonString);
            root["dataList"] = temp_arry;
            var selected_characters = Characters_list.First(u => u.id == (int)comboBox1.SelectedValue);
            File.WriteAllText($"{lang_dir}/EGOVoiceDig/Voice_EGO_{selected_characters.en_name}_{selected_characters.id}.json", root.ToString());
            //刷新列表中的信息
            var list = Voice_list
                .Where(u => u.id.Contains(comboBox4.SelectedValue.ToString()))
                .Select(u => new
                {
                    u.id,
                    u.desc,
                    u.dlg,
                }).ToList();
            dataGridView1.DataSource = list;
            MessageBox.Show("修改成功");
        }
    }
}
