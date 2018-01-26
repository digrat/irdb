using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.OleDb;


public partial class _Default : System.Web.UI.Page
{
    // Colors
    const string warningBackColor = "#FFC0CB"; // Pink
    const string warningTextColor = "#A52A2A"; // Red
    const string normalBackColor = "#FFFFFF";  // White
    const string normalTextColor = "#000000";  // Black
    const string validBackColor = "#90EE90"; // Light Green
    const string validTextColor = "#008000"; // Dark Green

    // Cookies
    const int cookieExpireDays = 3;
    const string requestDetailsCookie = "RequestDetails";
    const string icnFindAddCookie = "ICNFindAdd";
    const string icnListCookie = "ICNList";
    const string attachmentsCookie = "AttachmentDetails"; // subkey name = amended filename (w/ date/time stamp); subkey value = full file path

    // Files and uploads
    string destinationFolder = null; // Made this a global so that after each attachment upload it doesn't need to be pulled again
    const string dateTimeFormat = "yyyyMMdd HHmmss";

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
        }
        PopulateAttachmentFromCookie();
    }

    /********************/
    /* DROPDOWN METHODS */
    /********************/

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

    private void LoadDataView()
    {
        DataTable dt = new DataTable();
        dt.Columns.Add("filename");
        UploadedFiles.DataSource = dt;
        UploadedFiles.DataBind();
        ViewState["CurrentTable"] = dt;
    }

    /************************/
    /* PAGE CONTROL METHODS */
    /************************/


    public void AddIllustrationControlNumberToTextbox(string newIllustrationControlNumber, ListBox pageControl)
    {
        // Before adding an item to the listbox, check to see if it's already there.
        Boolean foundMatch = false;
        foreach (ListItem li in pageControl.Items)
        {
            if (li.Text == newIllustrationControlNumber)
            {
                foundMatch = true;
            }
        }

        // If it's not already there, add it.
        if (foundMatch == false)
        {
            pageControl.Items.Add(newIllustrationControlNumber);
        }

    }

    /******************************************/
    /* INFORMATION AND ERROR CHECKING METHODS */
    /******************************************/

    public Boolean IsRequiredDropdownFilled(DropDownList requiredDropdown)
    {
        if (requiredDropdown.SelectedValue == "" || requiredDropdown.SelectedValue == "none")
        {
            // Highlight background
            HighlightDropDownList(requiredDropdown, warningBackColor, warningTextColor);
            return false;
        }
        else
        {
            // Remove higlight
            HighlightDropDownList(requiredDropdown, normalBackColor, normalTextColor);
            return true;
        }
    }

    public Boolean IsRequiredTextboxFilled(TextBox requiredTextBox)
    {
        if (requiredTextBox.Text == "")
        {
            // Highlight background
            HighlightTextBox(requiredTextBox, warningBackColor, warningTextColor);
            return false;
        }
        else
        {
            // Remove higlight
            HighlightTextBox(requiredTextBox, normalBackColor, normalTextColor);
            return true;
        }
    }

    public void HighlightDropDownList(DropDownList requiredDropdown, string backColor, string textColor)
    {
        requiredDropdown.BackColor = System.Drawing.ColorTranslator.FromHtml(backColor);
        requiredDropdown.ForeColor = System.Drawing.ColorTranslator.FromHtml(textColor);
    }

    public void HighlightTextBox(TextBox requiredTextBox, string backColor, string textColor)
    {
        requiredTextBox.BackColor = System.Drawing.ColorTranslator.FromHtml(backColor);
        requiredTextBox.ForeColor = System.Drawing.ColorTranslator.FromHtml(textColor);
    }

    public void ErrorDisplay(string errorMessage, Boolean labelVisible = true)
    {
        // If using the error label:
        ErrorLabel.Visible = labelVisible;
        ErrorLabel.Text = errorMessage;

        // Look into using an alert message instead?
    }

    /******************************/
    /* DATABASE INTERFACE METHODS */
    /******************************/

    public string GetMaxIllustrationControlNumberUniqueID(string platformNumber, string functionalGroup)
    {
        int numericUniqueId;
        string defaultUniqueId = "0000";
        OleDbConnection con = OpenDatabaseConnection();
        string SqlString = "SELECT MAX(max_unique_id) as CurrentMax FROM illustration_control_number_max where platform='" + platformNumber + "' and functional_group='" + functionalGroup + "'";
        DataSet ds = new DataSet();

        using (OleDbCommand cmd = new OleDbCommand(SqlString, con))
        {
            try
            {
                OleDbDataAdapter olda = new OleDbDataAdapter(cmd);
                olda.Fill(ds);
                olda.Dispose();
                string newUniqueId = ds.Tables[0].Rows[0].ItemArray[0].ToString();

                // If there isn't an error but there are no return hits...
                if (newUniqueId == null || newUniqueId == "")
                {
                    return defaultUniqueId;
                }
                // There are return hits.
                else
                {
                    // Return a zero-padded 4 digit string.
                    numericUniqueId = Convert.ToInt16(newUniqueId) + 1;
                    return numericUniqueId.ToString("0000");
                }
            }
            catch (Exception ex)
            {
                ErrorDisplay("Error: " + ex);
                return null;
            }
        }
    }

    // This is a generic push of one query to the database
    public Boolean InsertIntoDatabase(string insertString)
    {
        try
        {
            OleDbConnection con = OpenDatabaseConnection();
            OleDbCommand cmd = new OleDbCommand(insertString, con);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            ErrorDisplay("", false);
            return true;
        }
        catch
        {
            ErrorDisplay("Unable to save to database. Connection error.", true);
            return false;
        }
    }

    // This is a generic database pull into a dataset
    public DataSet ReadFromDatabase(string readString)
    {
        OleDbConnection con = OpenDatabaseConnection();
        DataSet ds = new DataSet();

        using (OleDbCommand cmd = new OleDbCommand(readString, con))
        {
            try
            {
                OleDbDataAdapter olda = new OleDbDataAdapter(cmd);
                olda.Fill(ds);
                olda.Dispose();

                return ds;
            }
            catch (Exception ex)
            {
                ErrorDisplay("Error: " + ex);
                return null;
            }
        }
    }

    public OleDbConnection OpenDatabaseConnection()
    {
        OleDbConnection con = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=L:\Programs\Illustrators\Illustration Request Database\Administration\illustration request backend - Copy.accdb");
        return con;
    }

    public void GetDestinationFolder(string projectNumber)
    {

        // This should only return one result. Be sure the folder name ends in a backslash.

        DataSet destination = ReadFromDatabase("SELECT attachment_path FROM PROJECT WHERE project_index_number = 9");
        destinationFolder = destination.Tables[0].Rows[0].ItemArray[0].ToString();
        if (destinationFolder.Substring(destinationFolder.Length - 1, 1) != "\\")
        {
            destinationFolder = destinationFolder + "\\";
            // For now, I'll assume that the destination folder already exists.
        }
    }

    /*********************/
    /* FILE SAVE METHODS */
    /*********************/

    protected void AjaxFileUpload_UploadComplete(object sender, AjaxControlToolkit.AjaxFileUploadEventArgs e)
    {

        if (Request.Cookies[requestDetailsCookie] != null)
        {
            System.Collections.Specialized.NameValueCollection UserInfoCookieCollection;
            UserInfoCookieCollection = Request.Cookies[requestDetailsCookie].Values;
            string projectNumber = Request.Cookies[requestDetailsCookie]["Project"];
            string userName = Request.Cookies[requestDetailsCookie]["UserName"];

            // destinationFolder stored in a global variable so it doesn't need to be fetched from the DB on each upload.
            if (destinationFolder == null)
            {
                GetDestinationFolder(projectNumber);
            }

            // Generate file path
            string amendedFileName = TimeStampFileName(e.FileName);
            string filePath = destinationFolder + amendedFileName;

            //InitializeDownloadedFile();
            AddToAttachmentCookie(amendedFileName, filePath);

            // Save upload file to the file system
            //AjaxFileUpload.SaveAs(MapPath(filePath));  // Relative path on webserver
            AjaxFileUpload.SaveAs(filePath);  // Absolute path on local or network drive
        }
    }

    private string TimeStampFileName(string fileName)
    {
        return Path.GetFileNameWithoutExtension(fileName) + " " + DateTime.Now.ToString(dateTimeFormat) + Path.GetExtension(fileName);
    }

    private void AddToAttachmentCookie(string amendedFileName, string filePath)
    {
        HttpCookie oldCookie = Request.Cookies[attachmentsCookie];
        if (oldCookie.HasKeys)
        {
            System.Diagnostics.Debug.WriteLine("AddToAttachmentCookie method fired");

            string subkeyName;
            string subkeyValue;
            HttpCookie newCookie = new HttpCookie(attachmentsCookie);
            System.Collections.Specialized.NameValueCollection cookieValues = oldCookie.Values;
            string[] cookieValueNames = cookieValues.AllKeys;

            for (int i = 0; i < cookieValues.Count; i++)  // Loop through all existing cookie values (files) and add them to a new cookie.
            {
                subkeyName = Server.HtmlEncode(cookieValueNames[i]);
                subkeyValue = Server.HtmlEncode(cookieValues[i]);

                newCookie.Values[subkeyName] = subkeyValue;
            }

            newCookie.Values[amendedFileName] = filePath;  // Add the most recent file to the list
            newCookie.Expires = DateTime.Now.AddDays(cookieExpireDays);  // Reset cookie expiration date
            Response.Cookies.Add(newCookie);  // Overwrite old cookie with new cookie
        }
        else
        {
            CreateAttachmentCookie(amendedFileName, filePath);
        }

        PopulateAttachmentFromCookie();
        UploadedFilesPanel.Update();
    }

    private void CreateAttachmentCookie(string amendedFileName, string filePath)
    {
        HttpCookie createAttachmentCookie = new HttpCookie(attachmentsCookie);
        System.Diagnostics.Debug.WriteLine("CreateAttachmentCookie method fired");

        createAttachmentCookie.Values[amendedFileName] = filePath;
        createAttachmentCookie.Expires = DateTime.Now.AddDays(cookieExpireDays);  // Reset cookie expiration date
        Response.Cookies.Add(createAttachmentCookie);
    }

    private void ClearAttachmentCookie()
    {
        HttpCookie createRequestCookie = new HttpCookie(attachmentsCookie)
        {
            Expires = DateTime.Now.AddDays(-1)  // Set cookie expiration date
        };
        Response.Cookies.Add(createRequestCookie);
    }

    private void PopulateAttachmentFromCookie()
    {
        HttpCookie myCookie = Request.Cookies[attachmentsCookie];
        string subkeyName;
        string subkeyValue;
        if (myCookie != null)
        {
            if (myCookie.HasKeys)
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("FileName");

                for (int i = 0; i < myCookie.Values.Count; i++) // Loop through the subkey values (attachments) in attachmentsCookie
                {
                    subkeyName = Server.HtmlEncode(myCookie.Values.AllKeys[i]);
                    subkeyValue = Server.HtmlEncode(myCookie.Values[i]);
                    DataRow dr = dt.NewRow();
                    dr["FileName"] = subkeyName;

                    dt.Rows.Add(dr);
                }

                UploadedFiles.DataSource = dt;
                UploadedFiles.DataBind();
            }
        }
    }

    protected void AjaxFileUpload_UploadCompleteAll(object sender, AjaxControlToolkit.AjaxFileUploadCompleteAllEventArgs e)
    {
        System.Diagnostics.Debug.WriteLine("AjaxFileUpload_UploadCompleteAll method fired");
        UploadedFilesPanel.Update();
    }
}
