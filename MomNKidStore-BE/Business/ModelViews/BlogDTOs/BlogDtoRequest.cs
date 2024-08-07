using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MomNKidStore_BE.Business.ModelViews.BlogDTOs
{
    public class BlogDtoRequest
    {
        public string BlogTitle { get; set; } = null!;
        public string BlogContent { get; set; } = null!;
        public string? BlogImage { get; set; }
        public bool Status { get; set; }
    }
}
