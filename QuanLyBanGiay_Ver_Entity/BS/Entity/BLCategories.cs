using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyBanGiay_Ver_Entity.BS
{
    public class BLCategories : ICategory
    {
        public bool deleteCategory(int id, ref string err)
        {
            QuanLiBanGiayEntities qlBanGiay = new QuanLiBanGiayEntities();

            var category = (from c in qlBanGiay.categories
                            where c.id == id
                            select c).SingleOrDefault();

            if (category != null)
            {
                category.active = !category.active;
                qlBanGiay.SaveChanges();
            }
            else
            {
                err = "Không thể xóa sản phẩm";
                return false;
            }

            return true;
        }

        public bool deleteCategoryById(int id, ref string err)
        {
            QuanLiBanGiayEntities qlBanGiay = new QuanLiBanGiayEntities();

            category category = new category();
            category.id = id;

            qlBanGiay.categories.Attach(category);
            qlBanGiay.categories.Remove(category);

            qlBanGiay.SaveChanges();

            return true;
        }

        public DataTable getAllCategories()
        {
            QuanLiBanGiayEntities qlBanGiay = new QuanLiBanGiayEntities();

            var categories = from c in qlBanGiay.categories
                             where c.active == true
                             select c;

            DataTable dt = new DataTable();
            dt.Columns.Add("Mã Danh Mục", typeof(int));
            dt.Columns.Add("Tên Danh Mục");

            foreach (var category in categories)
            {
                dt.Rows.Add(category.id, category.name);
            }

            return dt;
        }

        public DataTable getAllCategoriesByAdmin()
        {
            QuanLiBanGiayEntities qlBanGiay = new QuanLiBanGiayEntities();

            var categories = from c in qlBanGiay.categories
                             select c;

            DataTable dt = new DataTable();
            dt.Columns.Add("Mã Danh Mục", typeof(int));
            dt.Columns.Add("Tên Danh Mục");
            dt.Columns.Add("Trạng Thái");

            foreach (var category in categories)
            {
                dt.Rows.Add(category.id, category.name, category.active);
            }

            return dt;
        }


        public bool saveCategory(int id, string name, ref string err)
        {
            QuanLiBanGiayEntities qlBanGiay = new QuanLiBanGiayEntities();

            category category = new category();
            category.id = id;
            category.name = name;
            category.active = true;

            qlBanGiay.categories.Add(category);
            qlBanGiay.SaveChanges();

            return true;
        }

        public bool updateCategory(int id, string name, ref string err)
        {
            QuanLiBanGiayEntities qlBanGiay = new QuanLiBanGiayEntities();

            var category = (from c in qlBanGiay.categories
                            where c.id == id
                            select c).SingleOrDefault();

            if (category != null)
            {
                category.name = name;
                qlBanGiay.SaveChanges();

            }

            return true; 
        }
    }
}
