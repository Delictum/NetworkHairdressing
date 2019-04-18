using System.Collections.Generic;
using System.Web.Mvc;

namespace NetworkHairdressing.Models
{
    public class ReceptionViewModels
    {
        public SelectList ListBarbershops { get; set; }
        public SelectList ListEmployees { get; set; }
        public SelectList ListPrices { get; set; }
        public Reception Reception { get; set; }
    }
}