using newPSG.PMS.Web.Bundling;
using System.Web.Optimization;

namespace newPSG.PMS.Web.App.Startup
{
    public static class AppBundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            //LIBRARIES

            AddAppCssLibs(bundles, isRTL: false);
            AddAppCssLibs(bundles, isRTL: true);

            bundles.Add(
                new ScriptBundle("~/Bundles/App/libs/js")
                    .Include(
                        ScriptPaths.Json2,
                        ScriptPaths.JQuery,
                        ScriptPaths.JQuery_Migrate,
                        ScriptPaths.Bootstrap,
                        ScriptPaths.Bootstrap_Hover_Dropdown,
                        ScriptPaths.JQuery_Slimscroll,
                        ScriptPaths.Bootstrap_Summernote,
                        ScriptPaths.JQuery_BlockUi,
                        ScriptPaths.Js_Cookie,
                        ScriptPaths.JQuery_Uniform,
                        ScriptPaths.SignalR,
                        ScriptPaths.LocalForage,
                        ScriptPaths.Morris,
                        ScriptPaths.Morris_Raphael,
                        ScriptPaths.JQuery_Sparkline,
                        ScriptPaths.JQuery_Color,
                        ScriptPaths.JQuery_Jcrop,
                        ScriptPaths.JQuery_Timeago,
                        ScriptPaths.JsTree,
                        ScriptPaths.Kendo,
                        ScriptPaths.Bootstrap_Switch,
                        ScriptPaths.SpinJs,
                        ScriptPaths.SpinJs_JQuery,
                        ScriptPaths.SweetAlert,
                        ScriptPaths.PushJs,
                        ScriptPaths.Toastr,
                        ScriptPaths.MomentJs,
                        ScriptPaths.MomentTimezoneJs,
                        ScriptPaths.Bootstrap_DateRangePicker,
                        ScriptPaths.Bootstrap_Select,
                               ScriptPaths.Bootstrap_Fileinput,
                        ScriptPaths.Underscore,
                        ScriptPaths.Angular,
                        ScriptPaths.Angular_Animate,
                        ScriptPaths.Angular_Aria,
                        ScriptPaths.Angular_Messages,
                        ScriptPaths.Angular_Sanitize,
                        ScriptPaths.Angular_Touch,
                        ScriptPaths.Angular_Ui_Router,
                        ScriptPaths.Angular_Ui_Utils,
                        ScriptPaths.Angular_Ui_Bootstrap_Tpls,
                        ScriptPaths.Angular_Ui_Grid,
                        ScriptPaths.Angular_OcLazyLoad,
                        ScriptPaths.Angular_File_Upload,
                        ScriptPaths.Angular_DateRangePicker,
                        ScriptPaths.Angular_Moment,
                        ScriptPaths.Angular_Bootstrap_Switch,
                        ScriptPaths.Abp,
                        ScriptPaths.Abp_JQuery,
                        ScriptPaths.Abp_Toastr,
                        ScriptPaths.Abp_BlockUi,
                        ScriptPaths.Abp_SpinJs,
                        ScriptPaths.Abp_SweetAlert,
                        ScriptPaths.Abp_Moment,
                        ScriptPaths.Abp_Angular,
                        ScriptPaths.Angular_Kendo,
                        ScriptPaths.FormValidation,
                        ScriptPaths.FormValidation_BootstrapExtend,
                        ScriptPaths.Angular_Bootstrap_MultiSelect,
                        ScriptPaths.Angular_DropDown_MultiSelect,
                        ScriptPaths.Angular_Content_Editable,
                        ScriptPaths.Pdf_Js,
                        ScriptPaths.Angular_Pdf_Viewer,
                        ScriptPaths.Angular_Linqjs,
                        ScriptPaths.Angular_Input_Masks_StandAlone,
                        ScriptPaths.Angular_Bootstrap_Summernote,
                        ScriptPaths.Angular_Material
                    ).ForceOrdered()
                );

            //METRONIC

            AddAppMetronicCss(bundles, isRTL: false);
            AddAppMetronicCss(bundles, isRTL: true);

            bundles.Add(
              new ScriptBundle("~/Bundles/App/metronic/js")
                  .Include(
                      "~/metronic/assets/global/scripts/app.js",
                      "~/metronic/assets/admin/layout4/scripts/layout.js",
                      //"~/metronic/assets/admin/layout5/scripts/layout.js",
                      "~/metronic/assets/layouts/global/scripts/quick-sidebar.js"
                  ).ForceOrdered()
              );

            //APPLICATION

            bundles.Add(
                new StyleBundle("~/Bundles/App/css")
                    .IncludeDirectory("~/App", "*.css", true)
                    .ForceOrdered()
                );

            bundles.Add(
                new ScriptBundle("~/Bundles/App/js")
                    .IncludeDirectory("~/App", "*.js", true)
                    .ForceOrdered()
                );

            //Viettel CA JS

            bundles.Add(new ScriptBundle("~/bundles/uploadfile").Include(
                        "~/Asset/upload-file/jquery.uploadfile.min.js",
                        "~/Asset/upload-file/jquery.form.js"));

