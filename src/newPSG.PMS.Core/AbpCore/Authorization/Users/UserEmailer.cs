using System;
using System.Text;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.Net.Mail;
using Abp.Runtime.Security;
using Abp.Runtime.Session;
using newPSG.PMS.Chat;
using newPSG.PMS.Emailing;
using newPSG.PMS.MultiTenancy;
using newPSG.PMS.Web;

namespace newPSG.PMS.Authorization.Users
{
    /// <summary>
    /// Used to send email to users.
    /// </summary>
    public class UserEmailer : PMSServiceBase, IUserEmailer, ITransientDependency
    {
        private readonly IEmailTemplateProvider _emailTemplateProvider;
        private readonly IEmailSender _emailSender;
        private readonly IWebUrlService _webUrlService;
        private readonly IRepository<Tenant> _tenantRepository;
        private readonly ICurrentUnitOfWorkProvider _unitOfWorkProvider;
        private readonly IAbpSession _session;

        public UserEmailer(IEmailTemplateProvider emailTemplateProvider,
            IEmailSender emailSender,
            IWebUrlService webUrlService,
            IRepository<Tenant> tenantRepository,
            ICurrentUnitOfWorkProvider unitOfWorkProvider,
            IAbpSession session)
        {
            _emailTemplateProvider = emailTemplateProvider;
            _emailSender = emailSender;
            _webUrlService = webUrlService;
            _tenantRepository = tenantRepository;
            _unitOfWorkProvider = unitOfWorkProvider;
            _session = session;
        }

        /// <summary>
        /// Send email activation link to user's email address.
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="plainPassword">
        /// Can be set to user's plain password to include it in the email.
        /// </param>
        [UnitOfWork]
        public virtual async Task SendEmailActivationLinkAsync(User user, string plainPassword = null)
        {
            if (user.EmailConfirmationCode.IsNullOrEmpty())
            {
                throw new ApplicationException("EmailConfirmationCode should be set in order to send email activation link.");
            }

            var tenancyName = GetTenancyNameOrNull(_session.TenantId);


            var link = _webUrlService.GetSiteRootAddress(tenancyName) + "Account/EmailConfirmation" +
                       "?userId=" + Uri.EscapeDataString(SimpleStringCipher.Instance.Encrypt(user.Id.ToString())) +
                       "&tenantId=" + (user.TenantId == null ? "" : Uri.EscapeDataString(SimpleStringCipher.Instance.Encrypt(user.TenantId.Value.ToString()))) +
                       "&confirmationCode=" + Uri.EscapeDataString(user.EmailConfirmationCode);

            var emailTemplate = new StringBuilder(_emailTemplateProvider.GetDefaultTemplate(user.TenantId));
            emailTemplate.Replace("{EMAIL_TITLE}", L("EmailActivation_Title"));
            emailTemplate.Replace("{EMAIL_SUB_TITLE}", L("EmailActivation_SubTitle"));

            var mailMessage = new StringBuilder();

            mailMessage.AppendLine("<b>" + L("NameSurname") + "</b>: " + user.Surname + " " + user.Name + "<br />");
            
            mailMessage.AppendLine("<b>" + L("UserName") + "</b>: " + user.UserName + "<br />");

            if (!plainPassword.IsNullOrEmpty())
            {
                mailMessage.AppendLine("<b>" + L("Password") + "</b>: " + plainPassword + "<br />");
            }

            mailMessage.AppendLine("<br />");
            mailMessage.AppendLine(L("EmailActivation_ClickTheLinkBelowToVerifyYourEmail") + "<br /><br />");
            mailMessage.AppendLine("<a href=\"" + link + "\">" + link + "</a>");

            emailTemplate.Replace("{EMAIL_BODY}", mailMessage.ToString());

            await _emailSender.SendAsync(user.EmailAddress, L("EmailActivation_Subject"), emailTemplate.ToString());
        }
        
