using System.Web;
using System.Web.Optimization;

namespace Blog
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/scripts").Include(
                      "~/vendor/jquery/jquery.min.js",
                      "~/vendor/bootstrap/js/bootstrap.bundle.min.js",
                      "~/js/clean-blog.min.js",
                      "~/js/jqBootstrapValidation.js",
                      "~/js/contact_me.js"));

            bundles.Add(new StyleBundle("~/bundles/styles").Include(
                      "~/vendor/bootstrap/css/bootstrap.min.css",
                      "~/vendor/fontawesome-free/css/all.min.css",
                      "~/css/clean-blog.min.css"));
        }
    }
}
