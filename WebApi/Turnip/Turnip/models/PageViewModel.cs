using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turnip.models
{
    public class PageViewModel : IPageViewModel
    {
        public LayoutModel Layout { get; } = new();
    }
}