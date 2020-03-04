﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SqliteCEF
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            Close_Button.Enabled = false;
            CreateCef_Button.Enabled = false;
        }

        private void Browse_Button_Click(object sender, EventArgs e)
        {

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                DbPath.Text = openFileDialog1.FileName;
            }
        }

        private void Connect_Button_Click(object sender, EventArgs e)
        {
            var dbConnection = SQLiteConnector.Instance();

            if (DbPath.Text != string.Empty)
            {
                if (DbPath.Text.Length > 4 && DbPath.Text.Substring(DbPath.Text.Length - 3) == ".db") 
                {
                    try
                    {
                        string root = DbPath.Text;
                        dbConnection.Path = root;
                        if (dbConnection.IsConnect())
                        {
                            Connect_Button.Enabled = false;
                            Browse_Button.Enabled = false;
                            Close_Button.Enabled = true;
                            CreateCef_Button.Enabled = true;
                            CreateQuery.Enabled = true;
                            mappingGroupBox.Enabled = true;

                            LoadTables();

                        }
                        else
                        {
                            MessageBox.Show("Something gone wrong!");
                        }
                    }
                    catch (Exception exp)
                    {
                        MessageBox.Show(exp.Message);
                    }
                }
                else
                {
                    MessageBox.Show("Please, chose database from file explorer!");
                }   
            }
            else
            {
                MessageBox.Show("Please, chose database from file explorer!");
            }

        }

        private void CreateCef_Button_Click(object sender, EventArgs e)
        {
            var dbConnection = SQLiteConnector.Instance();

            if (DbPath.Text != String.Empty)
            {
                if (Query_TextBox.Text != String.Empty)
                {
                    try
                    {
                        CEFFormat cef = dbConnection.Select(Query_TextBox.Text).Result;
                        Cef_textBox.Text = cef.CreateFormat(" ");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                }
                else
                {
                    queryError.Text = "Enter query first";
                }
            }
        }

        private void Close_Button_Click(object sender, EventArgs e)
        {
            var dbConnection = SQLiteConnector.Instance();
            try
            {
                dbConnection.Close();

                Connect_Button.Enabled = true;
                Browse_Button.Enabled = true;
                Close_Button.Enabled = false;
                CreateCef_Button.Enabled = false;
                CreateQuery.Enabled = false;
                mappingGroupBox.Enabled = false;

                tables_comboBox.Items.Clear();
                tables_comboBox.SelectedItem = null;
                id_comboBox.Items.Clear();
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void Query_TextBox_TextChanged(object sender, EventArgs e)
        {
            queryError.Text = string.Empty;
        }

        private void LoadTables()
        {
            tables_comboBox.Items.Clear();

            var dbConnection = SQLiteConnector.Instance();

            try
            {
                string[] tables = dbConnection.GetTables().Split('|');

                foreach (var table in tables)
                {
                    if (table != "")
                        tables_comboBox.Items.Add(table);
                }

                tables_comboBox.SelectedIndex = 0;
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
        }

        private void tables_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(tables_comboBox.SelectedItem != null)
            {
                id_comboBox.Items.Clear();
                eventClassId_comboBox.Items.Clear();
                name_comboBox.Items.Clear();
                severity_comboBox.Items.Clear();
                extension_comboBox.Items.Clear();

                var dbConnection = SQLiteConnector.Instance();

                try
                {
                    string[] columns = dbConnection.GetTableData(tables_comboBox.SelectedItem.ToString()).Split('|');

                    foreach(var item in columns)
                    {
                        if (item != "")
                        {
                            id_comboBox.Items.Add(item);
                            eventClassId_comboBox.Items.Add(item);
                            name_comboBox.Items.Add(item);
                            severity_comboBox.Items.Add(item);
                            extension_comboBox.Items.Add(item);
                        }
                    }
                }
                catch (Exception exp)
                {
                    MessageBox.Show(exp.Message);
                }
            }
        }

        private void CreateQuery_Click(object sender, EventArgs e)
        {
            if(version_textBox.Text != string.Empty 
               && vendor_textBox.Text != string.Empty
               && product_textBox.Text != string.Empty
               && deviceVersion_textBox.Text != string.Empty
               && exstension_textBox.Text != string.Empty
               )
            {
               if(id_comboBox.SelectedItem != null
                  && eventClassId_comboBox.SelectedItem != null
                  && name_comboBox.SelectedItem != null
                  && severity_comboBox.SelectedItem != null
                  && extension_comboBox.SelectedItem != null
                  )
                {
                    var dbConnection = SQLiteConnector.Instance();

                    int? lastId;
                    try
                    {
                        lastId = dbConnection.GetLastID(id_comboBox.SelectedItem.ToString(), tables_comboBox.SelectedItem.ToString());
                    }
                    catch 
                    {
                        lastId = null;
                    }

                    Query_TextBox.Text = $"SELECT {version_textBox.Text} AS Version, '{vendor_textBox.Text}' AS Device_Vendor , '{product_textBox.Text}' AS Device_Product, '{deviceVersion_textBox.Text}' AS Device_Version, '{eventClassId_comboBox.SelectedItem.ToString()}' AS EventClassId, '{name_comboBox.SelectedItem.ToString()}' AS Name, '{severity_comboBox.SelectedItem.ToString()}' AS Severity, '{extension_comboBox.SelectedItem.ToString()}' AS {exstension_textBox.Text} FROM {tables_comboBox.SelectedItem.ToString()} WHERE {id_comboBox.SelectedItem.ToString()} = {lastId.ToString()}";

                    mappingError.Text = string.Empty;
                }
                else
                {
                    mappingError.Text = "Please, select columns!";
                }
            }
            else
            {
                mappingError.Text = "Please, enter values!";
            }
        }

        private void ClearControls(GroupBox groupBox)
        {
            
        }

    }
}
