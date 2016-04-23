<%@ Page Language="C#" Inherits="Jhu.Graywulf.Web.Admin.Domain.UserDetails" MasterPageFile="~/EntityChildren.master"
    CodeBehind="UserDetails.aspx.cs" %>

<asp:Content runat="server" ContentPlaceHolderID="FormDetails">
    <table class="DetailsForm">
        <tr>
            <td class="FormLabel">
                <asp:Label ID="TitleLabel" runat="server" Text="Title:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="Title" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="FirstNameLabel" runat="server" Text="First Name:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="FirstName" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="MiddleNameLable" runat="server" Text="Middle Name:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="MiddleName" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="LastNameLabel" runat="server" Text="Last Name:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="LastName" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="GenderLabel" runat="server" Text="Gender:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="Gender" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="EmailLabel" runat="server" Text="E-mail Address:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="Email" runat="server" Text="Label"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="NonValidatedEmailLabel" runat="server" Text="E-mail Address:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="NonValidatedEmail" runat="server" Text="Label"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="DateOfBirthLabel" runat="server" Text="Date of birth:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="DateOfBirth" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="CompanyLabel" runat="server" Text="Company:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="Company" runat="server" Text="Label"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="JobTitleLabel" runat="server" Text="Job title:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="JobTitle" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="AddressLabel" runat="server" Text="Address:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="Address" runat="server" Text="Label"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="Address2Label" runat="server" Text="Address cont.:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="Address2" runat="server" Text="Label"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="StateLabel" runat="server" Text="State:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="State" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="StateCodeLabel" runat="server" Text="State code:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="StateCode" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="CityLabel" runat="server" Text="City:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="City" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="CountryLabel" runat="server" Text="Country:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="Country" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="CountryCodeLabel" runat="server" Text="Country code:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="CountryCode" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="ZipCodeLabel" runat="server" Text="Zip Code:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="ZipCode" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="WorkPhoneLabel" runat="server" Text="Work Phone:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="WorkPhone" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="HomePhoneLabel1" runat="server" Text="Home Phone:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="HomePhone" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="CellPhoneLabel" runat="server" Text="Cell Phone:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="CellPhone" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="IntegratedLabel" runat="server" Text="Integrated:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="Integrated" runat="server" Text="Label"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="TimeZoneLabel" runat="server" Text="Time Zone:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="TimeZone" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="NtlmUserLabel" runat="server" Text="Windows User Account:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="NtlmUser" runat="server" Text="Label"></asp:Label>
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="FormButtons">
    <jgwac:EntityButtons runat="server" ID="EntityButtons" />
    |
    <asp:Button ID="ToggleActive" runat="server" CssClass="FormButton" OnCommand="Button_Command" CommandName="ToggleDeploymentState" />

</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="FormTabs">
    <div class="dock-top">
        <jgwc:MultiViewTabHeader ID="Tabs" runat="server" MultiViewID="MultiViewTabs" />
    </div>
    <div class="TabFrame dock-fill dock-container">
        <asp:MultiView runat="server" ID="MultiViewTabs" ActiveViewIndex="0">
            <!-- User groups -->
            <jgwc:TabView runat="server" Text="Group Membership">
                <div class="dock-top">
                    <jgwc:MultiSelectGridView runat="server" ID="UserGroupMemberList" AllowPaging="false"
                        DataKeyNames="Guid" AutoGenerateColumns="false">
                        <Columns>
                            <jgwc:SelectionField ItemStyle-CssClass="GridViewIcon" />
                            <asp:ImageField DataImageUrlField="Guid" DataImageUrlFormatString="~/Icons/Small/UserGroup.gif"
                                ItemStyle-CssClass="GridViewIcon" />
                            <jgwc:BoundField HeaderText="Name" DataField="Name" ItemStyle-CssClass="GridViewSpan" />
                        </Columns>
                    </jgwc:MultiSelectGridView>
                </div>
                <div class="dock-bottom">
                    <p class="FormMessage">
                        <asp:CustomValidator ID="UserGroupSelectedValidator" runat="server" ErrorMessage="No item was selected."
                            OnServerValidate="UserGroupSelectedValidator_ServerValidate" ValidationGroup="UserGroup"></asp:CustomValidator></p>
                    <p class="FormButtons">
                        <asp:Button ID="AddUserGroupButton" runat="server" CssClass="FormButtonNarrow" Text="Add" ValidationGroup="UserGroup" CausesValidation="false" />
                        <asp:Button ID="RemoveUserGroupButton" runat="server" CssClass="FormButtonNarrow"
                            Text="Remove" OnCommand="Button_Command" CommandName="RemoveUserGroup"  ValidationGroup="UserGroup"/>
                    </p>
                </div>
            </jgwc:TabView>
            <!-- Identities -->
            <jgwc:TabView runat="server" Text="Identities">
                <jgwac:EntityList runat="server" ID="IdentitiesList" ChildrenType="UserIdentity" EntityGroup="Domain" DataObjectTypeName="UserIdentity" />
            </jgwc:TabView>
        </asp:MultiView>
    </div>
</asp:Content>
