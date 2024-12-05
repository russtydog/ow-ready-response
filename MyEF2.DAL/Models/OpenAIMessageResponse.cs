using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEF2.DAL.Models
{
    public class OpenAIMessageResponse
    {
        public string @object { get; set; }
        public List<Datum> data { get; set; }
        public string first_id { get; set; }
        public string last_id { get; set; }
        public bool has_more { get; set; }
    }
    public class Annotation
    {
        public string type { get; set; }
        public string text { get; set; }
        public int start_index { get; set; }
        public int end_index { get; set; }
        public FileCitation file_citation { get; set; }
    }

    public class Content
    {
        public string type { get; set; }
        public Text text { get; set; }
    }

    public class Datum
    {
        public string id { get; set; }
        public string @object { get; set; }
        public int created_at { get; set; }
        public string assistant_id { get; set; }
        public string thread_id { get; set; }
        public string run_id { get; set; }
        public string role { get; set; }
        public List<Content> content { get; set; }
        public List<object> file_ids { get; set; }
        public Metadata metadata { get; set; }
    }

    public class FileCitation
    {
        public string file_id { get; set; }
        public string quote { get; set; }
    }

    public class Metadata
    {
    }
    public class Text
    {
        public string value { get; set; }
        public List<Annotation> annotations { get; set; }
    }
}
