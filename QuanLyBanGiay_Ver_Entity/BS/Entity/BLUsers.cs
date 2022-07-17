using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyBanGiay_Ver_Entity.BS
{
    public class BLUsers : IUsers
    {
        private DataTable converTable(IQueryable<user> users)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Mã Người Dùng", typeof(int));
            dt.Columns.Add("Tên Người Dùng");
            dt.Columns.Add("Email");
            // chỉ có super admin có thể xem password người dùng để reset khi cần
            dt.Columns.Add("Password");
            dt.Columns.Add("Role");

            dt.Columns.Add("Trạng thái");


            foreach (var user in users)
            {
                dt.Rows.Add(user.id,  user.name, user.email, user.password, user.role, user.active);
            }

            return dt;
        }

        private DataTable converTable(user user)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Mã Người Dùng", typeof(int));
            dt.Columns.Add("Tên Người Dùng");
            dt.Columns.Add("Email");
            // chỉ có super admin có thể xem password người dùng để reset khi cần
            dt.Columns.Add("Password");
            dt.Columns.Add("Role");
            dt.Columns.Add("Trạng thái");

            dt.Rows.Add(user.id,  user.name, user.email, user.password,user.role, user.active);

            return dt;
        }
        public bool changeUserRole(string username, string role, ref string err)
        {
            QuanLiBanGiayEntities qlBanGiay = new QuanLiBanGiayEntities();

            var user = (from u in qlBanGiay.users
                        where u.username == username
                        select u).SingleOrDefault();

            if(user != null)
            {
                user.role = role;
                qlBanGiay.SaveChanges();
            }    

            return true;
        }

        public bool deleteUser(int id, ref string err)
        {
            QuanLiBanGiayEntities qlBanGiay = new QuanLiBanGiayEntities();

            var user = (from u in qlBanGiay.users
                        where u.id == id
                        select u).SingleOrDefault();

            if (user != null)
            {
                user.active = !user.active;
                qlBanGiay.SaveChanges();
            }

            return true;
        }

        public bool deleteUserByAdmin(int id, ref string err)
        {
            QuanLiBanGiayEntities qlBanGiay = new QuanLiBanGiayEntities();

            user user = new user();
            user.id = id;

            qlBanGiay.users.Attach(user);
            qlBanGiay.users.Remove(user);

            qlBanGiay.SaveChanges();

            return true;
        }


        public DataTable getUserByUsername(string username, ref string err)
        {
            QuanLiBanGiayEntities qlBanGiay = new QuanLiBanGiayEntities();

            var user = (from u in qlBanGiay.users
                        where u.username == username && u.active == true
                        select u).SingleOrDefault();
      
            return this.converTable(user); 
        }

        // (super admin)
        public DataTable getUsers()
        {
            QuanLiBanGiayEntities qlBanGiay = new QuanLiBanGiayEntities();

            var users = from u in qlBanGiay.users
                         select u;

            return this.converTable(users);
        }

        public bool hasUserWithEmail(string email, ref string errl)
        {
            QuanLiBanGiayEntities qlBanGiay = new QuanLiBanGiayEntities();

            var user = (from u in qlBanGiay.users
                        where u.email == email && u.active == true
                        select u).SingleOrDefault();

            if(user != null)
                return true;
            return false;
        }

        public bool hasUserWithUserName(string username, ref string err)
        {
            QuanLiBanGiayEntities qlBanGiay = new QuanLiBanGiayEntities();

            var user = (from u in qlBanGiay.users
                        where u.username == username
                        select u).SingleOrDefault();

            if (user != null)
                return true;
            return false;
        }

        public bool saveUser(string username, string password, string name, string email, ref string err)
        {
            QuanLiBanGiayEntities qlBanGiay = new QuanLiBanGiayEntities();

            if(hasUserWithEmail(username,ref err) || hasUserWithEmail(email,ref err))
            {
                err = "Tài khoản đã tồn tại";
                return false;
            }    

            user user = new user();

            //user.id = id;
            user.username = username;
            user.password = password;
            user.name = name;
            user.email = email;
            user.active = true;


            qlBanGiay.users.Add(user);
            qlBanGiay.SaveChanges();

            return true;
        }

        public bool updateUser(int id, string username, string password, string name, string email, ref string err)
        {
            QuanLiBanGiayEntities qlBanGiay = new QuanLiBanGiayEntities();

            var user = (from u in qlBanGiay.users
                        where u.id == id
                        select u).SingleOrDefault();

            if (user != null)
            {
                user.username = username;
                user.password = password;
                user.name = name;
                user.email = email;
                qlBanGiay.SaveChanges();
            }
            else
            {
                err = "Cập nhật user thất bại";
                return false;
            }    

            return true;
        }

        public bool login(string username, string password, ref string err)
        {
            QuanLiBanGiayEntities qlBanGiay = new QuanLiBanGiayEntities();

            var user = (from u in qlBanGiay.users
                        where u.username == username && u.password == password
                        select u).SingleOrDefault();

            if (user != null)
                return true;
            return false;

        }

        public DataTable getNameByID(int id)
        {
            QuanLiBanGiayEntities qlBanGiay = new QuanLiBanGiayEntities();

            var user = (from u in qlBanGiay.users
                        where u.id == id && u.active == true
                        select u).SingleOrDefault();

            return this.converTable(user);
        }
    }
}
