/*
 * 
 * (c) Copyright Ascensio System SIA 2010-2014
 * 
 * This program is a free software product.
 * You can redistribute it and/or modify it under the terms of the GNU Affero General Public License
 * (AGPL) version 3 as published by the Free Software Foundation. 
 * In accordance with Section 7(a) of the GNU AGPL its Section 15 shall be amended to the effect 
 * that Ascensio System SIA expressly excludes the warranty of non-infringement of any third-party rights.
 * 
 * This program is distributed WITHOUT ANY WARRANTY; 
 * without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. 
 * For details, see the GNU AGPL at: http://www.gnu.org/licenses/agpl-3.0.html
 * 
 * You can contact Ascensio System SIA at Lubanas st. 125a-25, Riga, Latvia, EU, LV-1021.
 * 
 * The interactive user interfaces in modified source and object code versions of the Program 
 * must display Appropriate Legal Notices, as required under Section 5 of the GNU AGPL version 3.
 * 
 * Pursuant to Section 7(b) of the License you must retain the original Product logo when distributing the program. 
 * Pursuant to Section 7(e) we decline to grant you any rights under trademark law for use of our trademarks.
 * 
 * All the Product's GUI elements, including illustrations and icon sets, as well as technical 
 * writing content are licensed under the terms of the Creative Commons Attribution-ShareAlike 4.0 International. 
 * See the License terms at http://creativecommons.org/licenses/by-sa/4.0/legalcode
 * 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using ASC.Api.Attributes;
using ASC.Api.Collections;
using ASC.CRM.Core;
using ASC.MessagingSystem;
using ASC.Web.CRM.Classes;
using ASC.Web.Core.Utility.Settings;
using ASC.Web.Studio.Utility;
using System.Security;
using ASC.Common.Threading.Progress;
using ASC.Web.Core.Utility;

namespace ASC.Api.CRM
{
    public partial class CRMApi
    {
        /// <summary>
        ///     Returns the list of all currencies currently available on the portal
        /// </summary>
        /// <short>Get currency list</short> 
        /// <category>Common</category>
        /// <returns>
        ///    List of available currencies
        /// </returns>
        [Read(@"settings/currency")]
        public IEnumerable<CurrencyInfoWrapper> GetAvaliableCurrency()
        {
            return CurrencyProvider.GetAll().ConvertAll(item => new CurrencyInfoWrapper(item)).ToItemList();
        }

        /// <summary>
        ///     Returns the result of convertation from one currency to another
        /// </summary>
        /// <param name="amount">Amount to convert</param>
        /// <param name="fromcurrency">Old currency key</param>
        /// <param name="tocurrency">New currency key</param>
        /// <short>Get the result of convertation</short> 
        /// <category>Common</category>
        /// <returns>
        ///    Decimal result of convertation
        /// </returns>
        [Read(@"settings/currency/convert")]
        public Decimal ConvertAmount(Decimal amount, String fromcurrency, String tocurrency)
        {
            return CurrencyProvider.MoneyConvert(amount, fromcurrency, tocurrency);
        }

        /// <summary>
        ///     Returns the summary table with rates for selected currency
        /// </summary>
        /// <param name="currency" remark="Allowed values: EUR, RUB etc. You can get the whole list of available currencies by api">Currency (Abbreviation)</param>
        /// <short>Get the summary table</short> 
        /// <category>Common</category>
        /// <returns>
        ///    Dictionary of currencies and rates
        /// </returns>
        /// <exception cref="ArgumentException"></exception>
        [Read(@"settings/currency/summarytable")]
        public IEnumerable<CurrencyRateInfoWrapper> GetSummaryTable(String currency)
        {
            var result = new List<CurrencyRateInfoWrapper>();

            if (string.IsNullOrEmpty(currency))
            {
                throw new ArgumentException();
            }

            var cur = CurrencyProvider.Get(currency.ToUpper());
            if (cur == null) throw new ArgumentException();

            var table = CurrencyProvider.MoneyConvert(cur);
            table.ToList().ForEach(tableItem => result.Add(ToCurrencyRateInfoWrapper(tableItem.Key, tableItem.Value)));
            return result;
        }

        /// <summary>
        ///     
        /// </summary>
        /// <param name="changeContactStatusGroupAuto" remark="true, false or null">Change contact status group auto</param>
        /// <short></short> 
        /// <category>Contacts</category>
        /// <returns>
        ///    ChangeContactStatusGroupAuto setting value (true, false or null)
        /// </returns>
        /// <exception cref="SecurityException"></exception>
        [Update(@"contact/status/settings")]
        public Boolean? UpdateCRMContactStatusSettings(Boolean? changeContactStatusGroupAuto)
        {
            var tenantSettings = Global.TenantSettings;
            tenantSettings.ChangeContactStatusGroupAuto = changeContactStatusGroupAuto;

            SettingsManager.Instance.SaveSettings(tenantSettings, TenantProvider.CurrentTenantID);
            MessageService.Send(Request, MessageAction.ContactTemperatureLevelSettingsUpdated);

            return changeContactStatusGroupAuto;
        }

        /// <summary>
        ///     
        /// </summary>
        /// <param name="writeMailToHistoryAuto" remark="true or false">Write mail to history auto</param>
        /// <short></short> 
        /// <category>Contacts</category>
        /// <returns>
        ///    WriteMailToHistoryAuto setting value (true or false)
        /// </returns>
        /// <exception cref="SecurityException"></exception>
        [Update(@"contact/mailtohistory/settings")]
        public Boolean UpdateCRMWriteMailToHistorySettings(Boolean writeMailToHistoryAuto)
        {
            var tenantSettings = Global.TenantSettings;
            tenantSettings.WriteMailToHistoryAuto = writeMailToHistoryAuto;

            SettingsManager.Instance.SaveSettings(tenantSettings, TenantProvider.CurrentTenantID);
            //MessageService.Send(Request, MessageAction.ContactTemperatureLevelSettingsUpdated);

            return writeMailToHistoryAuto;
        }

        /// <summary>
        ///     
        /// </summary>
        /// <param name="addTagToContactGroupAuto" remark="true, false or null">add tag to contact group auto</param>
        /// <short></short> 
        /// <category>Contacts</category>
        /// <returns>
        ///    AddTagToContactGroupAuto setting value (true, false or null)
        /// </returns>
        /// <exception cref="SecurityException"></exception>
        [Update(@"contact/tag/settings")]
        public Boolean? UpdateCRMContactTagSettings(Boolean? addTagToContactGroupAuto)
        {
            var tenantSettings = Global.TenantSettings;
            tenantSettings.AddTagToContactGroupAuto = addTagToContactGroupAuto;

            SettingsManager.Instance.SaveSettings(tenantSettings, TenantProvider.CurrentTenantID);
            MessageService.Send(Request, MessageAction.ContactsTagSettingsUpdated);

            return addTagToContactGroupAuto;
        }


        /// <summary>
        ///    Set IsConfiguredPortal tenant setting and website contact form key specified in the request
        /// </summary>
        /// <short>Set tenant settings</short> 
        /// <category>Common</category>
        /// <returns>
        ///    IsConfiguredPortal setting value (true or false)
        /// </returns>
        [Update(@"settings")]
        public Boolean SetIsPortalConfigured(Boolean? configured, Guid? webFormKey)
        {
            if (!CRMSecurity.IsAdmin) throw CRMSecurity.CreateSecurityException();
            var tenantSettings = Global.TenantSettings;
            tenantSettings.IsConfiguredPortal = configured ?? true;
            tenantSettings.WebFormKey = webFormKey ?? Guid.NewGuid();
            SettingsManager.Instance.SaveSettings(tenantSettings, TenantProvider.CurrentTenantID);
            return tenantSettings.IsConfiguredPortal;
        }

        /// <summary>
        ///  Save organisation company name
        /// </summary>
        /// <param name="companyName">Organisation company name</param>
        /// <short>Save organisation company name</short>
        /// <category>Organisation</category>
        /// <returns>Organisation company name</returns>
        /// <exception cref="SecurityException"></exception>
        [Update(@"settings/organisation/base")]
        public String UpdateOrganisationSettingsCompanyName(String companyName)
        {
            if (!CRMSecurity.IsAdmin) throw CRMSecurity.CreateSecurityException();
            var tenantSettings = Global.TenantSettings;
            if (tenantSettings.InvoiceSetting == null)
            {
                tenantSettings.InvoiceSetting = InvoiceSetting.DefaultSettings;
            }
            tenantSettings.InvoiceSetting.CompanyName = companyName;

            SettingsManager.Instance.SaveSettings(tenantSettings, TenantProvider.CurrentTenantID);
            MessageService.Send(Request, MessageAction.OrganizationProfileUpdatedCompanyName, companyName);

            return companyName;
        }

        /// <summary>
        ///  Save organisation company address
        /// </summary>
        /// <param name="companyAddress">Organisation company address</param>
        /// <short>Save organisation company address</short>
        /// <category>Organisation</category>
        /// <returns>Organisation company address</returns>
        /// <exception cref="SecurityException"></exception>
        [Update(@"settings/organisation/address")]
        public String UpdateOrganisationSettingsCompanyAddress(String companyAddress)
        {
            if (!CRMSecurity.IsAdmin) throw CRMSecurity.CreateSecurityException();
            var tenantSettings = Global.TenantSettings;
            if (tenantSettings.InvoiceSetting == null)
            {
                tenantSettings.InvoiceSetting = InvoiceSetting.DefaultSettings;
            }
            tenantSettings.InvoiceSetting.CompanyAddress = companyAddress;

            SettingsManager.Instance.SaveSettings(tenantSettings, TenantProvider.CurrentTenantID);
            MessageService.Send(Request, MessageAction.OrganizationProfileUpdatedAddress);

            return companyAddress;
        }

        /// <summary>
        ///  Save organisation logo
        /// </summary>
        /// <param name="reset">Reset organisation logo</param>
        /// <short>Save organisation logo</short>
        /// <category>Organisation</category>
        /// <returns>Organisation logo ID</returns>
        /// <exception cref="SecurityException"></exception>
        /// <exception cref="Exception"></exception>
        [Update(@"settings/organisation/logo")]
        public Int32 UpdateOrganisationSettingsLogo(bool reset)
        {
            if (!CRMSecurity.IsAdmin) throw CRMSecurity.CreateSecurityException();
            int companyLogoID;
            if (!reset)
            {
                companyLogoID = OrganisationLogoManager.TryUploadOrganisationLogoFromTmp();
                if (companyLogoID == 0)
                {
                    throw new Exception("Downloaded image not found");
                }
            }
            else
            {
                companyLogoID = 0;
            }

            var tenantSettings = Global.TenantSettings;
            if (tenantSettings.InvoiceSetting == null)
            {
                tenantSettings.InvoiceSetting = InvoiceSetting.DefaultSettings;
            }
            tenantSettings.InvoiceSetting.CompanyLogoID = companyLogoID;

            SettingsManager.Instance.SaveSettings(tenantSettings, TenantProvider.CurrentTenantID);
            MessageService.Send(Request, MessageAction.OrganizationProfileUpdatedInvoiceLogo);

            return companyLogoID;
        }

        /// <summary>
        ///  Get organisation logo in base64 format  (if 'id' is 0 then take current logo)
        /// </summary>
        /// <param name="id">organisation logo id</param>
        /// <short>Get organisation logo</short>
        /// <category>Organisation</category>
        /// <returns>Organisation logo content in base64</returns>
        /// <exception cref="Exception"></exception>
        [Read(@"settings/organisation/logo")]
        public String GetOrganisationSettingsLogo(int id)
        {
            if (id != 0)
            {
                return OrganisationLogoManager.GetOrganisationLogoBase64(id);
            }
            else
            {
                var tenantSettings = Global.TenantSettings;
                if (tenantSettings.InvoiceSetting == null)
                {
                    return string.Empty;
                }

                return OrganisationLogoManager.GetOrganisationLogoBase64(tenantSettings.InvoiceSetting.CompanyLogoID);
            }
        }

        /// <summary>
        ///  Change Website Contact Form key
        /// </summary>
        /// <short>Change web form key</short>
        /// <category>Common</category>
        /// <returns>Web form key</returns>
        /// <exception cref="SecurityException"></exception>
        [Update(@"settings/webformkey/change")]
        public string ChangeWebToLeadFormKey()
        {
            if (!CRMSecurity.IsAdmin) throw CRMSecurity.CreateSecurityException();

            var tenantSettings = Global.TenantSettings;
            tenantSettings.WebFormKey = Guid.NewGuid();

            SettingsManager.Instance.SaveSettings(tenantSettings, TenantProvider.CurrentTenantID);
            MessageService.Send(Request, MessageAction.WebsiteContactFormUpdatedKey);

            return tenantSettings.WebFormKey.ToString();
        }

        /// <summary>
        ///  Change default CRM currency
        /// </summary>
        /// <param name="currency" remark="Allowed values: EUR, RUB etc. You can get the whole list of available currencies by api">Currency (Abbreviation)</param>
        /// <short>Change currency</short>
        /// <category>Common</category>
        /// <returns>currency</returns>
        /// <exception cref="SecurityException"></exception>
        /// <exception cref="ArgumentException"></exception>
        [Update(@"settings/currency")]
        public CurrencyInfoWrapper UpdateCRMCurrency(String currency)
        {
            if (!CRMSecurity.IsAdmin) throw CRMSecurity.CreateSecurityException();

            if (string.IsNullOrEmpty(currency))
            {
                throw new ArgumentException();
            }
            currency = currency.ToUpper();
            var cur = CurrencyProvider.Get(currency);
            if (cur == null) throw new ArgumentException();

            Global.SaveDefaultCurrencySettings(cur);
            MessageService.Send(Request, MessageAction.CrmDefaultCurrencyUpdated);

            return ToCurrencyInfoWrapper(cur);
        }

        /// <summary>
        ///  Save SMTP Server Settings
        /// </summary>
        /// <short>Save SMTP Settings</short>
        /// <param name="host">Host name</param>
        /// <param name="port">Port</param>
        /// <param name="authentication">Need authentication</param>
        /// <param name="hostLogin">Host Login</param>
        /// <param name="hostPassword">Host Password</param>
        /// <param name="senderDisplayName">Sender Name</param>
        /// <param name="senderEmailAddress">Sender Email Address</param>
        /// <param name="enableSSL">Enable SSL</param>
        /// <category>Common</category>
        /// <returns>SMTP Server Settings</returns>
        /// <exception cref="SecurityException"></exception>
        [Update(@"settings/smtp")]
        public SMTPServerSetting SaveSMTPSettings(string host, int port, bool authentication, string hostLogin, string hostPassword, string senderDisplayName, string senderEmailAddress, bool enableSSL)
        {
            if (!CRMSecurity.IsAdmin) throw CRMSecurity.CreateSecurityException();

            Global.SaveSMTPSettings(host, port, authentication, hostLogin, hostPassword, senderDisplayName, senderEmailAddress, enableSSL);
            MessageService.Send(Request, MessageAction.CrmSmtpSettingsUpdated);

            var crmSettings = Global.TenantSettings;

            return crmSettings.SMTPServerSetting;
        }

        /// <visible>false</visible>
        [Create(@"settings/testmail")]
        public string SendTestMailSMTP(string toEmail, string mailSubj, string mailBody)
        {
            if (!CRMSecurity.IsAdmin) throw CRMSecurity.CreateSecurityException();

            if (string.IsNullOrEmpty(toEmail) || string.IsNullOrEmpty(mailBody)) throw new ArgumentException();

            MailSender.StartSendTestMail(toEmail, mailSubj, mailBody);
            MessageService.Send(Request, MessageAction.CrmTestMailSent);
            return "";
        }

        /// <visible>false</visible>
        [Create(@"{entityType:(contact|opportunity|case|task)}/import/start")]
        public string StartImportFromCSV(string entityType, string csvFileURI, string jsonSettings)
        {
            EntityType entityTypeObj;

            if (string.IsNullOrEmpty(entityType) || string.IsNullOrEmpty(csvFileURI) || string.IsNullOrEmpty(jsonSettings)) throw new ArgumentException();
            switch (entityType.ToLower())
            {
                case "contact":
                    entityTypeObj = EntityType.Contact;
                    break;
                case "opportunity":
                    entityTypeObj = EntityType.Opportunity;
                    break;
                case "case":
                    entityTypeObj = EntityType.Case;
                    break;
                case "task":
                    entityTypeObj = EntityType.Task;
                    break;
                default:
                    throw new ArgumentException();
            }

            new ImportFromCSVManager().StartImport(entityTypeObj, csvFileURI, jsonSettings);
            return "";
        }

        /// <visible>false</visible>
        [Read(@"{entityType:(contact|opportunity|case|task)}/import/status")]
        public IProgressItem GetStatus(string entityType)
        {
            EntityType entityTypeObj;

            if (string.IsNullOrEmpty(entityType)) throw new ArgumentException();
            switch (entityType.ToLower())
            {
                case "contact":
                    entityTypeObj = EntityType.Contact;
                    break;
                case "opportunity":
                    entityTypeObj = EntityType.Opportunity;
                    break;
                case "case":
                    entityTypeObj = EntityType.Case;
                    break;
                case "task":
                    entityTypeObj = EntityType.Task;
                    break;
                default:
                    throw new ArgumentException();
            }

            return ImportFromCSV.GetStatus(entityTypeObj);
        }

        /// <visible>false</visible>
        [Read(@"import/samplerow")]
        public String GetSampleRow(string csvFileURI, int indexRow, string jsonSettings)
        {
            if (String.IsNullOrEmpty(csvFileURI) || indexRow < 0) throw new ArgumentException();

            if (!Global.GetStore().IsFile("temp", csvFileURI)) throw new ArgumentException();

            var CSVFileStream = Global.GetStore().GetReadStream("temp", csvFileURI);

            return ImportFromCSV.GetRow(CSVFileStream, indexRow, jsonSettings);
        }

        /// <visible>false</visible>
        [Create(@"import/uploadfake")]
        public FileUploadResult ProcessUploadFake(string csvFileURI, string jsonSettings)
        {
            return new ImportFromCSVManager().ProcessUploadFake(csvFileURI, jsonSettings);
        }





        protected CurrencyInfoWrapper ToCurrencyInfoWrapper(CurrencyInfo currencyInfo)
        {
            return currencyInfo == null ? null : new CurrencyInfoWrapper(currencyInfo);
        }

        protected CurrencyRateInfoWrapper ToCurrencyRateInfoWrapper(CurrencyInfo currencyInfo, Decimal rate)
        {
            return currencyInfo == null ? null : new CurrencyRateInfoWrapper(currencyInfo, rate);
        }
    }
}