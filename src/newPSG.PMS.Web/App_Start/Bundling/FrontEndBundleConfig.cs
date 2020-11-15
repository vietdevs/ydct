using System.Web.Optimization;

namespace newPSG.PMS.Web.Bundling
{
    public static class FrontEndBundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            //FRONTEND
            bundles.Add(
               new StyleBundle("~/Bundles/FrontendChicuc/libs/css")
                //.Include(StylePaths.AFontawesome)
                //.Include(StylePaths.AFlation)
                .Include(StylePaths.ABootstrap)
                .Include(StylePaths.AAnimate)
                //.Include(StylePaths.ACarousel)
                //.Include(StylePaths.ACarouselTheme)
                //.Include(StylePaths.ACarouselTransition)
                .Include(StylePaths.AManificPopup)
                .Include(StylePaths.AMeanMenu)
                .Include(StylePaths.AMain)
                .Include(StylePaths.AResponsive)
                .ForceOrdered()
               );
            bundles.Add(
                new ScriptBundle("~/Bundles/FrontendChicucModernizr/libs/js")
                    .Include(
                        ScriptPaths.AModernizr
                    ).ForceOrdered()
                );
            bundles.Add(
                new ScriptBundle("~/Bundles/FrontendChicuc/libs/js")
                    .Include(
                        

                        ScriptPaths.Json2,
                        ScriptPaths.JQuery,
                        ScriptPaths.JQuery_Migrate,
                        ScriptPaths.Bootstrap,
                        ScriptPaths.Bootstrap_Hover_Dropdown,
                        ScriptPaths.JQuery_Slimscroll_Min,
                        ScriptPaths.JQuery_Slimscroll,
                        ScriptPaths.JQuery_BlockUi,
                        ScriptPaths.Js_Cookie,
                        ScriptPaths.SpinJs,
                        ScriptPaths.SpinJs_JQuery,
                        ScriptPaths.SweetAlert,
                        ScriptPaths.Toastr,
                        ScriptPaths.MomentJs,
                        ScriptPaths.MomentTimezoneJs,

                        ScriptPaths.Bootstrap_DateRangePicker,
                               ScriptPaths.Bootstrap_Fileinput,
                        ScriptPaths.Kendo,

                        ScriptPaths.Abp,
                        ScriptPaths.Abp_JQuery,
                        ScriptPaths.Abp_Toastr,
                        ScriptPaths.Abp_BlockUi,
                        ScriptPaths.Abp_SpinJs,
                        ScriptPaths.Abp_SweetAlert,
                        ScriptPaths.Abp_Moment,

                        ScriptPaths.Angular,
                        ScriptPaths.Angular_Ui_Bootstrap_Tpls,
                        ScriptPaths.Angular_DateRangePicker,
                        ScriptPaths.Angular_Kendo,
                        ScriptPaths.Angular_File_Upload,
                        ScriptPaths.Angular_Sanitize

                        

                    ).ForceOrdered()
                );

            //LIBRARIES

            AddFrontendCssLibs(bundles, false);
            AddFrontendCssLibs(bundles, true);

            bundles.Add(
                new ScriptBundle("~/Bundles/Frontend/libs/js")
                    .Include(
                        ScriptPaths.Json2,
                        ScriptPaths.JQuery,
                        ScriptPaths.JQuery_Migrate,
                        ScriptPaths.Bootstrap,
                        ScriptPaths.Bootstrap_Hover_Dropdown,
                        ScriptPaths.JQuery_Slimscroll_Min,
                        ScriptPaths.JQuery_Slimscroll,
                        ScriptPaths.JQuery_BlockUi,
                        ScriptPaths.Js_Cookie,
                        ScriptPaths.SpinJs,
                        ScriptPaths.SpinJs_JQuery,
                        ScriptPaths.SweetAlert,
                        ScriptPaths.Toastr,
                        ScriptPaths.MomentJs,
                        ScriptPaths.MomentTimezoneJs,
                        ScriptPaths.Abp,
                        ScriptPaths.Abp_JQuery,
                        ScriptPaths.Abp_Toastr,
                        ScriptPaths.Abp_BlockUi,
                        ScriptPaths.Abp_SpinJs,
                        ScriptPaths.Abp_SweetAlert,
                        ScriptPaths.Abp_Moment
                    ).ForceOrdered()
                );

            //METRONIC

            AddFrontendCssMetronic(bundles, false);
            AddFrontendCssMetronic(bundles, true);

            bundles.Add(
                new ScriptBundle("~/Bundles/Frontend/metronic/js")
                    .Include(
                        "~/metronic/assets/frontend/layout/scripts/back-to-top.js",
                        "~/metronic/assets/frontend/layout/scripts/layout.js"
                    ).ForceOrdered()
                );

