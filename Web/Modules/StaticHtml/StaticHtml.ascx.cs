namespace Cuyahoga.Web.Modules.StaticHtml
{
	using System;
	// using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

	using Cuyahoga.Web.UI;
	using Cuyahoga.Modules.StaticHtml;

	/// <summary>
	///		Summary description for StaticHtml.
	/// </summary>
	public class StaticHtml : BaseModuleControl
	{
		protected System.Web.UI.WebControls.PlaceHolder plcContent;

		private void Page_Load(object sender, System.EventArgs e)
		{
			StaticHtmlModule module = this.Module as StaticHtmlModule;
			if (module != null)
			{
				Literal htmlControl = new Literal();
				htmlControl.Text = module.StaticHtmlContent.Content;
				this.plcContent.Controls.Add(htmlControl);
			}
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion
	}
}