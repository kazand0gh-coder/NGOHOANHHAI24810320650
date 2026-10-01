using System;

namespace LogisticsManagement
{
    public class XeMay : PhuongTien
    {
        private int _dungTichXylanh;

        public int DungTichXylanh
        {
            get => _dungTichXylanh;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Dung tích xylanh phải lớn hơn 0 cc!");
                _dungTichXylanh = value;
            }
        }

        public XeMay(string maPT, string tenHang, int namSanXuat, decimal giaGoc, int dungTichXylanh)
            : base(maPT, tenHang, namSanXuat, giaGoc)
        {
            DungTichXylanh = dungTichXylanh;
        }

        public override decimal TinhGiaLanBanh()
        {
            decimal thueTruocBa = DungTichXylanh < 175 ? GiaGoc * 0.02m : GiaGoc * 0.05m;
            return GiaGoc + thueTruocBa;
        }

        public override string GetInfo()
        {
            return $"{base.GetInfo()} | Dung tích XL: {DungTichXylanh} cc | Giá lăn bánh: {TinhGiaLanBanh():N0} VNĐ";
        }
    }
}