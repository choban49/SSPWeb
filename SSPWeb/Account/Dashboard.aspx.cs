using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Npgsql;
using MySql.Data.MySqlClient;


namespace SSPWeb.Account
{
    public partial class Dashboard : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Email"] == null)
                Response.Redirect("Login.aspx");
            lblUserDetails.Text = "Username : " + Session["Email"];
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            using (MySqlConnection sqlCon =
                new MySqlConnection(@"Server=10.206.26.184;Database=sugarcrm;Uid=ssp_app_user;Pwd=sspuser1;"))
            {
                sqlCon.Open();

                var query1 = "SELECT COUNT(1) FROM map_cac_idx WHERE cac = @cac";
                var sqlCmd1 = new MySqlCommand(query1, sqlCon);
                sqlCmd1.Parameters.AddWithValue("@cac", CAC.Text.Trim());
                var count1 = Convert.ToInt32(sqlCmd1.ExecuteScalar());

                if (count1 == 1)
                {
                    lblErrorMessage.Text =
                        "The CAC/GCAC you have entered has already been submitted for Provisioning. Please verify the CAC/GCAC is correct before opening a help desk ticket";
                    lblErrorMessage.Visible = true;
                }

                var count2 = 0;

                string query2a = "SELECT COUNT(1) FROM accounts_cstm WHERE cac_c = @cac";
                MySqlCommand sqlCmd2a = new MySqlCommand(query2a, sqlCon);
                sqlCmd2a.Parameters.AddWithValue("@cac", CAC.Text.Trim());
                int count2a = Convert.ToInt32(sqlCmd2a.ExecuteScalar());

                string query2b = "SELECT COUNT(1) FROM cb_guestaccounts_cstm WHERE gac_c = @cac";
                MySqlCommand sqlCmd2b = new MySqlCommand(query2b, sqlCon);
                sqlCmd2b.Parameters.AddWithValue("@cac", CAC.Text.Trim());
                int count2b = Convert.ToInt32(sqlCmd2b.ExecuteScalar());

                if ((count2a == 0) && (count2b == 0))
                {
                    count2 = 0;
                    lblErrorMessage.Text =
                        "The CAC/GCAC you entered does not exist in Suite. Please verify the CAC/GCAC is correct before opening a help desk ticket";
                    lblErrorMessage.Visible = true;
                    //HttpClientHandler welcome = new HttpClientHandler();
                    //using (var handler = new HttpClientHandler { Credentials = new NetworkCredential("colmhoban", "Clock2019$54") })
                    //using (var client = new HttpClient(handler))
                    //{
                    //    HttpClient rtclient = new HttpClient();

                    //    var stringContent = new FormUrlEncodedContent(new[]
                    //    {
                    //        new KeyValuePair<string, string>("id", "ticket/new"),
                    //        new KeyValuePair<string, string>("Queue", "Tech Support"),
                    //        new KeyValuePair<string, string>("Requestor", "colmhoban"),
                    //        new KeyValuePair<string, string>("Subject","Error in provisioning process " + CAC.Text),
                    //        new KeyValuePair<string, string>("Cc","colmhoban"),
                    //        new KeyValuePair<string, string>("AdminCc","colmhoban"),
                    //        new KeyValuePair<string, string>("Owner","colmhoban"),
                    //        //new KeyValuePair<string, string>("Status", "<...>"),
                    //        new KeyValuePair<string, string>("Text", lblErrorMessage.Text)

                    //    });
                    //    var response = rtclient.PutAsync("https://help.castlebranch.com/REST/1.0/ticket/new", stringContent);
                    //    lblErrorMessage.Text = response.Result.RequestMessage.ToString();
                    //    lblErrorMessage.Visible = true;
                    //}
                }

                count2 = count2a + count2b;

                if ((count1 == 0) && (count2 == 1))
                {
                    if (Parent.Text.Length == 0)
                    {
                        string query3 = "INSERT INTO map_cac_idx (cac) VALUES (@cac);";
                        MySqlCommand sqlCmd3 = new MySqlCommand(query3, sqlCon);
                        sqlCmd3.Parameters.AddWithValue("@cac", CAC.Text.Trim());
                        //sqlCmd3.Parameters.AddWithValue("@parent", Parent.Text.Trim());
                        int count3 = Convert.ToInt32(sqlCmd3.ExecuteScalar());

                        string mailQuery = "SELECT mail(@recipient, @Subject);";
                        MySqlCommand mailsqlCmd = new MySqlCommand(mailQuery, sqlCon);
                        mailsqlCmd.Parameters.AddWithValue("@recipient", Session["Email"]);
                        string vsubject = "Subject: CAC/GCAC Submitted\r\n\r\n CAC/GCAC - " + CAC.Text +
                                          " has been submitted for processing";

                        mailsqlCmd.Parameters.AddWithValue("@Subject", vsubject);

                        int mCount = mailsqlCmd.ExecuteNonQuery();

                        if (count3 == 0)
                        {
                            lblSuccessMessage.Text =
                                "CAC / GCAC Added Successfully, please check your email for a response, if you dont get a response within 15 minutes, open a help desk ticket, thank you. ";
                            lblSuccessMessage.Visible = true;
                            if (CAC.Text.StartsWith("G"))
                            {
                                try
                                {
                                    string query4 = "CALL ssp_prov_organization ();";
                                    MySqlCommand sqlCmd4 = new MySqlCommand(query4, sqlCon);
                                    int count4 = sqlCmd4.ExecuteNonQuery();
                                    lblSuccessMessage.Text =
                                        " MySQL - ssp_prov_organization - success";
                                    lblSuccessMessage.Visible = true;
                                }
                                catch (MySqlException exception)
                                {
                                    lblErrorMessage.Text =
                                        "We're sorry but there was an error while attempting to process this CAC/GCAC "
                                        + "MySQL - 1a : ssp_prov_organization - " + CAC.Text + " - " +
                                        exception.Message;
                                    lblErrorMessage.Visible = true;

                                }

                            }
                            else
                            {
                                try
                                {
                                    string query4b = "CALL ssp_prov_organization ();";
                                    MySqlCommand sqlCmd4B = new MySqlCommand(query4b, sqlCon);
                                    int count4b = sqlCmd4B.ExecuteNonQuery();
                                }
                                catch (MySqlException exception)
                                {
                                    lblErrorMessage.Text =
                                        "We're sorry but there was an error while attempting to process this CAC/GCAC "
                                        + "MySQL - 1 : ssp_prov_organization - " + CAC.Text + " - " + exception.Message;
                                    lblErrorMessage.Visible = true;

                                }

                                try
                                {
                                    string query8 = "CALL ssp_process_new_admins ();";
                                    MySqlCommand sqlCmd8 = new MySqlCommand(query8, sqlCon);
                                    int count8 = sqlCmd8.ExecuteNonQuery();
                                }
                                catch (MySqlException exception)
                                {
                                    lblErrorMessage.Text =
                                        "We're sorry but there was an error while attempting to process this CAC/GCAC "
                                        + "MySQL - 5 : ssp_process_new_admins - " + CAC.Text + " - " +
                                        exception.Message;
                                    lblErrorMessage.Visible = true;

                                }

                                try
                                {
                                    string query9 = "CALL ssp_process_new_guests ();";
                                    MySqlCommand sqlCmd9 = new MySqlCommand(query9, sqlCon);
                                    int count9 = sqlCmd9.ExecuteNonQuery();
                                }
                                catch (MySqlException exception)
                                {
                                    lblErrorMessage.Text =
                                        "We're sorry but there was an error while attempting to process this CAC/GCAC "
                                        + "MySQL - 6 :ssp_process_new_guests - " + CAC.Text + " - " + exception.Message;
                                    lblErrorMessage.Visible = true;

                                }

                                try
                                {
                                    string query10 = "CALL ssp_process_new_students ();";
                                    MySqlCommand sqlCmd10 = new MySqlCommand(query10, sqlCon);
                                    int count10 = sqlCmd10.ExecuteNonQuery();
                                }
                                catch (MySqlException exception)
                                {
                                    lblErrorMessage.Text =
                                        "We're sorry but there was an error while attempting to process this CAC/GCAC "
                                        + "MySQL - 7 :ssp_process_new_students - " + CAC.Text + " - " +
                                        exception.Message;
                                    lblErrorMessage.Visible = true;

                                }

                            }

                            using (NpgsqlConnection pgsqlCon =
                                new NpgsqlConnection(
                                    @"Host=10.206.24.14;Database=bridges_platform;Uid=ssp_app_user;Pwd=sspuser1;"))
                            {
                                pgsqlCon.Open();

                                try
                                {
                                    string pgquery1a = "INSERT INTO provision.map_cac_idx (cac) VALUES (@cac);";
                                    NpgsqlCommand pgsqlCmd1 = new NpgsqlCommand(pgquery1a, pgsqlCon);
                                    pgsqlCmd1.Parameters.AddWithValue("@cac", CAC.Text.Trim());
                                    //pgsqlCmd1.Parameters.AddWithValue("@parent", Parent.Text.Trim());
                                    int count1A = Convert.ToInt32(pgsqlCmd1.ExecuteScalar());
                                    lblSuccessMessage.Text =
                                        " PostgreSQL - INSERT map_cac_idx - success";
                                    lblSuccessMessage.Visible = true;
                                    //string pgquery1 = "SELECT * FROM provision.ssp_prep_mysql_data();";
                                    //NpgsqlCommand pgsqlCmd1 = new NpgsqlCommand(pgquery1, pgsqlCon);
                                    //int pgcount1 = pgsqlCmd1.ExecuteNonQuery();
                                }
                                catch (NpgsqlException exception)
                                {
                                    Console.WriteLine(exception.Message);
                                    lblErrorMessage.Text =
                                        "We're sorry but there was an error while attempting to process this CAC/GCAC "
                                        + "PG 1 : INSERT INTO provision.map_cac_idx - " + CAC.Text + " - " + exception.Message;
                                    lblErrorMessage.Visible = true;

                                }

                                try
                                {
                                    string msqueryO = "SELECT mx.id, mx.parent_id, mx.source_table, mx.source_id, mx.cac, mx.account_name, mx.demo_account, " +
                                                      "       mx.member_type, mx.organization_id, mx.organization_type, mx.profile_attributes" +
                                                      "  FROM sugarcrm.map_organizations mx" +
                                                      "  JOIN sugarcrm.map_cac_idx mi ON mx.cac = mi.cac" +
                                                      " WHERE mi.processing = 0";

                                    string pgquery2 = "INSERT INTO provision.map_organizations " +
                                                      "(id, parent_id, source_table, source_id, cac, account_name, demo_account, " +
                                                      " member_type, organization_id, organization_type, profile_attributes)" +
                                                      "VALUES (@id, @parent_id, @source_table, @source_id, @cac, @account_name, @demo_account," +
                                                      "        @member_type, @organization_id, @organization_type, CAST(@profile_attributes AS JSONB))";

                                    string result = "";

                                    NpgsqlCommand pgsqlCmd2 = new NpgsqlCommand(pgquery2, pgsqlCon);
                                    using (var cmd = new MySqlCommand(msqueryO, sqlCon))
                                    using (var rdr = cmd.ExecuteReader())
                                        
                                        while (rdr.Read())
                                        {
                                            int xValue;
                                            
                                            pgsqlCmd2.Parameters.AddWithValue("@id", rdr.GetInt32(0));
                                            if (rdr.IsDBNull(1))
                                            {
                                                pgsqlCmd2.Parameters.AddWithValue("@parent_id", DBNull.Value);
                                                
                                            }
                                            else
                                            {
                                                pgsqlCmd2.Parameters.AddWithValue("@parent_id", rdr.GetInt32(1));
                                            }
                                            pgsqlCmd2.Parameters.AddWithValue("@source_table", rdr.GetString(2));
                                            pgsqlCmd2.Parameters.AddWithValue("@source_id", rdr.GetString(3));
                                            pgsqlCmd2.Parameters.AddWithValue("@cac", rdr.GetString(4));
                                            pgsqlCmd2.Parameters.AddWithValue("@account_name", rdr.GetString(5));
                                            pgsqlCmd2.Parameters.AddWithValue("@demo_account", rdr.GetString(6));
                                            pgsqlCmd2.Parameters.AddWithValue("@member_type", rdr.GetString(7));
                                            if (rdr.IsDBNull(8) )
                                            {
                                                pgsqlCmd2.Parameters.AddWithValue("@organization_id", DBNull.Value);
                                            }
                                            else
                                            {
                                                pgsqlCmd2.Parameters.AddWithValue("@organization_id", rdr.GetInt32(8));
                                            }
                                            pgsqlCmd2.Parameters.AddWithValue("@organization_type", rdr.GetString(9));
                                            pgsqlCmd2.Parameters.AddWithValue("@profile_attributes", rdr.GetString(10));

                                            result = result + rdr.GetString(4) + " : ";

                                            lblSuccessMessage.Text = result;
                                            lblSuccessMessage.Visible = true;
                                        }
                                    int pgcount2 = pgsqlCmd2.ExecuteNonQuery();
                                     

                                     string pgquery2A = "SELECT * FROM provision.ssp_prov_organizations();";
                                     NpgsqlCommand pgsqlCmd2A = new NpgsqlCommand(pgquery2A, pgsqlCon);
                                     int pgcount2A = pgsqlCmd2A.ExecuteNonQuery();
                                     lblSuccessMessage.Text =
                                        " PostgreSQL - ssp_prov_organizations - success";
                                    lblSuccessMessage.Visible = true;
                                }
                                catch (NpgsqlException exception)
                                {
                                    Console.WriteLine(exception.Message);
                                    lblErrorMessage.Text =
                                        "We're sorry but there was an error while attempting to process this CAC/GCAC "
                                        + "PG 2 : ssp_prov_organizations - " + CAC.Text + " - " + exception.Message;
                                    lblErrorMessage.Visible = true;

                                }

                                try
                                {
                                    string pgquery3 = "SELECT * FROM provision.ssp_prov_cbsetup_users();";
                                    NpgsqlCommand pgsqlCmd3 = new NpgsqlCommand(pgquery3, pgsqlCon);
                                    int pgcount3 = pgsqlCmd3.ExecuteNonQuery();
                                    lblSuccessMessage.Text =
                                        " PostgreSQL - ssp_prov_cbsetup_users - success";
                                    lblSuccessMessage.Visible = true;
                                }
                                catch (NpgsqlException exception)
                                {
                                    Console.WriteLine(exception.Message);
                                    lblErrorMessage.Text =
                                        "We're sorry but there was an error while attempting to process this CAC/GCAC "
                                        + "PG 3 : ssp_prov_cbsetup_users - " + CAC.Text + " - " + exception.Message;
                                    lblErrorMessage.Visible = true;

                                }

                                try
                                {
                                    string pgquery4 = "SELECT provision.ssp_prov_create_group_permissions();";
                                    NpgsqlCommand pgsqlCmd4 = new NpgsqlCommand(pgquery4, pgsqlCon);
                                    int pgcount4 = pgsqlCmd4.ExecuteNonQuery();
                                    lblSuccessMessage.Text =
                                        " PostgreSQL - ssp_prov_create_group_permissions - success";
                                    lblSuccessMessage.Visible = true;
                                }
                                catch (NpgsqlException exception)
                                {
                                    Console.WriteLine(exception.Message);
                                    lblErrorMessage.Text =
                                        "We're sorry but there was an error while attempting to process this CAC/GCAC "
                                        + "PG 4 : ssp_prov_create_group_permissions - " + CAC.Text + " - " +
                                        exception.Message;
                                    lblErrorMessage.Visible = true;

                                }
                               
                                try
                                {
                                    string msqueryUpd = "UPDATE sugarcrm.mysql_map_cac_idx SET processing = 1  WHERE cac = @cac;";
                                    MySqlCommand sqlCmdUpd = new MySqlCommand(msqueryUpd, sqlCon);
                                    sqlCmdUpd.Parameters.AddWithValue("@cac", CAC.Text.Trim());
                                    int countUpd = Convert.ToInt32(sqlCmdUpd.ExecuteScalar());
                                    lblSuccessMessage.Text =
                                        " MySQL - UPDATE map_cac_idx - success";
                                    lblSuccessMessage.Visible = true;
                                }
                                catch (MySqlException exception)
                                {
                                    Console.WriteLine(exception.Message);
                                    lblErrorMessage.Text =
                                        "We're sorry but there was an error while attempting to process this CAC/GCAC "
                                        + "MySQL : UPDATE map_cac_idx - " + CAC.Text + " - " +
                                        exception.Message;
                                    lblErrorMessage.Visible = true;
                                }
                                try
                                {
                                    string pgqueryUpd = "UPDATE provision.map_cac_idx SET processing=1 WHERE cac = @cac;";
                                    NpgsqlCommand pgsqlCmdUpd = new NpgsqlCommand(pgqueryUpd, pgsqlCon);
                                    pgsqlCmdUpd.Parameters.AddWithValue("@cac", CAC.Text.Trim());
                                    int pgcountUpd = pgsqlCmdUpd.ExecuteNonQuery();
                                    lblSuccessMessage.Text =
                                        " PostgreSQL - UPDATE map_cac_idx  - success";
                                    lblSuccessMessage.Visible = true;
                                }
                                catch (NpgsqlException exception)
                                {
                                    Console.WriteLine(exception.Message);
                                    lblErrorMessage.Text =
                                        "We're sorry but there was an error while attempting to process this CAC/GCAC "
                                        + "PG 5 : UPDATE map_cac_idx  - " + CAC.Text + " - " +
                                        exception.Message;
                                    lblErrorMessage.Visible = true;

                                }
                            }
                        }
                    }
                
                }
            }
        }

    }

}