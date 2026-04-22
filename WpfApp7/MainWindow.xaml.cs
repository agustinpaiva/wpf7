using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp7
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public ObservableCollection<Product> Products { get; set; }
        public ObservableCollection<Product> FilteredProducts { get; set; }
        public ObservableCollection<CartItem> Cart { get; set; }

        private decimal _total;
        public decimal Total
        {
            get => _total;
            set
            {
                _total = value;
                OnPropertyChanged();
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            // Datos de prueba
            Products = new ObservableCollection<Product>
            {
                new Product { Id = 1, Name = "Pan (kg)", Price = 1200 },
                new Product { Id = 2, Name = "Leche 1L", Price = 950 },
                new Product { Id = 3, Name = "Yerba 500g", Price = 1500 },
                new Product { Id = 4, Name = "Azúcar 1kg", Price = 800 },
                new Product { Id = 5, Name = "Aceite 900ml", Price = 1800 },
                new Product { Id = 6, Name = "Fideos 500g", Price = 700 },
                new Product { Id = 7, Name = "Arroz 1kg", Price = 850 },
                new Product { Id = 8, Name = "Gaseosa 2.25L", Price = 1600 },
                new Product { Id = 9, Name = "Jabón Blanco", Price = 450 },
                new Product { Id = 10, Name = "Detergente", Price = 1100 }
            };

            FilteredProducts = new ObservableCollection<Product>(Products);
            Cart = new ObservableCollection<CartItem>();
            
            Cart.CollectionChanged += (s, e) => CalculateTotal();
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            var search = ((TextBox)sender).Text.ToLower();
            FilteredProducts.Clear();
            foreach (var p in Products.Where(p => p.Name.ToLower().Contains(search)))
            {
                FilteredProducts.Add(p);
            }
        }

        private void BtnAddToCart_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Product product)
            {
                var existingItem = Cart.FirstOrDefault(i => i.Product.Id == product.Id);
                if (existingItem != null)
                {
                    existingItem.Quantity++;
                    CalculateTotal();
                }
                else
                {
                    Cart.Add(new CartItem { Product = product, Quantity = 1 });
                }
            }
        }

        private void BtnRemoveFromCart_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is CartItem item)
            {
                Cart.Remove(item);
            }
        }

        private void BtnFinalizeSale_Click(object sender, RoutedEventArgs e)
        {
            if (Cart.Count == 0)
            {
                MessageBox.Show("El carrito está vacío.", "Atención");
                return;
            }

            MessageBox.Show($"Venta finalizada con éxito.\nTotal: ${Total}", "Venta Completada");
            Cart.Clear();
        }

        private void CalculateTotal()
        {
            Total = Cart.Sum(i => i.Subtotal);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }

    public class CartItem : INotifyPropertyChanged
    {
        private int _quantity;
        public Product Product { get; set; }
        public int Quantity 
        { 
            get => _quantity; 
            set //asd//
            { 
                _quantity = value; 
                OnPropertyChanged();
                OnPropertyChanged(nameof(Subtotal));
            } 
        }
        public decimal Subtotal => Product.Price * Quantity;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
