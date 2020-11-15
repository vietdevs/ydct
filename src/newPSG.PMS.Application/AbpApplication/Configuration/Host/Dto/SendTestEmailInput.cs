using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using newPSG.PMS.Authorization.Users;

namespace newPSG.PMS.Configuration.Host.Dto
{
    public class SendTestEmailInput
    {
        [Required]
        [MaxLength(User.MaxEmailAddressLength)]
        public string EmailAddress { get; set; }
    }
}