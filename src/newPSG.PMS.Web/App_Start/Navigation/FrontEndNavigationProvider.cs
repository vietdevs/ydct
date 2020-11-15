using Abp.Application.Navigation;
using Abp.Localization;

namespace newPSG.PMS.Web.Navigation
{
    /// <summary>
    /// This class defines font-end web site's menu.
    /// It uses ABP's menu system.
    /// When you add menu items here, they are automatically appear in the front-end web site.
    /// </summary>
    public class FrontEndNavigationProvider : NavigationProvider
    {
        public const string MenuName = "Frontend";

        public override void SetNavigation(INavigationProviderContext context)
        {
            var frontEndMenu = new MenuDefinition(MenuName, new FixedLocalizableString("Frontend menu"));
            context.Manager.Menus[MenuName] = frontEndMenu;

            frontEndMenu

                //HOME
                .AddItem(new MenuItemDefinition(
                    PageNames.Frontend.Home,
                    L("Trang chủ"),
                    url: ""
                    )
                //Thutuchanhchinh
                ).AddItem(new MenuItemDefinition(
                    PageNames.Frontend.Thutuchanhchinh,
                    L("Bộ thủ tục"),
                    url: "/thu-tuc-hanh-chinh"
                    )
                //Hethongbaocao
                //).AddItem(new MenuItemDefinition(
                //    PageNames.Frontend.Thongke,
                //    L("Thống kê"),
                //    url: "thong-ke"
                //    )
                //Hethongbaocao
                ).AddItem(new MenuItemDefinition(
                   PageNames.Frontend.Baocao,
                    L("Thông báo"),
                    url: "thong-bao"
                    )
                //Lienhe
                ).AddItem(new MenuItemDefinition(
                    PageNames.Frontend.Lienhe,
                    L("Liên hệ"),
                    url: "lien-he"
                    )
                );
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, PMSConsts.LocalizationSourceName);
        }
    }
}