using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CRUD_project
{
	public partial class Contact : System.Web.UI.Page
	{
		SqlConnection Sqlcon = new SqlConnection(@"Data Source = localhost\SQLExpress;Initial Catalog=ASPCRUD; Integrated Security=true;");
		protected void Page_Load(object sender, EventArgs e)
		{
			if(!IsPostBack)
			{
				btnDelete.Enabled = false;
				FillGridView();

			}
		}

        protected void btnClear_Click(object sender, EventArgs e)
        {
			Clear();
        }
		public void Clear()
		{
			hfContactID.Value = "";
			txtName.Text = txtMobile.Text = txtAddress.Text = "";
			lplSuccessMessage.Text = lplErrorMessage.Text = "";
			btnSave.Enabled = true;
		}

		protected void btnSave_Click(object sender, EventArgs e)
		{
			if(Sqlcon.State == ConnectionState.Closed)
			
				Sqlcon.Open();
			
			SqlCommand sqlCmd = new SqlCommand("ContactCreateOrUpdate",Sqlcon);
			sqlCmd.CommandType = CommandType.StoredProcedure;
			sqlCmd.Parameters.AddWithValue("@ContactId",(hfContactID.Value==""?0:Convert.ToInt32(hfContactID.Value)));
			sqlCmd.Parameters.AddWithValue("@Name",txtName.Text.Trim());
			sqlCmd.Parameters.AddWithValue("@Mobile",txtMobile.Text.Trim());
			sqlCmd.Parameters.AddWithValue("@Address",txtAddress.Text.Trim());
			sqlCmd.ExecuteNonQuery();
			Sqlcon.Close();
			string contactId = hfContactID.Value;
			Clear();
			if (contactId == "")

				lplSuccessMessage.Text = "Saved Successfully";
			
			else
				lplSuccessMessage.Text = "Updated Successfully";
			
			FillGridView();
			
		}

		void FillGridView()
		{
			if(Sqlcon.State ==ConnectionState.Closed)
				Sqlcon.Open();
			SqlDataAdapter sqlDa = new SqlDataAdapter("ContactViewAll",Sqlcon);
			sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
			DataTable dtbl = new DataTable();
			sqlDa.Fill(dtbl);
			Sqlcon.Close();
			gvContact.DataSource = dtbl;
			gvContact.DataBind();
			

		}

		protected void lnk_OnClick(object sender, System.EventArgs e)
		{
			int contactId = Convert.ToInt32((sender as LinkButton).CommandArgument);
			if (Sqlcon.State == ConnectionState.Closed)
				Sqlcon.Open();
			SqlDataAdapter sqlDa = new SqlDataAdapter("ContactViewByID", Sqlcon);
			sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
			sqlDa.SelectCommand.Parameters.AddWithValue("@ContactId", contactId);
			DataTable dtbl = new DataTable();
			sqlDa.Fill(dtbl);
			Sqlcon.Close();
			hfContactID.Value = contactId.ToString();
			txtName.Text = dtbl.Rows[0]["Name"].ToString();
			txtMobile.Text = dtbl.Rows[0]["Mobile"].ToString();
			txtAddress.Text = dtbl.Rows[0]["Address"].ToString();
			btnSave.Text = "Update";
			btnDelete.Enabled = true;
		}

		protected void btnDelete_Click(object sender, EventArgs e)
		{
			if(Sqlcon.State == ConnectionState.Closed)
				Sqlcon.Open();
			SqlCommand sqlCmd = new SqlCommand("ContactDeleteByID",Sqlcon);
			sqlCmd.CommandType = CommandType.StoredProcedure;
			sqlCmd.Parameters.AddWithValue("@ContactId",Convert.ToInt32(hfContactID.Value));
			sqlCmd.ExecuteNonQuery();
			Sqlcon.Close();
			Clear();
			FillGridView();
			lplSuccessMessage.Text = "Deleted successfully";
			btnDelete.Enabled = false;
		}
	}
}