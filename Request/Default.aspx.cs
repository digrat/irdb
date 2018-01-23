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
            LoadDataView();
            LoadDropdowns();
        }
        PopulateRequestDetailsFromCookie();
        PopulateAttachmentFromCookie();
    }

    /********************/
    /* DROPDOWN METHODS */
    /********************/

    public void LoadDropdowns()
    {
        // Pass this connection to each method so it doesn't have to be reopened for each dropdown population
        OleDbConnection con = OpenDatabaseConnection();
        LoadUserDropdown(con);
        LoadProjectDropdown(con);
        LoadRequestTypeDropdown(con);
        LoadManualTypeDropdown(con);
        LoadPlatformDropdown(con);
    }

    public void LoadUserDropdown(OleDbConnection con)
    {
        PopulateDropdownList("select distinct requested_by from ILLUSTRATION_REQUEST order by requested_by",
            UserName, "requested_by", "requested_by", con);
    }

    public void LoadProjectDropdown(OleDbConnection con)
    {
        PopulateDropdownList("select project_index_number,project_name from PROJECT where project_active = True order by project_name",
            Project, "project_name", "project_index_number", con);
    }

    public void LoadRequestTypeDropdown(OleDbConnection con)
    {
        PopulateDropdownList("select field_value from FIELDS where table='REQUEST_TRACKING' and field_name='request_type' order by field_description",
            RequestType, "field_value", "field_value", con);
    }

    public void LoadManualTypeDropdown(OleDbConnection con)
    {
        PopulateDropdownList("select field_value,field_name,field_description from FIELDS where table='ILLUSTRATION_CONTROL_NUMBER' and field_name='type' order by field_description",
            ManualType, "field_description", "field_value", con);
    }

    public void LoadPlatformDropdown(OleDbConnection con)
    {
        /* unique_category is a concatenation of <category>|<platform ID> ex: air_SNS|10
        This is required so that each entry has a unique value */
        PopulateDropdownList("select field_description,field_value + '|' + category as unique_category from FIELDS where table='ILLUSTRATION_CONTROL_NUMBER' and field_name='platform' order by field_description",
            AddPlatform, "field_description", "unique_category", con);
        PopulateDropdownList("select field_description,field_value + '|' + category as unique_category from FIELDS where table='ILLUSTRATION_CONTROL_NUMBER' and field_name='platform' order by field_description",
            FindPlatform, "field_description", "unique_category", con);
    }

    public void LoadFunctionalGroupDropdown(OleDbConnection con)
    {
        if (AddPlatform.SelectedValue != null)
        {
            string ParsedCategory = AddPlatform.SelectedValue.Substring(AddPlatform.SelectedValue.IndexOf("|") + 1);
            PopulateDropdownList("select field_value + ' ' + field_description as displaytext, field_value from FIELDS where table='ILLUSTRATION_CONTROL_NUMBER' and field_name='" + ParsedCategory + "' order by field_value",
                AddFunctionalGroup, "displaytext", "field_value", con);
        }
        if (FindPlatform.SelectedValue != null)
        {
            string ParsedCategory = FindPlatform.SelectedValue.Substring(FindPlatform.SelectedValue.IndexOf("|") + 1);
            PopulateDropdownList("select field_value + ' ' + field_description as displaytext, field_value from FIELDS where table='ILLUSTRATION_CONTROL_NUMBER' and field_name='" + ParsedCategory + "' order by field_value",
                FindFunctionalGroup, "displaytext", "field_value", con);
        }
    }

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

    protected void Submit_Click(object sender, EventArgs e)
    {
        DateTime currentTimeStamp = DateTime.Now;

        Boolean illustrationControlNumberError = false;
        string illustrationControlNumbersErrored = "";
        //Boolean illustrationAttachmentError = false;

        if (IsRequiredDropdownFilled(UserName) &&
            IsRequiredDropdownFilled(Project) &&
            IsRequiredDropdownFilled(RequestType) &&
            IsRequiredDropdownFilled(ManualType) &&
            IsRequiredTextboxFilled(DataModuleCode) &&
            IsRequiredTextboxFilled(DataModuleTitle))
        {
            // Loop through each ICN
            foreach (ListItem li in IllustrationControlNumbers.Items)
            {
                // Add item to illustration request table
                string insertString = "INSERT INTO ILLUSTRATION_REQUEST " +
                    "(project_index_number,filename,request_type,request_status,request_date,requested_by,request_modified_by,conversion_source,data_module_code,figure_title) VALUES " +
                    "(" + Project.SelectedItem.Value + ",'" + li.Text + "','" + RequestType.SelectedItem.Value + "','New','" + currentTimeStamp.ToString("MM/dd/yyyy HH:mm:ss") + "','" + UserName.SelectedValue + "','" + UserName.SelectedValue + "','" +
                    ConversionSource.Text + "','" + DataModuleCode.Text + "','" + DataModuleTitle.Text + "')";
                if (InsertIntoDatabase(insertString) == false)
                {
                    illustrationControlNumberError = true;
                    illustrationControlNumbersErrored = illustrationControlNumbersErrored + " " + li.Text;
                }
                else  // On successful ICN insert, attach any pertinent attachments
                {
                    // Loop through all attachments
                    // If attachment file name like '[DMRTU]########*' and left(filename,9) <> this ICN then do nothing
                    // else, "INSERT INTO ATTACHMENT (request_index_number,filename) VALUES (<find the request index number>,<full file path>);"
                }

            }
            if (illustrationControlNumberError)
            {
                ErrorDisplay("Error saving the following ICNs: " + illustrationControlNumbersErrored, true);
            }
        }
        else
        {
            ErrorDisplay("Unable to save request. One or more required fields is empty.");
        }
    }

    protected void DeleteIllustrationControlNumber_Click(object sender, EventArgs e)
    {
        if (IllustrationControlNumbers.SelectedIndex >= 0)
        {
            IllustrationControlNumbers.Items.RemoveAt(IllustrationControlNumbers.Items.IndexOf(IllustrationControlNumbers.SelectedItem));
        }
    }

    protected void AddKnownIllustrationControlNumber_Click(object sender, EventArgs e)
    {
        AddIllustrationControlNumberToTextbox(KnownIllustrationControlNumber.Text, IllustrationControlNumbers);
    }

    protected void FindPlatform_SelectedIndexChanged(object sender, EventArgs e)
    {
        // Is there a way to pass this connection as a constant? Or maybe to make it a data connection?
        OleDbConnection con = OpenDatabaseConnection();
        LoadFunctionalGroupDropdown(con);
    }

    protected void AddPlatform_SelectedIndexChanged(object sender, EventArgs e)
    {
        // Is there a way to pass this connection as a constant? Or maybe to make it a data connection?
        OleDbConnection con = OpenDatabaseConnection();
        LoadFunctionalGroupDropdown(con);
    }

    protected void SelectIllustrationControlNumber_Click(object sender, EventArgs e)
    {
        // Transfer data to main list
        for (int i = 0; i < FindICNList.Items.Count; i++)
        {
            if (FindICNList.Items[i].Selected)
            {
                AddIllustrationControlNumberToTextbox(FindICNList.Items[i].Value, IllustrationControlNumbers);
            }
        }
    }


    protected void AddIllustrationControlNumber_Click(object sender, EventArgs e)
    {
        DateTime currentTimeStamp = DateTime.Now;

        // Check to see if all required fields are populated
        if (IsRequiredDropdownFilled(AddPlatform) &&
            IsRequiredDropdownFilled(AddFunctionalGroup) &&
            IsRequiredDropdownFilled(ManualType) &&
            IsRequiredTextboxFilled(DataModuleCode) &&
            IsRequiredTextboxFilled(DataModuleTitle))
        {
            // Parse out the platform number from Platform.SelectedValue
            string PlatformNumber = AddPlatform.SelectedValue.Substring(0, AddPlatform.SelectedValue.IndexOf("|"));
            string FunctionalGroupNumber = AddFunctionalGroup.SelectedValue;
            // Function returns the current max unique ID; add 1 for the new ICN.
            string UniqueID = GetMaxIllustrationControlNumberUniqueID(PlatformNumber, FunctionalGroupNumber);
            if (UniqueID != null)
            {
                string newIllustrationControlNumber = ManualType.SelectedValue +
                        Convert.ToInt16(PlatformNumber).ToString("00") +
                        FunctionalGroupNumber +
                        UniqueID;
                // Insert the new ICN into the database
                string insertString = "insert into ILLUSTRATION_CONTROL_NUMBER (illustration_control_number,title,data_module_code,requested_by,request_date) " +
                    "values ('" + newIllustrationControlNumber + "','" + DataModuleTitle.Text + "','" + DataModuleCode.Text + "','" + UserName.SelectedValue + "','" + currentTimeStamp.ToString("MM/dd/yyyy HH:mm:ss") + "')";
                if (InsertIntoDatabase(insertString))
                {
                    // Post new ICN into appropriate ICN textbox
                    AddIllustrationControlNumberToTextbox(newIllustrationControlNumber, IllustrationControlNumbers);
                }
                else
                {
                    //ErrorDisplay(insertString, true);
                    ErrorDisplay("Unable to create new ICN. Error saving ICN " + newIllustrationControlNumber + " to database.");
                }
            }
        }
        else
        {
            ErrorDisplay("Unable to create new ICN. One or more required fields is empty.");
        }
    }

    protected void CloseAddPopup_Click(object sender, EventArgs e)
    {
        // This method may not even fire since this button is designated as the popup close button.

        // Remove highlights on new ICN required fields?
        HighlightDropDownList(AddPlatform, normalBackColor, normalTextColor);
        HighlightDropDownList(AddFunctionalGroup, normalBackColor, normalTextColor);
        HighlightDropDownList(ManualType, normalBackColor, normalTextColor);
        HighlightTextBox(DataModuleCode, normalBackColor, normalTextColor);
        HighlightTextBox(DataModuleTitle, normalBackColor, normalTextColor);
    }

    protected void FindIllustrationControlNumber_Click(object sender, EventArgs e)
    {
        Boolean platformFilled = IsRequiredDropdownFilled(FindPlatform);
        Boolean functionalGroupFilled = IsRequiredDropdownFilled(FindFunctionalGroup);
        Boolean manualTypeFilled = IsRequiredDropdownFilled(ManualType);

        OleDbConnection con = OpenDatabaseConnection();

        if (platformFilled && functionalGroupFilled && manualTypeFilled)
        {
            // Parse out the platform number from Platform.SelectedValue
            string PlatformNumber = FindPlatform.SelectedValue.Substring(0, FindPlatform.SelectedValue.IndexOf("|"));
            string FunctionalGroupNumber = FindFunctionalGroup.SelectedValue;
            // Function returns the current max unique ID; add 1 for the new ICN.
            PopulateListBox("select illustration_control_number from illustration_control_number_breakdown where platform = '" + PlatformNumber + "' and functional_group = '" + FunctionalGroupNumber + "'",
                FindICNList, "illustration_control_number", "illustration_control_number", con);
        }
        else
        {
            ErrorDisplay("Unable to find ICNs. One or more required fields is empty.");
        }
    }

    protected void ValidateIllustrationControlNumbers_Click(object sender, EventArgs e)
    {
        int countOfIllustrationControlNumbers = IllustrationControlNumbers.Items.Count;

        // Check to see if there are any ICNs yet
        if (countOfIllustrationControlNumbers > 0)
        {
            /* Assume that the first ICN header (R0206 - <manual type><platform><functional group>) is the preferred and that
             * all following ICNs should follow suit. */
            int lengthOfHeader = 5;
            int startOfHeader = 0;
            int itemToSelect = 0;
            string illustrationControlNumberHeader = IllustrationControlNumbers.Items[itemToSelect].Value.Substring(startOfHeader, lengthOfHeader);

            // Populate data table
            OleDbConnection con = OpenDatabaseConnection();
            string sqlString = "SELECT illustration_control_number FROM illustration_control_number_breakdown where illustration_control_number like '" + illustrationControlNumberHeader + "%'";
            //System.Diagnostics.Debug.WriteLine(sqlString);
            DataTable dt = new DataTable();

            using (OleDbCommand cmd = new OleDbCommand(sqlString, con))
            {
                try
                {
                    OleDbDataAdapter olda = new OleDbDataAdapter(cmd);
                    olda.Fill(dt);
                    olda.Dispose();

                    // Loop through each ICN in listbox
                    IllustrationControlNumberFindInDatabase(dt, countOfIllustrationControlNumbers);
                }
                catch (Exception ex)
                {
                    ErrorDisplay("Error: " + ex);
                }
            }

        }
    }

    public void IllustrationControlNumberFindInDatabase(DataTable dt, int countOfIllustrationControlNumbers)
    {
        // Loop through each ICN in listbox
        for (int i = 0; i < countOfIllustrationControlNumbers; i++)
        {
            string thisIllustrationControlNumber = "illustration_control_number = '" + IllustrationControlNumbers.Items[i].Value + "'";
            DataRow[] resultsFound = dt.Select(thisIllustrationControlNumber);
            int foundRows = resultsFound.Length;
            if (foundRows > 0) // Write script to check each ICN listed
            {
                IllustrationControlNumbers.Items[i].Attributes.CssStyle.Add("background-color", validBackColor);
            }
            else
            {
                IllustrationControlNumbers.Items[i].Attributes.CssStyle.Add("background-color", warningBackColor);
            }
        }
    }

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

        DataSet destination = ReadFromDatabase("SELECT attachment_path FROM PROJECT WHERE project_index_number = " + projectNumber);
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

    protected void AjaxFileUpload_UploadCompleteAll(object sender, AjaxControlToolkit.AjaxFileUploadCompleteAllEventArgs e)
    {
        System.Diagnostics.Debug.WriteLine("AjaxFileUpload_UploadCompleteAll method fired");
        UploadedFilesPanel.Update();
    }

    private string TimeStampFileName(string fileName)
    {
        return Path.GetFileNameWithoutExtension(fileName) + " " + DateTime.Now.ToString(dateTimeFormat) + Path.GetExtension(fileName);
    }


    /****************************/
    /* COOKIE INTERFACE METHODS */
    /****************************/

    protected void CreateRequestDetailsCookie(object sender, EventArgs e)
    {
        HttpCookie createRequestCookie = new HttpCookie(requestDetailsCookie);
        createRequestCookie.Values["UserName"] = UserName.SelectedValue;
        createRequestCookie.Values["Project"] = Project.SelectedValue;
        createRequestCookie.Values["RequestType"] = RequestType.SelectedValue;
        createRequestCookie.Values["ManualType"] = ManualType.SelectedValue;
        createRequestCookie.Values["DataModuleCode"] = DataModuleCode.Text;
        createRequestCookie.Values["DataModuleTitle"] = DataModuleTitle.Text;
        createRequestCookie.Values["ConversionSource"] = ConversionSource.Text;

        createRequestCookie.Expires = DateTime.Now.AddDays(cookieExpireDays);  // Set cookie expiration date
        Response.Cookies.Add(createRequestCookie);
    }

    private void CreateAttachmentCookie(string amendedFileName, string filePath)
    {
        HttpCookie createAttachmentCookie = new HttpCookie(attachmentsCookie);
        createAttachmentCookie.Values[amendedFileName] = filePath;
        createAttachmentCookie.Expires = DateTime.Now.AddDays(cookieExpireDays);  // Reset cookie expiration date
        Response.Cookies.Add(createAttachmentCookie);
    }

    private void ClearRequestDetailsCookie()
    {
        HttpCookie createRequestCookie = new HttpCookie(requestDetailsCookie)
        {
            Expires = DateTime.Now.AddDays(-1)  // Set cookie expiration date
        };
        Response.Cookies.Add(createRequestCookie);
    }

    private void ClearAttachmentCookie()
    {
        HttpCookie createRequestCookie = new HttpCookie(attachmentsCookie)
        {
            Expires = DateTime.Now.AddDays(-1)  // Set cookie expiration date
        };
        Response.Cookies.Add(createRequestCookie);
    }

    private void PopulateRequestDetailsFromCookie()
    {
        HttpCookie myCookie = Request.Cookies[requestDetailsCookie];
        if (myCookie != null)
        {
            if (myCookie.HasKeys)
            {
                UserName.SelectedValue = Server.HtmlEncode(myCookie.Values["UserName"]);
                Project.SelectedValue = Server.HtmlEncode(myCookie.Values["Project"]);
                RequestType.SelectedValue = Server.HtmlEncode(myCookie.Values["RequestType"]);
                ManualType.SelectedValue = Server.HtmlEncode(myCookie.Values["ManualType"]);
                DataModuleCode.Text = Server.HtmlEncode(myCookie.Values["DataModuleCode"]);
                DataModuleTitle.Text = Server.HtmlEncode(myCookie.Values["DataModuleTitle"]);
                ConversionSource.Text = Server.HtmlEncode(myCookie.Values["ConversionSource"]);
            }
        }
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

}
