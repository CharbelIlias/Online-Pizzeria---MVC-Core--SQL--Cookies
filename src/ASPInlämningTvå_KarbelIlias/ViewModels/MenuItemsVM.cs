using ASPInlämningTvå_KarbelIlias.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASPInlämningTvå_KarbelIlias.ViewModels
{
    public class MenuItemsVM
    {
        public Matratt Matratt { get; set; }
        public List<Produkt> Produkter { get; set; }

    }
}
