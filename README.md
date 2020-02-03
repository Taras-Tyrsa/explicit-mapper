# ExplicitMapper
ExplicitMapper is an alternative to AutoMapper for those of us who feels frustrated with it and prefer explicit or even manual mapping configurations but still looks for convinient and laconic way to organize mappings.

# Examples
Let's say we want to map following classes:

    public class Product
    {
        public string Manufacturer { get; set; }
        public string Name { get; set; }
        public Size Size { get; set; }
    }

    public class Size
    {
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public decimal Depth { get; set; }
    }

    public class ProductViewModel
    {
        public string Description { get; set; }
        public SizeViewModel Size { get; set; }
    }

    public class SizeViewModel
    {
        public string Width { get; set; }
        public string Height { get; set; }
        public string Depth { get; set; }
    }



You can specify one of the two equivalent configurations for mapping.

Using per field configuration:

    CreateMap<Product, ProductViewModel>()
        .For(d => d.Description, s => s.Manufacturer + " - " + s.Name)
        .For(d => d.Size, s => Map<SizeViewModel>(s.Size));

    CreateMap<Size, SizeViewModel>()
        .For(d => d.Width, s => s.Width.ToString() + "m")
        .For(d => d.Height, s => s.Height.ToString() + "m")
        .For(d => d.Depth, s => s.Depth.ToString() + "m");
        
Or using object initializer syntax:

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

Then, you can do mapping in the following manner:

    var productViewModel = Mapper.Map<ProductViewModel>(product);
