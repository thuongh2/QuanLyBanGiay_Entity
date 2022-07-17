using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyBanGiay_Ver_Entity.BS
{
    public class BLProducts : IProducts
    {
        private DataTable convertTableProduct(IQueryable<product> products)
        {

            DataTable dt = new DataTable();
            dt.Columns.Add("Mã Sản Phẩm", typeof(int));
            dt.Columns.Add("Tên Sản Phẩm");
            // chỉ hiện columns description trong chi tiết sản phẩm
            dt.Columns.Add("Mô tả sản phẩm");
            dt.Columns.Add("Giá tiền");
            dt.Columns.Add("Hình ành");
            dt.Columns.Add("Danh mục");
            dt.Columns.Add("Trạng Thái");

            foreach (var product in products)
            {
                dt.Rows.Add(product.id, product.name, product.description, product.price,
                    product.image, product.category.name, product.active);
            }

            return dt;
        }

        private DataTable convertTableProduct(product product)
        {

            DataTable dt = new DataTable();
            dt.Columns.Add("Mã Sản Phẩm", typeof(int));
            dt.Columns.Add("Tên Sản Phẩm");
            // chỉ hiện columns description trong chi tiết sản phẩm
            dt.Columns.Add("Mô tả sản phẩm");
            dt.Columns.Add("Giá tiền");
            dt.Columns.Add("Hình ành");
            dt.Columns.Add("Danh mục");
            dt.Columns.Add("Trạng Thái");

            dt.Rows.Add(product.id, product.name, product.description, product.price, product.image, product.category.name, product.active);

            return dt;
        }

        /*
         * soft delete từ database bằng cách set active == !active
         */
        public bool deleteProduct(int id, ref string err)
        {
            QuanLiBanGiayEntities qlBanGiay = new QuanLiBanGiayEntities();

            var product = (from p in qlBanGiay.products
                           where p.id == id
                           select p).SingleOrDefault();

            if (product != null)
            {
                product.active = !product.active;
                qlBanGiay.SaveChanges();
            }

            return true;

        }

        public bool deleteProductByAdmin(int id, ref string err)
        {
            QuanLiBanGiayEntities qlBanGiay = new QuanLiBanGiayEntities();

            product product = new product();
            product.id = id;

            qlBanGiay.products.Attach(product);
            qlBanGiay.products.Remove(product);
            qlBanGiay.SaveChanges();

            return true;

        }

        /* CHỨC NĂNG TÌM KIẾM */

        public DataTable findProductById(int id, string err)
        {
            QuanLiBanGiayEntities qlBanGiay = new QuanLiBanGiayEntities();

            var product = (from p in qlBanGiay.products
                           where p.id == id
                           select p).SingleOrDefault();

            if(product == null)
            {
                err = "Sản phẩm không tồn tại";
                return new DataTable();
            }    
            else
            {
                return this.convertTableProduct(product);

            }    
        }

        public DataTable findProductByName(string name, string err)
        {
            QuanLiBanGiayEntities qlBanGiay = new QuanLiBanGiayEntities();

            var product = (from p in qlBanGiay.products
                           where p.name.Contains(name) && p.active == true
                           select p);

            if (product == null)
            {
                err = "Sản phẩm không tồn tại";
            }

            return this.convertTableProduct(product);
        }

        public DataTable getProductByCategory(int id)
        {
            QuanLiBanGiayEntities qlBanGiay = new QuanLiBanGiayEntities();

            var products = (from p in qlBanGiay.products
                            where p.active == true && p.category_id == id
                            select p);

            
            return this.convertTableProduct(products);
        }

        public DataTable getProducts()
        {
            QuanLiBanGiayEntities qlBanGiay = new QuanLiBanGiayEntities();

            var products = from p in qlBanGiay.products
                           where p.active == true
                           select p;

            return this.convertTableProduct(products);
        }

        public DataTable getProductsByAdmin()
        {
            QuanLiBanGiayEntities qlBanGiay = new QuanLiBanGiayEntities();

            var products = from p in qlBanGiay.products
                           select p;

            return this.convertTableProduct(products);
        }

        public bool saveProduct(int id, string name, string description, double price,
            string image, int category, ref string err)
        {
            QuanLiBanGiayEntities qlBanGiay = new QuanLiBanGiayEntities();

            product products = new product();
            products.id = id;
            products.name = name;
            products.description = description;
            products.price = price;
            products.image = image;
            products.category_id = category;
            products.active = true;

            qlBanGiay.products.Add(products);
            qlBanGiay.SaveChanges();

            return true;

        }

        public bool updateProduct(int id, string name, string description, double price,
            string image, int category, ref string err)
        {
            QuanLiBanGiayEntities qlBanGiay = new QuanLiBanGiayEntities();

            var product = qlBanGiay.products.Where(p => p.id == id).FirstOrDefault();

            if (product != null)
            {
                product.name = name;
                product.description = description;
                product.price = price;
                product.image = image;
                if(category != 0)
                    product.category_id = category;

                qlBanGiay.SaveChanges();
            }
            else
            {
                err = "Không thể cập nhật sản phẩm";
                return false;
            }    

            return true;

        }
    }
}