        /// <summary>
        /// Sends a password reset link to user's email.
        /// </summary>
        /// <param name="user">User</param>
        public async Task SendPasswordResetLinkAsync(User user)
        {
            if (user.PasswordResetCode.IsNullOrEmpty())
            {
                throw new ApplicationException("PasswordResetCode should be set in order to send password reset link.");
            }

            var tenancyName = GetTenancyNameOrNull(_session.TenantId);

            var link = _webUrlService.GetSiteRootAddress(tenancyName) + "Account/ResetPassword" +
                       "?userId=" + Uri.EscapeDataString(SimpleStringCipher.Instance.Encrypt(user.Id.ToString())) +
                       "&tenantId=" + (user.TenantId == null ? "" : Uri.EscapeDataString(SimpleStringCipher.Instance.Encrypt(user.TenantId.Value.ToString()))) +
                       "&resetCode=" + Uri.EscapeDataString(user.PasswordResetCode);

            var emailTemplate = new StringBuilder(_emailTemplateProvider.GetDefaultTemplate(user.TenantId));
            emailTemplate.Replace("{EMAIL_TITLE}", L("PasswordResetEmail_Title"));
            emailTemplate.Replace("{EMAIL_SUB_TITLE}", L("PasswordResetEmail_SubTitle"));

            var mailMessage = new StringBuilder();

            mailMessage.AppendLine("<b>" + L("NameSurname") + "</b>: " + user.Name + " " + user.Surname + "<br />");

            //if (!tenancyName.IsNullOrEmpty())
            //{
            //    mailMessage.AppendLine("<b>" + L("TenancyName") + "</b>: " + tenancyName + "<br />");
            //}

            mailMessage.AppendLine("<b>" + L("UserName") + "</b>: " + user.UserName + "<br />");

            mailMessage.AppendLine("<br />");
            mailMessage.AppendLine(L("PasswordResetEmail_ClickTheLinkBelowToResetYourPassword") + "<br /><br />");
            mailMessage.AppendLine("<a href=\"" + link + "\">" + link + "</a>");

            emailTemplate.Replace("{EMAIL_BODY}", mailMessage.ToString());

            await _emailSender.SendAsync(user.EmailAddress, L("PasswordResetEmail_Subject"), emailTemplate.ToString());
        }

        public void TryToSendChatMessageMail(User user, string senderUsername, string senderTenancyName, ChatMessage chatMessage)
        {
            try
            {
                var emailTemplate = new StringBuilder(_emailTemplateProvider.GetDefaultTemplate(user.TenantId));
                emailTemplate.Replace("{EMAIL_TITLE}", L("NewChatMessageEmail_Title"));
                emailTemplate.Replace("{EMAIL_SUB_TITLE}", L("NewChatMessageEmail_SubTitle"));

                var mailMessage = new StringBuilder();
                mailMessage.AppendLine("<b>" + L("Sender") + "</b>: " + senderTenancyName + "/" + senderUsername + "<br />");
                mailMessage.AppendLine("<b>" + L("Time") + "</b>: " + chatMessage.CreationTime.ToString("yyyy-MM-dd HH:mm:ss") + "<br />");
                mailMessage.AppendLine("<b>" + L("Message") + "</b>: " + chatMessage.Message + "<br />");
                mailMessage.AppendLine("<br />");

                emailTemplate.Replace("{EMAIL_BODY}", mailMessage.ToString());

                _emailSender.Send(user.EmailAddress, L("NewChatMessageEmail_Subject"), emailTemplate.ToString());
            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message, exception);
            }
        }

        private string GetTenancyNameOrNull(int? tenantId)
        {
            if (tenantId == null)
            {
                return null;
            }

            using (_unitOfWorkProvider.Current.SetTenantId(null))
            {
                return _tenantRepository.Get(tenantId.Value).TenancyName;
            }
        }


        #region Custom 

        /// <summary>
        /// Send email to confirm resgister HoanTD.
        /// </summary>
        /// <param name="user">User</param>
        /// </param>
        [UnitOfWork]
        public virtual async Task SendEmailConfirmRegisterAsync(User user)
        {

            var tenancyName = GetTenancyNameOrNull(_session.TenantId);

            var emailTemplate = new StringBuilder(_emailTemplateProvider.GetDefaultTemplate(user.TenantId));
            emailTemplate.Replace("{EMAIL_TITLE}", L("EmailConfirmRegister_Title"));
            emailTemplate.Replace("{EMAIL_SUB_TITLE}", L("EmailConfirmRegister_SubTitle"));

            var mailMessage = new StringBuilder();
            mailMessage.AppendLine("<b>" + L("NameSurname") + "</b>: " + user.Surname + " " + user.Name + "<br />");
            mailMessage.AppendLine("<b>" + L("UserName") + "</b>: " + user.UserName + "<br />");
            mailMessage.AppendLine("<br />");
            mailMessage.AppendLine(L("EmailActivation_WaitForEmailComfirmation") + "<br /><br />");

            emailTemplate.Replace("{EMAIL_BODY}", mailMessage.ToString());

            await _emailSender.SendAsync(user.EmailAddress, L("EmailConfirmRegister_Subject"), emailTemplate.ToString());
        }

