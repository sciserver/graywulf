<%@ Page Language="C#" Inherits="Jhu.Graywulf.Web.Admin.Domain.UserForm" MasterPageFile="~/EntityForm.master"
    CodeBehind="UserForm.aspx.cs" %>

<asp:Content runat="server" ContentPlaceHolderID="FormDetails">
    <table class="DetailsForm">
        <tr>
            <td class="FormLabel">
                <asp:Label ID="TitleLabel" runat="server" Text="Title:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="Title" runat="server" CssClass="FormField"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="FirstNameLabel" runat="server" Text="First Name:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="FirstName" runat="server" CssClass="FormField"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="MiddleNameLable" runat="server" Text="Middle Name:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="MiddleName" runat="server" CssClass="FormField"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="LastNameLabel" runat="server" Text="Last Name:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="LastName" runat="server" CssClass="FormField"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="GenderLabel" runat="server" Text="Gender:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:DropDownList ID="Gender" runat="server" CssClass="FormField">
                    <asp:ListItem Text="(select gender)" Value="Unknown" />
                    <asp:ListItem Text="Male" Value="Male" />
                    <asp:ListItem Text="Female" Value="Female" />
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="EmailLabel" runat="server" Text="E-mail Address:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="Email" runat="server" CssClass="FormField"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="DateOfBirthLabel" runat="server" Text="Date of birth:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="DateOfBirth" runat="server" CssClass="FormField"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="CompanyLabel" runat="server" Text="Company:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="Company" runat="server" CssClass="FormField"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="JobTitleLabel" runat="server" Text="Job title:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="JobTitle" runat="server" CssClass="FormField"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="AddressLabel" runat="server" Text="Address:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="Address" runat="server" CssClass="FormField"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="Address2Label" runat="server" Text="Address cont.:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="Address2" runat="server" CssClass="FormField"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="StateLabel" runat="server" Text="State:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="State" runat="server" CssClass="FormField"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="StateCodeLabel" runat="server" Text="State code:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="StateCode" runat="server" CssClass="FormField"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="CityLabel" runat="server" Text="City:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="City" runat="server" CssClass="FormField"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="CountryLabel" runat="server" Text="Country:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="Country" runat="server" CssClass="FormField"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="CountryCodeLabel" runat="server" Text="Country code:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="CountryCode" runat="server" CssClass="FormField"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="ZipCodeLabel" runat="server" Text="Zip Code:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="ZipCode" runat="server" CssClass="FormField"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="WorkPhoneLabel" runat="server" Text="Work Phone:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="WorkPhone" runat="server" CssClass="FormField"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="HomePhoneLabel1" runat="server" Text="Home Phone:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="HomePhone" runat="server" CssClass="FormField"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="CellPhoneLabel" runat="server" Text="Cell Phone:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="CellPhone" runat="server" CssClass="FormField"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="TimeZoneLabel" runat="server" Text="Time Zone:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="TimeZone" runat="server" CssClass="FormField"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="IntegratedLabel" runat="server" Text="Integrated:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:CheckBox ID="Integrated" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="NtlmUserLabel" runat="server" Text="Windows User Account:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="NtlmUser" runat="server" CssClass="FormField"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="PasswordLabel" runat="server" Text="Password:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="Password" runat="server" CssClass="FormField"></asp:TextBox>
                <asp:RequiredFieldValidator ID="PasswordRequiredValidator" runat="server" ControlToValidate="Password"
                    Display="Dynamic" ErrorMessage="Password is required"></asp:RequiredFieldValidator>
            </td>
        </tr>
    </table>
</asp:Content>
