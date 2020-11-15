using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Authorization.Users;
using Abp.Extensions;
using Microsoft.AspNet.Identity;

namespace newPSG.PMS.Authorization.Users
{
    /// <summary>
    /// Represents a user in the system.
    /// </summary>
    public class User : AbpUser<User>
    {
        public const int MinPlainPasswordLength = 6;

        public const int MaxPhoneNumberLength = 24;

        public virtual Guid? ProfilePictureId { get; set; }

        public virtual bool ShouldChangePasswordOnNextLogin { get; set; }

        //Can add application specific user properties here
        public long? DoanhNghiepId { get; set; }
        public int? PhongBanId { get; set; }
        public int? RoleLevel { get; set; }
        public int? Stt { get; set; }
        public bool? IsDonViTrucThuoc { get; set; }
        public int? BoNganhId { get; set; }
        public long? ParentId { get; set; }
        public int? TinhId { get; set; }
        public long? HuyenId { get; set; }
        public long? XaId { get; set; }
        [StringLength(1000)]
        [Column(TypeName = "nvarchar")]
        public string ChanChuKyNhay { get; set; }
        public bool? IsChanChuKyNhay { get; set; }
        [StringLength(500)]
        [Column(TypeName = "nvarchar")]
        public string UrlImageChuKyNhay { get; set; }
        public int? TieuBanId { get; set; }
        public User()
        {
            IsLockoutEnabled = true;
            IsTwoFactorEnabled = true;
        }
        
        /// <summary>
        /// Creates admin <see cref="User"/> for a tenant.
        /// </summary>
        /// <param name="tenantId">Tenant Id</param>
        /// <param name="emailAddress">Email address</param>
        /// <param name="password">Password</param>
        /// <returns>Created <see cref="User"/> object</returns>
        public static User CreateTenantAdminUser(int tenantId, string emailAddress, string password)
        {
            return new User
                   {
                       TenantId = tenantId,
                       UserName = AdminUserName,
                       Name = AdminUserName,
                       Surname = AdminUserName,
                       EmailAddress = emailAddress,
                       Password = new PasswordHasher().HashPassword(password)
                   };
        }

        public static string CreateRandomPassword()
        {
            return Guid.NewGuid().ToString("N").Truncate(16);
        }

        public void Unlock()
        {
            AccessFailedCount = 0;
            LockoutEndDateUtc = null;
        }
    }
}