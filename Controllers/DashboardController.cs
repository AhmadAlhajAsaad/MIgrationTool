using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Migration_Tool_GraphAPI.Controllers
{
    public class DashboardController : Controller
    {
        // GET: Dashboard
        public ActionResult Index()
        {
            return View();
        }

        // POST: Dashboard/TransferFile
        [HttpPost]
        public ActionResult TransferFile(HttpPostedFileBase file, string destinationLink)
        {
            if (file != null && !string.IsNullOrEmpty(destinationLink))
            {
                // Add logic here to handle file upload and transfer to the specified link
                // This can include validation, file processing, and API calls for file transfer

                ViewBag.Message = "File transfer initiated successfully!";
            }
            else
            {
                ViewBag.Message = "Please select a file and enter a destination link.";
            }

            return View("Index");
        }
    }
}

//using Microsoft.Graph;
//using Migration_Tool_GraphAPI.Helpers;
//using System;
//using System.IO;
//using System.Threading.Tasks;
//using System.Web;
//using System.Web.Mvc;
//using System.Configuration;

//namespace Migration_Tool_GraphAPI.Controllers
//{
//    public class DashboardController : Controller
//    {
//        public static string appId = ConfigurationManager.AppSettings["ida:AppId"];
//        // Method to render the dashboard view
//        public ActionResult Index()
//        {
//            return View();
//        }

//        // Start transfer action
//        [HttpPost]
//        public async Task<ActionResult> StartTransfer(HttpPostedFileBase file, string destinationUrl)
//        {
//            if (file == null || string.IsNullOrWhiteSpace(destinationUrl))
//            {
//                ViewBag.Error = "File or destination URL is missing.";
//                return View("Index");
//            }

//            try
//            {
//                // Get the Microsoft Graph client
//                var graphClient = GraphHelper.GetAuthenticatedClient();

//                // Read the file into a byte array
//                byte[] fileData;
//                using (var binaryReader = new BinaryReader(file.InputStream))
//                {
//                    fileData = binaryReader.ReadBytes(file.ContentLength);
//                }

//                // Extract SharePoint site and path info from the destination URL
//                var siteId = appId; // Replace with actual Site ID
//                var documentLibrary = "Documenten"; // Replace with the correct library name or path

//                // Perform the upload
//                var uploadResult = await graphClient
//                    .Sites[siteId]
//                    .Drives[documentLibrary]
//                    .Root
//                    .ItemWithPath(file.FileName)
//                    .Content
//                    .Request()
//                    .PutAsync<DriveItem>(new MemoryStream(fileData));

//                ViewBag.Message = "File uploaded successfully!";
//            }
//            catch (Exception ex)
//            {
//                ViewBag.Error = $"An error occurred: {ex.Message}";
//            }

//            return View("Index");
//        }
//    }
//}
