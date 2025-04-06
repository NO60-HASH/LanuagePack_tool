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
                if (Directory.Exists(dir + "/StoryData") && Directory.Exists(dir + "/BattleAnnouncerDlg") && File.Exists(dir + "/Personalities.json"))
                {
                    //选择了正确的语言包
                    label2.Visible = true;
                    lang_dir = dir;
                    //加载罪人列表
                    var dataList = dataContor.read_json(lang_dir + "/Characters.json").Select(u => new
                    {
                        id = u["id"],
                        name = u["name"],
                    }).ToList();
                    //绑定数据源
                    comboBox1.ValueMember = "id";
                    comboBox1.DisplayMember = "name";
                    comboBox1.DataSource = dataList;
                }
                else
                {
                    MessageBox.Show("选中的文件夹可能不是语言包");
                }
            }
        }
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
        bool skills_from_early_version = false;
        bool ego_skills_from_early_version = false;
        string ego_abName;

        private void button2_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedValue == null)
            {
                MessageBox.Show("请先选择一个罪人");
                return;
            }
            if (comboBox1.Text == textBox4.Text)
            {
                return;
            }
            //修改角色名称
            if (dataContor.change_value(lang_dir + "/Characters.json", "name", comboBox1.SelectedValue.ToString(), textBox4.Text))
            {
                //修改成功
                MessageBox.Show("修改成功");
            }
            else
            {
                MessageBox.Show("修改失败");
            }
            //刷新列表中的信息
            int index = comboBox1.SelectedIndex;
            var dataList = dataContor.read_json(lang_dir + "/Characters.json").Select(u => new
            {
                id = u["id"],
                name = u["name"],
            }).ToList();
            comboBox1.ValueMember = "id";
            comboBox1.DisplayMember = "name";
            comboBox1.DataSource = dataList;
            comboBox1.SelectedIndex = index;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (comboBox2.SelectedValue is not int)
            {
                MessageBox.Show("请先选择一个人格");
                return;
            }
            if (richTextBox1.Text == comboBox2.Text)
            {
                return;
            }
            //修改人格名称
            string Personalities_path = $"{lang_dir}/Personalities.json";
            if (dataContor.change_value(Personalities_path, "title", comboBox2.SelectedValue.ToString(), richTextBox1.Text))
            {
                //修改成功
                MessageBox.Show("修改成功");
            }
            else
            {
                MessageBox.Show("修改失败");
            }
            //刷新列表中的信息
            int index = comboBox2.SelectedIndex;
            var Personalities_dataList = dataContor.read_json(Personalities_path)
                .Where(u => u["id"].ToString().StartsWith("1" + comboBox1.SelectedValue.ToString().PadLeft(2, '0')))
                .Select(u => new
                {
                    id = u["id"],
                    title = u["title"],
                }).ToList();
            comboBox2.ValueMember = "id";
            comboBox2.DisplayMember = "title";
            comboBox2.DataSource = Personalities_dataList;
            comboBox2.SelectedIndex = index;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedValue == null)
            {
                return;
            }
            //加载人格列表
            string Personalities_path = $"{lang_dir}/Personalities.json";
            var Personalities_dataList = dataContor.read_json(Personalities_path)
                .Where(u => u["id"].ToString().StartsWith("1" + comboBox1.SelectedValue.ToString().PadLeft(2, '0')))
                .Select(u => new
                {
                    id = u["id"],
                    title = u["title"],
                }).ToList();
            comboBox2.ValueMember = "id";
            comboBox2.DisplayMember = "title";
            comboBox2.DataSource = Personalities_dataList;
            //加载ego列表
            string Egos_path = $"{lang_dir}/Egos.json";
            var Egos_dataList = dataContor.read_json(Egos_path)
                .Where(u => u["id"].ToString().StartsWith("2" + comboBox1.SelectedValue.ToString().PadLeft(2, '0')))
                .Select(u => new
                {
                    id = u["id"],
                    name = u["name"],
                }).ToList();
            comboBox4.ValueMember = "id";
            comboBox4.DisplayMember = "name";
            comboBox4.DataSource = Egos_dataList;
            //向新的罪人名称中添加默认选项
            textBox4.Text = comboBox1.Text;
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            string c_id = comboBox1.SelectedValue.ToString();
            string p_id = comboBox2.SelectedValue.ToString();
            //根据人格id加载对应的技能
            skills_from_early_version = false;
            string per_skill_path = $"{lang_dir}/Skills_personality-{c_id.PadLeft(2, '0')}.json";
            List<JObject> per_skill_list = dataContor.read_json(per_skill_path).Where(u => u["id"].ToString().StartsWith(p_id)).ToList();
            if (!per_skill_list.Any())
            {
                //如果没有找到技能列表，则加载早期版本的json
                skills_from_early_version = true;
                per_skill_path = $"{lang_dir}/Skills.json";
                per_skill_list = dataContor.read_json(per_skill_path).Where(u => u["id"].ToString().StartsWith(p_id)).ToList();
            }
            var skills_list = per_skill_list.Select(u => new
            {
                id = u["id"],
                skill_name = u["levelList"][0]["name"].ToString(),
            }).ToList();
            comboBox3.ValueMember = "id";
            comboBox3.DisplayMember = "skill_name";
            comboBox3.DataSource = skills_list;
            //向新的人格名称中添加默认选项
            richTextBox1.Text = comboBox2.Text;
        }
        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            //向新的ego名称中添加默认选项
            textBox1.Text = comboBox4.Text;
            //加载ego技能列表
            string p_id = comboBox1.SelectedValue.ToString();
            string ego_id = comboBox4.SelectedValue.ToString();
            ego_skills_from_early_version = false;
            string ego_skill_path = $"{lang_dir}/Skills_Ego_personality-{p_id.PadLeft(2, '0')}.json";
            List<JObject> ego_skill_list = dataContor.read_json(ego_skill_path).Where(u => u["id"].ToString().StartsWith(ego_id)).ToList();
            if (!ego_skill_list.Any())
            {
                //如果没有找到ego技能列表，则加载早期版本的json
                ego_skills_from_early_version = true;
                ego_skill_path = $"{lang_dir}/Skills_Ego.json";
                ego_skill_list = dataContor.read_json(ego_skill_path).Where(u => u["id"].ToString().StartsWith(ego_id)).ToList();
            }
            var abName = ego_skill_list[0]["levelList"][0]["abName"].ToString();
            //向异想体名称中添加默认选项
            textBox3.Text = abName;
            ego_abName = abName;
            //加载ego相关语言
            string ego_voice_path = $"{lang_dir}/EGOVoiceDig/Voice_EGO_{en_name_list[int.Parse(p_id) - 1]}_{p_id}.json";
            var ego_voice_list = dataContor.read_json(ego_voice_path)
                .Where(u => u["id"].ToString().Contains(ego_id))
                .Select(u => new
                {
                    id = u["id"],
                    desc = u["desc"],
                    dlg = u["dlg"],
                }).ToList();
            //绑定数据源
            dataGridView1.DataSource = ego_voice_list;
        }
        private void button7_Click(object sender, EventArgs e)
        {
            if (comboBox4.SelectedValue == null)
            {
                MessageBox.Show("请先选择一个ego");
                return;
            }
            if (comboBox4.Text == textBox1.Text && ego_abName == textBox3.Text)
            {
                return;
            }
            //修改ego名称
            string Egos_path = $"{lang_dir}/Egos.json";
            string path = $"{lang_dir}/Skills_Ego_personality-{comboBox1.SelectedValue.ToString()}.json";
            if (ego_skills_from_early_version)
            {
                path = $"{lang_dir}/Skills_Ego.json";
            }
            bool change_ego_name = dataContor.change_value(Egos_path, "name", comboBox4.SelectedValue.ToString(), textBox1.Text);
            bool change_ego_skill_name = dataContor.change_ego_skill_value(path, comboBox4.SelectedValue.ToString(), textBox1.Text, textBox3.Text);
            if (change_ego_name && change_ego_skill_name)
            {
                //修改成功
                MessageBox.Show("修改成功");
            }
            else
            {
                MessageBox.Show("修改失败");
            }
            //刷新列表中的信息
            int index = comboBox4.SelectedIndex;
            var Egos_dataList = dataContor.read_json(Egos_path)
                .Where(u => u["id"].ToString().StartsWith("2" + comboBox1.SelectedValue.ToString().PadLeft(2, '0')))
                .Select(u => new
                {
                    id = u["id"],
                    name = u["name"],
                }).ToList();
            comboBox4.ValueMember = "id";
            comboBox4.DisplayMember = "name";
            comboBox4.DataSource = Egos_dataList;
            comboBox4.SelectedIndex = index;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (comboBox3.SelectedValue == null)
            {
                MessageBox.Show("请先选择一个技能");
                return;
            }
            if (comboBox3.Text == richTextBox2.Text)
            {
                return;
            }
            //修改技能名称
            string path = $"{lang_dir}/Skills_personality-{comboBox1.SelectedValue.ToString().PadLeft(2,'0')}.json";
            if (skills_from_early_version)
            {
                path = $"{lang_dir}/Skills.json";
            }
            if (dataContor.change_skill_value(path, "name", comboBox3.SelectedValue.ToString(), richTextBox2.Text))
            {
                //修改成功
                MessageBox.Show("修改成功");
            }
            else
            {
                MessageBox.Show("修改失败");
            }
            //刷新列表中的信息
            string c_id = comboBox1.SelectedValue.ToString();
            string p_id = comboBox2.SelectedValue.ToString();
            string per_skill_path = $"{lang_dir}/Skills_personality-{c_id.PadLeft(2, '0')}.json";
            if (ego_skills_from_early_version)
            {
                per_skill_path = $"{lang_dir}/Skills.json";
            }
            List<JObject> per_skill_list = dataContor.read_json(per_skill_path).Where(u => u["id"].ToString().StartsWith(p_id)).ToList();
            var skills_list = per_skill_list.Select(u => new
            {
                id = u["id"],
                skill_name = u["levelList"][0]["name"].ToString(),
            }).ToList();
            comboBox3.ValueMember = "id";
            comboBox3.DisplayMember = "skill_name";
            comboBox3.DataSource = skills_list;

        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            //向新的技能名称中添加默认选项
            richTextBox2.Text = comboBox3.Text;
        }
        string selected_voice_ego_id = null;
        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
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
            if (selected_voice_ego_id == null)
            {
                MessageBox.Show("请先选择一条语音");
                return;
            }
            var selected_row = dataGridView1.SelectedRows[0];
            if (textBox5.Text == selected_row.Cells["desc"].Value.ToString() && richTextBox3.Text == selected_row.Cells["dlg"].Value.ToString())
            {
                return;
            }
            //修改ego相关语音
            string ego_voice_path = $"{lang_dir}/EGOVoiceDig/Voice_EGO_{en_name_list[int.Parse(comboBox1.SelectedValue.ToString()) - 1]}_{comboBox1.SelectedValue}.json";
            if(dataContor.change_ego_voice_value(ego_voice_path, selected_voice_ego_id, textBox5.Text,richTextBox3.Text))
            {
                //修改成功
                MessageBox.Show("修改成功");
            }
            else
            {
                MessageBox.Show("修改失败");
            }
            //刷新列表中的信息
            string ego_id = comboBox4.SelectedValue.ToString();
            var ego_voice_list = dataContor.read_json(ego_voice_path)
                .Where(u => u["id"].ToString().Contains(ego_id))
                .Select(u => new
                {
                    id = u["id"],
                    desc = u["desc"],
                    dlg = u["dlg"],
                }).ToList();
            //绑定数据源
            dataGridView1.DataSource = ego_voice_list;
        }
    }
}
