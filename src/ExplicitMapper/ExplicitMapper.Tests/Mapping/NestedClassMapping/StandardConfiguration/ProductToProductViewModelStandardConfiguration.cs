using System;
using System.Collections.Generic;
using System.Text;

namespace ExplicitMapper.Tests.Mapping.NestedClassMapping.StandardConfiguration
{
    public class ProductToProductViewModelStandardConfiguration : MappingConfiguration
    {
        public ProductToProductViewModelStandardConfiguration()
        {
            CreateMap<Product, ProductViewModel>()
                .For(d => d.Description, s => s.Manufacturer + " - " + s.Name)
                .For(d => d.Size, s => Map<SizeViewModel>(s.Size));

            CreateMap<Size, SizeViewModel>()
                .For(d => d.Width, s => s.Width.ToString() + "m")
                .For(d => d.Height, s => s.Height.ToString() + "m")
                .For(d => d.Depth, s => s.Depth.ToString() + "m");
        }
    }
}
