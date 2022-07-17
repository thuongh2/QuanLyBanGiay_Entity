using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyBanGiay_Ver_Entity.BS
{
    public class BLOrdersDetails : IOrdersDetails
    {
        private DataTable convertTableOrder(IQueryable<order_details> orders)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Mã Đơn Hàng", typeof(int));
            dt.Columns.Add("Tên Người Đặt Hàng");
            dt.Columns.Add("Tên Người Dùng");
            dt.Columns.Add("Tên sản phẩm");
            dt.Columns.Add("Kích thước");
            dt.Columns.Add("Số lượng");
            dt.Columns.Add("Giá");
            dt.Columns.Add("Tổng Tiền");
            dt.Columns.Add("Phương thức thanh toán");

            dt.Columns.Add("Ngày đặt hàng");
            dt.Columns.Add("Trạng thái");


            foreach (var order in orders)
            {
                dt.Rows.Add(order.id, order.order_id, order.users_id,
                    order.product_id, order.size, order.quantity, order.price,
                    order.amount, order.payment, order.date_order, order.active
                    );
            }

            return dt;
        }

       
        // xác nhận đơn hàng với khách hàng
        public bool createOrderDetail(int orderId, int productId, int userId, int size,
                                    int quantity, double price, string payment, ref string err)
        {
            QuanLiBanGiayEntities qlBanGiay = new QuanLiBanGiayEntities();

            var sizeChange = (from p in qlBanGiay.sizes
                              where p.product_id == productId && p.number_size == size
                              select p).SingleOrDefault();
            var orderChange = (from p in qlBanGiay.orders
                               where p.id == orderId
                               select p).SingleOrDefault();

            if (sizeChange.quantity < quantity)
            {
                err = "Không đủ hàng trong kho";
                return false;
            }


            order_details order = new order_details();

            //order.id = id;
            order.order_id = orderId;
            order.product_id = productId;
            order.users_id = userId;
            order.size = size;
            order.quantity = quantity;
            order.price = price;
            order.amount = quantity * price;
            order.payment = payment;
            order.date_order = DateTime.Now;
            order.active = 1;
            sizeChange.quantity -= quantity;
            orderChange.amount += quantity * price;


            qlBanGiay.order_details.Add(order);
   
            qlBanGiay.SaveChanges();

            return true;
        }


        public bool deleteOrderDetail(int id, ref string err)
        {
            QuanLiBanGiayEntities qlBanGiay = new QuanLiBanGiayEntities();

            var orderDetailChange = (from p in qlBanGiay.order_details
                                     where p.id == id
                                     select p).SingleOrDefault();

            var sizeChange = (from p in qlBanGiay.sizes
                              where p.product_id == orderDetailChange.product_id && p.number_size == orderDetailChange.size
                              select p).SingleOrDefault();

            if (orderDetailChange != null && sizeChange != null)
            {
               
                sizeChange.quantity += orderDetailChange.quantity;

                order_details orderDetail = new order_details();
                orderDetail.id = id;

                qlBanGiay.order_details.Attach(orderDetail);
                qlBanGiay.order_details.Remove(orderDetail);

                qlBanGiay.SaveChanges();


                return true;
            }

            return false;
        }

        public bool deleteOrderDetailByUser(int id, ref string err)
        {
            QuanLiBanGiayEntities qlBanGiay = new QuanLiBanGiayEntities();

            var orderDetailChange = (from p in qlBanGiay.order_details
                                     where p.id == id
                                     select p).SingleOrDefault();

            var sizeChange = (from p in qlBanGiay.sizes
                              where p.product_id == orderDetailChange.product_id && p.number_size == orderDetailChange.size
                              select p).SingleOrDefault();

            if (orderDetailChange != null && sizeChange != null)
            {
                orderDetailChange.active = 0;
                sizeChange.quantity += orderDetailChange.quantity;

                qlBanGiay.SaveChanges();

                return true;
            }

            return false;
        }


        // lấy danh sách order bởi admin
        // active 0 đã xóa, 1 đang được giao, 2 đã giao
        public DataTable getOrderDetails()
        {
            QuanLiBanGiayEntities qlBanGiay = new QuanLiBanGiayEntities();

            var orderDetails = from orderDetail in qlBanGiay.order_details
                               select orderDetail;

            return this.convertTableOrder(orderDetails);
        }

        public DataTable getOrderDetailsById(int id)
        {
            QuanLiBanGiayEntities qlBanGiay = new QuanLiBanGiayEntities();

            var orderDetails = from orderDetail in qlBanGiay.order_details
                               where orderDetail.id == id
                               select orderDetail;

            return this.convertTableOrder(orderDetails);
        }

        public DataTable getOrderDetailsByUser(string username)
        {
            QuanLiBanGiayEntities qlBanGiay = new QuanLiBanGiayEntities();

            var orderDetails = from orderDetail in qlBanGiay.order_details
                               where orderDetail.user.username == username
                               select orderDetail;

            return this.convertTableOrder(orderDetails);
        }

        // kiem tra lại
        public bool updateOrderDetail(int id, int orderId, int productId, int userId, int size,
                                    int quantity, double price, string payment, ref string err)
        {
            QuanLiBanGiayEntities qlBanGiay = new QuanLiBanGiayEntities();
            var orderDetailChange = (from p in qlBanGiay.order_details
                                     where p.id == id
                                     select p).SingleOrDefault();

            var sizeChange = (from p in qlBanGiay.sizes
                              where p.product_id == productId && p.number_size == size
                              select p).SingleOrDefault();
            var orderChange = (from p in qlBanGiay.orders
                               where p.id == orderId
                               select p).SingleOrDefault();

            if (orderDetailChange == null)
            {
                err = "Không tìm được đơn hàng";
                return false;
            }

            if (quantity > orderDetailChange.quantity + sizeChange.quantity)
            {
                err = "Không đủ hàng trong kho";
                return false;
            }

            int quantityNew = (int)(orderDetailChange.quantity + sizeChange.quantity - quantity);

            orderDetailChange.order_id = orderId;
            orderDetailChange.product_id = productId;
            orderDetailChange.users_id = userId;
            orderDetailChange.size = size;
            orderDetailChange.quantity = quantity;
            orderDetailChange.price = price;
            orderDetailChange.amount = quantity * price;
            orderDetailChange.payment = payment;
            orderDetailChange.date_order = DateTime.Now;
            orderDetailChange.active = 1;
            sizeChange.quantity = quantityNew;
            orderChange.amount = orderChange.amount - (orderDetailChange.quantity * price) + (quantityNew * price);

            qlBanGiay.SaveChanges();

            return true;
        }

        /* CHỨC NĂNG TÌM KIẾM THỐNG KÊ */

        public DataTable getOrderDetailsByEmail(string email)
        {
            QuanLiBanGiayEntities qlBanGiay = new QuanLiBanGiayEntities();

            var orderDetails = from orderDetail in qlBanGiay.order_details
                               where orderDetail.order.customer_email == email
                               select orderDetail;

            return this.convertTableOrder(orderDetails);
        }

        public DataTable getOrderDetailsByProduct(int productId)
        {
            QuanLiBanGiayEntities qlBanGiay = new QuanLiBanGiayEntities();

            var orderDetails = from orderDetail in qlBanGiay.order_details
                               where orderDetail.product_id == productId
                               select orderDetail;

            return this.convertTableOrder(orderDetails);
        }

        public DataTable findOrderByPhone(string phoneNumber)
        {
            QuanLiBanGiayEntities qlBanGiay = new QuanLiBanGiayEntities();

            // tìm kiếm đơn hàng bằng số điện thoại
            var order = from o in qlBanGiay.order_details
                        where o.order.customer_phone == phoneNumber
                        select o;

            return this.convertTableOrder(order);
        }

        public DataTable findOrderByDate(DateTime fromDate, DateTime toDate)
        {
            QuanLiBanGiayEntities qlBanGiay = new QuanLiBanGiayEntities();

            // tìm kiếm đơn đặt hàng trong khoảng từ ngày đến ngày để thống kê
            var order = (from o in qlBanGiay.order_details
                        where o.date_order >= fromDate && o.date_order <= toDate
                        group o by o.date_order into og
                        select new { Ngay = og.Key, TongTien = og.Sum(x => x.amount) }).ToList();


            DataTable dt = new DataTable();
            dt.Columns.Add("Ngay");
            dt.Columns.Add("TongTien", typeof(decimal));

            foreach (var o in order)
            {
                dt.Rows.Add(o.Ngay, o.TongTien);
            }

            return dt;
        }

        public DataTable findOrderByMonth(DateTime month)
        {
            QuanLiBanGiayEntities qlBanGiay = new QuanLiBanGiayEntities();

            // tìm kiếm đơn đặt hàng trong tháng để thống kê
            var order = (from o in qlBanGiay.order_details
                        where o.date_order.Month == month.Month && o.date_order.Year == month.Year
                        group o by o.date_order into og
                        select new {Ngay = og.Key , TongTien = og.Sum(x => x.amount)}).ToList();


            DataTable dt = new DataTable();
            dt.Columns.Add("Ngay");
            dt.Columns.Add("TongTien", typeof(decimal));

            foreach (var o in order)
            {
                dt.Rows.Add(o.Ngay, o.TongTien);
            }

            return dt;
        }

        public DataTable findOrderByYear(DateTime year)
        {
            QuanLiBanGiayEntities qlBanGiay = new QuanLiBanGiayEntities();

            // tìm kiếm đơn đặt hàng trong năm để thống kê
            var order = (from o in qlBanGiay.order_details
                        where o.date_order.Year == year.Year
                        group o by o.date_order into og
                        select new { Ngay = og.Key, TongTien = og.Sum(x => x.amount) }).ToList();

            DataTable dt = new DataTable();
            dt.Columns.Add("Ngay");
            dt.Columns.Add("TongTien", typeof(decimal));

            foreach(var o in order)
            {
                dt.Rows.Add(o.Ngay, o.TongTien);
            }    

            return dt;
        }

        public bool changeOrderShipment(int id, int active, ref string err)
        {
            QuanLiBanGiayEntities qlBanGiay = new QuanLiBanGiayEntities();

            var order = (from o in qlBanGiay.order_details
                        where o.id == id
                        select o).SingleOrDefault();

            if (order == null)
            {
                err = "Không tim thấy sản phẩm";
                return false;
            }    

            order.active = active;

            qlBanGiay.SaveChanges();

            return true;
        }
    }
}
