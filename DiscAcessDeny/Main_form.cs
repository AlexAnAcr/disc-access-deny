using System;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Security.AccessControl;

namespace DiscAcessDeny
{
    public partial class Main_form : Form
    {
        string name = Environment.UserDomainName + "\\" + Environment.UserName;
        public Main_form()
        {
            InitializeComponent();
        }
        bool rule_exist = false, rule_executing = false, no_close = false; string dir_way;
        private void Form1_Load(object sender, EventArgs e)
        {
            if (System.IO.File.Exists(Application.StartupPath + "\\config.ini"))
            {
                dir_way = System.IO.File.ReadAllText(Application.StartupPath + "\\config.ini",Encoding.UTF8);
                if (System.IO.Directory.Exists(dir_way))
                {
                    FileSystemAccessRule[] rules = Get_rules(System.IO.Directory.GetAccessControl(dir_way).GetAccessRules(true, false, Type.GetType("System.Security.Principal.SecurityIdentifier")), name);
                    if (rules.Length != 0)
                        if (rules[0].FileSystemRights.HasFlag(FileSystemRights.FullControl))
                        {
                            rule_exist = true;
                            if (rules[0].AccessControlType == AccessControlType.Deny)
                                rule_executing = true;
                            else
                                rule_executing = false;
                        }
                        else
                            rule_exist = false;
                    else
                        rule_exist = false;
                    if (rule_exist)
                    {
                        if (rule_executing)
                        {
                            button1.BackColor = Color.DarkRed;
                            button1.Text = "Отключить защиту";
                            label1.Text = "Сосотояние защиты: включена";
                        }
                        else
                        {
                            button1.BackColor = Color.Green;
                            button1.Text = "Включить защиту";
                            label1.Text = "Сосотояние защиты: выключена";
                        }
                    }
                    else
                    {
                        button1.BackColor = Color.Green;
                        button1.Text = "Включить защиту";
                        label1.Text = "Сосотояние защиты: выключена";
                    }
                }
                else Close();
            }
            else Close();
        }
        DirectorySecurity dsec;
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                dsec = System.IO.Directory.GetAccessControl(dir_way);
                System.IO.Directory.SetAccessControl(dir_way, dsec);
                progressBar1.Value = 0; progress = 0;
                if (rule_exist)
                {
                    if (rule_executing)
                    {
                        if (MessageBox.Show("Вы уверены, что хотите отключить защиту?","Сообщение",MessageBoxButtons.YesNo,MessageBoxIcon.None,MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                        {
                            Maze form = new Maze();
                            if (form.ShowDialog() == DialogResult.OK)
                            {
                                if (MessageBox.Show("Вы точно уверены, что хотите отключить защиту? Это на ваш страх и риск!", "Сообщение", MessageBoxButtons.YesNo, MessageBoxIcon.None, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                                {
                                    progressBar1.Maximum = 100;
                                    no_close = true;
                                    button1.Enabled = false;
                                    timer1.Start();
                                    button2.Enabled = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        progressBar1.Maximum = 15;
                        no_close = true;
                        button1.Enabled = false;
                        timer1.Start();
                    }
                }
                else
                {
                    progressBar1.Maximum = 15;
                    no_close = true;
                    button1.Enabled = false;
                    timer1.Start();
                }
            } catch
            {
                MessageBox.Show("Запустите программу от имени администратора!","Ошибка");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            no_close = false;
            progress = 0;
            progressBar1.Value = 0;
            button2.Enabled = false;
            button1.Enabled = true;
            MessageBox.Show("Выполнение отменено!", "Информация");
        }
        byte progress = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (progress == progressBar1.Maximum)
            {
                timer1.Stop();
                progressBar1.Value = progress;
                if (rule_exist)
                {
                    if (rule_executing)
                    {
                        FileSystemAccessRule rule = new FileSystemAccessRule(
                        Environment.UserDomainName + "\\" + Environment.UserName,
                        FileSystemRights.FullControl,
                        InheritanceFlags.ContainerInherit,
                        PropagationFlags.None,
                        AccessControlType.Deny);
                        dsec.RemoveAccessRule(rule);
                        System.IO.Directory.SetAccessControl(dir_way, dsec);
                        button1.BackColor = Color.Green;
                        button1.Text = "Включить защиту";
                        label1.Text = "Сосотояние защиты: выключена";
                        MessageBox.Show("Защита выключена!", "Информация");
                        rule_executing = false;
                        button2.Enabled = false;
                        no_close = false;
                        button1.Enabled = true;
                    }
                    else
                    {
                        dsec.RemoveAccessRule(new FileSystemAccessRule(
                        Environment.UserDomainName + "\\" + Environment.UserName,
                        FileSystemRights.FullControl,
                        InheritanceFlags.ContainerInherit,
                        PropagationFlags.None,
                        AccessControlType.Allow));
                        dsec.AddAccessRule(new FileSystemAccessRule(
                        Environment.UserDomainName + "\\" + Environment.UserName,
                        FileSystemRights.FullControl,
                        InheritanceFlags.ContainerInherit,
                        PropagationFlags.None,
                        AccessControlType.Deny));
                        System.IO.Directory.SetAccessControl(dir_way, dsec);
                        button1.BackColor = Color.DarkRed;
                        button1.Text = "Отключить защиту";
                        label1.Text = "Сосотояние защиты: включена";
                        MessageBox.Show("Защита включена!", "Информация");
                        rule_executing = true;
                        no_close = false;
                        button1.Enabled = true;
                    }
                }
                else
                {
                    FileSystemAccessRule rule = new FileSystemAccessRule(
                    Environment.UserDomainName + "\\" + Environment.UserName,
                    FileSystemRights.FullControl,
                    InheritanceFlags.ContainerInherit,
                    PropagationFlags.None,
                    AccessControlType.Deny);
                    dsec.AddAccessRule(rule);
                    System.IO.Directory.SetAccessControl(dir_way, dsec);
                    button1.BackColor = Color.DarkRed;
                    button1.Text = "Отключить защиту";
                    label1.Text = "Сосотояние защиты: включена";
                    MessageBox.Show("Защита включена!", "Информация");
                    rule_exist = true;
                    rule_executing = true;
                    no_close = false;
                    button1.Enabled = true;
                }
            }
            else
            {
                progress++;
                progressBar1.Value = progress;
            }
               
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (no_close)
            {
                MessageBox.Show("Вы не можете закрыть это окно!", "Ошибка");
                e.Cancel = true;
            }
        }

        FileSystemAccessRule[] Get_rules(AuthorizationRuleCollection rc, string users_rule)
        {
            FileSystemAccessRule[] rules = new FileSystemAccessRule[0];
            foreach (AccessRule r in rc)
            {
                if (r.IdentityReference.Translate(typeof(System.Security.Principal.NTAccount)).Value == users_rule)
                {
                    Array.Resize(ref rules, 1);
                    rules[0] = (FileSystemAccessRule)r;
                    break;
                }
            }
            return rules;
        }
    }
}
