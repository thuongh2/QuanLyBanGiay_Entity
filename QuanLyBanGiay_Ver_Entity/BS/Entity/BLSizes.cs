using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyBanGiay_Ver_Entity.BS
{
    public class BLSizes : ISizes
    {
        public bool createSize(int numberSize, int quantity, int product, ref string err)
        {
            QuanLiBanGiayEntities qlBanGiay = new QuanLiBanGiayEntities();

            size size = new size();

            //size.id = id;
            size.number_size = numberSize;
            size.quantity = quantity;
            size.product_id = product;
            size.active = true;

            qlBanGiay.sizes.Add(size);
            qlBanGiay.SaveChanges();

            return true;
        }

        public bool deleteSize(int id, ref string err)
        {
            QuanLiBanGiayEntities qlBanGiay = new QuanLiBanGiayEntities();

            var size = (from o in qlBanGiay.sizes
                         where o.id == id
                         select o).SingleOrDefault();

            if (size != null)
            {
                size.active = !size.active;
                qlBanGiay.SaveChanges();
            }

            return true;
        }

        public bool deleteSizeByAdmin(int id, ref string err)
        {
            QuanLiBanGiayEntities qlBanGiay = new QuanLiBanGiayEntities();

            size size = new size();

            size.id = id;

            qlBanGiay.sizes.Attach(size);
            qlBanGiay.sizes.Remove(size);
            qlBanGiay.SaveChanges();
            return true;
        }

        public DataTable getSizesByProduct(int productId)
        {
            QuanLiBanGiayEntities qlBanGiay = new QuanLiBanGiayEntities();

            var size = from s in qlBanGiay.sizes
                       where s.product_id == productId && s.active == true
                       select s;

            DataTable dt = new DataTable();
            dt.Columns.Add("Mã Size", typeof(int));
            dt.Columns.Add("Size");
            // chỉ hiện columns description trong chi tiết sản phẩm
            dt.Columns.Add("Số lượng");
            dt.Columns.Add("Tên sản phẩm");

            foreach (var s in size)
            {
                dt.Rows.Add(s.id, s.number_size, s.quantity, s.product.name);
            }

            return dt;
        }

        public DataTable getAllSize()
        {
            QuanLiBanGiayEntities qlBanGiay = new QuanLiBanGiayEntities();

            var size = from s in qlBanGiay.sizes
                           select s;

            DataTable dt = new DataTable();
            dt.Columns.Add("Mã Size", typeof(int));
            dt.Columns.Add("Size");
            // chỉ hiện columns description trong chi tiết sản phẩm
            dt.Columns.Add("Số lượng");
            dt.Columns.Add("Tên sản phẩm");
            dt.Columns.Add("Trạng thái");

            foreach (var s in size)
            {
                dt.Rows.Add(s.id, s.number_size, s.quantity, s.product.name, s.active);
            }

            return dt;
        }

        public bool updateSize(int id, int numberSize, int quantity, int product, ref string err)
        {
            QuanLiBanGiayEntities qlBanGiay = new QuanLiBanGiayEntities();

            var size = (from o in qlBanGiay.sizes
                        where o.id == id
                        select o).SingleOrDefault();

            if (size != null)
            {
                size.number_size = numberSize;
                size.quantity = quantity;
                size.product_id = product;
                qlBanGiay.SaveChanges();
            }

            return true;
        }

    }
}
