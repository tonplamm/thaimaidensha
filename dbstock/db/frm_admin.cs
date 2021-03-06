﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;

namespace db
{
    public partial class frm_admin : Form
    {
        OleDbDataAdapter oda;
        DataTable dt;
        public OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\\thaimaidensha\\dbstock\\database.accdb");

        public frm_admin()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
        }

        private void frm_admin_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'databaseDataSet.tb_login' table. You can move, or remove it, as needed.
            this.tb_loginTableAdapter.Fill(this.databaseDataSet.tb_login);
            oda = new OleDbDataAdapter("SELECT * FROM tb_login", conn);
            dt = new DataTable();
            oda.Fill(dt);
            dataGridView1.DataSource = dt;

            checkProject(sender, e);
            
        }

        private void checkProject(object sender, EventArgs e)
        {
            if (textBox9.Text == "" && textBox10.Text == "" && textBox11.Text == "")
            {
                conn.Open();
                DataSet ds = new DataSet();
                OleDbDataAdapter da = new OleDbDataAdapter("SELECT ProjName, ProjWBS, ProjAdmin FROM tb_Project", conn);
                da.Fill(ds, "data_Proj");
                string text_name = "", text_wbs = "", text_owner = "";
                foreach (DataRow dr in ds.Tables["data_Proj"].Rows)
                {
                    text_name = dr.ItemArray[0] + "";
                    text_wbs = dr.ItemArray[1] + "";
                    text_owner = dr.ItemArray[2] + "";
                    break;
                }
                conn.Close();
                if (text_name != "" && text_wbs != "" && text_owner != "")
                {
                    textBox10.Text = text_name;
                    textBox9.Text = text_wbs;
                    textBox11.Text = text_owner;
                    button2.Hide();
                    textBox9.ReadOnly = true;
                    textBox10.ReadOnly = true;
                    textBox11.ReadOnly = true;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "" || textBox2.Text == "" || textBox3.Text == "" || textBox4.Text == "" || textBox5.Text == "" || textBox6.Text == "")
            {
                MessageBox.Show("ข้อมูลไม่ครบถ้วน");
            }
            else
            {
                conn.Open();
                OleDbCommand command = new OleDbCommand();
                command.Connection = conn;
                command.CommandText = "SELECT * FROM tb_login WHERE LoginName='" + textBox1.Text + "'";
                OleDbDataReader reader = command.ExecuteReader();
                int count = 0;
                while (reader.Read())
                {
                    count = count + 1;
                    if (count == 1)
                    {
                        break;
                    }
                }
                if (count == 1)
                {
                    MessageBox.Show("มี Username นี้ภายในระบบแล้ว", "WARNING");
                }
                else
                {
                    OleDbCommand command2 = new OleDbCommand();
                    command2.Connection = conn;
                    command2.CommandText = "INSERT into tb_login (LoginName, LoginPass, LoginFirstName, LoginLastName, LoginPosition, LoginPhone) VALUES ('"+textBox1.Text+"', '"+textBox2.Text+"', '"+textBox3.Text+"', '"+textBox4.Text+"', '"+textBox5.Text+"', '"+textBox6.Text+"')";
                    command2.ExecuteNonQuery();
                    textBox1.Clear();
                    textBox2.Clear();
                    textBox3.Clear();
                    textBox4.Clear();
                    textBox5.Clear();
                    textBox6.Clear();
                    MessageBox.Show("เพิ่มผู้ดูแลระบบเรียบร้อย");
                }
                conn.Close();
                frm_admin_Load(sender, e);
            }
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            if (textBox7.Text != "")
            {
                oda = new OleDbDataAdapter("SELECT * FROM tb_login WHERE LoginName like '" + textBox7.Text + "%'", conn);
                dt = new DataTable();
                oda.Fill(dt);
                dataGridView1.DataSource = dt;
            }
            else
            {
                frm_admin_Load(sender, e);
            }
        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            if (textBox8.Text != "")
            {
                oda = new OleDbDataAdapter("SELECT * FROM tb_login WHERE (LoginFirstName like '" + textBox8.Text + "%') OR (LoginLastName like '" + textBox8.Text + "%') ", conn);
                dt = new DataTable();
                oda.Fill(dt);
                dataGridView1.DataSource = dt;
            }
            else
            {
                frm_admin_Load(sender, e);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            conn.Open();
            OleDbCommand command2 = new OleDbCommand();
            command2.Connection = conn;
            command2.CommandText = "INSERT into tb_Project (ProjName, ProjWBS, ProjAdmin) VALUES ('" + textBox10.Text + "', '" + textBox9.Text + "', '"+textBox11.Text+"')";
            command2.ExecuteNonQuery();
            conn.Close();
            button2.Hide();
            textBox9.ReadOnly = true;
            textBox10.ReadOnly = true;
            textBox11.ReadOnly = true;
            MessageBox.Show("ลงทะเบียนโครงการเรียบร้อย");
            frm_admin_Load(sender, e);
        }
    }
}
