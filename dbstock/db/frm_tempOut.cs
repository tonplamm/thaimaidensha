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
    public partial class frm_tempOut : Form
    {
        OleDbDataAdapter oda;
        DataTable dt;
        OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\\thaimaidensha\\dbstock\\database.accdb");

        public frm_tempOut(string Admin_Name)
        {
            InitializeComponent();
            textBox7.Text = Admin_Name;
            this.WindowState = FormWindowState.Maximized;
        }

        private void frm_tempOut_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'databaseDataSet.tb_tempOut' table. You can move, or remove it, as needed.
            this.tb_tempOutTableAdapter.Fill(this.databaseDataSet.tb_tempOut);
            oda = new OleDbDataAdapter("SELECT ID, OutDate, OutNo, OutName, OutAmount, OutUnit, OutPriceUnit, OutPriceTotal, OutPerson, OutStorage, OutAdmin FROM tb_tempOut", conn);
            dt = new DataTable();
            oda.Fill(dt);
            dataGridView1.DataSource = dt;
            textBox1.Text = DateTime.Now.ToString("dd/MM/yyyy");
            ActiveControl = textBox5;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OleDbCommand addinv = new OleDbCommand();
            OleDbCommand checkInvNo = new OleDbCommand();
            conn.Open();
            addinv.Connection = conn;
            checkInvNo.Connection = conn;
            checkInvNo.CommandText = "SELECT * FROM tb_Inv WHERE InvNo = '" + textBox2.Text + "'";
            OleDbDataReader readerCheckInvNo = checkInvNo.ExecuteReader();
            int count = 0;
            while (readerCheckInvNo.Read())
            {
                count = count + 1;
                if (count == 1)
                {
                    break;
                }
            }
            //พบว่ามีสินค้าชนิดที่ต้องการอยู่
            if (count == 1)
            {
                DataSet ds = new DataSet();
                OleDbDataAdapter da = new OleDbDataAdapter("SELECT InvName, InvUnit, InvPriceUnit, InvAmount, InvStorage FROM tb_Inv WHERE InvNo = '" + textBox2.Text + "'", conn);
                da.Fill(ds, "Inv_name");
                string text_invname = " ", text_recunit = " ", text_priceunit = " ", text_Amount = " ", text_storage = " ";
                foreach (DataRow dr in ds.Tables["Inv_name"].Rows)
                {
                    text_invname = dr.ItemArray[0] + "";
                    text_recunit = dr.ItemArray[1] + "";
                    text_priceunit = dr.ItemArray[2] + "";
                    text_Amount = dr.ItemArray[3] + "";
                    text_storage = dr.ItemArray[4] + "";
                    break;
                }
                //หาว่าใน tempOut มีสินค้าชนิดนี้อยู่ไหม
                OleDbCommand checkInTempOut = new OleDbCommand();
                checkInTempOut.Connection = conn;
                checkInTempOut.CommandText = "SELECT * FROM tb_tempOut WHERE OutNo = '" + textBox2.Text + "' AND OutPerson = '" + textBox7.Text + "'";
                OleDbDataReader readerCheckInTempOut = checkInTempOut.ExecuteReader();
                int count2 = 0;
                while (readerCheckInTempOut.Read())
                {
                    count2 = count2 + 1;
                    if (count2 == 1)
                    {
                        break;
                    }
                }
                //พบว่ามีสินค้าตัวนี้อยู่ใน tempOutอยู่แล้ว
                if (count2 == 1)
                {
                    DataSet ds2 = new DataSet();
                    OleDbDataAdapter da2 = new OleDbDataAdapter("SELECT OutAmount FROM tb_tempOut WHERE OutNo = '" + textBox2.Text + "'", conn);
                    da2.Fill(ds2, "Inv_tempOut");
                    string out_amount = "";
                    foreach (DataRow dr in ds2.Tables["Inv_tempOut"].Rows)
                    {
                        out_amount = dr.ItemArray[0] + "";
                        break;
                    }
                    int out_amount_tempOut = 0;
                    if (int.TryParse(out_amount, out out_amount_tempOut))
                    {
                        int amount_program, amount_db, checkamount = 0;
                        if (int.TryParse(text_Amount, out amount_db) && int.TryParse(textBox3.Text, out amount_program) && textBox1.Text != "" && textBox2.Text != "" && textBox3.Text != "" && textBox4.Text != "" && textBox7.Text != "")
                        {
                            checkamount = amount_db - (amount_program + out_amount_tempOut);
                            if (checkamount >= 0 && text_invname != "" && text_recunit != "" && text_priceunit != "" && text_Amount != "" && text_storage != "")
                            {
                                addinv.CommandText = "UPDATE tb_tempOut SET OutAmount = OutAmount + '" + amount_program + "' WHERE OutNo = '" + textBox2.Text + "' AND OutPerson = '" + textBox7.Text + "'";
                                addinv.ExecuteNonQuery();
                                textBox1.Clear();
                                textBox2.Clear();
                                textBox3.Clear();
                            }
                            else
                            {
                                MessageBox.Show("สินค้าไม่เพียงพอต่อการเบิก");
                            }
                        }
                        else
                        {
                            MessageBox.Show("กรอกข้อมูลไม่ครบ");
                        }

                    }
                }
                else
                {
                    int amount_program, amount_db, checkamount = 0;
                    if (int.TryParse(text_Amount, out amount_db) && int.TryParse(textBox3.Text, out amount_program) && textBox1.Text != "" && textBox2.Text != "" && textBox3.Text != "" && textBox4.Text != "" && textBox7.Text != "")
                    {
                        checkamount = amount_db - amount_program;
                        if (checkamount >= 0 && text_invname != "" && text_recunit != "" && text_priceunit != "" && text_Amount != "" && text_storage != "")
                        {
                            addinv.CommandText = "INSERT into tb_tempOut (OutDate, OutNo, OutName, OutAmount, OutUnit, OutPriceUnit, OutPerson, OutStorage, OutAdmin) VALUES ('" + textBox1.Text + "','" + textBox2.Text + "', '" + text_invname + "', '" + textBox3.Text + "','" + text_recunit + "','" + text_priceunit + "','" + textBox4.Text + "','" + text_storage + "','" + textBox7.Text + "')";
                            addinv.ExecuteNonQuery();
                            textBox1.Clear();
                            textBox2.Clear();
                            textBox3.Clear();
                        }
                        else
                        {
                            MessageBox.Show("สินค้าไม่เพียงพอต่อการเบิก");
                        }
                    }
                    else
                    {
                        MessageBox.Show("กรอกข้อมูลไม่ครบ");
                    }
                }
            }
            else
            {
                MessageBox.Show("ไม่มีรหัสสินค้านี้นี้");
            }
            conn.Close();
            frm_tempOut_Load(sender, e);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DataSet ds = new DataSet();
            OleDbDataAdapter da = new OleDbDataAdapter("SELECT OutNo, OutAmount FROM tb_tempOut", conn);
            da.Fill(ds, "Inv_no");
            OleDbCommand addtoinv = new OleDbCommand();
            conn.Open();
            addtoinv.Connection = conn;

            string text_invno = " ", text_invamount = " ";
            foreach (DataRow dr in ds.Tables["Inv_no"].Rows)
            {
                text_invno = dr.ItemArray[0] + "";
                text_invamount = dr.ItemArray[1] + "";
                addtoinv.CommandText = "UPDATE tb_Inv SET InvAmount = InvAmount - '" + text_invamount + "' WHERE InvNo = '" + text_invno + "'";
                addtoinv.ExecuteNonQuery();
            }

            OleDbCommand addtoout = new OleDbCommand();
            addtoout.Connection = conn;
            addtoout.CommandText = "INSERT into tb_Out(OutDate, OutNo, OutName, OutAmount, OutUnit, OutPriceUnit, OutPriceTotal, OutPerson, OutStorage, OutAdmin) SELECT OutDate, OutNo, OutName, OutAmount, OutUnit, OutPriceUnit, OutPriceTotal, OutPerson, OutStorage, OutAdmin FROM tb_tempOut";
            addtoout.ExecuteNonQuery();
            OleDbCommand deleteTempOut = new OleDbCommand();
            deleteTempOut.Connection = conn;
            deleteTempOut.CommandText = "DELETE * FROM tb_tempOut";
            deleteTempOut.ExecuteNonQuery();

            conn.Close();
            MessageBox.Show("บันทึกข้อมูลเรียบร้อย");
            frm_tempOut_Load(sender, e);

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            int count = 0;
            if (textBox5.Text != "")
            {
                conn.Open();
                OleDbCommand checkname_bq = new OleDbCommand();
                checkname_bq.Connection = conn;
                checkname_bq.CommandText = "SELECT * FROM tb_Inv WHERE InvNo LIKE '" + textBox5.Text + "%'";
                OleDbDataReader readerCheckNameBQ = checkname_bq.ExecuteReader();
                while (readerCheckNameBQ.Read())
                {
                    count = count + 1;
                    if (count > 1)
                    {
                        break;
                    }
                }
                conn.Close();
            }
            if (count == 1)
            {
                DataSet ds = new DataSet();
                OleDbDataAdapter da = new OleDbDataAdapter("SELECT InvNo FROM tb_Inv WHERE InvNo LIKE '" + textBox5.Text + "%'", conn);
                da.Fill(ds, "Inv_no");
                string text_no = " ";
                foreach (DataRow dr in ds.Tables["Inv_no"].Rows)
                {
                    text_no = dr.ItemArray[0] + "";
                    break;
                }
                conn.Close();
                if (text_no != textBox2.Text)
                {
                    textBox2.Text = text_no;
                    textBox3.Text = "1";
                }
                else
                {
                    textBox3.Text = Convert.ToString(int.Parse(textBox3.Text) + 1);
                }
                textBox5.Clear();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            conn.Open();
            OleDbCommand delete = new OleDbCommand();
            delete.Connection = conn;
            delete.CommandText = "DELETE FROM tb_tempOut WHERE ID = " + textBox6.Text + "";
            delete.ExecuteNonQuery();
            frm_tempOut_Load(sender, e);
            textBox6.Clear();
            conn.Close();
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            if (textBox4.Text != "")
            {
                if (textBox4.TextLength == 6)
                {
                    OleDbCommand checkCust = new OleDbCommand();
                    conn.Open();
                    checkCust.Connection = conn;
                    checkCust.CommandText = "SELECT CustFirstName, CustLastName FROM tb_Customer WHERE CustNo = '" + textBox4.Text + "'";
                    OleDbDataReader readerCheckLogin = checkCust.ExecuteReader();
                    int count = 0;
                    while (readerCheckLogin.Read())
                    {
                        count = count + 1;
                        if (count == 1)
                        {
                            DataSet ds = new DataSet();
                            OleDbDataAdapter da = new OleDbDataAdapter("SELECT CustFirstName, CustLastName FROM tb_Customer WHERE CustNo = '" + textBox4.Text + "'", conn);
                            da.Fill(ds, "FirstNameAndLastName");
                            string Cust_Name = "";
                            foreach (DataRow dr in ds.Tables["FirstNameAndLastName"].Rows)
                            {
                                Cust_Name = dr.ItemArray[0] + " ";
                                Cust_Name += dr.ItemArray[1] + "";
                                break;
                            }
                            textBox4.Text = Cust_Name;
                            break;
                        }
                    }
                    conn.Close();
                }
            }
        }
    }
}