using System.Web;
using System.Web.Optimization;

namespace HomeToWork_API
{
    public class BundleConfig
    {
        // Per altre informazioni sulla creazione di bundle, vedere https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Assets/js/core/jquery.3.2.1.min.js"));

            // Utilizzare la versione di sviluppo di Modernizr per eseguire attività di sviluppo e formazione. Successivamente, quando si è
            // pronti per passare alla produzione, usare lo strumento di compilazione disponibile all'indirizzo https://modernizr.com per selezionare solo i test necessari.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                "~/Assets/js/core/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                "~/Assets/js/core/bootstrap.min.js",
                "~/Assets/js/plugins/bootstrap-notify.js",
                "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/charts").Include(
                "~/Assets/js/plugins/chartist.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/dashboard-scripts").Include(
                "~/Assets/js/light-bootstrap-dashboard.js?v=1.4.0",
                "~/Assets/js/demo.js"));

            bundles.Add(new StyleBundle("~/Content/bootstrap").Include(
                "~/Assets/css/bootstrap.min.css"));

            bundles.Add(new StyleBundle("~/Content/style").Include(
                "~/Assets/css/style.css"));

            bundles.Add(new StyleBundle("~/Content/dashboard-css").Include(
                "~/Assets/css/light-bootstrap-dashboard.css?v=2.0.1",
                "~/Assets/css/demo.css",
                "~/Assets/css/style-dashboard.css"));
        }
    }
}