using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLyBanGiay_Ver_Entity.Report
{
    public partial class frmStatistic : Form
    {
        public frmStatistic()
        {
            InitializeComponent();
        }

        private void frmStatistic_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'dataSetQLBH.ThongKe' table. You can move, or remove it, as needed.
            this.thongKeTableAdapter.Fill(this.dataSetQLBH.ThongKe);

            this.reportViewer3.RefreshReport();

        }

        private void btnXem_Click(object sender, EventArgs e)
        {
            if (this.txtNam.Text != "" && this.txtThang.Text != "")
            {
                thongKeDoanhSo(Int32.Parse(this.txtThang.Text), Int32.Parse(this.txtNam.Text));
            }
            else if (this.txtNam.Text != "" && this.txtThang.Text == "")
            {
                thongKeDoanhSo(Int32.Parse(this.txtNam.Text));
            }
        }

        public void thongKeDoanhSo(int month, int year)
        {
            this.thongKeTableAdapter.FillBy(this.dataSetQLBH.ThongKe, year, month);

            this.lbTitle.Text = "Báo cáo doanh số bán hàng " + month + "/ " + year;

            this.reportViewer3.RefreshReport();

        }

        public void thongKeDoanhSo(int year)
        {
            this.thongKeTableAdapter.FillByYear(this.dataSetQLBH.ThongKe, year);

            this.lbTitle.Text = "Báo cáo doanh số bán hàng năm " + year;

            this.reportViewer3.RefreshReport();

        }

    }
}
