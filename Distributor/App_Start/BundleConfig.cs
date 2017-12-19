using System.Web;
using System.Web.Optimization;

namespace Distributor
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery-ui.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryextra").Include(
                        "~/Content/vendor/jquery/jquery.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquerydynatable").Include(
                        "~/Scripts/jquery.dynatable.js"));

            bundles.Add(new ScriptBundle("~/bundles/tables").Include(
                        "~/Content/vendor/datatables/js/jquery.dataTables.min.js",
                      "~/Content/vendor/datatables-plugins/dataTables.bootstrap.min.js",
                      "~/Content/vendor/datatables-responsive/dataTables.responsive.js"));

            bundles.Add(new ScriptBundle("~/bundles/table").Include(
                        "~/Content/vendor/datatables/js/jquery.dataTables.min.js",
                      "~/Content/vendor/datatables-plugins/dataTables.bootstrap.min.js",
                      "~/Content/vendor/datatables-responsive/dataTables.responsive.js"));

            bundles.Add(new ScriptBundle("~/bundles/full").Include(
                      "~/Scripts/jquery-{version}.js",
                      "~/Scripts/jquery-ui.min.js",
                      "~/Scripts/jquery.validate*",
                      "~/Content/vendor/jquery/jquery.min.js",
                      "~/Scripts/jquery.dynatable.js",
                      "~/Scripts/bootstrap.min.js",
                      "~/Scripts/respond.min.js",
                      "~/Content/vendor/metisMenu/metisMenu.min.js",
                      "~/Content/vendor/datatables/js/jquery.dataTables.min.js",
                      "~/Content/vendor/datatables-plugins/dataTables.bootstrap.min.js",
                      "~/Content/vendor/datatables-responsive/dataTables.responsive.js",
                      "~/Content/vendor/dist/js/sb-admin-2.js"));

            bundles.Add(new ScriptBundle("~/bundles/reduced").Include(
                      "~/Content/vendor/jquery/jquery.min.js",
                      "~/Scripts/bootstrap.min.js",
                      "~/Scripts/respond.min.js",
                      "~/Content/vendor/metisMenu/metisMenu.min.js",
                      "~/Content/vendor/datatables/js/jquery.dataTables.min.js",
                      "~/Content/vendor/datatables-plugins/dataTables.bootstrap.min.js",
                      "~/Content/vendor/datatables-responsive/dataTables.responsive.js",
                      "~/Content/vendor/dist/js/sb-admin-2.js"));

            bundles.Add(new ScriptBundle("~/bundles/standard").Include(
                      "~/Content/vendor/metisMenu/metisMenu.min.js",
                      "~/Content/vendor/datatables/js/jquery.dataTables.min.js",
                      "~/Content/vendor/datatables-plugins/dataTables.bootstrap.min.js",
                      "~/Content/vendor/datatables-responsive/dataTables.responsive.js",
                      "~/Content/vendor/dist/js/sb-admin-2.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.min.js",
                      "~/Scripts/respond.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/extras").Include(
                      "~/Content/vendor/metisMenu/metisMenu.min.js",
                      "~/Content/dist/js/sb-admin-2.js"));

            bundles.Add(new ScriptBundle("~/bundles/vendor").Include(
                      "~/Content/vendor/jquery/jquery.min.js",
                      "~/Scripts/bootstrap.min.js",
                      "~/Content/vendor/metisMenu/metisMenu.min.js",
                      "~/Content/vendor/datatables/js/jquery.dataTables.min.js",
                      "~/Content/vendor/datatables-plugins/dataTables.bootstrap.min.js",
                      "~/Content/vendor/datatables-responsive/dataTables.responsive.js",
                      "~/Content/vendor/dist/js/sb-admin-2.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.min.css",
                      "~/Content/site.css",
                      "~/Content/jquery-ui.min.css",
                      "~/Content/vendor/metisMenu/metisMenu.min.css",
                      "~/Content/vendor/morrisjs/morris.css",
                      "~/Content/vendor/datatables-plugins/dataTables.bootstrap.css",
                      "~/Content/vendor/datatables-responsive/dataTables.responsive.css",
                      "~/Content/vendor/dist/css/distributor.css",
                      "~/Content/vendor/font-awesome/css/font-awesome.min.css"));
        }
    }
}
