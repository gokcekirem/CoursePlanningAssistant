using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CoursePlanner.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public List<string> selectedCourses;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {

        }

        public ActionResult OnPostSend()
        {
            string sPostValue1 = "";
            string sPostValue2 = "";
            string sPostValue3 = "";
            {
                MemoryStream stream = new MemoryStream();
                Request.Body.CopyTo(stream);
                stream.Position = 0;
                using (StreamReader reader = new StreamReader(stream))
                {
                    string requestBody = reader.ReadToEnd();
                    if (requestBody.Length > 0)
                    {
                        var obj = JsonConvert.DeserializeObject<PostData>(requestBody);
                        if (obj != null)
                        {
                            sPostValue1 = obj.Item1;
                            sPostValue2 = obj.Item2;
                            sPostValue3 = obj.Item3;
                        }
                    }
                }
            }
            List<string> lstString = new List<string>
            {
                sPostValue1,
                sPostValue2,
                sPostValue3
            };

            selectedCourses = lstString;
            Console.WriteLine(selectedCourses[1]);

            return new JsonResult(lstString);
        }
    }

    public class PostData
    {
        public string Item1 { get; set; }
        public string Item2 { get; set; }
        public string Item3 { get; set; }
    }
}
