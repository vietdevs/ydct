using System;
using System.Collections.Generic;

namespace newPSG.PMS
{
    public static partial class CommonENum
    {
        #region Thanh toán
        public enum KENH_THANH_TOAN
        {
            [EnumDisplayString("Kênh thanh toán Viettel")]
            KENH_VIETTEL = 1,
            [EnumDisplayString("Kênh thanh toán Keypay")]
            KENH_KEYPAY = 2,
            [EnumDisplayString("Hình thức chuyển khoản")]
            HINH_THUC_CHUYEN_KHOAN = 3
        }
        public enum TRANG_THAI_GIAO_DICH
        {
            [EnumDisplayString("Dữ liệu đầu vào lỗi")]
            DU_LIEU_DAU_VAO_LOI = 0,
            [EnumDisplayString("Bắt đầu giao dịch")]
            BAT_DAU_GIAO_DICH = 1,
            [EnumDisplayString("Giao dịch đang chờ kết quả")]
            GIAO_DICH_DANG_CHO_KET_QUA = 2,
            [EnumDisplayString("Giao dịch thành công")]
            GIAO_DICH_THANH_CONG = 3,
            [EnumDisplayString("Giao dịch không thành công")]
            GIAO_DICH_KHONG_THANH_CONG = 4,
            [EnumDisplayString("Giao dịch bị hủy")]
            GIAO_DICH_BI_HUY = 5,
            [EnumDisplayString("Giao dịch bị từ chối")]
            GIAO_DICH_BI_TU_CHOI = 6,
        }
        public enum LOAI_THANH_TOAN
        {
            [EnumDisplayString("Thanh toán theo loại hồ sơ")]
            THANH_TOAN_THEO_LOAI_HO_SO = 0, //bao gom ca truong hop null
            [EnumDisplayString("Thanh toán theo chuyên viên nhập")]
            THANH_TOAN_THEO_CHUYEN_VIEN_NHAP = 1,
        }
        #endregion

        public static class KEYPAY_RESPONSE
        {
            //'00': 'Thành công'
            public static string THANH_CONG = "00";
            //'01': 'Đại lý không tồn tại trong hệ thống'
            public static string MA_01 = "01";
            //'02': 'Chuỗi mã hóa không hợp lệ'
            public static string MA_02 = "02";
            //'03': 'Mã giao dịch đại lý không hợp lệ'
            public static string MA_03 = "03";
            //'04': 'Không tìm thấy giao dịch trong hệ thống'
            public static string MA_04 = "04";
            //'05': 'Mã dịch vụ không hợp lệ'
            public static string MA_05 = "05";
            //'06': 'Lỗi xác nhận giao dịch: giao dịch đã được xác nhận(thành công / không thành công trước đó và không thể xác nhận lại)'
            public static string MA_06 = "00";
            //'07': 'Mã quốc gia không hợp lệ'
            public static string MA_07 = "00";
            //'08': 'Lỗi timeout xảy ra do không nhận được thông điệp trả về từ Ngân Hàng'
            public static string MA_08 = "00";
            //'09': 'Mô tả đơn hàng không hợp lệ'
            public static string MA_09 = "00";
            //'10': 'Mã đơn hàng không hợp lệ'
            public static string MA_10 = "00";
            //'11': 'Số tiền không hợp lệ'
            public static string MA_11 = "00";
            //'12': 'Phí vận chuyển không hợp lệ'
            public static string MA_12 = "00";
            //'13': 'Thuế không hợp lệ'
            public static string MA_13 = "00";
            //'14': 'Đại lý chưa được cấu hình phí'
            public static string MA_14 = "00";
            //'15': 'Sai mã Ngân hàng'
            public static string MA_15 = "00";
            //'16': 'Số tiền thanh toán của Đại lý không nằm trong khoảng cho phép'
            public static string MA_16 = "00";
            //'17': 'Tài khoản không đủ tiền'
            public static string MA_17 = "00";
            //'18': 'Khách hàng nhấn Hủy giao dịch trên giao diện Payment'
            public static string MA_18 = "00";
            //'19': 'Thời gian thanh toán không hợp lệ'
            public static string MA_19 = "00";
            //'20': 'Kiểu nhận mã OTP không hợp lệ'
            public static string MA_20 = "00";
            //'21': 'Mã OTP sai'
            public static string MA_21 = "00";
            //'25': 'Nhập sai thông tin chủ thẻ lần 1'
            public static string MA_25 = "00";
            //'26': 'Nhập sai thông tin chủ thẻ lần 2'
            public static string MA_26 = "00";
            //'27': 'Nhập sai thông tin chủ thẻ lần 3 - Thanh toán không thành công'
            public static string MA_27 = "00";
            //'30': 'Phiên bản không hợp lệ'
            public static string MA_30 = "00";
            //'31': 'Mã lệnh không hợp lệ'
            public static string MA_31 = "00";
            //'32': 'Loại tiền tệ không hợp lệ'
            public static string MA_32 = "00";
            //'33': 'Ngôn ngữ không hợp lệ'
            public static string MA_33 = "00";
            //'34': 'Thông tin thêm(desc 1) không hợp lệ'
            public static string MA_34 = "00";
            //'35': 'Thông tin thêm(desc 2) không hợp lệ'
            public static string MA_35 = "00";
            //'36': 'Thông tin thêm(desc 3) không hợp lệ'
            public static string MA_36 = "00";
            //'37': 'Thông tin thêm(desc 4) không hợp lệ'
            public static string MA_37 = "00";
            //'38': 'Thông tin thêm(desc 5) không hợp lệ'
            public static string MA_38 = "00";
            //'39': 'Chuỗi trả về - Return URL không hợp lệ'
            public static string MA_39 = "00";
            //'40': 'Loại thẻ không hợp lệ'
            public static string MA_40 = "00";
            //'41': 'Thẻ nghi vấn(thẻ đánh mất, hot card)'
            public static string MA_41 = "00";
            //'54': 'Thẻ hết hạn'
            public static string MA_54 = "00";
            //'57': 'Chưa đăng ký dịch vụ thanh toán trực tuyến'
            public static string MA_57 = "00";
            //'61': 'Quá hạn mức giao dịch trong ngày'
            public static string MA_61 = "00";
            //'62': 'Thẻ bị khóa'
            public static string MA_62 = "00";
            //'65': 'Quá hạn mức 1 lần giao dịch'
            public static string MA_65 = "00";
            //'97': 'Ngân hàng chưa sẵn sàng'
            public static string MA_97 = "00";
            //'98': 'Giao dịch không hợp lệ'
            public static string MA_98 = "00";
            //'99': 'Lỗi hệ thống'
            public static string MA_99 = "00";
        }
    }
}
