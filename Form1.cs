using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


namespace Group_8_Project
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
          
        }
        static string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=carpark_tlm2004;SslMode=none;";
        Boolean login = false;
        MySqlConnection databaseConnection = new MySqlConnection(connectionString);
        MySqlDataReader reader;
        string username = "";
        private void Form1_Load(object sender, EventArgs e)
        {

            tabControl1.Visible = false;
        }

        
        //LOGIN PAGE
        private void loginButton_Click_1(object sender, EventArgs e)    //DONE
        {
            String query = "SELECT * FROM user_info WHERE Username='" + nameTextbox.Text + "' AND Password='" + passwordTextbox.Text + "';";
            string password;
            
            MySqlCommand commanddatabase = new MySqlCommand(query, databaseConnection); //Inserts query and connection
             
            try
            {
                databaseConnection.Open(); //Open database
                reader = commanddatabase.ExecuteReader(); //Executes query
                if (reader.HasRows)//If rows are detected 
                {
                    while (reader.Read())
                    {
                        username = reader.GetString(0);//Remember, creates a new table, so therefore its 0 and 1 !!! !!!
                        password = reader.GetString(1);
                        if (nameTextbox.Text == username && passwordTextbox.Text == password)
                        {
                            nameTextbox.Text = "";
                            passwordTextbox.Text = "";
                            MessageBox.Show("Welcome, Carpark Search is now accessible.");
                            login = true;
                            textBox3.Text = username;
                            textBox4.Text = reader.GetString(2);
                            cardTextBox.Text = reader.GetString(3);
                        }
                        else
                        {
                            MessageBox.Show("Error! Log In denied!!");
                        }
                    }// End of While Read()
                    reader.Close();
                }
            }
            catch (Exception ek) //Check if something wrong, cannot establish connection, read or no data.
            {
                MessageBox.Show("Error! " + ek.Message);
            }
            databaseConnection.Close(); //Close the database connection
            if (username != "admin")
            {
                tabControl1.TabPages.Remove(tabPage7);
                
            }
            panel1.Visible = false;
            tabControl1.Visible = true;
            tabControl1.SelectedTab = tabPage2;
            panel2.Visible = false;
            splitContainer1.Visible = false;
            panel3.Visible = false;
            panel4.Visible = false;
        } 


        //User details page

        private void Topup_button_Click(object sender, EventArgs e)     // DONE
        {
            //string topup = topupTextBox.Text;
            float value = Convert.ToSingle(cardTextBox.Text) + Convert.ToSingle(topupTextBox.Text);


            string query = "UPDATE user_info SET Card_Balance=" + value + " WHERE Username='" + username + "';";
            string query2 = "INSERT INTO visit_history(username, carparkid,duration,cost, top_up, card_balance) Values ( '" + username + "' , 'NIL', 0,0," + Convert.ToSingle(topupTextBox.Text) + "," + value + ");";
            try
            {
                databaseConnection.Open();
                MySqlCommand commanddatabase = new MySqlCommand(query, databaseConnection); //Inserts query and connection
                commanddatabase.ExecuteReader(); //Executes query       
            }
            catch (Exception ek) //Check if something wrong, cannot establish connection, read or no data.
            {
                MessageBox.Show("Error! " + ek.Message);
            }
            reader.Close();
            databaseConnection.Close(); //Close the database connection
            try
            {
                databaseConnection.Open();
                MySqlCommand commanddatabase = new MySqlCommand(query2, databaseConnection); //Inserts query and connection
                commanddatabase.ExecuteReader(); //Executes query 
                MessageBox.Show("Update successful");
                cardTextBox.Text = value.ToString();
            }
            catch (Exception ek) //Check if something wrong, cannot establish connection, read or no data.
            {
                MessageBox.Show("Error! " + ek.Message);
            }
            reader.Close();
            databaseConnection.Close(); //Close the database connection



        } 




        // HDB SEARCH PAGE

        private void button3_Click_1(object sender, EventArgs e)    // HDB_SEARCH TAB, THE SEARCH BUTTON (DONE)
        {
            string query = getSearchQuery_HDB();
            ExcuteQueryDisplay(query, dataGridView1);
            panel2.Visible = true;
            textBox7.Text = "";
            textBox8.Text = "";
            comboBox1.SelectedIndex = -1;
            foreach (int i in checkedListBox1.CheckedIndices)
            {
                checkedListBox1.SetItemCheckState(i, CheckState.Unchecked);
            }
        }
     
        private void button4_Click_1(object sender, EventArgs e)    // SELECT WHICH HDB DATA TO USE (DONE)
        {
            DataGridViewRow row = dataGridView1.CurrentRow;
            textBox9.Text = dataGridView1.CurrentRow.Cells[0].Value.ToString();
            textBox10.Text = dataGridView1.CurrentRow.Cells[1].Value.ToString();
            textBox1.Text = dataGridView1.CurrentRow.Cells[2].Value.ToString();
            textBox11.Text = dataGridView1.CurrentRow.Cells[3].Value.ToString();
            textBox12.Text = dataGridView1.CurrentRow.Cells[4].Value.ToString();
            
            if (Redis.ReadData("localhost", textBox9.Text) == "(nil)" )
            {
                Avail_lots.Text = "N.A.";
            }
            
            Avail_lots.Text = Redis.ReadData("localhost", textBox9.Text);

            string ratesid = dataGridView1.CurrentRow.Cells[5].Value.ToString();
            string query = "SELECT * FROM Rates WHERE Rates_Id = '" + ratesid + "';";
            ExcuteQueryDisplay(query, dataGridView4);


            splitContainer1.Visible = true;
        }

        private void dataGridView4_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex > 0)
            {
                textBox15.Text = dataGridView4.SelectedCells[0].Value.ToString();
            }
        }   // selecting carparking rate on hdb search page

        private void Hdb_Calculate_button_Click(object sender, EventArgs e)
        {

            float rate, hr = 0, mins = 0, duration, total;
            rate = Convert.ToSingle(textBox15.Text);
            if (textBox2.Text != "" )
            {
                hr = Convert.ToSingle(textBox2.Text);
            }
            if (textBox5.Text != "")
            {
                mins = Convert.ToSingle(textBox5.Text)/60;
            }
   
            duration = hr + mins;
            total = rate * duration;
            textBox16.Text = String.Format("{0:0.00}",total);
        }       // Calculate rates

        private void SaveTo_History_Click(object sender, EventArgs e)
        {
            int hr = 0, mins = 0;
            if (textBox2.Text != "")
            {
                hr = Convert.ToInt16(textBox2.Text);
            }
            if (textBox5.Text != "")
            {
                mins = Convert.ToInt16(textBox5.Text);
            }

            int duration = (hr * 60) + mins;
            float cost = Convert.ToSingle(textBox16.Text);

            string query = "INSERT INTO Search_History (Username, Carparkid, Total_Duration,Total_Cost, Search_Type)" +
                " VALUES ('" + username + "', '" + textBox9.Text + "', " + duration + ", " + cost + ", 'HDB' );";

            try
            {
                databaseConnection.Open();
                MySqlCommand commanddatabase = new MySqlCommand(query, databaseConnection); //Inserts query and connection
                reader = commanddatabase.ExecuteReader(); //Executes query
                MessageBox.Show("Saved to Favourites!");
                

            }
            catch (Exception ek) //Check if something wrong, cannot establish connection, read or no data.
            {
                MessageBox.Show("Error! " + ek.Message);
            }
            reader.Close();
            databaseConnection.Close(); //Close the database connection

            panel1.Visible = false;
            tabControl1.Visible = true;
            panel2.Visible = false;
            splitContainer1.Visible = false;
            panel3.Visible = false;
            panel4.Visible = false;
            textBox2.Text = "";
            textBox5.Text = "";
            textBox15.Text = "";
            textBox16.Text = "";

        }   // Save to Favourites

        private void back_button_Click(object sender, EventArgs e)
        {
            panel1.Visible = false;
            tabControl1.Visible = true;
            panel2.Visible = false;
            splitContainer1.Visible = false;
            panel3.Visible = false;
            panel4.Visible = false;
            textBox2.Text = "";
            textBox5.Text = "";
            textBox15.Text = "";
            textBox16.Text = "";
        }       // Back to menu from hdb search

        private void button5_Click(object sender, EventArgs e)
        {
            int hr = 0, mins = 0;
            if (textBox2.Text != "")
            {
                hr = Convert.ToInt16(textBox2.Text);
            }
            if (textBox5.Text != "")
            {
                mins = Convert.ToInt16(textBox5.Text);
            }

            int duration = (hr * 60) + mins;
            float cost = Convert.ToSingle(textBox16.Text);
            string carparkid = textBox9.Text;

            float bal_value = Convert.ToSingle(cardTextBox.Text) - cost;


            string query = "UPDATE user_info SET Card_Balance=" + bal_value + " WHERE Username='" + username + "';";
            string query2 = "INSERT INTO visit_history(Username, Carparkid, Duration, Cost, Card_Balance) Values('" + username +"', '" + carparkid + "', " + duration + ", " + cost + ", " + bal_value + ");";
            try
            {
                databaseConnection.Open();
                MySqlCommand commanddatabase = new MySqlCommand(query, databaseConnection); //Inserts query and connection
                commanddatabase.ExecuteReader(); //Executes query       
            }
            catch (Exception ek) //Check if something wrong, cannot establish connection, read or no data.
            {
                MessageBox.Show("Error! " + ek.Message);
            }
            reader.Close();
            databaseConnection.Close(); //Close the database connection
            try
            {
                databaseConnection.Open();
                MySqlCommand commanddatabase = new MySqlCommand(query2, databaseConnection); //Inserts query and connection
                commanddatabase.ExecuteReader(); //Executes query 
                String.Format("{0:0.00}", bal_value);
                MessageBox.Show("Update to history successful. You have spent $" + cost + ". Your Card Balance is: $" + bal_value + ".");
                cardTextBox.Text = bal_value.ToString();
                
            }
            catch (Exception ek) //Check if something wrong, cannot establish connection, read or no data.
            {
                MessageBox.Show("Error! " + ek.Message);
            }
            reader.Close();
            databaseConnection.Close(); //Close the database connection


            panel1.Visible = false;
            tabControl1.Visible = true;
            panel2.Visible = false;
            splitContainer1.Visible = false;
            panel3.Visible = false;
            panel4.Visible = false;
            textBox2.Text = "";
            textBox5.Text = "";
            textBox15.Text = "";
            textBox16.Text = "";
        }           // Save to History




        // When changing tabs
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string query = "";
            if (tabControl1.SelectedIndex == 3)
            {
                query = "select c.address,h.total_duration,h.total_cost,h.search_type  from search_history h, carpark c where c.carparkid = h.carparkid and h.username = '" + username + "';";
                ExcuteQueryDisplay(query, dataGridView3);
            }
            if (tabControl1.SelectedIndex == 4)
            {
                query = "select c.address,v.duration,v.cost,v.top_up,v.card_balance from carpark c, visit_history v WHERE v.carparkid = c.carparkid AND username ='" + username + "';";
                ExcuteQueryDisplay(query, dataGridView6);
            }

            if (tabControl1.SelectedIndex == 5)
            {
                query = "select username, contact_no from user_info";
                ExcuteQueryDisplay(query, dataGridView7);
                query = "select COUNT(*) from user_info;";
                try
                {
                    databaseConnection.Open();
                    MySqlCommand commanddatabase = new MySqlCommand(query, databaseConnection); //Inserts query and connection
                    reader = commanddatabase.ExecuteReader();

                    while (reader.Read())
                    {
                        textBox13.Text = reader.GetString(0);
                    }
                }
                catch (Exception ek) //Check if something wrong, cannot establish connection, read or no data.
                {
                    MessageBox.Show("Error! " + ek.Message);
                }
                reader.Close();
                databaseConnection.Close(); //Close the database connection
                tabControl2.SelectedTab = tabPage8;

            }
        }
        
        // Search History Page
        private void clear_history_Click(object sender, EventArgs e)
        {
            string query = "delete from search_history where username = '" + username + "';";

            try
            {
                databaseConnection.Open();
                MySqlCommand commanddatabase = new MySqlCommand(query, databaseConnection); //Inserts query and connection
                reader = commanddatabase.ExecuteReader(); //Executes query
                MessageBox.Show("Search History has been deleted! Please refresh the page.");


            }
            catch (Exception ek) //Check if something wrong, cannot establish connection, read or no data.
            {
                MessageBox.Show("Error! " + ek.Message);
            }
            reader.Close();
            databaseConnection.Close(); //Close the database connection
        }


        // NON HDB SEARCH
        private void button7_Click_1(object sender, EventArgs e)
        {
            string query = getSearchQuery_NONHDB();
            ExcuteQueryDisplay(query, dataGridView2);
            panel3.Visible = true;
            textBox17.Text = "";
            textBox18.Text = "";
            comboBox4.SelectedIndex = -1;
            comboBox5.SelectedIndex = -1;
        }       // Search Button

        private void button8_Click(object sender, EventArgs e)
        {
            DataGridViewRow row = dataGridView1.CurrentRow;
            textBox19.Text = dataGridView2.CurrentRow.Cells[0].Value.ToString();
            textBox20.Text = dataGridView2.CurrentRow.Cells[1].Value.ToString();
            textBox21.Text = dataGridView2.CurrentRow.Cells[2].Value.ToString();
            textBox22.Text = dataGridView2.CurrentRow.Cells[3].Value.ToString();
            string ratesid = dataGridView2.CurrentRow.Cells[4].Value.ToString();
            string query = "SELECT * FROM Rates WHERE Rates_Id = '" + ratesid + "';";
            ExcuteQueryDisplay(query, dataGridView5);

            panel4.Visible = true;
        }         // Select which carpark

        private void dataGridView5_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex > 0)
            {
                textBox24.Text = dataGridView5.SelectedCells[0].Value.ToString();
            }
        }   // selecting carparking rate on hdb search page

        private void nonhdbCalculate_Click(object sender, EventArgs e)
        {
            float rate, hr = 0, mins = 0, duration, total;
            rate = Convert.ToSingle(textBox24.Text);
            if (textBox14.Text != "")
            {
                hr = Convert.ToSingle(textBox14.Text);
            }
            if (textBox6.Text != "")
            {
                mins = Convert.ToSingle(textBox6.Text) / 60;
            }

            duration = hr + mins;
            total = rate * duration;
            textBox23.Text = String.Format("{0:0.00}", total);
        }       // Calculate Rates

        private void button1_Click(object sender, EventArgs e)
        {
            panel1.Visible = false;
            tabControl1.Visible = true;
            panel2.Visible = false;
            splitContainer1.Visible = false;
            panel3.Visible = false;
            panel4.Visible = false;
            textBox17.Text = "";
            textBox23.Text = "";
            textBox6.Text = "";
            textBox14.Text = "";
            textBox18.Text = "";
        }           // Back to Menu

        private void button9_Click(object sender, EventArgs e)
        {
            int hr = 0, mins = 0;
            if (textBox14.Text != "")
            {
                hr = Convert.ToInt16(textBox14.Text);
            }
            if (textBox6.Text != "")
            {
                mins = Convert.ToInt16(textBox6.Text);
            }

            int duration = (hr * 60) + mins;
            float cost = Convert.ToSingle(textBox23.Text);

            string query = "INSERT INTO Search_History (Username, Carparkid, Total_Duration,Total_Cost, Search_Type)" +
                " VALUES ('" + username + "', '" + textBox19.Text + "', " + duration + ", " + cost + ", 'NON_HDB' );";

            try
            {
                databaseConnection.Open();
                MySqlCommand commanddatabase = new MySqlCommand(query, databaseConnection); //Inserts query and connection
                reader = commanddatabase.ExecuteReader(); //Executes query
                MessageBox.Show("Saved to Favourites!");


            }
            catch (Exception ek) //Check if something wrong, cannot establish connection, read or no data.
            {
                MessageBox.Show("Error! " + ek.Message);
            }
            reader.Close();
            databaseConnection.Close(); //Close the database connection

            panel1.Visible = false;
            tabControl1.Visible = true;
            panel2.Visible = false;
            splitContainer1.Visible = false;
            panel3.Visible = false;
            panel4.Visible = false;
            textBox2.Text = "";
            textBox5.Text = "";
            textBox15.Text = "";
            textBox16.Text = "";
        }           // Save to Favourites

        private void button2_Click(object sender, EventArgs e)              // Save to History
        {
            int hr = 0, mins = 0;
            if (textBox14.Text != "")
            {
                hr = Convert.ToInt16(textBox14.Text);
            }
            if (textBox6.Text != "")
            {
                mins = Convert.ToInt16(textBox6.Text);
            }

            int duration = (hr * 60) + mins;
            float cost = Convert.ToSingle(textBox23.Text);
            string carparkid = textBox19.Text;

            float bal_value = Convert.ToSingle(cardTextBox.Text) - cost;


            string query = "UPDATE user_info SET Card_Balance=" + bal_value + " WHERE Username='" + username + "';";
            string query2 = "INSERT INTO visit_history(Username, Carparkid, Duration, Cost, Card_Balance) Values('" 
                + username + "', '" + carparkid + "', " + duration + ", " + cost + ", " + bal_value + ");";
            try
            {
                databaseConnection.Open();
                MySqlCommand commanddatabase = new MySqlCommand(query, databaseConnection); //Inserts query and connection
                commanddatabase.ExecuteReader(); //Executes query       
            }
            catch (Exception ek) //Check if something wrong, cannot establish connection, read or no data.
            {
                MessageBox.Show("Error! " + ek.Message);
            }
            reader.Close();
            databaseConnection.Close(); //Close the database connection
            try
            {
                databaseConnection.Open();
                MySqlCommand commanddatabase = new MySqlCommand(query2, databaseConnection); //Inserts query and connection
                commanddatabase.ExecuteReader(); //Executes query 
                String.Format("{0:0.00}", bal_value);
                MessageBox.Show("Update to history successful. You have spent $" + cost + ". Your Card Balance is: $" + bal_value + ".");
                cardTextBox.Text = bal_value.ToString();
            }
            catch (Exception ek) //Check if something wrong, cannot establish connection, read or no data.
            {
                MessageBox.Show("Error! " + ek.Message);
            }
            reader.Close();
            databaseConnection.Close(); //Close the database connection


            panel1.Visible = false;
            tabControl1.Visible = true;
            panel2.Visible = false;
            splitContainer1.Visible = false;
            panel3.Visible = false;
            panel4.Visible = false;
            textBox2.Text = "";
            textBox5.Text = "";
            textBox15.Text = "";
            textBox16.Text = "";
        }


        // Functions 
        private string getSearchQuery_HDB()     
        {
            string query;
            string carparkidQuery = "", addressQuery = "", evstationQuery = "";
            string carparktypeQuery = "";
            string condition = "WHERE NOT EXISTS ( SELECT carparkid FROM Carpark WHERE c.carparkid LIKE '%z%') AND ";
         

            if (comboBox1.SelectedItem != null)
            {
                evstationQuery = comboBox1.SelectedItem.ToString();
                
            }

            // If a carpark type was chosen, compose query for carpark type  
            if (checkedListBox1.CheckedItems.Count > 0)
            {
                carparktypeQuery = "(h.carpark_type = '";
                for (int i = 0; i < checkedListBox1.CheckedItems.Count; i++)
                {
                    if (i == checkedListBox1.CheckedItems.Count - 1)
                    {
                        carparktypeQuery += checkedListBox1.CheckedItems[i].ToString() + "')";
                    }
                    else { carparktypeQuery += checkedListBox1.CheckedItems[i].ToString() + "' OR h.carpark_type = '"; }
                }
            }

            // If a carpark id was inputted in the text box, compose query for carpark id  
            if (textBox8.Text != "" && (textBox7.Text != "" || evstationQuery != "" || carparktypeQuery != ""))
            {
                carparkidQuery = "c.carparkid = '" + textBox8.Text + "' AND ";
            }
            else if (textBox8.Text != "" && addressQuery == "" && evstationQuery == "" && carparktypeQuery == "")
            {
                carparkidQuery = "c.carparkid = '" + textBox8.Text + "'";
            }

            // If an address was inputted in the text box, compose query for address  
            if (textBox7.Text != "" && (evstationQuery != "" || carparktypeQuery != ""))
            {
                addressQuery = "c.address LIKE '%" + textBox7.Text + "%' AND ";
            }
            else if (textBox7.Text != "" && evstationQuery == "" && carparktypeQuery == "")
            {
                addressQuery = "c.address LIKE '%" + textBox7.Text + "%'";
            }

            // If evstation is not empty, compose query for evstation  
            if (evstationQuery != "" && carparktypeQuery != "")
            {
                evstationQuery = "c.ev_station = '" + evstationQuery + "' AND ";
            }
            else if (evstationQuery != "" && carparktypeQuery == "")
            {
                evstationQuery = "c.ev_station = '" + evstationQuery + "'";
            }

            // If all field was left blank, all variables to be blank too, and the query will be everything in the table
            if (carparkidQuery == "" && addressQuery == "" && evstationQuery == "" && carparktypeQuery == "")
            {
                condition = "WHERE NOT EXISTS ( SELECT carparkid FROM Carpark WHERE c.carparkid LIKE '%z%') ";
            }

            query = "SELECT c.Carparkid, c.Address, h.Carpark_Type, c.Ev_Station, h.Total_Lots, c.Rates_Id FROM Carpark c, Hdb_Carpark h " 
                + condition + carparkidQuery + addressQuery + evstationQuery + carparktypeQuery + ";";

            return query;
        }

        private void ExcuteQueryDisplay(string query, DataGridView databox)
        {
            try
            {
                databaseConnection.Open();
                MySqlCommand commanddatabase = new MySqlCommand(query, databaseConnection); //Inserts query and connection
                reader = commanddatabase.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(reader);
                databox.DataSource = dt;
            }
            catch (Exception ek) //Check if something wrong, cannot establish connection, read or no data.
            {
                MessageBox.Show("Error! " + ek.Message);
            }
            reader.Close();
            databaseConnection.Close(); //Close the database connection
        }

        private string getSearchQuery_NONHDB()
        {
            string query = "";
            string carparkidQuery = "", addressQuery = "", evstationQuery = "", region = "";
            string condition = "WHERE c.carparkid LIKE '%z%' AND c.carparkid = h.carparkid AND ";

            if (comboBox5.SelectedItem != null)
            {
                evstationQuery = comboBox5.SelectedItem.ToString();

            }
            
            if (comboBox4.SelectedItem != null)
            {
                region = "h.region = '" + comboBox4.SelectedItem.ToString() + "'";
            }

            // // If a carpark id was inputted in the text box, compose query for carpark id  
            if (textBox17.Text != "" && (textBox18.Text != "" || evstationQuery != "" || region != ""))
            {
                carparkidQuery = "c.carparkid = '" + textBox17.Text + "' AND ";
            }
            else if (textBox17.Text != "" && addressQuery == "" && evstationQuery == "" && region == "")
            {
                carparkidQuery = "c.carparkid = '" + textBox17.Text + "'";
            }

            // If an address was inputted in the text box, compose query for address  
            if (textBox18.Text != "" && (evstationQuery != "" || region != ""))
            {
                addressQuery = "c.address LIKE '%" + textBox18.Text + "%' AND ";
            }
            else if (textBox18.Text != "" && evstationQuery == "" && region == "")
            {
                addressQuery = "c.address LIKE '%" + textBox18.Text + "%'";
            }


            if (evstationQuery != "" && region != "")
            {
                evstationQuery = "c.ev_station = '" + evstationQuery + "' AND ";
            }
            else if (evstationQuery != "" && region == "")
            {
                evstationQuery = "c.ev_station = '" + evstationQuery + "'";
            }


            if (carparkidQuery == "" && addressQuery == "" && evstationQuery == "" && region == "")
            {
                condition = "WHERE c.carparkid LIKE '%z%' AND c.carparkid = h.carparkid";
            }

            query = "SELECT c.Carparkid, c.Address, h.region, c.Ev_Station, c.Rates_Id FROM Carpark c, non_Hdb_Carpark h " + condition + carparkidQuery + addressQuery + evstationQuery + region + ";";

            return query;
        }


        // When changing tab page 
        private void tabControl2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl2.SelectedIndex == 1)
            {
                string query = "";
                query = "select DISTINCT c.address, count(*) as Count from search_history h, carpark c where c.carparkid = h.carparkid group by h.carparkid  ORDER BY Count DESC;";
                ExcuteQueryDisplay(query, dataGridView8);
            }
            if (tabControl2.SelectedIndex == 2)
            {
                string query = "";
                query = "select DISTINCT c.address, count(*) as Count from visit_history h, carpark c where c.carparkid = h.carparkid group by h.carparkid  ORDER BY Count DESC;";
                ExcuteQueryDisplay(query, dataGridView9);
            }
        }
    }
}
