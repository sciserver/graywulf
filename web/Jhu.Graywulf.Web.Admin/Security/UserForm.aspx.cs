using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Web.Admin.Security
{
    public partial class UserForm : EntityFormPageBase<Registry.User>
    {
        protected override void OnUpdateForm()
        {
            base.OnUpdateForm();

            Title.Text = item.Title;
            FirstName.Text = item.FirstName;
            MiddleName.Text = item.MiddleName;
            LastName.Text = item.LastName;
            Gender.SelectedValue = item.Gender.ToString();
            Email.Text = item.Email;
            DateOfBirth.Text = item.DateOfBirth.ToShortDateString();
            Company.Text = item.Company;
            JobTitle.Text = item.JobTitle;
            Address.Text = item.Address;
            Address.Text = item.Address2;
            State.Text = item.State;
            StateCode.Text = item.StateCode;
            City.Text = item.City;
            Country.Text = item.Country;
            CountryCode.Text = item.CountryCode;
            ZipCode.Text = item.ZipCode;
            WorkPhone.Text = item.WorkPhone;
            HomePhone.Text = item.HomePhone;
            CellPhone.Text = item.CellPhone;
            TimeZone.Text = item.TimeZone.ToString();
            Integrated.Checked = item.Integrated;
            NtlmUser.Text = item.NtlmUser;
            Password.Text = String.Empty;

            PasswordRequiredValidator.Enabled = !item.IsExisting;
        }

        protected override void OnSaveForm()
        {
            base.OnSaveForm();

            item.Title = Title.Text;
            item.FirstName = FirstName.Text;
            item.MiddleName = MiddleName.Text;
            item.LastName = LastName.Text;
            item.Gender = (Gender)Enum.Parse(typeof(Gender), Gender.SelectedValue);
            item.Email = Email.Text;
            item.DateOfBirth = DateTime.Parse(DateOfBirth.Text);
            item.Company = Company.Text;
            item.JobTitle = JobTitle.Text;
            item.Address = Address.Text;
            item.Address2 = Address2.Text;
            item.State = State.Text;
            item.StateCode = StateCode.Text;
            item.City = City.Text;
            item.Country = Country.Text;
            item.CountryCode = CountryCode.Text;
            item.ZipCode = ZipCode.Text;
            item.WorkPhone = WorkPhone.Text;
            item.HomePhone = HomePhone.Text;
            item.CellPhone = CellPhone.Text;
            item.TimeZone = int.Parse(TimeZone.Text);
            item.Integrated = Integrated.Checked;
            item.NtlmUser = NtlmUser.Text;
            if (Password.Text != String.Empty) item.SetPassword(Password.Text);
        }

    }
}