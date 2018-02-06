using System;

public class PopulateList
{

    // Populate drop down list from array
    public void PopulateDropdownList(string[] listArray, DropDownList pageControl)
    {
        DataTable dt = new DataTable();
        olda.Fill(dt);

        // Add blank selection on top
        ListItem li = new ListItem("", "none");
        pageControl.Items.Add(li);
        li.Selected = true;

        // Add list items from array
        pageControl.DataSource = dt;
        pageControl.DataTextField = textField;
        pageControl.DataValueField = valueField;
        pageControl.DataBind();

    }

    // Populate drop down list from OLE database (MS Access)
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
