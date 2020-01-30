using System;
using System.Collections.Generic;
using System.Text;

namespace ExplicitMapper.Tests.Integration.NestedClassMapping.ObjectInitializerConfiguration
{
    public class ProductToProductViewModelObjectInitializerConfiguration : MappingConfiguration
    {
        public ProductToProductViewModelObjectInitializerConfiguration()
        {
            CreateMap<Product, ProductViewModel>(
                s => new ProductViewModel()
                {
                    Description = s.Manufacturer + " - " + s.Name,
                    Size = Map<SizeViewModel>(s.Size)
                });

            CreateMap<Size, SizeViewModel>(
                s => new SizeViewModel()
                {
                    Width = s.Width.ToString() + "m",
                    Height = s.Height.ToString() + "m",
                    Depth = s.Depth.ToString() + "m"
                });
        }
    }
}