            bundles.Add(new StyleBundle("~/Content/uploadfile").Include(
                      "~/Asset/upload-file/uploadfile.css"));

            bundles.Add(new ScriptBundle("~/bundles/pluginCAViettel").Include(
                      "~/Asset/js/base64.js",
                      "~/Asset/js/ViettelCAPlugin.js",
                      "~/Asset/js/ViettelCAPluginSocket.js"));

            bundles.Add(new ScriptBundle("~/bundles/pluginCAFull").Include(
                     "~/Asset/js/CAPlugin.js",
                     "~/Asset/js/vt-connector.js"));
            bundles.Add(new ScriptBundle("~/bundles/ajax").Include(
                 "~/Asset/js/ajax/ajaxtags.js",
                 "~/Asset/js/ajax/ajaxtags_controls.js",
                 "~/Asset/js/ajax/ajaxtags_parser.js",
                 "~/Asset/js/ajax/Ajax.js"));
            bundles.Add(new ScriptBundle("~/bundles/jquerydownload").Include(
                       "~/Asset/js/jquery.fileDownload.js"));
        }

        private static void AddAppCssLibs(BundleCollection bundles, bool isRTL)
        {
            bundles.Add(
                new StyleBundle("~/Bundles/App/libs/css" + (isRTL ? "RTL" : ""))
                 .Include(StylePaths.FontRoboto, new CssRewriteUrlWithVirtualDirectoryTransform())
                  .Include(StylePaths.RobotoCondensed, new CssRewriteUrlWithVirtualDirectoryTransform())
                    .Include(StylePaths.FontAwesome, new CssRewriteUrlWithVirtualDirectoryTransform())
                    .Include(StylePaths.Simple_Line_Icons, new CssRewriteUrlWithVirtualDirectoryTransform())
                    .Include(StylePaths.FamFamFamFlags, new CssRewriteUrlWithVirtualDirectoryTransform())
                    .Include(isRTL ? StylePaths.BootstrapRTL : StylePaths.Bootstrap, new CssRewriteUrlWithVirtualDirectoryTransform())
                    .Include(StylePaths.JQuery_Uniform, new CssRewriteUrlWithVirtualDirectoryTransform())
                    .Include(StylePaths.Morris)
                    .Include(StylePaths.JsTree, new CssRewriteUrlWithVirtualDirectoryTransform())
                    .Include(StylePaths.Toastr)
                    .Include(StylePaths.Angular_Ui_Grid, new CssRewriteUrlWithVirtualDirectoryTransform())
                    .Include(StylePaths.Bootstrap_DateRangePicker)
                    .Include(StylePaths.Bootstrap_Select)
                    .Include(StylePaths.Bootstrap_Switch)
                     .Include(StylePaths.Bootstrap_Fileinput)
                    .Include(StylePaths.JQuery_Jcrop)
                    .Include(StylePaths.Bootstrap_Summernote)
                     .Include(StylePaths.Kendo_Common, new CssRewriteUrlWithVirtualDirectoryTransform())
                    .Include(StylePaths.Kendo_Common_Bootstrap, new CssRewriteUrlWithVirtualDirectoryTransform())
                    .Include(StylePaths.Kendo_Bootstrap, new CssRewriteUrlWithVirtualDirectoryTransform())
                    .Include(StylePaths.Kendo_Custom, new CssRewriteUrlWithVirtualDirectoryTransform())
                    .Include(StylePaths.FormValidation)
                    .Include(StylePaths.Bootstrap_MultiSelect)
                    .Include(StylePaths.Pdf_Viewer)
                    .Include(StylePaths.Angular_Material)
                    .ForceOrdered()
                );
        }

        private static void AddAppMetronicCss(BundleCollection bundles, bool isRTL)
        {
            bundles.Add(
                new StyleBundle("~/Bundles/App/metronic/css" + (isRTL ? "RTL" : ""))
                    .Include("~/metronic/assets/global/css/components-md" + (isRTL ? "-rtl" : "") + ".min.css", new CssRewriteUrlWithVirtualDirectoryTransform())
                    .Include("~/metronic/assets/global/css/plugins-md" + (isRTL ? "-rtl" : "") + ".min.css", new CssRewriteUrlWithVirtualDirectoryTransform())
                    .Include("~/metronic/assets/admin/layout4/css/layout" + (isRTL ? "-rtl" : "") + ".min.css", new CssRewriteUrlWithVirtualDirectoryTransform())
                    //.Include("~/metronic/assets/admin/layout5/css/layout" + (isRTL ? "-rtl" : "") + ".min.css", new CssRewriteUrlWithVirtualDirectoryTransform())
                    .Include("~/metronic/assets/admin/layout4/css/themes/light" + (isRTL ? "-rtl" : "") + ".min.css", new CssRewriteUrlWithVirtualDirectoryTransform())
                    .Include("~/metronic/assets/admin/layout/css/style.css", new CssRewriteUrlWithVirtualDirectoryTransform())
                    .Include("~/metronic/assets/admin/layout/css/custom.min.css", new CssRewriteUrlWithVirtualDirectoryTransform())
                    .ForceOrdered()
                );
        }
    }
}