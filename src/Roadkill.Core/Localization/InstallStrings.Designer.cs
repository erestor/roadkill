﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Roadkill.Core.Localization {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class InstallStrings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal InstallStrings() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Roadkill.Core.Localization.InstallStrings", typeof(InstallStrings).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Back.
        /// </summary>
        public static string Shared_Back {
            get {
                return ResourceManager.GetString("Shared_Back", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Back.
        /// </summary>
        public static string Shared_BackButton {
            get {
                return ResourceManager.GetString("Shared_BackButton", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Continue.
        /// </summary>
        public static string Shared_Continue {
            get {
                return ResourceManager.GetString("Shared_Continue", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Next.
        /// </summary>
        public static string Shared_NextButton {
            get {
                return ResourceManager.GetString("Shared_NextButton", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Welcome.
        /// </summary>
        public static string Shared_Stage1 {
            get {
                return ResourceManager.GetString("Shared_Stage1", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Database.
        /// </summary>
        public static string Shared_Stage2 {
            get {
                return ResourceManager.GetString("Shared_Stage2", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Security.
        /// </summary>
        public static string Shared_Stage3 {
            get {
                return ResourceManager.GetString("Shared_Stage3", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Extras.
        /// </summary>
        public static string Shared_Stage4 {
            get {
                return ResourceManager.GetString("Shared_Stage4", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Finished.
        /// </summary>
        public static string Shared_Stage5 {
            get {
                return ResourceManager.GetString("Shared_Stage5", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Thank for you downloading Roadkill .NET Wiki engine. The installer writes the settings you enter here to the web.config file (and also the database). .
        /// </summary>
        public static string Step1_Intro {
            get {
                return ResourceManager.GetString("Step1_Intro", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to write to the web.config file..
        /// </summary>
        public static string Step1_WebConfig_Failure_Message {
            get {
                return ResourceManager.GetString("Step1_WebConfig_Failure_Message", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to In order to proceed, first we need to test if you have write access to the web.config file. .
        /// </summary>
        public static string Step1_WebConfig_Intro {
            get {
                return ResourceManager.GetString("Step1_WebConfig_Intro", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The web.config can be written to..
        /// </summary>
        public static string Step1_WebConfig_Success_Message {
            get {
                return ResourceManager.GetString("Step1_WebConfig_Success_Message", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Note: you will need to create the database first..
        /// </summary>
        public static string Step2_ConnectionString_CreateDatabase {
            get {
                return ResourceManager.GetString("Step2_ConnectionString_CreateDatabase", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Example connection strings.
        /// </summary>
        public static string Step2_Example_ConnectionStrings {
            get {
                return ResourceManager.GetString("Step2_Example_ConnectionStrings", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Below are some required settings for Roadkill to work correctly. You can change these later via the settings page as an admin..
        /// </summary>
        public static string Step2_Intro {
            get {
                return ResourceManager.GetString("Step2_Intro", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failure - try copying the SQLite.Interop.dll file from ~/App_Data/SQLiteBinaries to your bin folder..
        /// </summary>
        public static string Step2_SQLiteFailureMessage {
            get {
                return ResourceManager.GetString("Step2_SQLiteFailureMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SQLite file copy failed.
        /// </summary>
        public static string Step2_SQLiteFailureTitle {
            get {
                return ResourceManager.GetString("Step2_SQLiteFailureTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Note: to use SQLite you need to add another file to the /bin folder, as SQLite is distributed for both x86 and x64. {BEGIN-SQLITE_LINK}Click here to copy the file{END-SQLITE_LINK}. If you experience any issues after this, delete &quot;SQLite.Interop.dll&quot; from your bin folder..
        /// </summary>
        public static string Step2_SQLiteInstructionsMessage {
            get {
                return ResourceManager.GetString("Step2_SQLiteInstructionsMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SQLite: please read.
        /// </summary>
        public static string Step2_SQLiteInstructionsTitle {
            get {
                return ResourceManager.GetString("Step2_SQLiteInstructionsTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SQLite should now be ready to use..
        /// </summary>
        public static string Step2_SQLiteSuccessMessage {
            get {
                return ResourceManager.GetString("Step2_SQLiteSuccessMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SQLite file copy success.
        /// </summary>
        public static string Step2_SQLiteSuccessTitle {
            get {
                return ResourceManager.GetString("Step2_SQLiteSuccessTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The connection string supports OU specifiers however doing it this way can cause issues. If you are unsure about this setting, it&apos;s recommended you download {BEGIN_ADEXPLORER_LINK}AD Explorer{END_ADEXPLORER_LINK} first..
        /// </summary>
        public static string Step3_AD_ADExplorer_Details {
            get {
                return ResourceManager.GetString("Step3_AD_ADExplorer_Details", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This is the security group that users should belong to in order to be admins. Admins have the same rights as editors, but also have access to configuration settings and tools for the site.The test button uses the connection settings entered above..
        /// </summary>
        public static string Step3_AD_Admin_GroupName_Details {
            get {
                return ResourceManager.GetString("Step3_AD_Admin_GroupName_Details", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Admin group name.
        /// </summary>
        public static string Step3_AD_Admin_GroupName_Title {
            get {
                return ResourceManager.GetString("Step3_AD_Admin_GroupName_Title", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to connect to the Active Directory server:.
        /// </summary>
        public static string Step3_AD_Connection_Error {
            get {
                return ResourceManager.GetString("Step3_AD_Connection_Error", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The connection and authentication with the Active Directory server was successful..
        /// </summary>
        public static string Step3_AD_Connection_Success {
            get {
                return ResourceManager.GetString("Step3_AD_Connection_Success", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This should be in the format LDAP://mydc01.company.internal (it should be uppercase LDAP) where mydc01 is your domain controller and &apos;company&apos; and &apos;internal&apos; are the dns suffixes for your network. This can also be an IP address. You can usually get this information by running &quot;ipconfig&quot;..
        /// </summary>
        public static string Step3_AD_ConnectionString_Details {
            get {
                return ResourceManager.GetString("Step3_AD_ConnectionString_Details", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Active Directory LDAP connection string.
        /// </summary>
        public static string Step3_AD_ConnectionString_Title {
            get {
                return ResourceManager.GetString("Step3_AD_ConnectionString_Title", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This is the security group that users should belong to in order to be editors (people who can edit and create pages). The test button uses the connection settings entered above. .
        /// </summary>
        public static string Step3_AD_Editor_GroupName_Details {
            get {
                return ResourceManager.GetString("Step3_AD_Editor_GroupName_Details", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Editor group name.
        /// </summary>
        public static string Step3_AD_Editor_GroupName_Title {
            get {
                return ResourceManager.GetString("Step3_AD_Editor_GroupName_Title", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to query:.
        /// </summary>
        public static string Step3_AD_Error {
            get {
                return ResourceManager.GetString("Step3_AD_Error", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The group was queried sucessfully..
        /// </summary>
        public static string Step3_AD_Success {
            get {
                return ResourceManager.GetString("Step3_AD_Success", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Enter a username and password if the user that the app pool is running under does not have read access to Active Directory. This is useful if your Active Directory domain is a separate forest from the main domain (i.e. another branch office)..
        /// </summary>
        public static string Step3_AD_Username_Password_Details {
            get {
                return ResourceManager.GetString("Step3_AD_Username_Password_Details", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This is the email address for the admin user. You can add more admins in the settings page, so this user does not &apos;own&apos; the site.
        /// </summary>
        public static string Step3_DB_Admin_Email {
            get {
                return ResourceManager.GetString("Step3_DB_Admin_Email", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Admin email.
        /// </summary>
        public static string Step3_DB_Admin_Email_Title {
            get {
                return ResourceManager.GetString("Step3_DB_Admin_Email_Title", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This is the password for the admin user..
        /// </summary>
        public static string Step3_DB_Admin_Password {
            get {
                return ResourceManager.GetString("Step3_DB_Admin_Password", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Admin password.
        /// </summary>
        public static string Step3_DB_Admin_Password_Title {
            get {
                return ResourceManager.GetString("Step3_DB_Admin_Password_Title", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This is the name of the admin role. Admins have the same rights as editors, but also have access to configuration settings and tools for the site..
        /// </summary>
        public static string Step3_DB_Admin_Role {
            get {
                return ResourceManager.GetString("Step3_DB_Admin_Role", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Admin role name.
        /// </summary>
        public static string Step3_DB_Admin_Role_Title {
            get {
                return ResourceManager.GetString("Step3_DB_Admin_Role_Title", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Check this option if you want to allow people to signup as a new users. If left unchecked, only admins will be able to create new users. .
        /// </summary>
        public static string Step3_DB_Allow_User_Signups {
            get {
                return ResourceManager.GetString("Step3_DB_Allow_User_Signups", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Allow new user signups. .
        /// </summary>
        public static string Step3_DB_Allow_User_Signups_Label {
            get {
                return ResourceManager.GetString("Step3_DB_Allow_User_Signups_Label", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Allow user signups.
        /// </summary>
        public static string Step3_DB_Allow_User_Signups_Title {
            get {
                return ResourceManager.GetString("Step3_DB_Allow_User_Signups_Title", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This is the name of the editor role - people who can edit and create pages..
        /// </summary>
        public static string Step3_DB_Editor_Role {
            get {
                return ResourceManager.GetString("Step3_DB_Editor_Role", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Editor role name.
        /// </summary>
        public static string Step3_DB_Editor_Role_Title {
            get {
                return ResourceManager.GetString("Step3_DB_Editor_Role_Title", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to If you are allowing user signups, it&apos;s recommended that you also enable Recaptcha support. This is a free anti-spam bot service provided by Google, you will need to signup and copy your public and private key into the fields below. {BEGIN_RECAPTCHA_LINK}Signup here{END_RECAPTCHA_LINK} - you will need to create a google account first..
        /// </summary>
        public static string Step3_DB_Enable_Recaptcha_Description {
            get {
                return ResourceManager.GetString("Step3_DB_Enable_Recaptcha_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to All passwords are one-way encrypted using SHA128 and a salt. This means passwords are not recoverable and will require an admin to reset them (there is no reset password functionality yet). The minimum password length is 6 characters..
        /// </summary>
        public static string Step3_One_Way_Hashed {
            get {
                return ResourceManager.GetString("Step3_One_Way_Hashed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Do you want to users to be authenticated against an Active Directory using Windows authentication?.
        /// </summary>
        public static string Step3_Use_WindowsAuth_Intro {
            get {
                return ResourceManager.GetString("Step3_Use_WindowsAuth_Intro", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Almost done...Finally some extra tweakable settings. .
        /// </summary>
        public static string Step4_Almost_Done {
            get {
                return ResourceManager.GetString("Step4_Almost_Done", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This is the directory where all files are uploaded to. This directory will need write permission by the user that the app pool runs under. Most of the time you shouldn&apos;t have to worry about changing the permissions for the folder, as they will be inherited from the parent folder. .
        /// </summary>
        public static string Step4_Attachments_Folder_Description {
            get {
                return ResourceManager.GetString("Step4_Attachments_Folder_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The folder path should start with &quot;~/&quot; for the website root, or be a file path (e.g. c:\myattachments)..
        /// </summary>
        public static string Step4_Attachments_Folder_RelativePath {
            get {
                return ResourceManager.GetString("Step4_Attachments_Folder_RelativePath", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Continue to the site.
        /// </summary>
        public static string Step5_Continue_Button {
            get {
                return ResourceManager.GetString("Step5_Continue_Button", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Please go back through the installation steps and check your settings. The error message was:.
        /// </summary>
        public static string Step5_Failure_GoBack_And_Check {
            get {
                return ResourceManager.GetString("Step5_Failure_GoBack_And_Check", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An error occurred during the install.
        /// </summary>
        public static string Step5_Failure_Title {
            get {
                return ResourceManager.GetString("Step5_Failure_Title", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Installation successful.
        /// </summary>
        public static string Step5_Success_Title {
            get {
                return ResourceManager.GetString("Step5_Success_Title", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to If you are using IIS7+ you may need to configure the web.config file for your security setup, below are the elements that are required.If you receive 500 errors after making these changes, then the section is locked at a parent level, and you will need to make the changes via the IIS manager..
        /// </summary>
        public static string Step5_WindowsAuth_IIS_Settings {
            get {
                return ResourceManager.GetString("Step5_WindowsAuth_IIS_Settings", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Windows Authentication - please read.
        /// </summary>
        public static string Step5_WindowsAuth_Please_Read {
            get {
                return ResourceManager.GetString("Step5_WindowsAuth_Please_Read", resourceCulture);
            }
        }
    }
}
