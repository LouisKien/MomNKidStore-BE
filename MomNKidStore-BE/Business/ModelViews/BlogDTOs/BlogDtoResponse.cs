using MomNKidStore_BE.Business.ModelViews.ProductDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MomNKidStore_BE.Business.ModelViews.BlogDTOs
{
    public class BlogDtoResponse
    {
        public int BlogId { get; set; }
        public string BlogTitle { get; set; } = null!;
        public string BlogContent { get; set; } = null!;
        public string? BlogImage { get; set; }
        public bool Status { get; set; }
        public List<ProductDtoResponse> BlogProducts { get; set; } = new List<ProductDtoResponse>();
    }
}
