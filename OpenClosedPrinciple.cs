using System;
using System.Collections.Generic;

namespace SOLID{
    public enum Color
  {
    Red, Green, Blue
  }

  public enum Size
  {
    Small, Medium, Large, Yuge
  }

  public class Product
  {
    public string Name;
    public Color Color;
    public Size Size;
    public Product(string name, Color color, Size size)
    {
      Name = name ?? throw new ArgumentNullException(paramName: nameof(name));
      Color = color;
      Size = size;
    }
  }
  public class ProductFilter
  {
    // let's suppose we don't want ad-hoc queries on products
    public IEnumerable<Product> FilterByColor(IEnumerable<Product> products, Color color)
    {
      foreach (var p in products)
        if (p.Color == color)
          yield return p;
    }
    
    public static IEnumerable<Product> FilterBySize(IEnumerable<Product> products, Size size)
    {
      foreach (var p in products)
        if (p.Size == size)
          yield return p;
    }

    public static IEnumerable<Product> FilterBySizeAndColor(IEnumerable<Product> products, Size size, Color color)
    {
      foreach (var p in products)
        if (p.Size == size && p.Color == color)
          yield return p;
    } 
    // state space explosion
      // 3 criteria = 7 methods

    // OCP = open for extension but closed for modification
  }

  //2 divisions => 1. Filter 2. Specification
  public interface ISpecification<T>
  {
    bool IsSatisfied(T p);
  }
//Filtering based o specification
  public interface IFilter<T>
  {
    IEnumerable<T> Filter(IEnumerable<T> items, ISpecification<T> spec);
  }
    //Color Filter
    //For COlor filter we need color specifocation
    public class ColorSpecification : ISpecification<Product>
    {
        private Color color;
        public ColorSpecification(Color color)
        {
            this.color = color;
        }
        public bool IsSatisfied(Product p)
        {
           return p.Color == color;
        }
    }
      public class SizeSpecification : ISpecification<Product>
    {
        private Size size;
        public SizeSpecification(Size size)
        {
            this.size = size;
        }
        public bool IsSatisfied(Product p)
        {
           return p.Size == size;
        }
    }

    //More than one specification
    public class AndSpecification<T> : ISpecification<T>
    {
        private ISpecification<T> first,second;
        public AndSpecification(ISpecification<T> first , ISpecification<T> second)
        {
           this.first = first;
            this.second = second;
        }
        public bool IsSatisfied(T p)
        {
           return first.IsSatisfied(p) && second.IsSatisfied(p);
        }
    }
    //For every new filter type.. a new specification calss is added. 
    public class BetterFilter : IFilter<Product>
    {
        public IEnumerable<Product> Filter(IEnumerable<Product> items, ISpecification<Product> spec)
        {
            foreach (var item in items)
            {
                if(spec.IsSatisfied(item)){
                    yield return item;
                }
            }
        }
    }
 public partial class Demo
  {
    public static void RunOCP()
    {
      var apple = new Product("Apple", Color.Green, Size.Small);
      var tree = new Product("Tree", Color.Green, Size.Large);
      var house = new Product("House", Color.Blue, Size.Large);

      Product[] products = {apple, tree, house};

      var pf = new ProductFilter();
      Console.WriteLine("Green products (old):");
      foreach (var p in pf.FilterByColor(products, Color.Green))
        Console.WriteLine($" - {p.Name} is green");

      // ^^ BEFORE

      // vv AFTER
      var bf = new BetterFilter();
      Console.WriteLine("Green products (new):");
      foreach (var p in bf.Filter(products, new ColorSpecification(Color.Green)))
        Console.WriteLine($" - {p.Name} is green");

      Console.WriteLine("Large products");
      foreach (var p in bf.Filter(products, new SizeSpecification(Size.Large)))
        Console.WriteLine($" - {p.Name} is large");

      Console.WriteLine("Large blue items");
      foreach (var p in bf.Filter(products,
        new AndSpecification<Product>(new ColorSpecification(Color.Blue), new SizeSpecification(Size.Large)))
      )
      {
        Console.WriteLine($" - {p.Name} is big and blue");
      }
    }
  }
}