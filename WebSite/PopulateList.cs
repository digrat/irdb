using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.OleDb;

/// <summary>
/// Summary description for PopulateList
/// </summary>
public class PopulateList
{

    public void PopulateDropdownList(string query, DropDownList pageControl, string textField, string valueField, OleDbConnection con)
    {
        using (OleDbCommand cmd = new OleDbCommand(query, con))
        {
            try
            {
                OleDbDataAdapter olda = new OleDbDataAdapter(cmd);
                DataTable dt = new DataTable();
                olda.Fill(dt);

                ListItem li = new ListItem("", "none");
                pageControl.Items.Add(li);
                li.Selected = true;

                pageControl.DataSource = dt;
                pageControl.DataTextField = textField;
                pageControl.DataValueField = valueField;
                pageControl.DataBind();

            }
            catch (Exception ex)
            {
                ErrorDisplay("Error: " + ex, true);
            }
        }
    }

    public void PopulateListBox(string query, ListBox pageControl, string textField, string valueField, OleDbConnection con)
    {
        using (OleDbCommand cmd = new OleDbCommand(query, con))
        {
            try
            {
                OleDbDataAdapter olda = new OleDbDataAdapter(cmd);
                DataTable dt = new DataTable();
                olda.Fill(dt);

                pageControl.DataSource = dt;
                pageControl.DataTextField = textField;
                pageControl.DataValueField = valueField;
                pageControl.DataBind();
            }
            catch (Exception ex)
            {
                ErrorDisplay("Error: " + ex, true);
            }
        }
    }

}