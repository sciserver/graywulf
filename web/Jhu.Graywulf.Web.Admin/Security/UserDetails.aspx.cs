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
    public partial class UserDetails : EntityDetailsPageBase<Registry.User>
    {
        protected override void UpdateForm()
        {
            base.UpdateForm();

            Title.Text = item.Title;
            FirstName.Text = item.FirstName;
            MiddleName.Text = item.MiddleName;
            LastName.Text = item.LastName;
            Gender.Text = item.Gender.ToString();
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
            NtlmUser.Text = item.NtlmUser;

            AddUserGroupButton.OnClientClick = Util.UrlFormatter.GetClientRedirect(AddUserGroupMember.GetUrl(item.Guid));

            if (item.DeploymentState == DeploymentState.Deployed)
            {
                ToggleActive.Text = "Deactivate";
            }
            else
            {
                ToggleActive.Text = "Activate";
            }
        }

        protected override void InitLists()
        {
            base.InitLists();

            item.LoadUserGroups();
            UserGroupMemberList.DataSource = item.UserGroups;
        }

        private void RemoveUserGroup(Guid[] guids)
        {
            Validate();

            if (IsValid)
            {
                foreach (var g in guids)
                {
                    item.RemoveMemberOf(g);
                }

                item.LoadUserGroups();
            }
        }

        public override void OnButtonCommand(object sender, CommandEventArgs e)
        {
            var guids = UserGroupMemberList.SelectedDataKeys.Select(g => new Guid(g)).ToArray();

            switch (e.CommandName)
            {
                case "RemoveUserGroup":
                    RemoveUserGroup(guids);
                    break;
                default:
                    base.OnButtonCommand(sender, e);
                    break;
            }

        }

        protected void UserGroupSelectedValidator_ServerValidate(object sender, ServerValidateEventArgs e)
        {
            e.IsValid = UserGroupMemberList.SelectedDataKeys.Count > 0;
        }

    }
}