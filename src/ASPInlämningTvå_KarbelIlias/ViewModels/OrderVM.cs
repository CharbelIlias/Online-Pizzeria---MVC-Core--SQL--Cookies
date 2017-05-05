using ASPInlämningTvå_KarbelIlias.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASPInlämningTvå_KarbelIlias.ViewModels
{
    public class OrderVM
    {
        public Kund kund { get; set; }

        public Bestallning beställning { get; set; }
    }
}
