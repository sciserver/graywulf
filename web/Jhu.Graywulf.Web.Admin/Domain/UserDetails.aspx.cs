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
    public partial class UserDetails : EntityDetailsPageBase<Registry.User>
    {
        protected override void UpdateForm()
        {
            base.UpdateForm();

            Title.Text = Item.Title;
            FirstName.Text = Item.FirstName;
            MiddleName.Text = Item.MiddleName;
            LastName.Text = Item.LastName;
            Gender.Text = Item.Gender.ToString();
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
            NtlmUser.Text = Item.NtlmUser;

            AddUserGroupButton.OnClientClick = Util.UrlFormatter.GetClientRedirect(AddUserGroupMember.GetUrl(Item.Guid));
            AddUserRoleButton.OnClientClick = Util.UrlFormatter.GetClientRedirect(AddUserRoleMember.GetUrl(Item.Guid));

            if (Item.DeploymentState == DeploymentState.Deployed)
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

            Item.LoadUserGroupMemberships(true);
            UserGroupMemberList.DataSource = Item.UserGroupMemberships.Values;

            Item.LoadUserRoleMemberships(true);
            UserRoleMemberList.DataSource = Item.UserRoleMemberships.Values;

            Item.LoadUserIdentities(true);
            IdentitiesList.ParentEntity = Item;
        }

        private void RemoveUserGroup(Guid[] guids)
        {
            Validate();

            if (IsValid)
            {
                foreach (var g in guids)
                {
                    Item.RemoveFromGroup(g);
                }
            }

            InitLists();
        }

        private void RemoveUserRole(Guid[] guids)
        {
            Validate();

            if (IsValid)
            {
                foreach (var g in guids)
                {
                    Item.RemoveFromRole(g);
                }
            }

            InitLists();
        }

        public override void OnButtonCommand(object sender, CommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "ToggleDeploymentState":
                    if (Item.IsActivated)
                    {
                        Item.Deactivate();
                    }
                    else
                    {
                        Item.Activate();
                    }
                    Item.Save();
                    UpdateForm();
                    break;
                case "RemoveUserGroup":
                    RemoveUserGroup(UserGroupMemberList.SelectedDataKeys.Select(g => new Guid(g)).ToArray());
                    break;
                case "RemoveUserRole":
                    RemoveUserRole(UserRoleMemberList.SelectedDataKeys.Select(g => new Guid(g)).ToArray());
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

        protected void UserRoleSelectedValidator_ServerValidate(object sender, ServerValidateEventArgs e)
        {
            e.IsValid = UserRoleMemberList.SelectedDataKeys.Count > 0;
        }
    }
}