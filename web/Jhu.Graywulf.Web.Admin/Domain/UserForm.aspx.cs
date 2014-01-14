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

namespace Jhu.Graywulf.Web.Admin.Domain
{
    public partial class UserForm : EntityFormPageBase<Registry.User>
    {
        protected override void OnUpdateForm()
        {
            base.OnUpdateForm();

            Title.Text = Item.Title;
            FirstName.Text = Item.FirstName;
            MiddleName.Text = Item.MiddleName;
            LastName.Text = Item.LastName;
            Gender.SelectedValue = Item.Gender.ToString();
            Email.Text = Item.Email;
            DateOfBirth.Text = Item.DateOfBirth.ToShortDateString();
            Company.Text = Item.Company;
            JobTitle.Text = Item.JobTitle;
            Address.Text = Item.Address;
            Address.Text = Item.Address2;
            State.Text = Item.State;
            StateCode.Text = Item.StateCode;
            City.Text = Item.City;
            Country.Text = Item.Country;
            CountryCode.Text = Item.CountryCode;
            ZipCode.Text = Item.ZipCode;
            WorkPhone.Text = Item.WorkPhone;
            HomePhone.Text = Item.HomePhone;
            CellPhone.Text = Item.CellPhone;
            TimeZone.Text = Item.TimeZone.ToString();
            Integrated.Checked = Item.Integrated;
            NtlmUser.Text = Item.NtlmUser;
            Password.Text = String.Empty;

            PasswordRequiredValidator.Enabled = !Item.IsExisting;
        }

        protected override void OnSaveForm()
        {
            base.OnSaveForm();

            Item.Title = Title.Text;
            Item.FirstName = FirstName.Text;
            Item.MiddleName = MiddleName.Text;
            Item.LastName = LastName.Text;
            Item.Gender = (Gender)Enum.Parse(typeof(Gender), Gender.SelectedValue);
            Item.Email = Email.Text;
            Item.DateOfBirth = DateTime.Parse(DateOfBirth.Text);
            Item.Company = Company.Text;
            Item.JobTitle = JobTitle.Text;
            Item.Address = Address.Text;
            Item.Address2 = Address2.Text;
            Item.State = State.Text;
            Item.StateCode = StateCode.Text;
            Item.City = City.Text;
            Item.Country = Country.Text;
            Item.CountryCode = CountryCode.Text;
            Item.ZipCode = ZipCode.Text;
            Item.WorkPhone = WorkPhone.Text;
            Item.HomePhone = HomePhone.Text;
            Item.CellPhone = CellPhone.Text;
            Item.TimeZone = int.Parse(TimeZone.Text);
            Item.Integrated = Integrated.Checked;
            Item.NtlmUser = NtlmUser.Text;
            if (Password.Text != String.Empty) Item.SetPassword(Password.Text);
        }

    }
}