using System.Net;
using igiagbin.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace igiagbin.Controllers
{
	public class PasteController : Controller
	{
		// GET: PasteController
		public ActionResult Index()
		{
			return View();
		}

		// GET: PasteController/Details/5
		public ActionResult Details(string id)
		{
			id = WebUtility.UrlDecode(id);
            using var session = DocumentStoreHolder.Store.OpenSession();
			ViewData.Model = session.Load<PasteModel>(id);
			return View();
		}

		public ActionResult PasteError()
		{
			int statusCode = HttpContext.Session.GetInt32("ErrorStatus") ?? 500;

            Response.StatusCode = statusCode;
			ViewData.Model = new PasteErrorModel()
			{
				ErrorMessage = HttpContext.Session.GetString("ErrorMessage") ?? "No further information...",
				ReturnHref = HttpContext.Session.GetString("ErrorReturnHref")
            };

			return View();
		}

		// POST: PasteController/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Create(PasteModel model, [FromForm(Name = "cf-turnstile-response")]string cf_token)
		{
			TurnstileVerifier turnstileVerifier = new(Environment.GetEnvironmentVariable("CF_SECRET") ?? throw new Exception("Turnstile secret is required"));
			var result = await turnstileVerifier.VerifyTokenAsync(cf_token);
			if (result.Success != true)
			{
                return ReturnError("Invalid captcha!", "/Paste", 400);
            }

			Console.WriteLine(cf_token);
			if (!ModelState.IsValid)
			{
                IEnumerable<string> errorMessages = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
                return ReturnError(String.Join("</br>",errorMessages), "/Paste", 400);
            }


			using var session = DocumentStoreHolder.Store.OpenSession();
			session.Store(model);
			session.SaveChanges();
			
			try
			{
				return RedirectToAction(nameof(Details),new {id = model.Id});
			}
			catch
			{
				return ReturnError("No further information.", "/Paste", 500);
            }
		}

		private ActionResult ReturnError(string message,string returnHref,int statusCode)
		{
            HttpContext.Session.SetString("ErrorMessage", message);
            HttpContext.Session.SetString("ErrorReturnHref", returnHref);
            HttpContext.Session.SetInt32("ErrorStatus", statusCode);
            return RedirectToAction(nameof(PasteError));
        }
    }
}
