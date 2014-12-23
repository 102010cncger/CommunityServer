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

using ASC.Core;
using ASC.Core.Caching;
using ASC.Core.Users;
using ASC.Web.Core;
using Resources;
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace ASC.Web.Studio.Core.SMS
{
    public static class SmsManager
    {
        private static readonly ICache CodeCache = AscCache.Default;

        public static string GetPhoneValueDigits(string mobilePhone)
        {
            var reg = new Regex(@"[^\d]");
            mobilePhone = reg.Replace(mobilePhone ?? "", String.Empty).Trim();
            return mobilePhone.Substring(0, Math.Min(64, mobilePhone.Length));
        }

        public static string BuildPhoneNoise(string mobilePhone)
        {
            if (String.IsNullOrEmpty(mobilePhone))
                return String.Empty;

            mobilePhone = GetPhoneValueDigits(mobilePhone);

            const int startLen = 4;
            const int endLen = 4;
            if (mobilePhone.Length < startLen + endLen)
                return mobilePhone;

            var sb = new StringBuilder();
            sb.Append("+");
            sb.Append(mobilePhone.Substring(0, startLen));
            for (var i = startLen; i < mobilePhone.Length - endLen; i++)
            {
                sb.Append("*");
            }
            sb.Append(mobilePhone.Substring(mobilePhone.Length - endLen));
            return sb.ToString();
        }

        public static string SaveMobilePhone(UserInfo user, string mobilePhone)
        {
            mobilePhone = GetPhoneValueDigits(mobilePhone);

            if (user == null || Equals(user, Constants.LostUser)) throw new Exception(Resource.ErrorUserNotFound);
            if (string.IsNullOrEmpty(mobilePhone)) throw new Exception(Resource.ActivateMobilePhoneEmptyPhoneNumber);
            if (!string.IsNullOrEmpty(user.MobilePhone) && user.MobilePhoneActivationStatus == MobilePhoneActivationStatus.Activated) throw new Exception(Resource.MobilePhoneMustErase);

            user.MobilePhone = mobilePhone;
            user.MobilePhoneActivationStatus = MobilePhoneActivationStatus.NotActivated;
            if (SecurityContext.IsAuthenticated)
            {
                CoreContext.UserManager.SaveUserInfo(user);
            }
            else
            {
                try
                {
                    SecurityContext.AuthenticateMe(ASC.Core.Configuration.Constants.CoreSystem);
                    CoreContext.UserManager.SaveUserInfo(user);
                }
                finally
                {
                    SecurityContext.Logout();
                }
            }

            if (StudioSmsNotificationSettings.Enable)
            {
                PutAuthCode(user, false);
            }

            return mobilePhone;
        }

        public static void PutAuthCode(UserInfo user, bool again)
        {
            if (user == null || Equals(user, Constants.LostUser)) throw new Exception(Resource.ErrorUserNotFound);
            var mobilePhone = GetPhoneValueDigits(user.MobilePhone);

            if (SmsKeyStorage.ExistsKey(mobilePhone) && !again) return;

            var key = SmsKeyStorage.GenerateKey(mobilePhone);
            SmsSender.SendSMS(mobilePhone, string.Format(Resource.SmsAuthenticationMessageToUser, key));
        }

        public static void ValidateSmsCode(UserInfo user, string code)
        {
            if (!StudioSmsNotificationSettings.IsVisibleSettings
                || !StudioSmsNotificationSettings.Enable)
            {
                return;
            }

            if (user == null || Equals(user, Constants.LostUser)) throw new Exception(Resource.ErrorUserNotFound);

            code = (code ?? "").Trim();

            if (string.IsNullOrEmpty(code)) throw new Exception(Resource.ActivateMobilePhoneEmptyCode);

            
            var counter = (int)(CodeCache.Get("loginsec/" + user.ID) ?? 0);
            if (++counter % 5 == 0)
            {
                Thread.Sleep(TimeSpan.FromSeconds(10));
            }
            CodeCache.Insert("loginsec/" + user.ID, counter, DateTime.UtcNow.Add(TimeSpan.FromMinutes(1)));

            if (!SmsKeyStorage.ValidateKey(user.MobilePhone, code))
                throw new ArgumentException(Resource.SmsAuthenticationMessageError);

            if (!SecurityContext.IsAuthenticated)
            {
                var cookiesKey = SecurityContext.AuthenticateMe(user.ID);
                CookiesManager.SetCookies(CookiesType.AuthKey, cookiesKey);
            }

            if (user.MobilePhoneActivationStatus == MobilePhoneActivationStatus.NotActivated)
            {
                user.MobilePhoneActivationStatus = MobilePhoneActivationStatus.Activated;
                CoreContext.UserManager.SaveUserInfo(user);
            }
        }
    }
}