            //Customs
            bundles.Add(
                new ScriptBundle("~/Bundles/LayoutFE/libs/js")
                    .Include(
                        ScriptPaths.Json2,
                        ScriptPaths.JQuery,
                        ScriptPaths.JQuery_Migrate,
                        ScriptPaths.Bootstrap,
                        ScriptPaths.Bootstrap_Hover_Dropdown,
                        ScriptPaths.JQuery_Slimscroll,
                        ScriptPaths.JQuery_BlockUi,
                        ScriptPaths.Js_Cookie,
                        ScriptPaths.SpinJs,
                        ScriptPaths.SpinJs_JQuery,
                        ScriptPaths.SweetAlert,
                        ScriptPaths.Toastr,
                        ScriptPaths.MomentJs,
                        ScriptPaths.MomentTimezoneJs,

                        ScriptPaths.Bootstrap_DateRangePicker,
                               ScriptPaths.Bootstrap_Fileinput,
                        ScriptPaths.Kendo,

                        ScriptPaths.Abp,
                        ScriptPaths.Abp_JQuery,
                        ScriptPaths.Abp_Toastr,
                        ScriptPaths.Abp_BlockUi,
                        ScriptPaths.Abp_SpinJs,
                        ScriptPaths.Abp_SweetAlert,
                        ScriptPaths.Abp_Moment,

                        ScriptPaths.Angular,
                        ScriptPaths.Angular_Ui_Bootstrap_Tpls,
                        ScriptPaths.Angular_DateRangePicker,
                        ScriptPaths.Angular_Kendo,
                        ScriptPaths.Angular_File_Upload
                    ).ForceOrdered()
                );
        }

        private static void AddFrontendCssLibs(BundleCollection bundles, bool isRTL)
        {
            bundles.Add(
                new StyleBundle("~/Bundles/Frontend/libs/css" + (isRTL ? "RTL" : ""))
                    .Include(StylePaths.Simple_Line_Icons, new CssRewriteUrlWithVirtualDirectoryTransform())
                    .Include(StylePaths.FontAwesome, new CssRewriteUrlWithVirtualDirectoryTransform())
                    .Include(StylePaths.FamFamFamFlags, new CssRewriteUrlWithVirtualDirectoryTransform())
                    .Include(isRTL ? StylePaths.BootstrapRTL : StylePaths.Bootstrap, new CssRewriteUrlWithVirtualDirectoryTransform())
                    .Include(StylePaths.Toastr)
                    //Custom
                    .Include(StylePaths.Bootstrap_DateRangePicker)
                                  .Include(StylePaths.Bootstrap_Fileinput)
                    .Include(StylePaths.Kendo_Common, new CssRewriteUrlWithVirtualDirectoryTransform())
                    .Include(StylePaths.Kendo_Common_Bootstrap, new CssRewriteUrlWithVirtualDirectoryTransform())
                    .Include(StylePaths.Kendo_Bootstrap, new CssRewriteUrlWithVirtualDirectoryTransform())
                    .Include(StylePaths.Kendo_Custom, new CssRewriteUrlWithVirtualDirectoryTransform())
                    .ForceOrdered()
                );
        }

        private static void AddFrontendCssMetronic(BundleCollection bundles, bool isRTL)
        {
            bundles.Add(
                new StyleBundle("~/Bundles/Frontend/metronic/css" + (isRTL ? "RTL" : ""))
                    .Include("~/metronic/assets/global/css/components" + (isRTL ? "-rtl" : "") + ".css", new CssRewriteUrlWithVirtualDirectoryTransform())
                    .Include("~/metronic/assets/frontend/layout/css/style" + (isRTL ? "-rtl" : "") + ".css", new CssRewriteUrlWithVirtualDirectoryTransform())
                    .Include("~/metronic/assets/frontend/pages/css/style-revolution-slider.css", new CssRewriteUrlWithVirtualDirectoryTransform())
                    .Include("~/metronic/assets/frontend/layout/css/style-responsive" + (isRTL ? "-rtl" : "") + ".css", new CssRewriteUrlWithVirtualDirectoryTransform())
                    .Include("~/metronic/assets/frontend/layout/css/themes/turquoise" + (isRTL ? "-rtl" : "") + ".css", new CssRewriteUrlWithVirtualDirectoryTransform())
                    .Include("~/metronic/assets/frontend/layout/css/custom.min.css", new CssRewriteUrlWithVirtualDirectoryTransform())
                    .ForceOrdered()
                );
        }
    }
}