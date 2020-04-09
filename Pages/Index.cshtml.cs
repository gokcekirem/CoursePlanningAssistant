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

        public JsonResult json;

        public JArray jarray;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            //Fill json here
            using (StreamReader file = System.IO.File.OpenText(@"BubbleData\bubble_data.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                jarray = (JArray)serializer.Deserialize(file, typeof(JArray));
                //Console.WriteLine(jarray);
            }
            //json = jarray;

        }
    }
}