        /// <summary>
        /// Send email to confirm resgister HoanTD.
        /// </summary>
        /// <param name="user">User</param>
        /// </param>
        [UnitOfWork]
        public virtual async Task SendEmailDoNotExceptRegisterAsync(User user, string lyDoKhongDuyet)
        {

            var tenancyName = GetTenancyNameOrNull(_session.TenantId);

            var emailTemplate = new StringBuilder(_emailTemplateProvider.GetDefaultTemplate(user.TenantId));
            emailTemplate.Replace("{EMAIL_TITLE}", L("EmailDoNotExcept_Title"));
            emailTemplate.Replace("{EMAIL_SUB_TITLE}", L("EmailDoNotExcept_SubTitle"));

            var mailMessage = new StringBuilder();
            mailMessage.AppendLine("<b>" + L("NameSurname") + "</b>: " + user.Surname + " " + user.Name + "<br />");
            mailMessage.AppendLine("<b>" + L("UserName") + "</b>: " + user.UserName + "<br />");
            mailMessage.AppendLine("<br />");
            mailMessage.AppendLine(L("EmailDoNotExcept_Content") + "<br /><br />");
            mailMessage.AppendLine(lyDoKhongDuyet + "<br /><br />");
            mailMessage.AppendLine(L("EmailDoNotExcept_Final") + "<br /><br />");
            emailTemplate.Replace("{EMAIL_BODY}", mailMessage.ToString());

            await _emailSender.SendAsync(user.EmailAddress, L("EmailDoNotExcept_Subject"), emailTemplate.ToString());
        }

        /// <summary>
        /// Send email deactive user account.
        /// </summary>
        /// <param name="user">User</param>
        /// </param>
        [UnitOfWork]
        public virtual async Task SendEmailDeactiveUserAccountAsync(User user)
        {

            var tenancyName = GetTenancyNameOrNull(_session.TenantId);

            var emailTemplate = new StringBuilder(_emailTemplateProvider.GetDefaultTemplate(user.TenantId));
            emailTemplate.Replace("{EMAIL_TITLE}", L("EmailDeactiveUserAccount_Title"));
            emailTemplate.Replace("{EMAIL_SUB_TITLE}", L("EmailDeactiveUserAccount_SubTitle"));

            var mailMessage = new StringBuilder();
            mailMessage.AppendLine("<b>" + L("NameSurname") + "</b>: " + user.Surname + " " + user.Name + "<br />");
            mailMessage.AppendLine("<b>" + L("UserName") + "</b>: " + user.UserName + "<br />");
            mailMessage.AppendLine("<br />");
            mailMessage.AppendLine(L("EmailDeactiveUserAccount_Message") + "<br /><br />");

            emailTemplate.Replace("{EMAIL_BODY}", mailMessage.ToString());

            await _emailSender.SendAsync(user.EmailAddress, L("EmailDeactiveUserAccount_Subject"), emailTemplate.ToString());
        }

        /// <summary>
        /// Send email deactive user account.
        /// </summary>
        /// <param name="user">User</param>
        /// </param>
        [UnitOfWork]
        public virtual async Task SendEmailActiveUserAccountAsync(User user)
        {
            var tenancyName = GetTenancyNameOrNull(_session.TenantId);

            var emailTemplate = new StringBuilder(_emailTemplateProvider.GetDefaultTemplate(user.TenantId));
            emailTemplate.Replace("{EMAIL_TITLE}", L("EmailActiveUserAccount_Title"));
            emailTemplate.Replace("{EMAIL_SUB_TITLE}", L("EmailActiveUserAccount_SubTitle"));

            var link = _webUrlService.GetSiteRootAddress(tenancyName) + "Account/Login";

            var mailMessage = new StringBuilder();
            mailMessage.AppendLine("<b>" + L("NameSurname") + "</b>: " + user.Surname + " " + user.Name + "<br />");
            mailMessage.AppendLine("<b>" + L("UserName") + "</b>: " + user.UserName + "<br />");
            mailMessage.AppendLine("<br />");
            mailMessage.AppendLine(L("EmailActiveUserAccount_Message") + "<br /><br />");
            mailMessage.AppendLine("<a href=\"" + link + "\">" + link + "</a>");
            emailTemplate.Replace("{EMAIL_BODY}", mailMessage.ToString());

            await _emailSender.SendAsync(user.EmailAddress, L("EmailActiveUserAccount_Subject"), emailTemplate.ToString());
        }
        #endregion
    }
}