using MomNKidStore_Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MomNKidStore_BE.Business.ModelViews.BlogDTOs
{
    public class BlogProductDto : BlogDtoRequest
    {
        public List<int>? productId {  get; set; }
    }
}
