using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.OleDb;

/*public partial class _Default : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
}*/

public partial class _Default : System.Web.UI.Page
{
    string databasePath = "";
    OleDbConnection con = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=L:\Programs\Illustrators\Illustration Request Database\Administration\illustration request backend - Copy.accdb");
    protected void Page_Load(object sender, EventArgs e)
    {

        if (!Page.IsPostBack)
        {
            LoadUserDropdown();
            LoadGrid("request_index_number", "desc", personFilterText());
        }
    }

    public string personFilterText()
    {
        string filterSelection = UserList.SelectedItem.Value;
        if (filterSelection == "none")
        {
            return "";
        }
        else
        {
            return " and requested_by = '" + filterSelection + "'";
        }

    }

    public void LoadUserDropdown()
    {
        using (OleDbCommand cmd = new OleDbCommand("select distinct requested_by from ILLUSTRATION_REQUEST order by requested_by", con))
        {
            try
            {
                OleDbDataAdapter olda = new OleDbDataAdapter(cmd);
                DataTable dt = new DataTable();
                olda.Fill(dt);

                UserList.DataSource = dt;
                UserList.DataTextField = "requested_by";
                UserList.DataValueField = "requested_by";
                UserList.DataBind();
            }
            catch (Exception ex)
            {
                ErrorDisplay("Error: " + ex.ToString());
            }
        }
    }

    public void LoadGrid(string sortExpr, string sortDirection, string filterString = "")
    {
        string searchString = "select request_index_number,filename,request_status,requested_by,data_module_code,request_date,illustrator_assigned from ILLUSTRATION_REQUEST WHERE request_status='New'" + filterString + " order by " + sortExpr + " " + sortDirection;
        System.Console.WriteLine(searchString);
        using (OleDbCommand cmd = new OleDbCommand(searchString, con))
        {
            try
            {
                OleDbDataAdapter olda = new OleDbDataAdapter(cmd);
                DataTable dt = new DataTable();
                olda.Fill(dt);

                GridView1.DataSource = dt;
                GridView1.DataBind();
            }
            catch (Exception ex)
            {
                ErrorDisplay("Error: " + ex.ToString());
            }
        }
    }

    protected void GridView1_Sorting(object sender, GridViewSortEventArgs e)
    {
        string sortExpression = e.SortExpression;

        ViewState["SortExpression"] = sortExpression;
        if (GridViewSortDirection == SortDirection.Ascending)
        {
            GridViewSortDirection = SortDirection.Descending;
            LoadGrid(sortExpression, "desc", personFilterText());
        }
        else
        {
            GridViewSortDirection = SortDirection.Ascending;
            LoadGrid(sortExpression, "asc", personFilterText());
        }
    }

    private SortDirection GridViewSortDirection
    {
        get
        {
            if (ViewState["sortDirection"] == null)
                ViewState["sortDirection"] = SortDirection.Ascending;
            return (SortDirection)ViewState["sortDirection"];
        }
        set { ViewState["sortDirection"] = value; }
    }

    protected void Search_Click(object sender, EventArgs e)
    {
        LoadGrid("request_index_number", "desc", personFilterText());
    }

    private void ErrorDisplay(string errorMessage)
    {
        System.Diagnostics.Debug.WriteLine(errorMessage);
    }

}