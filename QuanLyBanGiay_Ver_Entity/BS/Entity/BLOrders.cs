using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyBanGiay_Ver_Entity.BS
{
    public class BLOrders : IOrders
    {

        private DataTable convertTableOrder(IQueryable<order> orders)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Mã Danh Mục", typeof(int));
            dt.Columns.Add("Tên Khách Hàng");
            dt.Columns.Add("Địa Chỉ Khách Hàng");
            dt.Columns.Add("Email Khách Hàng");
            dt.Columns.Add("Số điện thoại");
            dt.Columns.Add("Tổng Tiền");
            dt.Columns.Add("Trạng Thái");


            foreach (var order in orders)
            {
                dt.Rows.Add(order.id, order.customer_name, order.customer_address, order.customer_email, order.customer_phone
                    , order.amount, order.active);
            }

            return dt;
        }

        private DataTable convertTableOrder(order order)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Mã Danh Mục", typeof(int));
            dt.Columns.Add("Tên Khách Hàng");
            dt.Columns.Add("Địa Chỉ Khách Hàng");
            dt.Columns.Add("Email Khách Hàng");
            dt.Columns.Add("Số điện thoại");
            dt.Columns.Add("Tổng Tiền");
            dt.Columns.Add("Trạng Thái");

            dt.Rows.Add(order.id, order.customer_name, order.customer_address, order.customer_email, order.customer_phone
                    , order.amount, order.active);

            return dt;
        }

        public bool deleteOrders(int id, ref string err)
        {
            QuanLiBanGiayEntities qlBanGiay = new QuanLiBanGiayEntities();

            var order = (from o in qlBanGiay.orders
                         where o.id == id
                         select o).SingleOrDefault();

            if (order != null)
            {
                order.active = !order.active;
                qlBanGiay.SaveChanges();
            }
            else
            {
                err = "Không thể xóa sản phẩm";
                return false;
            }    

            return true;
        }

        public bool deleteOrdersById(int id, ref string err)
        {
            QuanLiBanGiayEntities qlBanGiay = new QuanLiBanGiayEntities();

            order order = new order();
            order.id = id;

            qlBanGiay.orders.Attach(order);
            qlBanGiay.orders.Remove(order);

            qlBanGiay.SaveChanges();

            return true;
        }


        public DataTable findOrderByName(string name, ref string err)
        {
            QuanLiBanGiayEntities qlBanGiay = new QuanLiBanGiayEntities();

            var order = (from o in qlBanGiay.orders
                         where o.customer_name == name
                         select o).SingleOrDefault();

            return this.convertTableOrder(order);

        }

        public DataTable getOrderById(int id, ref string err)
        {
            QuanLiBanGiayEntities qlBanGiay = new QuanLiBanGiayEntities();

            var order = (from o in qlBanGiay.orders
                         where o.id == id && o.active == true
                         select o).SingleOrDefault();

            return this.convertTableOrder(order);

        }

        public DataTable findOrderByEmail(string email, ref string err)
        {
            QuanLiBanGiayEntities qlBanGiay = new QuanLiBanGiayEntities();

            var order = (from o in qlBanGiay.orders
                         where o.customer_email == email
                         select o).SingleOrDefault();

            return this.convertTableOrder(order);
        }

        public DataTable getAllOrdersByAdmin()
        {
            QuanLiBanGiayEntities qlBanGiay = new QuanLiBanGiayEntities();

            var orders = from o in qlBanGiay.orders
                         select o;

            return this.convertTableOrder(orders);
        }


        public int saveOrders(string name, string address, string email, string phone, ref string err)
        {
            QuanLiBanGiayEntities qlBanGiay = new QuanLiBanGiayEntities();

            var order = (from o in qlBanGiay.orders
                         where o.customer_phone == phone || o.customer_email == email
                         select o).SingleOrDefault();
            // nếu chưa có thông tin đơn hàng thì thực hiện tạo mới ngược lại thực hiện cập nhật
            if (order == null)
            {
                order orders = new order();

                orders.customer_name = name;
                orders.customer_address = address;
                orders.customer_phone = phone;
                orders.customer_email = email;
                orders.active = true;
                orders.amount = 0;

                order order1 =  qlBanGiay.orders.Add(orders);
                qlBanGiay.SaveChanges();

                return order1.id;
            }
            else
            {
                this.updateOrders(order.id, name, order.amount, address, email, phone, ref err);
                return order.id;
            }    
            

        }

        public bool updateOrders(int id, string name,double amount, string address, string email, string phone, ref string err)
        {
            QuanLiBanGiayEntities qlBanGiay = new QuanLiBanGiayEntities();

            var order = (from o in qlBanGiay.orders
                         where o.id == id
                         select o).SingleOrDefault();

            if(order != null)
            {
                order.customer_name = name;
                order.customer_address = address;
                order.customer_phone = phone;
                order.customer_email = email;
                order.amount = amount;

                qlBanGiay.SaveChanges();

                return true;
            }    
            else
            {
                err = "Thông tin đơn hàng không tồn tại";
                return false;
            }    

        }



    }
}
