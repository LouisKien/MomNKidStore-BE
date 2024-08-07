using MomNKidStore_BE.Business.ModelViews.CustomerDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MomNKidStore_BE.Business.ModelViews.FeedbackDTOs
{
    public class FeedbackDtoResponse
    {
        public int FeedbackId { get; set; }
        public int CustomerId { get; set; }
        public int ProductId { get; set; }
        public string FeedbackContent { get; set; } = null!;
        public double RateNumber { get; set; }
        public double? AverageNumber { get; set; }

        public bool Status { get; set; }
        public CustomerDto Customer { get; set; } = null!;


    }
}
