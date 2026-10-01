using System;

namespace LogisticsManagement
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            QuanLyPhuongTien ql = new QuanLyPhuongTien();

            Console.WriteLine("=================== CHẠY KỊCH BẢN KIỂM THỬ (TEST CASES) ===================");

            // [TC01] Validation Năm sản xuất
            Console.WriteLine("\n[TC01] Kiểm tra Validation Năm sản xuất (Ví dụ: NamSanXuat = 1850):");
            try
            {
                OTo otoLoi = new OTo("OTo00", "Toyota", 1850, 500_000_000m, 5, 2.0);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"-> Bắt lỗi thành công! Thông báo ngoại lệ: \"{ex.Message}\"");
            }

            // [TC02] Tính Giá Lăn Bánh Ô tô 5 chỗ
            Console.WriteLine("\n[TC02] Kiểm tra Tính Giá Lăn Bánh Ô tô (5 chỗ, Giá gốc = 1,000,000,000 VNĐ):");
            OTo oto5Cho = new OTo("OT001", "Mercedes", 2023, 1_000_000_000m, 5, 2.0);
            decimal giaLanBanhOTo = oto5Cho.TinhGiaLanBanh();
            Console.WriteLine($"-> Giá lăn bánh Ô tô tính được: {giaLanBanhOTo:N0} VNĐ");

            // [TC03] Tính Giá Lăn Bánh Xe máy
            Console.WriteLine("\n[TC03] Kiểm tra Tính Giá Lăn Bánh Xe máy (150cc, Giá gốc = 50,000,000 VNĐ):");
            XeMay xeMay150cc = new XeMay("XM001", "Honda", 2024, 50_000_000m, 150);
            decimal giaLanBanhXeMay = xeMay150cc.TinhGiaLanBanh();
            Console.WriteLine($"-> Giá lăn bánh Xe máy tính được: {giaLanBanhXeMay:N0} VNĐ");

            // [TC04] Đa hình
            Console.WriteLine("\n[TC04] Kiểm tra Tính Đa hình khi nạp vào List<PhuongTien>:");
            ql.AddPhuongTien(oto5Cho);
            ql.AddPhuongTien(xeMay150cc);
            ql.DisplayAll();

            // [TC05] Tìm Giá Lăn Bánh Max
            Console.WriteLine("[TC05] Kiểm tra Tìm Phương Tiện Có Giá Lăn Bánh Cao Nhất:");
            PhuongTien ptMax = ql.FindMaxGiaLanBanh();
            if (ptMax != null)
            {
                Console.WriteLine($"-> Phương tiện đắt nhất tìm được: {ptMax.GetInfo()}");
            }

            Console.WriteLine("\n===========================================================================");
        }
    }
}
