using Microsoft.AspNetCore.Mvc;
using DatabaseComparer.Models;
using DatabaseComparer.Services;

namespace DatabaseComparer.Controllers
{
    public class KarsilastirmaController : Controller
    {
        private readonly ConnectionService _connectionService;
        private readonly SchemaService _schemaService;
        private readonly CompareService _compareService;

        public KarsilastirmaController(ConnectionService connectionService, SchemaService schemaService, CompareService compareService)
        {
            _connectionService = connectionService;
            _schemaService = schemaService;
            _compareService = compareService;
        }

        [HttpGet]
        public IActionResult Baglanti()
        {
            return View(new ConnectionModel());
        }

        [HttpPost]
        public IActionResult TestEt([FromForm] ConnectionModel model, [FromForm] string taraf)
        {
            string server = (taraf == "source" ? model.SourceServer : model.DestinationServer) ?? string.Empty;
            string database = (taraf == "source" ? model.SourceDatabase : model.DestinationDatabase) ?? string.Empty;
            string authType = (taraf == "source" ? model.SourceAuthType : model.DestinationAuthType) ?? "Windows";
            string? username = taraf == "source" ? model.SourceUsername : model.DestinationUsername;
            string? password = taraf == "source" ? model.SourcePassword : model.DestinationPassword;

            string digerTaraf = taraf == "source" ? "destination" : "source";
            if (TempData.Peek($"{digerTaraf}Databases") is string digerDbs && !string.IsNullOrEmpty(digerDbs))
            {
                var digerListe = digerDbs.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
                if (digerTaraf == "source")
                    model.SourceDatabaseList = digerListe;
                else
                    model.DestinationDatabaseList = digerListe;

                TempData.Keep($"{digerTaraf}Databases");
            }
            if (TempData.Peek($"{digerTaraf}Mesaj") != null)
            {
                TempData.Keep($"{digerTaraf}Mesaj");
                TempData.Keep($"{digerTaraf}Basarili");
            }

            if (string.IsNullOrWhiteSpace(server))
            {
                TempData[$"{taraf}Mesaj"] = "Lütfen sunucu adını girin.";
                TempData[$"{taraf}Basarili"] = false;
                return View("Baglanti", model);
            }

            var connectionString = _connectionService.BuildConnectionString(server, database, authType, username, password);
            var (success, message) = _connectionService.TestConnection(connectionString);

            if (success)
            {
                var databases = _connectionService.GetDatabaseList(connectionString);
                if (taraf == "source")
                    model.SourceDatabaseList = databases;
                else
                    model.DestinationDatabaseList = databases;

                TempData[$"{taraf}Databases"] = string.Join(",", databases);
            }

            TempData[$"{taraf}Mesaj"] = message;
            TempData[$"{taraf}Basarili"] = success;

            return View("Baglanti", model);
        }

        [HttpPost]
        public IActionResult Karsilastir(ConnectionModel model)
        {
            if (string.IsNullOrEmpty(model.SourceDatabase) || string.IsNullOrEmpty(model.DestinationDatabase))
            {
                ModelState.AddModelError("", "Lütfen kaynak ve hedef veritabanlarını seçin.");
                return View("Baglanti", model);
            }

            if (model.SourceServer == model.DestinationServer && model.SourceDatabase == model.DestinationDatabase)
            {
                ModelState.AddModelError("", "Kaynak ve hedef olarak aynı veritabanı seçilemez.");
                return View("Baglanti", model);
            }

            try
            {
                var kaynakConnStr = _connectionService.BuildConnectionString(
                    model.SourceServer, model.SourceDatabase, model.SourceAuthType, model.SourceUsername, model.SourcePassword);

                var hedefConnStr = _connectionService.BuildConnectionString(
                    model.DestinationServer, model.DestinationDatabase, model.DestinationAuthType, model.DestinationUsername, model.DestinationPassword);

                var kaynakSchema = _schemaService.GetSchema(kaynakConnStr);
                var hedefSchema = _schemaService.GetSchema(hedefConnStr);

                var sonuc = _compareService.Karsilastir(kaynakSchema, hedefSchema);

                ViewBag.KaynakDb = model.SourceDatabase;
                ViewBag.HedefDb = model.DestinationDatabase;

                return View("Sonuc", sonuc);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Karşılaştırma sırasında hata oluştu: {ex.Message}");
                return View("Baglanti", model);
            }
        }
    }
}